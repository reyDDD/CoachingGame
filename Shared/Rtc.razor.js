let localVideoId = null;
const remoteVideoContainer = document.getElementById('streams__container');

export function setLocalStream(stream, gameId) {
    try {
        document.getElementById(`remoteVideo-${gameId}`);
        let video = document.getElementById(`remoteVideo-${gameId}`);
        if (video === null) {
            let player = `<div class="video__container video-player" id="user-container-${gameId}"><video class="video-player" id="remoteVideo-${gameId}" autoplay playsinline></video></div>`;
            remoteVideoContainer.insertAdjacentHTML("beforeend", player);
            video = document.getElementById(`remoteVideo-${gameId}`);
            video.style.width = '200px';
            video.style.height = '200px';
            video.style.borderRadius = '50%';
            video.style.objectFit = 'cover';
            video.style.display = 'flex';
            video.style.justifyContent = 'center';
            video.style.alignItems = 'center';
            video.style.border = '2px solid #b366f9';
            video.style.cursor = 'pointer';
            video.style.overflow = 'hidden';
            video.srcObject = stream;
        }
        else {
            video.srcObject = stream;
        }
        console.log("setLocalStream");
        localVideoId = gameId;
    }
    catch (err) {
        console.error(err);
    }
}

export function setRemoteStream(stream) {
    for (let i = 0; i < stream.length; i++) {
        try {
            document.getElementById(`remoteVideo-${stream[i].id}`);
            let video = document.getElementById(`remoteVideo-${stream[i].id}`);
            if (video === null) {
                let videoBlock = `<div class="video__container video-player" id="user-container-${stream[i].id}" style="width: 200px; height: 200px; border-radius: 50%; object-fit: cover; display: flex; justify-content: center; align-items: center; border: 2px solid #b366f9; cursor: pointer; overflow: hidden;"><video class="video-player" id="remoteVideo-${stream[i].id}" autoplay playsinline></video></div>`;
                remoteVideoContainer.insertAdjacentHTML("beforeend", videoBlock);
                video = document.getElementById(`remoteVideo-${stream[i].id}`);
                video.srcObject = stream[i];
            }
            else {
                video.srcObject = stream[i];
            }
            console.log(`setRemoteStream for ${stream[i].id}`);
        }
        catch (err) {
            console.error(err);
        }
    }
}

export function stopLocalStream() {
    //if (localVideo.srcObject) {
    //    localVideo.srcObject.getTracks().forEach((track) => track.stop());
    //    console.log("stop local stream");
    //}
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
