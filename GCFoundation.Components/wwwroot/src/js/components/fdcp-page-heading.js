document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll('.fdcp-page-heading-has-bg[data-bg-src]').forEach(function (el) {
        var bgEl = el.querySelector('.fdcp-page-heading-bg');
        var src = el.getAttribute('data-bg-src');
        if (src && bgEl) {
            bgEl.style.backgroundImage = 'url(' + src + ')';
        }
    });
});