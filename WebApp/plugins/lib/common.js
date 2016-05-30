/*
ValidationRuleLoad（验证规则文件）
验证规则文件位置：/page/validationConfig/验证文件.xml()
SubmitForm2(表单，回调函数，调用事件的按钮)
表单附加属性：
action:请求地址
target：请求数据应该放置的位置
调用事件的按钮附加属性：
param：附加参数
form:需要额外提交的表单参数
actionTip:提交前的提示信息
返回值：Json字符串     ｛res:"",tip:"",field:""｝
ok：关闭window	
alert：弹出tip
alertOk:弹出tip，并且关闭window	
exists：表单元素已经存在。
alertRefresh:提交按钮的属性(窗体调用者的按钮)，reFreshBtn   关闭并且 刷新
alertOKRefresh:提交按钮的属性(窗体调用者的按钮)，reFreshBtn  刷新不关闭 
OkRefresh:提交按钮的属性(窗体调用者的按钮)，reFreshBtn  刷新关闭 
刷新按钮，需要有class  【reFreshBtn】

ShowWindow2（调用弹出窗体的按钮）
action：请求地址
param：附加参数
form：需要提交的表单
initFunction：回调语句
注：返回的页面，只能有一个根级元素
*/
//依赖于：validation-framework.js
function ValidationRuleLoad(rule) {
    ValidationFramework.init("/validationForm/" + rule + ".aspx");
}
//ajax 的url，主要用于添加请求地址的前缀，后缀
function ajaxUrl(url) {

    var random = new Date().pattern("yyyyMMddhhmmssS") + "-" + String.uniqueID();

    if (url.startWith("http://") || url.endWith(".aspx")) {
        url = url + "?random23435=" + random;
        return url;
    }

    if ("undefined" == typeof ajaxSys) {
        return "/sys/" + url + ".aspx?random2345=" + random;
    } else {
        return ajaxSys + url + ".aspx?random2345=" + random;
    }

}
function UrlWithParam(url, param) {
    if (param)
        return ajaxUrl(url) + "?" + param;
    else
        return ajaxUrl(url);
}

function SubmitForm2(form, backMethod, btn) {
	SubmitForm3(form,null,backMethod,btn);
}

//提交一个普通表单
//表单id｛action，｝
function SubmitForm3(form, commitBefore, backMethod, btn) {
    
    //form = $(form);
    
    form = CssFirstElement(btn, form);

    if (!form) {
        alert("您提交的表单，无法识别，请检测表单id");
        return;
    }
    var action = form.getProperty("action");
    if (!action) {
        alert("您提交的表单，无法识别请求地址，请检查action");
        return;
    }
    if (form) {
        form.set('spinner', { "destroyOnHide": "true", "message": "..." });
    }
    if (btn) {
        form.set('spinner', { "destroyOnHide": "true", "message": "..." });
    }
    if (form != btn) {
        form.spin();
    } else {
        form.setProperty("noValidate", "noValidate");
        form.spin();
    }

    if (form.get("spiner") == 'no') {
        form.unspin();
    }
    //form.getElements(".codemirror").each(function (item) {
    //    item.value= $(item).retrieve("codemirror").getValue();
    //});

    form.getElements(".kindEditor").each(function (item) {       
         $(item).retrieve("keditor").sync();
    });
    
    form.getElements("*[contenteditable=true]").each(function (item) {
        
        var eleOld = item.retrieve("contentArea");
        if (!eleOld) {
            var name = item.getProperty("name");
            var ele = new Element("textarea", {"name":name});
            ele.setStyle("display", "none");
            
            ele.setProperty("value", item.innerHTML);
        
            item.store("contentArea", ele);
            ele.inject(form);
            
        }else{
            eleOld.setProperty("value", item.innerHTML);
        }
    });

   
    if (commitBefore) {
        if (!commitBefore.bind(btn, form, btn)()) {
            form.unspin();
            return;
        }
    }

    
    
    form = $(form);
    
    if (btn && btn.getProperty("actionTip")) {
        //        new SimpleWindow().confirm("提示", btn.getProperty("actionTip") == '' ? "确定要删除么？" : btn.getProperty("actionTip"), function (result) {
        //            if (result == 'ok') {
        //                InnerSubmitForm();
        //            }
        //        }, btn);
        if (confirm(btn.getProperty("actionTip") == '' ? "确定要删除么？" : btn.getProperty("actionTip"))) {
            InnerSubmitForm();
        } else {
            form.unspin();
        }
        
        return;
    } else if (form && form.getProperty("actionTip")) {
        if (confirm(btn.getProperty("actionTip") == '' ? "确定要删除么？" : btn.getProperty("actionTip"))) {
            InnerSubmitForm();
        } else {
            form.unspin();
        }
        return;
    } else {
        InnerSubmitForm(); return;
    }
    var myrequest;
    function InnerSubmitForm() {
        //生成随机码，防止从浏览器缓存获取数据

       
        //if (form.getProperty("enctype") || form.getProperty("enctype")!='application/x-www-form-urlencoded') {

        if (form.getProperty("enctype") && form.getProperty("enctype") == 'multipart/form-data') {
            // if (form.get("tag").toLowerCase() == 'form') {

            var random = new Date().pattern("yyyyMMddhhmmssS") + "-" + String.uniqueID();
            var formTarget = form.getProperty("target") ? form.getProperty("target") : random;

            if (!(form.getProperty("loaded"))) {
                form.setProperty("action", ajaxUrl(form.getProperty("action")));
                form.setProperty("method", "post");
                form.setProperty("target", formTarget);
                form.setProperty("enctype", "multipart/form-data");
                form.setProperty("loaded", true);

            }

            if (!$(document.body).getElement("iframe[name=" + formTarget + "]")) {
                var frame = new Element("iframe", { "name": "" + formTarget + "", "id": "" + formTarget + "" });

                frame.setStyle("display", "none");
                frame.inject(document.body);
            }

            var iframe = $(document.body).getElement("iframe[name=" + formTarget + "]");
            iframe.store("backmethod", backMethod);
            iframe.store("form", form);
            iframe.store("btn", btn?btn:currentWindow?currentWindow.wincaller
                :null);
            if (iframe.attachEvent) {
                iframe.attachEvent("onload", function () {
                    var backMethod = iframe.retrieve("backmethod");
                    var form = iframe.retrieve("form");
                    var btn = iframe.retrieve("btn");
                    parent.SuccessBackMethod(iframe.contentWindow.document.body.innerHTML, backMethod, form, btn);

                });
            } else {
                iframe.onload = function () {
                    var backMethod = iframe.retrieve("backmethod");
                    var form = iframe.retrieve("form");
                    var btn = iframe.retrieve("btn");
                    parent.SuccessBackMethod(iframe.contentDocument.body.innerHTML, backMethod, form, btn);

                };
            }

            if (btn) {
                var bparam = btn.getProperty("param");
                if (bparam) {
                    bparam.split("&").each(function (b) {
                        var barr = b.split('=');
                        if (barr.length == 2) {
                            var ele = form.getElement('input[name=' + barr[0] + ']');
                            if (ele && ele.get('zformEle')) {
                                ele.dispose();
                            } 
                            new Element("input", { "zformEle": "zformEle", "name": barr[0], "value": barr[1], "type": "hidden" }).inject(form);
                            
                            
                        }
                    });
                }
                var bform = btn.getProperty("form");
                if ($(bform)) {
                    $(bform).toQueryString().split("&").each(function (b) {
                        var barr = b.split('=');
                        if (barr.length == 2) {
                            var ele = form.getElement('input[name=' + barr[0] + ']');
                            if (ele && ele.get('zformEle')) {
                                ele.dispose();
                            }
                            new Element("input", { "zformEle": "zformEle", "name": barr[0], "value": barr[1], "type": "hidden" }).inject(form);
                        }
                    });
                }

            }

           
            // new Element("input", { "name": random, "value": random+"_3", "type": "hidden" }).inject(form);
            form.submit();

            return;
        }


        myrequest =
	        new Request({
	            url: ajaxUrl(action),
	            encoding: "UTF-8",
	            method: "get",
	            evalScripts: true,
	            onComplete: function () { },
	            onRequest: function () { },
	            onException: function () { },
	            onSuccess: function (responseText, responseXml) {	                
	                SuccessBackMethod(responseText, backMethod, form, btn);	               
	            }
	        });
       


        var param = "";
        if (btn) {
            btn = $(btn);
            param = btn.getProperty("param");
        }

        if (!param) {
            param = $(form).toQueryString();
        } else {
            param = param + "&" + $(form).toQueryString();
        }

        if (btn && btn.getProperty("form")) {
            var btnForm = CssFirstElement(btn, btn.getProperty("form"));
            
            if ($(btnForm)) {
                param = param + "&" + $(btnForm).toQueryString();
            } else {
                alert("找不到表单：" + btnForm);
            }
        }
        if (btn == form && btn.value) {
            param += "&" + btn.name + "=" + btn.value;
        }
        myrequest.send(param);
    }
}
function CssFirstElement(ele, css) {
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
}
function CssElements(ele,css) {
    var targetInpage;
    if (typeOf(css) == 'string' && css.startWith("css:#")) {
        return targetInpage = $$(css.substring(4));
    } else if (typeOf(css) == 'string' && css.startWith("css:")) {
        return targetInpage = ele.getParents(css.substring(4));
    } else {
        return $(css)?new Array($(css)):new Array();
    }    
}
function SuccessBackMethod(responseText, backMethod, form, btn) {
    form = $(form);

    // try {
    if (!responseText.startWith("{")) {
       
        var targetInpage = form.getProperty("target");
       
        targetInpage = CssFirstElement(form, targetInpage);

        if (targetInpage) {
            if ($(targetInpage)) {
                //targetInpage.empty();                
                //targetInpage.set("html", '正在加载……');

                if (form.get("replaceTarget")) {
                    var tempTarget = new Element("div");
                    tempTarget.setStyles({
                        position: "absolute",
                        left: "-9999px",
                        top: "-999px"
                    });
                    tempTarget.inject(document.body);
                    tempTarget.set('html', responseText);
                    var replaceStyle = form.get("replaceTarget").toLowerCase();
                    if (replaceStyle == 'replacetarget' || replaceStyle=='true') {
                        //替换目标元素本身，在目标原来的位置增加元素
                        targetInpage.setStyle('display', 'none');
                        tempTarget.getElements(">").each(function (ele) {
                            ele.inject(targetInpage, "before");
                        });
                        targetInpage.dispose();
                    } else if (replaceStyle == 'replaceafter') {
                        //将目标元素之后的兄弟节点移除，并将新的元素添加的目标元素之后
                        targetInpage.getAllNext().each(function (it) {
                            it.dispose();
                        });
                        var tarParent = targetInpage.getParent();
                        tempTarget.getElements(">").each(function (ele) {
                            ele.inject(tarParent, "bottom");
                        });
                    } else if (replaceStyle == 'replacebefore') {
                        //将目标元素之后的兄弟节点移除，并将新的元素添加的目标元素之后
                        targetInpage.getAllNext().each(function (it) {
                            it.dispose();
                        });
                        tempTarget.getElements(">").each(function (ele) {
                            ele.inject(targetInpage, "before");
                        });
                    } else {

                    }
                } else {
                    targetInpage.set('html',responseText);
                }

                var zdomready= function(){
                    window.fireEvent('zdomready');
                    window.removeEvents('zdomready');
                }
                zdomready.delay(300);
            } else {
                console.log("出现了html代码，您需要指定target属性");
                //alert("目标target不存在：" + targetInpage);
            }
            form.unspin();

            if (backMethod) {
                backMethod.attempt(null, form, btn);
            }

            return;
        } else {
            // alert("出现了html代码，您需要指定target属性");
            console.log("出现了html代码，您需要指定target属性");
            //alert(responseText);
            form.unspin();
            return;
        }
    }

    var json = null;
    var tempContainer = $("tempDiv");
    try {
        
        if (!tempContainer) {
            tempContainer = new Element("div", { "display": "none", "id": "tempDiv" });
            tempContainer.inject(document.body);
        }
        tempContainer.set("html",responseText);
        json = JSON.decode(responseText);
    } catch (Exception) {
        //alert("json解析出差%*&…^");
        form.unspin();
        return;
    }
    tempContainer.set("html","");

    if (backMethod) {
        try{
            backMethod.attempt([json,form,btn]);
            form.unspin();
            return;
        } catch (ex) {
            alert(ex);
        }
    }

    var backMethod2;
    if (form) {
        backMethod2 = form.getProperty("backMethod")
    }
    if (backMethod2) {
        //var tempt = Function.from(backMethod2 + "(" + json + "," + form + "," + btn + ")");
        var tempt = Function.from(backMethod2);
        tempt.attempt(json,form,btn);
        form.unspin();
    } else {
        defaultBackMethod_2(json, form, btn);
    }
    //	                } catch (Exception) {
    //	                    alert(Exception.name + ":" + Exception.description);
    //	                    new SimpleWindow().alert(responseText);
    //	                }



}

var ok = 'ok';
var fail = "fail";
var zalert = "alert";
var zerror = "error";
var alertOk = "alertOk";
var exists = "exists";
var alertOKRefresh = "alertOKRefresh";
var alertRefresh = "alertRefresh";
var OkRefresh = "OkRefresh";
function defaultBackMethod_2(json, form, btn) {
    form = $(form);
    if (json.res == ok) {
       
        form.unspin();

        debugger;
        if (btn) {
            var reRefreshBtn = $(btn).getProperty("reFreshBtn");
            if (reRefreshBtn) {
                //var reRefreshBtnObj=  CssFirstElement($(btn),reRefreshBtn);
                var reRefreshBtnObj = CssElements($(btn), reRefreshBtn);
                if (reRefreshBtnObj) {
                    reRefreshBtnObj.each(function (obj, index) {
                        var xx = function () {
                            try {
                                obj.click();
                            } catch (exc) {
                                alert(obj.get("onclick"));
                            }
                        }
                        xx.delay(10 * index);
                    });// reRefreshBtnObj.click();
                }
                else { }
            } else {
                //alert("无法完成刷新请求，未找到刷新按钮");
            }
        } else if (currentWindow.wincaller) {
            var reRefreshBtn = $(currentWindow.wincaller).getProperty("reFreshBtn");
            if (reRefreshBtn) {

                var reRefreshBtnObj = CssFirstElement(currentWindow.wincaller, reRefreshBtn);
                if (reRefreshBtnObj) reRefreshBtnObj.click();
                else { }

            } else {
                //alert("无法完成刷新请求，未找到刷新按钮");
            }
        }


        if (currentWindow) {
            currentWindow = currentWindow.dispose();
        }
        return;
    } else if (json.res == zalert) {
        //new SimpleWindow().alert(json.tip);
        alert(json.tip);
        form.unspin();
        return;
    } else if (json.res == alertOk) {
        new SimpleWindow().alert("提示",json.tip, function () {
            if (currentWindow) {
                currentWindow = closeCurrentWindow();
            }
        });
        form.unspin();
        return;
    } else if (json.res == alertRefresh || json.res == alertOKRefresh||json.res==OkRefresh) {
        //new SimpleWindow().alert(json.tip);
        if (json.res != OkRefresh) {
            alert(json.tip);
        }

        
        
        if (btn || currentWindow.wincaller) {
            var btnTemp;
            var reRefreshBtn = $(btn).getProperty("reFreshBtn");
            if (reRefreshBtn) {
                btnTemp = btn;
            } else {
                reRefreshBtn = $(currentWindow.wincaller).getProperty("reFreshBtn");
                btnTemp = currentWindow.wincaller;
            }
            if (reRefreshBtn) {
                //var reRefreshBtnObj=  CssFirstElement($(btn),reRefreshBtn);
                var reRefreshBtnObj = CssElements($(btnTemp), reRefreshBtn);
                if (reRefreshBtnObj) {
                    reRefreshBtnObj.each(function (obj, index) {
                        var xx = function () {
                            try{
                                obj.click();
                            }catch(exc){
                                alert(obj.get("onclick"));
                            }
                        }
                        xx.delay(10*index);
                    });// reRefreshBtnObj.click();
                }
                else { }
            } else {
                //alert("无法完成刷新请求，未找到刷新按钮");
            }
        }else {
            //alert("未找到调用窗体按钮按钮");
        }

       
        form.unspin();
        if (json.res != alertRefresh) {
            currentWindow = currentWindow.dispose();
        }
        //if (currentWindow && (json.res == alertOKRefresh || json.res == OkRefresh)) {
        //    currentWindow = currentWindow.dispose();
        //}
        return;
    } else if (json.res == exists || json.res == 'notbenull') {
        var field=form.getElement("*[name=" + json.field+ "]");
        //var field = form.getElementById(json.field);
        
        if (field) {
            var msg = field.getParent(".hold").getElement(".msg");
            if (msg) {
                var tip = json.tip ? json.tip : field.getParent().getFirst().get("text").replace(":", "").replace("：", "") + " 已存在";
                msg.set("html", tip);
                if (!field.hasClass(ValidationFailClass)) {
                    msg.removeClass(ValidationOkClass);
                    msg.addClass(ValidationFailClass);
                    field.addClass(ValidationFieldFailClass);
                }

            } else {
                alert(json.tip);
            }
        } else {
            alert("表单元素：【" + form.getProperty("id") + ",#" + json.field + "】不存在");
        }
        form.unspin();
        return;
    } else {
        if (currentWindow) {
            if (currentWindow) {
                currentWindow = currentWindow.dispose();
            }
        }
        alert("请求得到了未知的json……");
        form.unspin();
        return;
    }

}
var currentWindow;
function ShowWindow2(btn) {
    btn = $(btn);
    btn.spin();
    var action = btn.getProperty("action");
    var param = btn.getProperty("param");
    var form = btn.getProperty("form");
    if (!action) {
        alert("未配置action");
    }
    if (!param) {
        param = "";
    }
    if (form) {
        form = CssFirstElement(btn,form);
        if (form) {

            param = param + "&" + $(form).toQueryString();

            if (form.getProperty("noValidate") || form.get("tag")!='form') {
            } else {
                ValidationRuleLoad(form.getProperty("validateForm") ? form.getProperty("validateForm") : form.getProperty("id"));
                var validateRes = doValidate(form.getProperty("id"));
                if (validateRes == "undefined") {
                    //没有找到表单，不需要验证
                    alert("配置的表单验证规则不正确，请检查");
                    btn.unspin();
                    return false;
                } else if (!validateRes) {
                    //验证不通过，不需要提交
                    btn.unspin();
                    return;
                }
            }
        } else {
            alert("配置了错误的form");
            btn.unspin();
            return;
        }
    }
    param = param + btn.toQueryString();
    var myrequest =
	        new Request({
	            url: ajaxUrl(action),
	            encoding: "UTF-8",
	            method: "post",
	            evalScripts: true,
	            onComplete: function () { },
	            onRequest: function () { },
	            onException: function () { },
	            onSuccess: function (response) {
	                btn.unspin();
	                var tempEle = new Element("div");
					tempEle.setStyle("display","none");
	                tempEle.set("html", response);
					tempEle.inject(document.body);

					tempEle.setStyle("position", "absolute");
					tempEle.setStyle("left", "-1000px");
					tempEle.setStyle("display", "block");


	                var resEle = tempEle.getFirst();
	                if (!resEle && response.startWith("{")) {
	                    try {
	                        var json = JSON.decode(response);
	                        defaultBackMethod_2(json, btn, btn);
	                    } catch (ea) {
	                        alert(ea);
	                        new SimpleWindow().alert("页面未正确返回!");
	                        alert(response);
	                    }
	                    return;

	                }
	                if (!resEle) {
	                    resEle = new Element("div", {'html':response});
                        
	                }

	                //延迟执行函数
	                var initFunction = resEle.getProperty("initFunction") ? resEle.getProperty("initFunction") : resEle.getProperty("initfunction");
	                if (initFunction) {
	                    setTimeout(initFunction, 500);
	                }
	                window.fireEvent('zdomready');
	                window.removeEvents('zdomready');
	                currentWindow = new SimpleWindow();	           
	                currentWindow.show(resEle, resEle.getProperty("windowTitle") ? resEle.getProperty("windowTitle") : resEle.getProperty("windowtitle"), resEle.getElement("favicon"), btn);
	            }
	        });
    myrequest.send(param);
}


function ShowWindow3(btn,close) {
    btn = $(btn);
    btn.spin();
    var action = btn.getProperty("action");
    var param = btn.getProperty("param");
    var form = btn.getProperty("form");
    if (!action) {
        alert("未配置action");
    }
    if (!param) {
        param = "";
    }
    if (form) {
        form = CssFirstElement(btn, form);
        if (form) {

            param = param + "&" + $(form).toQueryString();

            if (form.getProperty("noValidate") || form.get("tag") != 'form') {
            } else {
                ValidationRuleLoad(form.getProperty("validateForm") ? form.getProperty("validateForm") : form.getProperty("id"));
                var validateRes = doValidate(form.getProperty("id"));
                if (validateRes == "undefined") {
                    //没有找到表单，不需要验证
                    alert("配置的表单验证规则不正确，请检查");
                    btn.unspin();
                    return false;
                } else if (!validateRes) {
                    //验证不通过，不需要提交
                    btn.unspin();
                    return;
                }
            }
        } else {
            alert("配置了错误的form");
            btn.unspin();
            return;
        }
    }
    param = param + btn.toQueryString();
    var myrequest =
	        new Request({
	            url: ajaxUrl(action),
	            encoding: "UTF-8",
	            method: "post",
	            evalScripts: true,
	            onComplete: function () { },
	            onRequest: function () { },
	            onException: function () { },
	            onSuccess: function (response) {
	                btn.unspin();
	                var tempEle = new Element("div");
	                tempEle.setStyle("display", "none");
	                tempEle.set("html", response);
	                tempEle.inject(document.body);

	                tempEle.setStyle("position", "absolute");
	                tempEle.setStyle("left", "-1000px");
	                tempEle.setStyle("display", "block");


	                var resEle = tempEle.getFirst();
	                if (!resEle && response.startWith("{")) {
	                    try {
	                        var json = JSON.decode(response);
	                        defaultBackMethod_2(json, btn, btn);
	                    } catch (ea) {
	                        alert(ea);
	                        new SimpleWindow().alert("页面未正确返回!");
	                        alert(response);
	                    }
	                    return;

	                }
	                if (!resEle) {
	                    resEle = new Element("div", { 'html': response });

	                }

	                //延迟执行函数
	                var initFunction = resEle.getProperty("initFunction") ? resEle.getProperty("initFunction") : resEle.getProperty("initfunction");
	                if (initFunction) {
	                    setTimeout(initFunction, 500);
	                }
	                window.fireEvent('zdomready');
	                window.removeEvents('zdomready');
	                currentWindow = new SimpleWindow();
	                currentWindow.showWithoutWindow(resEle, btn,null,close?resEle.getElement(close):null);
	            }
	        });
    myrequest.send(param);
}



//关闭当前窗口
function closeCurrentWindow() {
    if(currentWindow)
        currentWindow = currentWindow.dispose();
}
function GoPage(url) {
    var ax = new Element("a", { "href": url,"target":"_self" });
    ax.setStyle("display", "none");
    ax.inject(document.body);
    ax.click();
    ax.dispose();
}

function GoPage2(url) {
    var ax = new Element("a", { "href": url, "target": "_blank" });
    ax.setStyle("display", "none");
    ax.inject(document.body);   
    ax.click();
    ax.dispose();
}



function downloadFile(btn) {
    var param = $(btn).getProperty("param");
    var filepath = $(btn).getProperty("filepath");
    var id = $(btn).getProperty("id");
    if (param) {
        param = "filepath=" + filepath + "&" + param;
    } else {
        param = "filepath=" + filepath;
    }
    var action = $(btn).getProperty("action");
    if (!action) {
        action = "KindEditorDownloadFile";
    }
    param += "&callback=SuccessBackMethod&formId="+id+"&btnId="+id;

    var acor = new Element("a", { "href": UrlWithParam(action, param), "target": "tempIframe" });
    acor.inject($("tempContainer"));
    acor.click();
    acor.destroy();
}

function downloadErrMsg(msg) {
    alert(msg);
}


/**
path:请求路径
param:请求参数
backMethod:回调方法
*/
function requestServer(path, param, requestmsg, requestcomplatemsg, backMethod) {
    var myrequest =
	        new Request({
	            url: ajaxUrl(path),
	            encoding: "UTF-8",
	            method: "post",
	            evalScripts: true,
	            onComplete: function () { },
	            onRequest: function () {  },
	            onException: function () { },
	            onSuccess: backMethod ? backMethod : defaultBackMethod
	        });
    myrequest.send(param);
}
function defaultBackMethod(responseText) {
    if (responseText.startWith("{")) {
        var json = JSON.decode(responseText);
        if (json.res == 'ok') {

        } else {
            alert(json.tip);
        }
    }
    if (currentWindow) {
        closeCurrentWindow();
    }


}
function pointerX() {
    return event.pageX || (event.clientX + (document.documentElement.scrollLeft || document.body.scrollLeft));
}
function pointerY() {
    return event.pageY || (event.clientY + (document.documentElement.scrollTop || document.body.scrollTop));
}
/**
把容器放到指定元素的下面：
field:指定元素
container:容器
x_offset:偏移量x
y_offset:偏移量y
overlap:是否遮住指定元素
*/
function setRelPosition(field, container, x_offset, y_offset, overlap,relative) {
    //var field_position = field.getCoordinates(currentWindow);
    //var top = field_position.top + y_offset;
    //top += !overlap ? field_position.height : 0;
 
    //container.setStyles({
    //    top : top,
    //    left : field_position.left + x_offset
    //});

}
function LocationDomain() {
    return window.location.host;
}

/* 提示用户升级浏览器**/
//window.addEvent("domready", function () {
//    if (Browser.ie6 || Browser.ie7) {

//        var ieshengji = new Element("div", { "id": "ie6shengji", "html": "亲爱的朋友，您正在使用浏览器版本严重过低(IE6、IE7，或使用IE内核的国产浏览器)，为了更好的操作体验，请升级至 IE8，<a href='/res/ChromeStandalone_Setup.1416381114.exe'>Chrome</a>，<a href='http://www.firefox.com.cn/' target='_blank'>火狐</a>等新版浏览器。" });
//        ieshengji.setStyles({
//            "position": "absolute",
            
//            "top": "0px",
//            "left": "0px",
//            "font-size": "14px",
//            "color": "#ff0000",
//            "width": "100%",
//            "background-color": "#FFFF00",
//            "padding": "2px 15px 2px 23px",
//            "text-align": "center"
//        });


//        ieshengji.inject($(document.body));

//        if (!window.XMLHttpRequest)
//            window.onscroll = function () {
//                ieshengji.setStyle("top", (document.documentElement.scrollTop) + "px");
//                ieshengji.setStyle("left", (document.documentElement.scrollLeft) + "px");
//            }
//        else ieshengji.setStyle("position", "fixed");

//    }
//});


function AjaxMootools(tag) {
    tag = $(tag);
    if (!tag) {/*alert('AjaxTag 没有tag'+tag);*/ return; }
    var url = tag.getProperty("url"), param = tag.getProperty("param");
    if (!url) { alert('AjaxTag 没有url'); return; }
    if (param) {
        param += ("&" + tag.toQueryString());
    }

    tag.setProperty("action", url);
    tag.setProperty("target", tag.getProperty("id"));
    tag.setProperty("param", param);
    SubmitForm2(tag, null, tag);
}