// Audio element

window.audioElement = () => { };

window.audioElement.play = (id) => {
    document.getElementById(id).play();
};

window.audioElement.pause = (id) => {
    document.getElementById(id).pause();
};

window.audioElement.load = (id) => {
    document.getElementById(id).load();
};

window.audioElement.get_currentTime = (id) => {
    return document.getElementById(id).currentTime;
};

window.audioElement.set_currentTime = (id, value) => {
    document.getElementById(id).currentTime = value;
};

window.audioElement.onended = (id) => {
    document.getElementById(id).onended = function () {
        DotNet.invokeMethod('Blazor.Song.Net.Client', 'AudioEnded');
    };
};

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
        DotNet.invokeMethod('Blazor.Song.Net.Client', 'ExecuteTimeoutFunc');
    }, time);
};

