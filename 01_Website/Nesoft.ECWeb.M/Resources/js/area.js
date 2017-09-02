(function (win, $) {

    function _InitProvince() {
        $.ajax({
            type: "post",
            url: "/Common/GetAllProvince",
            dataType: "json",
            async: false,
            timeout: 30000,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                $("#Province").empty().append("<option value='0' selected='selected'>" + "请选择省份" + "</option>");
                $("#City").empty().append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
                $("#District").empty().append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
                var json = eval(data);
                var selectVal = $("#Province").attr("selectVal");
                for (i = 0; i < json.length; i++) {
                    if (json[i].SysNo.toString() == selectVal) {
                        $("#Province").append($("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].ProvinceName));
                    }
                    else {
                        $("#Province").append($("<option></option>").val(json[i].SysNo).html(json[i].ProvinceName));
                    }
                };
                _ProvinceChange();
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });

    };
    function _ProvinceChange() {
        if ($("#Province").val() != "0") {
            $("#City").empty();
            $("#District").empty();
            $("#City").append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
            $("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
            $.ajax({
                type: "post",
                url: "/Common/GetAllCity",
                dataType: "json",
                data: { proviceSysNo: $("#Province").val() },
                async: false,
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    var json = eval(data);
                    var selectVal = $("#City").attr("selectVal");
                    for (i = 0; i < json.length; i++) {
                        if (json[i].SysNo.toString() == selectVal) {
                            $("#City").append($("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].CityName));
                        }
                        else {
                            $("#City").append($("<option></option>").val(json[i].SysNo).html(json[i].CityName));
                        }
                    };
                    _CityChange();
                }
            });
        } else {
            $("#City").empty();
            $("#District").empty();
            $("#City").append("<option value='0' selected='selected'>" + "请选择城市" + "</option>");
            $("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
        }
    }
    function _CityChange() {
        if ($("#City").val() != "0") {
            $("#District").empty();
            $("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
            $.ajax({
                type: "post",
                url: "/Common/GetAllDistrict",
                dataType: "json",
                async: false,
                data: { citySysNo: $("#City").val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    var json = eval(data);
                    var selectVal = $("#District").attr("selectVal");
                    for (i = 0; i < json.length; i++) {
                        if (json[i].SysNo.toString() == selectVal) {
                            $("#District").append($("<option selected='selected'></option>").val(json[i].SysNo).html(json[i].DistrictName));
                        }
                        else {
                            $("#District").append($("<option></option>").val(json[i].SysNo).html(json[i].DistrictName));
                        }
                    };
                }
            });
        } else {
            $("#District").empty();
            $("#District").append("<option value='0' selected='selected'>" + "请选择地区" + "</option>");
        }
    };
    //页面引用该组件时，在页面加载完毕后调用此方法来初始化组件
    function _Init() {
        _InitProvince();
        $("#Province").change(function () {
            _ProvinceChange();
        });
        $("#City").change(function () {
            _CityChange();
        });
    }

    win["Area"] = win["Area"] || {

        init: _Init
    };

})(window, Zepto);

