const localVideo = document.getElementById('localVideo');
const remoteVideo = document.getElementById('remoteVideo');
export function setLocalStream(stream) {
    console.log("setLocalStream");
    localVideo.srcObject = stream;
}

export function setRemoteStream(stream) {
    console.log("setRemoteStream");
    remoteVideo.srcObject = stream;
}