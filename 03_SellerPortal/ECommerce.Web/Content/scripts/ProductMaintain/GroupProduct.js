$(document).ready(function () {
    $("#SaveInfo").click(function () {
        GroupProduct.CreateGroupProduct();
    });
});

var GroupProduct = {
    ProductGroupSysNo: 0,
    //保存选中的商品
    CreateGroupProduct: function () {
        var p = [];
        $("div[Property=Group]").each(function () {
            var curr = [];
            $(this).find("input").each(function () {
                if ($(this).attr("checked") == "checked") {
                    curr.push({
                        PropertyGroupName: $(this).attr("PropertyGroupName"),
                        PropertySysNo: $(this).attr("PropertySysNo"),
                        PropertyName: $(this).attr("PropertyName"),
                        ValueSysNo: $(this).val(),
                        ValueName: $(this).attr("ValueName")
                    });
                }
            });
            if (curr.length > 0) {
                p.push(curr);
            }
        });
        if (p.length == 0) {
            $.alert("请选择分组属性！");
            return;
        }
        var data = [];
        if (p.length == 1) {
            for (var i = 0; i < p[0].length; i++) {
                data.push({
                    ProductGroupSysNo: GroupProduct.ProductGroupSysNo,
                    PropertyGroupName: p[0][i].PropertyGroupName,
                    PropertySysNo1: p[0][i].PropertySysNo,
                    PropertyName1: p[0][i].PropertyName,
                    ValueSysNo1: p[0][i].ValueSysNo,
                    ValueName1: p[0][i].ValueName
                });
            }
        }
        else {
            for (var i = 0; i < p[0].length; i++) {
                for (var j = 0; j < p[1].length; j++) {
                    data.push({
                        ProductGroupSysNo: GroupProduct.ProductGroupSysNo,
                        PropertyGroupName: p[0][i].PropertyGroupName,
                        PropertySysNo1: p[0][i].PropertySysNo,
                        PropertyName1: p[0][i].PropertyName,
                        ValueSysNo1: p[0][i].ValueSysNo,
                        ValueName1: p[0][i].ValueName,
                        PropertySysNo2: p[1][j].PropertySysNo,
                        PropertyName2: p[1][j].PropertyName,
                        ValueSysNo2: p[1][j].ValueSysNo,
                        ValueName2: p[1][j].ValueName
                    });
                }
            }
        }

        $.ajax({
            url: "AjaxCreateGroupProduct",
            type: "POST",
            dataType: "json",
            data: { data: encodeURI(JSON.stringify(data)) },
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    $.confirm("保存商品成功！", function () {
                        location.href = 'GroupProduct?ProductGroupSysNo=' + GroupProduct.ProductGroupSysNo;
                    });
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    },
    //上架
    Online: function (productSysNo) {
        var productSysNoList = [];
        productSysNoList.push(productSysNo);

        $.confirm("您确定要上架该商品吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "/ProductMaintain/AjaxProductBatchOnline",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(productSysNoList)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.alert("上架商品成功！", function () {
                            location.reload();
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    $.hideLoading();
                }
            });
        });
    },
    //上架不展示
    OnlineNotShow: function (productSysNo) {
        var productSysNoList = [];
        productSysNoList.push(productSysNo);

        $.confirm("您确定要上架不展示该商品吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "/ProductMaintain/AjaxProductBatchOnlineNotShow",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(productSysNoList)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.alert("上架不展示商品成功！", function () {
                            location.reload();
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    $.hideLoading();
                }
            });
        });
    },
    //下架
    Offline: function (productSysNo) {
        var productSysNoList = [];
        productSysNoList.push(productSysNo);
        
        $.confirm("您确定要下架该商品吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "/ProductMaintain/AjaxProductBatchOffline",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(productSysNoList)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.alert("下架商品成功！", function () {
                            location.reload();
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    $.hideLoading();
                }
            });
        });
    },
    //作废
    Abandon: function (productSysNo) {
        var productSysNoList = [];
        productSysNoList.push(productSysNo);

        $.confirm("您确定要作废该商品吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "/ProductMaintain/AjaxProductBatchAbandon",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(productSysNoList)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.alert("作废商品成功！", function () {
                            location.reload();
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