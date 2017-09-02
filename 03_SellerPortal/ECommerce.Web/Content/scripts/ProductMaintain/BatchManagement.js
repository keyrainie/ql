$(document).ready(function () {
    //$('.bs-select').selectpicker({
    //    iconBase: 'fa',
    //    tickIcon: 'fa-check'
    //});
    $("#IsCollectBatchNo").attr("disabled", "disabled");
    $('#MaintainBatchManagementForm').bootstrapValidator({
        message: '输入值无效！',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            MinReceiptDays: {
                validators: {
                    notEmpty: { message: '请输入最小收货天数！' },
                    regexp: { regexp: /^\d{0,4}?$/, message: '仅阿拉伯数字、整数可填，最多9999，单位“天”！' }
                }
            },
            MaxDeliveryDays: {
                validators: {
                    notEmpty: { message: '请输入最大可发货天数！' },
                    regexp: { regexp: /^\d{0,4}?$/, message: '仅阿拉伯数字、整数可填，最多9999，单位“天”！' }
                }
            },
            GuaranteePeriodYear: {
                validators: {
                    notEmpty: { message: '请输入保质期年！' },
                    regexp: { regexp: /^\d{0,4}?$/, message: '仅阿拉伯数字、整数可填，最多9999，单位“年”！' }
                }
            },
            GuaranteePeriodMonth: {
                validators: {
                    notEmpty: { message: '请输入保质期月！' },
                    regexp: { regexp: /^\d{0,4}?$/, message: '仅阿拉伯数字、整数可填，最多9999，单位“月”！' }
                }
            },
            GuaranteePeriodDay: {
                validators: {
                    notEmpty: { message: '请输入保质期日！' },
                    regexp: { regexp: /^\d{0,4}?$/, message: '仅阿拉伯数字、整数可填，最多9999，单位“日”！' }
                }
            },
            Note: {
                validators: {
                    //notEmpty: { message: '请输入备注！' },
                    stringLength: { max: 10000, message: '最多10000个汉字！' }
                }
            }
        }
    }).on("success.form.bv", function (e) {
        BatchManagementInfo.SaveBatchManagementInfo();
    });
    $(".EditBatchManagementInfo").click(function () {
        $('#MaintainBatchManagementForm').data('bootstrapValidator').resetForm(true);
        BatchManagementInfo.EditBatchManagementInfo(parseInt($(this).attr("ProductSysNo")));
    });
    $("#SaveBatchManagementInfo").click(function () {
        $('#MaintainBatchManagementForm').bootstrapValidator('validate');
    });
    $("#IsBatch").click(function () {
        if ($(this).attr("checked") == "checked") {
            $("#MaintainBatchManagementForm").find(".readonly").attr("disabled", false);
            $("#IsCollectBatchNo").removeAttr("disabled");
        }
        else {
            $("#MaintainBatchManagementForm").find(".readonly").attr("disabled", true);
            $("#IsCollectBatchNo").attr("disabled", "disabled");
        }
    });
});

var BatchManagementInfo = {
    //编辑批号信息
    EditBatchManagementInfo: function (sysNo) {
        $.ajax({
            url: "AjaxGetBatchManagementInfo?sysno=" + sysNo,
            type: "POST",
            dataType: "json",
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    if (data.IsBatch == true) {
                        $("#IsBatch").attr("checked", "checked");
                        $("#IsBatch").parent().addClass("checked");
                        $(".readonly").attr("disabled", false);
                    }
                    else {
                        $("#IsBatch").attr("checked", false);
                        $("#IsBatch").parent().removeClass("checked");
                        $(".readonly").attr("disabled", true);
                    }
                    if (data.IsCollectBatchNo == true) {
                        $("#IsCollectBatchNo").removeAttr("disabled");
                        $("#IsCollectBatchNo").attr("checked", "checked");
                        $("#IsCollectBatchNo").parent().addClass("checked");
                    }
                    else {
                        $("#IsCollectBatchNo").attr("checked", false);
                        $("#IsCollectBatchNo").parent().removeClass("checked");
                    }
                    $.bindEntity($("#MaintainBatchManagementForm"), data);
                    $("#SaveBatchManagementInfo").show();
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    },
    //保存批号信息
    SaveBatchManagementInfo: function () {
        var data = [];
        data.push($.buildEntity($("#MaintainBatchManagementForm")));
        if (data[0].ProductSysNo <= 0) {
            $.alert("请选择商品！");
            return false;
        }
        if ($("#IsBatch").attr("checked") == "checked") {
            data[0].IsBatch = true;
        }
        else {
            data[0].IsBatch = false;
        }
        if ($("#IsCollectBatchNo").attr("checked") == "checked") {
            data[0].IsCollectBatchNo = true;
        }
        else {
            data[0].IsCollectBatchNo = false;
        }
        if (parseInt(data[0].MinReceiptDays) > 9999) {
            $.alert("最小收货天数,最多9999！");
            return false;
        }
        if (parseInt(data[0].MaxDeliveryDays) > 9999) {
            $.alert("最大可发货天数,最多9999！");
            return false;
        }
        if (parseInt(data[0].GuaranteePeriodYear) > 9999) {
            $.alert("保质期年,最多9999！");
            return false;
        }
        if (parseInt(data[0].GuaranteePeriodMonth) > 9999) {
            $.alert("保质期月,最多9999！");
            return false;
        }
        if (parseInt(data[0].GuaranteePeriodDay) > 9999) {
            $.alert("保质日,最多9999！");
            return false;
        }
        if ($.trim(data[0].CollectType).length == 0) {
            $.alert("请选择付款类型！");
            return false;
        }
        $.confirm("您确定要保存价格信息吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "AjaxSaveBatchManagementInfo",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(data)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.confirm("保存批号信息成功！", function () {
                            if (data.IsBatch == true) {
                                $("#IsBatch").attr("checked", true);
                                $("#IsBatch").parent().addClass("checked");
                                $(".readonly").attr("disabled", false);
                            }
                            else {
                                $("#IsBatch").attr("checked", false);
                                $("#IsBatch").parent().removeClass("checked");
                                $(".readonly").attr("disabled", true);
                            }
                            if (data.IsCollectBatchNo == true) {
                                $("#IsCollectBatchNo").attr("checked", true);
                                $("#IsCollectBatchNo").parent().addClass("checked");
                            }
                            else {
                                $("#IsCollectBatchNo").attr("checked", false);
                                $("#IsCollectBatchNo").parent().removeClass("checked");
                            }
                            $.bindEntity($("#MaintainBatchManagementForm"), data);
                            $("#SaveBatchManagementInfo").hide();
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    $.hideLoading();
                }
            });
        });
    }
}