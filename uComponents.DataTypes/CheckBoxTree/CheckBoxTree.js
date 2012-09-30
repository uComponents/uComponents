
function OnCheckBoxCheckChanged(evt) {

    var src = window.event != window.undefined ? window.event.srcElement : evt.target;
    var isChkBoxClick = (src.tagName.toLowerCase() == 'input' && src.type == 'checkbox');
    if (isChkBoxClick) {

        var parentTable = GetParentByTagName('table', src);
        var nxtSibling = parentTable.nextSibling;
        if (nxtSibling && nxtSibling.nodeType == 1)//check if nxt sibling is not null & is an element node 
        {
            if (nxtSibling.tagName.toLowerCase() == 'div') //if node has children 
            {
                if (!src.checked) {
                    //uncheck children at all levels 
                    CheckUncheckChildren(parentTable.nextSibling, src.checked);
                }
            }
        }
        //check or uncheck parents at all levels 
        CheckUncheckParents(src, src.checked);
    }
}

function CheckUncheckChildren(childContainer, check) {
    var childChkBoxes = childContainer.getElementsByTagName('input');
    var childChkBoxCount = childChkBoxes.length;
    for (var i = 0; i < childChkBoxCount; i++) {
        childChkBoxes[i].checked = check;
    }
}

function CheckUncheckParents(srcChild, check) {
    var parentDiv = GetParentByTagName('div', srcChild);
    var parentNodeTable = parentDiv.previousSibling;

    if (parentNodeTable) {
        var checkUncheckSwitch;

        if (check) //checkbox checked 
        {
            checkUncheckSwitch = true;
        }
        else //checkbox unchecked 
        {
            var isAllSiblingsUnChecked = AreAllSiblingsUnChecked(srcChild);
            if (!isAllSiblingsUnChecked) {
                checkUncheckSwitch = true;
            } else {
                checkUncheckSwitch = false;
            }
        }

        var inpElemsInParentTable = parentNodeTable.getElementsByTagName('input');
        if (inpElemsInParentTable.length > 0) {
            var parentNodeChkBox = inpElemsInParentTable[0];
            parentNodeChkBox.checked = checkUncheckSwitch;
            //do the same recursively 
            CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
        }
    }
}

//utility function to get the container of an element by tagname 
function GetParentByTagName(parentTagName, childElementObj) {
    var parent = childElementObj.parentNode;
    while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
        parent = parent.parentNode;
    }
    return parent;
}

// ---------------------------------------------------------------------------------









var CheckBoxTree = CheckBoxTree || (function () {

    function init(treeView) {

        // dom objects
        var div = treeView.parent('div.check-box-tree');

    }

    function toggleAncestors() {

    }

    function toggleDescendants() {

    }


    return {

        init: init


    }

} ());