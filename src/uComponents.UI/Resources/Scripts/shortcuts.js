
(function ($) {

    if (window == top) {
        //Global shortcuts
        $.shortcuts.add(window, "CS+1", function () {            
            var tree = UmbClientMgr.mainTree()._tree;            
            if (tree) {
                window.focus(); //Bug, at least in chrome. If window.focus has been called on another window, .focus() methods will not work on elements unless their windows are focused.
                tree.settings.plugins.keyboard.focus();
            }
        }, { cue: "#treeWindow_innerContent" });


        //Used to get tray links and then tabs in the content frame in the same shortcut sequence
        function trayAndTabsHandler(n, execute) {
            var trays = $("#tray li a");
            if (n < trays.length) {
                var link = trays.eq(n);
                if (execute) {
                    link.click();
                } else {
                    return link;
                }
            }

            n -= trays.length;

            return tabsHandler(n, execute);
        };

        function tabsHandler(n, execute) {
            var contentWin, contentDoc;
            try {
                //Bypass cross domain errors
                contentWin = UmbClientMgr.contentFrame();
                contentDoc = contentWin.document;
            } catch (e) { }

            if (contentDoc) {
                var tabs = $(".tabOn a,.tabOff a", contentDoc);
                if (!tabs.length) {
                    if (n == 0) {
                        //No tabs. Just focus content frame
                        if (execute) {
                            focusPropertyPage($("body", contentDoc), contentWin);
                        } else {
                            return $(contentWin.frameElement);
                        }
                    }
                } else {
                    if (n >= 0 && n < tabs.length) {
                        var link = tabs.eq(n);
                        if (execute) {
                            link.click();

                            var tabPageID = link.parent().attr("id") + "layer";

                            //Do more than just "click" the tab. Also select the first focusable element on the tab page
                            var tabRoot = link.parent()/*li*/.parent()/*ul*/.parent()/*div.header*/.parent();
                            var tabPage = $("#" + tabPageID + " > div.tabpagescrollinglayer", contentDoc);

                            focusPropertyPage(tabPage, contentWin);
                        } else {
                            return link;
                        }
                    }
                }
            }

            return null;
        }

        $.shortcuts.add(window, "CS+[#]", function (n) { trayAndTabsHandler(n, true); }, { cue: trayAndTabsHandler, start: 1, max: 12 /* until C */ });
        $.shortcuts.add(window, "CSA+[#]", function (n) { tabsHandler(n, true); }, { cue: tabsHandler, max: 12 /* until C */ });

        $.shortcuts.add(window, "TreeMenu+[#]", "#jstree-contextmenu:visible li:visible a", {
            cuePos: { my: "left center", at: "left center" }
        });


        $.shortcuts.add(window, "CS+S", function () { $("#umbSearchField").focus() }, { cue: "#umbSearchField", enabled: function () { return $("#umbSearchField").is(":visible"); } });
        $.shortcuts.add(window, "CS+U,CS+A", "button.topBarButton:eq(1)", { final: true, fade: true });
        $.shortcuts.add(window, "CS+H", "button.topBarButton:eq(2)", { final: true });
        $.shortcuts.add(window, "CS+L", "button.topBarButton:eq(3)", { final: true });



        function focusPropertyPage(scope, contentWin) {
            var sections = $(".propertypane", scope);
            if (sections.length) {
                focusEls = $("a, :input", sections);
                //:visible is really slow in ie.
                var match = false;
                if (focusEls.length) {
                    for (var i = 0; i < focusEls.length; i++) {
                        var el = focusEls.eq(i);
                        if (el.is(":visible")) {
                            //Focus the first normal focusable element
                            contentWin.focus();
                            el.focus();
                            match = true;
                            break;
                        }
                    }
                }
                if (!match) {
                    try {
-//Try to focus the first tinyMCE editor if any
                        contentWin.tinyMCE.execCommand('mceFocus', false, $("textarea:first", sections).attr("id"));
                    } catch (e) { }
                }
            }
        }
    } else {
        //Frame specific shortcuts

        //Not working, yet: $.shortcuts.add(window, "CSA+[#]", ".menubar:visible a:visible, .menubar:visible input:visible");
    }

})(jQuery);