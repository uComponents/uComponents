
function activateSubTab(hostTabAnchor, subTabsPanel, subTabsType, reClick) {

    // property where the subTabs datatype currently exists (it gets moved into the active sub tab)
    var subTabsProperty = $(subTabsPanel).parentsUntil('div.tabpageContent', 'div.propertypane').parent();
    var activeSubTabName;

    switch (subTabsType) {

        case 'Buttons':
            // the disabed button is the active tab
            activeSubTabName = $(subTabsPanel).children('button[disabled]').data('tab');
            break;

        case 'DropDownList':
            // ths currently selected item is the active tab
            activeSubTabName = $(subTabsPanel).children('select').val();
            break;
    }

    var tabAnchor = $('span > nobr')
                        .filter(function (){
                            return $(this).html() == activeSubTabName;
                        })
                        .parentsUntil('li', 'a');

    var tab = $(tabAnchor).parent();
    var tabArea = $('div#' + $(tab).attr('id') + 'layer_contentlayer div.tabpageContent');

    // move the sub tabs property to the tab being activated
    $(subTabsProperty).prependTo(tabArea);

    if (reClick) {

        // cause umbraco to activate the hidden tab
        $(tabAnchor).click();

        // reset the umbraco tabs so that the tab where the subTabs proeprty originally stared out appears to be selected
        $(hostTabAnchor).parent('li').attr('class', 'tabOn');
    }    
}
