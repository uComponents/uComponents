(function ($) {

	// jquery plugin for the character limit
	$.fn.CharLimit = function () {

		var $txt = this.find('.CharLimit');
		var $info = this.find('.CharLimitStatus');
		var limit = $txt.attr('rel');

		$txt.keyup(function () {

			if ($txt.val().length > limit) {

				$info.html('You cannot write more than ' + limit + ' characters!');

				$txt.val($txt.val().substr(0, limit));

			}
			else {

				$info.html('You have ' + (limit - $txt.val().length) + ' characters left.');

			}

		});

	}

})(jQuery);
