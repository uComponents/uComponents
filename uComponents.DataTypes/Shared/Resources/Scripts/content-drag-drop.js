
//No "var". It's added to window.UmbDragDrop
UmbDragDrop = (function ($) {


    var BORDER_WIDTH = 6;
    var CORNER_RADIUS = 4;

    var DRAG_COLOR_VALID = "#F36F21";
    var DRAG_COLOR_INVALID = "#F32222";

    var targets = [];

    var draggingNode = null;


    var overlays = [];
    function createDragOverlay(target) {

        if (!target.container.is(":visible")) {
            return;
        }


        var filterInfo = target.filter(draggingNode);

        filterInfo.hoverOpacity = filterInfo.hoverOpacity || 1;
        filterInfo.opacity = filterInfo.opacity || .6;

        var overlay = $("<div class='drag-overlay' style='cursor: default;position:absolute;z-index: 10000;border: " + BORDER_WIDTH + "px solid white; border-radius: " + CORNER_RADIUS + "px; -moz-border-radius: " + CORNER_RADIUS + "px'></div>");


        var text = $("<div style='color: white; white-space: nowrap; font-weight: bold;'></div>").appendTo(overlay);

        overlay.css("opacity", 0);
        //Insert overlay after the target in the DOM
        overlay.insertAfter(target.container);


        //Center the overlay over the target
        overlay.css({ width: target.container.width() + "px", height: target.container.height() + "px" });
        overlay.position({
            my: "left top",
            at: "left top",
            of: target.container, offset: "-" + BORDER_WIDTH + " -" + BORDER_WIDTH,
            collision: "none"
        });

        text.position({ my: "center", at: "center", of: overlay, collision: "none" });


        overlay.animate({ opacity: filterInfo.opacity }, { duration: 200, easing: "swing" });

        //Method to update overlay
        filterInfo.update = function (animate) {

            //Center the text in the overlay
            text.html(filterInfo.message);
            text.css("position", "absolute");
            text.position({ my: "center", at: "center", of: overlay, collision: "none" });

            var color = filterInfo.canDrop ? DRAG_COLOR_VALID : DRAG_COLOR_INVALID;
            var opacity = filterInfo.mouseOver ? filterInfo.hoverOpacity : filterInfo.opacity;

            var props = { borderColor: color, backgroundColor: color, opacity: opacity }

            overlay.stop();
            if (animate) {
                overlay.animate(props, { duration: 200, easing: "swing" });
            } else {
                overlay.css(props);
            }
        }

        filterInfo.message = filterInfo.message ? filterInfo.message : filterInfo.canDrop ? "Drop here..." : "That can't be dropped here";
        filterInfo.update(false);

        var locked = false;
        //Use this method to handle mouse movements within the drop target manually. Call "release" on the returned object when done        
        filterInfo.lock = function () {
            locked = true;

            overlay.hide();

            return {
                cancel: function () {
                    locked = false;
                    overlay.show();
                },
                release: function (drop) {
                    locked = false;
                    overlay.show();
                    if (drop) {
                        overlay.trigger("mouseup");
                    } else {
                        overlay.trigger("mouseout");
                    }
                }
            }
        };


        if (filterInfo.canDrop) {

            if (filterInfo.mousemove) {
                overlay.mousemove(function (e) {
                    filterInfo.mousemove(e, overlay);
                });
            }

            overlay.mouseup(function () {
                if (draggingNode && !locked) {
                    target.action(draggingNode);
                    destroyDragOverlays();
                }
            });

            overlay.mouseover(function (e) {
                filterInfo.mouseOver = true;
                if (draggingNode) {
                    overlay.stop().animate({ opacity: filterInfo.hoverOpacity });
                    if (filterInfo.mouseover) {
                        filterInfo.mouseover(e);
                    }
                }
            });

            overlay.mouseout(function (e) {
                filterInfo.mouseOver = true;
                if (draggingNode && !locked) {
                    overlay.stop().animate({ opacity: filterInfo.opacity });
                    if (filterInfo.mouseout) {
                        filterInfo.mouseout(e);
                    }
                }
            });
        }

        overlays.push(overlay);
    }

    function destroyDragOverlays() {
        $.each(overlays, function (i, x) {
            var overlay = $(x);
            overlay.stop().animate({ opacity: 0 }, { duration: 200, complete: function () { overlay.remove(); easing: "swing" } });
        });
        overlays = [];
    }

    return {
        //Registers an area where tree items can be dropped
        //
        //container: The element to place the "drag here" visual over
        //action: The action to perform when dropping. It's passed an Umbraco.Controls.NodeDefinition
        //filter: A function that returns if the dragged item can be dropped here {canDrop: true/false, message: string}. It's passed an Umbraco.Controls.NodeDefinition                
        register: function (container, action, filter) {
            targets.push({ container: container, action: action, filter: filter || function () { return { canDrop: true }; } });
        },

        //This method is called from the top window when drag starts or ends
        notify: function (action, info) {
            if (!UC_SETTINGS.ENABLE_DRAG_AND_DROP) {
                return;
            }
            
            if (action == "started") {
                draggingNode = info;
                $.each(targets, function (i, x) {
                    createDragOverlay(x);
                });
            } else if (action == "ended") {
                draggingNode = null;
                destroyDragOverlays();
            }
        },

        //Gets the node currently being dragged if any.
        getDraggingNode: function () {
            return draggingNode;
        }
    };
})(jQuery);

(function ($) {
    
    $(function() {

        $(".propertyItemContent, .propertypane .field").each(function () {
            var container = $(this);        
            $("a", container).each(function() {
                var href = $(this).attr("href");
                var m;
                if( href && 
                    (m = href.match(/javascript\:([^\.]+)\.LaunchPicker/)) ) {                          
                    //Sort of hacky. Extract tree type from picker url
                    var picker = eval(m[1]);
                    
                    //Issue #12697. MediaChooser uses _mediaPickerUrl
                    var url = picker._itemPickerUrl || picker._mediaPickerUrl;
                    if( url ) {
                        
                        
                        var treeType = null;
                        if( url.indexOf("mediaPicker") != -1 ) {
                            //Advanced media picker
                            treeType = "media";
                        } else {
                            var m = url.match(/treeType=([^&]+)/);                                        
                            if( m ) {
                                treeType = m[1];
                            }
                        }

                        if( treeType != null ) {                        
                            UmbDragDrop.register(container, function(info) {                        
                                picker.SaveSelection({outVal: info.umbracoItem.nodeId});
                            }, function(info) {
                                var nodeType = info.umbracoItem.nodeType;                                              

                                // This is a very unelegant way to support the MNTP's FilteredContentTree and FilteredMediaTree
                                // The problem is that this is what nodeType says                        
                                var canDrop = nodeType == treeType || nodeType.toLowerCase() == "filtered" + treeType + "tree";
                                return {canDrop: canDrop, message: canDrop ? "" : "Item's type is not allowed here" }
                            });                    
                        }
                    }
                }                
            });              
        });      
        
        if( typeof(tinymce) != "undefined" ) {            
            tinymce.onAddEditor.add(function(sender, editor) {
                editor.onInit.add(function(editor) {   
                
                    var container = $("iframe", editor.getContainer()).eq(0);     
                    var doc = editor.getBody().ownerDocument;
                    var win = doc.defaultView || doc.parentWindow; 
                    
                    var toScroll = null;
                    scrollDelta = 0;
                    var doScroll = function() {                        
                        //Scroll the editor when the cursor is at the top or bottom of the overlay
                        clearTimeout(toScroll);                           
                        var st = Math.max(0, $(doc).scrollTop() + scrollDelta);
                        $(doc).scrollTop(st);                                               
                        toScroll = setTimeout(doScroll, 50);
                    };

                    var stopScroll = function() {
                        clearTimeout(toScroll);
                        scrollDelta = 0;
                    }


                    var currentFilterInfo;

                    var handleMouseMove = null;                    

                    $(container).mouseout(function(e) {
                        handleMouseMove = null;
                        if( lock != null ) {
                            lock.release(false);
                        }
                    });

                    $("body", doc).mouseup(function(e) {
                        handleMouseMove = null;
                        if( lock != null ) {
                            lock.release(true);
                        }
                    });

                    $("body", doc).mousemove(function(e) {
                        if( handleMouseMove == "ff" ) {
                            //Firefox
                            var p = e.originalEvent.rangeParent;
                            var o = e.originalEvent.rangeOffset;
                            var range = doc.createRange();
                            range.setStart(p, o);
                            range.setEnd(p, o);
                            var sel = win.getSelection();
                            sel.removeAllRanges();
                            sel.addRange(range);                          
                        }
                    });
                                                           
                              
                    var dragInsertEnabled = null, state; //enabled / disabled     
                    var lock = null; //This is used to take over the drag and drop functionality over the editor

                    UmbDragDrop.register(container, function(info) {

                        stopScroll();

                        //Insert node
                        if( dragInsertEnabled ) {
                            info.getItem(function(item) {                                    
                                if( item.typeAlias == "Image" ) {
                                    //The dragged item is an image. Insert it
                                    //TODO: Consider umbraco_maximumDefaultImageWidth
                                        $.ajax({
                                        type: "POST",
                                        url: UC_IMAGE_SERVICE,
                                        data: '{ "mediaId": ' + item.id + ', "style": "ThumbnailPreview", "linkTarget": "_blank"}',
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function(msg) {      
                                            msg = msg.d;                              
                                            var args = { src: msg.url, width: msg.width, height: msg.height, alt: msg.alt };
                                            editor.execCommand('mceInsertContent', false, '<img id="__mce_tmp" />', { skip_undo: 1 });
                                            editor.dom.setAttribs('__mce_tmp', args);
                                            editor.dom.setAttrib('__mce_tmp', 'id', '');
                                            editor.undoManager.add();
                                        }
                                    });
                                } else {
                                    editor.execCommand('mceInsertContent', false, '<a id="__mce_tmp" />', { skip_undo: 1 });                                    

                                    $("#__mce_tmp", doc)
                                        .attr("href", "/{localLink:" + item.id + "}")
                                        .text(item.text).attr("id", "");
                                    editor.undoManager.add();
                                }
                            });                            
                        }
                    }, function(info) {
                              
                        state = null;                

                        var filterInfo = currentFilterInfo = {
                            canDrop: true, 
                            message: "Drag here to insert", hoverOpacity: .2, 
                                                        
                            mouseout: function(e) {
                                window.focus();                                
                                stopScroll();
                            },
                            mousemove: function(e, overlay) {
                                clearTimeout(toScroll);                                                                    
                                
                                var offset, x, y;     
                                                                                                       
                                if( doc.caretRangeFromPoint ) {
                                    //Chrome
                                    dragInsertEnabled = true;
                                    win.focus();                                                                                                                                      

                                    //Selection
                                    offset = container.offset();
                                    x = e.pageX - offset.left;
                                    y = e.pageY - offset.top;

                                    var range = doc.caretRangeFromPoint(x, y);
                                    var sel = win.getSelection();
                                    sel.removeAllRanges();
                                    sel.addRange(range);

                                }  else if( e.originalEvent.rangeParent ) {
                                    //Firefox

                                    offset = container.offset();
                                    x = e.pageX - offset.left;
                                    y = e.pageY - offset.top;
                                    if( x >= 0 && x <= container.width() &&
                                        y >= 0 && y <= container.height() ) {
                                        lock = filterInfo.lock();
                                    }
                                                                    
                                    dragInsertEnabled = true;
                                    handleMouseMove = "ff";

                                    win.focus();                                                                              
                                } else if( doc.body && doc.body.createTextRange ) {
                                    //IE, possibly Opera                                
                                    var r = doc.body.createTextRange();                                                                
                                    if( r.moveToPoint ) {                                        
                                        dragInsertEnabled = true;
                                        win.focus();                                                                

                                        offset = container.offset();
                                        x = e.pageX - offset.left;
                                        y = e.pageY - offset.top;                                                                                                                                               
                                    
                                        var l = filterInfo.lock();
                                        try {
                                            r.moveToPoint(x, y);                                        
                                            r.select();
                                        } catch(e) {}
                                        l.cancel();
                                    }
                                }                                                                 

                                if( dragInsertEnabled ) {
                                    offset = overlay.offset();
                                    x = e.pageX - offset.left;
                                    y = e.pageY - offset.top;

                                    var scrollMargin = 35, height = overlay.outerHeight();
                                    var yBottom = height - y;
                                    if( y < scrollMargin ) {
                                        scrollDelta = -(scrollMargin - y);
                                        doScroll();
                                    } else if( yBottom >= 0 && yBottom < scrollMargin ) {
                                        scrollDelta = scrollMargin - yBottom;
                                        doScroll();
                                    }
                                }

                                var newState = dragInsertEnabled ? "caret-insert" : "disabled";

                                if( state != newState ) {
                                    state = newState;
                                    if( state == "disabled" ) {
                                        filterInfo.message = "Sorry, your browser doesn't support this feature";
                                        filterInfo.canDrop = false;
                                        filterInfo.hoverOpacity = filterInfo.opacity = .6;
                                        filterInfo.update(false);                                                      
                                    }
                                }
                            }
                       };

                        return filterInfo;
                    });                                        
                });                
            });            
        }  
    });


})(jQuery);