using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using TamboliyaLibrary.Models;

namespace Tamboliya.Services
{
    public class WebRtcService
    {
        private readonly IJSRuntime _js;
        private IJSObjectReference? _jsModule;
        private DotNetObjectReference<WebRtcService>? _jsThis;
        private HubConnection? _hub;
        private string? _signalingChannel;
        private readonly IConfiguration _config;


        public event EventHandler<IJSObjectReference>? OnRemoteStreamAcquired;

        public WebRtcService(IJSRuntime js, IConfiguration config)
        {
            _js = js;
            _config = config;

        }

        public async Task Join(string signalingChannel)
        {
            if (_signalingChannel != null)
                return;

            _signalingChannel = signalingChannel;
            Console.WriteLine("_signalingChannel is" + _signalingChannel);

            var hub = await GetHub();
            await hub.SendAsync("join", _signalingChannel);
            _jsModule = await _js.InvokeAsync<IJSObjectReference>("import", "/js/WebRtcService.cs.js");
            _jsThis = DotNetObjectReference.Create(this);
            await _jsModule.InvokeVoidAsync("initialize", _jsThis);
        }

        public async Task<IJSObjectReference> StartLocalStream(string gameId)
        {
            if (_jsModule == null) throw new InvalidOperationException();
            var stream = await _jsModule.InvokeAsync<IJSObjectReference>("startLocalStream", gameId);
            return stream;
        }

        public async Task Call()
        {
            if (_jsModule == null)
                throw new InvalidOperationException();

            var offerDescription = await _jsModule.InvokeAsync<string>("callAction");
            await SendOffer(offerDescription);
        }

        public async Task Hangup()
        {
            if (_jsModule == null)
                throw new InvalidOperationException();
            await _jsModule.InvokeVoidAsync("hangupAction");
            _signalingChannel = null;
        }

        public async Task ToggleCamera()
        {
            if (_jsModule == null)
                throw new InvalidOperationException();
            await _jsModule.InvokeVoidAsync("toggleCamera");
        }

        public async Task ToggleMic()
        {
            if (_jsModule == null)
                throw new InvalidOperationException();
            await _jsModule.InvokeVoidAsync("toggleMic");
        }

        public async Task ToggleCall()
        {
            if (_jsModule == null)
                throw new InvalidOperationException();
            await _jsModule.InvokeVoidAsync("toggleCall");
        }

        private async Task<HubConnection> GetHub()
        {

            if (_hub != null)
                return _hub;

            var chatHubUrl = _config.GetSection(SolutionPathes.HubUrl).Get<string>()!;
            var hub = new HubConnectionBuilder()
                .WithUrl(chatHubUrl)
                .Build();

            hub.On<string, string, string>("SignalWebRtc", async (signalingChannel, type, payload) =>
            {
                if (_jsModule == null) throw new InvalidOperationException();

                if (_signalingChannel != signalingChannel)
                    return;
                switch (type)
                {
                    case "offer":
                        await _jsModule.InvokeVoidAsync("processOffer", payload);
                        break;
                    case "answer":
                        await _jsModule.InvokeVoidAsync("processAnswer", payload);
                        break;
                    case "candidate":
                        await _jsModule.InvokeVoidAsync("processCandidate", payload);
                        break;
                }
            });

            await hub.StartAsync();
            _hub = hub;
            return _hub;
        }

        [JSInvokable]
        public async Task SendOffer(string offer)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "offer", offer);
        }

        [JSInvokable]
        public async Task SendAnswer(string answer)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "answer", answer);
        }

        [JSInvokable]
        public async Task SendCandidate(string candidate)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "candidate", candidate);
        }

        [JSInvokable]
        public async Task SetRemoteStream()
        {
            if (_jsModule == null) throw new InvalidOperationException();
            var stream = await _jsModule.InvokeAsync<IJSObjectReference>("getRemoteStream");
            OnRemoteStreamAcquired?.Invoke(this, stream);
        }


        //[JSInvokable]
        //public async Task HideBlockRemoteVideo()
        //{
        //    var _module = await _js.InvokeAsync<IJSObjectReference>(
        //       "import", "./Shared/Rtc.razor.js");
        //    await _module.InvokeVoidAsync("hideBlockRemoteVideo");
        //}
    }
}
