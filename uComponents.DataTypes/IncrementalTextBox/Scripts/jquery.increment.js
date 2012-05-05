(function($){  
    /**
    * Returns the current value of the counter.
    */
    function value(counter) {
        return parseInt(counter.val());
    }
    /**
    * Changes the counter's value.
    */
    function change(counters, step) {
    return counters.each(function() {  
        var $counter = $(this);
        // Increment counter
        var count = value($counter) + step;
        $counter.val(count);
        // Trigger event
        var event_name = step > 0 ? 'increment' : 'decrement';
        $counter.trigger(event_name, [$counter, count]);
        return count;
    });  
    }
    $.fn.increment = function(step) {  
    if(!step) { step = 1; }
    change(this, step);
    };  
    $.fn.decrement = function(step) {  
    if(!step) { step = -1; }
    change(this, step);
    };  
    $.fn.counterValue = function() {  
    return value($(this));
    };  
})(jQuery); 