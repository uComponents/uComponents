/*
    <div id="body_prop_sQLAutoComplete_ctl00" class="sql-auto-complete" data-sql-autocomplete-id="EE395ED1-CE6E-4417-AEEB-BCA780D3E96B" data-datatypedefinitionid="1056" data-currentid="1051" data-min-length="2">
        <ul class="propertypane ui-sortable">
            <li data-value="2320">hello@hello.com<a class="delete" title="remove" href="javascript:void(0);" onclick="SqlAutoComplete.removeItem(this)"></a></li>
        </ul>
        <input name="ctl00$body$prop_sQLAutoComplete$ctl02" type="text" class="umbEditorTextField ui-autocomplete-input" autocomplete="off" role="textbox" aria-autocomplete="list" aria-haspopup="true">
        <input type="hidden" name="ctl00$body$prop_sQLAutoComplete$ctl03" class="ui-autocomplete-input" autocomplete="off" role="textbox" aria-autocomplete="list" aria-haspopup="true">
    </div>
*/

var SqlAutoComplete = SqlAutoComplete || (function () {

    function init(div) {

        var sqlAutoCompleteId = div.data('sql-autocomplete-id');
        var dataTypeDefinitionId = div.data('datatype-definition-id');
        var currentId = div.data('current-id');
        var minLength = div.data('min-length');


        // make selection list sortable
        div.children('ul').sortable({
            axis: 'y',
            update: function (event, ui) {

                // update the hidden field
                alert('sorted');
            }
        });


        div.children('input:first').autocomplete({

            minLength: minLength,

            source: function (request, response) {
                jQuery.ajax({
                    type: 'GET',
                    dataType: 'json',
                    url: '/Base/' + sqlAutoCompleteId + '/GetData/' + dataTypeDefinitionId + '/' + currentId + '/' + encodeURI(request.term),
                    success: function (data) {
                        response(data);
                    }
                });
            },

            open: function () {
                div.children('input:first').autocomplete('widget').width(300);

            },

            autoFocus: true,

            focus: function (event, ui) {
                return false; // prevent the autocomplete text box from being populated with the value of the currenly highlighted item
            },

            select: function (event, ui) {

                // is there an li with a matching data-value attribute ?

                if (div.find('ul li[data-value=' + ui.item.value + ']').length == 0) {
                    div.children('ul').append('<li data-value="' + ui.item.value + '">' + ui.item.label + '<a class="delete" title="remove" href="javascript:void(0);" onClick="SqlAutoComplete.removeItem(this);"></a></li>');
                }

                // return empty textbox                               
                event.target.value = '';
                return false;
            }

        });



    }


    function addItem(div, item) {

        // find ul and add item

    }

    function removeItem(a) {

        // remove the li that the supplied a is within
        jQuery(a).parent().remove();

    }

    function updateHiddenField(div) {

        // find ul, and for each li, add to the hidden field

    }


    return {
        init: init,
        addItem: addItem,
        removeItem: removeItem
    };

} ());




