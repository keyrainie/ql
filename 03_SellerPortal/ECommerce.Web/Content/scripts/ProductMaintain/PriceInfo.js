$(document).ready(function () {
    //$('.bs-select').selectpicker({
    //    iconBase: 'fa',
    //    tickIcon: 'fa-check'
    //});
    $('#MaintainPriceForm').bootstrapValidator({
        message: '输入值无效！',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            BasicPrice: {
                validators: {
                    notEmpty: { message: '请输入市场价！' },
                    regexp: { regexp: /^\d{0,8}\.{0,1}(\d{1,2})?$/, message: '市场价只能输入大于等于0的整数或者2位小数！' }
                }
            },
            CurrentPrice: {
                validators: {
                    notEmpty: { message: '请输入销售价！' },
                    regexp: { regexp: /^\d{0,8}\.{0,1}(\d{1,2})?$/, message: '销售价只能输入大于等于0的整数或者2位小数！' }
                }
            },
            VirtualPrice: {
                validators: {
                    notEmpty: { message: '请输入采购价！' },
                    regexp: { regexp: /^\d{0,8}\.{0,1}(\d{1,2})?$/, message: '采购价只能输入大于等于0的整数或者2位小数！' }
                }
            },
            UnitCost: {
                validators: {
                    notEmpty: { message: '请输入成本价！' },
                    regexp: { regexp: /^\d{0,8}\.{0,1}(\d{1,2})?$/, message: '成本价格只能输入大于等于0的整数或者2位小数！' }
                }
            },
            UnitCostWithoutTax: {
                validators: {
                    notEmpty: { message: '请输入去税成本价！' },
                    regexp: { regexp: /^\d{0,8}\.{0,1}(\d{1,2})?$/, message: '去税成本价只能输入大于等于0的整数或者2位小数！' }
                }
            },
            CashRebate: {
                validators: {
                    notEmpty: { message: '请输入返现！' },
                    regexp: { regexp: /^[0-9]*$/, message: '返现只能输入正整数！' }
                }
            },
            Point: {
                validators: {
                    notEmpty: { message: '请输入增送积分！' },
                    regexp: { regexp: /^[0-9]*$/, message: '赠送积分只能输入正整数！' }
                }
            },
            MaxPerOrder: {
                validators: {
                    notEmpty: { message: '请输入每单限购数！' },
                    regexp: { regexp: /^[0-9]*[1-9][0-9]*$/, message: '每单限购数只能输入正整数！' }
                }
            },
            MinCountPerOrder: {
                validators: {
                    notEmpty: { message: '请输入最小订购数！' },
                    regexp: { regexp: /^[0-9]*[1-9][0-9]*$/, message: '最小订购数只能输入正整数！' }
                }
            }
        }
    }).on("success.form.bv", function (e) {
        PriceInfo.SavePriceInfo();
    });
    $(".EditPriceInfo").click(function () {
        $('#MaintainPriceForm').data('bootstrapValidator').resetForm(true);
        $("#ProductTitle").val($(this).attr("ProductTitle"));
        PriceInfo.EditPriceInfo(parseInt($(this).attr("ProductSysNo")));
    });
    $("#SavePriceInfo").click(function () {
        $('#MaintainPriceForm').bootstrapValidator('validate');
    });
});

var PriceInfo = {
    //编辑价格信息
    EditPriceInfo: function (sysNo) {
        $.ajax({
            url: "AjaxGetProductPriceInfo?sysno=" + sysNo,
            type: "POST",
            dataType: "json",
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    $.bindEntity($("#MaintainPriceForm"), data);
                    $("#PointType").val("0");
                    $("#SavePriceInfo").show();
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    },
    //保存价格信息
    SavePriceInfo: function () {
        var data = [];
        data.push($.buildEntity($("#MaintainPriceForm")));
        if (data[0].ProductSysNo <= 0) {
            $.alert("请选择商品！");
            return false;
        }
        if (parseFloat(data[0].CurrentPrice) <= 0) {
            $.alert("销售价必须大于0！");
            return false;
        }
        if (parseInt(data[0].MinCountPerOrder) > parseInt(data[0].MaxPerOrder)) {
            $.alert("每单限购数必须大于等于最小订购数！");
            return false;
        }
        if ($.trim(data[0].PointType).length == 0) {
            $.alert("请选择付款类型！");
            return false;
        }
        if (parseFloat(data[0].MaxPerOrder) >= 999999) {
            $.alert("请输出合理的最大订购数！");
            return false;
        }
        if (data[0].PointType == ""){
            $.alert("请选择付款类型！");
            return false;
        }
        $.confirm("您确定要保存价格信息吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "AjaxSavePriceInfo",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(data)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.confirm("保存价格信息成功！", function () {
                            $('#MaintainPriceForm')[0].reset();
                            $("input[data-model=ProductSysNo]").val(0);
                            $('#MaintainPriceForm').data('bootstrapValidator').resetForm(true);
                            $("#SavePriceInfo").hide();
                            window.location.reload();
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