$(function () {
    //$(".login-form input.intxt").focusin(function () {
    //    $(this).addClass("intxtfocus").removeClass("Validform_error");
    //    $("#wrongmsg").hide();
    //}).focusout(function () {
    //    $(this).removeClass("intxtfocus");
    //    if ($(this).val() != "") {
    //        $(this).prev("label").find(".input_tip").hide();
    //    }
    //    else {
    //        $(this).prev("label").find(".input_tip").show();
    //        $(this).addClass("Validform_error");
    //    }
    //});

    $("input.intxt").focusin(function () {
        var me = $(this);
        me.parent(".inputli").addClass("intxtfocusli")
        me.addClass("intxtfocus").removeClass("Validform_error");
        me.parent(".inputli").find(".hover_text").hide();
    }).focusout(function () {
        var me = $(this);
        me.parent(".inputli").removeClass("intxtfocusli")
        me.removeClass("intxtfocus").removeClass("Validform_error");;

        if (me.val().length > 0) {
            me.parent(".inputli").find(".hover_text").hide();
        } else {
            me.parent(".inputli").find(".hover_text").show();
        }
    }).val("").focusout();

    //登录回车事件
    $("#input_login_name").keydown(function (e) {
        if (e.keyCode == 13) {
            Biz.Customer.Login.Submit();
        }
    }).blur(function (e) {
        Biz.Customer.Login.showAuchCode();
    });
    $("#input_login_pwd").keydown(function (e) {
        if (e.keyCode == 13) {
            Biz.Customer.Login.Submit();
        }
    });
    $("div.tab.log-type a").click(function () {
        $("div.tab.log-type a").attr("class", "");
        $(this).attr("class", "now");
    });
});

usingNamespace("Biz.Customer")["Login"] = {
    refreshValidator: function (img, input) {
        var url = $(img).attr('ref1');
        newurl = url + "?r=" + Math.random();
        $(img).attr('src', newurl);
        $(input).focus();
    },
    sendLoginValidSMS: function (obj) {

        var num = 0;
        var login_name = $("#input_login_name");

        login_name.parent(".inputli").removeClass("intxtfocusli")
        login_name.removeClass("intxtfocus");

        if ($String.Trim(login_name.val()) == "" || $String.Trim(login_name.val()) == login_name.attr("tip")) {
            num++;
            login_name.parent(".inputli").addClass("intxtfocusli");
            login_name.addClass("intxtfocus");
            $("#wrongmsg").html("请输入账户名").show();
            return;
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(login_name.val()))) {
            num++;
            login_name.parent(".inputli").addClass("intxtfocusli");
            login_name.addClass("intxtfocus");
            $("#wrongmsg").html("请输入账户名").show();
            return;
        }
        $("#wrongmsg").hide();
        $("#sendsms").hide();
        $.ajax({
            type: "post",
            url: "/Home/SendLoginValidSMS",
            dataType: "json",
            async: false,
            timeout: 30000,
            data: { CustomerID: $String.Trim(login_name.val()) },
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data) {
                if (data == 's') {

                    var second = 59;
                    var timer = window.setInterval(function () {
                        if (second > 0) {
                            $(".tipReget").html("<span>(" + (second--) + ")秒后重新获取</span>");
                        } else {
                            clearInterval(timer);
                            $("#sendsms").show();
                            $(".tipReget").hide();
                        }
                    }, 1000);
                    $(".tipReget").html("<span></span>").show();
                } else {
                    $("#wrongmsg").html(data).show();
                    $("#sendsms").show();
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    },
    showAuchCode: function () {
        var showAuchCodeUrl = "/Home/AjaxCheckShowAuthCode";
        $.ajax({
            type: "POST",
            url: showAuchCodeUrl + "?r=" + Math.random(),
            data: { customerID: $("#input_login_name").val() },
            dataType: 'json',
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (result) {
                if (result && !result.error) {
                    if (result.verifycode) {
                        Biz.Customer.Login.refreshValidator('#imgValidator', '#ValidatedCode');
                        $(".otherlogin").parent().removeClass("mt40").end().css("margin-top", "-12px");
                        $("#validcodeli").show();
                    } else {
                        $(".otherlogin").parent().addClass("mt40").end().css("margin-top", "0");
                        $("#validcodeli").hide();
                    }
                }
            }
        });
    },

    Submit: function () {
        var num = 0;
        var login_name = $("#input_login_name");

        login_name.parent(".inputli").removeClass("intxtfocusli")
        login_name.removeClass("intxtfocus");


        if ($String.Trim(login_name.val()) == "" || $String.Trim(login_name.val()) == login_name.attr("tip")) {
            num++;
            login_name.parent(".inputli").addClass("intxtfocusli");
            login_name.addClass("intxtfocus");
            $("#wrongmsg").html("请输入账户名").show();
            return;
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(login_name.val()))) {
            num++;
            login_name.parent(".inputli").addClass("intxtfocusli");
            login_name.addClass("intxtfocus");
            $("#wrongmsg").html("请输入账户名").show();
            return;
        }

        var login_pwd = $("#input_login_pwd");

        login_pwd.parent(".inputli").removeClass("intxtfocusli")
        login_pwd.removeClass("intxtfocus");

        var login_verifycode = $("#input_login_validecode");
        if (!login_verifycode.is(":hidden")) {
            if ($String.Trim(login_verifycode.val()) == "") {
                $("#wrongmsg").html("请输入验证码").show();
                return;
            }
        }

        if (login_pwd.val() == "") {
            num++;
            login_pwd.parent(".inputli").addClass("intxtfocusli");
            login_pwd.addClass("intxtfocus");
            if (num > 0)
                $("#wrongmsg").html("请输入密码").show();
        }

        var loginSourceType = $("div.tab.log-type a.now").data("source-type");

        if (num == 0) {
            $.ajax({
                type: "post",
                url: "/Home/AjaxLogin",
                dataType: "json",
                data: { CustomerID: $String.Trim(login_name.val()), Password: login_pwd.val(), ValidatedCode: login_verifycode.val(),SourceType:loginSourceType },
                async: false,
                timeout: 30000,
                beforeSend: function (XMLHttpRequest) {
                    $("#wrongmsg").html("").hide();
                    $("#PageLoginBtn").hide();
                    $("#PageLoginPanel").show();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data.error) return;
                    if (data.type != "s") {
                        if (data.verifycode == "y") {
                            Biz.Customer.Login.refreshValidator('#imgValidator', '#ValidatedCode');
                            $(".otherlogin").parent().removeClass("mt40").end().css("margin-top", "-12px");
                            $("#validcodeli").show();
                        } else {
                            $(".otherlogin").parent().addClass("mt40").end().css("margin-top", "0");
                            $("#validcodeli").hide();
                        }
                        $("#PageLoginBtn").show();
                        $("#PageLoginPanel").hide();
                        $("#wrongmsg").html(data.message).show();
                    } else {
                        if (ReturnUrl.toUpperCase().indexOf("FINDPASSWORD") > 0) {
                            var arr = ReturnUrl.split("/");
                            window.location = arr[0] + "//" + arr[1] + arr[2];
                        } else {
                            window.location = ReturnUrl;
                        }
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    }
}
