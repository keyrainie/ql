$(function(){
	//Header中的店家名称Hover后显示浮层
	$(".store_name").hover(function(){
		$(this).find(".ele_intro").show();
		if(ie6){
			$(this).find(".ele_intro .bdr_a").width($(this).width()+38);	
		}
		
			
	},function(){
		$(this).find(".ele_intro").hide();	
	});
	
	
	//如果用户设定了通栏的店铺导航栏，则...
	if($("#main #menu").length!=0){
		//弹制将通栏的店铺导航栏前面的元素下间距清除为0
		$("#main #menu").prev().css("marginBottom",0);	
	}
	
	//左栏中"全部商品分类"菜单展开了分类
	$(".store_ele_cate .cate_1").bind("click",function(){
		var $this = $(this);
		if($this.hasClass("cate_1_curr")){
			$this.next(".cate_2_ctner").hide("fast");
			$this.removeClass("cate_1_curr");
		}
		else{
			$this.next(".cate_2_ctner").show("fast");	
			$this.addClass("cate_1_curr");
		}
	});
		
	
	//滚动banner列表初始化
	$(".store_ele_bannerSlider").each(function(){
		var bannerSlider = $(this);
		var bsUL = bannerSlider.find("ul");
		var itemH = bannerSlider.find("li").first().height();
		bannerSlider.height(itemH);
		bsUL.height(itemH);
		bsUL.width(bannerSlider.find("li").length*bannerSlider.width());
		if(bannerSlider.find("li").length<2){
			bannerSlider.find(".nav").hide();	
		}
		else{
			UI.Xslider(bannerSlider,{
				autoScroll:5000,
				speed:2000,
				showNav:".nav a"
			});	
		}
	});
	
	//商品列表筛选
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
	
	//通栏中的“最近浏览”动态添加滚动效果
	$(".store_layout_main .store_ele_historyProbox").each(function(){
		var $this = $(this);
		var itemNum = $this.find(".prolist li").length;
		var dotNav = '';
		//var viewedSize = 
		if(itemNum>5){
			for(var i=1,j=Math.ceil(itemNum/5);i<=j;i++){
				dotNav+='<a href="javascript:void(0);">'+i+'</a>';
			}
			
			$this.find(".prolist_ctner").before('<div class="nav"><div class="navInner">'+dotNav+'</div></div>');
			
			UI.Xslider($this.find(".ele_cont"),{
				numtoMove:1,
				unitLen:$this.width(),
				scrollObjSize:$this.width()*Math.ceil(itemNum/5),
				showNav:".nav a"
			});	
			
		}	
	});
	
	
	//通栏中的“店铺公告”展开Intro效果
	$(".store_layout_main .store_ele_newslist li").bind("click",function(){
		var $this = $(this);
		$this.addClass("item_show").find(".ele_intro").show("fast");
		$this.siblings(".item_show").removeClass("item_show").find(".ele_intro").hide("fast");
		$this.siblings(".item_show_last").removeClass("item_show_last");
		if($this.next("li").length==0){
			$this.addClass("item_show_last");
		}
	});
	
	
	//通栏列表页中，通栏menubar结构中是没有“.menulist”元素的
	if($("#menu").length!=0 && $("#menu .menubar").find(".menulist").length==0){
		var eleCate = $("#menu").next(".store_layout_left_main").find(".store_layout_col_left").find(".store_ele_cate");
		if(eleCate.length!=0){
			eleCate.css("marginTop",-49);
			$("#menu").find(".navigation").after('<div class="menu"><h3><a class="tc" style="background-image:none;padding:0;">'+eleCate.find(".ele_caption").text()+'</a></h3></div>');
		}
	}
	
	
	
});