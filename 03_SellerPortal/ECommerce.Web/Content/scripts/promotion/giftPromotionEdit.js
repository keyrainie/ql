var giftPromotionEdit = {
    mainProductGrid: null,
    giftProductGrid: null,
    currentSelectedtype: null,
    actionType: 0,
    init: function () {
        $('.bs-select').selectpicker({
            iconBase: 'fa',
            tickIcon: 'fa-check'
        });

        $('#condForm').bootstrapValidator({
            excluded: [],
            feedbackIcons: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {
                Title: {
                    validators: {
                        notEmpty: {
                            message: '活动名称不能为空！'
                        }
                    }
                },
                Type: {
                    validators: {
                        notEmpty: {
                            message: '请选择一个活动类型！'
                        }
                    }
                },
                PromotionLink: {
                    validators: {
                        regexp: {
                            regexp: /(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?/,
                            message: '活动链接Url格式错误!'
                        }
                    }
                },
                BeginDate: {
                    validators: {
                        notEmpty: {
                            message: '活动生效时间不能为空！'
                        }
                    }

                },
                EndDate: {
                    validators: {
                        notEmpty: {
                            message: '活动结束时间不能为空！'
                        }
                    }
                },
                Description: {
                    validators: {
                        notEmpty: {
                            message: '活动描述不能为空！'
                        }
                    }
                },
            }
        }).on("success.form.bv", function (e) {
            if (!giftPromotionEdit.checkInputGiftProducts()) {
                return;
            }
            giftPromotionEdit.doAction();
        });



        $('#divBeginDate').datepicker({
            rtl: Metronic.isRTL(),
            orientation: "left",
            autoclose: true,
            language: "zh-CN"
        }).on("changeDate", function () {
            $("#condForm").data("bootstrapValidator").revalidateField("BeginDate");
        });
        $('#divEndDate').datepicker({
            rtl: Metronic.isRTL(),
            orientation: "left",
            autoclose: true,
            language: "zh-CN"
        }).on("changeDate", function () {
            $("#condForm").data("bootstrapValidator").revalidateField("EndDate");
        });


        mainProductGrid = $.grid($("#mainProductGrid"), {

            columns: [
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false }
            ]
        });

        giftProductGrid = $.grid($("#giftProductGrid"), {
            columns: [
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false },
                        { orderable: false }
            ]
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

        $("#btnDeleteGiftItems").click(function () {
            giftProductGrid.deleteSelectedRows();

        });
        $("#btnDeleteProductItems").click(function () {
            mainProductGrid.deleteSelectedRows();

        });

        //$("#checkboxIsGlobalProduct").change(function () {
        //    if ($("#checkboxIsGlobalProduct").attr('checked')) {
        //        $("#divProductRule").hide();
        //    }
        //    else {
        //        $("#divProductRule").show();
        //    }
        //});

        $("#selectRuleType").change(function () {

            if ($("#selectRuleType").val() == '0') {
                $("#divProductRule").hide();
            }
            else {
                $("#divProductRule").show();
            }
        });

        $("#selectActivityType").change(function () {

            if ($("#selectActivityType").val() == '2') {
                $("input[id=OrderMinAmount]").removeAttr('readonly');
                $("#selectRule").removeAttr('disabled');
                $("#selectRule").val('0');
                $("#selectRuleType").val('0');
                $("#selectRuleType").removeAttr('disabled');
                $("#selectRuleType").change();
                $('.bs-select').selectpicker('refresh');
            }
            else {
                $("input[id=OrderMinAmount]").attr('readonly', 'readonly');
                $("input[id=OrderMinAmount]").val('');
                $("#selectRuleType").val('1');
                $("#selectRuleType").attr('disabled', 'disabled');
                $("#selectRuleType").change();
                $("#divProductRule").show();
                $("#selectRule").attr('disabled', 'disabled');
                $("#selectRule option[value=0]").attr("selected", "selected");
                $("#selectRule").change();
            }
        });
    },

    doAction: function () {
        var entity = $.buildEntity($("#condForm"));
        entity.OrderMinAmount = $('input[id=OrderMinAmount]').val();
        if ($("#selectActivityType").val() == '2') {
            if (!entity.OrderMinAmount || entity.OrderMinAmount.length <= 0) {
                $.alert('请输入门槛金额!');
                $('input[id=OrderMinAmount]').focus();
                return;
            }
            else {
                var reg = /^[0-9]+\.{0,1}[0-9]{0,2}$/;
                var r = entity.OrderMinAmount.match(reg);
                if (!r || (Number(entity.OrderMinAmount) != undefined && Number(entity.OrderMinAmount) <= 0)) {
                    $.alert('门槛金额格式不正确，请重新输入!');
                    $('input[id=OrderMinAmount]').focus();
                    return;
                }
            }
        }
        var isEdit = $("input[name=SysNo]").val().length > 0 ? true : false;
        entity.IsGlobalProduct = $("#selectRuleType").val() == '0' ? true : false;
        switch (actionType) {
            case 0:
                //保存（新建）操作:
                $.confirm((isEdit ? '确定要进行该活动信息的更新' : '确定要进行该活动信息的创建') + '操作吗?', function (r) {
                    if (r) {
                        entity.ProductRuleList = giftPromotionEdit.buildMainProductsRulesList();
                        entity.GiftRuleList = giftPromotionEdit.buildGiftProductsRulesList();
                        $.ajax({
                            url: "/Promotion/SaveGiftPromotion",
                            type: "POST",
                            dataType: "json",
                            data: { giftInfo: $.toJSON(entity) },
                            success: function (data) {
                                if (data.Data && data.Data != 0) {
                                    $.alert(isEdit ? '保存赠品活动信息成功!' : '创建赠品活动成功!', function () {
                                        if (!isEdit) {
                                            window.location = '/Promotion/GiftPromotionEdit?sysNo=' + data.Data.toString();
                                        }
                                        else {
                                            window.location.reload();
                                        }
                                    });
                                }

                            }
                        });
                    }
                });
                break;
            case 1:
                //发布:
                $.confirm('确定要提交审核该活动吗?', function (r) {
                    if (r) {
                        $.ajax({
                            url: "/Promotion/PublishGiftPromotion",
                            type: "POST",
                            dataType: "json",
                            data: { giftPromotionSysNo: entity.SysNo },
                            success: function (data) {
                                if (data.Data) {
                                    $.alert('提交审核操作成功!', function () {
                                        location.reload();
                                    });
                                }
                            }
                        });

                    }
                });
                break;
            case 2:
                //作废:
                $.confirm('确定要作废该活动吗?', function (r) {
                    if (r) {
                        $.ajax({
                            url: "/Promotion/AbandonGiftPromotion",
                            type: "POST",
                            dataType: "json",
                            data: { giftPromotionSysNo: entity.SysNo },
                            success: function (data) {
                                if (data.Data) {
                                    $.alert('作废活动操作成功!', function () {
                                        location.reload();
                                    });
                                }
                            }
                        });
                    }
                });
                break;
            case 3:
                //终止:
                $.confirm('确定要终止该活动吗?', function (r) {
                    if (r) {
                        $.ajax({
                            url: "/Promotion/TerminateGiftPromotion",
                            type: "POST",
                            dataType: "json",
                            data: { giftPromotionSysNo: entity.SysNo },
                            success: function (data) {
                                if (data.Data) {
                                    $.alert('终止活动操作成功!', function () {
                                        location.reload();
                                    });
                                }
                            }
                        });
                    }
                });
                break;
            default:
                break;
        }
    },

    //活动保存操作:
    save: function () {
        actionType = 0;
        $('#condForm').bootstrapValidator('validate');
    },
    //活动发布操作:
    publish: function () {
        actionType = 1;
        giftPromotionEdit.doAction();
    },
    //活动作废操作:
    abandon: function () {
        actionType = 2;
        giftPromotionEdit.doAction();
    },
    //活动终止操作:
    terminate: function () {
        actionType = 3;
        giftPromotionEdit.doAction();
    },

    //显示商品选择Modal
    showProductCommonModal: function (type) {
        currentSelectedtype = type;
        //显示模态窗口
        $("#productCommonModal").modal("show");
    },

    //商品选择回调地址
    selectedProductsCallback: function (data) {

        if (currentSelectedtype == 0) {
            //添加主商品:
            if (data.length > 0) {
                for (var i = 0 ; i < data.length; i++) {
                    if (giftPromotionEdit.checkExistProducts(currentSelectedtype, data[i].SysNo, data[i].ProductTitle)) {
                        continue;
                    }
                    mainProductGrid.addRow(['<input type="checkbox"/>', data[i].SysNo, data[i].ProductID, data[i].ProductTitle, data[i].StatusString]);
                }
            }
        }
        else if (currentSelectedtype == 1) {
            //添加赠品:
            if (data.length > 0) {
                for (var i = 0 ; i < data.length; i++) {
                    if (giftPromotionEdit.checkExistProducts(currentSelectedtype, data[i].SysNo, data[i].ProductTitle)) {
                        continue;
                    }
                    giftProductGrid.addRow(['<input type="checkbox"/>', data[i].SysNo, data[i].ProductID, data[i].ProductTitle, data[i].StatusString, '<input class="form-control" id="inputGiftPlusPrice_' + data[i].SysNo + '" value=""  onblur="giftPromotionEdit.inputNumber(this);"/>']);
                }
            }
        }
    },



    buildMainProductsRulesList: function () {

        var list = [];
        var mainProductData = mainProductGrid.getAllDatas();
        for (var i = 0; i < mainProductData.length; i++) {
            list.push({
                ProductSysNo: mainProductData[i][1]
                , Type: 'I'
                , ProductID: mainProductData[i][2]
                , ProductName: mainProductData[i][3]
                , ComboType: !$("#selectRuleType").val() == '0' ? $("#selectRule").val() : 0
            });
        }
        return list;
    },
    buildGiftProductsRulesList: function () {
        var list = [];
        var giftProductData = giftProductGrid.getAllDatas();
        for (var i = 0; i < giftProductData.length; i++) {
            list.push({
                ProductSysNo: giftProductData[i][1]
                , ProductID: giftProductData[i][2]
                , ProductName: giftProductData[i][3]
                , Count: $("#inputGiftPlusPrice_" + giftProductData[i][1]).val()
            });
        }
        return list;
    },
    //Check操作:
    checkInputGiftProducts: function () {
        var mainProductData = mainProductGrid.getAllDatas();
        var giftProductData = giftProductGrid.getAllDatas();
        if ($("#selectRuleType").val() == '0' && mainProductData.length > 0) {
            $.alert('当选择主商品为"整网商品"时，无需再指定任何的主商品规则，请删除后再操作！');
            return false;
        }
        if ((!mainProductData || mainProductData.length <= 0) && !($("#selectRuleType").val() == '0')) {
            $.alert('请添加至少一个主商品规则!');
            return false;
        }

        if ($("#selectActivityType").val() == '0' && mainProductData.length > 1) {
            $.alert('当活动类型为"单品买赠"时，只能设置一个主商品规则，请修改!');
            return false;
        }

        if (!giftProductData || giftProductData.length <= 0) {
            $.alert('请添加至少一个赠品规则!');
            return false;
        }
        if (giftProductData.length > 0) {
            for (var i = 0; i < giftProductData.length; i++) {
                var plusPrice = $("#inputGiftPlusPrice_" + giftProductData[i][1]).val();
                if (!Number(plusPrice) || Number(plusPrice) <= 0 || Number(plusPrice) > 9999) {
                    $.alert('赠品规则中，商品:' + giftProductData[i][3] + '，数量格式不正确，请检查，数量至少是1,最大是9999！');
                    return false;
                }
            }
        }
        return true;
    },
    checkExistProducts: function (type, productSysNo, productTitle) {
        var exist = false;
        var datas;
        if (type == 0) {
            datas = mainProductGrid.getAllDatas();

        }
        else if (type == 1) {
            datas = giftProductGrid.getAllDatas();
        }
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
    inputNumber: function (src) {
        if ($(src).attr('readonly') == 'readonly') {
            return;
        }
        var val = $(src).val();
        if (Number(val)) {
            $(src).val(Number(val).toFixed(0));
            if (Number(val) <= 0) {
                $(src).val('1');
            }
            if (Number(val) > 9999) {
                $(src).val('9999');
            }
        }
        else {
            $(src).val('1');
        }
    }
}





