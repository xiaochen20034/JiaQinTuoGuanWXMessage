//与照片选择对应的
//此类，依赖于plupload，pluploadUtil
var CommVideoSelector = new Class({
    Implements: [Options],
    options: {
        wrap:'',//当前文件展示框
        videoImg: '',//选择完成后，显示视频截图图片的img对象
        videoLen: '',//选择完成后，显示视频时长对象
        selector: null,//选择图片的按钮，直接父级需要为div
        VideoFileInput: null,//视频的路径输入框
        VideoImgInput: null,//视频的缩略图输入框
        VideoLenInput: null,//视频的时长输入框
    },
    initialize: function (options) {
        this.setOptions(options);
        z_plupload({
            "selector":this.options.selector,
            selectorWrap: this.options.selector.getParent("div"),
            site:"CommVideoSelector",
            code: "",
            url: "/VideoUpload.aspx",
            args: "",
            maxSize: '4096mb',
            splitSize:'4mb',
            SlectCallback: this.SlectCallback.bind(this),
            UploadCallback: this.UploadCallback.bind(this),
            UploadProgress: this.UploadProgress.bind(this)
        });
       
       
        this.width = $(this.options.selector).getStyle('width');
    },
    SlectCallback: function (file, up) {
        this.options.wrap.mask();
        up.start();
        this.options.selector.setStyle('width', '320px');
    },
    UploadCallback: function (json, uploader, file) {
        this.options.wrap.unmask();
        if (json.res != 'ok') {
            alert(json.tip);
            return;
        }
        
        this.options.selector.setStyle('width', this.width);

        this.options.videoImg.set('src', json.Thumb);
        if (this.options.videoLen) {
            this.options.videoLen.set('html', json.Duration);
        }
        if (this.options.VideoFileInput) {
            this.options.VideoFileInput.set('value', json.WebPath);
        }
        if (this.options.VideoImgInput) {
            this.options.VideoImgInput.set('value', json.Thumb);
        }
        if (this.options.VideoLenInput) {
            this.options.VideoLenInput.set('value', json.Duration);
        }
        
        if (this.options.selector.value) { this.options.selector.set('value', '重新选择'); } else { this.options.selector.set('text', '重新选择'); }

    },
    UploadProgress: function (uploader, file) {
        var txt = file.percent == 100 ?
                       "上传中99.9%" :
                       ('上传中' + file.percent + "%" + "  " + file.loaded + "/" + file.origSize);
        
        if (this.options.selector.value) {
            this.options.selector.set('value', txt);
        } else {
            this.options.selector.set('text', txt);
        }
    }
});