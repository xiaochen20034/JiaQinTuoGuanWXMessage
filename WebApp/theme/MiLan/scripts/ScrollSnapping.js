var ScrollSnapping = new Class({
    Implements: [Options],
    options: {
        ele: ''
    },
    initialize: function (options) {
        this.setOptions(options);
        this.element = $(this.options.ele);
        this.elementPos = this.getElementPos(this.element);
        this.judgeFun = this.judge.bind(this)
        window.addEvent('scroll', this.judgeFun)
        window.addEvent('resize', this.judgeFun)
       
    },
    getElementPos: function (el) {
        return {
            x: el.offsetParent ? el.offsetLeft + arguments.callee(el.offsetParent)['x'] : el.offsetLeft,
            y: el.offsetParent ? el.offsetTop + arguments.callee(el.offsetParent)['y'] : el.offsetTop
        }
    },
    judge: function () {
        if ($(document.body).getScroll().y.toInt() >= this.elementPos.y.toInt()) {
            this.Snapp();
        } else {
            this.unSnapp();
        }

    },

    Snapp: function () {
        this.element.setStyles({
            'position': 'fixed',
            'top': "0px"
        });
    },
    unSnapp: function () {
        this.element.setStyles({
            'position': '',
            'top': ""
        });
    }
})