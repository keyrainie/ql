$(function () {
    $(".error").hide();
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
    });

    $("#slt_reg_province").focusin(function () {
        $(this).parent().addClass("intxtfocusli");
    }).focusout(function () { });

    $("#input_reg_name").blur(function () { Biz.Customer.Register.validateCustomerID(); });
    $("#input_reg_address").blur(function () { Biz.Customer.Register.validateAddress(); });
    $("#input_reg_name_zip").blur(function () { Biz.Customer.Register.validateZip(); });
    $("#input_reg_name_contact").blur(function () { Biz.Customer.Register.validateContact(); });
    $("#input_reg_name_cellphone").blur(function () { Biz.Customer.Register.validateCellphone(); });
    $("#input_reg_name_phone").blur(function () { Biz.Customer.Register.validatePhone(); });
    $("#input_reg_name_fax").blur(function () { Biz.Customer.Register.validateFax(); });
    $("#input_reg_name_site").blur(function () { Biz.Customer.Register.validateSite(); });
    $("#intxt_reg_email").blur(function () { Biz.Customer.Register.validateCustomerEmail(); });
    $("#slt_reg_province").change(function () {
        var pSysNo = $(this).val();
        var str = "";
        $.ajax({
            type: "post",
            url: "/Home/GetAllCity",
            dataType: "json",
            data: { proviceSysNo: pSysNo },
            async: false,
            timeout: 30000,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data) {
                var json = eval(data);
                for (i = 0; i < json.length; i++) {
                    str += '<option value="' + json[i].SysNo + '">' + json[i].CityName + '</option>';
                };
                $("#slt_reg_city").html(str);
                $("#slt_reg_city").change();
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    });
    $("#slt_reg_city").change(function () {
        var pSysNo = $(this).val();
        var str = "";
        $.ajax({
            type: "post",
            url: "/Home/GetAllDistrict",
            dataType: "json",
            data: { citySysNo: pSysNo },
            async: false,
            timeout: 30000,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data) {
                var json = eval(data);
                for (i = 0; i < json.length; i++) {
                    str += '<option value="' + json[i].SysNo + '">' + json[i].DistrictName + '</option>';
                };
                $("#slt_reg_distrct").html(str);
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });
    });
    $("#slt_reg_province").change();
});


usingNamespace("Biz.Customer")["Register"] = {
    refreshValidator: function (img, input) {
        var url = $(img).attr('ref1');
        newurl = url + "?r=" + Math.random();
        $(img).attr('src', newurl);
        $(input).focus();
    },
    validateCustomerID: function () {
        var reg_name = $("#input_reg_name");
        if ($String.Trim(reg_name.val()) == "" || $String.Trim(reg_name.val()) == "请输入商家名称") {
            reg_name.parent().find(".error").html("商家名称不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_name.val()))) {
            reg_name.parent().find(".error").html("商家名称不能为空").show();
        }
    },
    validateAddress: function () {
        var reg_address = $("#input_reg_address");
        if ($String.Trim(reg_address.val()) == "" || $String.Trim(reg_address.val()) == "请输入联系地址") {
            reg_address.parent().find(".error").html("联系地址不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_address.val()))) {
            reg_address.parent().find(".error").html("联系地址不能为空").show();
        }
    },
    validateZip: function () {
        var reg_zip = $("#input_reg_name_zip");
        if ($String.Trim(reg_zip.val()) == "" || $String.Trim(reg_zip.val()) == "请输入邮编") {
            reg_zip.parent().find(".error").html("邮编不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_zip.val()))) {
            reg_zip.parent().find(".error").html("邮编不能为空").show();
        } else if (!reg_zip.val().match(/^\d{6}$/)) {
            reg_zip.parent().find(".error").html("邮编格式不正确").show();
        }
    },
    validateContact: function () {
        var reg_contact = $("#input_reg_name_contact");
        if ($String.Trim(reg_contact.val()) == "" || $String.Trim(reg_contact.val()) == "请输入联系人") {
            reg_contact.parent().find(".error").html("联系人不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_contact.val()))) {
            reg_contact.parent().find(".error").html("联系人不能为空").show();
        }
    },
    validateCellphone: function () {
        var reg_cellphone = $("#input_reg_name_cellphone");
        if (reg_cellphone.val().length > 0 && !Biz.Common.Validation.isMobile($String.Trim(reg_cellphone.val()))) {
            reg_cellphone.parent().find(".error").html("联系手机格式不正确").show();
        }
    },
    validatePhone: function () {
        var reg_phone = $("#input_reg_name_phone");
        if ($String.Trim(reg_phone.val()) == "" || $String.Trim(reg_phone.val()) == "请输入联系电话") {
            reg_phone.parent().find(".error").html("联系电话不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_phone.val()))) {
            reg_phone.parent().find(".error").html("联系电话不能为空").show();
        } else if (!Biz.Common.Validation.isTel($String.Trim(reg_phone.val()))) {
            reg_phone.parent().find(".error").html("联系电话格式不正确").show();
        }
    },
    validateFax: function () {
        var reg_fax = $("#input_reg_name_fax");
        if ($String.Trim(reg_fax.val()) == "" || $String.Trim(reg_fax.val()) == "请输入传真") {
            reg_fax.parent().find(".error").html("传真不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_fax.val()))) {
            reg_fax.parent().find(".error").html("传真不能为空").show();
        } else if (reg_fax.val().length > 0 && !Biz.Common.Validation.isTel($String.Trim(reg_fax.val()))) {
            reg_fax.parent().find(".error").html("传真格式不正确").show();
        }
    },
    validateSite: function () {
        var reg_site = $("#input_reg_name_site");
        if ($String.Trim(reg_site.val()) == "" || $String.Trim(reg_site.val()) == "请输入网址") {
            reg_site.parent().find(".error").html("网址不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_site.val()))) {
            reg_site.parent().find(".error").html("网址不能为空").show();
        } else if (!Biz.Common.Validation.isUrl($String.Trim(reg_site.val()))) {
            reg_site.parent().find(".error").html("网址格式不正确").show();
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
    },
    Submit: function () {
        $(".error").hide();
        var num = 0;
        var reg_name = $("#input_reg_name");

        if ($String.Trim(reg_name.val()) == "" || $String.Trim(reg_name.val()) == reg_name.attr("tip")) {
            num++;
            reg_name.parent().find(".error").html("商家名称不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_name.val()))) {
            num++;
            reg_name.parent().find(".error").html("商家名称不能为空").show();
        }

        var reg_contact = $("#input_reg_name_contact");
        if ($String.Trim(reg_contact.val()) == "" || $String.Trim(reg_contact.val()) == "请输入联系人") {
            reg_contact.parent().find(".error").html("联系人不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_contact.val()))) {
            reg_contact.parent().find(".error").html("联系人不能为空").show();
        }

        var reg_phone = $("#input_reg_name_phone");
        if ($String.Trim(reg_phone.val()) == "" || $String.Trim(reg_phone.val()) == "请输入联系电话") {
            reg_phone.parent().find(".error").html("联系电话不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_phone.val()))) {
            reg_phone.parent().find(".error").html("联系电话不能为空").show();
        } else if (!Biz.Common.Validation.isTel($String.Trim(reg_phone.val()))) {
            reg_phone.parent().find(".error").html("联系电话格式不正确").show();
        }

        var reg_cellphone = $("#input_reg_name_cellphone");
        if (reg_cellphone.val().length > 0 && !Biz.Common.Validation.isMobile($String.Trim(reg_cellphone.val()))) {
            reg_cellphone.parent().find(".error").html("联系手机格式不正确").show();
        }

        var reg_fax = $("#input_reg_name_fax");
        if ($String.Trim(reg_fax.val()) == "" || $String.Trim(reg_fax.val()) == "请输入传真") {
            reg_fax.parent().find(".error").html("传真不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_fax.val()))) {
            reg_fax.parent().find(".error").html("传真不能为空").show();
        } else if (reg_fax.val().length > 0 && !Biz.Common.Validation.isTel($String.Trim(reg_fax.val()))) {
            reg_fax.parent().find(".error").html("传真格式不正确").show();
        }

        var reg_email = $("#intxt_reg_email");
        if ($String.Trim(reg_email.val()) == "" || $String.Trim(reg_email.val()) == "请输入邮箱") {
            num++;
            reg_email.parent().find(".error").html("邮箱不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_email.val()))) {
            num++;
            reg_email.parent().find(".error").html("邮箱不能为空").show();
        }
        else if (!Biz.Common.Validation.isEmail($String.Trim(reg_email.val()))) {
            reg_email.parent().find(".error").html("邮箱格式不正确").show();
            num++;
        }

        var reg_site = $("#input_reg_name_site");
        if ($String.Trim(reg_site.val()) == "" || $String.Trim(reg_site.val()) == "请输入网址") {
            reg_site.parent().find(".error").html("网址不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_site.val()))) {
            reg_site.parent().find(".error").html("网址不能为空").show();
        } else if (!Biz.Common.Validation.isUrl($String.Trim(reg_site.val()))) {
            reg_site.parent().find(".error").html("网址格式不正确").show();
        }

        var reg_zip = $("#input_reg_name_zip");
        if ($String.Trim(reg_zip.val()) == "" || $String.Trim(reg_zip.val()) == "请输入邮编") {
            reg_zip.parent().find(".error").html("邮编不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_zip.val()))) {
            reg_zip.parent().find(".error").html("邮编不能为空").show();
        } else if (!reg_zip.val().match(/^\d{6}$/)) {
            reg_zip.parent().find(".error").html("邮编格式不正确").show();
        }

        var reg_address = $("#input_reg_address");
        if ($String.Trim(reg_address.val()) == "" || $String.Trim(reg_address.val()) == "请输入联系地址") {
            reg_address.parent().find(".error").html("联系不能为空").show();
        }
        else if (Biz.Common.Validation.CheckSpecialString($String.Trim(reg_address.val()))) {
            reg_address.parent().find(".error").html("联系地址不能为空").show();
        }

        if ($("#input_reg_validecode").val() == "") {
            num++;
            $("#input_reg_validecode").parent().find(".error").show();
        }

        if (num == 0) {
            $.ajax({
                type: "post",
                url: "/Home/AjaxSellerRegister",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: {
                    VendorName: $String.Trim($("#input_reg_name").val()),
                    EnglishName: $("#input_reg_name_en").val(),
                    InvoiceType: $("#slt_reg_type_i").val(),
                    StockType: $("#slt_reg_type_stock").val(),
                    District: $("#slt_reg_province").find("option:selected").text() + $("#slt_reg_city").find("option:selected").text() + $("#slt_reg_distrct").find("option:selected").text(),
                    Address: $("#input_reg_address").val(),
                    EmailAddress: $("#intxt_reg_email").val(),
                    Website: $("#intxt_reg_site").val(),
                    Phone: $("#input_reg_name_phone").val(),
                    CellPhone: $("#input_reg_name_cellphone").val(),
                    Fax: $("#input_reg_name_fax").val(),
                    Contact: $("#input_reg_name_contact").val(),
                    ZipCode: $("#input_reg_name_zip").val(),
                    ValidatedCode: $("#input_reg_validecode").val()
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
                        alert(data.Data);
                        window.location = "/";
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    }
}

