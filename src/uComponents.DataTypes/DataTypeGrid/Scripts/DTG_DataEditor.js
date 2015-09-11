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
				var grid;
				
				var defaults = {
					bJQueryUI: true,
					bLengthChange: false,
					oLanguage: $.uComponents.dictionary().dataTablesTranslation,
					sScrollY: getTableHeight(this),
					bPaginate: false,
					bScrollCollapse: false,
					bSort: false,
					bAutoWidth: true,
					sScrollX: "100%",
					sScrollXInner: "100%",
					aoColumnDefs: [
						{ "sTitle": "", "bSearchable": false, "bSortable": false, "sType": "numeric", "aTargets": [0] },
						{ "sTitle": "", "bSearchable": false, "bSortable": false, "aTargets": [1] }
					],
					fnDrawCallback: function (oSettings) {
						configureToolbar($(oSettings.nTableWrapper).parent());
						configureRows($(oSettings.nTableWrapper).parent());
						configureSortable($(oSettings.nTableWrapper).parent());
					}
				};

				var settings = $.extend({}, defaults, options);

				// Make sure to attach events only once
				if ($(this).data("datatypegridloaded") !== true) {

					// Dont add datatables if there is no table
					if ($("table.display", this).length > 0) {
						grid = $("table.display", this).dataTable(settings);
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
					
					// Trigger column resizing
					grid.fnAdjustColumnSizing(false);

					return grid;
				}

				// Private functions
				function getTableHeight(element) {
					var defaultHeight = 300;

					if ($(element).find("input[id$='TableHeight']").length > 0) {
						defaultHeight = parseFloat($(element).find("input[id$='TableHeight']").val());
					}

					return defaultHeight;
				}

				function configureToolbar(container) {
					if ($(container).find("input[id$='ShowGridHeader']").val() == "False") {
						$(container).find(".fg-toolbar.ui-widget-header:first").hide();
					}

					if ($(container).find("input[id$='ShowGridFooter']").val() == "False") {
						$(container).find(".fg-toolbar.ui-widget-header:last").hide();
					}
				}

				function configureRows(container) {
					// Make sure disabled buttons are not clickable
					$(container).find("tbody .ui-button").click(function () {
						if ($(this).hasClass("ui-state-disabled")) {
							return false;
						}
					});
				    
				    // Hide actions column if read only mode
					if ($(container).find("input[id$='ReadOnly']").val() == "True") {
					    $(container).find("th.actions, td.actions").hide();
					}
				}
				
				function configureSortable(container) {
					var table = $(container).find(".dataTables_scrollBody table.display");
					var tbody = $(table).children("tbody");

					var sortable = $(tbody).sortable({
						axis: "y",
						containment: 'parent',
						items: 'tr',
						tolerance: 'pointer',
						cursor: "move",
						opacity: 0.6,
						helper: function (e, ui) {
							ui.children().each(function () {
								$(this).width($(this).width());
								$(this).height($(this).height());
							});

							return ui;
						},
						stop: function (event, ui) {
							sortValue(container);
						}
					});
				}
				
				function sortValue(container) {
					// Update value sortorder
					var values = $.parseXML($(container).find("input[id$='Value']").val());
					var rows = $(".dataTables_scrollBody table.display tr", container);

					// Loop all rows currently in grid
					$.each(rows, function (i, r) {
						// Find existing element with matching id as the current object
						$.each(values.childNodes[0].childNodes, function (j, e) {
							if (e.attributes["id"].value == $(r).data("dtg-rowid")) {
								// Update sortorder on element
								e.attributes["sortOrder"].value = i;
							}
						});
					});

					$(container).find("input[id$='Value']").val((new XMLSerializer()).serializeToString(values));

					// Set correct row class
					$.each($(container).find("tr"), function (i, o) {
						$(o).removeClass("even, odd");

						if ((i + 1) % 2 == 0) {
							$(o).addClass("even");
						} else {
							$(o).addClass("odd");
						}
					});
				}
			});
		},
		openDialog: function () {
		    $(this).dialog('open');
		    
		    if ($(this).height() > $(window).height()) {
		        var maxHeight = $(window).height() - 28 - 46 - 10;
		        $(this).dialog("option", "height", maxHeight);
		        $(this).dialog("option", "width", $(this).dialog("option", "width") + 9);
		    }
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