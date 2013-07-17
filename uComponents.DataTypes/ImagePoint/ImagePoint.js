
/*    
    <div id="body_prop_hotSpotCoordinates_ctl00">
        X <input name="ctl00$body$prop_hotSpotCoordinates$xTextBox" type="text" maxlength="4" id="body_prop_hotSpotCoordinates_xTextBox" style="width:30px;"> 
        Y <input name="ctl00$body$prop_hotSpotCoordinates$yTextBox" type="text" value="3" maxlength="4" id="body_prop_hotSpotCoordinates_yTextBox" style="width:30px;">
        <br />
        <div class="area">
            <img src="/media/1001/wp_000142.jpg" style="width:400px;height=400px;" width="400" height="400" />
            <img class="marker" />
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

        // set temp vars for setup
        var x = xTextBox.val();
        var y = yTextBox.val();
        var width = mainImage.attr('width');
        var height = mainImage.attr('height');
        
        // check to see if the marker should be rendered from the stored value
        if (jQuery.isNumeric(x) && jQuery.isNumeric(y) && x >= 0 && y >= 0 && x <= width && y <= height) {
            setPoint(xTextBox, yTextBox, markerImage, { x: x, y: y });
        }

        mainImage.click(function (event) {
            setPoint(xTextBox, yTextBox, markerImage, getCoodinates(event.clientX, event.clientY, mainImage));
        });

        markerImage.draggable({
            start: function () {
            },
            drag: function (event) {

                var coordinates = getCoodinates(event.clientX, event.clientY, mainImage);

                if (coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x <= mainImage.attr('width') && coordinates.y <= mainImage.attr('height')) {
                    setPoint(xTextBox, yTextBox, markerImage, coordinates);
                }
                
            },
            stop: function () {
            }
        });
    }

    function getCoodinates(clientX, clientY, mainImage) {

        return {
            x: Math.round(clientX - mainImage.offset().left),
            y: Math.round(clientY - mainImage.offset().top)
        };
    }

    function setPoint(xTextBox, yTextBox, markerImage, coordinates) {
        
        xTextBox.val(coordinates.x);
        yTextBox.val(coordinates.y);

        markerImage.css({
            'display': 'block',
            'left': (coordinates.x - 7) + 'px',
            'top': (coordinates.y - 7) + 'px'
        });
        
    }

    // public interface
    return {
        init: init
    };

}());