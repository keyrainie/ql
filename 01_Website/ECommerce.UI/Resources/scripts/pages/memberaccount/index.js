var verifyDate = 0;

var CustomerIndex = {
    loaded: function () {
        $('a[tag="trackertag"]').hover(function () { $(this).next().show(); }, function () { $(this).next().hide(); });
        $('.openr').hover(function () { $(this).show(); }, function () { $(this).hide(); });

    },

    loadNoReviewOrderProducts: function (pageIndex) {
        $.ajax({
            type: "post",
            url: "/MemberAccount/ucnorevieworderproducts",
            dataType: "html",
            async: false,
            timeout: 30000,
            data: { pageIndex: pageIndex, pageSize: 5 },
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data) {
                $("#noreview_order").html(data);
            },
            complete: function (XMLHttpRequest, textStatus) { }
        })
    }
}

$(document).ready(function () {
    UI.Xslider(".combolist");
    $(".menulist.ie6png").hide();
    $(".Validform_checktip").hide();
    CustomerIndex.loaded();

    $(".centerPopBody input.intxt").focusin(function () {
        $(this).parent().find(".Validform_wrong").hide();
    }).focusout(function () {
        if ($(this).val() == "") {
            $(this).parent().find(".Validform_wrong").show();
        }
    });
    $(".tracker a").each(function () {
        $(this).mouseleave(function () {
            $(this).find(".openr").hide();
        }).mouseenter(function () {
            $(this).find(".openr").show();
            $.ajax({
                type: "post",
                url: "/MemberAccount/QuerySOLogBySOSysNo",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { SOSysNo: $(this).attr("sosysno") },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) { },
                success: function (data) {
                    var sologs = "";
                    $.each(data, function (i, n) {
                        sologs += "<p><span class='gray'>" + n.OptTimeString + "</span>  " + n.Note + "</p>";
                    });
                    $("#loadsolog").replaceWith(sologs);
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        });
    });
});

var CustomerVerify = {
    clearValidformWrong: function () {
        $(".Validform_wrong").hide();
    },
    refreshValidator: function (img, input) {
        var url = $(img).attr('ref1');
        newurl = url + "?r=" + Math.random();
        $(img).attr('src', newurl);
        //$(input).focus();
    },
    AcquireVerifyCode: function (obj) {
        var type = 0;
        if (!$(obj).hasClass("btn_orange24")) return;
        //验证手机号是否为空
        var phoneText = $.trim($("#myphone").val());
        if (phoneText == "") {
            $("#PhoneError").show()
            $("#PhoneError").text("请输入手机号码");
            $("#SMSInfoError").hide();
            $("#smsTime").hide();
            return;
        }
        //验证手机号是否合法
        //if (phoneText != CustomerVerifyVariation.telStr) {
        type = 1;
        var reg = /^1\d{10}$/;
        if (phoneText.length < 11 || phoneText.length > 11 || !reg.test(phoneText)) {
            $("#PhoneError").show()
            $("#PhoneError").text("号码输入不正确，请重新输入");
            $("#SMSInfoError").hide();
            $("#smsTime").hide();
            return;
        }
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
            url: "/MemberAccount/AjaxSendValidateCellphoneByCode",
            dataType: "json",
            async: false,
            timeout: 30000,
            data: { CellPhoneNumber: phoneText },
            beforeSend: function (XMLHttpRequest) {
                $("#sendSMSbtn").hide();
                $("#smsTime").show();
                $("#SMSInfoError").show();
                var second = 59;
                var timer = window.setInterval(function () {
                    if (second > 0) {
                        $(".tipReget").html("<span>(" + (second--) + "秒后) 重新获取验证码</span>");
                        $(".tipReget").show();
                    } else {
                        clearInterval(timer);
                        $(".tipReget").html("");
                        $(".tipReget").hide();
                        $("#sendSMSbtn").show();
                        $("#smsTime").hide();
                        $("#SMSInfoError").hide();
                        $("#sendSMSText").text("获取验证码");
                    }
                }, 1000);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data == 's') {
                } else {
                    alert(data);
                    $("#sendSMSText").text("获取验证码");
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    },
    ValidateCellPhone: function () {
        var code = $("#ValidatedCode");
        var smsCode = $("#smsCode");
        if (smsCode.val() == '') {
            smsCode.parent().find(".Validform_checktip").html("请输入短信校验码").show();
        }
        else if (code.val() == '') {
            code.parent().find(".Validform_checktip").html("请输入验证码").show();
        }
        else {
            var phoneText = $.trim($("#myphone").val());
            $("#submitcellloading").show();
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxValidateCellphoneByCode",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CellPhoneNumber: phoneText, ValidatedCode: code.val(), SmsCode: smsCode.val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data == 's') {
                        PopWin('#checkphone_suc').fn.popIn();
                        PopWin('#checkphone').fn.popOut();
                        $("#checkphone_suc strong").html(phoneText);
                    } else {
                        $("#ValidatedCode").parent().find(".Validform_wrong").html(data).show();
                        $("#submitcellloading").hide();
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    },
    ValidatorEmail: function () {
        var email = $("#input_Email");
        var code = $("#emailValidatorCode");
        //验证邮箱格式是否正确
        var reg = /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/;
        if (email.val == "" || !reg.test(email.val())) {
            email.parent().find(".Validform_checktip").html("请输入正确的邮箱地址").show();
        }
        else if (code.val() == '') {
            code.parent().find(".Validform_checktip").html("请输入验证码").show();
        }
        else {
            $("#submitemailloading").show();
            var emailText = $.trim(email.val());
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxValidateCustomerEmail",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { Email: emailText, ValidatedCode: code.val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data == 's') {
                        $("#submitemailloading").hide();
                        PopWin('#checkemail_suc').fn.popIn();
                        PopWin('#checkemail').fn.popOut();
                        //window.setInterval(function () { window.location = window.location; }, 2000);
                    } else {
                        $("#emailValidatorCode").parent().find(".Validform_wrong").html(data).show();
                        $("#submitemailloading").hide();
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
    },
    RefreshValidateEmail: function () {
        PopWin('#checkemail_suc').fn.popOut();
    },
    RefreshValidateCell: function () {
        PopWin('#checkphone_suc').fn.popOut();
        $("#phoneval").html("<p class=\"phone\">手机已验证</p>");
        location.reload();
    },
    Verify: function () {
        if ($("#valicode").val() == null) {
            $("#CodeVerify").css("display", "block");
            $("#EmailCode").text(CustomerVerifyVariation.CustomerVeridy_Phone18);
        }
    },
    dynamicMessage: function (timeSecond) {
        var showTimmer;
        if (showTimmer) {
            clearTimeout(showTimmer);
        }
        showTimmer = setTimeout(function () {
            var messageRefresh = "(" + timeSecond + "秒后) 重新获取短信校验码";
            $("#linkText").text(messageRefresh);
            timeSecond--;
            if (timeSecond < 0) {
                $("#sendSMSbtn").show();
                $("#smsTime").hide();
                clearTimeout(showTimmer);
                $("#linkText").text("重新获取短信校验码");
            } else {
                $("#sendSMSbtn").hide();
                $("#smsTime").show();
                CustomerVerify.dynamicMessage(timeSecond);
            }
        }, 1000);
    },
    CloseWindow: function (id, type) {
        //关闭当前窗口、调用父窗口元素
        with ($("#" + id, window.parent.document)) {
            $("#popBack", window.parent.document).css("display", "none");
            $("#overlay", window.parent.document).css("display", "none");
            attr("showed", "0");
            if (type == "email") {
                css("display", "none");
            }
            else if (type == "phone") {
                //手机验证通过进行特殊处理（拿回验证后的手机号码）
                css("display", "none");
                var strData = "{\"CellPhone\":\"" + $("#myphone").val() + "\",\"ConfirmKey\":\"\",\"Process\":0}";
                var phoneValidation = { PhoneValidation: strData, IsCancel: "" };
                $.newegg.ajaxPost(
                    "AcquirePhone", $.newegg.buildCurrent("Ajax/Customer/AjaxCellPhoneValidation.aspx"), phoneValidation,
                     function (result) {
                         var resultMessage = $(result);
                         var info = resultMessage.filter("#AccountCenter").val();
                         if (info != "isPhone") {
                             var strInfo = $.newegg.format(CustomerVerifyVariation.PassPhone, info);
                             $("#Aspan", window.parent.document).html("<span class=\"phone\">" + strInfo + "</span>");
                         }
                         else {
                             return;
                         }
                     }
                );
            }
        }
    }
};

//处理键盘事件  
function doKey(e) {
    var ev = e || window.event;//获取event对象  
    var obj = ev.target || ev.srcElement;//获取事件源  
    var t = obj.type || obj.getAttribute('type');//获取事件源类型  
    if (ev.keyCode == 8 && t != "password" && t != "text" && t != "textarea") {
        return false;
    }
}
//禁止后退键 作用于Firefox、Opera  
document.onkeypress = doKey;
//禁止后退键  作用于IE、Chrome  
document.onkeydown = doKey;