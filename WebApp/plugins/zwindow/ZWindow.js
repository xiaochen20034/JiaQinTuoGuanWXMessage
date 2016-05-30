var SimpleWindow = new Class({
    Implements: [Options],
    options: {
        disposeStyle:"destory",
		zWindowId:null,
		z_window:"z_window",
		z_header:"z_header",
		z_body:"z_body",
		z_ico:"z_ico",
		z_title:"z_title",
		z_close:"z_close",
		allowClose:true
    },
    initialize: function (options) {
		this.setOptions(options);        
        var len = $$(".z_window").length; //文档中win窗口的数量
        this.term = len;
        var winName =  "my_window_t" + len;
		if(this.options.z_window=="z_window"){
			this.options.z_window+=" z_window";
		}
        this.z_window = new Element("div", { "class": this.options.z_window, "id": winName, "term": len });
        this.z_header = new Element("div", { "class": this.options.z_header });		
        this.z_body = new Element("div", { "class": this.options.z_body });		
        this.z_footer = new Element("div", { "class": this.options.z_footer });
		
        this.z_ico = new Element("span", { "class": this.options.z_ico });
        this.z_title = new Element("span", { "class":this.options.z_title, text: "默认标题" });
        //this.z_min = new Element("span",{"class":"z_min"});
        //this.z_max=new Element("span",{"class":"z_max"});
        this.z_close = new Element("em", { "class": this.options.z_close});
        //this.z_max.inject(this.z_header);
        //this.z_min.inject(this.z_header);
        
		this.z_ico.inject(this.z_header);
        this.z_title.inject(this.z_header);
        this.z_close.inject(this.z_header);
        //this.z_max.inject(this.z_header);
        //this.z_min.inject(this.z_header);
        
		
		this.z_close.set("text", "×");
        this.dispose.bind(this)
        this.z_close.addEvent("click", this.dispose.bind(this));

        this.z_header.inject(this.z_window);
        this.z_body.inject(this.z_window);
        this.z_footer.inject(this.z_window);
		
		this.z_window.store("tag", this);
		this.repositionVar = this.reposition.bind(this);

		this.initArg();

        window.addEvent("resize", this.repositionVar);
		
		var dragHandle = $(this.z_header);
        var myDrag = new Drag.Move(this.z_window, {
            // Drag选项 
            handle: dragHandle
        });
		

		
        
    },

	initArg:function(){
		//窗体调用者
		this.wincaller=null;
		//窗体关闭前的回调函数
		this.callback=null;
		//窗体的内容部分元素
		this.compontent=null;
		//窗体内容部分的原始位置
		this.compontentPlacement=null;
			
	},
	restoreCompontent:function(){
		
		try {
            if (this.compontent) {                
                if (this.options.disposeStyle == 'restore') {
                    this.compontent.setStyle("display", this.display);
                    this.compontent.inject(this.compontentPlacement);
                } else if (!flag) {
                    this.compontent.setStyle("display", this.display);
                    this.compontent.inject(this.compontentPlacement);
                } else if (this.options.disposeStyle == 'destroy') {
					this.compontent.set("width","0");
                    this.compontent.destory();
                } else {
                    this.compontent.destory();
                }
            }
        } catch (e) {

        }
		
	},
	//释放当前窗口，并返回下一个上一个可视窗口，
    dispose: function (flag) {
        //参数标识，是否释放窗体内的内容，默认是释放的
        //this.z_window.setStyle("display","none");       
		if(this.callback){
			this.callback();	
		}
        window.removeEvent("resize", this.repositionVar);		
        this.restoreCompontent();
		this.initArg();
		
        this.z_window.dispose();
        this.z_window.destroy();
        this.z_window = null;

        var zwins = $$(".z_window");
        var currentWin = null;
        if (zwins) {
            if (zwins.length > 0) {

                var zwins = $$(".z_window");
                if (zwins) {
                    zwins.each(function (item) {
                        if (item.getProperty("term") == (this.term - 1)) {
                            item.setStyle("display", "block");
                            currentWin = item.retrieve("tag");
                        } else {
                            item.setStyle("display", "none");
                        }
                    } .bind(this));
                }
            } else {
                $(document.body).unmask();
            }
        }
        return currentWin;

    },
	//显示某个网址在iframe，由于考虑跨域问题，直接传递宽高，需要自己计算
    showPage: function (url, width, height, caller,callback) {

        if (this.z_window == null) return;
		
        //根据有无window进行遮罩
        var zwins = $$(".z_window");
        if (zwins) {
            if (zwins.length < 1)
                $(document.body).mask();
        }
        this.z_window.setStyle("visibility", "visible");
        this.z_window.setStyle("display", "block");

		

		this.restoreCompontent();
		
        var iframe = new Element("iframe", { src: url,frameborder:0 });

        iframe.setStyles({ "width": width, "height": height });
        iframe.onload = iframe.onreadystatechange = function () {
            if (this.readyState && this.readyState != 'complete') return;
            else {
				var iDocument = iframe.document || iframe.contentDocument;
				var f = $(iDocument).getElementByTag("title");
			    if (f) this.z_title.set("text", f); else this.z_title.set("text", "默认标题");
            }

        }
		
       	this.z_body.empty();	   
        iframe.inject(this.z_body);
        this.z_window.inject(document.body);

        var scroll_size = $(document.body).getScroll();
        var body_size = $(document.body).getSize();

       
	   var topR=0;
        if (this.z_window.getStyle("height").toInt() >= window.getHeight().toInt()) {
			topR=scroll_size.y;
            this.z_window.setStyle("top", topR + "px");
        } else {
			topR=(body_size.y - height) / 3 + scroll_size.y;
           
			this.z_window.setStyle("top", ((body_size.y - height) / 3 + scroll_size.y) + "px");
        }


        this.z_window.setStyle("left", ((body_size.x - width) / 3 + scroll_size.x) + "px");

		if(topR<0)
		{
			this.z_window.setStyle("top",  "10px");
		}
        var zwins = $$(".z_window");
        if (zwins) {
            zwins.each(function (item) {
                if (item.getProperty("term") == this.term) {
                    item.setStyle("display", "block");
                } else {
                    item.setStyle("display", "none");
                }
            } .bind(this));
        }
		this.callback=callback;
        return this;


    },
	//去除框进行显示元素
	showWithoutWindow:function(compontent,caller,callback,closeBtn){
		
		
        if (this.z_window == null) return;
		
        if (typeof (compontent) == 'string')
            compontent = $(compontent);
		if(!compontent){
			alert('要显示的元素不存在：'+compontent);
			return;	
		}
		if (this.compontent == compontent) {
            return;
        }

		this.restoreCompontent();
        this.wincaller = caller;
        //当前窗口的父级窗口，这样有利于多层遮罩
        var zwins = $$(".z_window");
        if (zwins) {
            if (zwins.length < 1)
                $(document.body).mask();
        }

        this.z_window.setStyle("visibility", "visible");
        this.z_window.setStyle("display", "block");		
		this.z_header.dispose();
       
        this.z_window.inject(document.body);
        this.display = compontent.getStyle("display");
        this.compontent = $(compontent);
        this.compontent.setStyle("display","block");
        this.compontentPlacement = this.compontent.getParent();
        var compontent_size = this.compontent.getDimensions();
        var width = compontent_size.x;
        var height = compontent_size.y + this.z_header.getDimensions().y + this.z_footer.getDimensions().y;

        this.compontent.inject(this.z_body);

        //this.z_footer.inject(this.z_body);
        var compontent_size = this.compontent.getDimensions();
        var scroll_size = $(document.body).getScroll();
        var body_size = $(document.body).getSize();
		
		
		 var topR=0;
        if (this.z_window.getStyle("height").toInt() >= window.getHeight().toInt()) {
			topR=scroll_size.y;
            this.z_window.setStyle("top", topR + "px");
        } else {
           
		   topR=(body_size.y - compontent_size.y) / 3 + scroll_size.y;
			this.z_window.setStyle("top", topR + "px");
        }
		
		var dime= $(document.body).getDimensions({computeSize :true});
        this.z_window.setStyle("left", ((body_size.x - compontent_size.x) / 2 + scroll_size.x) + "px");


		if(topR<0)
		{
			this.z_window.setStyle("top",  "10px");
		}
		
		
		
        var zwins = $$(".z_window");
        if (zwins) {
            zwins.each(function (item) {
                if (item.getProperty("term") == this.term) {
                    item.setStyle("display", "block");
                } else {
                    item.setStyle("display", "none");
                }
            } .bind(this));
        }
		this.callback=callback;
		this.createedCallback();
        if(closeBtn){
		    closeBtn.addEvent("click", this.dispose.bind(this));
        }
        return this;
		
		
		
	},
    show: function (compontent, title, favicon, caller,callback) {

        if (this.z_window == null) return;
		
        if (typeof (compontent) == 'string')
            compontent = $(compontent);
		if(!compontent){
			alert('要显示的元素不存在：'+compontent);
			return;	
		}
		
		this.restoreCompontent();
        this.wincaller = caller;
        //当前窗口的父级窗口，这样有利于多层遮罩
        var zwins = $$(".z_window");
        if (zwins) {
            if (zwins.length < 1)
                $(document.body).mask();
        }

        this.z_window.setStyle("visibility", "visible");
        this.z_window.setStyle("display", "block");
        try {

            if (favicon)
                this.z_ico.set("html", "<img src='" + favicon + "'/>");
            else
                this.z_ico.setStyle("border", "1px dashed green");
        } catch (e) {
            this.z_ico.setStyle("background-color", "red");
        }
        if (title) this.z_title.set("text", title); else this.z_title.set("text", "默认标题");

      	if (this.compontent == compontent) {
            return;
        }

        this.z_window.inject(document.body);
        this.display = compontent.getStyle("display");
        this.compontent = $(compontent);
        this.compontent.setStyle("display","block");
        this.compontentPlacement = this.compontent.getParent();
        var compontent_size = this.compontent.getDimensions();
        var width = compontent_size.x;
        var height = compontent_size.y + this.z_header.getDimensions().y + this.z_footer.getDimensions().y;

        this.compontent.inject(this.z_body);

        //this.z_footer.inject(this.z_body);
        var compontent_size = this.compontent.getDimensions();
        var scroll_size = $(document.body).getScroll();
        var body_size = $(document.body).getSize();
		var topR=scroll_size.y;
        if (this.z_window.getStyle("height").toInt() >= window.getHeight().toInt()) {
            this.z_window.setStyle("top", topR + "px");
        } else {
           
		   topR=(body_size.y - compontent_size.y) / 3 + scroll_size.y;
			this.z_window.setStyle("top", topR + "px");
        }
		var dime= $(document.body).getDimensions({computeSize :true});
        this.z_window.setStyle("left", ((body_size.x - compontent_size.x) / 2 + scroll_size.x) + "px");

		if(topR<0)
		{
			this.z_window.setStyle("top",  "10px");
		}
		
		
        var zwins = $$(".z_window");
        if (zwins) {
            zwins.each(function (item) {
                if (item.getProperty("term") == this.term) {
                    item.setStyle("display", "block");
                } else {
                    item.setStyle("display", "none");
                }
            } .bind(this));
        }
		this.callback=callback;
		this.createedCallback();
		
        return this;
    },
	
	createedCallback:function(){
		if(!this.options.allowClose){
			this.z_close.dispose();
		}
	},
    reposition: function () {
        var compontent_size;
        if (this.compontent)
            compontent_size = this.compontent.getSize();
        else
            compontent_size = { "x": 0, "y": 0 };

        var scroll_size = $(document.body).getScroll();
        var body_size = $(document.body).getSize();

		this.z_window.setStyle("left", ((body_size.x - compontent_size.x) / 2 + scroll_size.x) + "px");
		var topR=(body_size.y - compontent_size.y) / 3 + scroll_size.y;
		if(topR<0){
			topR=10;	
		}
		this.z_window.setStyle("top", topR + "px");
        
    }
});



SimpleWindow.implement({
    alert: function (title,msg, callBack, owner) {
        var ok = new Element("span",
        { "class": "ok", "text": "确定" });
        ok.addEvent("click", function () {
            
            if (typeof (callBack) == 'function') {
                callBack.attempt("ok");
            }            
			this.dispose();
        } .bind(this));
        ok.inject(this.z_footer);

        var msgCont = new Element("div", { "text": msg, "class": "z_message" });
		
//		(compontent, title, favicon, caller,callback) 
        this.show(msgCont,title?title:"提示信息",null, owner);
    },
    confirm: function (title, msg, callBack, owner) {
        var ok = new Element("span",
        { "class": "ok", "text": "确定" });
        ok.addEvent("click", function () {

            
            if (typeof (callBack) == 'function') {
                callBack.attempt("ok");
            }
			this.dispose();
        } .bind(this));
        ok.inject(this.z_footer);

        var ok = new Element("span",
        { "class": "cancle", "text": "取消" });
        ok.addEvent("click", function () {
            
            if (typeof (callBack) == 'function') {
                callBack.attempt("cancle");
            }
			this.dispose();
        } .bind(this));
        ok.inject(this.z_footer);

        var msgCont = new Element("div", { "text": msg, "class": "msg" });
		//		(compontent, title, favicon, caller,callback) 
        this.show(msgCont, title?title:"提示信息", null, owner);
    },
    prompt: function (title, msg, callBack, owner) {
        var ok = new Element("span",
        { "class": "ok", "text": "确定" });
        ok.addEvent("click", function () {
            var vv = this.inputText.getProperty("value");
            this.dispose();
            if (typeof (callBack) == 'function') {
                callBack.attempt([vv, 'ok']);
            }

        } .bind(this));
        ok.inject(this.z_footer);

        var ok = new Element("span",
        { "class": "cancle", "text": "取消" });
        ok.addEvent("click", function () {
            var vv = this.inputText.getProperty("value");
            this.dispose();
            if (typeof (callBack) == 'function') {
                callBack.attempt([vv, 'cancle']);
                //callBack.attempt("cancle");
            }
            this.dispose();

        } .bind(this));
        ok.inject(this.z_footer);

        var msgCont = new Element("div", { "text": msg, "class": "msg" });
        this.inputText = new Element("input", { "type": "text", "class": "promptTxt" });
        this.inputText.inject(msgCont);
				//		(compontent, title, favicon, caller,callback) 
        this.show(msgCont,title?title:"提示信息", null, owner);
    }
   
});