
var ucItemInfo = (function ($) {

    var knownItems = {};


    function updateKnownItems(data) {
        for (var i = 0; i < data.length; i++) {
            knownItems[data[i].id] = data[i];
            var c = successHandlers[data[i].id];
            if (c) {
                for (var j = 0; j < c.length; j++) {
                    c[j](data[i]);
                }
                successHandlers[data[i].id] = null;
            }
        }
    }

    function request(methodName, data, success) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: UC_ITEM_INFO_SERVICE + "/" + methodName,
            data: data,
            dataType: "json",
            success: function (data) {
                updateKnownItems(data);
                if (success) {
                    success(data);
                }
            }
        });
    }

    //Success handler functions for id's: successHandlers[id] = [handler1, handler2, ..., handlerN]
    var successHandlers = {};

    //Timeout to collect ids to load
    var toLoad;
    //The ids to load by the toLoad
    var loadIds = {};

    return {
        prepareChildren: function (parentID, success) {
            request("children", "{parentID: " + parentID + "}", success);
        },

        prepareRange: function (range, success) {
            request("range", "{ids: [" + range.join(",") + "]}", success);
        },

        getItem: function (id, success) {
            var item = knownItems[id];
            if (item) {
                success(item);
            } else {
                var c = successHandlers[id] = successHandlers[id] || [];
                c.push(success);
                clearTimeout(toLoad);

                //Load the id after a timeout of 1 ms, to allow ids from subsequent calls to be included in the server request
                loadIds[id] = true;
                
                toLoad = setTimeout(function () {
                    var ids = [];
                    for (var id in loadIds) {
                        ids.push(id);
                    }
                    ucItemInfo.prepareRange(ids);
                    ids = {};
                }, 1);
            }
        }
    }

})(jQuery);