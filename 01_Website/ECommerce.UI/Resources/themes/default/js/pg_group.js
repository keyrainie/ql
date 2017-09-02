$(function(){
	
	
	//banner Slider
	/*UI.Xslider(".topbanner",{
		showNav:".nav a",
		dir:"V"
	});	*/
	$(".topbanner").find(".close").click(function(){
		$(this).parent().hide();	
	});
	
	//热销排行排行榜
	$(".ranklist li").hover(function(){	
		var lastShow = $(this).parent().data("currItem");
		if(lastShow && (lastShow[0] !== this)) {lastShow.removeClass("curr");}	
		$(this).addClass("curr").parent().data("currItem",$(this));		
	},function(){
		$(this).removeClass("curr");	
	});

	$(".ranklist").bind("mouseleave",function(){
		$(this).data("currItem").addClass("curr");
	});
	
	//Switch sidecate Positon
	/*var sideOjbBox = $("#sideobj");
	var sideOjbWrap = sideOjbBox.parent();
	
	var sideOjbBox2 = $("#sideFloatAd");
	var sideOjbWrap2 = sideOjbBox2.parent();
	
	$(window).bind("scroll",function(){
		setTimeout(function(){
			var st = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop;
			if(!!sideOjbWrap.offset()){
				if(st > sideOjbWrap.offset().top){
					sideOjbBox.next(".sidePlaceHolder").css("height",sideOjbBox.outerHeight());
					sideOjbBox.addClass("sideFixed");
				}
				else {
					sideOjbBox.next(".sidePlaceHolder").css("height",0);
					sideOjbBox.removeClass("sideFixed");
				}
			}
			
			if(!!sideOjbWrap2.offset()){
				if(st > sideOjbWrap2.offset().top){
					sideOjbBox2.next(".sidePlaceHolder").css("height",sideOjbBox2.outerHeight());
					sideOjbBox2.addClass("sideAdFixed");
				}
				else {
					sideOjbBox2.next(".sidePlaceHolder").css("height",0);
					sideOjbBox2.removeClass("sideAdFixed");
				}
			}
			
		},0);
	});
	$(window).scroll();*/
	
	
})