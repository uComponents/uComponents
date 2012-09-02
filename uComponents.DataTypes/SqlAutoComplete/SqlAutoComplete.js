/*
    <div class="sql-auto-complete" data-sql-autocomplete-id="EE395ED1-CE6E-4417-AEEB-BCA780D3E96B" data-datatype-definition-id="1056" data-current-id="1051" data-min-length="2">

        <ul class="propertypane ui-sortable">
            <li data-value="1">
                This is an item
                <a class="delete" title="remove" href="javascript:void(0);" onclick="SqlAutoComplete.removeItem(this);"></a>
            </li>
            <li data-value="7">
                Another item
                <a class="delete" title="remove" href="javascript:void(0);" onclick="SqlAutoComplete.removeItem(this);"></a>
            </li>
        </ul>
            
        <input name="ctl00$body$prop_sQLAutoComplete$ctl02" type="text" value="" id="body_prop_sQLAutoComplete_ctl02" class="umbEditorTextField ui-autocomplete-input" autocomplete="off" role="textbox" aria-autocomplete="list" aria-haspopup="true">
        <input type="hidden" name="ctl00$body$prop_sQLAutoComplete$ctl03">

    </div>
*/

var SqlAutoComplete = SqlAutoComplete || (function () {

    function init(input) {

        // dom objects
        var div = input.parent('div.sql-auto-complete');
        var ul = div.children('ul');
        //
        var hidden = div.children('input:hidden');

        // values
        var sqlAutoCompleteId = div.data('sql-autocomplete-id');
        var dataTypeDefinitionId = div.data('datatype-definition-id');
        var currentId = div.data('current-id');
        var minLength = div.data('min-length');


        // fromm values in the hidden field, create list items - do it server side or here ? here = single mechanism to create items
        createList(hidden, ul);


        // make selection list sortable
        ul.sortable({
            axis: 'y',
            update: function (event, ui) {
                updateHidden(ul, hidden);
            }
        });

        input.autocomplete({

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

            open: function (event, ui) {
                input.autocomplete('widget').width(300);

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

    // private
    function createList(hidden, ul) {

        // for each item in the hidden field create the appropriate list item

        // foreach .... createListItem(ul, text, value);
    }

    // private
    function createListItem(ul, text, value) {

    }

    // private
    function addItem(ul, item) {

        // if item doesn't already exist then add (TODO: configuration option to allow duplicates ?)
        //createListItem(ul, item.label, item.value);
        //updateHidden(ul, hidden);
    }


    function removeItem(a) {

        // walk up the dom from the a to find the div
        // from the div find the hidden field


        jQuery(a).parent().remove();
        //

    }

    // private
    function updateHidden(ul, hidden) {

        // find ul, and for each li, add to the hidden field

        // store data as an xml (or json ?) fragment - 
        // need to put all selected data (as KVP) in the hidden field, as no way of knowing how to get the label for an id wiithout configuring extra sql statements

        alert('updating hidden');

    }




    return {
        init: init,
        removeItem: removeItem // needs to be public, as call is made from outside the init scope (from the a tag in the selection list)
    };

} ());




