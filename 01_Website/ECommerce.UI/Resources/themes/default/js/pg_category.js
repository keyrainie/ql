$(function(){
	
	//左栏：展开/收起二级分类
	$(".submenu .icon_corner").click(function(){
		var $this = $(this);
		if($this.hasClass("icon_corner_down")){
			$this.parents("dt").next("dd").show("fast");
			$this.removeClass("icon_corner_down");	
		}
		else{
			$this.parents("dt").next("dd").hide("fast");
			$this.addClass("icon_corner_down");	
		}	
	});
	
	//排行榜
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
	
	
	UI.Xslider(".topbanner",{
		autoScroll:5000,
		speed:2000,
		showNav:".nav a"
	});
	UI.Xslider(".hotgoods");
	var seachtab=$(".filterbox");
	seachtab.find(".filtermore span").click(function(){
		var me=$(this);
		me.toggleClass("up");	
		if(!me.hasClass("up")){
			me.parents("tr").find(".inner").addClass("filterhidden")
			//seachtab.find(".filtertype > ul > li:gt(4)").removeClass("hidden");
		}else{
			//seachtab.find(".filtertype > ul > li:gt(4)").addClass("hidden");		
			me.parents("tr").find(".inner").removeClass("filterhidden")
		}
	});
	$(".seprator .ckbox").click(function(){
		$(this).toggleClass("ckbox_checked");	
	});
	
	//Compare Bar
	$("#compareclose").click(
		function(){
			var cp=$("#compare");
			var me=$(this)
			if(me.attr("rel")==0){
				me.html("&#91;&#23637;&#24320;&#93;");
				cp.stop().animate({height:35},1000);
				me.attr("rel",1)
			}else{
				me.html("&#91;&#25910;&#36215;&#93;");
				cp.stop().animate({height:143},1000);
				me.attr("rel",0)
			}
		}
	);
	$("#compare .compareArea li").hover(function(){
		$(".del",this).show()
	},function(){
		$(".del",this).hide()
	});
	//解决小窗口下“开始比较”按钮被floatFunc遮挡问题
	var timerFixComparePos;
	$(window).bind("resize",function(){
		clearTimeout(timerFixComparePos);
		timerFixComparePos =  setTimeout(function(){
		if($(window).width()<1280){
			$("#compare").css({right:92});
		}
		else{
			$("#compare").css({right:"auto"});	
		}
		},10);
	});
	$(window).resize();
	
	//三级分类商品列表部分按钮hover效果
	$(".catepro .action .btn_gray24").hover(
		function(){
			$(this).addClass("btn_red24");
		},
		function(){
			$(this).removeClass("btn_red24");
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
	
});