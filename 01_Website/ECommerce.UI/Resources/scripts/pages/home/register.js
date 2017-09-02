$(function () {
    $(".error").hide();
    //$(".reg-form input.intxt").focusin(function () {
    //    $(this).addClass("intxtfocus").removeClass("Validform_error");
    //    $(this).parent().find(".Validform_checktip").hide();
    //}).focusout(function () {
    //    $(this).removeClass("intxtfocus");
    //    if ($(this).val() != "") {
    //        $(this).prev("label").find(".input_tip").hide();
    //    }
    //    else {
    //        $(this).prev("label").find(".input_tip").show();
    //        $(this).addClass("Validform_error");
    //        $(this).parent().find(".Validform_checktip").show();
    //    }
    //});

    $("input.intxt").focusin(function () {
        var me = $(this);
        me.parent(".inputli").addClass("intxtfocusli")
        me.addClass("intxtfocus");
        me.parent(".inputli").find(".hover_text").hide();
    }).focusout(function () {
        $(".error").hide();
        var me = $(this);
        me.parent(".inputli").removeClass("intxtfocusli")
        me.removeClass("intxtfocus");

        if (me.val().length > 0) {
            me.parent(".inputli").find(".hover_text").hide();
        } else {
            me.parent(".inputli").find(".hover_text").show();
        }
    }).val("").focusout();

    $("#input_reg_name").blur(function () { Biz.Customer.Register.validateCustomerID(); });
    $("#input_reg_pwd").blur(function () { Biz.Customer.Register.validatePassword(); });
    $("#input_reg_repwd").blur(function () { Biz.Customer.Register.validatePWD(); });
    $("#intxt_reg_email").blur(function () { Biz.Customer.Register.validateCustomerEmail(); });
    $("#myphone").blur(function () { Biz.Customer.Register.ValidatePhoneNumber(); });
    $("#input_reg_validecode").blur(function () { Biz.Customer.Register.ValidatePhoneNumber(); });
});


usingNamespace("Biz.Customer")["Register"] = {
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
    refreshValidator: function (img, input) {
        var url = $(img).attr('ref1');
        newurl = url + "?r=" + Math.random();
        $(img).attr('src', newurl);
        $(input).focus();
    },
    validateCustomerID: function () {
        var reg_name = $("#input_reg_name");
        if ($String.Trim(reg_name.val()) == "" || $String.Trim(reg_name.val()) == "请输入帐号") {
            reg_name.parent().find(".error").html("账户名不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_name.val()))) {
            reg_name.parent().find(".error").html("账户名不能为空").show();
        }
        else {
            $.ajax({
                type: "post",
                url: "/Home/AjaxCheckRegisterID",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CustomerID: $String.Trim(reg_name.val()) },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data != "s") {
                        $("#input_reg_name").parent().find(".error").html(data).show();
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    },
    validateCustomerEmail: function () {
        var reg_email = $("#intxt_reg_email");
        if ($String.Trim(reg_email.val()) == "" || $String.Trim(reg_email.val()) == "请输入邮箱") {
            reg_email.parent().find(".error").html("邮箱不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_email.val()))) {
            reg_email.parent().find(".error").html("邮箱不能为空").show();
        }
        else if (!Biz.Common.Validation.isEmail($String.Trim(reg_email.val()))) {
            reg_email.parent().find(".error").html("邮箱格式不正确").show();
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
                        $("#intxt_reg_email").parent().find(".error").html(data).show();
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    },
    validatePassword: function () {
        var password = $("#input_reg_pwd").val();
        if (password == "") {
            $("#input_reg_pwd").parent().find(".error").html("密码不能为空").show();
        }
        else if (password.length < 6 || password.length > 20) {
            $("#input_reg_pwd").parent().find(".error").html("请输入6-20个大小写英文字母与数字的混合,可包含符号").show();
        }
        else if (!(password.match(/([a-zA-Z])/) && password.match(/([0-9])/))) {
            $("#input_reg_pwd").parent().find(".error").html("请输入6-20个大小写英文字母与数字的混合,可包含符号").show();
        }
    },
    validatePWD: function () {
        var reg_repwd = $("#input_reg_repwd");
        if (reg_repwd.val() == "") {
            reg_repwd.parent().find(".error").html("密码不能为空").show();
        }
        if ($("#input_reg_pwd").val() != reg_repwd.val()) {
            reg_repwd.parent().find(".error").html("两次输入密码不一致").show();
        }
    },

    ValidatePhoneNumber: function () {
        var reg_Phone = $("#myphone");
        var reg = /^1\d{10}$/;

        if ($String.Trim(reg_Phone.val()) == "" || $String.Trim(reg_Phone.val()) == "您常用的电话号码") {
            reg_Phone.parent().find(".error").html("手机号码不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_Phone.val()))) {
            reg_Phone.parent().find(".error").html("手机号码不能为空").show();
        }
        else if (reg_Phone.val().length < 11 || reg_Phone.val().length > 11 || !reg.test(reg_Phone.val())) {
            reg_Phone.parent().find(".error").html("手机号码不正确").show();
        }
        else {
            $.ajax({
                type: "post",
                url: "/Home/AjaxCheckRegisterPhoneNumber",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CellPhoneNumber: $String.Trim(reg_Phone.val()) },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data != "s") {
                        $("#myphone").parent().find(".error").html(data).show();
                        $("#sendSMSbtn").hide();
                        $("#sendSMSbtn_Gray").show();
                    }
                    else {
                        var validateCode = $.trim($("#input_reg_validecode").val());
                        if (validateCode.length == 0) {
                            $("#input_reg_validecode").parent().find(".error").html("请输入验证码").show();
                            $("#sendSMSbtn").hide();
                            $("#sendSMSbtn_Gray").show();
                        }
                        else
                        {
                            $.ajax({
                                type: "post",
                                url: "/Home/AjaxCheckRegisterimgNumber",
                                dataType: "json",
                                async: false,
                                timeout: 30000,
                                data: { ImgValidatedCode: $.trim($("#input_reg_validecode").val()) },
                                beforeSend: function (XMLHttpRequest) { },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                },
                                success: function (data) {
                                    if (data != "s") {
                                        $("#input_reg_validecode").parent().find(".error").html("验证码错误").show();
                                        $("#sendSMSbtn").hide();
                                        $("#sendSMSbtn_Gray").show();
                                        Biz.Customer.Register.refreshValidator('#imgValidator', '#ValidatedCode');
                                    }
                                    else
                                    {
                                        $("#sendSMSbtn").show();
                                        $("#sendSMSbtn_Gray").hide();
                                    }
                                }
                            });

                        }

                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    },

    Submit: function () {
        $(".error").hide();
        var num = 0;
        if (!$("#ck1").is(":checked")) {
            num++;
            $("#ck1").parent().find(".error").show();
        }


        var reg_name = $("#input_reg_name");

        if ($String.Trim(reg_name.val()) == "" || $String.Trim(reg_name.val()) == reg_name.attr("tip")) {
            num++;
            reg_name.parent().find(".error").html("账户名不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_name.val()))) {
            num++;
            reg_name.parent().find(".error").html("账户名不能为空").show();
        }
        else {
            $.ajax({
                type: "post",
                url: "/Home/AjaxCheckRegisterID",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CustomerID: $String.Trim(reg_name.val()) },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data != "s") {
                        num++;
                        if (data == "验证码不正确") {
                            $("#input_reg_validecode").parent().find(".error").html(data).show();
                        }
                        else {
                            $("#input_reg_name").parent().find(".error").html(data).show();
                        }
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
        //验证短信
        $("#phoneCodeError").hide();
        var smsCode = $("#input_reg_phonevalidecode");
        if (smsCode.val() == '') {
            num++;
            smsCode.parent().find(".Validform_checktip").html("请输入短信校验码").show();
        }
        else {
            var phoneText = $.trim($("#myphone").val());
            $.ajax({
                type: "post",
                url: "/LoginRegister/AjaxValidateCellphoneByCode",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CellPhoneNumber: phoneText, SmsCode: smsCode.val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data == 's') {
                    } else {
                        num++;
                        $("#phoneCodeError").show();
                        //$(".tipReget").html("");
                        //$(".tipReget").hide();
                        //$("#sendSMSbtn").show();
                        //$("#smsTime").hide();
                        //$("#SMSInfoError").hide();
                        //$("#sendSMSText").text("获取验证码");
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });

        }
        //var reg_email = $("#intxt_reg_email");
        //if ($String.Trim(reg_email.val()) == "" || $String.Trim(reg_email.val()) == "请输入邮箱") {
        //    num++;
        //    reg_email.parent().find(".error").html("邮箱不能为空").show();
        //}
        //else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_email.val()))) {
        //    num++;
        //    reg_email.parent().find(".error").html("邮箱不能为空").show();
        //}
        //else if (!Biz.Common.Validation.isEmail($String.Trim(reg_email.val()))) {
        //    reg_email.parent().find(".error").html("邮箱格式不正确").show();
        //    num++;
        //}
        //else {
        //    $.ajax({
        //        type: "post",
        //        url: "/Home/AjaxCheckRegisterEmail",
        //        dataType: "json",
        //        async: false,
        //        timeout: 30000,
        //        data: { CustomerEmail: $String.Trim(reg_email.val()) },
        //        beforeSend: function (XMLHttpRequest) { },
        //        error: function (XMLHttpRequest, textStatus, errorThrown) {
        //        },
        //        success: function (data) {
        //            if (data != "s") {
        //                num++;
        //                $("#intxt_reg_email").parent().find(".error").html(data).show();
        //            }
        //        },
        //        complete: function (XMLHttpRequest, textStatus) { }
        //    });
        //}

        if ($("#input_reg_pwd").val() == "") {
            num++;
            $("#input_reg_pwd").parent().find(".error").html("密码不能为空").show();
        }
        else if ($("#input_reg_pwd").val().length < 6 || $("#input_reg_pwd").val().length > 20) {
            num++;
            $("#input_reg_pwd").parent().find(".error").html("请输入6-20个大小写英文字母与数字的混合,可包含符号").show();
        }
        else if (!($("#input_reg_pwd").val().match(/([a-zA-Z])/) && $("#input_reg_pwd").val().match(/([0-9])/))) {
            num++;
            $("#input_reg_pwd").parent().find(".error").html("请输入6-20个大小写英文字母与数字的混合,可包含符号").show();
        }

        if ($("#input_reg_repwd").val() == "") {
            num++;
            $("#input_reg_repwd").parent().find(".error").html("确认密码不能为空").show();
        }
        if ($("#input_reg_pwd").val() != $("#input_reg_repwd").val()) {
            num++;
            $("#input_reg_repwd").parent().find(".error").html("两次输入密码不一致").show();
        }
        //if ($("#input_reg_validecode").val() == "") {
        //    num++;
        //    $("#input_reg_validecode").parent().find(".error").show();
        //}
        if (num == 0) {
            $.ajax({
                type: "post",
                url: "/Home/AjaxRegister",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: {
                    CustomerID: $String.Trim(reg_name.val()),
                    Password: $("#input_reg_pwd").val(),
                    RePassword: $("#input_reg_repwd").val(),
                    Email: $("#intxt_reg_email").val(),
                    ValidatedCode: $("#input_reg_validecode").val(),
                    CellPhoneCode: $("#cellphoneSysNo").attr("data-sysno"),
                    CellPhone: $("#myphone").val()
                },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data.ContentType != "s") {
                        var num = 0;
                        if (data.ContentType == "y") {
                            $("#input_reg_validecode").parent().find(".error").html(data.Data).show();
                        }
                        else {
                            $("#input_reg_name").parent().find(".error").html(data.Data).show();
                        }
                    }
                    else {
                        //var _mvq = window._mvq || []; window._mvq = _mvq;
                        //_mvq.push(['$setAccount', 'm-78512-0']);
                        //_mvq.push(['$setGeneral', 'registered', '', $String.Trim(reg_name.val()), data.Data]);
                        //_mvq.push(['$logConversion']);
                        window.location = "/Welcome";
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    }
}

var CustomerVerify = {
    AcquireVerifyCode: function (obj) {
        var type = 0;
        $("#phoneCodeError").hide();
        if (!$(obj).hasClass("btngetcode")) return;
        //验证手机号是否为空
        var phoneText = $.trim($("#myphone").val());
        var validateCode = $.trim($("#input_reg_validecode").val());
        //if (phoneText == "") {
        //    $("#PhoneError").show()
        //    $("#PhoneError").text("请输入手机号码");
        //    $("#SMSInfoError").hide();
        //    $("#smsTime").hide();
        //    return;
        //}
        //验证手机号是否合法
        //if (phoneText != CustomerVerifyVariation.telStr) {
        //type = 1;
        //var reg = /^1\d{10}$/;
        //if (phoneText.length < 11 || phoneText.length > 11 || !reg.test(phoneText)) {

        //    //$("#PhoneError").show()
        //    $("#PhoneError").html("号码输入不正确，请重新输入").show();
        //    $("#SMSInfoError").hide();
        //    $("#smsTime").hide();
        //    return;
        //}
        $("#PhoneError").hide(); //隐藏错误提示
        if (!CustomerVerify.checkRefreshGet(60)) {
            $("#PhoneError").text("60秒内不能重复获取，请稍后再获取");
            $("#SMSInfoError").hide();
            $("#smsTime").hide();
            return;
        }

        //}

        $("#sendSMSText").text("处理中...");

        //var strData = "{\"CellPhone\":\"" + phoneText + "\",\"ConfirmKey\":\"\",\"Process\":1}";
        //var phoneValidation = { PhoneValidation: strData, IsCancel: "", type: type };
        //向服务器请求发送验证码到手机
        $.ajax({
            type: "post",
            url: "/LoginRegister/AjaxSendValidateCellphoneByCode",
            dataType: "json",
            async: false,
            timeout: 30000,
            data: { CellPhoneNumber: phoneText, ValidateCode: validateCode },
            beforeSend: function (XMLHttpRequest) {
                $("#sendSMSbtn").hide();
                $("#smsTime").show();
                $("#SMSInfoError").show();
                var second = 59;
                var timer = window.setInterval(function () {
                    if (second > 0) {
                        $(".tipReget").html("<span>(" + (second--) + "秒后)重新获取验证码</span>");
                        $(".tipReget").show();
                    } else {
                        clearInterval(timer);
                        $(".tipReget").html("");
                        $(".tipReget").hide();
                        $("#sendSMSbtn").show();
                        $("#smsTime").hide();
                        $("#SMSInfoError").hide();
                        $("#sendSMSText").text("获取验证码");
                        $("#input_reg_validecode").parent().find(".error").html("请重新输入验证码").show();
                        $("#sendSMSbtn").hide();
                        $("#sendSMSbtn_Gray").show();
                        Biz.Customer.Register.refreshValidator('#imgValidator', '#ValidatedCode');

                    }
                }, 1000);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data.ContentType == 's') {
                    $("#cellphoneSysNo").attr("data-sysno", data.Data);
                } else {
                    $(".tipReget").html("");
                    $(".tipReget").hide();
                    $("#sendSMSbtn").show();
                    $("#smsTime").hide();
                    $("#SMSInfoError").hide();
                    $("#sendSMSText").text("获取验证码");
                    alert(data.ContentType);
                    $("#sendSMSbtn").hide();
                    $("#sendSMSbtn_Gray").show();
                    Biz.Customer.Register.refreshValidator('#imgValidator', '#ValidatedCode');
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    },
    ValidateCellPhone: function () {
        //var numb = 0;
        $("#phoneCodeError").hide();
        var smsCode = $("#input_reg_phonevalidecode");
        if (smsCode.val() == '') {
            smsCode.parent().find(".Validform_checktip").html("请输入短信校验码").show();

        }
        else {
            var phoneText = $.trim($("#myphone").val());
            $.ajax({
                type: "post",
                url: "/LoginRegister/AjaxValidateCellphoneByCode",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CellPhoneNumber: phoneText, SmsCode: smsCode.val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data == 's') {
                        return 0;
                    } else {
                        $("#phoneCodeError").show();
                        return 1;
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });

        }
    },
    checkRefreshGet: function (timeLimit) {
        var nowTime = new Date();
        var nowMinitePoint = nowTime.getHours() * 3600 + nowTime.getMinutes() * 60 + nowTime.getSeconds();
        if (nowMinitePoint - CustomerVerify.verifyDate < timeLimit) {
            return false;
        } else {
            return true;
        }
    }
};

