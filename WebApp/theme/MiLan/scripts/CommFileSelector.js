//与照片选择对应的
//此类，依赖于plupload，pluploadUtil
var CommFileSelector = new Class({
    Implements: [Options],
    options: {
        wrap:'',//当前文件展示框
        photo: '',//选择完成后，显示图片的img对象
        selector: null,//选择图片的按钮，直接父级需要为div
        pathInput:null//图片路径防止的input元素
    },
    initialize: function (options) {
        this.setOptions(options);
        z_plupload({
            "selector":this.options.selector,
            selectorWrap: this.options.selector.getParent("div"),
            file:null,
            site:"CommFileSelector",
            code: "File",
            args: "",
            maxSize: '4096mb',
            SlectCallback: this.SlectCallback.bind(this),
            UploadCallback: this.UploadCallback.bind(this),
            UploadProgress: this.UploadProgress.bind(this)
        });

    },
    SlectCallback: function (file, up) {
        this.options.wrap.mask();
        up.start();
        if (this.options.file) {
            $(this.options.file).set('html',file.name);
        }
    },
    UploadCallback: function (json, uploader, file) {
        this.options.wrap.unmask();
        this.options.pathInput.set('value', json.orgfilepath);
        if (this.options.selector.value) { this.options.selector.set('value', '重新选择'); } else { this.options.selector.set('text', '重新选择'); }

    },
    UploadProgress: function (uploader, file) {
        var txt = file.percent == 100 ?
                        "上传中99.9%" :
                        '上传中' + file.percent + "%";

        if (this.options.selector.value) {
            this.options.selector.set('value', txt);
        } else {
            this.options.selector.set('text', txt);
        }
    }
});