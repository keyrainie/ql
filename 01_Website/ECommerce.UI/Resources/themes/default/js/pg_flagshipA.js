$(function(){
	UI.Xslider(".topbanner",{
		showNav:".dotnav a",
		navEventType:"click"
	});
	
	//边栏商品列表中hover到含有价格的按钮上
	$(".sidebox .btn_gray24").hover(
		function(){
			var price = $(this).data("priceText",$(this).text());
			$(this).addClass("btn_red24");
			$(this).find("em").text("加入购物车").addClass("cmnTxt");
		},
		function(){			
			//var price = $(this).data("priceText",$(this).text());
			$(this).removeClass("btn_red24");
			$(this).find("em").text($(this).data("priceText")).removeClass("cmnTxt");
			
		}
	);
	
	/*UI.Xslider(".slider",{
		showNav:".dotnav a",
		scrollObj:".moverlist",
		scrollunits:".item",
		viewedSize:990,
		unitLen:990,
		//loop:"cycle",
		navEventType:"click",
		afterEnd:function(e){
			if (!-[1, ] && !window.XMLHttpRequest){//ie6;
				$(e.container).find(".abtn").css({
					opacity:1	
				}).filter(".agrayleft,.agrayright").css({
					opacity:.3	
				});
			}
		}
	});
	
	$(".caty .slider").hover(function(){
		$(this).find(".abtn").show();
	},function(){
		$(this).find(".abtn").hide();	
	});
	
	$(".sub_menu").find(".child").hover(function(){
		$(this).find("ol").length && $(this).addClass("expand");
	},function(){
		$(this).removeClass("expand");
	});*/
	
})