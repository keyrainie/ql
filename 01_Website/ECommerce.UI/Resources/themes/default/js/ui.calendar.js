 var calendar=(function(){
		var defaultConfig = {
			SelectDay:		null,//选择日期
            onSelectDay:	null,//在选择日期触发
            onToday:		null,//在当天日期触发
            onFinish:		null,//日历画完后触发					
			dayEvent:		null,
			dayTemplate:    '<span class="yl">{d}</span>',
			noHover:		false		
		};
		function ClassObj(container,config){
			var self = this;
			if(!(self instanceof ClassObj)) {
				return new ClassObj(container,$.extend(defaultConfig,config));
			}
			this.Container = $(container);//容器(table结构)
            this.Days = [];//日期对象列表
            this.Year = config.Year || new Date().getFullYear();
            this.Month = config.Month || new Date().getMonth() + 1;
            this.SelectDay = config.SelectDay ? new Date(config.SelectDay) : null;
            this.onSelectDay = !!config.onSelectDay && config.onSelectDay;
            this.onToday = !!config.onToday && config.onToday;
            this.onFinish = !!config.onFinish && config.onFinish;
			this.dayTemplate=!!config.dayTemplate && config.dayTemplate;
			this.noHover = config.noHover;
            this.Draw();
		};
		$.extend(ClassObj.prototype, {
			Binds:function(thisp,func){
				var args=Array.prototype.slice.call(arguments,2);
				return function(){
					return func.apply(thisp,args.concat(Array.prototype.slice.call(arguments)));
				}
			},
			NowMonth: function() {
                this.PreDraw(new Date());
              },
              //上一月
              PreMonth: function() {
                this.PreDraw(new Date(this.Year, this.Month - 2, 1));
              },
              //下一月
              NextMonth: function() {
                this.PreDraw(new Date(this.Year, this.Month, 1));
              },
              //上一年
              PreYear: function() {				
                this.PreDraw(new Date(this.Year - 1, this.Month - 1, 1));
              },
              //下一年
              NextYear: function() {
				this.PreDraw(new Date(this.Year + 1, this.Month - 1, 1));
				
              },
              //根据日期画日历
              PreDraw: function(date) {
				var year = date.getFullYear();
                if(year<=2049 && year>=1900){
					//再设置属性
					this.Year = year; this.Month = date.getMonth() + 1;
	                //重新画日历
	                this.Draw();
				}
              },
              //画日历
              Draw: function() {
				var me=this;
                //用来保存日期列表
                var arr = [];
                //用当月第一天在一周中的日期值作为当月离第一天的天数
				var preMonthDate=new Date(this.Year, this.Month - 1, 0).getDate();
				
				
				var prearr=[],
					nextarr=[];
                for(var i = 1, firstDay = new Date(this.Year, this.Month - 1, 1).getDay(); i <= firstDay; i++){ 
					arr.push(-1); 
					prearr.push(preMonthDate-(firstDay-i));
				}
                //用当月最后一天在一个月中的日期值作为当月的天数
                for(var i = 1, monthDay = new Date(this.Year, this.Month, 0).getDate(); i <= monthDay; i++){ 
					arr.push(i); 
				}
				for(var i = 1, len=42-arr.length;i<=len;i++){
					arr.push(0);
					nextarr.push(i);
				}				
                //清空原来的日期对象列表
                this.Days = [];
				var thism=new Date(this.Year, this.Month - 1, 1);
				var prem=new Date(this.Year, this.Month - 1, 0);
				var nextm=new Date(this.Year, this.Month, 1);
				
						
                //插入日期				
                var frag = document.createDocumentFragment();				
                while(arr.length){
                    //每个星期插入一个tr
                    var row = document.createElement("tr");
                    //每个星期有7天
                    for(var i = 1; i <= 7; i++){
                        var cell = document.createElement("td"); cell.innerHTML = "&nbsp;";
						var $cell=$(cell);
                        if(arr.length){
                            var d = arr.shift();
                            if(d>0){	
								var cell_str=me.dayTemplate;								
								cell_str=cell_str.replace(/\{d\}/ig,d);
								
								//$cell.html("<a href='###'><span class='yl'>"+d+"</span><span class='nl'>"+lDay+"</span></a>");
								$cell.html(cell_str);				
								
                                this.Days[d] = cell;
                                var on = new Date(this.Year, this.Month - 1, d);
                                //判断是否今日
                                this.IsSame(on, new Date()) && this.onToday(cell);
                                //判断是否选择日期
                                this.SelectDay && this.IsSame(on, this.SelectDay) && this.onSelectDay(cell);
                            }else if(d<0){
								var dp=	prearr.shift();							
								
								var cell_str=me.dayTemplate
								cell_str=cell_str.replace(/\{d\}/ig,dp);
								
								cell.innerHTML = cell_str;
								$(cell).addClass("mdatepre").unbind().click(function(){me.PreMonth();});							
							}else{
								var dn=	nextarr.shift();
								
								var cell_str=me.dayTemplate
								cell_str=cell_str.replace(/\{d\}/ig,dn);
								
								cell.innerHTML = cell_str;
								$(cell).addClass("mdatenext").unbind().click(function(){me.NextMonth();});								
							}
							if(!this.noHover){
								$cell.find("a").hover(function(){
									$("span:eq(0)",this).stop().animate({"margin-top":"-16px"},300);
								},function(){
									$("span:eq(0)",this).stop().animate({"margin-top":"0"},300);
								})
							}
                        }
                        row.appendChild(cell);
                    }
                    frag.appendChild(row);
                }
                //先清空内容再插入(ie的table不能用innerHTML)
                while(this.Container[0].hasChildNodes()){ this.Container[0].removeChild(this.Container[0].firstChild); }
                this.Container[0].appendChild(frag);
                //附加程序
                this.onFinish();
              },
              //判断是否同一日
              IsSame: function(d1, d2) {
                return (d1.getFullYear() == d2.getFullYear() && d1.getMonth() == d2.getMonth() && d1.getDate() == d2.getDate());
              } 
		});
		
		
		
		
		return ClassObj;
})();