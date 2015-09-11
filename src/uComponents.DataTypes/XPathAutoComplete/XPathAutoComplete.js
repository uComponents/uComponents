/*
    <div class="xpath-auto-complete" 
         data-xpath-autocomplete-id="6905F13E-829D-4827-9B9F-72D5A00E7A9E" 
         data-datatype-definition-id="1056" 
         data-current-id="1051"
         data-type="Media" 
         data-min-length="2"
         data-max-items="3">
 
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
            
        <input type="text" name="ctl00$body$prop_sQLAutoComplete$ctl02" id="body_prop_sQLAutoComplete_ctl02" class="umbEditorTextField">
        <input type="hidden" name="ctl00$body$prop_sQLAutoComplete$ctl03">

    </div>

----------

    <XPathAutoComplete Type="c66ba18e-eaf3-4cff-8a22-41b16d66a972">
        <Item Text="ABC" Value="1 />
        <Item Text="XYZ" Value="9" />
    </XPathAutoComplete>
*/

var XPathAutoComplete = XPathAutoComplete || (function () {



    // public
    function init(input) {

        // dom objects
        var div = input.parent('div.xpath-auto-complete');
        var ul = div.children('ul');
        var hidden = div.children('input[type="hidden"]');

        // values
        var xPathAutoCompleteId = div.data('xpath-autocomplete-id');
        var dataTypeDefinitionId = div.data('datatype-definition-id');
        var currentId = div.data('current-id');

        var type = div.data('type');
        var minLength = div.data('min-length');
        var maxItems = div.data('max-items');
        var allowDuplicates = div.data('allow-duplicates');

        // create list items from saved xml - ignore max-items here, always add all saved items
        try {
            var xml = jQuery.parseXML(hidden.val());
            
            jQuery(xml).find('Item').each(function(index, element) {
                addItem(
                    ul,
                    {
                        label: jQuery(element).attr('Text'),
                        value: jQuery(element).attr('Value')
                    },
                    allowDuplicates
                );
            });
        } catch(err) {
            // value was not xml
        }

        // make selection list sortable
        ul.sortable({
            axis: 'y',
            update: function (event, ui) {
                updateHidden(ul, hidden, type);
            }
        });

        // TODO: prevent enter key in textbox from causing a page save so losing the focus 

        // wire up the jQuery UI AutoComplete
        input.autocomplete({
            minLength: minLength,
            source: function (request, response) {
                jQuery.ajax({
                    type: 'POST',
                    data: { autoCompleteText: request.term, selectedItems: hidden.val() },
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    url: '/Base/' + xPathAutoCompleteId + '/GetData/' + dataTypeDefinitionId + '/' + currentId,
                    dataType: 'json',
                    success: function(data) {
                        response(data);
                    }
                });
            },
            open: function (event, ui) {
                input.autocomplete('widget').width(300); // TODO: can we get at the input field from the event or ui params ?
            },
            autoFocus: true,
            focus: function (event, ui) {
                return false; // prevent the autocomplete text box from being populated with the value of the currenly highlighted item
            },
            create: function(event, ui) {
                input.autocomplete('widget').addClass("xpath-auto-complete-widget");
            },
            select: function (event, ui) {

                // only call add item if the maxItems hasn't been reached
                if (maxItems == 0 || ul.children('li').length < maxItems) {
                    addItem(ul, ui.item, allowDuplicates);
                }

                updateHidden(ul, hidden, type);
                event.target.value = ''; // remove the typed text from the autocomplete textbox
                return false; // prevent the selected items value from being put into the autocomplete textbox
            }
        });
    }

    // private -- add a new item to the end of the selected items list
    function addItem(ul, item, allowDuplicates) {

        // if duplicates are allowed or item doesn't already exist then add
        if (allowDuplicates == 'True' ||
            ul.children('li[data-value=' + item.value + ']').length == 0) {
            ul.append('<li data-text="' + item.label + '" data-value="' + item.value + '"><span>' + item.label + '</span><a class="delete" title="remove" href="javascript:void(0);" onClick="XPathAutoComplete.removeItem(this);"></a></li>');
        }
    }

    // public -- from the clicked anchor, remove it's <li> and re-generate the hidden field
    function removeItem(a) {

        var ul = jQuery(a).parentsUntil('div.xpath-auto-complete', 'ul');
        var hidden = ul.siblings('input:hidden');
        var type = ul.parent().data('type');

        jQuery(a).parent().remove(); // remove the <li>

        updateHidden(ul, hidden, type);
    }

    // private -- re-generates the xml fragment of selected items, and stores in the hidden field    
    function updateHidden(ul, hidden, type) {

        var xml = '<XPathAutoComplete Type="' + type + '">';
        ul.children().each(function (index, element) {
            xml += '<Item Text="' + jQuery(element).data('text') + '" Value="' + jQuery(element).data('value') + '" />';
        });
        xml += '</XPathAutoComplete>';

        hidden.val(xml);
    }

    // public interface to the above methods
    return {

        init: init,
        removeItem: removeItem // needs to be public, as call is made from outside the init scope (from the a tag in the selection list)
    };

} ());




