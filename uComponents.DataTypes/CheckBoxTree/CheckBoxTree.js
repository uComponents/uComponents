/*
    <div class="check-box-tree"
         data-select-ancestors="true || false"
         data-toggle-descendants="true || false">

        <!-- ASP.NET TreeView -->


    </div>
*/

var CheckBoxTree = CheckBoxTree || (function () {

    function init(treeView) {

        // dom objects
        var div = treeView.parent('div.check-box-tree');

        // values
        var selectAncestors = div.data('select-ancestors');
        var toggleDescendants = div.data('toggle-descendants');

        // hook into all checkboxes
        div.find('input[type="checkbox"]')
           .change(function () {
               checkBoxChanged(jQuery(this), selectAncestors, toggleDescendants);
           });
    }


    function checkBoxChanged(checkBox, selectAncestors, toggleDescendants) {

        var isChecked = checkBox.is(':checked');

        if (selectAncestors) {
            if (isChecked) {

                checkBox.parentsUntil('div.check-box-tree', 'div').each(function (index, element) {
                    jQuery(element).prev('table:first').find('input[type="checkbox"]:first').attr('checked', true);
                });

            } else {

                // walk down the tree unchecking - force this by setting toggleDescendants
                toggleDescendants = true;
            }
        }

        if (toggleDescendants) {

            // walk down the tree and set all checkboxes to match the current
            checkBox.closest('table').next('div').find('input[type="checkbox"]').attr('checked', isChecked);
        }
    }

    return {
        init: init
    }

} ());