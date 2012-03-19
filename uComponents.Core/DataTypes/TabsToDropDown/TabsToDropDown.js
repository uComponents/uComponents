
/// hostTabAnchor is the tab on which the TabsToDropDown datatyp is one
/// dropDown is the selector to the dropdown list
/// reClick is true if a hidden tab needs to be clicked so as the correct form is shown
function changeTabToDropDownView(hostTabAnchor, dropDown, reClick) {

    // Property where dropdown currently exists
    var dropDownProperty = $(dropDown).parentsUntil('div.tabpageContent', 'div.propertypane').parent();

    // Tab being selected
    var tabAnchor = $('span > nobr:contains(' + $(dropDown).val() + ')').parentsUntil('li', 'a');
    var tab = $(tabAnchor).parent();
    var tabArea = $('div#' + $(tab).attr('id') + 'layer_contentlayer div.tabpageContent');

    $(dropDownProperty).prependTo(tabArea);

    if (reClick) {
        $(tabAnchor).click();
        $(hostTabAnchor).parent('li').attr('class', 'tabOn');
    }

}