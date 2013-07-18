
/*    
    <div id="" class="image-point">
        X <input type="text" class="x" />
        Y <input type="text" class="y" />
        <br />
        <div class="area">
            <img src="" width="" height="" class="main" />
            <ing src="" class="marker" />
        </div>
    </div>
*/

var ImagePoint = ImagePoint || (function () {

    function init(div) {

        // find other dom objects
        var xTextBox = div.find('input.x:first');
        var yTextBox = div.find('input.y:first');
        var areaDiv = div.find('div.area:first');
        var mainImage = areaDiv.find('img.main:first');
        var markerImage = areaDiv.find('img.marker:first');

        // set vars
        var x = parseInt(xTextBox.val());
        var y = parseInt(yTextBox.val());
        var width = mainImage.attr('width');
        var height = mainImage.attr('height');
        
        // obj used to group params for setPoint call
        var context = {
            'xTextBox': xTextBox,
            'yTextBox': yTextBox,
            'width': width,
            'height': height,
            'markerImage': markerImage
        };

        // set point from saved data
        setPoint(context, { 'x': x, 'y': y });

        mainImage.click(function (event) {
            setPoint(context, getCoodinates(event, mainImage));
        });

        markerImage.draggable({
            start: function () {
            },
            drag: function (event) {
                setPoint(context, getCoodinates(event, mainImage));
            },
            stop: function () {
            }
        });        

        xTextBox.change(function () {
            if (!yTextBox.val()) {
                yTextBox.val(0);
            }

            setPoint(context, { 'x': parseInt(xTextBox.val()), 'y': parseInt(yTextBox.val()) });
        });

        yTextBox.change(function () {
            if (!xTextBox.val()) {
                xTextBox.val(0);
            }

            setPoint(context, { 'x': parseInt(xTextBox.val()), 'y': parseInt(yTextBox.val()) });
        });
    }

    function getCoodinates(event, mainImage) {

        return {
            'x': Math.round(event.clientX - mainImage.offset().left),
            'y': Math.round(event.clientY - mainImage.offset().top)
        };
    }

    function setPoint(context, coordinates) {

        if (coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x <= context.width && coordinates.y <= context.height) {

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