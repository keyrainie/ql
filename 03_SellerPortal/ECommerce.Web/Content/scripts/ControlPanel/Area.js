(function ($) {
    $.area = {
        province: function (tag) {
            return $("#sltProvince" + (tag == undefined ? "" : tag)).val();
        },
        city: function (tag) {
            return $("#sltCity" + (tag == undefined ? "" : tag)).val();
        },
        district: function (tag) {
            return $("#sltDistrict" + (tag == undefined ? "" : tag)).val();
        },
        init: function (param) {
            param = param || {};
            if (param.pId == undefined) {
                param.pId = "sltProvince";
            }
            if (param.cId == undefined) {
                param.cId = "sltCity";
            }
            if (param.dId == undefined) {
                param.dId = "sltDistrict";
            }
            if (param.changed == undefined) {
                param.changed = function () { };
            }
            if (param.hideCity == undefined) {
                param.hideCity = false;
            }
            if (param.hideDistrict == undefined) {
                param.hideDistrict = false;
            }
            if (param.hideCity == true) {
                param.hideDistrict = true;
            }
            //init default value
            param.defaultP = $("#" + param.pId).attr("default");
            param.defaultC = $("#" + param.cId).attr("default");
            param.defaultD = $("#" + param.dId).attr("default");
            //init change event
            $("#" + param.pId).change(function () {
                var valueP = $(this).children("option:selected").val();
                if (param.hideCity == false) {
                    var cParam = param;
                    cParam.provinceSysNo = valueP;
                    cParam.callback = function () { };
                    $.area.loadCity(cParam);
                }
                if (param.changed) {
                    param.changed(valueP);
                }
            });
            $("#" + param.cId).change(function () {
                var valueP = $("#" + param.pId).val();
                var valueC = $(this).children("option:selected").val();
                if (param.hideDistrict == false) {
                    var dParam = param;
                    dParam.citySysNo = valueC;
                    dParam.callback = function () { };
                    $.area.loadDistrict(dParam);
                }
                if (param.changed) {
                    param.changed(valueP, valueC);
                }
            });
            $("#" + param.dId).change(function () {
                var valueP = $("#" + param.pId).val();
                var valueC = $("#" + param.cId).val();
                var valueD = $(this).children("option:selected").val();
                if (param.changed) {
                    param.changed(valueP, valueC, valueD);
                }
            });

            var pParam = param;
            pParam.callback = function () {
                if (param.defaultP != "" && param.hideCity == false) {
                    var cParam = param;
                    cParam.provinceSysNo = cParam.defaultP;
                    cParam.callback = function () {
                        if (param.defaultC != "" && param.hideCity == false) {
                            var dParam = param;
                            dParam.citySysNo = dParam.defaultC;
                            dParam.callback = function () { };
                            $.area.loadDistrict(dParam);
                        }
                    };
                    $.area.loadCity(cParam);
                }
            };
            $.area.loadProvince(pParam);
            if (param.changed) {
                param.changed(param.defaultP, param.defaultC, param.defaultD);
            }
            if (param.hideCity == true) {
                $("#" + param.cId).hide();
                $("#" + param.dId).hide();
            }
            if (param.hideDistrict == true) {
                $("#" + param.dId).hide();
            }
        },
        loadProvince: function (p) {
            var param = {
                id: p.pId,
                value: p.defaultP,
                url: "/Common/Province?hasCountry=" + p.hasCountry + "&hasRegion=" + p.hasRegion,
                showAll: p.showAll,
                callback: function () {
                    $('#' + p.pId).selectpicker('refresh');
                    if (p.callback) {
                        p.callback();
                    }
                },
            };
            $.selecter(param);
        },
        loadCity: function (p) {
            if (p.provinceSysNo != undefined && p.provinceSysNo != "") {
                var param = {
                    id: p.cId,
                    value: p.defaultC,
                    url: "/Common/City?provinceSysNo=" + p.provinceSysNo,
                    showAll: p.showAll,
                    callback: function () {
                        $.bindSelecter({
                            id: p.dId,
                            data: [],
                        });
                        $('#' + p.dId).selectpicker('refresh');
                        $('#' + p.cId).selectpicker('refresh');
                        if (p.callback) {
                            p.callback();
                        }
                    },
                };
                $.selecter(param);
            } else {
                $.bindSelecter({
                    id: p.cId,
                    data: [],
                });
                $('#' + p.cId).selectpicker('refresh');
                $.bindSelecter({
                    id: p.dId,
                    data: [],
                });
                $('#' + p.dId).selectpicker('refresh');
            }
        },
        loadDistrict: function (p) {
            if (p.citySysNo != undefined && p.citySysNo != "") {
                var param = {
                    id: p.dId,
                    value: p.defaultD,
                    url: "/Common/District?citySysNo=" + p.citySysNo,
                    showAll: p.showAll,
                    callback: function () {
                        $('#' + p.dId).selectpicker('refresh');
                        if (p.callback) {
                            p.callback();
                        }
                    },
                };
                $.selecter(param);
            } else {
                $.bindSelecter({
                    id: p.dId,
                    data: [],
                });
                $('#' + p.dId).selectpicker('refresh');
            }
        },
    };
})(jQuery);