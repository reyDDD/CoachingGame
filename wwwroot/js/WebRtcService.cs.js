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
let localGameId = 0;
let localStream = null;
let remoteStreams = new Map();
let peerConnections = new Map();

let isOffering = false;
let isOffered = false;

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
    if (peerConnections != null) {
        document.getElementById('leave-btn').style.backgroundColor = 'rgb(255, 80, 80)';
    }
    else {
        document.getElementById('leave-btn').style.backgroundColor = 'rgb(179, 102, 249, .9)';
    }
}


export async function startLocalStream(gameId) {
    console.log("Requesting local stream.");
    localGameId = gameId;
    localStream = await navigator.mediaDevices.getUserMedia(mediaStreamConstraints);
    getPeerConnection(gameId);
    return localStream;
}

function createPeerConnection(gameId) {
    if (peerConnections.has(gameId)) {
        console.log(`Local stream to peerConnection for game Id ${gameId} was created. Using exist`);
        return peerConnections.get(gameId);
    }

    // Create peer connections and add behavior.
    console.log(`start create peerConnections for id ${gameId}.`);

    let peerConnection = new RTCPeerConnection(servers);
    console.log(`Created local peer connection object peerConnection for game id ${gameId}.`);


    peerConnection.addEventListener("icecandidate", (event) => { handleConnection(event, gameId) });
    peerConnection.addEventListener("iceconnectionstatechange", (event) => { handleConnectionChange(event, gameId) }); //срабатывает
    //когда идут попытки установить соединение между двумя обменявшимися любезностями пирами
    peerConnection.addEventListener("addstream", (event) => { gotRemoteMediaStream(event, gameId) }); // https://www.mediaevent.de/javascript/add-event-listener-arguments.html

    //peerConnection.ontrack = (event) => gotRemoteMediaStream(event); //Обрабатывает удаленный успех
    // MediaStream, передавая поток компоненту blazor.
    // peerConnection.onicecandidate = async (event) => await handleConnection(event); //срабатывает
    // когда пользователь делает предложение и получает Ответ. Отправляет кандидатов на пиринг посредством сигнализации.

    // Add local stream to connection and create offer to connect.
    peerConnection.addStream(localStream);
    peerConnections.set(gameId, peerConnection);
    console.log(`Added local stream to peerConnection for game Id ${gameId}.`);
    return peerConnection;
}

export async function getLocalStream() {
    return localStream;
}

// first flow: This client initiates call. Sequence is:
// Create offer - send to peer - receive answer - set stream
// Handles call button action: creates peer connection.
export async function callAction(gameId) {
    //if (isOffered)
    //    return Promise.resolve();

    /*isOffering = true;*/
    console.log(`Starting call for game Id ${gameId}.`);
    let peerConnection = createPeerConnection(gameId);
    let offerDescription = await peerConnection.createOffer(offerOptions);
    console.log(`Offer with game ID ${gameId} from peerConnections:\n${offerDescription.sdp}`);
    console.log(`peerConnections setLocalDescription start for game ID ${gameId}.`);
    await peerConnection.setLocalDescription(offerDescription); // отримуємо локальний опис і 
    // надсилаємо його за допомогою каналу сигналізації віддаленому одноранговому вузлу.

    console.log(`peerConnections.setLocalDescription(offerDescription) success for game ID ${gameId}.`);
    return JSON.stringify(offerDescription);
}


// In this flow, the user gets an offer from signaling from a peer.
// In this case, we setRemoteDescription similar to when we got the answer
// in the flow above. srd triggers addStream.
export async function processOffer(descriptionText, gameId) {
    console.log(`processOffer for game ID ${gameId}`);
    /*  if (isOffering) return;*/

    //createPeerConnection();
    let description = JSON.parse(descriptionText);
    console.log(`peerConnections setRemoteDescription start for game ID ${gameId}`);

    let peerConn = getPeerConnection(gameId);

    await peerConn.setRemoteDescription(description);

    console.log(`peerConnections createAnswer start  for game ID ${gameId}`);
    let answer = await peerConn.createAnswer();
    //после строки выше у отправителя запроса срабатывает обработчик события peerConnections.ontrack = (event) => gotRemoteMediaStream(event);
    console.log(`Answer  for game ID ${gameId}: ${answer.sdp}.`);
    console.log(`peerConnections setLocalDescription start for game ID ${gameId}`);
    await peerConn.setLocalDescription(answer);

    peerConnections.set(gameId, peerConn);

    console.log(`dotNet SendAnswer for game ID ${gameId}`);
    dotNet.invokeMethodAsync("SendAnswer", JSON.stringify(answer), gameId); //TODO: а так работать будет?
}


// Signaling calls this once an answer has arrived from other peer. Once
// setRemoteDescription is called, the addStream event trigger on the connection.
export async function processAnswer(descriptionText, gameId) {
    let description = JSON.parse(descriptionText);
    console.log(`processAnswer: peerConnections setRemoteDescription start for game ID ${gameId}.`);

    let peerConnection = getPeerConnection(gameId);

    await peerConnection.setRemoteDescription(description);
    peerConnections.set(gameId, peerConnection);
    console.log(`peerConnections.setRemoteDescription(description) success for game ID ${gameId}`);
}

export async function processCandidate(candidateText, gameId) {
    let candidate = JSON.parse(candidateText);
    console.log(`IceCandidate for game ID ${gameId} is ${candidate}`);
    console.log(`processCandidate: peerConnections addIceCandidate start for game ID ${gameId} and localGameId ${localGameId}`);

    let peerConnection = getPeerConnection(gameId);

    try {
        await peerConnection.addIceCandidate(candidate);
        console.log(`addIceCandidate added for game ID ${gameId} and localGameId ${localGameId}`);
        peerConnections.set(gameId, peerConnection);
    } catch (err) {
        console.error(err);
    }
}

// Обрабатывает действие завершения: завершает вызов, закрывает соединения и сбрасывает одноранговые узлы.
export function hangupAction(gameId) {

    let peerConnection = getPeerConnection(gameId);

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
        peerConnections = null;
        console.log(`Ending call for game ID ${gameId}`);
    }
}


// Handles remote MediaStream success by handing the stream to the blazor component.
async function gotRemoteMediaStream(event, gameId) {

    const mediaStream = event.stream;
    console.log(`${mediaStream} for game ID ${gameId}`);
    remoteStreams.set(gameId, mediaStream);

    await dotNet.invokeMethodAsync("SetRemoteStream");
    console.log("Remote peer connection received remote stream.");
}

export async function getRemoteStreams() {
    let streamsIdForDelete = new Array();

    for (const [key, value] of remoteStreams) {
        var data = {
            a: DotNet.createJSObjectReference(value),
            b: key
        }
        await dotNet.invokeMethodAsync("RemoteStreamCallBack", data);
        streamsIdForDelete.push(key);
    }

    for (let streamId of streamsIdForDelete) {
        remoteStreams.delete(streamId);
    }
}

// Sends candidates to peer through signaling.
async function handleConnection(event, gameId) {
    const iceCandidate = event.candidate;

    if (iceCandidate) {
        await dotNet.invokeMethodAsync("SendCandidate", gameId, JSON.stringify(iceCandidate));

        console.log(`peerConnections ICE candidate:${iceCandidate.candidate} for game ID ${gameId}`);
    }
}

// Logs changes to the connection state.
function handleConnectionChange(event, gameId) {
    const peerConnectionForLogging = event.target;
    console.log(`peerConnections ICE state: ${peerConnectionForLogging.iceConnectionState} for game with id ${gameId}.`);

    /* hideBlockRemoteVideo();*/
}


function getPeerConnection(gameId) {
    let peerConnection = "hello";
    try {
        if (peerConnections.get(gameId) === undefined) {
            peerConnection = createPeerConnection(gameId);
        }
        else {
            peerConnection = peerConnections.get(gameId);
        }
    } catch (err) {
        console.error(err);
    }
    return peerConnection;
}

//export function hideBlockRemoteVideo() {
//    if (peerConnections.iceConnectionState == 'disconnected') {
//        console.log('hideBlockRemoteVideo');
//        dotNet.invokeMethodAsync("HideBlockRemoteVideo");
//    }
//}


