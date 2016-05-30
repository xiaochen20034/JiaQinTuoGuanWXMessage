//要延迟加载的图片加class="lazyload"
//真实地址放在data-lazy-src=""属性中
	var LazyLoad=new Class({
		Implements: [Options],
		options: {
			selector: '#tab .tabItem'
		},
		initialize: function (options) {
			this.setOptions(options);
		},
		//获得页面的可视宽度和高度
		winSize:function(){
			var a='client';
			var e=document.documentElement||document.body;
			return {width:e[a+'Width'],height:e[a+'Height']};
			},
		//加载图片
		loading:function(Imgs,scrollsize,clientheigh){
			
			Array.each(Imgs,function(item,index){
						
						if(item.getPosition().y<(scrollsize.y+clientheigh))
						{
							var real_src=item.getProperty('data-lazy-src');
							//item.setProperty('src',real_src);
							this.addImg(real_src,item);
							}			
						}.bind(this));
			},
		
		//添加图片
		addImg:function(Issrc,ImgEl){
			try
			{
				var Img = new Image();
				Img.onload = function ()
				{
					ImgEl.setProperty('src',Issrc);
				}
				Img.src = Issrc;
			}
			catch(e)
			{
				alert('addImg:'+e.message);	
			}

		},
		init:function(lazyloadImgs){
			var lazyloadImgs=lazyloadImgs;
			var scroll_size=$(document.body).getScroll();
			var clientheigh=this.winSize().height;
			this.loading(lazyloadImgs,scroll_size,clientheigh);
		},
		
		//滚动条滚动事件
		scrollFun:function(myImgs){
			
			window.addEvent('scroll',function(){
				var lazyImgs=myImgs.filter(function(item,index){
					return item.getProperty('data-lazy-src')!=item.getProperty('src');
				});
				
				if(lazyImgs.length==0)
				{
					return;
					}
				else
				{
					var scroll_size=$(document.body).getScroll();
					var clienty=document.documentElement||document.body;
					var clientheigh=clienty['client'+'Height'];
					Array.each(lazyImgs,function(item,index){
						if(item.getPosition().y<(scroll_size.y+clientheigh))
						{
							var real_src=item.getProperty('data-lazy-src');
							this.addImg(real_src,item);
							}			
						}.bind(this));
				}
			}.bind(this));
			},
		//窗体改变大小事件
		resizelFun:function(myImgs){
			
			window.addEvent('resize',function(){
				
				var lazyImgs=myImgs.filter(function(item,index){
					return item.getProperty('data-lazy-src')!=item.getProperty('src');
				});
				if(lazyImgs.length==0)
				{
					return;
					}
				else
				{
					var scroll_size=$(document.body).getScroll();
					var clienty=document.documentElement||document.body;
					var clientheigh=clienty['client'+'Height'];
					Array.each(lazyImgs,function(item,index){
						if(item.getPosition().y<(scroll_size.y+clientheigh))
						{
							var real_src=item.getProperty('data-lazy-src');
							this.addImg(real_src,item);
							}			
						}.bind(this));
				}
			}.bind(this));
			},
		play:function(){
			var myImgs=$$(this.options.selector);
			this.init(myImgs);
			this.scrollFun(myImgs);
			this.resizelFun(myImgs);
			}
		})