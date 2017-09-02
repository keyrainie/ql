$(function () {
    var now = new Date();
    $("#sexarea a").each(function () {
        $(this).click(function () {
            $("#selectsex").attr("sysno", $(this).attr("sysno"));
        });
    });
    $("#selectdistrict a").each(function () {
        $(this).click(function () {
            $("#selecteddistrict").attr("sysno", $(this).attr("sysno"));
        });
    });
    var str = "";
    for (var d = 1970; d <= now.getFullYear() ; d++) {
        str += "<li><a href='javascript:void(0);'>" + d + "</a></li>";
    }
    $("#selectyear").html(str);
    str = "";
    for (var d = 1; d <= 31; d++) {
        str += "<li><a href='javascript:void(0);'>" + d + "</a></li>";
    }
    $("#selectday").html(str);
    str = "";
    for (var d = 1; d <= 12; d++) {
        str += "<li><a href='javascript:void(0);'>" + d + "</a></li>";
    }

    InitDateControl();

    $("#selectmonth").html(str).find("a").each(function () {
        $(this).click(function () {
            var dDate = new Date();
            var dPrevDate = new Date($("#selectedyear").html(), $(this).html(), 0);
            var daysInMonth = dPrevDate.getDate();
            var str = "";
            //$("#selectday").empty();
            for (var d = 1; d <= parseInt(daysInMonth) ; d++) {
                str += "<li><a href='javascript:void(0);'>" + d + "</a></li>";
            }
            $("#selectday").html(str);
        });
    });
    $("#selectyear").find("a").each(function () {
        $(this).click(function () {
            var dDate = new Date();
            var dPrevDate = new Date($(this).html(), $("#selectedmonth").html(), 0);
            var daysInMonth = dPrevDate.getDate();
            var str = "";
            //$("#selectday").empty();
            for (var d = 1; d <= parseInt(daysInMonth) ; d++) {
                str += "<li><a href='javascript:void(0);'>" + d + "</a></li>";
            }
            $("#selectday").html(str);
        });
    });

    str = "";
    $.ajax({
        type: "post",
        url: "/Home/GetAllProvince",
        dataType: "json",
        async: false,
        timeout: 30000,
        beforeSend: function (XMLHttpRequest) { },
        error: function (XMLHttpRequest, textStatus, errorThrown) { },
        success: function (data) {
            var json = eval(data);
            var selectVal = jQuery("#Province" + address).attr("selectVal");
            for (i = 0; i < json.length; i++) {
                str += "<li><a href='javascript:void(0);' sysno='" + json[i].SysNo + "'>" + json[i].ProvinceName + "</a></li>";
            };
            $("#selectprovince").html(str).find("a").each(function () {
                $(this).click(function () {
                    $("#selectedprovince").html($(this).html());
                    $("#selectedcity").html("请选择城市");
                    $("#selecteddistrict").html("请选择区县");
                    Biz.AccountCenter.PersonalInfo.ProvinceChange($(this).attr("sysno"));
                });
            });
        },
        complete: function (XMLHttpRequest, textStatus) { }
    });
    if ($("#selectedprovince").attr("sysno") != "0") {
        Biz.AccountCenter.PersonalInfo.ProvinceChange($("#selectedprovince").attr("sysno"));
    }
    if ($("#selectedcity").attr("sysno") != "0") {
        Biz.AccountCenter.PersonalInfo.CityChange($("#selectedcity").attr("sysno"));
    }
});

function InitDateControl() {
    var dDate = new Date();
    var dPrevDate = new Date($("#selectedyear").html(), $("#selectedmonth").html(), 0);
    var daysInMonth = dPrevDate.getDate();
    var str = "";
    for (var d = 1; d <= parseInt(daysInMonth) ; d++) {
        str += "<li><a href='javascript:void(0);'>" + d + "</a></li>";
    }
    $("#selectday").html(str);
}
var timer;
var second = 0;
usingNamespace("Biz.AccountCenter")["PersonalInfo"] = {
    PersonalInfoManager: function () {
        this.CurrentElementContent = '';
        this.CurrentEditAreaInnerHtml = '';

        var this_reference = this;
        this.CancelCallHandler = function () {
            this_reference.InvokeCancelDelegateFunction.call(this_reference);
        }

        this.InvokeCancelDelegateFunction = function () {
            var jqueryObj = this.CurrentElementContent;
            var innerHTML = this.CurrentEditAreaInnerHtml;
            var jqueryObject = $(innerHTML);
            $(jqueryObject[1]).parent().find("#name").val(jqueryObj.data("name"));
            $(jqueryObject[1]).parent().find("#sex").attr("value", jqueryObj.data("sex"));
            $(jqueryObject[1]).parent().find("#birthday").val(jqueryObj.data("birthday"));
            $(jqueryObject[1]).parent().find("#month").attr("value", jqueryObj.data("month"));
            $(jqueryObject[1]).parent().find("#day").attr("value", jqueryObj.data("day"));
            $(jqueryObject[1]).parent().find("#mobile").val(jqueryObj.data("mobile"));
            $(jqueryObject[1]).parent().find("#tel").val(jqueryObj.data("tel"));
            $(jqueryObject[1]).parent().find("#address").val(jqueryObj.data("address"));
            $(jqueryObject[1]).parent().find("#zip").val(jqueryObj.data("zip"));

            if (typeof ($(jqueryObject[1]).parent().find("#idCardType")[0]) != 'undefined') {
                $(jqueryObject[1]).parent().find("#idCardType").attr("value", jqueryObj.data("idCardType"));
                $(jqueryObject[1]).parent().find("#idCardNo").val(jqueryObj.data("idCardNo"));
            }
            if (typeof ($(jqueryObject[1]).parent().find("#email")[0]) == 'undefined') {
                $(jqueryObject[1]).parent().find("#spanEmail").html(jqueryObj.data("spanEmail"));
            }
            else {
                $(jqueryObject[1]).parent().find("#email").val(jqueryObj.data("email"));
            }
            $(jqueryObject[1]).parent().find("#friend").val(jqueryObj.data("friend"));

            $(jqueryObject[1]).parent().find("#subscribe").attr("checked", Boolean(jqueryObj.data("subscribe")));
            $(jqueryObject[1]).parent().find("#areaZone").empty().append(jqueryObj.data("areaZone"));
            $(jqueryObject[1]).parent().find("#region").attr("value", jqueryObj.data("region"));
            $(jqueryObject[1]).parent().find("#city").attr("value", jqueryObj.data("city"));
            $(jqueryObject[1]).parent().find("#area").attr("value", jqueryObj.data("area"));
            jqueryObj.empty().append(jqueryObject);
            var obj = $('#cancelPersonalInfo');
            Biz.Common.Loading.removeLoadingForShowElement(obj);
            obj = $('#btnSavePersonalInfo');
            Biz.Common.Loading.removeLoadingForShowElement(obj);
        }
    },
    validateEmail: function () {
        if ($("#Email").val() != "" && Biz.Common.Validation.isEmail($("#Email").val())) {
            $("#validateEmail").html("正在发送验证邮件...");
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxSendValidateEmail",
                dataType: "json",
                async: false,
                data: { Email: $("#Email").val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data == "s") {
                        var web = $("#Email").val().split('@');
                        $("#validateEmail").replaceWith("<span class='Validform_checktip'>邮件已发送，请前往<a target='_blank' href='http://www." + web[1] + "' class=‘btns btns28 btn-grayC’>" + web[1] + "</a>验证邮箱</span>");
                    }
                    else {
                        $("#validateEmail").html("验证邮箱");
                    }
                }
            });
        }
        else {
            alert("请输入正确的邮箱地址"); $("#Email").addClass("intxtfocus");
        }
    },
    EditEmail: function () {
        $("#showemail").hide();
        $("#emailvalidated").hide();
        $("#editEmail").hide();
        $("#canceledit").show();
        $("#validateEmail").hide();
        $("#Email").show();
    },
    CancelEditEmail: function () {
        $("#showemail").show();
        $("#emailvalidated").show();
        $("#editEmail").show();
        $("#canceledit").hide();
        $("#validateEmail").hide();
        $("#Email").hide();
    },
    CellPhoneValidatIng: function () {
        var code = $("#CellPhoneValidateCode");
        if (code.val() != "") {
            $("#ajaxValidateCellPhone").html("正在验证手机号码...");
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxValidateCellphoneByCode2",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CellPhoneNumber: $("#UnCellPhone").val(), SmsCode: code.val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data == 's') {
                        alert("手机号码验证成功");
                        //window.location.reload();
                        $("#ValidatedPhoneArea").show();
                        $("#UnValidatedPhoneArea").hide();
                        $('#btnValidateCellphone').hide();
                        $('#showcellphonevalidate').hide();
                        $("#CellPhone").removeAttr("readonly");
                        var newPhoneNumber = $("#UnCellPhone").val();
                        $("#CellPhone").val(newPhoneNumber);
                        $("#CellPhone").attr("readonly", "readonly");
                        $("#UnCellPhone").removeAttr("readonly");
                        $(".blue").hide();
                        $("#CellPhoneValidateCode").val("");
                        $("#ajaxValidateCellPhone").html("<span>确定</span>");
                        $("#cellPhoneMsg").html("手机号码验证成功，稍候生效！");
                    } else {
                        $("#ajaxValidateCellPhone").html("<span>确定</span>");
                        alert(data);
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
        else {
            alert("请输入验证码");
        }
    },
    validateCellPhone2: function () {
        window.clearInterval(timer);
        $("#getvalidatecode").hide();
        $.ajax({
            type: "post",
            url: "/MemberAccount/AjaxSendValidateCellphoneByCode",
            dataType: "json",
            async: false,
            timeout: 30000,
            data: { CellPhoneNumber: $("#UnCellPhone").val() },
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data) {
                if (data == 's') {
                    second = 59;
                    $(".tipReget").html("");
                    timer = window.setInterval(function () {
                        if (second > 0) {
                            $(".tipReget").html("<span>(" + (second--) + "秒后) 重新获取验证码</span>");
                            $(".tipReget").show();
                        } else {
                            clearInterval(timer);
                            $("#getvalidatecode").show();
                            $(".tipReget").html("");
                            $(".tipReget").hide();
                        }
                    }, 1000);
                } else {
                    alert(data);
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    },
    
    validateCellPhone: function () {
        window.clearInterval(timer);
        if ($("#UnCellPhone").val() != "" && Biz.Common.Validation.isMobile($("#UnCellPhone").val())) {
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxSendValidateCellphoneByCode",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { CellPhoneNumber: $("#UnCellPhone").val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) { },
                success: function (data) {
                    if (data == 's') {
                        $('#btnValidateCellphone').hide().parent().find(".blue").show();
                        $("#UnCellPhone").attr("readonly", "readonly");
                        $('#showcellphonevalidate').slideDown("fast");
                        second = 59;
                        $(".tipReget").html("");
                        timer = setInterval(function () {
                            if (second > 0) {

                                $(".tipReget").html("<span>(" + (second--) + "秒后) 重新获取验证码</span>");
                                $(".tipReget").show();
                            } else {
                                clearInterval(timer);
                                $("#getvalidatecode").show();
                                $(".tipReget").html("");
                                $(".tipReget").hide();
                            }
                        }, 1000);
                    } else {
                        alert(data);
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
        else {
            alert("请输入正确的手机号码");
        }
    },
    init: function () {
        var name = $('#name');
        var sex = $('#selectsex').attr("sysno");
        var birthday = $('#selectedyear').html() + '-' + $('#selectedmonth').html() + '-' + $('#selectedday').html();
        var mobile = $('#CellPhone');
        var tel = $('#mobile');
        var area = $('#selecteddistrict');
        var address = $('#address');
        var zip = $('#zip');
        var email = $('#Email');

        if ($String.Trim(name.val()) == "") {
            name.addClass("intxtfocus").focus();
            return null;
        }
        if ($String.Trim(name.val()).length > 20) {
            name.addClass("intxtfocus").focus();
            return null;
        }
        if ($('#selectsex').html() == "" || $('#selectsex').html() == "请选择性别") {
            $('#selectsex').addClass("intxtfocus").focus();
            return null;
        }
        if ($('#selectedyear').html() == "" || $('#selectedyear').html() == "请选择年") {
            $('#selectedyear').addClass("intxtfocus").focus();
            return null;
        }
        if ($('#selectedmonth').html() == "" || $('#selectedmonth').html() == "请选择月") {
            $('#selectedmonth').addClass("intxtfocus").focus();
            return null;
        }
        if ($('#selectedday').html() == "" || $('#selectedday').html() == "请选择日") {
            $('#selectedday').addClass("intxtfocus").focus();
            return null;
        }
        if ($String.Trim(mobile.val()) != "") {
            if (!Biz.Common.Validation.isMobile($String.Trim(mobile.val()))) {
                mobile.addClass("intxtfocus").focus();
                return null;
            }
        }
        else {
            mobile.addClass("intxtfocus").focus();
            return null;
        }

        if ($('#selectedprovince').html() == "" || $('#selectedprovince').html() == "请选择省份") {
            $('#selectedprovince').addClass("intxtfocus").focus();
            return null;
        }
        if ($('#selectedcity').html() == "" || $('#selectedcity').html() == "请选择城市") {
            $('#selectedcity').addClass("intxtfocus").focus();
            return null;
        }
        if ($('#selecteddistrict').html() == "" || $('#selecteddistrict').html() == "请选择区县") {
            $('#selecteddistrict').addClass("intxtfocus").focus();
            return null;
        }
        //if ($String.Trim(address.val()) == "") {
        //    address.addClass("intxtfocus").focus();
        //    return null;
        //}
        if ($String.Trim(zip.val()) != "") {
            if (!Biz.Common.Validation.isZip($String.Trim(zip.val()))) {
                zip.addClass("intxtfocus").focus();
                return null;
            }
        }
        if (!Biz.Common.Validation.isEmail($String.Trim(email.val())) && typeof ($('#Email')[0]) != 'undefined') {
            email.addClass("intxtfocus").focus();
            return null;
        }

        var personalInfo = {
            CellPhone: $String.Trim(mobile.val()),
            Phone: $String.Trim(tel.val()),
            DwellAreaSysNo: $('#selecteddistrict').attr("sysno"),
            DwellAddress: $String.Trim(address.val()),
            DwellZip: $String.Trim(zip.val()),
            CustomerName: $String.Trim(name.val()),
            Gender: $('#selectsex').attr("sysno"),
            BirthDay: birthday,
            Email: email.val()
        };
        return personalInfo;
    },
    privateInitCustomer: function () {
        var name = $('#name');
        var sex = $('#sex').val();
        var birthday = $('#birthday').val() + '-' + $('#month').val() + '-' + $('#day').val();
        var mobile = $('#mobile');
        var tel = $('#tel');
        var area = $('#area');
        var address = $('#address');
        var zip = $('#zip');
        var fax = $('#fax');
        var email = $('#email');
        var idCardType = $('#idCardType'); //.val();
        var idCardNo = $('#idCardNo');

        if (typeof (email[0]) == 'undefined') {
            var emailObject = $('#emailValidationArea');
            email = emailObject.find('#spanEmail').html();
        }
        else
            email = email.val();

        var friend = $('#friend').val();
        var subscribe = $('#subscribe')[0].checked;
        var isdefaultaddress = $('#subscribe2')[0].checked ? 1 : 0;
        var customerNickName = $('#')
        var contacts = {
            CellPhone: $String.Trim(mobile.val()),
            Phone: $String.Trim(tel.val()),
            AddressAreaID: area.val(),
            Address: $String.Trim(address.val()),
            Zip: $String.Trim(zip.val()),
            Fax: typeof ($('#fax')[0]) != 'undefined' ? fax.val() : '',
            IDCardType: typeof ($('#idCardNo')[0]) != 'undefined' ? idCardType.val() : '0',
            IDCardNo: typeof ($('#idCardNo')[0]) != 'undefined' ? $String.Trim(idCardNo.val()) : '0'
        };
        var personalInfo = {
            Name: $String.Trim(name.val()),
            Gender: sex,
            BirthDay: birthday,
            Contacts: contacts,
            Email: email,
            RecommenderCode: friend,
            HasSubscribed: subscribe,
            IsDefaultAddress: isdefaultaddress
        };
        return personalInfo;
    },

    post: function (obj, personalInfo, isForAmbassador) {

        var strPersonalInfo = Web.Utils.Json.ToJson(personalInfo);
        var isSuccess = false;

        $.ajax({
            type: "post",
            dataType: "json",
            url: "/MemberAccount/AjaxUpdateCustomerInfo",
            timeout: 30000,
            data: { PersonalInfo: escape(strPersonalInfo) },
            beforeSend: function (XMLHttpRequest) { },
            success: function (data) { //Process result data
                if (data == "s") {
                    alert("操作成功，稍候生效！");
                }
            },
            complete: function (XMLHttpRequest, textStatus) { },
            error: function () { }
        });
    },
    proccessed: function (result, obj, flag, isForAmbassador) {
        if (result.Type == MessageType.Error) {  //var jsonResult = $Json.FromJson(result);
            if (result.Description.length > 0) {
                window.location = result.Description;
            }
            else {
                Biz.Common.Loading.removeLoadingForShowElement($(obj));
                alert($Resource.BuildContent("CheckCustomerIDAndEmail_ReturnResult3"));
            }
        }
        else if (result.Type == MessageType.Warning) {
            if (result.Description != null && result.Description != undefined) {
                Biz.Common.Loading.removeLoadingForShowElement($(obj));
                alert(result.Description);  //$("#summaryMsg").html(result.Description);
            }
            else {
                $(obj).parent().parent().find("#friend").val("");
                Biz.Common.Loading.removeLoadingForShowElement($(obj));
                alert($Resource.BuildContent("AccountCenter_PersonalInfo_RecommendedError"));
            }
        }
        else {
            if (flag == 1) {
                $(obj).parent().parent().find("#friend").attr('disabled', true);
            }
            var deletepersonflag = $(obj).parent().parent().find("#deletepersonflag").val();
            if (deletepersonflag == "1") {
                //$($(obj).parent().parent()[0]).empty().append("<h1>"+$Resource.BuildContent("AccountCenter_PersonalInfo_SaveSuccessfully")+"</h1>");
                //$("#Step2").show();
                $("#btnSavePersonalInfo").hide();
                var redirectUrl = $("#redirectUrl").val();
                if (redirectUrl.length > 0) {
                    window.location = redirectUrl;
                }
            }
            else {
                Biz.Common.Loading.removeLoadingForShowElement($(obj));
                alert($Resource.BuildContent("AccountCenter_PersonalInfo_SaveSuccessfully"));
            }
            //            if ($("#ReturnUrl").val().length > 0) {
            //                window.location.href = $("#ReturnUrl").val()
            //            }
        }
    },

    update: function () {
        var obj = $('#btnSavePersonalInfo')[0];
        var personalInfo = Biz.AccountCenter.PersonalInfo.init();
        if (personalInfo == null) return;
        Biz.AccountCenter.PersonalInfo.post(obj, personalInfo);
    },

    ProvinceChange: function (sysno) {
        var str = "";
        $.ajax({
            type: "post",
            url: "/Home/GetAllCity",
            dataType: "json",
            data: { proviceSysNo: sysno },
            async: false,
            timeout: 30000,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data) {
                var json = eval(data);
                var selectVal = jQuery("#Province" + address).attr("selectVal");
                for (i = 0; i < json.length; i++) {
                    str += "<li><a href='javascript:void(0);' sysno='" + json[i].SysNo + "'>" + json[i].CityName + "</a></li>";
                };
                $("#selectcity").html(str).find("a").each(function () {
                    $(this).click(function () {
                        $("#selectedcity").html($(this).html());
                        $("#selecteddistrict").html("请选择区县");
                        Biz.AccountCenter.PersonalInfo.CityChange($(this).attr("sysno"));
                    });
                });
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    },
    CityChange: function (sysno) {
        var str = "";
        $.ajax({
            type: "post",
            url: "/Home/GetAllDistrict",
            dataType: "json",
            data: { citySysNo: sysno },
            async: false,
            timeout: 30000,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data) {
                var json = eval(data);
                var selectVal = jQuery("#Province" + address).attr("selectVal");
                for (i = 0; i < json.length; i++) {
                    str += "<li><a href='javascript:void(0);' sysno='" + json[i].SysNo + "'>" + json[i].DistrictName + "</a></li>";
                };
                $("#selectdistrict").html(str).find("a").each(function () {
                    $(this).click(function () {
                        $("#selecteddistrict").html($(this).html());
                        $("#selecteddistrict").attr("sysno", $(this).attr("sysno"));
                    });
                });
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    }
};