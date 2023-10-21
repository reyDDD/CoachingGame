const localVideo = document.getElementById('localVideo');
const remoteVideoContainer = document.getElementById('streams__container');

export function setLocalStream(stream) {
    console.log("setLocalStream");
    localVideo.srcObject = stream;
}

export function setRemoteStream(stream) {
    try {
        document.getElementById(`remoteVideo-${stream[0].id}`);
        let video = document.getElementById(`remoteVideo-${stream[0].id}`);
        if (video === null) {
            let videoBlock = `<div class="video__container video-player" id="user-container-${stream[0].id}" style="width: 200px; height: 200px; border-radius: 50%; object-fit: cover; display: flex; justify-content: center; align-items: center; border: 2px solid #b366f9; cursor: pointer; overflow: hidden;"><video class="video-player" id="remoteVideo-${stream[0].id}" autoplay playsinline></video></div>`;
            remoteVideoContainer.insertAdjacentHTML("beforeend", videoBlock);
            video = document.getElementById(`remoteVideo-${stream[0].id}`);
            video.srcObject = stream[0];
        }
        else {
            video.srcObject = stream[0];
        }
        console.log(`setRemoteStream for ${stream[0].id}`);
    }
    catch (err) {
        console.log(err);
    }
}

export function stopLocalStream() {
    if (localVideo.srcObject) {
        localVideo.srcObject.getTracks().forEach((track) => track.stop());
        console.log("stop local stream");
    }
}

export function stopRemoteStream() {
    if (remoteVideoContainer.srcObject) {
        remoteVideoContainer.srcObject.getTracks().forEach((track) => track.stop());
        console.log("stop remote streams");
    }
}

export function showBlockRemoteVideo() {
    //remoteVideoContainer.style.display = 'block';
    //localVideo.classList.add('smallFrame');
}

export function hideBlockRemoteVideo() {
    //remoteVideoContainer.style.display = 'none';
    //localVideo.classList.remove('smallFrame');
}


let displayFrame = document.getElementById('stream__box');
let videoFrames = document.getElementsByClassName('video__container');
let userIdInDisplayFrame = null;
let expandVideoFrame = (e) => {
    let child = displayFrame.children[0];
    if (child) {
        e.currentTarget.style = "height: 100% !important; width: 100 % !important; object-fit: cover; display: flex; justify-content: center; align-items: center; border: 2px solid #b366f9; border-radius: 0; cursor: pointer; overflow: hidden;";
        child.style = "width: 200px; height: 200px; object-fit: cover; display: flex; justify-content: center; align-items: center; border: 2px solid #b366f9; border-radius: 50%; cursor: pointer; overflow: hidden;";
        document.getElementById('streams__container').appendChild(child);
    }

    displayFrame.style.display = 'block';
    displayFrame.appendChild(e.currentTarget);
    userIdInDisplayFrame = e.currentTarget.id;
}

export function addEventListenerForVideoFrames() {
    for (let i = 0; videoFrames.length > i; i++) {
        videoFrames[i].addEventListener('click', expandVideoFrame)
    }
}
