// Audio element

window.audioElement = () => { };

var sound;

var bassFilter;

var trebleFilter;

initHowler = () => {
    if (bassFilter != undefined) {
        return;
    }
    bassFilter = Howler.ctx.createBiquadFilter();
    bassFilter.type = "lowshelf";
    bassFilter.frequency.value = 200;
    bassFilter.gain.value = 0;

    trebleFilter = Howler.ctx.createBiquadFilter();
    trebleFilter.type = "highshelf";
    trebleFilter.frequency.value = 2000;
    trebleFilter.gain.value = 0;

    bassFilter.connect(trebleFilter);
}

window.audioElement.get_bass = () => {
    if (bassFilter == undefined) {
        return 0;
    }
    return bassFilter.gain.value;
}

window.audioElement.get_currentTime = () => {
    return sound.seek();
};

window.audioElement.get_treble = () => {
    if (trebleFilter == undefined) {
        return 0;
    }
    return trebleFilter.gain.value;
}

window.audioElement.onended = () => {
};

window.audioElement.play = (trackPath) => {
    if (sound == undefined || trackPath != sound._src) {
        if (sound != undefined) {
            sound.pause();
            sound.unload();
        }
        sound = new Howl({
            src: [trackPath],
            onend: function () {
                DotNet.invokeMethod('Blazor.Song.Net.Client', 'AudioEnded');
            }
        });

        initHowler();

        const gainNode = sound._sounds[0]._node;
        gainNode.disconnect(Howler.masterGain);
        gainNode.connect(bassFilter);
        bassFilter.connect(trebleFilter);
        trebleFilter.connect(Howler.masterGain);
    }
    sound.play();
};

window.audioElement.pause = () => {
    sound.pause();
};

window.audioElement.set_bass = (value) => {
    bassFilter.gain.value = value;
}

window.audioElement.set_currentTime = (value) => {
    if (sound.playing()) {
        sound.seek(value);
    }
};

window.audioElement.set_treble = (value) => {
    trebleFilter.gain.value = value;
}

window.classElement = () => { };

window.classElement.updateBackgroundImage = (className, imageSource) => {
    var elements = document.getElementsByClassName(className);
    for (i = 0; i < elements.length; i++) {
        elements[i].style.backgroundImage = "url('" + imageSource + "')";
    }
};

window.cookie = () => { };

window.cookie.get = () => {
    return document.cookie;
};

window.cookie.set = (cookieValue) => {
    document.cookie = cookieValue;
};

window.document.updateTitle = (newValue) => {
    window.document.title = newValue;
};

window.element = () => { };

window.element.get_offsetWidth = (id) => {
    return document.getElementById(id).offsetWidth;
};

window.functions = () => { };

window.functions.setTimeout = (time) => {
    setTimeout(function () {
        DotNet.invokeMethodAsync('Blazor.Song.Net.Client', 'ExecuteTimeoutFunc');
    }, time);
};