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

export function stopLocalStream() {
    if (localVideo.srcObject) {
        localVideo.srcObject.getTracks().forEach((track) => track.stop());
        console.log("stop local stream");
    }
}

export function stopRemoteStream() {
    if (remoteVideo.srcObject) {
        remoteVideo.srcObject.getTracks().forEach((track) => track.stop());
        console.log("stop remote streams");
    }
}

export function showBlockRemoteVideo() {
    remoteVideo.style.display = 'block';
    localVideo.classList.add('smallFrame');
}

export function hideBlockRemoteVideo() {
    remoteVideo.style.display = 'none';
    localVideo.classList.remove('smallFrame');
}


let displayFrame = document.getElementById('stream__box');
let videoFrames = document.getElementsByClassName('video__container');
let userIdInDisplayFrame = null;
let expandVideoFrame = (e) => {
    let child = displayFrame.children[0];
    if (child) {
        document.getElementById('streams__container').appendChild(child);
    }

    displayFrame.style.display = 'block';
    displayFrame.appendChild(e.currentTarget);
    userIdInDisplayFrame = e.currentTarget.id;
}

for (let i = 0; videoFrames.length > i; i++) {
    videoFrames[i].addEventListener('click', expandVideoFrame)
}