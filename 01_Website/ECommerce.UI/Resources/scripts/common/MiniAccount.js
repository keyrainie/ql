//Mini登录
var MiniLogin = {
    option: null,
    RefreshValidCode: function (e) {
        var url = $(e).attr('ValidCodeUrl');
        newurl = url + "?r=" + Math.random();
        $(e).find("img").attr('src', newurl);
        $(e).parent().find("input").focus();
    },
    Open: function (returnUrl, callback) {

        this.option = {
            returnUrl: returnUrl,
            callback: callback
        };


        $("#BtnLogin").show();
        $("#LoginPanel").hide();
        $("#UserID").val("");
        $("#UserPassword").val("");
        $("#ValidCode").val("");
        $("#wrongmsg").html("").hide();
        if (returnUrl == undefined)
            returnUrl = "";
        $("#MiniLoginReturnUrl").val(returnUrl);
        //MiniLogin.RefreshValidCode($("#ValidCodeAction"));
        var minilogin = PopWin("#mini_loginpanel", {
            animate: true,
            speed: 200,
            callback: function (popWin) {
                if (ie6) {
                    $(popWin).find(".close").css({ backgroundColor: "gray" }).css({ backgroundColor: "transparent" });
                }

                try {
                    DD_belatedPNG.fix('.ie6png2');
                } catch (e) { }

            }
        });
        minilogin.fn.popIn();
    },
    Close: function () {
        var minilogin = PopWin("#mini_loginpanel", {
            callback: function (popWin) {
                if (ie6) {
                    $(popWin).find(".close").css({ backgroundColor: "gray" }).css({ backgroundColor: "transparent" });
                }

                try {
                    DD_belatedPNG.fix('.ie6png2');
                } catch (e) { }

            }
        });
        minilogin.fn.popOut();
    },
    Login: function () {
        var login_name = $("#UserID");
        var login_pwd = $("#UserPassword");
        if ($String.Trim(login_name.val()) == "" || $String.Trim(login_name.val()) == "请输入账户名，邮箱，手机") {
            login_name.addClass("Validform_error");
            $("#wrongmsg").html("请输入登录帐号").show();
            return false;
        }
        else {
            login_name.removeClass("Validform_error");
        }
        if (Biz.Common.Validation.CheckSpecialString($String.Trim(login_name.val()))) {
            login_name.addClass("Validform_error");
            $("#wrongmsg").html("请输入登录帐号").show();
            return false;
        }
        else {
            login_name.removeClass("Validform_error");
        }
        if (login_pwd.val() == "") {
            login_pwd.addClass("Validform_error");
            $("#wrongmsg").html("请输入登录密码").show();
            return false;
        }
        else {
            login_pwd.removeClass("Validform_error");
        }
        $("#wrongmsg").html("").hide();

        $.ajax({
            type: "post",
            url: "/home/ajaxlogin",
            dataType: "json",
            data: { CustomerID: $String.Trim(login_name.val()), Password: login_pwd.val() },
            cache: false,
            beforeSend: function (XMLHttpRequest) {
                $("#BtnLogin").hide();
                $("#LoginPanel").show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data != "s") {
                    $("#wrongmsg").html(data).show();
                    $("#BtnLogin").show();
                    $("#LoginPanel").hide();
                }
                else {
                    if (typeof MiniLogin.option.callback == "function") {
                        MiniLogin.Close();
                        MiniLogin.option.callback();
                        return true;
                    }
                    var returnUrl = $("#MiniLoginReturnUrl").val();
                    if (returnUrl.length == 0) {
                        location.reload();
                    }
                    else {
                        window.location = returnUrl;
                    }
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    }
}

//Mini注册
var MiniRegister = {
    PasswordStrength: {
        Level: ["4", "3", "2", "1"], //太短:1,较弱:2,一般:3,极佳:4
        //强度提示信息
        LevelValue: [15, 10, 5, 0],
        //和上面提示信息对应的强度值
        Factor: [1, 2, 5],
        //强度加权数,分别为字母，数字，其它
        KindFactor: [0, 0, 10, 20],
        //密码含几种组成的权数
        Regex: [/[a-zA-Z]/g, /[0-9]/g, /[^a-zA-Z0-9]/g] //字符，数字，非字符数字（即特殊符号）
    },
    ValidateCustomerID: function () {
        var reg_name = $("#UserIDReg");
        if ($String.Trim(reg_name.val()) == "" || $String.Trim(reg_name.val()) == "请输入账户名，邮箱，手机") {
            $("#wrongmsgReg").html("账户名不能为空！").show();
            return false;
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_name.val()))) {
            $("#wrongmsgReg").html("账户名不能为空！").show();
            return false;
        }
        else {
            $.ajax({
                type: "post",
                url: "/Home/AjaxCheckRegisterID",
                dataType: "json",
                data: { CustomerID: $String.Trim(reg_name.val()) },
                async: false,
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data != "s") {
                        $("#wrongmsgReg").html(data).show();
                    }
                    else {
                        $("#wrongmsgReg").html("").hide();
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
            return true;
        }
    },
    ValidatePassword: function () {
        var password = $("#UserPasswordReg").val();
        if (password == "") {
            $("#wrongmsgReg").html("密码不能为空！").show();
            return false;
        }
        else if (password.length < 6 || password.length > 20) {
            $("#wrongmsgReg").html("请输入6-20个大小写英文字母与数字的混合,可包含符号！").show();
            return false;
        }
        else if (!(password.match(/([a-zA-Z])/) && password.match(/([0-9])/))) {
            $("#wrongmsgReg").html("请输入6-20个大小写英文字母与数字的混合,可包含符号！").show();
            return false;
        }
        return true;
    },
    ValidatePWD: function () {
        var reg_repwd = $("#UserRePasswordReg");
        if (reg_repwd.val() == "") {
            $("#wrongmsgReg").html("确认密码不能为空！").show();
            return false;
        }
        if ($("#UserPasswordReg").val() != reg_repwd.val()) {
            $("#wrongmsgReg").html("确认密码与密码不相同！").show();
            return false;
        }
        return true;
    },
    ValidateEmail: function () {
        var reg_email = $("#UserEmailReg");
        if ($String.Trim(reg_email.val()) == "") {
            $("#wrongmsgReg").html("邮箱不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_email.val()))) {
            $("#wrongmsgReg").html("邮箱不能为空").show();
        }
        else if (!Biz.Common.Validation.isEmail($String.Trim(reg_email.val()))) {
            $("#wrongmsgReg").html("邮箱格式不正确").show();
        }
        else {
            $.ajax({
                type: "post",
                url: "/Home/AjaxCheckRegisterEmail",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CustomerEmail: $String.Trim(reg_email.val()) },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data != "s") {
                        $("#wrongmsgReg").html(data).show();
                    }
                    else {
                        $("#wrongmsgReg").html("").hide();
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
            return true;
        }
    },
    RefreshValidCode: function (e) {
        var url = $(e).attr('ValidCodeUrl');
        newurl = url + "?r=" + Math.random();
        $(e).find("img").attr('src', newurl);
        $(e).parent().find("input").focus();
    },
    Open: function () {
        $("#UserIDReg").val("");
        $("#UserPasswordReg").val("");
        $("#UserRePasswordReg").val("");
        $("#ValidCodeReg").val("");
        $("#wrongmsgReg").html("").hide();
        MiniRegister.RefreshValidCode($("#ValidCodeRegAction"));
        var minireg = PopWin("#mini_regpanel", {
            animate: true,
            speed: 200,
            callback: function (popWin) {
                if (ie6) {
                    $(popWin).find(".close").css({ backgroundColor: "gray" }).css({ backgroundColor: "transparent" });
                }

                try {
                    DD_belatedPNG.fix('.ie6png2');
                } catch (e) { }
            }
        });
        minireg.fn.popIn();
    },
    Close: function () {
        var minireg = PopWin("#mini_regpanel", {
            callback: function (popWin) {
                if (ie6) {
                    $(popWin).find(".close").css({ backgroundColor: "gray" }).css({ backgroundColor: "transparent" });
                }

                try {
                    DD_belatedPNG.fix('.ie6png2');
                } catch (e) { }
            }
        });
        minireg.fn.popOut();
    },
    Register: function () {
        if (!$("#mini_ck_agree").is(":checked")) {
            $("#wrongmsgReg").html("必须阅读并同意服务协议！").show();
            return false;
        }
        var checkResult = MiniRegister.ValidateCustomerID();
        if (!checkResult)
            return false;
        checkResult = MiniRegister.ValidatePassword();
        if (!checkResult)
            return false;
        checkResult = MiniRegister.ValidatePWD();
        if (!checkResult)
            return false;
        checkResult = MiniRegister.ValidateEmail();
        if (!checkResult)
            return false;
        if ($("#ValidCodeReg").val().length == 0) {
            $("#wrongmsgReg").html("必须输入验证码！").show();
            return false;
        }

        if ($("#wrongmsgReg").html().length > 0)
            return false;
        $.ajax({
            type: "post",
            url: "/home/ajaxregister",
            dataType: "json",
            data: { CustomerID: $String.Trim($("#UserIDReg").val()), Password: $("#UserPasswordReg").val(), RePassword: $("#UserRePasswordReg").val(), ValidatedCode: $("#ValidCodeReg").val(), Email: $("#UserEmailReg").val() },
            cache: false,
            beforeSend: function (XMLHttpRequest) {
                $("#BtnReg").hide();
                $("#RegPanel").show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data.ContentType != "s") {
                    $("#wrongmsgReg").html(data.Data).show();
                    $("#BtnReg").show();
                    $("#RegPanel").hide();
                }
                else {
                    //var _mvq = window._mvq || []; window._mvq = _mvq;
                    //_mvq.push(['$setAccount', 'm-78512-0']);
                    //_mvq.push(['$setGeneral', 'registered', '', $String.Trim($("#UserIDReg").val()), data.Data]);
                    //_mvq.push(['$logConversion']);
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    }
}

$(function () {
    //Mini登录回车事件
    $("#UserID").keydown(function (e) {
        if (e.keyCode == 13) {
            MiniLogin.Login();
        }
    });
    $("#UserPassword").keydown(function (e) {
        if (e.keyCode == 13) {
            MiniLogin.Login();
        }
    });
    $("#ValidCode").keydown(function (e) {
        if (e.keyCode == 13) {
            MiniLogin.Login();
        }
    });
    //Mini注册回车事件
    $("#UserIDReg").keydown(function (e) {
        if (e.keyCode == 13) {
            MiniRegister.Register();
        }
    });
    $("#UserPasswordReg").keydown(function (e) {
        if (e.keyCode == 13) {
            MiniRegister.Register();
        }
    });
    $("#UserRePasswordReg").keydown(function (e) {
        if (e.keyCode == 13) {
            MiniRegister.Register();
        }
    });
    $("#ValidCodeReg").keydown(function (e) {
        if (e.keyCode == 13) {
            MiniRegister.Register();
        }
    });

    $("#UserIDReg").blur(function () { MiniRegister.ValidateCustomerID(); });
    $("#UserPasswordReg").blur(function () { MiniRegister.ValidatePassword(); });
    $("#UserRePasswordReg").blur(function () { MiniRegister.ValidatePWD(); });
    $("#UserEmailReg").blur(function () { MiniRegister.ValidateEmail(); });
});