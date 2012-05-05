// todo : bool flag to refresh tree from parent / root
function jumpToEditContent(nodeId) {

    var js = "function delayedNavigate(url) { setTimeout(function() {UmbClientMgr.contentFrame(url);},1000);}";
    var injectScript = UmbClientMgr.mainWindow().jQuery('<script>').attr('type', 'text/javascript').html(js);
    UmbClientMgr.mainWindow().jQuery("head").append(injectScript);
    UmbClientMgr.mainWindow().delayedNavigate("/umbraco/editContent.aspx?id=" + nodeId);
    UmbClientMgr.mainWindow().location.hash = "#content";

}


// todo : bool flag to refresh tree from parent / root
function jumpToEditMedia(mediaId) {

    var js = "function delayedNavigate(url) { setTimeout(function() {UmbClientMgr.contentFrame(url);},1000);}";
    var injectScript = UmbClientMgr.mainWindow().jQuery('<script>').attr('type', 'text/javascript').html(js);
    UmbClientMgr.mainWindow().jQuery("head").append(injectScript);
    UmbClientMgr.mainWindow().delayedNavigate("/umbraco/editMedia.aspx?id=" + mediaId);
    UmbClientMgr.mainWindow().location.hash = "#media";

}