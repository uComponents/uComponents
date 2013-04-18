/**
* Shortcut plugin for the jquery.tree
*
* Copyright (c) 2010 Niels Kühnel (niels.kuhnel@eksponent.com / @nielskuhnel)
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*
*/



(function ($) {

    function KeyHandler(tree) {

        var FAR_LEFT = -10000;

        var searchTimeout = null;
        var search = "";

        function clearSearch() {
            search = "";
        }

        function getSelected() {
            var sel = tree.selected;
            return sel && sel.is(":visible") ? sel : null;
        }

        function onNavigation() {
            var handler = tree.settings.plugins.keyboard.onnavigation;
            if (handler) {
                handler();
            }
        }

        function select(node) {
            onNavigation();
            tree.select_branch(node);
        }

        var el = $("<input type='text' style='position:absolute;left:" + FAR_LEFT + "px'>");
        var tabIndex = tree.settings.plugins.keyboard.tabIndex;
        if (tabIndex) {
            el.attr("tabindex", tabIndex);
        }

        el.insertBefore($(tree.container));

        //TODO: Find out if this can be done better. 
        //This works, though. The problem is that some browsers scrolls the input field into view before the tree can react to events. If this happens the user ends up selecting another item than intended because the browser scrolls before the mouse click "lands".
        //The only problem with this solution is that the selected element is not scrolled into view with a keyboard triggered focus change if the mouse is over the tree. This may actually be the best behavior.
        var elY = null;
        tree.container.mouseover(function (e) { if (elY === null) elY = el.offset().top; });
        tree.container.mouseout(function (e) {
            try {
                el.offset({ top: elY, left: FAR_LEFT });
            } catch (e) { /* IE */ }
            elY = null;
        });
        //Let the input field track the mouse to avoid weird focus/scroll jump problems
        tree.container.mousemove(function (e) {
            try {
                el.offset({ top: e.pageY, left: FAR_LEFT });
            } catch (e) { /* IE */ }
        });


        tree.container.click(function (e) {
            el.focus();
            if (e) {
                e.preventDefault();
            }
        });


        el.keypress(function (e) {
            function evalNode(n) {
                var text = current && current.children("a:visible").text();
                //Trim right, and &nbsp; in ie
                text = text.replace(/[\xa0]/, " ").replace(/^\s+/, "");

                if (text && text.toLowerCase().indexOf(search) == 0) {
                    select(current);
                    return true;
                }

                return false;
            }

            search += String.fromCharCode(e.which).toLowerCase();
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(clearSearch, 500);

            var first = tree.container.find("li:visible:first"); ;
            var current = getSelected() || first;


            if (search.length == 1 || !evalNode(current)) {

                var breaker = current;
                do {
                    if ((current = tree.next(current)) == null || !current.length) {
                        //We're at last or no node so "next" node is the first one.            
                        current = first;
                    }
                    if (evalNode(current)) {
                        break;
                    }
                } while (current[0] && current[0] != breaker[0]); // INV: current != breaker => (eventually) current == breaker;                
            }
        });

        el.keydown(function (e) {

            var node = getSelected();

            function fire(keyCode) {
                switch (keyCode) {
                    case 36: /*Home*/
                        node = tree.container.find("li:visible:first");
                        select(node);
                        return true;

                    case 35: /*End*/
                        node = tree.container.find("li:visible:last");
                        select(node);
                        return true;

                    case 38: /*Up */
                        if (!node) {
                            fire(35);
                        } else {
                            node = tree.prev(node)
                            if ($("a.loading", node).length) {
                                //Skip the loading node
                                fire(38);
                            } else {
                                select(node);
                            }
                            select(node);
                        }
                        return true;

                    case 40: /*Down*/
                        if (!node) {
                            fire(36);
                        } else {
                            node = tree.next(node);
                            if ($("a.loading", node).length) {
                                //Skip the loading node
                                fire(40);
                            } else {
                                select(node);
                            }
                        }
                        return true;

                    case 33: /*Page Up*/
                        for (var i = 0; i < 10; i++) {
                            fire(38);
                        }
                        return true;

                    case 34: /*Page Down*/
                        for (var i = 0; i < 10; i++) {
                            fire(40);
                        }
                        return true;

                    case 37: /*Left*/
                        if (node) {
                            if (node.hasClass("open")) tree.close_branch(node);
                            else {
                                return tree.select_branch(tree.parent(node));
                            }
                        }
                        return true;

                    case 39: /*Right*/
                        if (node) {
                            if (node.hasClass("closed")) tree.open_branch(node);
                            else {
                                return tree.select_branch(node.find("li:eq(0)"));
                            }
                        }
                        return true;


                        /*case 13:
                        if (node.hasClass("closed")) tree.open_branch(node);
                        return true;*/
                }
            }
            if (fire(e.which)) {
                e.preventDefault();
            }

            var c = tree.settings.plugins.keyboard.customHandlers;
            if (c[e.which]) {
                c[e.which](node, e.which, tree, e);
            }
        });

        var fromTree = false;
        el.focus(function () {
            if (!fromTree) {
                tree.focus();
            }
            fromTree = false;
        });

        this.alignKeyhandler = function (target) {
            var options = { duration: 150, easing: "swing" };

            if (!target) {
                var selected = tree.hovered || getSelected();
                if (selected != null) {
                    target = selected.find("a:visible:first");
                }
            }
            if (target) {
                //TODO: Use some kind of scroll plugin to do this, or extract this as a scoll plugin (scrollIntoView or something)               

                var pos = target.offset();

                //Position the key handler (i.e. textbox) at the element
                el.offset({ top: pos.top, left: FAR_LEFT });
                //List of all elements that possibly must be scrolled to show the element
                var scrollParents = [target.scrollParent()];
                for (var i = 0; i < scrollParents.length && i < 10; i++) {

                    //Scroll into view
                    var sp = scrollParents[i];
                    if (sp[0] != document) {
                        scrollParents.push(sp.scrollParent());
                    }
                    var elementHeight = target.outerHeight();

                    var spTop = sp[0] == document ? 0 : sp.offset().top;
                    var spHeight = sp[0] == document ? $(window).height() : sp.height();
                    var scrollTop = sp.scrollTop();

                    var scrollTarget = sp[0] == document ? $("html,body") : sp;
                    var bottomMargin = 20; //Horizontal scrollbar
                    spHeight -= bottomMargin;

                    var top = pos.top - spTop + (sp[0] == document ? 0 : scrollTop);
                    if (scrollTop > top) {
                        scrollTarget.scrollTop(top); // stop().animate({ scrollTop: top }, options);
                    } else if (scrollTop != top && top + elementHeight > scrollTop + spHeight) {
                        var newTop = top + Math.min(elementHeight - spHeight, 0);
                        scrollTarget.scrollTop(newTop); // stop().animate({ scrollTop: top + Math.min(elementHeight - spHeight, 0) }, options);
                    }
                }
            }
        }

        el.blur(function () {
            if (tree.blur) {
                tree.blur();
            }
        });

        var _this = this;
        this.focus = function () {
            fromTree = true;
            el[0].focus();
        }

        this.blur = function () {
            el.blur();
        }


        this.destroy = function () {
            el.remove();
        }

        tree.settings.plugins.keyboard.focus = function () {            
            var node = getSelected();
            if (!node) {
                select(tree.container.find("li:visible:first"));
            }            
            _this.focus();
        };
    }

    if ($.tree) {
        $.extend($.tree.plugins, {
            "keyboard": {
                callbacks: {
                    oninit: function (tree) {
                        tree.keyHandler = new KeyHandler(tree);
                    },

                    onfocus: function (tree) {
                        tree.keyHandler.focus();
                    },

                    onselect: function (node, tree) {
                        //Ensure that focus isn't lost         
                        tree.keyHandler.alignKeyhandler();

                        tree.keyHandler.focus();
                    },

                    ondestroy: function (tree) {
                        tree.keyHandler.destroy();
                    }
                }
            }
        });
    }
})(jQuery);