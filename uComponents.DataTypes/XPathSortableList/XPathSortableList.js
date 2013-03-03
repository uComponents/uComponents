/*
    <div id="body_prop_xPathSortableListDocuments_ct100" class="xpath-sortable-list" 
         data-min-items="1"          
         data-max-items="3"
         data-allow-duplicates="false"
         data-type="">
 
        <ul class="source-list propertypane">            
            <li class="active" data-text="ABC" data-value="1">                
                <a class="add" title="add" href="javascript:void(0);" onclick="XPathSortableList.addItem(this);">
                    ABC
                </a>
            </li>

            <li data-text="XYZ" data-value="9">                
                <a class="add" title="add" href="javascript:void(0);" onclick="XPathSortableList.addItem(this);">
                    XYZ
                </a>))
            </li>)
        </ul>

        <ul class="sortable-list propertypane">
            <li data-text="ABC" data-value="1">
                ABC)
                <a class="delete" title="remove" href="javascript:void(0);" onclick="XPathSortableList.removeItem(this);"></a>
            </li>
            <li data-text="XYZ" data-value="9">
                XYZ
                <a class="delete" title="remove" href="javascript:void(0);" onclick="XPathSortableList.removeItem(this);"></a>
            </li>)
        </ul>
            
        <input type="hidden" name="ctl00$body$prop_sQLAutoComplete$ctl03">

    </div>

----------

    <XPathSortableList Type="c66ba18e-eaf3-4cff-8a22-41b16d66a972">
        <Item Text="ABC" Value="1" />
        <Item Text="XYZ" Value="9" />
    </XPathSortableList>)


*/

var XPathSortableList = XPathSortableList || (function () {

    // public
    function init(div) {

        // dom
        var ul = div.find('ul.sortable-list');
        var hidden = div.find('input:hidden:first');
       
        // data
        var type = div.data('type');

        // make selection list sortable
        ul.sortable({
            axis: 'y',
            update: function (event, ui) {
                updateHidden(ul, hidden, type);
            }
        });
    }


    // public
    function addItem(a) {

        // dom
        var selectedLi = jQuery(a).parentsUntil('ul.source-list', 'li');
        var div = selectedLi.parents('div.xpath-sortable-list:first');

        var sortableUl = div.find('ul.sortable-list:first');
        var hidden = div.find('input:hidden:first');
       
        // data
        var type = div.data('type');
        var minItems = div.data('min-items');
        var maxItems = div.data('max-items');
        var allowDuplicates = (div.data('allow-duplicates') === 'True');
        
        
        if (allowDuplicates || sortableUl.children('li[data-value=' + selectedLi.data('value') + ']').length == 0) {

            if (!allowDuplicates) {
                selectedLi.removeClass('active');
            }

            // add li to the sortable
            sortableUl.append('<li data-text="' + selectedLi.data('text') +
                                '" data-value="' + selectedLi.data('value') + '">' +
                                selectedLi.data('text') +
                                '<a class="delete" title="remove" href="javascript:void(0);" onclick="XPathSortableList.removeItem(this);"></a>' +                                
                                '</li>');
        }

        updateHidden(sortableUl, hidden, type);
    }
    

    // public
    function removeItem(a) {
       
        // dom        
        var removedLi = jQuery(a).parentsUntil('ul.sortable-list', 'li');
        var sortableUl = removedLi.parent();
        var hidden = sortableUl.siblings('input:hidden');
        var sourceUl = sortableUl.siblings('ul.source-list');

        // data
        var type = sortableUl.parent().data('type');
        var value = removedLi.data('value');

        // re activate the matching item by value in the source list
        sourceUl.children('li[data-value=' + value + ']').addClass('active');

        // remove the <li>
        jQuery(a).parent().remove(); 

        // update the xml fragment
        updateHidden(sortableUl, hidden, type);
    }

    //// private -- re-generates the xml fragment of selected items, and stores in the hidden field    
    function updateHidden(ul, hidden, type) {

        var xml = '<XPathSortableList Type="' + type + '">';
            ul.children().each(function (index, element) {
                xml += '<Item Text="' + jQuery(element).data('text') + '" Value="' + jQuery(element).data('value') + '" />';
            });
            xml += '</XPathSortableList>';

        hidden.val(xml);
    }

    // public interface to the above methods
    return {

        init: init,
        addItem: addItem,
        removeItem: removeItem

    };

}());




