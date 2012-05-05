$(function () {
	var accordion = $(".dtg_accordion")
			.accordion({
				header: ".propertyHeader",
				event: false,
				icon: false
			})
			.sortable({
				axis: "y",
				containment: 'parent',
				items: 'div.editProperty',
				handle: ".propertyHeader",
				tolerance: 'pointer',
				stop: function (event, ui) {
					$.each($(ui.item).parent().find("div.editProperty"), function (index, elem) {
						var itemid = $(this).attr("id");
						var id = itemid.substring(itemid.lastIndexOf('_') + 1, itemid.length);

						var params = {
							'preValueId': id,
							'sortOrder': index + 1
						};

						$.ajax({
							type: "POST",
							data: JSON.stringify(params),
							dataType: "json",
							url: "/umbraco/plugins/uComponents/DataTypeGrid/PreValueWebService.asmx/ReorderPreValue",
							contentType: "application/json; charset=UTF-8",
							success: function (data, textStatus, jqXHR) {
								// Update sortorder values
								$(elem).find("input[type=hidden]").val(params.sortOrder);
							},
							error: function (jqXHR, textStatus, errorThrown) {
								if (console && console.log) {
									console.log("DTG: " + textStatus);
								}
							}
						});
					});
				}
			});

	$(".dtg_accordion h3.propertyTitle").each(function (index) {
		$(this).click(function () {
			accordion.accordion("activate", index);
		});
	});

	// Fix for jQuery 1.7.2
	$(".DeleteProperty").live('mouseover', function () {
		$(this).addClass("ui-state-hover");
	});
	$(".DeleteProperty").live('mouseout', function () {
		$(this).removeClass("ui-state-hover ui-state-active ui-state-focus");
	});
	$(".DeleteProperty").live('mousedown', function () {
		$(this).addClass("ui-state-active ui-state-focus");
	});

	$(".DeleteProperty").click(function () {
		EmptyNewPreValueInputs();
		DisableAllValidators();
	});

	$(".validationLink").click(function () {
		var textareaid = $(this).siblings(".validationExpression").attr("id");
		var dialogurl = "dialogs/regexWs.aspx?target=" + textareaid;
		UmbClientMgr.openModalWindow(dialogurl, 'Search for regular expression', true, 600, 500, 0, 0, '', '');

		return false;
	});

	$('.newContentSortPriority').change(function () {
		var order = $(this).parent().parent().find(".newContentSortOrder");

		if ($(this).val() == "") {
			order.val("");
		}
		else if (order.val() == "") {
			order.val("asc");
		}
	});

	$(".newContentSortOrder").change(function () {
		var priority = $(this).parent().parent().find(".newContentSortPriority");

		if ($(this).val() == "") {
			priority.val("");
		}
		else if (priority.val() == "") {
			priority.val("1");
		}
	});

	$('.editContentSortPriority').change(function () {
		var order = $(this).parent().parent().find(".editContentSortOrder");

		if ($(this).val() == "") {
			order.val("");
		}
		else if (order.val() == "") {
			order.val("asc");
		}
	});

	$(".editContentSortOrder").change(function () {
		var priority = $(this).parent().parent().find(".editContentSortPriority");

		if ($(this).val() == "") {
			priority.val("");
		}
		else if (priority.val() == "") {
			priority.val("1");
		}
	});

	$('.newName, .newAlias, .newValidation, .editName, .editAlias, .editValidation').keyup(function () {
		$(this).trigger('change');
	});

	$('.editName, .editAlias, .editValidation').change(function () {
		var containervalid = true;

		$.each($(this).siblings(".validator"), function () {
			var v = document.getElementById($(this).attr("id"));

			ValidatorValidate(v);

			containervalid = v.isvalid;

			return v.isvalid;
		});

		var container = $(this).closest('.addNewProperty, .editProperty');
		if (containervalid) {
			ClearFailedPreValue(container);
		}
		else {
			MarkFailedPreValue(container);
		}
	});

	$('.newName, .newAlias, .newValidation').change(function () {
		var container = $(this).closest('.addNewProperty');

		var vn = document.getElementById($(".addNewProperty .newName").siblings(".validator").attr("id"));
		var va1 = document.getElementById($(".addNewProperty .newAlias").siblings(".validator").attr("id"));
		var va2 = document.getElementById($(".addNewProperty .newAlias").siblings(".validator.exists").attr("id"));
		var vr = document.getElementById($(".addNewProperty .newValidation").siblings(".validator").attr("id"));

		// If any of them are not empty, then enable the validator
		if ($(".addNewProperty .newName").val().length > 0 || $(".addNewProperty .newAlias").val().length > 0 || $(".addNewProperty .newValidation").val().length > 0) {
			ValidatorEnable(vn, true);
			ValidatorEnable(va1, true);
			ValidatorEnable(va2, true);
			ValidatorEnable(vr, true);

			var containervalid = true;

			$.each($(this).siblings(".validator"), function () {
				var v = document.getElementById($(this).attr("id"));

				ValidatorValidate(v);

				containervalid = v.isvalid;

				return v.isvalid;
			});

			if (containervalid) {
				ClearFailedPreValue(container);
			}
			else {
				MarkFailedPreValue(container);
			}
		}
		else {
			ValidatorEnable(vn, false);
			ValidatorEnable(va1, false);
			ValidatorEnable(va2, false);
			ValidatorEnable(vr, false);

			ClearFailedPreValue(container);
		}
	});

	function DisableAllValidators() {
		$(".dtg_accordion .validator").each(function () {
			var v = document.getElementById($(this).attr("id"));

			ValidatorEnable(v, false);
		});
	}

	function EmptyNewPreValueInputs() {
		$('.newName').val("");
		$('.newAlias').val("");
		$('.newValidation').val("");
	}

	function MarkFailedPreValue(elem) {
		var icon = $(elem).find(".ErrorProperty");
		$(icon).fadeIn('fast');
	}

	function ClearFailedPreValue(elem) {
		var icon = $(elem).find(".ErrorProperty");
		$(icon).fadeOut('fast');
	}
});

function ValidateRegex(source, e) {
	var regex;

	if (e.Value && (e.Value.replace(/\s+/g, '').length > 0)) {
		try {
			regex = new RegExp(e.Value);
		}
		catch (err) {
			e.IsValid = false;
		}
	}

	if (regex && typeof (regex) != 'undefined') {
		e.IsValid == true;
	}
}

function ValidateNewAliasExists(source, e) {
	e.IsValid = true;

	if (e.Value.length > 0) {
		// Are there any other textboxes with this alias?
		$(document).ready(function () {
			$(".dtg_accordion").find(".editAlias").each(function () {
				if ($(this).val() == e.Value) {
					e.IsValid = false;
				}
			});
		});
	}
}

function ValidateAliasExists(source, e) {
	e.IsValid = true;

	if (e.Value.length > 0) {
		// Are there any other textboxes with this alias?
		$(document).ready(function () {
			$(".dtg_accordion").find(".newAlias").each(function () {
				if ($(this).val() == e.Value) {
					e.IsValid = false;
				}
			});

			$(".dtg_accordion").find(".editAlias").each(function () {
				if ($(this).attr("id") != source.controltovalidate && $(this).val() == e.Value) {
					e.IsValid = false;
				}
			});
		});
	}
}