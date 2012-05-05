function addItem(dateListId, dateValue, dataElementId) {

    if (!dateValue) {
        return;
    }

    $("#" + dateListId).parent().fadeIn(150);

    if ($('#' + dataElementId).val() == '') {
        $('#' + dataElementId).val(dateValue);
        var itemId = dateListId + dateValue;
        var item = '<li id="' + itemId + '" >' + dateValue + ' <div class="deleteButton" onclick="removeItem(this,\'' + dataElementId + '\',\'' + dateValue + '\')"></div> </li>';
        $('#' + dateListId).append(item);
    }
    else {
        if ($('#' + dataElementId).val().search(dateValue) == -1) {
            $('#' + dataElementId).val($('#' + dataElementId).val() + ',' + dateValue);

            var itemId = dateListId + dateValue;
            var item = '<li id="' + itemId + '" >' + dateValue + '  <div class="deleteButton" onclick="removeItem(this, \'' + dataElementId + '\',\'' + dateValue + '\')"><img src="images/delete_button.png" height="15" width="15" /></div> </li>';
            $('#' + dateListId).append(item);
        }
    }

    
}

function removeItem(button,dataElementId,dateValue) {

    var item = $(button).parent();
    var container = item.parent().parent();

    item.remove();
    

    if ($('#' + dataElementId).val().search(',') == -1)
        $('#' + dataElementId).val($('#' + dataElementId).val().replace(dateValue, ''));

    if ($('#' + dataElementId).val().search(dateValue + ',') > -1)
        $('#' + dataElementId).val($('#' + dataElementId).val().replace(dateValue + ',', ''));

    if ($('#' + dataElementId).val().search(',' + dateValue) > -1)
        $('#' + dataElementId).val($('#' + dataElementId).val().replace(',' + dateValue, ''));

    if ($('#' + dataElementId).val().search(dateValue) > -1)
        $('#' + dataElementId).val($('#' + dataElementId).val().replace(dateValue, ''));

    
    if (!$('#' + dataElementId).val()) {
        container.fadeOut(150);
    }
    
}