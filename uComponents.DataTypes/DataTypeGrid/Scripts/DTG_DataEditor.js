function RequiredFieldValidate(source, args) {
    args.IsValid = $(source).uComponents().datatypegrid("requiredFieldValidate", args);
}

function RegexValidate(source, args) {
    args.IsValid = $(source).uComponents().datatypegrid("regexValidate", args);
}

(function ($) {
    var uComponentsDataTypeGrid = {
        init: function (options) {
            return this.each(function () {
                // Make sure to attach events only once
                if ($(this).data("datatypegridloaded") !== true) {

                    // Dont add datatables if there is no table
                    if ($("table.display", this).length > 0) {
                        $("table.display", this).dataTable({
                            bJQueryUI: true,
                            bRetrieve: true,
                            bLengthChange: false,
                            oLanguage: $.uComponents.dictionary().dataTablesTranslation,
                            iDisplayLength: getNumberOfRows(this),
                            sPaginationType: "full_numbers",
                            bSort: false,
                            aoColumnDefs: [
                                { "bVisible": false, "bSearchable": false, "aTargets": [0], "sType": "numeric" },
                                { "sTitle": "", "bSortable": false, "aTargets": [1] }
                            ],
                            fnDrawCallback: function (oSettings) {
                                configureToolbar($(oSettings.nTableWrapper).parent());
                                configureRows($(oSettings.nTableWrapper).parent());
                            }
                        });
                    }
                    
                    // Setup hover events
                    $(".ui-button", this).on("hover", function () {
                        $(this).toggleClass("ui-state-hover");
                    });

                    $(".InsertControls", this).dialog({
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
                            $(this).uComponents().datatypegrid("toggleValidators", true);
                        },
                        close: function (event, ui) {
                            // Disable validators
                            $(this).uComponents().datatypegrid("toggleValidators", false);
                        }
                    });

                    $(".EditControls", this).dialog({
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
                            $(this).uComponents().datatypegrid("toggleValidators", true);
                        },
                        close: function (event, ui) {
                            // Disable validators
                            $(this).uComponents().datatypegrid("toggleValidators", false);
                        }
                    });

                    $(".DeleteControls", this).dialog({
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

                    // Set loaded indicator
                    $(this).data("datatypegridloaded", true);
                }

                // Private functions
                function getNumberOfRows(element) {
                    var numberOfRows = 10;

                    if ($(element).find("input[id$='RowsPerPage']").length > 0) {
                        numberOfRows = parseFloat($(element).find("input[id$='RowsPerPage']").val());
                    }

                    return numberOfRows;
                }

                function configureToolbar(element) {
                    if ($(element).find("input[id$='ShowTableHeader']").val() == "False") {
                        $(element).find(".fg-toolbar.ui-widget-header:first").hide();
                    }

                    if ($(element).find("input[id$='ShowTableFooter']").val() == "False") {
                        $(element).find(".fg-toolbar.ui-widget-header:last").hide();
                    }
                }

                function configureRows(element) {
                    // Make sure disabled buttons are not clickable
                    $(element).find("tbody .ui-button").click(function () {
                        if ($(this).hasClass("ui-state-disabled")) {
                            return false;
                        }
                    });

                    // Set first column width
                    $(element).find("thead th:first, tbody td.actions").width(38);
                }
            });
        },
        openDialog: function () {
            $(this).dialog('open');
        },
        getValidatorValue: function (validationProperty) {
            if ($(this).is("input") && validationProperty == "Text") {
                return $(this).val();
            } else if ($(this).is("input") && validationProperty == "Value") {
                return $(this).val();
            } else if ($(this).is("select") && validationProperty == "SelectedItem") {
                return $(this).val();
            }

            // The control to validate does not support validation
            return null;
        },
        toggleValidators: function (enable) {
            var validators = $(this).find(".validator");

            if (validators.length > 0) {
                $.each(validators, function () {
                    // Check if validation scripts are enabled
                    if ($.isFunction(ValidatorEnable)) {
                        var e = document.getElementById($(this).attr("id"));

                        // Check if an element exist with the specified id
                        if (e) {
                            ValidatorEnable(e, enable);
                        }
                    }
                });
            }
        },
        requiredFieldValidate: function (args) {
            var source = this;

            var controlToValidate = document.getElementById($(source).data("controltovalidate"));
            var validationProperty = $(source).data("validationproperty");

            if (controlToValidate) {
                // Register change events for instant validation
                if ($(controlToValidate).data("changeeventsattached-required") != true) {
                    $(controlToValidate).bind("change keyup", function () {
                        source.each(function () {
                            ValidatorValidate(this);
                        });
                    });

                    $(controlToValidate).data("changeeventsattached-required", true);
                }

                // Set up HTML5 validation if browser supports it
                if (typeof document.createElement('input').checkValidity == 'function') {
                    $(controlToValidate).attr("required", "required");
                }

                var value = $(controlToValidate).uComponents().datatypegrid("getValidatorValue", validationProperty);

                if (value != null && value.length == 0) {
                    return false;
                }
            }

            return true;
        },
        regexValidate: function (args) {
            var source = this;

            var controlToValidate = document.getElementById($(source).data("controltovalidate"));
            var validationProperty = $(source).data("validationproperty");
            var validationExpression = $(source).data("validationexpression");

            if (controlToValidate) {
                // Register change events
                if ($(controlToValidate).data("changeeventsattached-regex") != true) {
                    $(controlToValidate).bind("change keyup", function () {
                        source.each(function () {
                            ValidatorValidate(this);
                        });
                    });

                    $(controlToValidate).data("changeeventsattached-regex", true);
                }

                // Set up HTML5 validation if browser supports it
                if (typeof document.createElement('input').checkValidity == 'function') {
                    $(controlToValidate).attr("pattern", validationExpression);
                }

                var value = $(controlToValidate).uComponents().datatypegrid("getValidatorValue", validationProperty);

                if (value && !new RegExp(validationExpression).test(value)) {
                    return false;
                }
            }

            return true;
        }
    };

    $.fn.uComponents().datatypegrid = $.fn.uComponents().datatypegrid || function (method) {
        if (uComponentsDataTypeGrid[method]) {
            return uComponentsDataTypeGrid[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return uComponentsDataTypeGrid.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.uComponents.datatypegrid');
        }
    };
})(jQuery);

$(function () {
    $(".dtg").uComponents().datatypegrid();
});