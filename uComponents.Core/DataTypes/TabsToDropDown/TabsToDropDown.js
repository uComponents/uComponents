

(function ($) {
    $.fn.moveTo = function (selector) {
        return this.each(function () {
            var cl = $(this).clone();
            $(cl).prependTo(selector);
            $(this).remove();
        });
    }
})(jQuery);


