ie6 = false;
if(!-[1, ] && !window.XMLHttpRequest){
	ie6 = true;	
}
(function (win, UI, undefined) {
    if (win[UI] === undefined) {
        win[UI] = {};
    } else {
        return;
    }
    var mix = function (r, s, ov) {
        if (!s || !r) return r;
        if (ov === undefined) ov = true;
        for (var p in s) {
            if (ov || !(p in r)) {
                r[p] = s[p];
            }
        }
        return r;
    };
    UI = win[UI];
    mix(UI, {
        laterEvent: function (fn, times, me) {
            if (me.sleepid) {
                clearTimeout(me.sleepid);
            }
            me.sleepid = setTimeout(fn, times);
        },
		popWinA: function (s, op) {
            op = $.extend({
                opener: ".opener",
                openerC: ".openerC",
                addClass: "now"
            },op || {});
			
			$(s).each(function(){
				var $this=$(this),
					objOpner = $(this).find(op.opener),
					objC = $(this).find(op.openerC);
	
				$(this).hover(function () {
					$this.addClass(op.addClass);
					objC.show();
				}, function () {
					$this.removeClass(op.removeClass || op.addClass);
					objC.hide();
				});
			});
        },
		popWinB: function(s,hover,clickHide,delayTime,callback) {
			var objOpner = $("." + s + " .opener");
			var objShuter = $("." + s + " .shuter");
			var callback = callback || {funcIn:function(){},funcOut:function(){}};
			if(hover == false) {
				if (objOpner.length) {
					objOpner.click(function(){
					if($(this).parents(".over").length) {
						$(this).parents("." + s).removeClass("over");
						callback.funcOut.call(this,objOpner.parents("."+s),objOpner,objShuter);
					}
					else {
						$("." + s).removeClass("over");
							$(this).parents("." + s).addClass("over");
							callback.funcIn.call(this,objOpner.parents("."+s),objOpner,objShuter);
					}
					});
				}
				if (objShuter.length) {
					objShuter.click(function(){
						$(this).parents("." + s).removeClass("over");
						callback.funcOut.call(this,objOpner.parents("."+s),objOpner,objShuter);
					});
				}
				if(clickHide==true){
					$("html, body").bind("click",function(e){
						if($(e.target).parents("."+s).length==0){
							objOpner.parents("." + s).removeClass("over");
							callback.funcOut.call(this,objOpner.parents("."+s),objOpner,objShuter);
						}
					});
				}
				if(delayTime){
					objOpner.parents("." + s).hover(
					function(){
						objOpner.parents("." + s)[0].sleepid && clearTimeout(objOpner.parents("." + s)[0].sleepid);
					}, function(){
						UI.laterEvent(function(){
							objOpner.parents("." + s).removeClass("over");
							callback.funcOut.call(this,objOpner.parents("."+s),objOpner,objShuter);
						},delayTime,objOpner.parents("." + s)[0]);
					});
				}
			}
			if(hover == true) {
				if ($.browser.msie) {
					objOpner.parents("." + s).hover(
					function(){
						$(this).addClass("over");
						callback.funcIn.call(this,objOpner.parents("."+s),objOpner,objShuter);
					}, function(){
						$(this).removeClass("over");
						callback.funcOut.call(this,objOpner.parents("."+s),objOpner,objShuter);
					});
				}else {
					objOpner.parents("." + s).mouseover(
					function(){
						$(this).addClass("over");
						callback.funcIn.call(this,objOpner.parents("."+s),objOpner,objShuter);
					}).mouseout(function(){
						$(this).removeClass("over");
						callback.funcOut.call(this,objOpner.parents("."+s),objOpner,objShuter);
					});
				}
			}
		},
		popWinC: function (s, op) {
            op = $.extend({
                opener: ".opener",
                openerC: ".openerC",
                addClass: "now",
				funcIn : function(){},
				funcOut: function(){}
            },op || {});
			
			$(s).each(function(){
				var $this=$(this),
					objOpner = $(this).find(op.opener),
					objC = $(this).find(op.openerC);
	
				$(this).hover(function () {
					$this.addClass(op.addClass);
					op.funcIn.call(this,objC,op);
				}, function () {
					$this.removeClass(op.removeClass || op.addClass);
					op.funcOut.call(this,objC,op);
				});
			});
        },
		childUntil:function(expr,obj){
			if(obj.length==0) return null;
			var child=obj.children(expr);
			if(child.length==0){
				return arguments.callee(expr,obj.children());
			}else{
				return child;	
			}
		},
        defaultText: function (op) {
            op = $.extend(op || {},{
                hasDefaultText: ".hasDefaultText",
                removeClass: "hasDefaultText",
                addClass: "hasDefaultText"
            });
            var obj = $(op.hasDefaultText);
            var tmpText = new Array();
            var objIndex = 0;
            for (i = 1; i <= obj.length; i++) {
				tmpText[i-1] = obj.eq(i-1).attr("tip") ? obj.eq(i-1).attr("tip") : obj.eq(i-1).val();
				obj.eq(i-1).attr("tip",tmpText[i-1]);
				obj.eq(i-1).val(tmpText[i-1]);
            }
            obj.focus(function () {
                objIndex = obj.index($(this));
                if ($(this).val() == $(this).attr("tip")) {
                    $(this).val("");
                }
				$(this).removeClass(op.removeClass);
            });
            obj.blur(function () {
                objIndex = obj.index($(this));
                if ($(this).val() == "") {
                    $(this).val($(this).attr("tip"));
                    $(this).addClass(op.addClass);
                }
            });
        },
		cookie:{
			set:function(key, val, expires, domain, path, secure){
				if (!path) path = '/';
				if (!~~expires) expires = 365;
				expires = expires * 86400000;
				var e_date = new Date();
				e_date.setTime(+e_date + expires);
				document.cookie = 
					key + '=' + encodeURIComponent(val)
					+ (expires ? '; expires=' + e_date.toGMTString() : '')
					+ (domain ? '; domain=' + domain : '')
					+ (path ? '; path=' + path : '')
					+ (secure ? '; secure' : '');
			},
			get: function(key) {
				var c = document.cookie.split("; ");
				for (var i = 0; i < c.length; i++) {
					var p = c[i].split("=");
					if (key == p[0]) try { return decodeURIComponent(p[1]) } catch (e) { return null }
				}
				return null;
			},
			del: function(key, domain, path) {
				document.cookie = key + "=1" + (path ? "; path="+path:"; path=/")+(domain?"; domain="+domain:"")+";expires=Fri, 02-Jan-1970 00:00:00 GMT";
			},			
			getDomain:function(){
				return "." + location.host.split(".").slice(-2).join(".");
			}
		}

    });
})(window, "UI");

var tabPanel=function(obj){
	obj.each(function(){
		var $this=$(this),
			tabc=UI.childUntil(".tabc",$this.parent());
		$this.children("a:not([rel*='link'])").add($this.find(".tabitem")).each(function(n){
			$(this).attr("rel",n);	
		}).mouseenter(function(){
			if($this.attr("trigger")=="click"){return false;}
			$(this).addClass("now").siblings().removeClass("now");
			$(this).siblings("b[class*=hide]").removeClass("hide");
			$(this).prev("b").addClass("hide").andSelf().next("b").addClass("hide");
			tabc.hide().eq(parseInt($(this).attr("rel"))).show();
			if($(this).attr("command")){
				eval($(this).attr('command')+"(this)");
			}
		}).click(function(){			
			if($this.attr("enableLink")!="true"){
				if($this.attr("trigger")!="click"){return false;}	
			}
			$(this).addClass("now").siblings().removeClass("now");
			tabc.hide().eq(parseInt($(this).attr("rel"))).show();
			//if($(this).prev().is("input")){$(this).prev().attr("checked","checked")}
			if($(this).attr("command")){
				eval($(this).attr('command')+"(this)");
			}
			//return false;
		});
	});
}

/***
 * Customized Select Menu
 * Version: 0.0.1
 * Last Update:		2013.05.30
 * Author:				Sean Huang
 * Modified By:			Teller Shen, Dan Yang
 * JQuery Requires: 	1.6.2+
 * JS Method Requires:	UI.laterEvent()
 * CSS Style Requires:	.select
	
 * Call Method:		bindSelect() or bindSelect("#id .select") or bindSelect($("#id .select"))
***/
var bindSelect=function(_obj){
	var obj = (arguments.length!=0) ?  $(_obj) : $(".select");
	obj.mouseenter(function(){		
		if(this.sleepid) {
           clearTimeout(this.sleepid);
        }
	}).mouseleave(function(){
		var me=$(this);
		UI.laterEvent(function(){
			me.css({zIndex:0}).removeClass("select_expand").find("dd").hide();
			me.parents("li").css({zIndex:0});				
		},200,this);
	}).delegate("dd a","click",function(){
		var curselect=$(this).parents(".select");
		if(this.selected=="selected"){
			curselect.parents("li").css({zIndex:0});
			$(this).parents("dd").hide();
			return;
		}
		var text=$(this).html();
		$(this).parents("dd").prev("dt").find("a").html(text);
		$(this).parents("dd").find("a").each(function(){
			this.selected="";	
		});
		this.selected="selected";
		$(this).parent().addClass("selected").siblings("li").removeClass("selected");
		curselect.parents("li").css({zIndex:0});
		curselect.siblings("input[name='"+curselect.attr("name")+"']").val($(this).attr("value"));
		curselect.removeClass("select_expand").trigger("change");
		
		$(this).parents("dd").hide();
		
	}).end().find("dt").click(function(){
		var curselect=$(this).parent(".select");
		if(curselect.is(".disabled")){return false;}
		curselect.css({zIndex:1}).find("dd").toggle();
		curselect.toggleClass("select_expand");	
		curselect.parents("li").css({zIndex:1});
		//Start: Options Width & Height Fix		
		var ul_w=curselect.find("ul").width();
		var ul_h=curselect.find("ul").height();
		var dt_w=curselect.width();		
		if(ul_h>300) {
			ul_w+=15;
			ul_h=300;
		}
		
		if(ul_w<dt_w){
			curselect.find("ul").css({width:dt_w});
			ul_w=dt_w;
		}
		curselect.find("dd").css({width:ul_w,heigth:ul_h});
		//End: Options Width & Height Fix		
	});
	obj.each(function(){
		var val=$(this).find("dt a").html();
		$(this).find("dd a").each(function(){
			if($(this).html()==val){
				this.selected="selected";
				var curselect=$(this).parents(".select");
				$(this).parent().addClass("selected");
				curselect.siblings("input[name='"+curselect.attr("name")+"']").val($(this).attr("value"));
			}	
		});	
	});
}

$(function () {
    /* Default Input Place Holder */
    UI.defaultText();
	
	bindSelect();
	
	tabPanel($(".tab"));
	
	/* Fix Price_old */
	$(".price_old").each(function(){
		$(this).html("<s style='display:inline-block;margin-left:-11px;width:10px;'>&nbsp;&nbsp;&nbsp;</s>"+$(this).text());	
	});
	
	/* FadeSiblings */
	$(".fadeSiblings").children().each(function(i,v){
		var me=$(v);
		var bkbox=$("<div class='bkbox'></div>");
		bkbox.css({
		  "background-color":"#333",
		  "position":"absolute",
		  "height":me.outerHeight(),
		  "width":me.outerWidth(),
		  "left":0,
		  "top":0,
		  opacity:0.2  
		}).hide();
		me.append(bkbox);
	})
	$(".fadeSiblings").children().css({
		//"background-color":"#000",
		"position":"relative",
		"display":"block"	
	}).hover(function () {
		var me=$(this);
		//$(this).siblings().find(".bkbox").stop().show().animate({opacity:0.5},600,function(){me.find(".bkbox").hide();});
		$(this).siblings().find(".bkbox").show();
		me.find(".bkbox").hide();
    }, function () {		
		//$(this).siblings().find(".bkbox").stop().animate({opacity:0},400);
		$(this).siblings().find(".bkbox").hide();
    });
	
	/* Input text hover */
	$(".inputhover").live("focusin",function(){
		$(this).addClass("intxtfocus");	
	}).live("focusout",function(){
		$(this).removeClass("intxtfocus");	
	});
	
	
	//Navigation Hover show Submenu
	$(".navigation > ul > li").hover(function(){
		if(ie6){
			$(this).find(".submenu").width($(this).width()+1);	
		}
		$(this).addClass("hover");
	},function(){
		$(this).removeClass("hover");		
	});
	
	
	//menu mouseleave auto hidden;
	isindex=false;
	if($("#menu").find(".menulist:visible").length!=0){isindex=true;}
	$(".menu").hover(function(){
		if (this.sleepid) {
            clearTimeout(this.sleepid);        };
		if(isindex){return false;}
		$(this).toggleClass("menu_expand").find(".menulist").toggle();
	},function(){		
		if(isindex){return false;}
		var me=$(this);		
		UI.laterEvent(function () {
			me.removeClass("menu_expand");
			me.find(".menulist").hide();		
		},200,this);
	})
	
	//Cateogry Menu sub popup layer;
	var firstime=true;	
	var maindl=$(".menunavi").find("dl");
	var mtop=0;
	var pdl;
	var mclose=false;
	//var menuHeight = $("#menu").find(".menunavi").children("li").length * 38;
	$(".menunavi").hover(
		function(e){
			if (this.sleepid) {
				clearTimeout(this.sleepid);
			};
			$(this).parent().css("width",1000);
			 mclose=false;
			 //mtop=$("#menu").find(".menunavi").offset().top;
		},
		function(){	
			var me=$(this);			
				UI.laterEvent(function () {
					maindl.removeClass("now");
					$("select").css("visibility","visible");
					firstime=true;
					me.parent().css("width",205);
			},160,this);		 	
			
		});
	
	if($(".menunavi > li").length>7){
		$(".menulist .viewAllCate").show();
		$(".menunavi").height(413);	
	}
	else{
		$(".menulist .viewAllCate").hide();	
		$(".menunavi").height(429);	
	}
	
	$(".menulist .expand").toggle(function(){
		$(this).addClass("collapse");
		$(".menunavi").stop().animate({"height":649},500);
	},function(){
		$(this).removeClass("collapse");
		$(".menunavi").stop().animate({"height":413},500);
	});
	$(".menunavi").find(">li dt").hover(function(){
		var $this=$(this);
		
		mtop=$("#menu").length && $("#menu").offset().top;//get menunavi offset().top;
		UI.laterEvent(function(){
			if(mclose){
				firstime=true;
				mclose=false;
				return;						
			}
			var me=$this.parents("li");
			var dd=me.find(">dl>dd");
			//maindl.removeClass("now");
			if(pdl){
				pdl.removeClass("now");
			}
			pdl=me.find(">dl");
			pdl.addClass("now");
			pdl.find("table tr").first().addClass("firstTr");
			pdl.find("table tr.catebrand").next().addClass("noBorderTr");
			$("select").css("visibility","hidden");
			if(firstime){
				me.find(".ddwrap").css({
					marginLeft:-776
				}).animate({
					marginLeft:0
				},200,"easeOutQuint");
			}
			firstime=false;				
			if(!$this[0].ddtop){
				var h = dd.height(),
				curH = $(me).offset().top - mtop,
				toTop = curH-h+18;
				toTop=toTop<0?0:toTop;
				dd.css("top", toTop);
				$this[0].ddtop=true;
			}
		},200,$this[0]);			
		 
		
	},function(){
		if(this.sleepid){
			clearTimeout(this.sleepid);
		}
	});
   $(".menunavi").find(".close").click(function () {
        maindl.removeClass("now");
		$("select").css("visibility","visible");
		firstime=true;
		mclose=true;
        return false;
    });
	
	//Mycart Popup;
	$(".mycart").hover(function(){
		var $this=$(this);
		UI.laterEvent(function(){
			if($this.find("li").length==0){
				$this.find("dt").removeClass("dt_hover");
				$this.find("dd").hide();
			}else{
				$this.find("dt").addClass("dt_hover");
				$this.find("dd").show();
			}
		},200,this);
	},function(){
		var $this=$(this);
		UI.laterEvent(function(){
			$this.find("dt").removeClass("dt_hover");
			$this.find("dd").hide();
		},200,this);
		
	});
	
	/* Mycart Product line Hover Highlight */
	$(".mycart .prolist li").hover(
		function(){
			$this = $(this);
			$this.addClass("cur");	
		},
		function(){
			$this = $(this);
			$this.removeClass("cur");	
		}
	
	);
	
	$(".mycart .prolist .btn_del").hover(
		function(){
			$(this).parents("li").addClass("hover");
		},
		function(){
			$(this).parents("li").removeClass("hover");
		}
	);
   
   /*  Mycart Popup XSlider*/
   $(".mycart .listwrap").each(function(k,v){
		var $this = $(this);
		var li = $this.find(".prolist").children("li");
		if(li.length>4){
			$this.find(".slidewrap").height(236);
			$this.find(".abtn").css("display","block");
			$this.Xslider({
				dir:"V",
				viewedSize:236,
				unitLen: 59,
				unitDisplayed:34,
				numtoMove:4,
				speed:600
			});
		}   
	});

	
	//Float Fucntions;
	var floatFunc = $(".floatFunc");
	var wraperDom = $(".wraper").first();
	//var floatSpace  = 101;
	var floatSpace  = 74;
	var toRight=($(window).width()-wraperDom.width())/2-floatSpace;
	toRight=toRight>0 ? toRight :0;
	
    $(".toTop").click(function () {
        $($.browser.safari || document.compatMode == 'BackCompat' ? document.body : document.documentElement).animate({
            scrollTop: 0
        }, 200);
        return false;
    });
	
	floatFunc.css({
		right:toRight	
	});
	
	$(window).bind("scroll resize",function(){
		var sctop=document.documentElement.scrollTop?document.documentElement.scrollTop:document.body.scrollTop;
		toRight=($(window).width()-wraperDom.width())/2-floatSpace;
		toRight=toRight>0 ? toRight :0;
		floatFunc.css({
			right:toRight	
		});
		/*if(sctop>130){
			floatFunc.css({
				right:toRight	
			})//.fadeIn(300);		
		}else{
			//floatFunc.fadeOut(400);
		}*/
	});
	
	/* Topbar pop submenu */
	$(".topnav .expand").hover(function(){
		$(this).addClass("ex_hover");
		if(ie6){
			$(this).find("dd").width($(this).outerWidth()+1);				
		}			
	},function(){
		$(this).removeClass("ex_hover");	
	})
	

});