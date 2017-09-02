$(document).ready(function () {
    $('.date-picker').datepicker({
        rtl: Metronic.isRTL(),
        orientation: "left",
        autoclose: true,
        language: "zh-CN"
    });

    $('#MaintainEntryForm').bootstrapValidator({
        message: '输入值无效！',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            //ProductSKUNO: {
            //    validators: {
            //        notEmpty: { message: '请输入货号！' }
            //    }
            //},
            //SuppliesSerialNo: {
            //    validators: {
            //        notEmpty: { message: '请输入物资序号！' }
            //    }
            //},
            ApplyUnit: {
                validators: {
                    //notEmpty: { message: '请输入申报单位！' },
                    stringLength: {
                        min: 1,
                        max: 3,
                        message: '申报单位最多输入3位！'
                    }
                }
            },
            ApplyQty: {
                validators: {
                    //notEmpty: { message: '请输入申报数量！' },
                    regexp: { regexp: /^[0-9]*$/, message: '返现只能输入正整数！' }
                }
            }
        }
    }).on("success.form.bv", function (e) {
        if (EntryInfo.SaveType == 1) {
            EntryInfo.SaveEntryInfo();
        }
        else if (EntryInfo.SaveType == 2) {
            EntryInfo.SaveAndSubmitAudit();
        }
    });
    $(".EditEntryInfo").click(function () {
        $('#MaintainEntryForm').data('bootstrapValidator').resetForm(true);
        EntryInfo.EditEntryInfo(parseInt($(this).attr("ProductSysNo")));
    });
    $("#SaveEntryInfo").click(function () {
        EntryInfo.SaveType = 1;
        $('#MaintainEntryForm').bootstrapValidator('validate');
    });
    $("#SubmitEntryInfo").click(function () {
        EntryInfo.SaveType = 2;
        $('#MaintainEntryForm').bootstrapValidator('validate');
    });
    if (EntryInfo.InitProductSysNo > 0) {
        EntryInfo.EditEntryInfo(EntryInfo.InitProductSysNo);
    }
});

var EntryInfo = {
    InitProductSysNo: 0,
    //1-保存，2-保存并提交申请
    SaveType: 1,
    //编辑备案信息
    EditEntryInfo: function (sysNo) {
        EntryInfo.SaveType = 1;
        $.ajax({
            url: "AjaxGetProductEntryInfo?sysno=" + sysNo,
            type: "POST",
            dataType: "json",
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    if (data.ManufactureDate != null) {
                        data.ManufactureDate = data.ManufactureDate.replace(" 0:00:00", "");
                    }
                    $.bindEntity($("#MaintainEntryForm"), data);
                    $("#EntryStatusPanel").html(data.EntryStatusText);
                    if (data.EntryStatus == -1 || data.EntryStatus == 0) {
                        $("#SaveEntryInfo").show();
                        $("#SubmitEntryInfo").show();
                    }
                    else {
                        $("#SaveEntryInfo").hide();
                        $("#SubmitEntryInfo").hide();
                    }
                    $("#EntryStatusPanel").removeClass("label-info");
                    $("#EntryStatusPanel").removeClass("label-warning");
                    $("#EntryStatusPanel").removeClass("label-danger");
                    $("#EntryStatusPanel").removeClass("label-success");
                    switch (data.EntryStatus) {
                        case -2:
                            $("#EntryStatusPanel").addClass("label-danger");
                            break;
                        case -1:
                        case 0:
                            $("#EntryStatusPanel").addClass("label-warning");
                            break;
                        case 4:
                            $("#EntryStatusPanel").addClass("label-success");
                            break;
                        default:
                            $("#EntryStatusPanel").addClass("label-info");
                            break;
                    }
                    Metronic.updateUniform();
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    },
    //保存备案信息
    SaveEntryInfo: function () {
        var data = $.buildEntity($("#MaintainEntryForm"));
        if (data.ProductSysNo <= 0) {
            $.alert("请选择商品！");
            return false;
        }
        //if (data.ManufactureDate.length == 0) {
        //    $.alert("请选择出厂日期！");
        //    return false;
        //}
        $.confirm("您确定要保存备案信息吗？", function (result) {
            if (!result)
                return;
            $.ajax({
                url: "AjaxSaveEntryInfo",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(data)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.confirm("保存备案信息成功！", function () {
                            $('#MaintainEntryForm')[0].reset();
                            $("input[data-model=ProductSysNo]").val(0);
                            $('#MaintainEntryForm').data('bootstrapValidator').resetForm(true);
                            $("#SaveEntryInfo").hide();
                            $("#SubmitEntryInfo").hide();
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    $.hideLoading();
                }
            });
        });
    },
    //保存并提交申请
    SaveAndSubmitAudit: function () {
        var data = $.buildEntity($("#MaintainEntryForm"));
        if (data.ProductSysNo <= 0) {
            $.alert("请选择商品！");
            return false;
        }
        //if (data.ManufactureDate.length == 0) {
        //    $.alert("请选择出厂日期！");
        //    return false;
        //}
        $.confirm("您确定要保存并提交备案申请吗？", function (result) {
            if (!result)
                return;
            $.ajax({
                url: "AjaxSubmitProductEntryAudit",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(data)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.confirm("保存并提交备案申请成功！", function () {
                            $('#MaintainEntryForm')[0].reset();
                            $("input[data-model=ProductSysNo]").val(0);
                            $('#MaintainEntryForm').data('bootstrapValidator').resetForm(true);
                            $("#SaveEntryInfo").hide();
                            $("#SubmitEntryInfo").hide();
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