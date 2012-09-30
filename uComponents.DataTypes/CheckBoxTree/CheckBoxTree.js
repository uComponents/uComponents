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

    return {

        init: init

    }

} ());