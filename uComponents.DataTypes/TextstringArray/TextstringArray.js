/// <reference path="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js" />

; (function ($) {

	$.textstringArray = function (el, options) {

		var defaults = {
			hiddenId: '',
			minimum: 1,
			maximum: -1
		}

		var plugin = this,
			$this = $(el),
			$rows = $this.find('.textstring-row');

		plugin.settings = {}

		var init = function () {
			plugin.settings = $.extend({}, defaults, options);
			plugin.el = el;

			$this.sortable({
				axis: 'y',
				containment: $this.closest('.propertyItemContent'),
				items: '.textstring-row',
				handle: '.textstring-row-sort',
				forcePlaceholderSize: true,
				placeholder: 'textstring-row-placeholder',
				stop: function (event, ui) {
					serialize();
				}
			});

			$this.find('.textstring-row-add').click(function () {
				plugin.addRow(this);
				return false;
			});

			$this.find('.textstring-row-remove').click(function () {
				plugin.removeRow(this);
				return false;
			});

			$this.find('input').blur(function(){
				serialize();
			});

			$this.find('input').keydown(function (e) {
				var keyCode = e.keyCode || e.which;

				// if ENTER is pressed
				if (keyCode == 13) {

					e.preventDefault();

					// add a new row
					return $(this).parent().parent().find('.textstring-row-add').click();
				}

/*				// if BACKSPACE if pressed and the textstring value is empty
				if (keyCode == 8 && $(this).val() == '') {

					e.preventDefault();

					// remove the row
					return $(this).parent().parent().find('.textstring-row-remove').click();
				}
*/
			});

			// Set width of header row columns
			// var curWidth = $this.find('.textstring-row-field').first().width();
			// $this.find('.textstring-header-row-col').css('width', curWidth);

			// toggle visibility of edit controls
			updateEditControls();
		}

		plugin.addRow = function (e) {
			var $parent = $(e).parent().parent();
			var $row = $parent.clone(true); // clone the row
			var $input = $row.find('input');

			if ($rows.length < plugin.settings.maximum || plugin.settings.maximum <= 0) {

				// clear the text field
				$input.val('');

				// append the new row
				$row.insertAfter($parent);

				// set the focus
				$input[0].focus();

				// re-populate the hidden field
				serialize();

				// toggle visibility of edit controls
				updateEditControls();
			}
		}

		plugin.removeRow = function (e) {
			// make sure the user wants to remove the row
			if (confirm('Are you sure you want to delete this row?')) {
				
				// check if this is the last row...
				if ($rows.length == 1) {

					// ... if so, just clear it.
					$rows.find('input').val('').get(0).focus();

				} else if ($rows.length > plugin.settings.minimum) {
					var $parent = $(e).parent().parent();

					// set the focus
					var $focusElement = $parent.prev('.textstring-row').length > 0 ? $parent.prev('.textstring-row') : $parent.next('.textstring-row');
					$focusElement.find('input').get(0).focus();

					// remove the row
					$parent.remove();
				}

				// re-populate the hidden field
				serialize();

				// toggle visibility of edit controls
				updateEditControls();
			}
		}

		var serialize = function () {
			var values = [];

			$rows = $this.find('.textstring-row');

			$rows.each(function(i, e1){
				var value = [];

				$(this).find('.textstring-row-field').each(function(j, e2){
					value.push($(this).find('input').val());
				});

				values.push(value);
			});

			var $hidden = $this.find(plugin.settings.hiddenId);
			$hidden.val(JSON.stringify(values));
		}

		function updateEditControls() {
			var addButton = $this.find('.textstring-row-add'),
				removeButton = $this.find('.textstring-row-remove'),
				sortButton = $this.find('.textstring-row-sort');

			if (plugin.settings.maximum == -1 || plugin.settings.maximum > $rows.length) {
				addButton.show();
			} else {
				addButton.hide();
			}

			if (plugin.settings.minimum == -1 || plugin.settings.minimum == 0 || plugin.settings.minimum < $rows.length) {
				removeButton.show();
			} else {
				removeButton.hide();
			}

			if ($rows.length > 1) {
				sortButton.show();
			} else {
				sortButton.hide();
			}
		}

		init();

	}

})(jQuery);