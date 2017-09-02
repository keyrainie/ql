var isSupportTouch = "ontouchstart" in window || "ontouchend" in window.document;
var _tapClick = isSupportTouch ? "tap" : "click"; //正式环境使用  PS:如果不使用此方法，可以使用Fastclick.js插件代替
var isOldWebKit = +navigator.userAgent.toLowerCase().replace(/.*(?:applewebkit|androidwebkit)\/(\d+).*/, "$1") < 536;

window["UI"] = window["UI"] || {};
(function(ui){
	function mark(){
		this.guid=(new Date()).getTime();
		var markdiv=$("<div class='markdiv' id='mark"+this.guid+"'></div>");
		if($("#main").length!=0){
			$("#main").append(markdiv);
		}
		else{
			$("body").append(markdiv);
		}
		markdiv.hide();
		
		this.$el=markdiv;
		var me=this;
		this.callback=function(){};
		markdiv.unbind("click");
		markdiv.click(function(){me.callback.call(me);});
        //markdiv[0].addEventListener('touchmove', function (e) { e.preventDefault(); }, false);
	}
	mark.prototype={
		show:function(){
			this.$el.show();	
		},
		hide:function(){
			this.$el.hide();	
		},
		setcallback:function(callback){
			this.callback=callback;	
		},
		callback:function(callback){
			this.callback.call(this);	
		}
	}
	ui.mark=mark;
})(UI);

(function(ui){
	ui.childUntil=function(expr,obj){
		if(obj.length==0) return null;
		var child=obj.children(expr);
		if(child.length==0){
			return arguments.callee(expr,obj.children());
		}else{
			return child;	
		}
	}
})(UI);

(function(ui) {
	function uialert (msg,s) {
		var self=this;
		self.mark=new ui.mark();
		self.mark.setcallback(function(){
			if(self.tid){
				clearTimeout(self.tid);
			}
			self.$el.hide();
			self.mark.hide();
		})
		var alertdiv=$("<div class='alertdiv'><div class='msg'></div></div>");
		$("body").append(alertdiv);	
		alertdiv.hide();	
		
		self.$el=alertdiv;
		
	}
	uialert.prototype={
		show:function(msg,s){
			var self=this;
			self.mark.show();
			self.$el.find(".msg").html(msg);
			self.$el.show();
			if(s){
				if(self.tid){
					clearTimeout(self.tid);
				}	
				self.tid=setTimeout(function(){
					self.mark.callback();
				},s)	
			}
		}

	}
	var uialertobj=new uialert();
	ui.alert=function(msg,s){
		uialertobj.show(msg,s);
	}
})(UI);

(function(ui) {
	function uiloading () {
		if(ui.loadingobj){
			return ui.loadingobj;	
		}
		var self=this;
		self.mark=new ui.mark();
		self.mark.setcallback(function(){});
		var loadingdiv=$(".uiload");
		if(loadingdiv.length<=0){
			loadingdiv=$("<div class='uiload'><div class='uiloadicon'></div></div>");
			$("body").append(loadingdiv);	
			loadingdiv.hide();	
		}else{
			loadingdiv.hide();
		}
		this.$el=loadingdiv;
		ui.loadingobj=this;
	}
	uiloading.prototype={
		show:function(){
			this.$el.show();
			this.mark.show();
		},
		hide:function(){
			this.$el.hide();
			this.mark.hide();
		}
	}
	ui.loading=uiloading;
})(UI);

(function(ui) {
	function uiLoadScript(url, options, cb) {
        var existNodes = document.getElementsByTagName("script");
        var nodeLoaded = false;
        for (var i = 0,enl = existNodes.length; i<enl; i++) {        
            if(url==existNodes[i].getAttribute("src")){                
                if("true"==existNodes[i].getAttribute("data-loaded")){
                    nodeLoaded = true;
                     cb && cb();                    
                    break;           
                }
            }           
        }
       if(nodeLoaded==true){return false;}    
        var node = document.createElement("script");
        var def = {
            type: "text/javascript"
        }
        options = options || {}
        for (var i in options) {
            def[i] = options[i];
        }
        node.src = url;
        for (var i in def) {
            node.setAttribute(i, def[i]);
        }
        node.addEventListener("load", function () {
            node.setAttribute("data-loaded","true");
            cb && cb();            
        }, false)
        document.getElementsByTagName("head")[0].appendChild(node);
        return node;
    }
	ui.loadScript=uiLoadScript;   
})(UI);


(function(ui) {
	function uiLoadCSS(url, options, cb) {
        var existNodes = document.getElementsByTagName("link");
        var nodeLoaded = false;  

        for (var i = 0, enl = existNodes.length; i < enl; i++) {
            if (url == existNodes[i].getAttribute("href")) {
                if ("true" == existNodes[i].getAttribute("data-loaded")) {
                    nodeLoaded = true;
                    cb && cb();                   
                    break;
                }
            }
        }
        if (nodeLoaded == true) { return false; }
        var node = document.createElement("link");
       
        var def = {
            type: "text/css",
            rel:"stylesheet"
        }
        options = options || {}
        for (var i in options) {
            def[i] = options[i];
        }
        node.href = url;
        for (var i in def) {
            node.setAttribute(i, def[i]);
        }
        addOnload(node, cb); 
        document.getElementsByTagName("head")[0].appendChild(node);
        return node;
    }

    function addOnload(node, callback) {
        var supportOnload = "onload" in node;

        // for Old WebKit and Old Firefox
        if (isOldWebKit || !supportOnload) {  
            setTimeout(function() {
                pollCss(node, callback);
            }, 1); // Begin after node insertion
            return;
        }
        
        if (supportOnload) {         
            node.onload = onload;
            node.onerror = function() {                
                onload();
            }
        }
        else {
            node.onreadystatechange = function() {
                if (/loaded|complete/.test(node.readyState)) {
                    onload();
                }
            }
        }

        function onload() {           
            // Ensure only run once and handle memory leak in IE
            node.onload = node.onerror = node.onreadystatechange = null;
            node.setAttribute("data-loaded", "true");            
            callback();
        }
    }
        
    function pollCss(node, callback) {
        var sheet = node.sheet;
        var isLoaded;

        // for WebKit < 536
        if (isOldWebKit) {
            if (sheet) {
                isLoaded = true;
            }
        }
        // for Firefox < 9.0
        else if (sheet) {
            try {
                if (sheet.cssRules) {
                isLoaded = true;
                }
            } catch (ex) {
                // The value of `ex.name` is changed from "NS_ERROR_DOM_SECURITY_ERR"
                // to "SecurityError" since Firefox 13.0. But Firefox is less than 9.0
                // in here, So it is ok to just rely on "NS_ERROR_DOM_SECURITY_ERR"
                if (ex.name === "NS_ERROR_DOM_SECURITY_ERR") {
                isLoaded = true;
                }
            }
        }

        setTimeout(function() {
            if (isLoaded) {
                // Place callback here to give time for style rendering
                node.setAttribute("data-loaded", "true");
                callback();
            }
            else {
                pollCss(node, callback);
            }
        }, 20);
    }

    ui.loadCSS = uiLoadCSS;
})(UI);

//tab 切换优惠券
(function(ui){
	ui.tabPanel=function(obj){
		obj.each(function () {
			var $this = $(this),
			tabc = UI.childUntil(".tabc", $this.parent());
			//console.log(tabc);
			//if(tabc.length==0) return;
			$this.children("a:not([rel*='link'])").add($this.find(".tabitem")).each(function (n) {
				$(this).attr("rel", n);
			}).on(_tapClick,function (e) {
				if ($this.attr("enableLink") != "true") {
					if ($this.attr("trigger") != "click") { return false; }
				}
				$(this).addClass("current").siblings().removeClass("current");
				tabc.hide().eq(parseInt($(this).attr("rel"))).show();
				if ($(this).attr("command")) {
					eval($(this).attr('command') + "(this)");
				}
			});
		});
	}
})(UI);

$(function () {
    //处理移动触屏设备点击延迟问题
    FastClick.attach(document.body);

    //标识AndroidWebkit
    if ($.os.android) {     
        $("body").addClass("android");
    }
	
	//初始化tab
	UI.tabPanel($(".tab"));
   

    //用于隐藏浏览器地址拦
    addEventListener("load", function () { setTimeout(hideURLbar, 0); }, false);
    function hideURLbar() {
        window.scrollTo(0, 1);
        $('#main').css("height", window.innerHeight + 100);  //强制让内容超过         
        $("#main").css("height", "auto");
    }

    //Header中返回上一页按钮
    $(".returnico").on(_tapClick, function () {
        window.history.back();
        //navigator.app.backHistory();
        //window.location.href="index.html"
    })

})