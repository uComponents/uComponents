

function changeTabToDropDownView(hostTabAnchor, dropDown, tabCaption, reClick) {

    alert('hostTabAnchor = ' + $(hostTabAnchor).find('nobr').html());

    // Property where dropdown currently exists
    var dropDownProperty = $(dropDown).parentsUntil('div.tabpageContent', 'div.propertypane').parent();

    // Tab being selected
    var tabAnchor = $('span > nobr:contains(' + tabCaption + ')').parentsUntil('li', 'a');
    var tab = $(tabAnchor).parent();
    var tabArea = $('div#' + $(tab).attr('id') + 'layer_contentlayer div.tabpageContent');

    alert('before move');
    $(dropDownProperty).prependTo(tabArea);
    alert('after move');

    if (reClick) {
        alert('before reClick');
        $(tabAnchor).click();
        alert('afte reClick');
    }


    $(hostTabAnchor).parent('li').attr('class', 'tabOn');
}