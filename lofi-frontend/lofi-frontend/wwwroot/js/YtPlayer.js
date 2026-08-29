let player;
let dotNetHelper;

window.initializeYouTubePlayer = (elementId, videoId, dotNetRef) => {
    isPlayerReady = false;
    dotNetHelper = dotNetRef;
    player = new YT.Player(elementId, {
        height: '100%',
        width: '100%',
        videoId: videoId,
        playerVars: {
            'playsinline': 1,
            'controls': 0,
            'disablekb': 1,
            'fs': 0,
            'rel': 0,
            'mute': 0,
        },
        events: {
            onReady: () => {
                const iframe = document.getElementById(elementId).querySelector('iframe');
                if (iframe) {
                    iframe.setAttribute('title', 'Youtube music player');
                }
            },
            onStateChange: (event) => {
                if (dotNetHelper) {
                    dotNetHelper.invokeMethodAsync('OnPlayerStateChanged', event.data);
                }
            }
        }
    });
};

// Global commands Blazor can trigger
window.playYouTubeVideo = () => { if (player) player.playVideo(); };
window.stopYouTubeVideo = () => { if (player) player.pauseVideo(); };
window.resetSeeker = () => { if (player) player.seekTo(0); };
window.loadVideoById = (id) => {
    if (player) { player.loadVideoById(id, 0, "large") }
};
