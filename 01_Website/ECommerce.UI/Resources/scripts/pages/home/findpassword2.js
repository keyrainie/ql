$(function () {
    $(".error").hide();
    $("input.intxt").focusin(function () {
        var me = $(this);
        me.parent(".inputli").addClass("intxtfocusli")
        me.addClass("intxtfocus");
        me.parent(".inputli").find(".hover_text").hide();
    }).focusout(function () {
        var me = $(this);
        me.parent(".inputli").removeClass("intxtfocusli")
        me.removeClass("intxtfocus");

        if (me.val().length > 0) {
            me.parent(".inputli").find(".hover_text").hide();
        } else {
            me.parent(".inputli").find(".hover_text").show();
        }
    });
});
usingNamespace("Biz.Customer")["FindPassword"] = {
    refreshValidator: function (img, input) {
        var url = $(img).attr('ref1');
        newurl = url + "?r=" + Math.random();
        $(img).attr('src', newurl);
        $(input).focus();
    },
    CheckStepOne: function () {
        $(".error").hide();
        var num = 0;
        var account_name = $("#input_account_name");
        account_name.parent().find(".error").hide();
        if ($String.Trim(account_name.val()) == "") {
            num++;
            account_name.parent().find(".error").html('请输入账户名').show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(account_name.val()))) {
            num++;
            account_name.parent().find(".error").html('请输入正确的账户名').show();
        }

        var input_validcode = $("#input_validcode");
        if (input_validcode.val() == "") {
            num++;
            input_validcode.parent().find(".error").html('请输入验证码').show();
        }
        if (num == 0) {
            if (true) {//通过邮箱找回
                $.ajax({
                    type: "post",
                    url: "/Home/AjaxCheckFindPasswordForCustomer",
                    dataType: "json",
                    async: false,
                    timeout: 30000,
                    data: { CustomerID: $String.Trim(account_name.val()), ValidatedCode: input_validcode.val() },
                    beforeSend: function (XMLHttpRequest) { },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                    },
                    success: function (data) {
                        if (data.indexOf("@") > 0) {
                            $("#step1").hide();
                            $("#step2").show();
                            $(".findsuc strong").html(data);
                            $("div.step_1").removeClass("step_1").addClass("step_2");
                            $(".findsuc a").attr("href", "http://www." + data.substring(data.indexOf("@") + 1));
                        }
                        else if (data == '验证码不正确') {
                            $("#input_validcode").parent().find(".error").html(data).show();
                        } else {
                            alert(data);
                        }
                    },
                    complete: function (XMLHttpRequest, textStatus) { }
                });
            }
            else {//通过手机短信找回 
                $.ajax({
                    type: "post",
                    url: "/Home/AjaxFindPasswordByPhone",
                    dataType: "json",
                    async: false,
                    timeout: 30000,
                    data: { CustomerID: $String.Trim(account_name.val()), ValidatedCode: input_validcode.val() },
                    beforeSend: function (XMLHttpRequest) { },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                    },
                    success: function (data) {
                        if (Biz.Common.Validation.isMobile(data)) {
                            $("#step11").hide();
                            $("#step12").show();
                            $("#step122").show();
                            $("#step121").hide();
                            var str = data.substring(3, 8);
                            $("#input_cellphone").val(data.replace(str, "*****"));
                            $("div.step_1").removeClass("step_1").addClass("step_2");
                        }
                        else if (data == '验证码不正确') {
                            $("#input_validcode").parent().find(".error").html(data).show();
                        } else {
                            $("#step11").hide();
                            $("#step12").show();
                        }
                    },
                    complete: function (XMLHttpRequest, textStatus) { }
                });
            }
        }
    },
    SendFindPasswordSMS: function () {
        $("#sendsms").hide();
        $.ajax({
            type: "post",
            url: "/Home/SendFindPasswordSMS",
            dataType: "json",
            async: false,
            timeout: 30000,
            //data: { CustomerID: $String.Trim(account_name.val()), ValidatedCode: input_validcode.val() },
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data) {
                if (data == 's') {
                    $(".tipReget").html("<span></span>").show();
                    var second = 59;
                    var timer = window.setInterval(function () {
                        if (second > 0) {
                            $(".tipReget").html("<span>(" + (second--) + "秒后) 重新获取验证码</span>");
                        } else {
                            clearInterval(timer);
                            $("#sendsms").show();
                            $(".tipReget").hide();
                        }
                    }, 1000);
                } else {
                    $("#sendsms").show();
                    alert("服务器忙,稍后重试");
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    },
    ReturnStepOne: function () {
        $(".size a").toggleClass("selted");
        $("#step11").show();
        $("#step12").hide();
    },
    CheckStepTwo: function () {
        if ($("#input_smscode").val() != '') {
            $.ajax({
                type: "post",
                url: "/Home/AjaxCheckFindPasswordBySMSCode",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { ValidatedCode: $("#input_smscode").val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data == 's') {
                        window.location = window.location;
                    } else {
                        alert(data);
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    },
    ResetPassword: function () {
        $(".error").hide();
        var num = 0;
        var password = $("#intxt_password");
        var repassword = $("#intxt_password_c");

        if ($.trim(password.val()) == "") {
            password.parent().find(".error").html('密码不能为空').show();
            return;
        } else {
            password.parent().find(".error").hide();
        }

        if (password.val().length < 6 || password.val().length > 20) {
            password.parent().find(".error").html('请输入6-20个大小写英文字母与数字的混合,可包含符号！').show();
            return;
        } else {
            password.parent().find(".error").hide();
        }

        if (!(password.val().match(/([a-zA-Z])/) && password.val().match(/([0-9])/))) {
            password.parent().find(".error").html('请输入6-20个大小写英文字母与数字的混合,可包含符号！').show();
            return;
        } else {
            password.parent().find(".error").hide();
        }

        if (password.val() != repassword.val()) {
            repassword.parent().find(".error").html('两次输入密码不一致').show();
            return;
        } else {
            repassword.parent().find(".error").hide();
        }
        $.ajax({
            type: "post",
            url: "/Home/AjaxResetPassword",
            dataType: "json",
            async: false,
            timeout: 30000,
            data: { Password: $String.Trim(password.val()) },
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data != "s") {                   
                    $("#intxt_password").parent().find(".error").html(data).show();
                }
                else {
                    $("#step22").show();
                    $("#step21").hide();
                    $("#intxt_password").parent().find(".error").hide();
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    }
}
