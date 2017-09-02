/*# AVOID COLLISIONS #*/
if(window.jQuery) (function($){
/*# AVOID COLLISIONS #*/
	
	// IE6 Background Image Fix
	if ($.browser.msie) try { document.execCommand("BackgroundImageCache", false, true)} catch(e) { };
	
	// plugin initialization
	$.fn.rating = function(options){
		if(this.length==0) return this; // quick fail
		
		// Initialize options for this call
		var options = $.extend(
			{}/* new object */,
			$.fn.rating.options/* default options */,
			options || {} /* just-in-time options */
		);
		
		// loop through each matched element
		this.each(function(){

			var $this = $(this);
			var markObj = $this.next('.mark');
			var oMarkText = parseFloat(markObj.text()).toFixed(0);
			
			
			//Common Rate Display
			if($this.children('.highlight')!=0){
				var markNum = oMarkText * 10;
				$this.children('.highlight').width(90 * markNum/50 - 0);
			}
			
			//Do Rating function
			if(!$this.hasClass('rating')) return;
			
			var input = $this.find('input[type=hidden]');
			var isHalf = ( $this.attr('half') == 'true' ) ? true : false; 
			var cellW = 18;
			if(isHalf){
				oMarkText = parseFloat(markObj.text()).toFixed(1);	
			}
			for(var i = 0; i < 5; i++ ){
				$this.append('<a class="rating-btn" href="javascript:void(0);"></a>');	
			}
			var highlight = $('<span class="rating-highlight" style="width:'+oMarkText*cellW+'px"></span>').data('clickWidth',oMarkText*cellW).data('clickMark',oMarkText);
			$this.prepend(highlight);
			
			$this.mousemove(function(e){
				//var currA = $(e.target);
				var x = e.clientX - $this.offset().left;
				var mtext;
				if(isHalf){
					var fixW =  cellW / 2;
					x =  (parseFloat(x / fixW).toFixed(0)) * fixW;
					mtext = parseFloat(x / cellW).toFixed(1);
					//currA[0].title = (parseFloat(x / 13).toFixed(1));
					markObj.text(mtext);
				}
				else {
					x =  Math.ceil((x / cellW)) * cellW;
					mtext = parseFloat(x / cellW).toFixed(0);
					//currA[0].title = (parseFloat(x / 13).toFixed(1));
					markObj.text(mtext);
				}
				
				highlight.css("width",x).data('lastWidth',x).data('lastMark' ,mtext);
				
			}).mouseout(function(){
					highlight.css("width", highlight.data('clickWidth'));
					var cm = highlight.data('clickMark');
					setTimeout(function(){markObj.text(cm);},1);
			}).click(function(){
				var lm = highlight.data('lastMark');
				highlight.data('clickWidth', highlight.data('lastWidth')).data('clickMark',lm);
				input.val(lm);
				
			});
			
  		}); // each element
		
		
		return this; // don't break the chain...
	};
	
	/*
		### Core functionality and API ###
	*/
	$.extend($.fn.rating, {
		
		focus: function(){
		}, // $.fn.rating.focus
		
		blur: function(){
		}, // $.fn.rating.blur
		
		fill: function(){
		},// $.fn.rating.fill
		
		drain: function() {
		},// $.fn.rating.drain
		
		draw: function(){ 
		},// $.fn.rating.draw
		
		select: function(value){ 
		},// $.fn.rating.select
		
		readOnly: function(toggle, disable){
		},// $.fn.rating.readOnly
		
		disable: function(){ // make read-only and never submit value
			this.rating('readOnly', true, true);
		},// $.fn.rating.disable
		
		enable: function(){ // make read/write and submit value
			this.rating('readOnly', false, false);
		}// $.fn.rating.select
		
 });
	
	/*
		### Default Settings ###
		eg.: You can override default control like this:
	*/
	$.fn.rating.options = { //$.extend($.fn.rating, { options: {
			
			//NB.: These don't need to be pre-defined (can be undefined/null) so let's save some code!
			half:     false,         // just a shortcut to control.split = 2
			//required: false,         // disables the 'cancel' button so user can only select one of the specified values
			readOnly: false,         // disable rating plugin interaction/ values cannot be changed
			//focus:    function(){},  // executed when stars are focused
			//blur:     function(){},  // executed when stars are focused
			callback: function(){}  // executed when a star is clicked
    }; //} });

	$('.rate').rating();	
	
/*# AVOID COLLISIONS #*/
})(jQuery);
/*# AVOID COLLISIONS #*/
