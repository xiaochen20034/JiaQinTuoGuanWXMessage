
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

var DropButton=new Class({
	Implements: [Options],
    options: {
        //handle和target 在同一个父容器中，不要一个容器放置多个handler
		dropRoot:".dropButton",
        dropToggle: ".dropButton-toggle",//句柄要触发的对象		
        dropMenu:".dropButton-Menu",
        trigger: "click"//click, 暂时不能用hover  
    },
    initialize: function (options) {
        this.setOptions(options);
		if(typeOf(this.options.dropRoot)=='string'){
			$$(this.options.dropRoot).each(function(r){
				this.options.dropRoot=r;
				new DropButton(this.options);
			}.bind(this));
		}else if(typeOf(this.options.dropRoot)=='element'){
			this.dropRoot =this.options.dropRoot;
			this.createDropButton(this.dropRoot);
		}
		
	},
	createDropButton:function(button){
		this.dropToggle =button.getElement(this.options.dropToggle);
		this.dropMenu =button.getElement(this.options.dropMenu);
	
		if(!this.dropToggle || !this.dropMenu)
		{
			//consol("button配置不正确");
			return;	
		}
		this.dropMenu.setStyle('display','none');
		if(this.options.trigger=='hover'){
			this.hoverEvent();
		}else if(this.options.trigger=='click'){
			this.clickEvent();
		}

	},
	hoverEvent:function(){
		this.dropToggle.addEvent('mouseover',function(){			
				this.dropMenu.setStyle('display','block');			
		}.bind(this));
		this.dropRoot.addEvent('mouseleave',function(){			
			this.dropMenu.setStyle('display','none');
		}.bind(this));
	
	},
	clickEvent:function(){
		var documentClick=function(){
			var e = arguments[0] || window.event;
            var src = e.srcElement || e.target;
			src=$(src);
			if(src==this.dropToggle || src==this.dropMenu || src.getParent(this.options.dropMenu)){
				return;	
			}
			this.dropMenu.setStyle('display','none');
		}.bind(this);
		this.dropToggle.addEvent('click',function(){
			this.dropMenu.setStyle('display','block');
			document.addEvent('click',documentClick);
		}.bind(this));

		
	}
});