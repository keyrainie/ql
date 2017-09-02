$(function(){
	//购物车页面对“收藏、删除”功能不做全并，设置与对应商品高度对应
	$(".cartlist td.op .operbox").each(function(i){
		var thisTr = $(this).closest("tr");
		var pro = thisTr.find("td.title .pro");
		$(this).height(pro.eq(i).outerHeight(true));
	});
	
	/*UI.Xslider(".sliderA",{
		numtoMove:1,
		unitLen:980,
		scrollObjSize:980*Math.ceil($(".sliderA .prolist li").length / 4),
		showNav:".nav a"
	});	*/
	/*UI.Xslider(".sliderB",{
		numtoMove:1,
		scrollObjSize:980*Math.ceil($(".sliderB .prolist li").length / 4),
		showNav:".nav a"
	});	*/
	
	//"热卖推荐"、"最近浏览滚动"
	$(".sliderB").each(function(){
		var $this = $(this); 
			UI.Xslider($this,{
			numtoMove:1,
			unitLen:984,
			scrollObjSize:984*Math.ceil($(".prolist li",$this).length / 4)
			//showNav:".nav a"
		});	
	});
	 
	
	
	/*UI.Xslider(".sliderC",{
		numtoMove:4,		
		showNav:".nav a"
	});*/
	
	
	/*$(".delslider").hover(function(){
		$(".abtnp",this).stop(true,true).fadeIn()
	},function(){
		$(".abtnp",this).stop(true,true).fadeOut()
	});*/
	
	
	/*购物车底部商品列表价格及加入购物车按钮hover切换*/
	/*$(".prolistRecommend .prolist .pp").hover(function(){
		$(this).addClass("pp_hover");		
		},function(){
		$(this).removeClass("pp_hover");
	});*/
	
	
	//Checkout页面"收货人信息"
	/*$(".myaddrlist > li > label").bind("click",function(e){
		var currline = $(this).parent("li");		
		currline.addClass("curr").siblings("li.curr").removeClass("curr");
		if(currline.hasClass("j-newaddr")){
			$(".newaddr").show();
		}
		else{
			$(".newaddr").hide();
		}
	});*/
	$(".myaddrlist > li").bind("mouseenter",function(){
		$(this).addClass("li-hover");
	}).bind("mouseleave",function(){
		$(this).removeClass("li-hover");
	});
	
	//Checkout页面"支付方式"
	$(".pay_cod > li > label").bind("click",function(e){
		var currline = $(this).parent("li");		
		currline.addClass("curr").siblings("li.curr").removeClass("curr");
	});
	
	//Checkout页面"配送方式"
	$(".post_cod > li > label").bind("click",function(e){
		var currline = $(this).parent("li");		
		currline.addClass("curr").siblings("li.curr").removeClass("curr");
		$(".self_post_r").removeClass("curr");
		$(".self_post_inner").hide();
		if(ie6){
			$(this).find("input[type='radio']").attr("checked","true");
		}
	});
	$(".self_post_r label").bind("click",function(e){
		$(".self_post_r").addClass("curr");
		$(".self_post_inner").show();
		$(".post_cod > li.curr").removeClass("curr");
		if(ie6){
			$(this).find("input[type='radio']").attr("checked","true");
		}
	});
	if(ie6){
		$(".self_post_inner_cod > li > label").bind("click",function(e){
			$(this).find("input[type='radio']").attr("checked","true");	
		});
	}
	
	//切换开据发票内容
	$(".invest_need").click(function(e){
		if($(this).find("input").attr("checked")){
			$(this).siblings(".investc").show();
			$(this).parent(".invest").find(".action").removeClass("action-state2");	
		}else{
			$(this).siblings(".investc").hide();
			$(this).parent(".invest").find(".action").addClass("action-state2");	
		}
	});
	$(".invest_noneed").click(function(e){
		if($(this).find("input").attr("checked")){
			$(this).siblings(".investc").hide();
			$(this).parent(".invest").find(".action").addClass("action-state2");
		}else{
			$(this).siblings(".investc").show();
			$(this).parent(".invest").find(".action").removeClass("action-state2");	
		}
	});
	
	/*if(ie6){
		$("input[type='radio']").each(function(){
			$this = $(this);
			var id;
			if($this.attr("id")){
				if($this.parent("label").attr("for")!=$this.attr("id")){
					$this.parent("label").attr("for",$this.attr("id"));
				}
			}
			else{
				id ="radio" + (new Date()).getTime();
				//id = $this.index();
				$this.parent("label").attr("for",id);
				$this.attr("id",id);
			}			
		});
	}*/
	
	
		
	//提交订单BOX中使用优惠券等功能
	$("#discount dt s").toggle(function(){
		$(this).find(".icon").addClass("now");
		$(this).parent().next().slideDown();
		//$("#discount dt").next().slideDown();
	},function(){
		$(this).find(".icon").removeClass("now");
		$(this).parent().next().slideUp();
		//$("#discount dt").next().slideUp();
	});
	
	//表单验证
	if($(".formsub").length>0){
		$(".formsub").Validform({
			tiptype:function(msg,o,cssctl){
				if(!o.obj.is("form")){
					
					var objtip=o.obj.parent("li").find(".Validform_checktip");
					if(o.type==2){
						o.obj.parent("li").find(".tipmsg").hide();
					}else{
						o.obj.parent("li").find(".tipmsg").show();
					}
					
					cssctl(objtip,o.type);
					objtip.text(msg);
				}			
			}		
		
			
		});
	}
	/*$(".combobox .intnobrd").blur(function(){	
		var me=$(this);	
		var lia=$(this).parent().find(".select dd a");
		lia.each(function(i,v){
			this.selected=false;
			if($(this).html()==me.val()){
				this.selected="selected";
			}	
		})
	})
	$("#addnewcord1").click(function(){
		$("#newcord1").show();
		$(this).parent().hide();
	})
	$("#addnewcord2").click(function(){
		$("#newcord2").show();
		$(this).parent().hide();
	})*/
	
	if(ie6){
		/* Paytype Tab */
		$(".paytype .tab .tabitem").each(function(){
			var fix = $(this).index();
			$(this).attr("for","paytype_radio_"+fix);
			$(this).find("input").attr("id","paytype_radio_"+fix);
		});
		$(".investc .tab .tabitem").each(function(){
			var fix = $(this).index();
			$(this).attr("for","invest_radio_"+fix);
			$(this).find("input").attr("id","invest_radio_"+fix);
		});
		$(".pay_cod").each(function(){
			var fix = new Date().getTime();
			$(this).find("label").each(function(){
				fix = fix  + "" + $(this).index();
				$(this).attr("for","invest_radio_"+fix);
				$(this).find("input").attr("id","invest_radio_"+fix);
			})
		});
	}
	
	// Paytype Banklist hover & click @ Thankyou 
	$(".banklist li input,.banklist li img,.banklist li a").click(function () {
		if($(this).closest(".bank-selected").length!=0) return false;
		$(this).parents("li").find("input").attr("checked","checked");
		//$(".banklist li").removeClass("current");
		//$(this).parents("li").addClass("current");
			
		$(this).closest(".banklist").hide("fast");
		var s = $(this).parents("li").clone();
		setTimeout(function(){
			$(".bank-selected").show().find(".banklistalter").html(s);
		},0);
	});
	
	//点击“选择其它银行”
	$(".bank-selected").find(".bw-return").click(function(){
		$(".paytype > .tab .now").click();
	});
	
	window.resetPaytype = function(){
	    $(".bank-selected").hide();
	    $(".action").hide();
	}
	// Remove border for IE6
	$(".banklist li a").focus(function(){this.blur();});
})