(function ($) {

    var uComponents = {
        version: "1.0",
        init: function (options) {
            return this;
        }
    };

    $.fn.uComponents = $.fn.uComponents || function (method) {
        if (uComponents[method]) {
            return uComponents[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return uComponents.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.uComponents');
        }
    };
    
    $.uComponents = $.uComponents || $(document).uComponents();
})(jQuery);