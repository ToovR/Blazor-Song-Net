export function afterWebStarted(blazor) {
    var bulmaSliderSrcipt = document.createElement('script');
    bulmaSliderSrcipt.setAttribute('src', 'lib/bulma-slider/dist/js/bulma-slider.js');
    document.head.appendChild(bulmaSliderSrcipt);

    var howlerScript = document.createElement('script');
    howlerScript.setAttribute('src', 'lib/howler/howler.core.min.js');
    document.head.appendChild(howlerScript);

    var wrapScript = document.createElement('script');
    wrapScript.setAttribute('src', 'Wrap.js');
    document.head.appendChild(wrapScript);
}