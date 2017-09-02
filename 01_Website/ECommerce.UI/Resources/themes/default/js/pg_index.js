$(function(){
	UI.Xslider(".topbanner",{
		autoScroll:5000,
		speed:2000,
		showNav:".nav a"
	});
	
	$(".sideprolist .tab").find(".tab_1").click(function(){
			$(".sideprolist .tab").removeClass("tabAlter");
	});
	$(".sideprolist .tab").find(".tab_2").click(function(){
			$(".sideprolist .tab").addClass("tabAlter");
	});
	
	
	//UI.Xslider($(".floor2 .slideBig"),{showNav:".dotnav a"});	
	//UI.Xslider($(".floor3 .slidearea"),{showNav:".dotnav a"});
	//UI.Xslider($(".floor8 .brandSlider"),{showNav:".dotnav a"});
	
	
	if(ie6){
		$(".stfloor .tab").each(function(){
			//.tab为absolute，在absolute元素和float浮动元素间加入一个空div
			$(this).after("<div class='ie6_blankdiv'></div>");
		});
		//在absolute元素和float浮动元素的父级元素最后加入一个清浮动的div
		$(".stfloor .inner").append("<div class='clear'></div>");
	}
	
});