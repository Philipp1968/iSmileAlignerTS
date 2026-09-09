$(document).ready(function () {
    $(document).on({
        mouseenter: function () {
            var img = $(this).find('img.animate'),
                src = $(img).attr('src') + '?' + new Date().getTime(),
                animated = $(img).attr('src', src.replace('_static.gif', '_animate.gif'));
        },
        mouseleave: function () {
            var img = $(this).find('img.animate'),
                src = $(img).attr('src'),
                split = src.split('?')[0];
            var animated = $(img).attr('src', split.replace('_animate.gif', '_static.gif'));
        }
    }, '.animate-hover');
});