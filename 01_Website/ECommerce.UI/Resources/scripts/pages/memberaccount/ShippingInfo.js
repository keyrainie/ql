$(function () {
    //Biz.Common.Area.InitComponent();
	$(".Validform_wrong").hide();
});
usingNamespace("Biz.AccountCenter")["ShippingAddressInfo"] = {
    ShippingAddressManager: function () {
        this.CurrentEditAreaData = '';
        this.CurrentElementContent = '';
        this.CurrentEditAreaInnerHtml = '';

        var this_reference = this;
        this.SubmitCallHandler = function () {
            this_reference.InvokeSubmitDelegateFunction.call(this_reference);
        }

        this.CancelCallHandler = function () {
            this_reference.InvokeCancelDelegateFunction.call(this_reference);
        }

        this.InvokeSubmitDelegateFunction = function () {
            var jqueryObj = this.CurrentElementContent;
            var oldData = this.CurrentEditAreaData;
            var newData = Biz.AccountCenter.ShippingAddressInfo.init(jqueryObj);
            if (newData == null) { return; }

            if (!this.Compared(oldData, newData)) {
                var submitObj = jqueryObj.parent().find('.action')[0];
                Biz.AccountCenter.ShippingAddressInfo.post(jqueryObj, newData, 1, submitObj);
            }
            else {
                $.Showmsg("没有可修改的数据", {
                    caption: "系统提示",
                    afterPopOut: function (obj) { }
                });
            }
        }

        this.InvokeCancelDelegateFunction = function () {
            var jqueryObj = this.CurrentElementContent;
            var innerHTML = this.CurrentEditAreaInnerHtml;
            jqueryObj.empty().append(innerHTML);
        }

        this.Compared = function (oldData, newData) {
            if (oldData.AddressTitle == newData.AddressTitle
            && oldData.IsDefault == newData.IsDefault
            && oldData.ReceiveContact == newData.ReceiveContact
            && oldData.ReceiveCellPhone == newData.ReceiveCellPhone
            && oldData.ReceivePhone == newData.ReceivePhone
            && oldData.AddressAreaID == newData.AddressAreaID
            && oldData.ReceiveAddress == newData.ReceiveAddress
            && oldData.ReceiveZip == newData.ReceiveZip
            && oldData.ReceiveFax == newData.ReceiveFax) {
                return true;
            }
            return false;
        }
    },

    getCurrentData: function (jqueryObj, hasValidated, actionType) {
        var addressShortName = jqueryObj.find('#addressShort');
        var isDefaultAddress = jqueryObj.find('#defaultAddress')[0].checked;
        var addressTitle = jqueryObj.find('#addressTitle');
        var contactMan = jqueryObj.find('#contactMan');
        var cellPhone = jqueryObj.find('#mobile')
        var phone = jqueryObj.find('#tel');
        //var systemAreaID = jqueryObj.find('#District')
        var address = jqueryObj.find('#address');
        var zip = jqueryObj.find('#zip');
        var fax = jqueryObj.find('#fax');
        var id = jqueryObj.find('#txtShippingAddressID');
        var area = jqueryObj.find("select[name=District]");

        if (hasValidated) {
            if ($String.Trim(addressShortName.val()) == "") {
                addressShortName.addClass("intxtfocus").focus();
                return null;
            }
            if ($String.Trim(contactMan.val()) == "") {
                contactMan.addClass("intxtfocus").focus();
                return null;
            }
            else if ($String.Trim(contactMan.val()).length > 20) {
                contactMan.addClass("intxtfocus").focus();
                return null;
            }
            if ($String.Trim(cellPhone.val()) != "") {
                if (!Biz.Common.Validation.isMobile($String.Trim(cellPhone.val()))) {
                    cellPhone.addClass("intxtfocus").focus();
                    return null;
                }
            }
            else {
                cellPhone.addClass("intxtfocus").focus();
                return null;
            }
            //if ($String.Trim(phone.val()) == "") {
            //    phone.addClass("intxtfocus").focus();
            //    return null;
            //}
            if (area[0].options.selectedIndex == 0) {
                area.addClass("intxtfocus").focus();
                return null;
            }
            //Biz.Common.ValidateHelper.clearMessageForShippingAddress(address, 'AccountCenter_ModifyShippingAddress_ReceiveAddressDesc');
            if ($String.Trim(address.val()) == "") {
                address.addClass("intxtfocus").focus();
                return null;
            }
            //Biz.Common.ValidateHelper.clearMessageForShippingAddress(zip, 'AccountCenter_ModifyShippingAddress_ZipDesc');
            if ($String.Trim(zip.val()) == "") {
                zip.addClass("intxtfocus").focus();
                return null;
            }
            else if (!Biz.Common.Validation.isZip($String.Trim(zip.val()))) {
                zip.addClass("intxtfocus").focus();
                return null;
            }
        }
        var shippingAddress = {
            AddressTitle: $String.Trim(addressShortName.val()),
            IsDefault: isDefaultAddress,
            //ReceiveName: $String.Trim(addressTitle.val()),
            ReceiveContact: $String.Trim(contactMan.val()),
            ReceiveCellPhone: $String.Trim(cellPhone.val()),
            ReceivePhone: $String.Trim(phone.val()),
            ReceiveAreaSysNo: area.val(), //options.value,//systemAreaID.val()),
            ReceiveAddress: $String.Trim(address.val()),
            ReceiveZip: $String.Trim(zip.val()),
            ReceiveFax: $String.Trim(fax.val()),
            SysNo: $String.Trim(id.val())
        };
        return shippingAddress;
    },

    init: function (jqueryObj, actionType) {
        return Biz.AccountCenter.ShippingAddressInfo.getCurrentData(jqueryObj, true, actionType);
    },

    ValidateMobile: function (obj) {
        //Biz.Common.ValidateHelper.clearMessage(obj);
        //var emObj = $(obj.parent().find('em')[0]);
        if ($String.Trim(obj.val()) != "") {
            if (!Biz.Common.Validation.isMobile($String.Trim(obj.val()))) {
                var spanElement = "<em class='tiptext tiperror'>请输入正确的手机号码</em>";
                //emObj.replaceWith(spanElement);
                return false;
            }
            else {
                var spanElement = "<em class=''>请输入正确的手机号码</em>";
                //emObj.replaceWith(spanElement);
                return true;
            }
        }
        else {
            var spanElement = "<em class='tiptext tiperror'>请输入正确的手机号码</em>";
            //emObj.replaceWith(spanElement);
            return false;
        }
    },

    clearValue: function (obj) {
        var jqueryObj = $($(obj).parent().find('.cls')[0]);
        Biz.AccountCenter.ShippingAddressInfo.clear(jqueryObj);
    },

    clear: function (jqueryObj) {
        jqueryObj.find('#addressShort').val('');
        jqueryObj.find('#defaultAddress').attr('checked', false);
        jqueryObj.find('#addressTitle').val('');
        jqueryObj.find('#contactMan').val('');
        jqueryObj.find('#mobile').val('');
        jqueryObj.find('#tel').val('');

        jqueryObj.find('#address').val('');
        jqueryObj.find('#zip').val('');
        jqueryObj.find('#fax').val('');

        jqueryObj.find(".sys_select[name='Province']").val(0);
        var cityObj=jqueryObj.find(".sys_select[name='City']");
        var districtObj = jqueryObj.find(".sys_select[name='District']");
        $(cityObj).html("<option selected=\"selected\" value=\"0\">请选择城市</option>");
        $(districtObj).html("<option selected=\"selected\" value=\"0\">请选择地区</option>");
        $(cityObj).val(0);
        $(districtObj).val(0);
    },
    post: function (jqueryObj, addressAreaInfo, actionType, obj) {
        var strAreaInfo = Web.Utils.Json.ToJson(addressAreaInfo);
        $.ajax({
            type: "post",
            dataType: "json",
            url: "/MemberAccount/CreateCustomerShippingInfo",
            timeout: 30000,
            data: { Action: actionType, Data: escape(strAreaInfo) }, //actionType:2为删除
            cache: false,
            global: true,
            beforeSend: function (XMLHttpRequest) { },
            success: function (data) {
                if (data == "s") {
                    alert("操作成功！");
                    var timer = window.setInterval(function () {
                        window.location = window.location;
                        clearInterval(timer);
                    }, 1500);
                }
                else {
                    alert(data);
                }
            },
            complete: function (XMLHttpRequest, textStatus) { },
            error: function () { }
        });
    },

    submit: function (obj, actionType) {
        var jqueryObj = $($(obj).parent().find('.cls')[0]);
        var processobj = $($(obj).parent().find('.cls')[0]);
        var addressAreaInfo = Biz.AccountCenter.ShippingAddressInfo.init(jqueryObj, actionType);
        if (addressAreaInfo == null) return;
        Biz.AccountCenter.ShippingAddressInfo.post(jqueryObj, addressAreaInfo, actionType, processobj);
    },

    addAddress: function (obj) {
        Biz.AccountCenter.ShippingAddressInfo.submit(obj, 0);
    },

    editAddress: function (callbackFunction) {
        if ($.isFunction(callbackFunction)) callbackFunction.call($);
    },

    deleteAddress: function (obj) {
        if (confirm("确定删除吗？")) {
            var jqueryObj = $($(obj).parent().parent().parent().next("div"));
            var addressAreaInfo = Biz.AccountCenter.ShippingAddressInfo.getCurrentData(jqueryObj, false);
            Biz.AccountCenter.ShippingAddressInfo.post(jqueryObj, addressAreaInfo, 2, obj);
        };
    },
    CanelEdit: function (obj) {
        var jqueryObj = $(obj).parent().parent();
        var newObj = jqueryObj.prev("div");
        newObj.slideDown("fast");
        jqueryObj.hide();
        jqueryObj.find('.bottomarea #clearAll')[0].onclick();
    },
    showEdit: function (obj, b) {
        if (b) {
            var jqueryObj = $(obj).parent().parent().parent();
            var newObj = jqueryObj.next("div");
            newObj.slideDown("fast");
            jqueryObj.hide();
            newObj.find('.action #submit').each(function (index, item) {
                var addressManager = new Biz.AccountCenter.ShippingAddressInfo.ShippingAddressManager();
                var editAreaJQueryObj = $($(item).parent().parent().find('.cls')[0]);
                addressManager.CurrentElementContent = editAreaJQueryObj;
                addressManager.CurrentEditAreaData = Biz.AccountCenter.ShippingAddressInfo.getCurrentData(editAreaJQueryObj, false);

                item.onclick = function () {
                    Biz.AccountCenter.ShippingAddressInfo.editAddress(addressManager.SubmitCallHandler);
                };
            });
            newObj.find('.action #clearAll').each(function (index, item) {
                var addressManager = new Biz.AccountCenter.ShippingAddressInfo.ShippingAddressManager();
                var editAreaJQueryObj = $($(item).parent().parent().prev("div")[0]);
                addressManager.CurrentElementContent = editAreaJQueryObj;
                addressManager.CurrentEditAreaInnerHtml = editAreaJQueryObj[0].innerHTML;
                item.onclick = function () {
                    Biz.AccountCenter.ShippingAddressInfo.editAddress(addressManager.CancelCallHandler);
                };
            });
        }
        else {
            var jqueryObj = $(obj).parent().parent().parent();
            var preObj = jqueryObj.prev("div");
            preObj.slideDown("fast");
            jqueryObj.hide();
            jqueryObj.find('.action #clearAll')[0].onclick();
        }
    },
    /*下面是取省市*/

    InitProvince: function (address) {
        $.ajax({
            type: "post",
            url: "/Home/GetAllProvince",
            dataType: "json",
            async: false,
            timeout: 30000,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                jQuery("#Province" + address).append("<option value='0' selected='selected'>" + "请选择省份" + "</option>");
                jQuery("#City" + address).append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
                jQuery("#District" + address).append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
                var json = eval(data);
                var selectVal = jQuery("#Province" + address).attr("selectVal");
                for (i = 0; i < json.length; i++) {
                    if (json[i].SysNo.toString() == selectVal) {
                        jQuery("#Province" + address).append(jQuery("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].ProvinceName));
                    }
                    else {
                        jQuery("#Province" + address).append(jQuery("<option></option>").val(json[i].SysNo).html(json[i].ProvinceName));
                    }
                };
                Biz.AccountCenter.ShippingAddressInfo.ProvinceChange(address);
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });

    },
    ProvinceChange: function (address) {
        if (typeof (provinceChanged) == "function") {
            provinceChanged($("#Province" + address), $("#Province" + address).val());
        }
        if ($("#Province" + address).val() != "0") {
            jQuery("#City" + address).empty();
            jQuery("#District" + address).empty();
            jQuery("#City" + address).append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
            jQuery("#District" + address).append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
            $.ajax({
                type: "post",
                url: "/Home/GetAllCity",
                dataType: "json",
                data: { proviceSysNo: $("#Province" + address).val() },
                async: false,
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    var json = eval(data);
                    var selectVal = jQuery("#City" + address).attr("selectVal");
                    for (i = 0; i < json.length; i++) {
                        if (json[i].SysNo.toString() == selectVal) {
                            jQuery("#City" + address).append(jQuery("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].CityName));
                        }
                        else {
                            jQuery("#City" + address).append(jQuery("<option></option>").val(json[i].SysNo).html(json[i].CityName));
                        }
                    };
                    Biz.AccountCenter.ShippingAddressInfo.CityChange(address);
                }
            });
        } else {
            jQuery("#City" + address).empty();
            jQuery("#District" + address).empty();
            jQuery("#City" + address).append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
            jQuery("#District" + address).append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
        }
    },
    CityChange: function (address) {
        if (typeof (cityChanged) == "function") {
            cityChanged($("#City" + address), $("#City" + address).val());
        }
        if ($("#City" + address).val() != "0") {
            jQuery("#District" + address).empty();
            jQuery("#District" + address).append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
            $.ajax({
                type: "post",
                url: "/Home/GetAllDistrict",
                dataType: "json",
                async: false,
                data: { citySysNo: $("#City" + address).val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    var json = eval(data);
                    var selectVal = jQuery("#District" + address).attr("selectVal");
                    for (i = 0; i < json.length; i++) {
                        if (json[i].SysNo.toString() == selectVal) {
                            jQuery("#District" + address).append(jQuery("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].DistrictName));
                        }
                        else {
                            jQuery("#District" + address).append(jQuery("<option></option>").val(json[i].SysNo).html(json[i].DistrictName));
                        }
                    };
                }
            });
        } else {
            jQuery("#District" + address).empty();
            jQuery("#District" + address).append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
        }
    }, DistrictChange: function () {
        if (typeof (districtChanged) == "function") {
            districtChanged($("#District" + address), $("#District").val());
        }
    },
    //页面引用该组件时，在页面加载完毕后调用此方法来初始化组件

    InitComponent: function (address) {
        Biz.AccountCenter.ShippingAddressInfo.InitProvince(address);
        $("#Province" + address).change(function () {
            Biz.AccountCenter.ShippingAddressInfo.ProvinceChange(address);
        });
        $("#City" + address).change(function () {
            Biz.AccountCenter.ShippingAddressInfo.CityChange(address);
        });
        $("#District" + address).change(function () {
            Biz.AccountCenter.ShippingAddressInfo.DistrictChange(address);
            jQuery("#District" + address).removeClass("intxtfocus");
        });
    }
};
