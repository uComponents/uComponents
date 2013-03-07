$(function () {

    if ($("table.display").length > 0) {

        $("table.display").dataTable({
            bJQueryUI: true,
            bRetrieve: true,
            bLengthChange: false,
            oLanguage: $.uComponents.dictionary().dataTablesTranslation,
            iDisplayLength: GetNumberOfRows(this),
            sPaginationType: "full_numbers",
            aaSorting: GetContentSorting(this),
            aoColumnDefs: [
                { "bVisible": false, "bSearchable": false, "aTargets": [0], "sType": "numeric" },
                { "sTitle": "", "bSortable": false, "aTargets": [1] }
            ],
            fnDrawCallback: function (oSettings) {
                ConfigureToolbar($(oSettings.nTableWrapper).parent());
            }
        });
    }

    // Fix for jQuery 1.7.2
    $(".ui-button").live('mouseover', function () {
        $(this).addClass("ui-state-hover");
    });
    $(".ui-button").live('mouseout', function () {
        $(this).removeClass("ui-state-hover ui-state-active ui-state-focus");
    });
    $(".ui-button").live('mousedown', function () {
        $(this).addClass("ui-state-active ui-state-focus");
    });

    $(".InsertControls").dialog({
        autoOpen: false,
        width: 436,
        dialogClass: 'dtg dtg-dialog',
        modal: true,
        draggable: true,
        resizable: true,
        title: "Insert",
        maxWidth: $(window).width(),
        maxHeight: $(window).height(),
        open: function (type, data) {
            var dialog = $(this).parent();
            dialog.appendTo("form");

            // Move insert button to dialog button area
            dialog.append("<div class='ui-dialog-buttonpane ui-widget-content ui-helper-clearfix'><div class='ui-dialog-buttonset'></div></div>");
            dialog.find('a.insertButton').appendTo(dialog.find('.ui-dialog-buttonset'));

            // Enable validators
            EnableValidators(this, true);
        },
        close: function (event, ui) {
            // Disable validators
            EnableValidators(this, false);
        }
    });

    $(".EditControls").dialog({
        autoOpen: false,
        width: 436,
        dialogClass: 'dtg dtg-dialog',
        modal: true,
        draggable: true,
        resizable: true,
        title: "Edit",
        maxWidth: $(window).width(),
        maxHeight: $(window).height(),
        open: function (type, data) {
            var dialog = $(this).parent();
            dialog.appendTo("form");

            // Move update button to dialog button area
            dialog.append("<div class='ui-dialog-buttonpane ui-widget-content ui-helper-clearfix'><div class='ui-dialog-buttonset'></div></div>");
            dialog.find('a.updateButton').appendTo(dialog.find('.ui-dialog-buttonset'));

            // Enable validators
            EnableValidators(this, true);
        },
        close: function (event, ui) {
            // Disable validators
            EnableValidators(this, false);
        }
    });

    $(".DeleteControls").dialog({
        autoOpen: false,
        width: 436,
        dialogClass: 'deletedialog',
        modal: true,
        draggable: true,
        title: "Delete",
        open: function (type, data) {
            $(this).parent().appendTo("form");
        }
    });

    // Reposition dialogs to fix bug with dialog being positioned out of window bounds
    $(window).trigger('resize');

    // Fix for TinyMCE Editor
    $(".tinymceContainer").each(function () {
        if ($(this).find(".dtgTinymceMenu").length == 0) {
            $(this).before("<div id='umbTinymceMenu_" + $(this).attr("id") + "' class='dtgTinymceMenu'></div>");
        }
    });

    function GetContentSorting(element) {
        var e = "";

        if ($(element).find("input[id$='ContentSorting']").length > 0) {
            e = $(element).find("input[id$='ContentSorting']").val();
        }

        return eval(e);
    }

    function GetNumberOfRows(element) {
        var numberOfRows = 10;

        if ($(element).find("input[id$='RowsPerPage']").length > 0) {
            numberOfRows = parseFloat($(element).find("input[id$='RowsPerPage']").val());
        }

        return numberOfRows;
    }

    function ConfigureToolbar(element) {
        if ($(element).find("input[id$='ShowTableHeader']").val() == "False") {
            $(element).find(".fg-toolbar.ui-widget-header:first").hide();
        }

        if ($(element).find("input[id$='ShowTableFooter']").val() == "False") {
            $(element).find(".fg-toolbar.ui-widget-header:last").hide();
        }
    }
});

function EnableValidators(elem, enabled) {
    // Enable validators
    var validators = $(elem).find(".validator");
    
    if (validators.length > 0) {
        $.each(validators, function () {
            // Check if validation scripts are enabled
            if ($.isFunction(ValidatorEnable)) {
                var e = document.getElementById($(this).attr("id"));
                
                // Check if an element exist with the specified id
                if (e) {
                    ValidatorEnable(e, enabled);
                }
            }
        });
    }
}

function RequiredFieldValidate(source, args) {
    var controlToValidate = document.getElementById($(source).data("controltovalidate"));
    var validationProperty = $(source).data("validationproperty");
    
    if (controlToValidate) {
        // Register change events
        if ($(controlToValidate).data("changeeventsattached-required") != true) {
            $(controlToValidate).bind("change keyup", function() {
                ValidatorValidate(source);
            });

            $(controlToValidate).data("changeeventsattached-required", true);
        }
        
        // Set up HTML5 validation if browser supports it
        if (typeof document.createElement('input').checkValidity == 'function') {
            $(controlToValidate).attr("required", "required");
        } 
            
        var value = "";

        if (validationProperty == "Text") {
            value = $(controlToValidate).val();
        } else if (validationProperty == "Value") {
            value = $(controlToValidate).val();
        } else {
            value = $(controlToValidate).attr(validationProperty);
        }

        if (!value) {
            args.IsValid = false;

            return;
        }

        args.IsValid = true;
    }
}

function RegexValidate(source, args) {
    var controlToValidate = document.getElementById($(source).data("controltovalidate"));
    var validationProperty = $(source).data("validationproperty");
    var validationExpression = $(source).data("validationexpression");

    if (controlToValidate) {
        // Register change events
        if ($(controlToValidate).data("changeeventsattached-regex") != true) {
            $(controlToValidate).bind("change keyup", function () {
                ValidatorValidate(source);
            });

            $(controlToValidate).data("changeeventsattached-regex", true);
        }
        
        // Set up HTML5 validation if browser supports it
        if (typeof document.createElement('input').checkValidity == 'function') {
            $(controlToValidate).attr("pattern", validationExpression);
        }
        
        var value = "";
        
        if (validationProperty == "Text") {
            value = $(controlToValidate).val();
        } else if (validationProperty == "Value") {
            value = $(controlToValidate).val();
        } else {
            value = $(controlToValidate).attr(validationProperty);
        }

        if (!new RegExp(validationExpression).test(value)) {
            args.IsValid = false;

            return;
        }

        args.IsValid = true;
    }
}

function openDialog(elementId) {
    $(function () {
        $('#' + elementId).dialog('open');
    });
}
