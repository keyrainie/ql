$(function(){
	$(".acmenu dt").addClass("expand").click(function(){
		$(this).toggleClass("expand").next().toggle();
	});
	
	
})