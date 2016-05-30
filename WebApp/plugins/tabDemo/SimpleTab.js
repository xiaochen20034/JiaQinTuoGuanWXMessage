
/*play：auto、click、hover、script         播放方式
//interal：n (int)                 自动切换间隔时间
选项卡必须的属性：name=[在选项卡组里面是唯一的],target=[选项卡对应的容器对象]
事件：
onmove：选项卡向下一个切换时的事件
onmoved:选项卡移动完成

如果是通过选择器添加的选项卡，元素必须有name及target属性。name属性作为选项卡的标签名字，必须唯一，target作为指向内容，指向内容的容器的id即为target属性值

//current:当前选项卡，只读
方法：
next()：切换到下一个
previous()：上一个
move(string/int)：切换至指定名称/指定索引的选项卡
add(string,element/string)：添加选项卡，arg1：label，arg2：compontent
remove(string/int)：移除指定选项卡	
*/
var SimpleTab = new Class({
    Implements: [Options],
    options: {
        play: "click",
        interal: 3,
		slideContainerShow:".slideContainerShow",//显示部分容器
		selectorContainer:null,//选择器的容器。
        selector: "#tab .tabItem",
        onMove: null,
        onMoved: null,
        actived: "activeTab",
        moveSelf: false,
		itemWidth:300,//每个元素的宽度均相等时，填写元素宽度，如果元素的宽度不相等，填写0
		itemHeight:300,//每个元素的高度均相等时，填写元素高度，如果元素的高度不相等，填写0
		moveStyle:"display",//切换方式。默认隐藏显示.display、horizontalLinear、verticalLiner
		duration:800,//切换过程持续的时长
		transition:"linear"//切换的方式

    },
    initialize: function (options) {
        this.setOptions(options);
        //this.setOptions(options);
        this.tabItems = new Hash();
        this.tabLabels = new Hash();
        this.current = null;
        this.currentIndex = 0;
        this.playStyle = this.options.play;
        this.add(this.selector);
		
    },
    /*
    在执行play之前 加载选项卡，用这个方法
    arg1：选择器
    arg2：此参数有值时，将第一个参数作为名字，该参数作为选项卡目标
    */
    add: function (selector, tabTarget) {
        if (tabTarget) {
            this.tabItems.set(selector, tabTarget);
            return;
        }
        var tabs = $$(this.options.selector);
        var k;
        tabs.each(function (t) {
            k = this.CssFirstElement(t, t.getProperty("target"));
            var name=t.getProperty('name');
            if (!name) {
                name = String.uniqueID();
                t.set('name',name);
            }
            this.tabItems.set(name, k);
            this.tabLabels.set(name, t);
        } .bind(this));
    },

    /*
    在 执行play之后，执行这个方法。多次添加具有同一名字的对象会无法添加
    arg1：tab对象
    arg2：tab目标对象
    */
    addElement: function (selector, tabTarget) {

        if (!tabTarget) {
            tabTarget =  this.CssFirstElement(selector,selector.getProperty("target"));
        }
        if (this.tabLabels.get(selector.getProperty("name"))) {
            selector.dispose();
            tabTarget.dispose();
            //			alert("您已经添加过该选项卡，请移除");
            return;
        }
        this.tabLabels.set(selector.getProperty("name"), selector);
        this.tabItems.set(selector.getProperty("name"), tabTarget);
        
        switch (this.options.play) {
            case "click":
                selector.addEvent("click", this.move.bind(this, selector.getProperty("name")));
                break;
            case "hover":
                selector.addEvent("mouseover", this.move.bind(this, selector.getProperty("name")));
                break;
        }
    },
    /**
    移动到指定的选项卡
    移动之前会执行移动事件，只有返回false时，会终止执行
    arg：int 索引 或者 string label 的name属性
    */
    move: function (index) {
        var currentValue, currentKey,currentIndex;
				
        if (typeOf(index) == 'string') {
            currentKey = index;
            currentValue = this.tabItems.get(index);
            currentIndex = this.tabLabels.getKeys().indexOf(index);
        } 
        else {
            currentKey = this.tabLabels.getKeys()[index];
            currentValue = this.tabItems.get(currentKey);
            currentIndex = index;
        }
		
        if (this.current == currentKey && !this.options.moveSelf) {
            //            alert("同一个选项卡");
            return;
        }
        if (!currentKey || !currentValue) {
            if (this.tabLabels.getLength() > 0) this.move(0);
            else alert("无法移动到指定的选项卡，指定索引键值无效：" + index);
            return;
        }
		if (!this.current) {
			this.current = currentKey;
			this.currentIndex=currentIndex;
		
		}
        //执行 移动切换事件
        if (this.options.onMove) {
            
            var r = this.options.onMove.attempt([this.tabLabels.get(currentKey), this.tabItems.get(this.currentKey)], this);
            if (r == 'false') return;
        }

        this.previous=this.current;
        this.current = currentKey;
		
        this.previousIndex=this.currentIndex;
        this.currentIndex =currentIndex;
        
		switch(this.options.moveStyle){
			case "display":this.moveDisplay();break;
			case "horizontalLinear":this.moveHorizontalLinear();break;
			case "verticalLiner":this.moveVerticalLiner();break;
		}
        //执行 移动切换事件
        if (this.options.onMoved) {            
            var r = this.options.onMoved.attempt([this.tabLabels.get(this.current), this.tabItems.get(this.current)], this);
        }


    }, 
	//初始化默认的元素
	initDisplay:function(){
		this.tabItems.each(function(v,k){
			v.setStyle("display", "none");
		});

	},
	//默认的切换方式，通过切换
	moveDisplay:function(){
		if(this.previous){
			this.tabLabels.get(this.previous).removeClass(this.options.actived);	
			this.tabItems.get(this.previous).setStyle("display", "none");
		}				
		this.tabLabels.get(this.current).addClass(this.options.actived);	
        this.tabItems.get(this.current).setStyle("display", "block");
	},
	//初始化默认的水平元素
	initHorizontalLinear:function(){		
		
		var totalWidth=0;
		this.container= $(this.options.selectorContainer);
		if(!this.container){
			alert("水平滚动需要selectorContainer容器");
			return;	
		}
		if(this.options.itemWidth==0){
			this.tabItems.each(function(v,k){
				totalWidth+=v.getSize().x;
			});			
		}else{
			totalWidth=this.tabItems.getLength()*this.options.itemWidth;	
		}
		this.container.setStyle("width",totalWidth);
		this.container.setStyle("margin-left",0);
		this.totalWidth=totalWidth;
		
		  
		this.container.set("tween",{
			duration: this.options.duration,
			transition: this.options.transition,
			link: 'cancel'
		});
	},	
	//横向水平滚动
	moveHorizontalLinear:function(){		
		//超出容器的部分宽度
		var marginLeftOrg=this.container.getStyle("margin-left").toInt();
		var marginLeftDes=0;
		var outWidth=Math.abs(marginLeftOrg);
		
  	    
		if(this.currentIndex==0){
			marginLeftDes=0;
		}else if(this.currentIndex>this.previousIndex){
			marginLeftDes=0-this.options.itemWidth*(this.currentIndex-this.previousIndex)-outWidth;
		}else if(this.currentIndex<this.previousIndex){
			marginLeftDes=this.options.itemWidth*(this.previousIndex-this.currentIndex)+marginLeftOrg;
		}else if(outWidth+this.options.itemWidth==this.totalWidth){
        	//this.container.setStyle("margin-left",0);
			marginLeftDes=0;
		}

		this.container.tween("margin-left",marginLeftOrg,marginLeftDes);
		if(this.previous){
			this.tabLabels.get(this.previous).removeClass(this.options.actived);	
		}				
		this.tabLabels.get(this.current).addClass(this.options.actived);
	},
	//垂直切换初始化
	initVerticalLiner:function(){
		
		var totalHeight=0;
		this.container= $(this.options.selectorContainer);
		if(!this.container){
			alert("水平滚动需要selectorContainer容器");
			return;	
		}
		if(this.options.itemHeight==0){
			this.tabItems.each(function(v,k){
				totalWidth+=v.getSize().y;
			});			
		}else{
			totalHeight=this.tabItems.getLength()*this.options.itemHeight;	
		}
		this.container.setStyle("height",totalHeight);
		this.container.setStyle("margin-top",0);
		this.totalHeight=totalHeight;
		
		  
		this.container.set("tween",{
			duration: this.options.duration,
			transition: this.options.transition,
			link: 'cancel'
		});
	},
	//垂直切换移动	
	moveVerticalLiner:function(){
		//超出容器的部分宽度
		var marginTopOrg=this.container.getStyle("margin-top").toInt();
		var marginTopDes=0;
		var outHeight=Math.abs(marginTopOrg);
		
  	    
		if(this.currentIndex==0){
			marginTopDes=0;
		}else if(this.currentIndex>this.previousIndex){
			marginTopDes=0-this.options.itemHeight*(this.currentIndex-this.previousIndex)-outHeight;
		}else if(this.currentIndex<this.previousIndex){
			marginTopDes=this.options.itemHeight*(this.previousIndex-this.currentIndex)+marginTopOrg;
		}else if(outWidth+this.options.itemHeight==this.totalHeight){
        	//this.container.setStyle("margin-left",0);
			marginTopDes=0;
		}

		this.container.tween("margin-top",marginTopOrg,marginTopDes);
		if(this.previous){
			this.tabLabels.get(this.previous).removeClass(this.options.actived);	
		}				
		this.tabLabels.get(this.current).addClass(this.options.actived);
	},
    getTab: function (key) {
        if (key)
            return this.tabLabels.get(key);
        return this.tabLabels.get(this.current);
    },
    getItem: function (key) {
        if (key)
            return this.tabItems.get(key);
        return this.tabItems.get(this.current);
    },
    /**
    移动当下一个
    */
    next: function (arg) {
		var timer;
	
		if(this.timer && arg){
			timer=this.timerFun;
			clearInterval(this.timer);
				
		}
		var len= this.tabItems.getLength();
		var tempIndex=this.currentIndex + 1;
		if(tempIndex==len){
			tempIndex=0;	
		}
        this.move(tempIndex);
		
		if(timer && arg){
			this.timer=timer();
		}
    },
    /*
    移动到上一个 previous
    */
    pre: function (arg) {
		
		var timer;
	
		if(this.timer && arg){
			timer=this.timerFun;
			clearInterval(this.timer);
				
		}
		
		
		var len= this.tabItems.getLength();
		var tempIndex=this.currentIndex - 1;
		if(tempIndex<0){
			tempIndex=len-1;	
		}
        this.move(tempIndex);
		
		if(timer && arg){
			this.timer=timer();
		}
		
    },
    /**
    删除指定的选项卡，通过索引或者名字
    */
    remove: function (index,dispose) {

        if (typeOf(index) == 'number') {
            index = this.tabLabels.getKeys()[index];
        }
		
        $(this.tabLabels.get(index)).removeEvents("click");
      
        //this.tabItems.get(index).dispose();
        var eles = [$(this.tabLabels.get(index)), $(this.tabItems.get(index))];
        

        
        this.previous();
        this.tabLabels.erase(index);
        this.tabItems.erase(index);
        if (dispose) {
            eles[0].dispose();
            eles[1].dispose();
        }
        return eles;
    },
    /*
    移除所有选项卡
    */
    clear: function () {
        this.tabLabels.getKeys().each(function (index) {
            this.remove(index);
        } .bind(this));
        this.current = null;
        this.currentIndex = 0;

    },
	/*
	**
	**获取元素的相对元素
	*****/
	CssFirstElement:function(ele, css) {
		ele = $(ele);
		if (!ele) {
			var event = event ? arguments[0] : window.event;
			ele = event.srcElement ? event.srcElement : event.target;
			ele = $(ele);
		}
		var targetInpage;
		if (typeOf(css) == 'string' && css.startWith("css:#")) {
			targetInpage = $$(css.substring(4))[0];
		}else if (typeOf(css) == 'string' && css.startWith("css:")) {
			
			targetInpage = ele.getParent(css.substring(4));

			//targetInpage = $$(ele.getParent(css.substring(4)));
			//if (targetInpage.length > 0) {
			//    targetInpage = targetInpage[0];
			//}
		}else{
			return $(css);
		}
		return targetInpage;
	},


    /*
    添加事件，并且选项卡，开始生效
    */
    play: function () {
		
        var mm = this.next.bind(this);
        var moveItem = null;
		
		switch(this.options.moveStyle){
			case "display":this.initDisplay();break;
			case "horizontalLinear":this.initHorizontalLinear();break;
			case "verticalLiner":this.initVerticalLiner();break;
		}
		
		  
        switch (this.options.play) {
            case "click":
                
                this.tabLabels.getValues().each(function (item) {                   
                    item.addEvent("click", this.move.bind(this, item.getProperty("name")));
                    if(!moveItem)
                        moveItem = item;
                } .bind(this));
              
                break;
            case "hover":

                this.tabLabels.getValues().each(function (item) {

                    item.addEvent("mouseover", this.move.bind(this, item.getProperty("name")));
                    if (!moveItem)
                        moveItem = item;
                } .bind(this));
               
                break;
            case "auto":
                //自动切换有点复杂

                this.tabLabels.getValues().each(function (item) {
                    item.addEvent("click", this.move.bind(this, item.getProperty("name")));
                    item.addEvent("mouseover", function () { clearInterval(this.timer); this.move(item.getProperty("name")) } .bind(this));
                    item.addEvent("mouseout", function () { this.timer = this.next.periodical(this.options.interal * 1000, this); } .bind(this));
                } .bind(this));
                this.tabItems.getValues().each(function (item) {
                    item.addEvent("mouseover", function () { clearInterval(this.timer); } .bind(this));
                    item.addEvent("mouseout", function () { this.timer = this.next.periodical(this.options.interal * 1000, this); }.bind(this));
                    if (!moveItem)
                        moveItem = item;
                } .bind(this));
				
				this.timerFun=function(){
					return this.next.periodical(this.options.interal * 1000, this);	
				}.bind(this);
                this.timer = this.timerFun();
                break;                
        }
        var activeTabDefault = $$(this.options.selector+ "." + this.options.actived);
        var activeTabItem;
        if (activeTabDefault && activeTabDefault.length > 0) {
            activeTabItem = activeTabDefault[0];
            
            //this.move(activeTabDefault[0].get("name"))
        } else {
            activeTabItem= moveItem;
        }
        if (!activeTabItem) {
            activeTabItem = this.tabLabels.getValues()[0];
        }
        switch (this.options.play) {
            case "click": activeTabItem.click(); break;
            case "hover": this.move(activeTabItem.get('name'));break;
            case "auto":this.move(activeTabItem.get('name')); break;
			case "script": this.move(activeTabItem.get('name'));break;
        }
		
		
       
    }
});
