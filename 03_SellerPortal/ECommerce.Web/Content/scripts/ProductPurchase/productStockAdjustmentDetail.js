var adjustDetailManager = {
    productGrid: null,
    init: function () {
        $('.bs-select').selectpicker({
            iconBase: 'fa',
            tickIcon: 'fa-check'
        });

        $('#form').bootstrapValidator({
            excluded: [],

            feedbackIcons: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {
                StockSysNo: {
                    validators: {
                        notEmpty: {
                            message: '请选择一个入库仓库！'
                        }
                    }
                }
            }
        }).on("success.form.bv", function (e) {
            if (!adjustDetailManager.checkInputProducts()) {
                return;
            }
            adjustDetailManager.saveAction();
        });


        productGrid = $.grid($("#productStockAdjustmentItem_Grid"), {
            columns: [
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false }
            ]
        });

        $("#btnDeleteProductItems").click(function () {
            productGrid.deleteSelectedRows();

        });

        //初始化商品选择模态窗口
        $("#productCommonModal").modal({
            show: false
        }).on("shown.bs.modal", function () {
            $.ajax({
                type: "GET",
                url: "/Product/ProductCommon",
                dataType: "html",
                success: function (data) {
                    $("#productCommonModal").find(".modal-content").html(data);
                }
            });
        }).on("hide.bs.modal", function (e) {
            $("#productCommonModal").find(".modal-content").html("");
        });
    },

    //Check操作:验证数量格式是否正确
    checkInputProducts: function () {
        var mainProductData = productGrid.getAllDatas();

        if (!mainProductData || mainProductData.length <= 0) {
            $.alert('请添加至少一个商品库存调整明细!');
            return false;
        }


        for (var i = 0; i < mainProductData.length; i++) {
            var adjustQty = $("input[id^=inputAdjustQty_" + mainProductData[i][1] + "]").val();
            if (!Number(adjustQty)) {
                $.alert('商品库存调整明细中，商品:' + mainProductData[i][3] + '，库存调整数量格式不正确，请检查！');
                return false;
            }
        }

        return true;
    },
    //验证是否有相同商品
    checkExistProducts: function (productSysNo, productTitle) {
        var exist = false;
        var datas = productGrid.getAllDatas();
        if (datas.length > 0) {
            for (var i = 0; i < datas.length; i++) {
                if (productSysNo == datas[i][1]) {
                    exist = true;
                    break;
                }
            }
        }
        if (exist) {
            $.alert('已经存在相同的商品，商品编号:' + productSysNo + ',商品名称:' + productTitle + ',不能重复添加!');
        }
        return exist;

    },

    buildAdjustProductList: function () {

        var list = [];
        var mainProductData = productGrid.getAllDatas();
        for (var i = 0; i < mainProductData.length; i++) {
            list.push({
                AdjustSysNo: $("input[name=SysNo]").val()
               , ProductSysNo: mainProductData[i][1]
               , AdjustQty: $("#inputAdjustQty_" + mainProductData[i][1]).val()
            });
        }
        return list;
    },

    selectProductCallback: function (data) {

        var errorMsg = '';
        if (data.length > 0) {
            for (var i = 0 ; i < data.length; i++) {
                if (adjustDetailManager.checkExistProducts(data[i].SysNo, data[i].ProductTitle)) {
                    continue;
                }
                //else if (data[i].ProductTradeType == 1) {
                //    errorMsg += '商品编号为' + data[i].SysNo + '的商品是自贸商品，不能创建库存调整单!<br/>';
                //    continue;
                //}
                productGrid.addRow(['<input type="checkbox"/>', data[i].SysNo, data[i].ProductID, data[i].ProductTitle, data[i].OnlineQty, Number(data[i].CurrentPrice).toFixed(2), '<input class="form-control" id="inputAdjustQty_' + data[i].SysNo + '" value="0"/>']);
            }
            if (errorMsg.length > 0) {
                $.alert(errorMsg);
            }
        }
    },

    //活动保存操作:验证
    save: function () {
        var $trs = $("#productStockAdjustmentItem_Grid tbody tr");
        var isVal = true;
        $.each($trs, function (i, n) {
            var num = $(this).find("td:eq(6)").find(":input").val();
            if (parseInt(num) > 999999) {
                isVal = false;
                return false;
            }
        });
        if (!isVal) {
            $.alert("请填写调整库存数量为1-999999这间的有效数字");
        }
        else {
            $('#form').bootstrapValidator('validate');
        }
    },
    //保存操作Ajax
    saveAction: function () {

        var isEdit = $("input[name=SysNo]").val().length > 0 ? true : false;
        var entity = $.buildEntity($("#form"));
        entity.SysNo = $("input[name=SysNo]").val().length <= 0 ? null : $("input[name=SysNo]").val();
        entity.AdjustItemList = adjustDetailManager.buildAdjustProductList();

        $.ajax({
            url: "/ProductPurchase/AjaxSaveProductStockAdjustmentInfo",
            type: "POST",
            dataType: "json",
            data: { info: $.toJSON(entity) },
            success: function (data) {
                if (data.Data && !data.Error) {
                    $.alert(isEdit ? '保存商品库存调整单信息成功!' : '创建商品库存调整单成功!', function () {
                        if (!isEdit) {
                            window.location = '/ProductPurchase/ProductStockAdjustmentList';
                        }
                        else {
                            window.location.reload();
                        }
                    });
                }
            }
        });
    },
    //提交审核操作
    submitAudit: function () {
        $.confirm('确定要进行提交审核操作吗?', function (r) {
            if (!adjustDetailManager.checkInputProducts()) {
                return;
            }
            if (r) {
                $.ajax({
                    url: "/ProductPurchase/AjaxSubmitAuditAdjust",
                    type: "POST",
                    dataType: "json",
                    data: { sysNo: $("input[name=SysNo]").val() },
                    success: function (data) {
                        if (data.Data) {
                            $.alert('操作成功', function () {
                                window.location.reload();
                            });
                        }

                    }
                });
            }
        });
    },
    abandon: function () {
        $.confirm('确定要进行作废操作吗?', function (r) {
            if (r) {
                $.ajax({
                    url: "/ProductPurchase/AjaxAbandonAdjust",
                    type: "POST",
                    dataType: "json",
                    data: { sysNo: $("input[name=SysNo]").val() },
                    success: function (data) {
                        if (data.Data) {
                            $.alert('操作成功', function () {
                                window.location.reload();
                            });
                        }

                    }
                });
            }
        });
    },
    auditPass: function () {
        $.confirm('确定要进行审核通过操作吗?', function (r) {
            if (!adjustDetailManager.checkInputProducts()) {
                return;
            }
            if (r) {
                $.ajax({
                    url: "/ProductPurchase/AjaxAuditPassAdjust",
                    type: "POST",
                    dataType: "json",
                    data: { sysNo: $("input[name=SysNo]").val() },
                    success: function (data) {
                        if (data.Data) {
                            $.alert('操作成功', function () {
                                window.location.reload();
                            });
                        }

                    }
                });
            }
        });
    },
    auditFailed: function () {
        $.confirm('确定要进行审核拒绝操作吗?', function (r) {
            if (r) {
                $.ajax({
                    url: "/ProductPurchase/AjaxAuditFailedAdjust",
                    type: "POST",
                    dataType: "json",
                    data: { sysNo: $("input[name=SysNo]").val() },
                    success: function (data) {
                        if (data.Data) {
                            $.alert('操作成功', function () {
                                window.location.reload();
                            });
                        }

                    }
                });
            }
        });
    }
}

//显示商品选择Modal
function showProductCommonModal() {
    //显示模态窗口
    $("#productCommonModal").modal("show");
}

//商品选择回调地址
function productCommonCallback(data) {
    adjustDetailManager.selectProductCallback(data);
}