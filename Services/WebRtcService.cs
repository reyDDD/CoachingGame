using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
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
        private string _gameId = null!;


        public event EventHandler<(string gameId, IJSObjectReference e)>? OnRemoteStreamAcquired;
        public event EventHandler<string> StopStreams;

        public WebRtcService(IJSRuntime js, IConfiguration config)
        {
            _js = js;
            _config = config;
        }

        public async Task Join(string signalingChannel, int gameId)
        {
            if (_signalingChannel != null)
                return;

            _gameId = gameId.ToString();
            _signalingChannel = signalingChannel;
            Console.WriteLine("parent game which we trying to connect is " + _signalingChannel);

            var hub = await GetHub();
            await hub.SendAsync("join", _signalingChannel);
            _jsModule = await _js.InvokeAsync<IJSObjectReference>("import", "/js/WebRtcService.cs.js");
            _jsThis = DotNetObjectReference.Create(this);
            await _jsModule.InvokeVoidAsync("initialize", _jsThis);
        }

        public async Task<IJSObjectReference> StartLocalStream(string gameId)
        {
            if (_jsModule == null) throw new InvalidOperationException();
            var answer = await _jsModule.InvokeAsync<At>("startLocalStream", gameId);
            return answer.A;
        }

        public async Task Call()
        {
            if (_jsModule == null)
                throw new InvalidOperationException();

            var offerDescription = await _jsModule.InvokeAsync<string>("callAction", _gameId);
            await SendOffer(offerDescription, _gameId!);
        }

        public async Task Hangup()
        {
            if (_jsModule == null)
                throw new InvalidOperationException();
            await _jsModule.InvokeVoidAsync("hangupAction", _gameId);
            
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "leave", null, _gameId);
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


            hub.On<string, string, string, string>("SignalWebRtc", async (signalingChannel, type, payload, gameId) =>
            {
                if (_jsModule == null) throw new InvalidOperationException();

                if (_signalingChannel != signalingChannel)
                    return;
                switch (type)
                {
                    case "offer":
                        await _jsModule.InvokeVoidAsync("processOffer", payload, gameId);
                        break;
                    case "answer":
                        await _jsModule.InvokeVoidAsync("processAnswer", payload, gameId);
                        break;
                    case "candidate":
                        await _jsModule.InvokeVoidAsync("processCandidate", payload, gameId);
                        break;
                    case "leave":
                        StopStreams.Invoke(this, _gameId);
                        break;
                }
            });

            await hub.StartAsync();
            _hub = hub;
            return _hub;
        }

        [JSInvokable]
        public async Task SendOffer(string offer, string gameId)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "offer", offer, gameId);
        }

        [JSInvokable]
        public async Task SendAnswer(string answer, string gameId)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "answer", answer, gameId);
        }

        [JSInvokable]
        public async Task SendCandidate(string gameId, string candidate)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "candidate", candidate, gameId);
        }

        [JSInvokable]
        public async Task SetRemoteStream()
        {
            if (_jsModule == null) throw new InvalidOperationException();
            await _jsModule.InvokeAsync<At>("getRemoteStreams");
        }

        [JSInvokable]
        public async Task RemoteStreamCallBack(At removeStream)
        {
            OnRemoteStreamAcquired?.Invoke(this, (removeStream.B, removeStream.A));
            removeStream.A = null!;
            removeStream.B = String.Empty;
            removeStream = null!;
        }

        public class At
        {
            [JsonPropertyName("a")]
            public IJSObjectReference A { get; set; }
            [JsonPropertyName("b")]
            public string B { get; set; }
        }

    }
}
