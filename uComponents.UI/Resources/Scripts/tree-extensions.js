
(function ($) {
    $(function () {

        //Make configurable?
        var FOLD_TYPES = ["nodeTypes", "templates", "dataType"];
        var foldTypes = {};
        for (var i = 0; i < FOLD_TYPES.length; i++) { foldTypes[FOLD_TYPES[i]] = true; }

        var org = Umbraco.Controls.UmbracoTree;
        //Extend the trees
        Umbraco.Controls.UmbracoTree = function () {
            var options = org();

            //Don't change the content frame when navigating in tress with the keyboard
            var keyboardNav = false;



            //THIS (SLIGHTLY) REDEFINES THE BEHAVIOR OF UMBRACO
            //It's essential for the drag and drop fields to work.
            //Otherwise, you can't open the media library to drag files from it.

            //It's a bit tricky. We don't want to navigate away from the current item when we click the tray icons.
            //This is carried out by disabling onSelect while rebuildTree is executing
            //The dashboard is still shown when tray icons are double clicked (i.e. _loadedApps is empty)

            var rebuilding = false;
            var orgRebuildTree = options.rebuildTree;
            options.rebuildTree = function (app, callback) {
                rebuilding = UC_SETTINGS.ENABLE_TRAY_PEEK;
                keyboardNav = false;
                var newCallback = UC_SETTINGS.ENABLE_TRAY_PEEK ? function (lastSelected) {
                    if (callback) {
                        callback(lastSelected ||
                        //If _loadedApps is empty, it means that clearTreeCache has just been called. This occurs when the user double clicks a tray icon to show the dashboard
                        //The dashboard is shown if a null argument is provided. That's why an empty object is normally included
                            (this._loadedApps.length == 0 ? null : {}));
                    }
                } : callback;

                var r = orgRebuildTree.call(this, app, newCallback);
                rebuilding = false;

                this._tree.settings.plugins.keyboard.focus();

                return r;
            }

            var shownApp;
            var orgOnSelect = options.onSelect;
            options.onSelect = function (NODE, TREE_OBJ) {

                //Reset "passively selected" css
                $(NODE).find("a:first:visible").removeClass("passive");

                if (!keyboardNav
                    && (!rebuilding || this._loadedApps.length == 0)) {
                    //Bypass base's onSelect if 1) The user is navigating with the keyboard or 2) The tree is resumed from cache (a tray icon has been clicked)
                    shownApp = this._opts.app;
                    return orgOnSelect.call(this, NODE, TREE_OBJ);
                } else {
                    if (this._opts.app != shownApp && !keyboardNav) {
                        //Signal that the node is the selected node in this tree but not the node shown in the content frame
                        $(NODE).find("a:first:visible").addClass("passive");
                    }
                }
            }

            var orgOnChange = options.onChange;
            //Don't fire the nodeClicked event when doing keyboard navigation
            options.onChange = function (NODE, TREE_OBJ) {
                if (!keyboardNav) {
                    orgOnChange.call(this, NODE, TREE_OBJ);
                }

                //Next time change/select is called this will be false. Unless the keyboard is used again
                keyboardNav = false;
            }


            var org_configureNodes = options._configureNodes;
            options._configureNodes = function (nodes, reconfigure) {

                //Also fix items under nested bin folders
                org_configureNodes.call(this, nodes.filter("li.bin-folder").find("li"), reconfigure);

                return org_configureNodes.call(this, nodes, reconfigure);
            }


            //KEYBOARD SHORTCUTS AND FOLDER FOLDING

            var org_getInitOptions = options._getInitOptions;
            options._getInitOptions = function (initData) {

                var _this = this;

                var r = org_getInitOptions.call(this, initData);

                //Shortcuts
                r.plugins.keyboard = {
                    //tabIndex: 1,
                    onnavigation: function () {
                        keyboardNav = true;
                    },
                    customHandlers: {
                        13: function (n, w, t, e) {
                            //Act as if the user clicked the node with the mouse
                            _this._raiseEvent("nodeClicked", [n]);
                            options.onSelect(n, t);

                            e.preventDefault();
                        },
                        32: function (n, w, t, e) {

                            t.callback("onrgtclk", [n, t, e]);

                            var oldTop = -1;
                            $.shortcuts.setContext("TreeMenu", function (cues) {
                                var menu = $("#jstree-contextmenu");
                                $.each(cues, function () { $(this).css("opacity", 0.9 * menu.css("opacity")); });
                                var top = menu.offset().top;
                                if (top != oldTop) {
                                    //The menu has moved. Reposition cues
                                    oldTop = top;
                                    $.shortcuts.refreshCues(false);
                                }
                                return menu.is(":visible");
                            });
                        }
                    }
                };


                var orgOndata = r.callback.ondata;
                r.callback.ondata = function (data, tree) {

                    //Call base to inject scripts etc.
                    data = orgOndata.call(this, data, tree);

                    if (!data.length) {
                        //A saved state is loaded
                        return data;
                    }

                    var ids = [];

                    for (var i = 0; i < data.length; i++) {
                        data[i].data.attributes = data[i].data.attributes || {};
                        //*data[i].data.attributes.unselectable = "on";

                        //Load additional item info with item info service
                        try {
                            ids.push(1 * data[i].attributes.id);
                        } catch (e) { }
                    }

                    ucItemInfo.prepareRange(ids);

                    //Dot grouping in folders
                    var binID = 0;
                    var rootBin = { id: binID++, items: [], bins: {} };
                    function addToBin(item) {
                        //Adds an item to bins based on its title if their node type is to be folded                        

                        var metadata = {};
                        try {
                            eval("metadata = " + item.data.attributes["umb:nodedata"]);
                        } catch (e) { }
                        if (!foldTypes[metadata.nodeType]) {
                            //No folding for this type
                            rootBin.items.push(item);
                            return;
                        }

                        var parts = item.data.title.split(".");

                        var parent = rootBin;

                        for (var i = 0; i < parts.length; i++) {
                            if (i == parts.length - 1) {
                                item.data.title = parts[i];
                                parent.items.push(item);
                            } else {
                                var bin = parent.bins[parts[i]];
                                if (!bin) {
                                    parent.bins[parts[i]] = bin = { id: binID++, items: [], bins: {} };
                                }

                                parent = bin;
                            }
                        }
                    }

                    function getBinNodes(bin) {
                        //Gets the nodes for the bin. Sub bins are ordered by name and added first. Then items are added
                        var nodes = [];

                        var names = [];
                        for (var s in bin.bins) names.push(s);

                        for (var i = 0; i < names.length; i++) {
                            var binItem = {
                                data: {
                                    title: names[i],
                                    attributes: { "class": "sprTree sprTreeFolder" }
                                },
                                attributes: {
                                    "class": "bin-folder",
                                    "umb:type": "@folding-folder",
                                    "id": "bin_" + bin.id
                                },
                                state: "closed",
                                children: getBinNodes(bin.bins[names[i]])
                            };
                            nodes.push(binItem);
                        }
                        for (var i = 0; i < bin.items.length; i++) {
                            nodes.push(bin.items[i]);
                        }

                        return nodes;
                    }

                    //Fold the items by adding them to bins
                    for (var i = 0; i < data.length; i++) {
                        addToBin(data[i]);
                    }

                    //Return the nodes generated from the "root bin", i.e. first-level bins and items without dots in titles
                    return getBinNodes(rootBin);
                }

                return r;
            };

            return options;
        };

        for (var s in org) {
            Umbraco.Controls.UmbracoTree[s] = org[s];
        }



        if ($.tree) {

            $.tree.drag_start = function (event, info) {
                info.umbracoItem = UmbClientMgr.mainTree().getNodeDef(info.drag_node);


                info.getItem = function (success) { ucItemInfo.getItem(this.umbracoItem.nodeId, success); };

                visitFrames(function (iframe, doc, win) {
                    if (win.UmbDragDrop) {
                        win.UmbDragDrop.notify("started", info);
                    }
                });
            }

            $.tree.drag_end = function () {
                visitFrames(function (iframe, doc, win) {
                    if (win.UmbDragDrop) {
                        win.UmbDragDrop.notify("ended");
                    }
                });
            }

            //Drag and drop events in other frames

            var treeDoc = $(document);

            function framePos(frame) {
                frameEl = $(frame)[0];
                var pos = $(frameEl).offset();

                for (; ; ) {
                    var win = frameEl.ownerDocument.defaultView || frameEl.ownerDocument.parentWindow;

                    frameEl = win.frameElement;
                    if (!frameEl) {
                        break;
                    }
                    var _pos = $(frameEl).offset();
                    pos.left += _pos.left;
                    pos.top += _pos.top;
                }

                return pos;
            }


            function visitFrames(action, iframe, doc, win) {
                if (doc) {
                    action(iframe, doc, win);
                } else {
                    doc = top.document;
                }

                $("iframe", $(doc)).each(function () {
                    var frameDoc;
                    try {
                        //Bypass cross domain error
                        frameDoc = $(this).contents()[0];
                        frameDoc.body;
                    } catch (e) { }

                    if (frameDoc) {
                        visitFrames(action, this, frameDoc, frameDoc.defaultView || frameDoc.parentWindow);
                    }
                });
            }

            //var debug = $("<div style='position: absolute; top: 0; left: 0; background-color: white; color: black' />").appendTo("body");

            function probe() {

                visitFrames(function (iframe, doc, win) {
                    if (doc == document) {
                        return;
                    }
                    var $doc = $(doc);
                    if (!$doc.data("drag-and-drop")) {
                        $doc.data("drag-and-drop", true);
                        $doc.data("related-frame", iframe);
                        $doc.mousemove(function (e) {

                            if (e.target.ownerDocument == this) {
                                var pos = framePos($(this).data("related-frame"));
                                treeDoc.simulate("mousemove", { clientX: pos.left + e.clientX, clientY: pos.top + e.clientY });
                            }

                        });
                        $doc.mouseup(function (e) {
                            treeDoc.simulate("mouseup");
                        });

                    }
                });

                setTimeout(probe, 150);
            }

            probe();
        }

    });
})(jQuery);