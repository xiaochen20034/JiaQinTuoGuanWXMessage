
/**
dropmenu 意指下拉菜单，可以实现 顶级菜单横向、竖向， 子集菜单横向。采用标准的 li 列表。
实例化后，凡是在ul上应用了 .dropmenu 的li 列表均被视为菜单处理，进行初始化，并且鼠标悬停li元素时，展开子集一层菜单

name: DropMenu
description: 下拉菜单
authors:郑延杰
date:2015-01-30
example：
	new Placeholder();
**/

var DropButtonAdvance=new Class({
	Implements: [Options],
    options: {
        //handle和target 在同一个父容器中，不要一个容器放置多个handler
		dropRoot:".dropButton",
        dropToggle: ".dropButton-toggle",//句柄要触发的对象		
        dropingClass:"dropButton-Menu",
        trigger: "click",//click, 暂时不能用hover  
        callback:null
    },
    initialize: function (options) {
        this.setOptions(options);
		if(typeOf(this.options.dropRoot)=='string'){
			$$(this.options.dropRoot).each(function(r){
				this.options.dropRoot=r;
				new DropButtonAdvance(this.options);
			}.bind(this));
		}else if(typeOf(this.options.dropRoot)=='element'){
			this.dropRoot =this.options.dropRoot;
			this.createDropButton(this.dropRoot);
		}
		
	},
	createDropButton:function(button){
	
			this.dropToggle =button.getElement(this.options.dropToggle);
		
		this.dropMenu =button.getElement(this.options.dropMenu);
	
		if(this.options.trigger=='hover'){
			this.hoverEvent();
		}else if(this.options.trigger=='click'){
			this.clickEvent();
		}

	},
	hoverEvent:function(){
		this.dropToggle.addEvent('mouseover',function(){
		    this.dropRoot.addClass(this.options.dropingClass);
		   
		}.bind(this));
		this.dropRoot.addEvent('mouseleave',function(){			
		    this.dropRoot.removeClass(this.options.dropingClass);
		  
		}.bind(this));
	
	},
	clickEvent:function(){
		this.dropToggle.addEvent('click',function(){
			this.dropRoot.toggleClass(this.options.dropingClass);
			if (this.options.callback) {
			    this.options.callback(this.dropRoot,this.dropRoot.hasClass(this.options.dropingClass));
			}
		}.bind(this));

		
	}
});