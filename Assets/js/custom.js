document.addEventListener('DOMContentLoaded', function () {
    var content_padding = document.getElementById('padding_header');
    var height_header = document.getElementById('header').offsetHeight;
    content_padding.style.paddingTop = height_header + 'px';
});