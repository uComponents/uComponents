/**
* Shortcut plugin
*
* Copyright (c) 2010 Niels Kühnel (niels.kuhnel@eksponent.com / @nielskuhnel)
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*
*/


(function ($) {

    $.shortcuts = (function () {

        if (parent && parent.jQuery && parent.jQuery.shortcuts) {
            return parent.jQuery.shortcuts;
        }

        var shortcuts = [];

        var modalWindow = null;

        var currentShortcut = "", mask = "", keyMask = "", context = "";
        var toShortcut = null, toPollFunction = null;
        var currentWindow = window;

        function containsWindow(parent, child) {
            var w = child;
            while (w && w != w.parent) {
                if (w == parent) {
                    return true;
                }
                w = w.parent;
            }
            return false;
        }

        function handlerEnabled(info, index) {
            if (!info.enabled || info.enabled(index)) {
                var scopeWindow = null;

                if (modalWindow != null && $(modalWindow.frameElement).is(":visible")) {
                    if (!containsWindow(modalWindow, info.window[0])) {
                        //The shortcut is outside the modal window
                        return false;
                    }
                }

                if (info.scope == "this" && info.window[0] != currentWindow) {
                    //The current window is not the shortcut's
                    return false;
                }

                if (info.scope == "nest" && !containsWindow(info.window[0], currentWindow)) {
                    //The current window is not contained within the shortcut's
                    return false;
                }

                return true;
            }
        }


        //This is used to ensure that nothing happens when the user releases the modifier keys after using a shortcut
        var passiveKeyups = {};


        //The current keys pressed.
        var keysPressed = {};

        var toHideCues;

        function keydownHandler(e) {

            //The user pressed a key down. This means that modifiers from a previous shortcut should be considered again
            passiveKeyups = {};
            keysPressed[e.which] = true;

            clearTimeout(toHideCues);
            toHideCues = setTimeout(function() {
                keyMask = "";
                hideCues(false);
                processMask();
            }, 800);

            var w = e.which;

            if (!processMask(e)) {
                clearTimeout(toShortcut);

                var prefix = mask + "+";

                var index = 0;

                var key = String.fromCharCode(w);
                var info = shortcuts[prefix + key];
                if (!info) {
                    var n = w >= 49 && w <= 57 ? w - 49 :
                        (w >= 65 && w <= 90 ? w - 65 + 10 - 1 : null);

                    if (n !== null) {
                        info = shortcuts[prefix + "[#]"];
                        if (info) {
                            n -= info.start;
                            if (n < 0 || n > info.max - info.start) {
                                // This range doesn't go until Z to make room  for other shortcuts...                                
                                info = null;
                            } else {
                                // A range
                                index = n;
                            }
                        }
                    }
                }

                if (info) {
                    if (handlerEnabled(info)) {

                        var cueEl = info._cueEls[index];
                        if (cueEl) {
                            highlightCue(cueEl, !info.final || info.fade == true);
                        }

                        setFirstLevelShortcut("");
                        if (info.final) {
                            keyMask = "";

                            hideCues(info.fade);

                            for (var code in keysPressed) {
                                //Also, ensure that nothing happens when modifier keys are released                                
                                passiveKeyups[code] = true;
                            }
                            processMask();

                        } else {
                            updateCues(true);
                        }
                        info.handler(index, e);

                        e.preventDefault();
                        e.stopPropagation();
                    }
                } else if (keyMask && !currentShortcut) {
                    //Only add composite shortcuts for something that starts with a (keyboard) mask (keyMask!="") and only in one level (currentShortcut=="")
                    var firstLevelPrefix = prefix + key + ",";
                    for (var s in shortcuts) {

                        if (s.indexOf(firstLevelPrefix) == 0 && handlerEnabled(shortcuts[s])) {
                            //Don't do anything if no two level shortcuts are defined
                            setFirstLevelShortcut(prefix + key + ",");
                            updateCues();

                            toShortcut = setTimeout(function () {
                                setFirstLevelShortcut("");
                            }, 2000);
                            break;
                        }
                    }
                } else {
                    setFirstLevelShortcut("");
                    updateCues();
                }

            }

        }

        function keyupHandler(e) {
            delete keysPressed[e.which];
            if (passiveKeyups[e.which]) {
                delete passiveKeyups[e.which];
            } else {
                processMask(e);
            }
        }

        //Sets the prefix for composite shortcuts
        function setFirstLevelShortcut(s) {
            currentShortcut = s;
            processMask();
        }

        function processMask(e, ctx) {

            var somethingChanged = false;

            if (e) {
                var w = e.which;
                keyMask = (e.ctrlKey ? "C" : "") + (e.shiftKey ? "S" : "") + (e.altKey ? "A" : "");
                somethingChanged = (w == 16 /*SHIFT*/ || w == 17 /*CTRL*/ || w == 18 /*ALT*/);
            }

            if (arguments.length > 1) {
                context = ctx || "";
                somethingChanged = true;
            }

            mask = currentShortcut + keyMask + context;
            if (somethingChanged) {
                //Some of the control keys or the context have changed
                //Use a little delay to avoid flimsy behavior when Alt Gr is used     
                toRefreshCues = setTimeout(function () { updateCues(); }, 10);
                return true;
            }

            return false;
        }


        var toRefreshCues = null;
        var cues = [];
        var cueMask;
        function updateCues(refresh, fade) {

            if (!refresh) {
                //This prevents very quick refreshes when a modifier key is held down (toRefreshCues still refreshes)
                if (cueMask == mask) {
                    return;
                }
            }

            cueMask = mask;
            var prefix = mask + "+";
            var cue;
            //Hide visible cues that are no longer enabled
            for (var i = 0; i < cues.length; i++) {
                cue = cues[i];
                if (cue.info.shortcut.indexOf(prefix) != 0 || !handlerEnabled(cue.info, cue.index)) {
                    removeCue(cue, true);
                }
            }

            //Create new cues            
            for (var s in shortcuts) {
                if (s.indexOf(prefix) == 0) {
                    info = shortcuts[s];
                    if (info.cue) {
                        var key = s.substring(prefix.length);
                        if (key.indexOf(",") == -1) {
                            // Don't show composite shortcuts. (Maybe the first key should be shown?)                            

                            var sequence = key == "[#]";
                            for (var i = sequence ? info.start : 0; i < (sequence ? info.max : 1); i++) {
                                //Only repeat once for single shortcuts (to reuse code), and info.max times for sequences                                

                                var index = sequence ? i - info.start : i;

                                cue = info._cueEls[index];
                                if (!cue) {
                                    var cueTarget = info.cue(index);
                                    if (!cueTarget) {
                                        //If there's no cue target it means that we're done
                                        break;
                                    } else {
                                        if (handlerEnabled(info, index)) {
                                            cue = createCue(sequence ? getIndexLabel(i) : key, cueTarget, info.cuePos, fade); //Only create cue if it doesn't exist
                                        }
                                    }
                                }
                                if (cue) {
                                    cue.index = index;
                                    cue.info = info;
                                    info._cueEls[index] = cue;
                                }
                            }
                        }
                    }
                }
            }

            //Refresh cues 4 times pr second
            clearTimeout(toRefreshCues);
            toRefreshCues = setTimeout(function () { updateCues(true, false); }, 250);
        }

        function hideCues(fade) {
            clearTimeout(toRefreshCues);
            while (cues.length) {
                removeCue(cues.pop(), fade);
            }
        }

        function createCue(key, el, pos, fade) {
            if (!$(el).length) {
                return false;
            } else {

                var doc = $(el[0].ownerDocument);
                if (doc.length) {
                    if (!doc.data("has-shortcuts-style")) {
                        $("head", doc).append($("<style type='text/css'>span.shortcut-cue{" + $.shortcuts.style + "} span.picked-shortcut-cue {" + $.shortcuts.pickedStyle + "}</style>"));
                        doc.data("has-shortcuts-style", true);
                    }
                }

                var cue = $("<span class='shortcut-cue'></span>").text(key);
                if (pos.append) {
                    cue.appendTo(el);
                } else {
                    cue.appendTo(el.parents("body"));
                }

                cue.position({ my: pos.my || "center", at: pos.at || "center", of: $(el) });
                if (fade !== false) {
                    cue.css("opacity", 0).animate({ opacity: $.shortcuts.cueOpacity }, { duration: 250 });
                } else {
                    cue.css("opacity", $.shortcuts.cueOpacity);
                }

                cues.push(cue);
                return cue;
            }
        }

        function removeCue(el, fade) {
            try { // try..catch to avoid "permission denied" in IE                        
                if (!$(el).hasClass("picked-shortcut-cue")) {
                    delete el.info._cueEls[el.index];
                    if (fade && !document.all) {
                        el.animate({ opacity: 0 }, { duration: 150, complete: function () { el.remove(); } });
                    } else {
                        el.remove();
                    }
                }
            } catch (e) {
                delete el.info._cueEls[el.index];
            }
        }


        //Used to indicate that a shortcut was pressed
        function highlightCue(cue, animate) {
            if (animate !== false) {
                var grow = 8;
                $(cue).addClass("picked-shortcut-cue");

                $(cue).animate({
                    opacity: 0,
                    top: "-=" + grow,
                    left: "-=" + grow,
                    fontSize: "+=" + grow,
                    paddingTop: "+=" + Math.floor(grow / 2),
                    height: "+=" + (grow + Math.ceil(grow / 2)),
                    width: "+=" + grow * 2
                }, {
                    duration: 300,
                    complete: function () {
                        clearTimeout(toRefreshCues);
                        delete cue.info._cueEls[cue.index];
                        cue.remove();
                        toRefreshCues = setTimeout(function () { updateCues(true, true); }, 300);
                    }
                });
            }
        }


        //Converts a number to a label 1, 2, ... , 9, A, B, ... , Z
        function getIndexLabel(i) {
            return "" + (i < 9 ? i + 1 : String.fromCharCode(i + 65 - 9));
        }

        function getWindow(doc) {
            return doc.window || doc.defaultView;
        }

        function probe(doc, sub) {

            doc = doc || document;
            var $doc = $(doc);

            if (!$doc.data("shortcuts")) {
                $doc.data("shortcuts", true)
                    .keydown(function (e) { currentWindow = getWindow(this); keydownHandler(e); })
                    .keyup(function (e) { currentWindow = getWindow(this); keyupHandler(e); });



                //                $(getWindow(doc)).blur(hideOnBlur);
            }
            $("iframe", $doc).each(function () {
                try {
                    probe($(this).contents(), true);
                } catch (e) {

                }
            });

            if (!sub) {
                setTimeout(probe, 150);
            }
        }

        probe();

        return {
            //The css attributes for cues
            style: "z-index: 1001; position: absolute; background-color: #F36F21; color: White; padding: 4px; -moz-border-radius: 4px; border-radius: 4px;  font-family:Arial,Lucida Grande; font-weight: bold; font-size: .8em; min-width: 10px; text-align: center;",
            //This is the style applied to a shortcut cue that is used
            pickedStyle: "" /*Too much: "background-color: #21f000;"*/,

            //The opacity for cues
            cueOpacity: 0.9,

            modal: function (window) {
                modalWindow = window;
                $(window).unload(function () {
                    if (modalWindow == window) {
                        modalWindow = null;
                    }
                });
            },

            add: function (window, shortcut, handler, opts) {

                var $win = $(window), $doc = $(window.document);


                var jqe = typeof (handler) == "string" ? handler : null;
                opts = $.extend({
                    shortcut: shortcut,
                    enabled: null,
                    cue: jqe ? function (n) { return $(jqe, $doc).eq(n); } : null,
                    cuePos: { my: "center", at: "center", append: false },
                    max: 1000, /* Max for ranges (1,2,...,9,A,B,...,Z) */
                    start: 0, /* Start for intervals (0-indexed, i.e. zero = "1") */
                    final: false, /* Set this to true if the action creates an alert box, confirm box or similar. Otherwise the cues will stick*/
                    scope: "global" /* global (=all windows) | nest (=this window and nested ones) | this (=only this window) */
                }, opts);

                opts.window = $win;

                //Holds label elements
                opts._cueEls = [];

                if (typeof (opts["cue"]) == "string") {
                    var expr = opts["cue"];
                    opts["cue"] = function (n) { return $(expr, $doc).eq(n); };
                }

                handler = jqe ? function (n) { $(jqe, $doc).eq(n).click(); } : handler;

                opts["handler"] = handler;

                shortcuts[shortcut] = opts;

                if (!$win.data("shortcuts")) {

                    //These should go away when the window unloads
                    $win.data("shortcuts", []);
                    $win.unload(function () {
                        $.each($(this).data("shortcuts"), function (i, x) {
                            $.shortcuts.remove(x);
                        });
                    });
                }

                $win.data("shortcuts").push(shortcut);

                return $.shortcuts;
            },

            remove: function (shortcut) {
                delete shortcuts[shortcut];

                return $.shortcuts;
            },

            setContext: function (ctx, pollFunction) {
                $.shortcuts.clearContext();
                processMask(null, ctx);
                if (pollFunction) {
                    var p = function () {
                        if (!pollFunction(cues)) {
                            $.shortcuts.clearContext();
                        } else {
                            toPollFunction = setTimeout(p, 150);
                        }
                    };

                    p();
                }
                return $.shortcuts;
            },

            clearContext: function () {
                clearTimeout(toPollFunction);
                processMask(null, "");

                return $.shortcuts;
            },

            refreshCues: function (fade) {
                updateCues(true, fade);
                return $.shortcuts;
            }
        }

    })();

})(jQuery);