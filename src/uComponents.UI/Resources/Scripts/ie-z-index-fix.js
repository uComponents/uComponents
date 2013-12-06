// This fixes a z-index issue with Properties that extend beyond their DIV
// E.g. Image Dropdown

; (function ($) {
    var contentWin, contentDoc;
    try {
        //Bypass cross domain errors
        contentWin = UmbClientMgr.contentFrame();
        contentDoc = contentWin.document;
    } catch (e) { }

    // Add a z-index to each Property Pane to ensure all panes below are always at a lower z-index
    $(contentDoc).ready(function() {
        cnt = 1000;

        $("<style type='text/css'> .ui-widget{ z-index:2000 !important;} </style>").appendTo("head");

        $('.tabpageContent > div', contentDoc).css('position', 'relative').each(function (i) {
            $(this).css('z-index', cnt);
            cnt = cnt - 1;
            })
        })
})(jQuery);