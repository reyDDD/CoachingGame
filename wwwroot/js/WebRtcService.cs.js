"use strict";
// Set up media stream constant and parameters.
const mediaStreamConstraints = {
    video: {
        //width: { min: 640, ideal: 1920, max: 1920 },
        //height: { min: 480, ideal: 1080, max: 1080 }
        width: { min: 320, ideal: 320, max: 320 },
        height: { min: 240, ideal: 240, max: 240 }
    },
    //audio: true
    audio: false
};

// Set up to exchange only video.
let offerOptions = {
    offerToReceiveVideo: 1,
    offerToReceiveAudio: 1
};

const servers = {
    iceServers: [
        {
            urls: /*"turn:coturn.myserver.com:3478",*/
                ['stun:stun1.l.google.com:19302', 'stun:stun2.l.google.com:19302'],
            //username: "username",
            //credential: "password"
        }
    ]
}

let dotNet;
let localStream;
let remoteStream = [];
let peerConnection;

let isOffering;
let isOffered;

export function initialize(dotNetRef) {
    dotNet = dotNetRef;
}


export async function toggleCamera() {
    if (localStream != null) {
        let videoTrack = localStream.getTracks().find(track => track.kind === 'video');

        if (videoTrack.enabled) {
            videoTrack.enabled = false;
            document.getElementById('camera-btn').style.backgroundColor = 'rgb(255, 80, 80)';
        }
        else {
            videoTrack.enabled = true;
            document.getElementById('camera-btn').style.backgroundColor = 'rgb(179, 102, 249, .9)';
        }
    }
}

export async function toggleMic() {
    if (localStream != null) {
        let audioTrack = localStream.getTracks().find(track => track.kind === 'audio');

        if (audioTrack.enabled) {
            audioTrack.enabled = false;
            document.getElementById('mic-btn').style.backgroundColor = 'rgb(255, 80, 80)';
        }
        else {
            audioTrack.enabled = true;
            document.getElementById('mic-btn').style.backgroundColor = 'rgb(179, 102, 249, .9)';
        }
    }
}

export async function toggleCall() {
    if (peerConnection != null) {
        document.getElementById('leave-btn').style.backgroundColor = 'rgb(255, 80, 80)';
    }
    else {
        document.getElementById('leave-btn').style.backgroundColor = 'rgb(179, 102, 249, .9)';
    }
}


export async function startLocalStream() {
    console.log("Requesting local stream.");
    localStream = await navigator.mediaDevices.getUserMedia(mediaStreamConstraints);
    return localStream;
    //return localStream.getTracks().find(track => track.kind === 'video');
}

function createPeerConnection() {
    if (peerConnection != null)
        return;

    // Create peer connections and add behavior.
    peerConnection = "hello";
    peerConnection = new RTCPeerConnection(servers);
    console.log("Created local peer connection object peerConnection.");

    //localStream.getTracks()
    //    .forEach(track =>
    //        peerConnection.addTrack(track, localStream)
    //    );

   

    peerConnection.addEventListener("icecandidate", handleConnection);
    peerConnection.addEventListener("iceconnectionstatechange", handleConnectionChange); //срабатывает
    //когда идут попытки установить соединение между двумя обменявшимися любезностями пирами
    peerConnection.addEventListener("addstream", gotRemoteMediaStream);

    //peerConnection.ontrack = (event) => gotRemoteMediaStream(event); //Обрабатывает удаленный успех
    // MediaStream, передавая поток компоненту blazor.
    // peerConnection.onicecandidate = async (event) => await handleConnection(event); //срабатывает
    // когда пользователь делает предложение и получает Ответ. Отправляет кандидатов на пиринг посредством сигнализации. 

    // Add local stream to connection and create offer to connect.
    peerConnection.addStream(localStream);
    console.log("Added local stream to peerConnection.");
}


// first flow: This client initiates call. Sequence is:
// Create offer - send to peer - receive answer - set stream
// Handles call button action: creates peer connection.
export async function callAction() {
    if (isOffered)
        return Promise.resolve();

    isOffering = true;
    console.log("Starting call.");
    createPeerConnection();

    console.log("peerConnection createOffer start.");
    let offerDescription = await peerConnection.createOffer(offerOptions);
    console.log(`Offer from peerConnection:\n${offerDescription.sdp}`);
    console.log("peerConnection setLocalDescription start.");
    await peerConnection.setLocalDescription(offerDescription); // отримуємо локальний опис і 
    // надсилаємо його за допомогою каналу сигналізації віддаленому одноранговому вузлу.

    console.log("peerConnection.setLocalDescription(offerDescription) success");
    return JSON.stringify(offerDescription);
}


// Signaling calls this once an answer has arrived from other peer. Once
// setRemoteDescription is called, the addStream event trigger on the connection.
export async function processAnswer(descriptionText) {
    let description = JSON.parse(descriptionText);
    console.log("processAnswer: peerConnection setRemoteDescription start.");
    await peerConnection.setRemoteDescription(description);
    console.log("peerConnection.setRemoteDescription(description) success");
}

// In this flow, the user gets an offer from signaling from a peer.
// In this case, we setRemoteDescription similar to when we got the answer
// in the flow above. srd triggers addStream.
export async function processOffer(descriptionText) {
    console.log("processOffer");
    if (isOffering) return;

    createPeerConnection();
    let description = JSON.parse(descriptionText);
    console.log("peerConnection setRemoteDescription start.");
    await peerConnection.setRemoteDescription(description);

    console.log("peerConnection createAnswer start.");
    let answer = await peerConnection.createAnswer();
    //после строки выше у отправителя запроса срабатывает обработчик события peerConnection.ontrack = (event) => gotRemoteMediaStream(event);
    console.log(`Answer: ${answer.sdp}.`);
    console.log("peerConnection setLocalDescription start.");
    await peerConnection.setLocalDescription(answer);

    console.log("dotNet SendAnswer.");
    dotNet.invokeMethodAsync("SendAnswer", JSON.stringify(answer));
}

export async function processCandidate(candidateText) {
    let candidate = JSON.parse(candidateText);
    console.log("IceCandidate is " + candidate);
    console.log("processCandidate: peerConnection addIceCandidate start.");

    try {
        await peerConnection.addIceCandidate(candidate);
        console.log("addIceCandidate added.");
    } catch (err) {
        console.error(err);
    }
}

// Обрабатывает действие завершения: завершает вызов, закрывает соединения и сбрасывает одноранговые узлы.
export function hangupAction() {
    if (peerConnection) {
        console.log("Start ending call.");
        peerConnection.ontrack = null;
        peerConnection.onremovetrack = null;
        peerConnection.onremovestream = null;
        peerConnection.onicecandidate = null;
        peerConnection.oniceconnectionstatechange = null;
        peerConnection.onsignalingstatechange = null;
        peerConnection.onicegatheringstatechange = null;
        peerConnection.onnegotiationneeded = null;
        peerConnection.close();
        peerConnection = null;
        console.log("Ending call.");
    }
}


// Handles remote MediaStream success by handing the stream to the blazor component.
async function gotRemoteMediaStream(event) {

    const mediaStream = event.stream;
    console.log(mediaStream);
    remoteStream.push(mediaStream);

    //remoteStream = new MediaStream();
    //event.streams[1].getTracks().forEach((track) => {
    //    remoteStream.addTrack(track);
    //});

    //console.log(remoteStream);
    await dotNet.invokeMethodAsync("SetRemoteStream");
    console.log("Remote peer connection received remote stream.");
}


export function getRemoteStream() {
    return remoteStream;
}

// Sends candidates to peer through signaling.
async function handleConnection(event) {
    const iceCandidate = event.candidate;

    if (iceCandidate) {
        await dotNet.invokeMethodAsync("SendCandidate", JSON.stringify(iceCandidate));

        console.log(`peerConnection ICE candidate:${iceCandidate.candidate}.`);
    }
}

// Logs changes to the connection state.
function handleConnectionChange(event) {
    const peerConnectionForLogging = event.target;
    console.log("ICE state change event: ", event);
    console.log(`peerConnection ICE state: ${peerConnectionForLogging.iceConnectionState}.`);

    /* hideBlockRemoteVideo();*/
}


//export function hideBlockRemoteVideo() {
//    if (peerConnection.iceConnectionState == 'disconnected') {
//        console.log('hideBlockRemoteVideo');
//        dotNet.invokeMethodAsync("HideBlockRemoteVideo");
//    }
//}


