//与照片选择对应的
//此类，依赖于plupload，pluploadUtil
var CommPhotoSelector = new Class({
    Implements: [Options],
    options: {
        wrap:'',//当前文件展示框
        photo: '',//选择完成后，显示图片的img对象
        selector: null,//选择图片的按钮，直接父级需要为div
        pathInput: null,//图片路径防止的input元素
        args: null,//参数 默认为thumbnailwith=120&thumbnailheight=150
        SlectCallback: null,
        UploadProgress:null,
        UploadCallback:null
    },
    initialize: function (options) {
        this.setOptions(options);
        z_plupload({
            "selector":this.options.selector,
            selectorWrap: this.options.selector.getParent("div"),
            site:"CommPhotoSelector",
            code: "Photo",
            args: this.options.args ? this.options.args : "thumbnailwith=120&thumbnailheight=150",
            maxSize: '4096mb',
            SlectCallback: this.SlectCallback.bind(this),
            UploadCallback: this.UploadCallback.bind(this),
            UploadProgress: this.UploadProgress.bind(this)
        });

    },
    SlectCallback: function (file, up) {
        this.options.wrap.mask();
        up.start();
        if (this.options.SlectCallback && typeOf(this.options.SlectCallback)=='function') {
            this.options.SlectCallback.bind(this)(file,up);
        }
    },
    UploadCallback: function (json, uploader, file) {
        this.options.wrap.unmask();
       
        if (this.options.photo) {
            this.options.photo.set('src', json.res == 'ok'&&json.thumbnailfilepath ? json.thumbnailfilepath : json.orgfilepath);
        }        
        this.options.pathInput.set('value', json.orgfilepath);
        if (this.options.selector.value) {
            this.options.selector.set('value', '重新选择');
        } else if (this.options.selector.get('tag')=='img') {
        }else{
            this.options.selector.set('text', '重新选择');
        }
        if (this.options.UploadCallback) {
            this.options.UploadCallback.bind(this)(json, uploader, file);
        }
    },
    UploadProgress: function (uploader, file) {
        
        var txt = file.percent == 100 ?
                        "上传中99.9%" :
                        '上传中' + file.percent + "%";
        if (this.options.UploadProgress && typeOf(this.options.UploadProgress) == 'function') {
            this.options.UploadProgress.bind(this)(file, uploader, txt);
            return;
        }
        if (this.options.selector.value) {
            this.options.selector.set('value', txt);
        } else {
            this.options.selector.set('text', txt);
        }
    }
});