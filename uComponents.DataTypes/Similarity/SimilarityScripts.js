$(function () {

    var hiddenField = $('.similarity').find("input[type='hidden']");
    var tooltipAjaxUrl = 'plugins/uComponents/MultiNodePicker/CustomTreeService.asmx/GetNodeInfo';
    var umbPath = '/umbraco';
    $('.rightNode li').click(function () {
        var nodeId = $(this).attr('rel');
        //only add if not already there
        if (hiddenField.val().indexOf(nodeId) == -1) {
            var selectedId = hiddenField.val() + nodeId + ",";
            hiddenField.val(selectedId);
            var jNode = $(this).clone();
            jNode.unbind('click');
            jNode.find(".noSpr").removeAttr("style");
            var closeLink = $('<a class="close" rel="' + nodeId + '" title="Remove" href="javascript:void(0);"></a>');
            closeLink.click(function () {
                closeLink.remove();
                StorePickedNodes(hiddenField, $('.ulSelected'), nodeId);
            });
            //wire click event to close
            jNode.appendTo($('.ulSelected'));
            closeLink.insertBefore(jNode.find("a"));
            $('.ulSelected').sortable('refresh');
        }
        else {
            alert("Already added");
        }
    });

    $('.ulSelected li a.close').click(function () {
        var itemToRemoveId = $(this).attr('rel');
        $(this).remove();
        StorePickedNodes(hiddenField, $('.ulSelected'), itemToRemoveId);
    });

    //create a sortable, drag/drop list
    $('.ulSelected').sortable({
        stop: function (event, ui) { StorePickedNodes(hiddenField, $(this), null); },
        items: 'li'
    });

    //used to create the tooltip nicked from mmntp code
    var tooltipOptions = {
        tip: "#SimilarityTooltip",
        effect: "fade",
        predelay: 0,
        position: 'center left',
        relative: true,
        offset: [30, 0],
        onShow: function () {
            //get the id of the item being queried
            var id = this.getTrigger().next().find("li").attr("rel");
            $.ajax({
                type: "POST",
                data: "{\"id\": " + id + "}",
                dataType: "json",
                url: tooltipAjaxUrl,
                contentType: "application/json; charset=UTF-8",
                success: function (data) {
                    var newLocation = umbPath + "/editContent.aspx?id=" + data.d.Id;
                    var h = $("<a href='" + newLocation + "'>[edit]</a><h5>ID: " + data.d.Id + "</h5><p><b>Path:</b> " + data.d.Path + "</p><p><i>" + data.d.PathAsNames + "</i></p>");
                    h.click(function () {

                        if (!confirm("Are you sure you want to navigate away from this page?\n\nYou may have unsaved changes.\n\nPress OK to continue or Cancel to stay on the current page.")) {
                            return false;
                        }

                        //this is a VERY dodgy work around for deep linking between sections and pages
                        var iframe = UmbClientMgr.mainWindow().jQuery("#deepLinkScriptFrame");
                        if (iframe.length == 0) {
                            var html = "<html><head><script type='text/javascript'>"
                                            + "this.window.top.delayedNavigate = function(url, app) { "
                                            + "  if (UmbClientMgr.historyManager().getCurrent() == app) {"
                                            + "    UmbClientMgr.contentFrame(url);"
                                            + "  }"
                                            + "  else {"
                                            + "    var origContentFrameFunc = UmbClientMgr.contentFrame;"
                                            + "    var newContentFrameFunc = function (location) {"
                                            + "       UmbClientMgr.contentFrame = origContentFrameFunc;"
                                            + "       origContentFrameFunc.call(this, url);"
                                            + "    };"
                                            + "    UmbClientMgr.contentFrame = newContentFrameFunc;"
                                            + "    UmbClientMgr.mainTree()._loadedApps['tree_' + app] = null;"
                                            + "    UmbClientMgr.mainTree().setActiveTreeType(app);"
                                            + "    UmbClientMgr.mainWindow().location.hash = '#' + app   ; "
                                            + "  }"
                                            + "};"
                                            + "</script></head><body></body></html>";
                            iframe = UmbClientMgr.mainWindow().jQuery("<iframe id='deepLinkScriptFrame'>")
                                            .append(html)
                                            .hide()
                                            .css("width", "0px")
                                            .css("height", "0px");
                            UmbClientMgr.mainWindow().jQuery("body").append(iframe);
                        }

                        UmbClientMgr.mainWindow().delayedNavigate(newLocation, treeType);

                        return false;
                    });
                    throbber.hide().next().html("").append(h).show();
                },
                error: function (data) {
                    alert("Error!" + data.d.Message);
                }
            });
        },
        onBeforeShow: function (ev, pos) {
            tooltip = this.getTip();
            //move the tooltip just before the trigger so that it's relatively placed
            this.getTrigger().before(tooltip);
            throbber = tooltip.find(".throbber");
            throbber.show().next().hide();
        },
        events: {
            def: 'click, mouseleave'
        }
    };

    //hidden field to store nodes selected also handles sort
    function StorePickedNodes(hiddenField, rightCol, nodeId) {
        if (!hiddenField || !rightCol)
            return;
        var val = "";
        rightCol.find("li").each(function () {
            if ($(this).attr("rel") != nodeId) {
                val += $(this).attr("rel") + ",";
            }
            else {
                if (nodeId != null) {
                    $(this).remove();
                }
            }
        });
        hiddenField.val(val);
    }
    //add the tooltips        
    $('a.info').tooltip(tooltipOptions);
});