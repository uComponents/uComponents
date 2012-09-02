/*
    <div class="sql-auto-complete" data-sql-autocomplete-id="EE395ED1-CE6E-4417-AEEB-BCA780D3E96B" data-datatype-definition-id="1056" data-current-id="1051" data-min-length="2">

        <ul class="propertypane">
            <li data-text="ABC" data-value="1">
                ABC
                <a class="delete" title="remove" href="javascript:void(0);" onclick="SqlAutoComplete.removeItem(this);"></a>
            </li>
            <li data-text="XYZ" data-value="9">
                XYZ
                <a class="delete" title="remove" href="javascript:void(0);" onclick="SqlAutoComplete.removeItem(this);"></a>
            </li>
        </ul>
            
        <input type="text" name="ctl00$body$prop_sQLAutoComplete$ctl02" id="body_prop_sQLAutoComplete_ctl02" class="umbEditorTextField" value="" >
        <input type="hidden" name="ctl00$body$prop_sQLAutoComplete$ctl03">

    </div>

    ----------

    <SqlAutoComplete>
        <Item Text="ABC" Value="1 />
        <Item Text="XYZ" Value="9" />
    </SqlAutoComplete>
*/

var SqlAutoComplete = SqlAutoComplete || (function () {

    // public
    function init(input) {

        // dom objects
        var div = input.parent('div.sql-auto-complete');
        var ul = div.children('ul');
        // input
        var hidden = div.children('input:hidden');

        // values
        var sqlAutoCompleteId = div.data('sql-autocomplete-id');
        var dataTypeDefinitionId = div.data('datatype-definition-id');
        var currentId = div.data('current-id');
        var minLength = div.data('min-length');

        createList(hidden, ul); // could pull method back to here ?

        // make selection list sortable
        ul.sortable({

            axis: 'y',

            update: function (event, ui) {
                updateHidden(ul, hidden);
            }

        });

        //        // on typing something into the textbox, prevent the enter key from saving the page -  NOT WORKING
        //        input.keyup(function (event) {
        //            if (event.keyCode == 13) {
        //                event.stopPropagation();
        //                return false;
        //            }
        //        });


        // setup jQuery UI AutoComplete
        input.autocomplete({

            minLength: minLength,

            source: function (request, response) {
                jQuery.ajax({
                    type: 'POST',
                    data: {
                        autoCompleteText : request.term
                    },
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    url: '/Base/' + sqlAutoCompleteId + '/GetData/' + dataTypeDefinitionId + '/' + currentId,
                    dataType: 'json',
                    success: function (data) {
                        response(data);
                    }
                });
            },

            open: function (event, ui) {

                // TODO: can we get at the input field from the event or ui params ?
                input.autocomplete('widget').width(300);

            },

            autoFocus: true,

            focus: function (event, ui) {

                // prevent the autocomplete text box from being populated with the value of the currenly highlighted item
                return false;

            },

            select: function (event, ui) {

                addItem(ul, ui.item);

                updateHidden(ul, hidden);

                // remove the typed text from the autocomplete textbox
                event.target.value = '';

                // prevent the selected items value from being put into the autocomplete textbox
                return false;
            }
        });
    }

    // private -- from the xml fragment in the hidden field, recreate the selected items list
    function createList(hidden, ul) {

        var xml = jQuery.parseXML(hidden.val());

        jQuery(xml).find('Item').each(function (index, element) {

            addItem(
                ul,
                {
                    // return an object that looks like the autocomplete item one
                    label: jQuery(element).attr('Text'),
                    value: jQuery(element).attr('Value')
                }
            );

        });

    }

    // private -- add a new item to the end of the selected items list
    function addItem(ul, item) {

        // if item doesn't already exist then add (TODO: configuration option to allow duplicates ?)
        if (ul.children('li[data-value=' + item.value + ']').length == 0) {

            ul.append('<li data-text="' + item.label + '" data-value="' + item.value + '">' + item.label + '<a class="delete" title="remove" href="javascript:void(0);" onClick="SqlAutoComplete.removeItem(this);"></a></li>');

        }
    }

    // public -- from the clicked anchor, remove it's <li> and re-generate the hidden field
    function removeItem(a) {

        var ul = jQuery(a).parentsUntil('div.sql-auto-complete', 'ul');
        var hidden = ul.siblings('input:hidden');

        // remove the <li>
        jQuery(a).parent().remove();

        updateHidden(ul, hidden);
    }

    // private -- re-generates the xml fragment of selected items, and stores in the hidden field    
    function updateHidden(ul, hidden) {

        var xml = '<SqlAutoComplete>';

        ul.children().each(function (index, element) {

            xml += '<Item Text="' + jQuery(element).data('text') + '" Value="' + jQuery(element).data('value') + '" />';

        });

        xml += '</SqlAutoComplete>';

        hidden.val(xml);
    }

    // public interface to the above methods
    return {

        init: init,
        removeItem: removeItem // needs to be public, as call is made from outside the init scope (from the a tag in the selection list)
    };

} ());




