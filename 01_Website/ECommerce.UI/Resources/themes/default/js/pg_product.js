$(function(){
	//商品缩略图滚动
	var slider=UI.Xslider(".imgzoomlist",{
		numtoMove:1,
		stepOne:true,	
		start:2,
		viewedSize:425,
		unitLen:85,
		unitDisplayed:1,
		beforeStart:function(e){
			$(e.next).find("a").trigger("mouseenter");
		}
	});
	var bigimg=$(".bigimg").find("img");
	$(".imgzoomlist").find("li a").mouseenter(function(){
		var $this=$(this);
		bigimg.attr("src",$this.attr("href"));
		bigimg.parent("a").attr("href",$this.attr("bigimg"));
		$this.parent().addClass("current").siblings().removeClass("current");
		$(".imgzoomlist").data("curindex",~~$(this).parent().attr("index")+1);
	}).click(function(){	
		return false;	
	});	
	
	//to remove exist jqZoomWindow
	$(".jqzoom").bind("mouseenter",function(){
		var jqZoomWin = $(".jqZoomWindow");
		if(jqZoomWin.length!=0) {
			jqZoomWin.remove(); 
		}
	});
	var options = {
		zoomWidth: 380,
		zoomHeight: 380,
		
		xOffset: 18,
		yOffset: 0,
		title : false,
		showEffect:"fadein",
		hideEffect:"fadeout",
		position: "right" 
	}
	$(".jqzoom").jqzoom(options);

	$(".proshare, .shareB").hover(function(){
		$(this).addClass("share_now");		
		$(this).parents(".proinfo").css("z-index",999);
	},function(){
		$(this).removeClass("share_now");
		$(this).parents(".proinfo").css("z-index",1);
	});
	
	$(".relay").hover(
		function(){
			$(this).addClass("relay_now");
			$(this).parents(".proinfo").css("z-index",999);
			if($(".relay").data("isfocus")){
				$(".relay").data("canHide",false);
			}else{
				$(".relay").data("canHide",true);
			}
			$(".relay").data("isShow",true);
		},
		function(){
			$(".relay").data("isShow",false);
			if($(".relay").data("canHide")){
				$(this).removeClass("relay_now");
				$(this).parents(".proinfo").css("z-index",1);	
			}
		}
	);
	$(".relay input[type='text'],.relay textarea").focus(function(){
		$(".relay").data("canHide",false);
		$(".relay").data("isfocus",true);
	});
	$(".relay input[type='text'],.relay textarea").blur(function(){
		$(".relay").data("isfocus",false);
		$(".relay").data("canHide",true);
		if($(".relay").data("isShow")==false){
			$(".relay").removeClass("relay_now");
			$(this).parents(".proinfo").css("z-index",1);	
		}
	});
	$(".giftlst").hover(
		function(){
			var me=$(this);
			me.find("dt").addClass("hovered");
			me.find("dd").show();
		},
		function(){
			var me=$(this);
			me.find("dt").removeClass("hovered");
			me.find("dd").hide();
		}
	);
	
	UI.Xslider(".combolist");
	UI.Xslider(".promlist");
	
	//套餐组合Tab
	(function(){
		var tablinkInner = $(".tablinks .tablinkInner");
		var tablinkInnerB = $(".tablinks .tablinkInnerB");
		var itemsW = 10;
		//if too many tabitems break the line 
		if(tablinkInnerB.height()>40){
			$(".tablinks .tabitem").each(function(){
				itemsW += $(this).width();
				itemsW += $(this).next("b").outerWidth(true);
			});
			tablinkInnerB.width(itemsW);
			
			$(".tablinks .abtns").show();
			UI.Xslider(".tablinks",{
				unitDisplayed: 1,
				numtoMove: 1,
				viewedSize:	tablinkInner.width(),
				unitLen:tablinkInner.width(),
				scrollObj:".tablinkInnerB",
				scrollObjSize:itemsW,
				scrollunits:null
			});
		}
	})();
	
	/*$(".prominfo li a").click(function(){
		var win;
		if($(this).data("rel_win")!=undefined){
			win =  $(this).data("rel_win");
			win.fn.popIn();
		}
		else {
			win = PopWin("#pop"+$(this).attr("id"),{
				action: "in",
				animate : false,
				overlay: "#overlay_transparent",
				callback: function(o){
					$("#overlay_transparent").click(function(){
						win.fn.popOut();
						$(this).unbind("click");
					});
				}
				,
				afterPopOut: function(){
					$("#overlay_transparent").unbind("click");
				}
			});
			$(this).data("rel_win",win);
		}
	});
	$("#brandinfomore").click(function(){
		var win;
		if($(this).data("rel_win")!=undefined){
			win =  $(this).data("rel_win");
			win.fn.popIn();
		}
		else {
			win = PopWin("#brandinfomorePop",{
				action: "in",
				animate : false,
				overlay: "#overlay_transparent",
				callback: function(o){
					$("#overlay_transparent").click(function(){
						win.fn.popOut();
						$(this).unbind("click");
					});
				}
				,
				afterPopOut: function(){
					$("#overlay_transparent").unbind("click");
				}
			});
			$(this).data("rel_win",win);
		}
		
	});
	$(".hasPopup .opener").click(function(){
		var me=$(this);
		me.parent().find(".popWinA").show();
	})
	$(".hasPopup .shuter").click(function(){
		$(this).parent().parent().hide();
	})
	
	*/
	
	//Switch TabBarBox Positon
	var TabBarBox = $("#TabBarBox");
	var TBBWrap = TabBarBox.parent();
	var canJump = false;
	$(window).bind("scroll",function(){
		var st = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop;
		if(!TBBWrap.offset()) return;
		if(st > TBBWrap.offset().top){
			TabBarBox.next(".tabPlaceHolder").css("height",37);
			TabBarBox.addClass("tabBarBoxFixed");
			canJump=true;
		}
		else {
			TabBarBox.next(".tabPlaceHolder").css("height",0);
			TabBarBox.removeClass("tabBarBoxFixed");
			canJump=false;
		}
	});
	TabBarBox.bind("click",function(e){
		var clickedTab = $(e.target).parent("a");
		if(clickedTab.attr("rel")=="link"){
			clickedTab.addClass("now").siblings("a").removeClass("now");
			 $($.browser.safari || document.compatMode == 'BackCompat' ? document.body : document.documentElement).animate({
				scrollTop: $(clickedTab.attr("href")).offset().top - 30
			}, 200);
			canJump=false;
		}
		if(canJump==true) {
			 $($.browser.safari || document.compatMode == 'BackCompat' ? document.body : document.documentElement).animate({
	            scrollTop: TBBWrap.offset().top
	        }, 200);
		}
	});
	$(".shoppingcartp").hover(
		function(){
			$(this).addClass("shoppingcartp_hover");	
		},
		function(){
			$(this).removeClass("shoppingcartp_hover");	
		}
	);
	
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
	
	
	/* Group Detail Virtual */
	$(".accord dt").click(function(){
		$(this).parent().siblings().removeClass("expand");
		$(this).parent().addClass("expand");
	});
	$(".accord dt a").focus(function(){
		$(this).blur();	
	});
	
	$(".prodetail .rateinfo_inner").hover(
		function(){
			$(this).addClass("rateinfo_inner_hover").parent(".rateinfop").addClass("rateinfop_hover");
		},
		function(){			
			//var price = $(this).data("priceText",$(this).text());
			$(this).removeClass("rateinfo_inner_hover").parent(".rateinfop").removeClass("rateinfop_hover");
			
			
		}
	)
})