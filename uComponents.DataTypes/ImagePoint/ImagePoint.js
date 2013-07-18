
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
        
        // set point from saved data
        setPoint(xTextBox, yTextBox, width, height, { 'x': x, 'y': y }, markerImage);

        mainImage.click(function (event) {
            setPoint(xTextBox, yTextBox, width, height, getCoodinates(event, mainImage), markerImage);
        });

        markerImage.draggable({
            start: function () {
            },
            drag: function (event) {
                setPoint(xTextBox, yTextBox, width, height, getCoodinates(event, mainImage), markerImage);                
            },
            stop: function () {
            }
        });
    }

    function getCoodinates(event, mainImage) {

        return {
            'x': Math.round(event.clientX - mainImage.offset().left),
            'y': Math.round(event.clientY - mainImage.offset().top)
        };
    }

    function setPoint(xTextBox, yTextBox, width, height, coordinates, markerImage) {
                        
        if (coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x <= width && coordinates.y <= height) {

            xTextBox.val(coordinates.x);
            yTextBox.val(coordinates.y);

            markerImage.css({
                'display': 'block',
                'left': (coordinates.x - 7) + 'px',
                'top': (coordinates.y - 7) + 'px'
            });

        } else { // outside bounds
            
            xTextBox.val('');
            yTextBox.val('');

            markerImage.hide();
        }
    }

    // public interface
    return {
        init: init
    };

}());