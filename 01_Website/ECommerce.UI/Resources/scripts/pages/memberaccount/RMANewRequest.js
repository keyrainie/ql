$(function () {
    for (var i = 0; i < SoItemCount; i++) 
    {

        var selectRequestArea = 'selectRequestArea_' + i;
        var selectRequest = 'selectRequest_' + i;
        $("#"+selectRequestArea+" a").each(function() {
            $(this).click(function () {
                var select = $(this).parent().parent().parent().parent().parent().find("dt").find("a");
                select.attr("sysno", $(this).attr("sysno"));
                select.removeClass();
            });
        });

        var selectReasonArea = 'selectReasonArea_' + i;
        var selectReason = 'selectReason_' + i;
        $("#" + selectReasonArea + " a").each(function () {
            $(this).click(function () {
                var select = $(this).parent().parent().parent().parent().parent().find("dt").find("a");
                select.attr("sysno", $(this).attr("sysno"));
                select.removeClass();
            });
        });

        var Quantity = 'Quantity_' + i;
        $("#" + Quantity).keyup(function () {
            this.value = this.value.replace(/[^\d]/g, '');

            var limitQty = $(this).parent().attr('qty');
            if (parseInt(this.value) > limitQty) 
                this.value = limitQty;
        });

        $("#" + Quantity).blur(function () {
            var limitQty = $(this).parent().attr('qty');
            if(this.value==''||this.value==0)
                this.value = $(this).parent().attr('qty');
        });
    }

    $("#selectShipTypeArea a").each(function () {
        $(this).click(function () {
            $("#selectShipType").attr("sysno", $(this).attr("sysno"));
            $('#selectShipType').removeClass();
        });
    });

    $("#ExpressName").change(function () {
        $('#ExpressName').removeClass("intxtfocus");
    });
    $("#PackageNumber").change(function () {
        $('#PackageNumber').removeClass("intxtfocus");
    });
    $("#BackAddress").change(function () {
        $('#BackAddress').removeClass("intxtfocus");
    });
    $("#BackTelephone").change(function () {
        $('#BackTelephone').removeClass("intxtfocus");
    });
    $("#Contact").change(function () {
        $('#Contact').removeClass("intxtfocus");
    });
    $("#Province").change(function () {
        $('#Province').removeClass("intxtfocus");
    });
    $("#City").change(function () {
        $('#City').removeClass("intxtfocus");
    });
    $("#District").change(function () {
        $('#District').removeClass("intxtfocus");
    });
    $(".inarea").keyup(function () {
        if ($String.Trim($(this).val()) != "") {
            $(this).removeClass("intxtfocus");
        }
    });
    $(".inarea").blur(function () {
        if ($String.Trim($(this).val()) != "") {
            $(this).removeClass("intxtfocus");
        }
    });

    function validata() {

        var IsHaveChecked = false;
        for (i = 0; i < SoItemCount; i++) {

            var checkProduct = "checkProduct_" + i;

            if ($("#" + checkProduct).attr("checked") == "checked") {
                IsHaveChecked = true;

                var QuantityLimit = "QuantityLimit_" + i;
                Quantity = 'Quantity_' + i;
                if ($('#' + Quantity).val() > $('#' + QuantityLimit).attr('qty') || $('#' + Quantity).val() == 0 || $('#' + Quantity).val()=='') {
                    $('#' + Quantity).addClass("intxtfocus").focus();
                    return false;
                }
                selectRequest = 'selectRequest_' + i;
                if ($('#' + selectRequest).attr("sysno") == -1) {
                    $('#' + selectRequest).addClass("intxtfocus").focus();
                    return false;
                }
                selectReason = 'selectReason_' + i;
                if ($('#' + selectReason).attr("sysno") == -1) {
                    $('#' + selectReason).addClass("intxtfocus").focus();
                    return false;
                }
                var reasonTxt = 'reasonTxt_' + i;
                if ($String.Trim($('#' + reasonTxt).val()) == "") {
                    $('#' + reasonTxt).addClass("intxtfocus").focus();
                    return false;
                }
            }
        }
        if (IsHaveChecked == false) {
            alert("请选择至少一个商品");
            return false;
        }

        if ($('#selectShipType').attr("sysno") == -1) {
            $('#selectShipType').addClass("intxtfocus").focus();
            return false;
        }
        if ($String.Trim($("#ExpressName").val()) == "") {
            $('#ExpressName').addClass("intxtfocus").focus();
            return false;
        }
        if ($String.Trim($("#PackageNumber").val()) == "") {
            $('#PackageNumber').addClass("intxtfocus").focus();
            return false;
        }
        //if ($("#Province").val() == 0) {
        //    $('#Province').addClass("intxtfocus").focus();
        //    return false;
        //}
        //if ($("#City").val() == 0) {
        //    $('#City').addClass("intxtfocus").focus();
        //    return false;
        //}
        //if ($("#District").val() == 0) {
        //    $('#District').addClass("intxtfocus").focus();
        //    return false;
        //}
        if ($("#shippingAddressContent").hasClass("step-box-cur")) {
            alert("请保存返还地址");
            return false;
        }

        if ($String.Trim($("#BackAddress").val()) == "") {
            $('#BackAddress').addClass("intxtfocus").focus();
            return false;
        }
        if ($String.Trim($("#BackTelephone").val()) == "") {
            $('#BackTelephone').addClass("intxtfocus").focus();
            return false;
        }
        if ($String.Trim($("#Contact").val()) == "") {
            $('#Contact').addClass("intxtfocus").focus();
            return false;
        }
        return true;
    }

    function createData() {
        var RegisterList = new Array();
        for (i = 0; i < SoItemCount; i++) {
            var checkProduct = "checkProduct_" + i;
            if ($("#" + checkProduct).attr("checked") == "checked") {
                selectRequest = 'selectRequest_' + i;
                selectReason = 'selectReason_' + i;
                Quantity = 'Quantity_' + i;
                var ProductSysNo = 'ProductSysNo_' + i;
                reasonTxt = 'reasonTxt_' + i;
                var RegisterItem = {
                    RequestType: $('#' + selectRequest).attr("sysno"),
                    RMAReason: $('#' + selectReason).attr("sysno"),
                    RMAReasonDesc: $('#' + reasonTxt).val(),
                    Quantity: $('#' + Quantity).val(),
                    ProductSysNo: $('#' + ProductSysNo).attr("sysno"),
                    SOItemType: $('#' + ProductSysNo).attr("itemtype")
                }
                RegisterList[i] = RegisterItem;
            }
        }

        var RMA_RequestInfo = {
            SOSysNo: $('#SOSysNo').attr("sysno"), //订单编号
            ShippingType: $('#selectShipType').attr("sysno"), //寄回方式
            ShipViaCode: $String.Trim($("#ExpressName").val()), //快递名称
            TrackingNumber: $String.Trim($("#PackageNumber").val()),
            //AreaSysNo: $("#District").val(), //返回地区
            AreaSysNo: $("#DistrictId").val(), //返回地区
            Address: $String.Trim($("#BackAddress").val()), //返还联系地址
            Phone: $String.Trim($("#BackTelephone").val()), //返还联系电话
            CustomerSysNo: $('#UserSysno').attr("sysno"), //顾客编号,
            Contact: $String.Trim($("#Contact").val()), //返还联系地址
            Registers: RegisterList
        };
        return Web.Utils.Json.ToJson(RMA_RequestInfo) ;
    }

    $("#SubmitRequestWait").click(function () {

        if (!validata())
            return;

        var strRMA_RequestInfo = createData();

        $.ajax({
            type: "post",
            dataType: "json",
            url: "/MemberAccount/CreateRMARequestInfo",
            timeout: 30000,
            data: { Action:'wait' , Data: escape(strRMA_RequestInfo) },
            cache: false,
            global: true,
            beforeSend: function (XMLHttpRequest) { },
            success: function (data) {
                if (data.Result == "s") {
                    window.location = $('#SuccessHref').attr("href") + data.RequestrSysno;
                }
            },
            complete: function (XMLHttpRequest, textStatus) { },
            error: function () { }
        });
    });

    $("#SubmitRequest").click(function () {

        if (!validata())
            return;

        var strRMA_RequestInfo = createData();

        $.ajax({
            type: "post",
            dataType: "json",
            url: "/MemberAccount/CreateRMARequestInfo",
            timeout: 30000,
            data: { Action:'submit', Data: escape(strRMA_RequestInfo)},
            cache: false,
            global: true,
            beforeSend: function (XMLHttpRequest) { },
            success: function (data) {
                if (data.Result == "s") {
                    window.location = $('#SuccessHref').attr("href") + data.RequestrSysno;
                }
            },
            complete: function (XMLHttpRequest, textStatus) { },
            error: function () { }
        });

    });

    $('#selectShipTypeArea').delegate('a', 'click', function () {
        var $this = $(this);
        if ($this.attr('sysno') != '3') {
            $('#ExpressName').val($this.text());
        } else {
            $('#ExpressName').val('');
        }
    })

});