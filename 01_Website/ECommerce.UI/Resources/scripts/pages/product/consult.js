var consult = {

    preTimePoint: 0,

    checkTimeLimitBeforePublish: function (postTimeLimit) {
        var nowTime = new Date();
        var nowMinitePoint = nowTime.getHours() * 3600 + nowTime.getMinutes() * 60 + nowTime.getSeconds();
        if (nowMinitePoint - consult.preTimePoint < postTimeLimit) {
            return false;
        }
        return true;
    },
    resetPublishLimitTime: function () {
        var nowTime = new Date();
        var nowMinitePoint = nowTime.getHours() * 3600 + nowTime.getMinutes() * 60 + nowTime.getSeconds();
        consult.preTimePoint = nowMinitePoint;
    },

    createconsult: function (obj) {
        consult.CheckLogin();
        if (!consult.checkTimeLimitBeforePublish(60)) {
            window.alert("很抱歉，您发表咨询的频率过快，请稍后再试。")
            return;
        }
        var rad = document.getElementsByName("rd1")
            , $btn = $(obj);
        var value;
        for (var i = 0; i < rad.length; i++) {
            if (rad[i].checked) {
                value = rad[i].value;
            }
        }

        var consultinfo = {
            Content: $("#txtConsult").val(),
            ProductSysNo: $("#ProductSysNo").val(),
            Type: value
        };

        if (consultinfo.Type == "") {
            window.alert("请选择咨询类别！");
            return;
        }
        if (consultinfo.Content.length <= 0) {
            window.alert("咨询内容不能为空！");
            return;
        }
        else if (consultinfo.Content.length > 300) {
            window.alert("咨询内容长度不能大于300！");
            return;
        }

        $.ajax({
            url: $("#CreateConsult").val(),
            type: 'POST',
            timeout: 30000,
            data: consultinfo,
            dataType: 'json',
            beforeSend: function (XMLHttpRequest) { $btn.removeClass("btn_consult").addClass("btn_disconsult"); },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data.error) return;
                if (data <= 0) {
                    window.alert("发表咨询失败！");
                }
                else {
                    window.alert("发表咨询成功！");
                    consult.resetPublishLimitTime();
                }
            },
            complete: function () {
                $btn.removeClass("btn_disconsult").addClass("btn_consult");
            }
        });
    },

    ///咨询详情页页脚处回复
    createconsultReply: function (obj) {
        consult.CheckLogin();
        if (!consult.checkTimeLimitBeforePublish(60)) {
            window.alert("很抱歉，您发表咨询的频率过快，请稍后再试。")
            return;
        }
        var $btn = $(obj);

        var CreateConsultReply = {
            Content: $("#txtconsultrep").val(),
            ConsultSysNo: $("#ConsultSysNo").val(),
            ProductName: $("#ProductName").val(),
            Email: $("#Email").val(),
            ProductSysNo: $("#ProductSysNo").val()
        };

        if (CreateConsultReply.Content == null || CreateConsultReply.Content.length <= 0) {
            window.alert("回复内容不能为空！");
            return;
        }
        else if (CreateConsultReply.Content.length > 300) {
            window.alert("回复内容长度不能超过300！");
            return;
        }

        $.ajax({
            url: $("#CreateConsultReply").val(),
            type: 'POST',
            timeout: 30000,
            dataType: "json",
            data: CreateConsultReply,
            beforeSend: function (XMLHttpRequest) { $btn.removeClass("btn_consult").addClass("btn_disconsult").attr("onclick", ""); },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data.error) return;
                if (data != "1") {
                    window.alert(data);
                }
                else {
                    window.alert("发表咨询回复成功！");
                    consult.resetPublishLimitTime();
                }
            },
            complete: function () {
                $btn.removeClass("btn_disconsult").addClass("btn_consult").attr("onclick", "consult.createconsultReply(this);");
            }
        });

    },

    //检查登录
    CheckLogin: function () {
        $.ajax({
            url: $("#CheckLogin").val(),
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data < 0) {
                    window.alert("登录之后才能发表咨询！");
                    window.location.href = $("#LoginUrl").val() + "?ReturnUrl=" + $("#ReturnUrl").val();
                }
            }
        });
    },

    //加入购物车
    addProductToCart: function () {
        window.location.href = $.format("{0}?Category=Product&SysNo={1}&Qty={2}",
            $("#ShoppingCartUrl").val(),
            $("#ProductSysNo").val(),
           1);
    },

    //加入收藏
    addToWish: function (obj) {
        $.ajax({
            url: $("#AddToWishListUrl").val(),
            type: 'post',
            dataType: 'json',
            data: { ProductSysNo: $("#ProductSysNo").val() },
            success: function (data) {
                if (!data.error) {
                    $(obj).removeClass("btn_addfavorB").addClass("btn_favoredB").removeAttr("onclick");
                    PopWin('#addwish').fn.popIn();
                }
                else {
                    window.location.href = $("#LoginUrl").val() + "?ReturnUrl=" + $("#ReturnUrl").val();
                }
            }

        })
    },
    wish: function () {
        PopWin('#wish').fn.popIn();
    },
    Login: function () {
        if (confirm("评论需要登录，是否立即登录？")) {
            var LoginUrl = $("#LoginUrl").val();
            var ReturnUrl = $("#ReturnUrl").val();
            location.href = LoginUrl + "?returnurl=" + ReturnUrl;
        };
    }
}