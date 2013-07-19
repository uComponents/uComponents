
/*    
    <div id="" class="image-point">
        X <input type="text" class="x" />
        Y <input type="text" class="y" />
        <div class="area">
            <img src="" width="" height="" class="main" />
            <ing src="" class="marker" />
        </div>
    </div>
*/

var ImagePoint = ImagePoint || (function () {

    function init(div) {
        
        // used to group dom objects for this specific Image Point instance
        var context = {
            'xTextBox': div.children('input.x:first'),
            'yTextBox': div.children('input.y:first'),
            'mainImage' : div.find('img.main:first'),
            'markerImage': div.find('img.marker:first')
        };

        // set point from saved data
        setPoint(context);

        context.mainImage
            .click(function (event) {
                setPoint(context, getCoodinates(context, event));
            })
            .bind('dragstart', function () {
                return false;
            })
            .bind('contextmenu', function () {
                //setPoint(context, {'x' : -1, 'y': -1});
                return false;
            });

        context.markerImage
            .draggable({
                drag: function (event) {
                    setPoint(context, getCoodinates(context, event));
                }
            })
            .bind('contextmenu', function () {
                return false;
            });

        context.xTextBox.change(function () {

            if (!context.yTextBox.val()) {
                context.yTextBox.val(0);
            }

            setPoint(context);
        });

        context.yTextBox.change(function () {

            if (!context.xTextBox.val()) {
                context.xTextBox.val(0);
            }

            setPoint(context);
        });
    }

    /**
    * Calculates the X, Y values based on the mouse event, and the main image
    * @param {object} context Object conaining all DOM elements specific to an Image Point instance . 
    * @param {object} event The mouse event
    */
    function getCoodinates(context, event) {

        return {
            'x': Math.round(event.clientX - context.mainImage.offset().left),
            'y': Math.round(event.clientY - context.mainImage.offset().top)
        };
    }

    /**
    * Sets (or removes) the marker position on the image, and updates the textbox values.
    * @param {object} context Object conaining all DOM elements specific to an Image Point instance .
    * @param {object} coordinates X and Y valaues of point to set, if not supplied, then the point is set from the data in the x,y textboxes
    */
    function setPoint(context, coordinates) {

        // if coordinates not supplied, then use values from the textboxes
        if (!coordinates) {
            coordinates = { 
                'x': parseInt(context.xTextBox.val()), 
                'y': parseInt(context.yTextBox.val())
            };
        }

        if (coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x <= context.mainImage.attr('width') && coordinates.y <= context.mainImage.attr('height')) {

            context.xTextBox.val(coordinates.x);
            context.yTextBox.val(coordinates.y);

            context.markerImage.css({
                'display': 'block',
                'left': (coordinates.x - 7) + 'px',
                'top': (coordinates.y - 7) + 'px'
            });

        } else { // outside bounds
            
            context.xTextBox.val('');
            context.yTextBox.val('');

            context.markerImage.hide();
        }
    }

    // public interface
    return {
        init: init
    };

}());