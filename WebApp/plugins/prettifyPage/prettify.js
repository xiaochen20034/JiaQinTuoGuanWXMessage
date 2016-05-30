
/**

name: ExpressionSize
description: 
html元素添加 expressionWidth 属性，设置动态宽度； expressionHeight 属性，设置动态高度。
可以使用的变量，parentWidth、documentWidth、parentHeight、documentHeight，以及方法ele(id)，获取对象的宽高
authors:郑延杰
date:2015-01-30
example：

**/

var ExpressionSize=new Class({
	Implements:[Events, Options],
	options:{
		root:null,
		parentHeight:".parentHeight",//设置高度与父级元素同高
		documentHeight:".documentHeight"//设置高度与body元素同高
		
	},
	initialize:function(options){
		this.setOptions(options);
		if(!this.options.root){
			this.doc = $(document.body);	
		}else if(typeOf(this.options.root)=='string'){
			this.doc = $(document.body).getElement(this.options.root);		
		}else{
			this.doc = $(this.options.root);			
		}
		this.resize();
		window.addEvent('resize',function(){
			this.resize();
		}.bind(this));

	},
	resize:function(){
		this.documentHeight();
		this.parentHeight();	
		
		this.expressionHeight();
		this.expressionWidth();

		this.expressionMinHeight();
		this.expressionMinWidth();
	},
	//获取元素的 size ：x,y	
	element:function(e){
		var el= $(e);
		return el.getSize();				
	},
	//html元素添加 expressionWidth 属性，设置动态宽度。可以使用的变量，parentWidth、documentWidth、parentHeight、documentHeight，以及方法ele(id)，获取对象的宽高
	expressionWidth:function(){
		this.doc.getElements('*[expressionWidth]').each(function(express){
			var ele=this.element;
			var e= express.get('expressionWidth');
			var parentSize=express.getParent().getSize();
			var parentWidth=parentSize.x;
			var parentHeight=parentSize.y;
			
			var documentSize=$(document.body).getSize();
			var documentWidth=documentSize.x;
			var documentHeight=documentSize.y;			
			var width=eval(e).toInt();
			if(!isNaN(width) && width>=0)
			{
				express.setStyle('width',width);
			}
			else{
				express.setStyle('width','auto');
			}
			
		}.bind(this));
	},
	//html元素添加 expressionWidth 属性，设置动态宽度。可以使用的变量，parentWidth、documentWidth、parentHeight、documentHeight，以及方法ele(id)，获取对象的宽高
	expressionMinWidth:function(){
		this.doc.getElements('*[expressionMinWidth]').each(function(express){
			var ele=this.element;
			var e= express.get('expressionWidth');
			var parentSize=express.getParent().getSize();
			var parentWidth=parentSize.x;
			var parentHeight=parentSize.y;
			
			var documentSize=$(document.body).getSize();
			var documentWidth=documentSize.x;
			var documentHeight=documentSize.y;			
			var minWidth=eval(e).toInt();
			if(!isNaN(minWidth) && minWidth>=0){
				express.setStyle('min-width',minWidth);
			}else{
				express.setStyle('min-width','auto');
			}
			
		}.bind(this));
	},
	
	//html元素添加 expressionHeight 属性，设置动态高度。可以使用的变量，parentWidth、documentWidth、parentHeight、documentHeight，以及方法ele(id)，获取对象的宽高
	expressionHeight:function(){
		this.doc.getElements('*[expressionHeight]').each(function(express){

			var ele=this.element;
			var e= express.get('expressionHeight');
			var parentSize=express.getParent().getSize();
			var parentWidth=parentSize.x;
			var parentHeight=parentSize.y;
			
			var documentSize=$(document.body).getSize();
			var documentWidth=documentSize.x;
			var documentHeight=documentSize.y;			

			var height=eval(e).toInt();
			
			if(!isNaN(height) && height>=0){
				express.setStyle('height',height);
			}else{
				express.setStyle('height','auto');
			}
		}.bind(this));

	},
		//html元素添加 expressionHeight 属性，设置动态高度。可以使用的变量，parentWidth、documentWidth、parentHeight、documentHeight，以及方法ele(id)，获取对象的宽高
	expressionMinHeight:function(){
		this.doc.getElements('*[expressionMinHeight]').each(function(express){
			var ele=this.element;
			var e= express.get('expressionHeight');
			var parentSize=express.getParent().getSize();
			var parentWidth=parentSize.x;
			var parentHeight=parentSize.y;
			
			var documentSize=$(document.body).getSize();
			var documentWidth=documentSize.x;
			var documentHeight=documentSize.y;			
			
			
			var minHeight=eval(e).toInt();
			if(!isNaN(minHeight) && minHeight>=0){
				express.setStyle('min-height',minHeight);
			}else{
				express.setStyle('min-height','auto');
			}
			
		}.bind(this));

	},
	parentHeight:function(){
		this.doc.getElements(this.options.parentHeight).each(function(item){
			item.setStyle("height", item.getParent().getSize().y);
		});
	},
	documentHeight:function(){
		this.doc.getElements(this.options.documentHeight).each(function(item){
			item.setStyle("height", $(document.body).getSize().y);
		});
	}

});
window.addEvent('domready',function(){ var size=new ExpressionSize();});