/***
 * Global Popup Window & Layer
 * Version: 1.0.2
 * Last Update:		2013.10.23
 * Author:				Dan Yang
 * JQuery Requires: 	1.4.2+
 * Weibo:				http://weibo.com/divayang
 * Email:				diva83@163.com
 
 *---------------------------------------------------------------------------
 * Bugs Pending to Fix:
 	
	1、Bug 描述: 多次调用$.ShowConfirm("***"，{settings})时，首次调用时settings的属性会一直驻留，后面调用的settings无法生效;
		
	   Bug 原因: 首次调用后，if(confirmbox && confirmobj)语句始终成立，后续调用时没有对弹窗对象进行重新初始化，故首页配置参数一直会生效;
	
	   Bug 可选解决文案: 
	   	
			A: 将if else条件语句删除，每次调用时均生成一个全新的html DOM，并全新生成弹窗对像，所以事件也互相独立；
			B: 为PopWin方法添加init()方法，每次调用$.ShowConfirm时使用配置参数对窗口对象重新初始化
			C(目前使用的临时解决方法):
			
			添加全局配置变量（见代码主体）：_confirmSetting
		
 *---------------------------------------------------------------------------
 
 * Updates Logs:
 
 * Version: 1.0.2:
 	
	1、修正了overlay Bug: 当多个对话窗口存在时，关闭窗口overlay不应该消失，改为直到所有窗口都关闭才消失；

 *---------------------------------------------------------------------------
	
 * Demo:
 
		////Demo 1： 一般用于在页面中仅弹出一次对话窗口的情况
		
		方式1： 
		PopWin("#popID");
		
		方式2：
		var myPopWin = document.getElementById("popWinID");
		PopWin(myPopWin,{...});
		
		
		////Demo 2： 一般用于在页面中对弹出窗口进行多次操作的情况
		
		//调用构造函数并将返回的对话窗口对象赋值给变量
		var myPopWin = PopWin(							
			"#popID",									//String | object， 必填项，用于指定对话窗口，可以是ID、Class、HTML DOM对象或JQuery对象
			{
				action:	"in" | "out",  					//String，可选项，用于指定将对话窗口在调用构造函数时即可弹入|弹出，此参考与obj.fn.popIn()|obj.fn.popOut()方法不可同时使用
				animate:	false | true, 				//Boolean，可选项，对话窗口是否以动画形式弹入|弹走
				speed:	200,							//Number，可选项，对话窗口弹入|弹走的速度
				overlay:	"#overlay" | overlay 		//String | object，可选项，用于指定遮层，可以是ID、Class、HTML DOM对象或JQuery对象
				olSpeed:	200, 						//Number，可选项，遮层的出现|消失的动画速度，禁用设置为0即可
				olOpacity:	0.5,						//Float Number, 可选项，
				
				//待添加参考Start
				follow:		true | false | "auto"		//Boolean | "auto"，可选项，用于指定对话窗口是否跟随页面滚动，"auto"时自动判断弹窗口与浏览器可视区域的高度来实现跟随与否
				referObj: 	"#referObj" | HTMLElement | JQuery Obj   //String | object，可选项，用于指定对话窗口弹出位置的参考对像，默认为document.body
				offsetX:	0;							//Number，可选项，用于指定弹出窗口的水平位置的偏移量
				offsetY: 	0;							//Number，可选项，用于指定弹出窗口的重直位置的偏移量
				//待添加参考End
				
				beforeStart:	funciton(), 			//function，可选项，在弹入窗口前执行的方法
				callOnce:	funciton(), 				//function，可选项，仅在第一次弹入窗口动作完成后执行的方法
				callback:	funciton(), 				//function，可选项，在每次弹入窗口动作完成后执行的方法
				afterPopOut:	function()				//function，可选项，在每次弹走窗口动作完成后执行的方法
			}
		);
		
		//对话窗口对象调用内部方法
		myPopWin.fn.popIn(false); 	//对话窗口动作： 无动画弹入
		myPopWin.fn.popOut(false); 	//对话窗口动作： 无动画弹走
		myPopWin.fn.popIn(true); 	//对话窗口动作： 动画弹入
		myPopWin.fn.popOut(true); 	//对话窗口动作： 动画弹走
		
		
***/


/**	Global Method to fix Mask Overlay Height & Width **/
function fixMaskLayer(_o){
	//if IE6
	if(!-[1, ] && !window.XMLHttpRequest){
		var o = $(_o);
		var win = $(window);
		setTimeout(function(){
			o.css({width:win.width(),height:win.height()});
		},0);
	}
	
}

/**	Mousewheel Plugin **/
/* Copyright (c) 2006 Brandon Aaron (brandon.aaron@gmail.com || http://brandonaaron.net)
 * Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
 * Thanks to: http://adomas.org/javascript-mouse-wheel/ for some pointers.
 * Thanks to: Mathias Bank(http://www.mathias-bank.de) for a scope bug fix.
 *
 * $LastChangedDate: 2007-12-20 09:02:08 -0600 (Thu, 20 Dec 2007) $
 * $Rev: 4265 $
 *
 * Version: 3.0
 * 
 * Requires: $ 1.2.2+
 */

(function($) {
$.event.special.mousewheel = {
	setup: function() {
		var handler = $.event.special.mousewheel.handler;
		
		// Fix pageX, pageY, clientX and clientY for mozilla
		if ( $.browser.mozilla )
			$(this).bind('mousemove.mousewheel', function(event) {
				$.data(this, 'mwcursorposdata', {
					pageX: event.pageX,
					pageY: event.pageY,
					clientX: event.clientX,
					clientY: event.clientY
				});
			});
	
		if ( this.addEventListener )
			this.addEventListener( ($.browser.mozilla ? 'DOMMouseScroll' : 'mousewheel'), handler, false);
		else
			this.onmousewheel = handler;
	},
	
	teardown: function() {
		var handler = $.event.special.mousewheel.handler;
		
		$(this).unbind('mousemove.mousewheel');
		
		if ( this.removeEventListener )
			this.removeEventListener( ($.browser.mozilla ? 'DOMMouseScroll' : 'mousewheel'), handler, false);
		else
			this.onmousewheel = function(){};
		
		$.removeData(this, 'mwcursorposdata');
	},
	
	handler: function(event) {
		var args = Array.prototype.slice.call( arguments, 1 );
		
		event = $.event.fix(event || window.event);
		// Get correct pageX, pageY, clientX and clientY for mozilla
		$.extend( event, $.data(this, 'mwcursorposdata') || {} );
		var delta = 0, returnValue = true;
		
		if ( event.wheelDelta ) delta = event.wheelDelta/120;
		if ( event.detail     ) delta = -event.detail/3;
		//if ( $.browser.opera  ) delta = -event.wheelDelta;
		
		event.data  = event.data || {};
		event.type  = "mousewheel";
		
		// Add delta to the front of the arguments
		args.unshift(delta);
		// Add event to the front of the arguments
		args.unshift(event);

		return $.event.handle.apply(this, args);
	}
};

$.fn.extend({
	mousewheel: function(fn) {
		return fn ? this.bind("mousewheel", fn) : this.trigger("mousewheel");
	},
	
	unmousewheel: function(fn) {
		return this.unbind("mousewheel", fn);
	}
});

})(jQuery);


/**	PopWin Constructor **/
function PopWin(_o, _settings){
	var self		= 	this;
	if (!(self instanceof PopWin)) {
        return new PopWin(_o, _settings);
    }
	
	var o 			= 	$(_o);
	var ol			= 	null;
	var settings	= 	{};
	var _default 	= 	{
			x			:	( $(window).width() - o.outerWidth() ) / 2,
			y			:	( $(window).height() - o.outerHeight() ) / 2,
			action		:	"in",
			animate		: 	false,
			speed		:	200,
			overlay		:	"#overlay",
			olSpeed		:	200,
			olOpacity	:	0.5,
			queue		: 	false,
			beforeStart	: 	function(){},
			callOnce	:  	function(){},
			callback	: 	function(){},
			afterPopOut	: 	function(){}
	}
	
	_default.y	= (document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop) + (( _default.y > 0 ) ? _default.y : 0);
	
	if(_settings){ 
		for(var v in _settings){
			_default[v] = _settings[v];//Overwite defaults settings if customed parameters parsed in
		}
	}
	
	settings = _default;
	ol = $(settings.overlay);//Overlay DOM Obj
	
	//fix functions
	var fixFunc = {
		preventScroll : function(){
			return false;	
		},
		preventResize : function(){
			return false;	
		}	
		
	};
	
	//funciton init
	self.initDOM = function(){
		//overlay style
		if(ol.length==0){
			ol = $('<div id="overlay" style="position:fixed;top:0;left:0;right:0;bottom:0;"></div>');
			//if IE6
			if(!-[1, ] && !window.XMLHttpRequest){
				ol =  $('<div id="overlay" style="position:absolute;width:0;height:0;"><div style="position:absolute;width:100%;height:100%;background-color:transparent;z-index:1;"></div><iframe height="100%" frameborder="0" width="100%" scrolling="no" style="filter:alpha(opacity=0);background-color:transparent;"></iframe></div>');	
				var css = document.createElement("style");
				css.setAttribute("type","text/css");
				if(css.styleSheet){//For IE
					css.styleSheet.cssText = '#overlay,#overlay_transparent{top:expression(documentElement.scrollTop);left:expression(documentElement.scrollLeft);}';
				}
				document.getElementsByTagName("head")[0].appendChild(css);
			}
			$("body").append(ol);
			var olBgColor = ol.css("backgroundColor"),
				olBgImg = ol.css("backgroundImage"),
				olBgRpt = ol.css("backgroundRepeat"),
				olBgPst = ol.css("backgroundPosition");
			olBgColor = (olBgColor) ? olBgColor : "#000";
			olBgImg = (olBgImg) ? olBgImg : "url(base64:*.gif)";
			olBgRpt = (olBgRpt) ? olBgRpt : "repeat";
			olBgPst = (olBgPst) ? olBgPst : "left top";
			ol.css({
				backgroundColor: olBgColor,
				backgroundImage: olBgImg,	
				backgroundRepeat: olBgRpt,
				backgroundPosition: olBgPst
			});
		}
		else {
			//if IE6
			if(!-[1, ] && !window.XMLHttpRequest){
				//ol.css("left",0);
			}else{
				ol.css({				
					top: 0,	
					left: 0,
					right:0,
					bottom:0
				});	
			}
		}
		
		//Pop Window style
		if(o.length!=0){
			o.css({
				left : "50%",
				top	: 0 - o.outerHeight(),
				marginLeft : 0 - o.outerWidth() / 2
			});
			if(o.css("zIndex")=="auto" || o.css("zIndex")== 0){
				o.css("zIndex",1000);
			}
			ol.css("zIndex",parseInt(o.css("zIndex"))-1);
		}
		
		self.ol = ol;
		
		//System Evirement Vars
		/*if(typeof PopWinObj == 'undefined' ){
			PopWinObj = {};
			PopWinObj.Objects = [];
			PopWinObj.ObjDoms = [];
			PopWinObj.ObjDoms_Showed = [];
			PopWinObj.ObjDoms_Hidden = [];
			PopWinObj.ObjDoms_Overlay = [];
			PopWinObj.ObjOverlay_Showed = [];
			
			PopWinObj.O = PopWinObj.Objects;
			PopWinObj.OD = PopWinObj.ObjDoms;
			PopWinObj.ODS = PopWinObj.ObjDoms_Showed;
			PopWinObj.ODH = PopWinObj.ObjDoms_Hidden;
			PopWinObj.ODO = PopWinObj.ObjDoms_Overlay;
			PopWinObj.OOS = PopWinObj.ObjOverlay_Showed;
		}
		o.length!=0 ? PopWinObj.O.push(self) : '';
		o.length!=0 ? PopWinObj.OD.push(o) : '';
		o.length!=0 ? PopWinObj.ODO.push(self.ol) : '';*/
		
	};//init end
	
	self.initDOM();
	settings.callOnce.call(o,o,self);
	
	//function obj
	self.fn	= {
		popIn: 	
			function(_a, _fn,_data){
				if(o.attr("moving") == "1" ) return false;
				if(o.attr("showed") == "1" ) return false;
				
				settings.animate = (_a != undefined) ? _a : settings.animate;
				self.fn.specialFix("beforeIn");
				
				//Call Customized BeforeStart Function
				if(settings.beforeStart){
					settings.beforeStart.call(o,o,_data);		
				}
				
				//Overlay Control for Multiple PopWin Layers
				var olSpeed, olInitOpacity;
				if(self.ol.data('popShowedNum')>=1){
					self.ol.data('popShowedNum',~~self.ol.data('popShowedNum')+1)	
					olSpeed = 0;
					olInitOpacity = settings.olOpacity;
				}else {
					self.ol.data('popShowedNum',1);
					olSpeed = settings.olSpeed;
					olInitOpacity = 0;
				}
				
				if(settings.animate == true) {
					self.fn.domMouseWheel(false);
					o.attr("moving","1");
					fixMaskLayer(ol);
					//ol.css({opacity : 0, display : "block"}).animate({opacity : settings.olOpacity},settings.olSpeed,
					ol.css({opacity : olInitOpacity, display : "block"}).animate({opacity : settings.olOpacity},olSpeed,
						function(){
							o.css("top",self.fn.getEndPos().inY).show().animate({
								top	 :	self.fn.getEndPos().y
							},{
								duration : 	settings.speed,
								queue	 :	settings.queue,
								complete : 	function(){
									//Call Customized Callback Function
									settings.callback.call(o,o);
									(typeof _fn == "function") ? _fn.call(o,o) : '';
									o.attr("showed","1");
									o.attr("moving","0");
									self.fn.addEvent();
									self.fn.domMouseWheel(true);
								}
							});	
						}
					);
				}
				else {
					ol.css({opacity : settings.olOpacity, display : "block"});
					fixMaskLayer(ol);
					o.show().css({
						top	 :	self.fn.getEndPos().y
					});
					//Call Customized BeforeStart Function
					settings.callback.call(o,o);
					(typeof _fn == "function") ? _fn.call(o,o) : '';	
					o.attr("showed","1");
					o.attr("moving","0");
					self.fn.addEvent();
				}	
			},//popIn End
		
		popOut	: 	
			function(_a, _fn){
				if(o.attr("moving") == "1" ) return false;
				if(o.attr("showed") != "1" ) return false;
				settings.animate = (_a != undefined) ? _a : settings.animate;
				
				if(settings.animate == true) {
					self.fn.domMouseWheel(false);
					o.attr("moving","1").show().animate({
						top	 :	self.fn.getEndPos().outY
					},{
						duration : 	settings.speed,
						queue	 :	settings.queue,
						complete : 	function(){
							//Call Customized Callback Function
							o.hide().attr("showed","0");
							o.attr("moving","0");
							//ol.fadeOut(settings.olSpeed);
							if(self.ol.data("popShowedNum")<=1){
								self.ol.fadeOut(settings.olSpeed).data("popShowedNum",0);
							}else {
								self.ol.data("popShowedNum",self.ol.data("popShowedNum")-1);
							}
							settings.afterPopOut.call(o,o);
							(typeof _fn == "function") ? _fn.call(o,o) : '';
							self.fn.specialFix("afterOut");
							self.fn.domMouseWheel(true);
						}
					});
				}
				else {
					o.show().css({
						top	 :	0 - self.fn.getEndPos().y,
						display: "none"
					});
					//Call Customized BeforeStart Function
					o.attr("showed","0");
					o.attr("moving","0");
					//ol.hide();
					if(self.ol.data("popShowedNum")<=1){
						self.ol.hide().data("popShowedNum",0);
					}else {
						self.ol.data("popShowedNum",self.ol.data("popShowedNum")-1);
					}
					settings.afterPopOut.call(o,o);
					(typeof _fn == "function") ? _fn.call(o,o) : '';
					self.fn.specialFix("afterOut");
				}	
			},//popOut end
		
		getEndPos :
			function(){
				var st = (document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop);
				//var sl = (document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft);
				var win = $(window);
				var _y = ( win.height() - o.outerHeight() )/2;
				//var _x = ( win.width() - o.outerWidth() )/2;
				return {
					//x : sl + ((_x > 0 ) ? _x : 0),
					y : st + ((_y > 0 ) ? _y : 0),
					//curTop : st,
					inY : 0 - o.outerHeight() + st,
					outY : 0 - o.outerHeight() + st
				}
			},//getEndPos end
		
		addEvent ://events which runs after popWindow finishing Entering 
			function(){
				//When Scroll & Resize Window, Reposition the PopWindow
				$(window).bind("resize scroll",function(e){
					( e.type == "resize") && fixMaskLayer(ol);
					self.fn.rePosition();
				});
				
				//More Events could happen here....
				//.....
				
			},//addEvent End
				
		rePosition:
			function(_a){
				//if(o.attr("moving") == "1" ) return false;
				if(o.attr("showed") != "1" ) return false;
				settings.animate = (_a!=undefined) ? _a : settings.animate;
				if(settings.animate == true) {
					o.attr("moving","1");
					o.show().animate({
						top	 :	self.fn.getEndPos().y
					},{
						duration : 	200,
						queue	 :	settings.queue,
						complete : 	function(){
							o.attr("moving","0");
							self.fn.specialFix("onMoving");
						}
					});	
				}
				else {
					o.show().css({
						top	 :	self.fn.getEndPos().y
					});
					o.attr("moving","0");
					self.fn.specialFix("onMoving");
				}
				
			},//rePostion end
			
		domMouseWheel :
			function(_f){
				if(_f == false){
					$(window).bind("resize",fixFunc.preventResize);	
					$(document).bind("mousewheel",fixFunc.preventScroll);		
				}
				else {
					$(window).unbind("resize",fixFunc.preventResize);	
					$(document).unbind("mousewheel",fixFunc.preventScroll);	
				}
			},
		
		specialFix ://Special Fix Events which runs Before popIn, PopWin Moving and After popOut
			function(_f){
				//if IE
				if(!!window.ActiveXObject){
					if((_f == "beforeIn") || (_f == "onMoving")){
						var focusedInput = $("input:focus").not(o.find("input:focus"));//not support for jquery 1.4.2 and below
						if(focusedInput.length!==0){
							window.data_tempFocusInput = focusedInput;
							data_tempFocusInput.blur();
							window.data_bluredInput = window.data_tempFocusInput;
						}
					}
					else if(_f == "afterOut"){
						if(window.data_bluredInput){
							data_bluredInput.focus();
							window.data_tempFocusInput = null;	
							window.data_bluredInput = null;
						}
					}
				}
				
				//if IE6
				if(!-[1, ] && !window.XMLHttpRequest){
					if(_f == "beforeIn") {
						$("select").not(o.find("select")).css("visibility","hidden");	
					}
					else if(_f == "afterOut"){
						$("select").not(o.find("select")).css("visibility","visible");	
					}
					
					/*for(var i = 0, selects = document.getElementsByTagName("select"); i<selects.length; i++){
						selects[i].style.visibility = v;	
					}*/
					
				}
				
				//More Special Fix could happens here....
				//.....
				
			}// specialFix end
	}//fn end
	
	//Bind click Event to Close Icon	
	o.find(".close").not("[data-popwin-bound=true]").bind("click",function(){
		self.fn.popOut(settings.animate);		
	}).attr("data-popwin-bound","true");
	
	if(_settings){
		//初始化对象时，如果参数中设置了action为"in"|"out"，则执行相应的弹入|弹出的动作
		(_settings.action == "in") ? self.fn.popIn(_settings.animate) : "";
		(_settings.action == "out") ? self.fn.popOut(_settings.animate) : "";		
	}
	
	return self;
}


/** Simulate Window.alert： Init Method **/
var initAlertBox=function(obj,sett){
	return PopWin(obj,{
		animate:true,
		olOpacity:0.5,
		callback:function(o){
			if(!-[1, ] && !window.XMLHttpRequest){
				try{
					DD_belatedPNG.fix('.ie6png2');
				}catch(e){}
			}
		},
		callOnce: function(o){
			//初始化弹出层后仅执行一次的内容
			o.find(".btns .btn").click(function(){
				sett.callback.call(o,o,$(this));	
				(sett.preventClose !== true) && o.find(".Alert_close").trigger("click");
			})
			o.find(".iframe iframe").height(o.outerHeight(true));
		},
		beforeStart:function(o){
			sett.onStart.call(o,o);	
		},
		afterPopOut:function(o){
			sett.afterPopOut.call(o,o);
		}
	});
} 

$.Showmsg = function(msgHtml,settings){
	var alertbox = window["alertTemplate"];
	var alertobj = window["alertobj"];
	var btnsHtml;
	var maskIframe = "";
	var sett = $.extend({
		caption: "",
		preventClose:false,
		callback:function(){},
		onStart:function(){},
		afterPopOut:function(){}
	},settings);
	
	if(alertbox && alertobj){
		//$(".AlertBox").find(".Alert_info").empty();
	}
	else {
		maskIframe = (!-[1, ] && !window.XMLHttpRequest) ? '<div class="iframe"><iframe height="100%" frameborder="0" width="100%" scrolling="no"></iframe></div>' : '';
		btnsHtml = '<div class="btns tc"><a class="btn btn0" href="javascript:void(0)"><span>确 定</span></a></div>';
		alertbox = $('<div id="AlertBox'+(new Date()).getTime()+'" class="AlertBox"><div class="Alert_title ie6png2">'+sett.caption+'<a href="javascript:void(0);" class="Alert_close close">χ</a></div><div class="Alert_info ie6png2"><div class="msg_txt tc mb20"></div>'+btnsHtml+'</div>'+maskIframe+'</div>');
		window["alertTemplate"]=alertbox;
		$("body").append(alertbox);
		window["alertobj"]=initAlertBox(alertbox,sett);
		alertobj=window["alertobj"];
	}
	alertbox.find(".msg_txt").html(msgHtml);
	alertobj.fn.popIn();
}

/** Simulate Window.confirm： Init Method **/
var initConfirmBox=function(obj,sett){
	return PopWin(obj,{
		animate:true,
		olOpacity:0.5,
		callback:function(o){
			if(!-[1, ] && !window.XMLHttpRequest){
				try{
					DD_belatedPNG.fix('.ie6png2');
				}catch(e){}
			}
		},
		callOnce: function(o){
			//初始化弹出层后仅执行一次的内容
			o.find(".btns .btn0").click(function(){
				sett.callback.call(o,o,$(this));	
				(sett.preventClose !== true) && o.find(".Alert_close").trigger("click");
			})
			o.find(".btns .btn1").click(function(){
				sett.cancelCallback.call(o,o,$(this));
				o.find(".Alert_close").trigger("click");
			})
			o.find(".iframe iframe").height(o.outerHeight(true));
		},
		beforeStart:function(o){
			sett.onStart.call(o,o);	
		},
		afterPopOut:function(o){
			sett.afterPopOut.call(o,o);
		}
	});
} 

var _confirmSetting = { 
	caption: "", 
	preventClose: false, 
	callback: function() {}, 
	cancelCallback: function (){}, 
	onStart: function(){}, 
	afterPopOut: function() {} 
}; 
	
$.ShowConfirm = function(msgHtml,settings){	
	var confirmbox = window["confirmTemplate"];	
	var confirmobj = window["confirmobj"];
	var btnsHtml;
	var maskIframe = "";
	/*var sett = $.extend({
		caption: "",
		preventClose:false,
		callback:function(){},
		cancelCallback:function(){},
		onStart:function(){},
		afterPopOut:function(){}
	},settings);*/
	
	$.extend(_confirmSetting,settings);

	if(confirmbox && confirmobj){
		//$(".AlertBox").find(".Alert_info").empty();
	}
	else {
		maskIframe = (!-[1, ] && !window.XMLHttpRequest) ? '<div class="iframe"><iframe height="100%" frameborder="0" width="100%" scrolling="no"></iframe></div>' : '';
		btnsHtml = '<div class="btns tc"><a class="btn btn0 mr10" rel="0" href="javascript:void(0);"><span>确 定</span></a><a class="btn btn1" rel="1" href="javascript:void(0);"><span>取 消</span></a></div>';
		confirmbox = $('<div id="ConfirmBox'+(new Date()).getTime()+'" class="AlertBox"><div class="Alert_title ie6png2">'+_confirmSetting.caption+'<a href="javascript:void(0);" class="Alert_close close">χ</a></div><div class="Alert_info ie6png2"><div class="msg_txt tc mb20"></div>'+btnsHtml+'</div>'+maskIframe+'</div>');
		window["confirmTemplate"]=confirmbox;
		$("body").append(confirmbox);
		window["confirmobj"]=initConfirmBox(confirmbox,_confirmSetting);
		confirmobj=window["confirmobj"];
	}
	confirmbox.find(".msg_txt").html(msgHtml);
	confirmobj.fn.popIn();
	
}