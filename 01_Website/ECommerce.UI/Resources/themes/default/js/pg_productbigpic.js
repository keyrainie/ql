$(function () { 
	var thumbSlider = UI.Xslider(".smallPicList",{
		dir: "V",
		unitDisplayed: 5,
		stepOne: true,
		numtoMove:1,
		viewedSize:720,
		beforeStart:function(e){
			var me = $(e.next);
			var img=$(".bigPicShow img");
			img.hide();
			img.timer && clearTimeout(img.timer);
			img.timer = setTimeout(function(){
				img.attr("src",me.find("img").attr("bigsrc"));
			},200);
		}
	});
	var smallPicList = $(".smallPicList");
	var smallPicNum = smallPicList.find("li").length;
	$(".smallPicList li").click(function(){
		var me=$(this);
		var img=$(".bigPicShow img");
		img.hide();
		img.timer && clearTimeout(img.timer);
		img.timer = setTimeout(function(){
			img.attr("src",me.find("img").attr("bigsrc"));
		},200);		
		me.addClass("current").siblings().removeClass("current");		
		smallPicList.data("curindex",~~me.attr("index")+1);
		smallPicList.data("curobj",me);
		//smallPicList.data("runenv").start = 4;		
		if(me.attr("index") >=1){
			$(".agrayleft").removeClass("agrayleft");	
		}
		else{
			$(".aleft").addClass("agrayleft");	
		}
		if(smallPicNum - me.attr("index") >=2){
			$(".agrayright").removeClass("agrayright");	
		}
		else{
			$(".aright").addClass("agrayright");	
		}
	});	
	$(".bigPicShow img").load(function(){
		/*if(!-[1, ] && !window.XMLHttpRequest){
			//IE 6 fix
			this.style.display = "block";
			this.style.height = "auto";
			this.style.width = "auto";
			if(this.height-600>=0){
				this.style.height = "auto";
				this.style.width = (this.width-980>=0) ? "980px" : "auto";
			}
			else {
				this.style.height = "600px";
			}
			
		}*/		
		$(this).fadeIn(800);
	});
});