$(function () {
    Biz.Common.Area.InitComponent();
});
var BizPersonInfoExtend = {
    init: function () {
        var liveStatus = ""; //居住现状
        $('[name=LiveStatus]:checkbox:checked').each(function () { liveStatus += $(this).val() + ","; });
        var profession = $('[name=Profession]:radio:checked').val(); //职业身份
        var marry = $('[name=Marry]:radio:checked').val(); //婚姻状况
        var homeTown = $('#District option:selected')[0].value; //我的家乡
        var areaId = $('#region option:selected').val(); //得到地区的id
        var favorURLTotle = ""; //喜欢的网址
        var favorURL1 = $('#favorURL1').val() || '';
        var favorURL2 = $('#favorURL2').val() || '';
        var favorURL3 = $('#favorURL3').val() || '';
        if (favorURL1.length > 0) { favorURLTotle += favorURL1 + ","; }
        if (favorURL2.length > 0) { favorURLTotle += favorURL2 + ","; }
        if (favorURL3.length > 0) { favorURLTotle += favorURL3 + ","; }
        var hotLove = $('#hotLove').val(); //兴趣爱好
        var hotPerson = $('#hotPerson').val(); //喜欢的明星
        var hotBrand = $('#hotBrand').val(); //喜欢的品牌
        if (hotLove == "多条内容用逗号分隔，每个词不能超过8个汉字") { hotLove = ""; }
        if (hotPerson == "多条内容用逗号分隔，每个词不能超过8个汉字") { hotPerson = ""; }
        if (hotBrand == "多条内容用逗号分隔，每个词不能超过8个汉字") { hotBrand = ""; }
        if (areaId != "-1" && homeTown == "-1") {
            alert("请选择您所在的城市。");
            return;
        }
        liveStatus = liveStatus || '';
        profession = profession || '';
        marry = marry || '';
        favorURLTotle = favorURLTotle || '';
        hotLove = hotLove || '';
        hotPerson = hotPerson || '';
        hotBrand = hotBrand || '';
        if (liveStatus.length == 0 && profession.length == 0 && marry.length == 0
            && homeTown == "-1" && favorURLTotle.length == 0 && hotLove.length == 0 && hotPerson.length == 0 && hotBrand.length == 0) {
            window.location.reload();  //没有填入任何信息，刷新页面
            return;
        }

        var customerExtendPersonInfo = {
            LivingCondition: liveStatus,
            Occupation: profession,
            Marriage: marry,
            HomeTownAreaSysNo: homeTown,
            FavorURL: favorURLTotle,
            PurchaseInterest: hotLove,
            FavorStar: hotPerson,
            PurchaseBrand: hotBrand
        };
        return customerExtendPersonInfo;
    },

    updateExtend: function () {
        if (!BizPersonInfoExtend.ValideAllData()) { return; }

        var obj = $('#btnSavePersonalInfo')[0];
        var personalInfo = BizPersonInfoExtend.init();
        if (personalInfo == null) return;
        BizPersonInfoExtend.postExtend(obj, personalInfo);
    },
    postExtend: function (obj, personalInfo) {
        var strPersonalInfo = JSON.stringify(personalInfo);
        var isSuccess = false;

        $.ajax({
            type: "post",
            dataType: "json",
            url: "/MemberAccount/AjaxUpdateCustomerExtendedInfo",
            timeout: 30000,
            data: { PExtendInfo: escape(strPersonalInfo) },
            beforeSend: function (XMLHttpRequest) { },
            success: function (data) {
                if (data == "s") {
                    alert("操作成功！");
                }
                else {
                    alert(data);
                }
            },
            complete: function (XMLHttpRequest, textStatus) { },
            error: function () { }
        });
    },
    clearMessage: function () {
        $('[name=LiveStatus]:checkbox').each(function () { $(this).attr('checked', false); });
        $('[name=Profession]:radio').attr('checked', false);
        $('[name=Marry]:radio').attr('checked', false);
        $("#Province").attr("value", '-1');
        $("#District").attr("value", '-1');
        $('#favorURL1').val("");
        $('#favorURL2').val("");
        $('#favorURL3').val("");
        if ($('#hotLove').val() != "多条内容用逗号分隔，每个词不能超过8个汉字") $('#hotLove').val("");
        if ($('#hotPerson').val() != "多条内容用逗号分隔，每个词不能超过8个汉字") $('#hotPerson').val("");
        if ($('#hotBrand').val() != "多条内容用逗号分隔，每个词不能超过8个汉字") $('#hotBrand').val("");
    },

    ValideAllData: function () {
        if (!BizPersonInfoExtend.valideUrlLen(null)) { return false; }

        if (!BizPersonInfoExtend.valideHotLove("#hotLove", "兴趣爱好：")) { return false; }
        if (!BizPersonInfoExtend.valideHotLove("#hotPerson", "喜欢的明星：")) { return false; }
        if (!BizPersonInfoExtend.valideHotLove("#hotBrand", "喜欢的品牌：")) { return false; }

        return true;
    },
    valideHotLove: function (obj, objTitle) {
        var value = $(obj).val();
        if (value == "多条内容用逗号分隔，每个词不能超过8个汉字") { return true; }
        var hotLove = $.trim(value)
        if (!$String.IsNullOrEmpty(hotLove)) {
            if (hotLove.length > 85) {
                alert("长度不能超过85个");
                return false;
            } else {   //英文","分割后，子串长度<= 8
                var arrSplit = new Array();
                arrSplit = hotLove.split(',');
                for (i = 0; i < arrSplit.length; i++) {
                    if ($.trim(arrSplit[i]).length > 8) {
                        alert("您的输入单个词超过8个字，请用英文逗号进行分割");
                        return false;
                    }
                }
            }
        }
        return true;
    },
    valideUrlLen: function () {
        var url1Length = $.trim($("#favorURL1").val()).length;
        var url2Length = $.trim($("#favorURL2").val()).length;
        var url3Length = $.trim($("#favorURL3").val()).length;

        if (!BizPersonInfoExtend.checkSpecialChar($("#favorURL1").val())) return false;
        if (!BizPersonInfoExtend.checkSpecialChar($("#favorURL2").val())) return false;
        if (!BizPersonInfoExtend.checkSpecialChar($("#favorURL3").val())) return false;

        //检测单个地址长度 <= 150
        if (url1Length > 150
            || url2Length > 150
            || url3Length > 150) {
            alert("博客地址长度超过限制，请仔细检查");
            return false;
        }

        //检测所有地址长度<= 150 * 3 + 2
        if (url1Length + url2Length + url3Length > 150 * 3 + 2) {
            alert("博客地址总长度超过限制，请仔细检查");
            return false;
        }

        return true;
    },
    checkSpecialChar: function (str) {
        if (str.indexOf(',') != -1) {
            alert("（微）博客地址不能包含特殊字符");
            return false;
        } else {
            return true;
        }
    },
    defaultText: function () { /*为页面上特定输入元素增加默认文字的效果*/
        var obj = $(".hasDefaultText");
        var tmpText = new Array();
        var defaultText = new Array();
        var objIndex = 0;
        for (i = 1; i <= obj.length; i++) {
            tmpText[i - 1] = obj.eq(i - 1).val();
            defaultText[i - 1] = "多条内容用逗号分隔，每个词不能超过8个汉字";// ExtendInfoMessage.defaultMessage;
        }
        obj.focus(function () {
            objIndex = obj.index($(this));
            if ($(this).val() == defaultText[objIndex]) {
                $(this).val("");
                //$(this).addClass("hasDefaultTextInputOn");
                $(this).removeClass("hasDefaultText");
            }
        });
        obj.blur(function () {
            objIndex = obj.index($(this));
            if ($(this).val() == "") {
                $(this).val(defaultText[objIndex]);
                $(this).addClass("hasDefaultText");
                //$(this).removeClass("hasDefaultTextInputOn");
            }
        });
    }
}