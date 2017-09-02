
usingNamespace("Biz.Common")["Area"] = {
    InitProvince: function () {
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
                jQuery("#Province").append("<option value='0' selected='selected'>" + "请选择省份" + "</option>");
                jQuery("#City").append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
                jQuery("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
                var json = eval(data);
                var selectVal = jQuery("#Province").attr("selectVal");
                for (i = 0; i < json.length; i++) {
                    if (json[i].SysNo.toString() == selectVal) {
                        jQuery("#Province").append(jQuery("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].ProvinceName));
                    }
                    else {
                        jQuery("#Province").append(jQuery("<option></option>").val(json[i].SysNo).html(json[i].ProvinceName));
                    }
                };
                Biz.Common.Area.ProvinceChange();
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });

    },
    ProvinceChange: function () {
        if (typeof (provinceChanged) == "function") {
            provinceChanged($("#Province"), $("#Province").val());
        }
        if ($("#Province").val() != "0") {
            jQuery("#City").empty();
            jQuery("#District").empty();
            jQuery("#City").append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
            jQuery("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
            $.ajax({
                type: "post",
                url: "/Home/GetAllCity",
                dataType: "json",
                data: { proviceSysNo: $("#Province").val() },
                async: false,
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    var json = eval(data);
                    var selectVal = jQuery("#City").attr("selectVal");
                    for (i = 0; i < json.length; i++) {
                        if (json[i].SysNo.toString() == selectVal) {
                            jQuery("#City").append(jQuery("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].CityName));
                        }
                        else {
                            jQuery("#City").append(jQuery("<option></option>").val(json[i].SysNo).html(json[i].CityName));
                        }
                    };
                    Biz.Common.Area.CityChange();
                }
            });
        } else {
            jQuery("#City").empty();
            jQuery("#District").empty();
            jQuery("#City").append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
            jQuery("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
        }
    },
    CityChange: function () {
        if (typeof (cityChanged) == "function") {
            cityChanged($("#City"), $("#City").val());
        }
        if ($("#City").val() != "0") {
            jQuery("#District").empty();
            jQuery("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
            $.ajax({
                type: "post",
                url: "/Home/GetAllDistrict",
                dataType: "json",
                async: false,
                data: { citySysNo: $("#City").val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    var json = eval(data);
                    var selectVal = jQuery("#District").attr("selectVal");
                    for (i = 0; i < json.length; i++) {
                        if (json[i].SysNo.toString() == selectVal) {
                            jQuery("#District").append(jQuery("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].DistrictName));
                        }
                        else {
                            jQuery("#District").append(jQuery("<option></option>").val(json[i].SysNo).html(json[i].DistrictName));
                        }
                    };
                }
            });
        } else {
            jQuery("#District").empty();
            jQuery("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
        }
    }, DistrictChange: function () {
        if (typeof (districtChanged) == "function") {
            districtChanged($("#District"), $("#District").val());
        }
    },
    //页面引用该组件时，在页面加载完毕后调用此方法来初始化组件
    InitComponent: function () {
        Biz.Common.Area.InitProvince();
        $("#Province").change(function () {
            Biz.Common.Area.ProvinceChange();
        });
        $("#City").change(function () {
            Biz.Common.Area.CityChange();
        });
        $("#District").change(function () {
            Biz.Common.Area.DistrictChange();
        });
    }
}