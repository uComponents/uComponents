/*
    <div class="check-box-tree"
        data-auto-selection-option="0 || 1 || 2">

        <!-- ASP.NET TreeView -->


    </div>
*/

var CheckBoxTree = CheckBoxTree || (function () {

    function init(treeView) {

        // dom objects
        var div = treeView.parent('div.check-box-tree');

        // values
        var autoSelectionOption = div.data('auto-selection-option');

        // fix markup - convert spans into labels for any checkboxes (TODO: swap this to server side)
        treeView.find('span').each(function (index, element) {

            var id = jQuery(element).prev('input[type="checkbox"]:first').attr('id');
            var text = jQuery(element).text();
            
            if (typeof id != 'undefined') {                
                jQuery(element).replaceWith('<label for="' + id + '">' + text + '</label>');
            }

        });

        // hook into all checkboxes
        div.find('input[type="checkbox"]')
           .change(function () {
               checkBoxChanged(jQuery(this), autoSelectionOption);
           });
    }


    function checkBoxChanged(checkBox, autoSelectionOption) {

        var isChecked = checkBox.is(':checked');

        switch (autoSelectionOption) {

            case 0: // None
                break;

            case 1: // Ensure Ancestors

                if (isChecked) {
                    // ensure all ancestors are checked (match) / ignore descendants
                    toggleAncestors(checkBox);
                } else {
                    // ignore ancestors / ensure all descendants (match)
                    toggleDescendants(checkBox);
                }
                break;

            case 2: // Ensure Descendants                

                if (isChecked) {
                    // ignore ancestors / ensure all descendants are checked (match)
                    toggleDescendants(checkBox);

                    //expand tree to ensure all newly selected items are visible
                    expandDescendants(checkBox);

                } else {
                    // ensure ancestors are unchecked (match) / ignore descendants
                    toggleAncestors(checkBox);
                }
                break;
        }
    }

    function toggleAncestors(checkBox) {

        // walk up the tree and set all ancestors to match the current
        checkBox.parentsUntil('div.check-box-tree', 'div').each(function (index, element) {
            jQuery(element).prev('table:first').find('input[type="checkbox"]:first').attr('checked', checkBox.is(':checked'));
        });

    }

    function toggleDescendants(checkBox) {

        // walk down the tree and set all checkboxes to match the current
        checkBox.closest('table').next('div').find('input[type="checkbox"]').attr('checked', checkBox.is(':checked'));

    }

    function expandDescendants(checkBox) {

        checkBox.closest('table').parent().find('div').css('display', 'block');

        // TODO: ensure the +/- icon is also updated
    }

    return {

        init: init

    }

} ());