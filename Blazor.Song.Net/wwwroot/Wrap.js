// Audio element

window.audio = {
    _panNode: null,
    _bassFilter: null,
    _serviceRef: null,
    _sound: null,
    _trebleFilter: null,
    _volumeNodeL: null,
    _volumeNodeR: null,
    _canvasUpdateMark: 0,
    initHowler: function (trackPath) {
        var self = this;
        var newSound = new Howl({
            src: [trackPath],
            onend: function () {
                self._serviceRef.invokeMethodAsync('AudioEnded');
            }
        });

        if (this._bassFilter == undefined) {
            this._bassFilter = Howler.ctx.createBiquadFilter();
            this._bassFilter.type = "lowshelf";
            this._bassFilter.frequency.value = 200;
            this._bassFilter.gain.value = 0;

            this._trebleFilter = Howler.ctx.createBiquadFilter();
            this._trebleFilter.type = "highshelf";
            this._trebleFilter.frequency.value = 2000;
            this._trebleFilter.gain.value = 0;
            this._bassFilter.connect(this._trebleFilter);

            this._panNode = Howler.ctx.createStereoPanner();
            this._panNode.pan.value = 0;
            this._trebleFilter.connect(this._panNode);
        }

        const gainNode = newSound._sounds[0]._node;
        gainNode.disconnect(Howler.masterGain);
        gainNode.connect(this._bassFilter);
        this._bassFilter.connect(this._trebleFilter);
        this._trebleFilter.connect(this._panNode);
        this._panNode.connect(Howler.masterGain);

        // PRES equalizer
        this.updateCanvas();
        return newSound;
    },

    updateCanvas: function () {
        this._canvasUpdateMark++;
        const canvas = document.getElementById("equalizer");
        const canvasCtx = canvas.getContext("2d");
        const parentDiv = document.body;
        _self = this;

        var WIDTH = parentDiv.offsetWidth;
        var HEIGHT = parentDiv.offsetHeight;
        canvas.width = parentDiv.offsetWidth;
        canvas.height = parentDiv.offsetHeight;

        const audioCtx = Howler.ctx;

        var analyser = Howler.ctx.createAnalyser();
        Howler.masterGain.connect(analyser);
        analyser.connect(Howler.ctx.destination);

        analyser.fftSize = 256;
        const bufferLength = analyser.frequencyBinCount;
        const dataArray = new Uint8Array(bufferLength);

        canvasCtx.clearRect(0, 0, WIDTH, HEIGHT);

        // TODO new ResizeObserver(() => {
        //}).observe(parentDiv);

        var currentMark = this._canvasUpdateMark;
        function draw() {
            if (_self._canvasUpdateMark != currentMark) {
                return;
            }
            drawVisual = requestAnimationFrame(draw);

            analyser.getByteFrequencyData(dataArray);

            canvasCtx.fillStyle = "rgb(0, 0, 0)";
            canvasCtx.fillRect(0, 0, WIDTH, HEIGHT);
            const barWidth = (WIDTH / bufferLength) * 2.5;
            let barHeight;
            let x = 0;
            for (let i = 0; i < bufferLength; i++) {
                barHeight = dataArray[i] * 6;
                canvasCtx.fillStyle = `rgb(${barHeight / 10 + 33}, ${barHeight / 10 + 140}, ${barHeight / 10 + 124})`;
                canvasCtx.fillRect(x, HEIGHT - barHeight / 2, barWidth, barHeight);

                x += barWidth + 1;
            }
        }

        draw();
    },

    get_balance: function () {
        if (this._panNode == undefined) {
            return 0;
        }
        return this._panNode.pan.value;
    },

    get_bass: function () {
        if (this._bassFilter == undefined) {
            return 0;
        }
        return this._bassFilter.gain.value;
    },

    get_currentTime: function () {
        if (this._sound == null) {
            return 0;
        }
        return this._sound.seek();
    },

    get_treble: function () {
        if (this._trebleFilter == undefined) {
            return 0;
        }
        return this._trebleFilter.gain.value;
    },

    play: function (trackPath) {
        if (this._sound == undefined || trackPath != this._sound._src) {
            if (this._sound != undefined) {
                this._sound.pause();
                this._sound.unload();
            }
            this._sound = this.initHowler(trackPath);
        }
        this._sound.play();
    },

    pause: function () {
        this._sound.pause();
    },

    set_balance: function (value) {
        if (this._panNode == undefined) {
            return;
        }
        this._panNode.pan.value = value;
    },

    set_bass: function (value) {
        this._bassFilter.gain.value = value;
    },

    set_serviceRef: function (serviceRef) {
        this._serviceRef = serviceRef;
    },

    set_currentTime: function (value) {
        if (this._sound.playing()) {
            this._sound.seek(value);
        }
    },

    set_treble: function (value) {
        this._trebleFilter.gain.value = value;
    },

    setProgressTimeout: function (time) {
        _self = this;
        setTimeout(function () {
            _self._serviceRef.invokeMethodAsync('TimeoutExecuteFunc');
        }, time);
    }
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

window.element = () => { };

window.element.get_offsetWidth = (id) => {
    return document.getElementById(id).offsetWidth;
};