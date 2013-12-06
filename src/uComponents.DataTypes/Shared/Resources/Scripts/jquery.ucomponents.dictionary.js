(function ($) {

    var uComponentsDictionary = {
        init: function(options) {
            return this;
        },
        getItem: function (key, fallback) {
            return $.ajax({
                type: "GET",
                url: "/Umbraco/plugins/uComponents/Shared/WebServices/DictionaryService.asmx/GetDictionaryItem?key=%22" + key + "%22&fallback=%22" + fallback + "%22",
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });
        }
    };
    
    $.fn.uComponents().dictionary = $.fn.uComponents().dictionary || function (method) {
        if (uComponentsDictionary[method]) {
            return uComponentsDictionary[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return uComponentsDictionary.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.uComponents.dictionary');
        }
    };
})(jQuery);