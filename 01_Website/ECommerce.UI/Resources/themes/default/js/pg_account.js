$(function(){
	UI.Xslider(".combolist");
	if($(".formsub").length>0){
		var op={
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
	  		}
		if($.fn.passwordStrength){
	  		op.usePlugin={
	  				passwordstrength:{
	  					minLen:8,
	  					maxLen:18,
	  					trigger:function(obj,error){					
	  						if(error){
	  							obj.parent("li").find(".Validform_checktip").show();
	  							//obj.parent("li").find(".passwordStrength").hide();
	  						}else{
	  							obj.parent("li").find(".Validform_checktip").hide();
	  							//obj.parent("li").find(".passwordStrength").show();	
	  						}
	  					}
	  				}
	  			}
		}
		$(".formsub").Validform(op);
	}
});