// Audio element

window.audioElement = () => { };

var _bassFilter;
var _serviceRef;
var _sound;
var _trebleFilter;

initHowler = () => {
    if (_bassFilter != undefined) {
        return;
    }
    _bassFilter = Howler.ctx.createBiquadFilter();
    _bassFilter.type = "lowshelf";
    _bassFilter.frequency.value = 200;
    _bassFilter.gain.value = 0;

    _trebleFilter = Howler.ctx.createBiquadFilter();
    _trebleFilter.type = "highshelf";
    _trebleFilter.frequency.value = 2000;
    _trebleFilter.gain.value = 0;

    _bassFilter.connect(_trebleFilter);
}

window.audioElement.get_bass = () => {
    if (_bassFilter == undefined) {
        return 0;
    }
    return _bassFilter.gain.value;
}

window.audioElement.get_currentTime = () => {
    return _sound.seek();
};

window.audioElement.get_treble = () => {
    if (_trebleFilter == undefined) {
        return 0;
    }
    return _trebleFilter.gain.value;
}

window.audioElement.play = (trackPath) => {
    if (_sound == undefined || trackPath != _sound._src) {
        if (_sound != undefined) {
            _sound.pause();
            _sound.unload();
        }
        _sound = new Howl({
            src: [trackPath],
            onend: function () {
                _serviceRef.invokeMethodAsync('AudioEnded');
            }
        });

        initHowler();

        const gainNode = _sound._sounds[0]._node;
        gainNode.disconnect(Howler.masterGain);
        gainNode.connect(_bassFilter);
        _bassFilter.connect(_trebleFilter);
        _trebleFilter.connect(Howler.masterGain);
    }
    _sound.play();
};

window.audioElement.pause = () => {
    _sound.pause();
};

window.audioElement.set_bass = (value) => {
    _bassFilter.gain.value = value;
}

window.audioElement.set_serviceRef = (serviceRef) => {
    _serviceRef = serviceRef;
}

window.audioElement.set_currentTime = (value) => {
    if (_sound.playing()) {
        _sound.seek(value);
    }
};

window.audioElement.set_treble = (value) => {
    _trebleFilter.gain.value = value;
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
        _serviceRef.invokeMethodAsync('TimeoutExecuteFunc');
    }, time);
};