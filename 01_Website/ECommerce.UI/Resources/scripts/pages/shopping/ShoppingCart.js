$(function () {
    $(".pop_exchange .btn_gray24").live("click", function () {
        $(this).parent().parent().hide().removeClass("pop_exchange_show");
    });
});
//购物车操作
var ShoppingCart = {
    //收藏商品
    AddFavorites: function (sysno) {
        $.utility.settings.showLoading = false;
        $.ajax({
            url: "/home/checklogin",
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data > 0) {
                    $.ajax({
                        url: $("#AddFavoritesUrl").val() + "?ProductSysNo=" + sysno,
                        type: "POST",
                        dataType: "json",
                        beforeSend: function (XMLHttpRequest) {
                            ShoppingCart.ShowMaskLayer();
                        },
                        success: function (data) {
                            if (!data) {
                                alert("收藏成功！");
                            }
                            else {
                                alert(data.message);
                            }
                        },
                        complete: function (XMLHttpRequest, textStatus) {
                            ShoppingCart.HideMaskLayer();
                        }
                    });
                }
                else {
                    //MiniLogin.Open();
                    ShoppingCart.GoToLoginAndSucessToCart();
                }
            }
        });        
    },
    //修改购物车
    ModifyCart: function (e){
        var action = $(e).attr("Action");
        switch (action) {
            case "ProductQtySubtract":
            case "ProductQtyUpdate":
            case "ProductQtyAdd":
                ShoppingCart.UpdateProduct(e, action);
                break;
            case "PackageQtySubtract":
            case "PackageQtyUpdate":
            case "PackageQtyAdd":
                ShoppingCart.UpdatePackage(e, action);
                break;
            case "DelProduct":
                ShoppingCart.DelProduct(e);
                break;
            case "DelPackageProduct":
                ShoppingCart.DelPackageProduct(e);
                break;
            case "DelPackage":
                ShoppingCart.DelPackage(e);
                break;
            case "DelGift":
                ShoppingCart.DelGift(e);
                break;
            case "SltGift":
                ShoppingCart.SltGift(e);
                break;
            case "DelSltGift":
                ShoppingCart.DelSltGift(e);
                break;
            case "DelOrderGift":
                ShoppingCart.DelOrderGift(e);
                break;
            case "DelOrderSltGift":
                ShoppingCart.DelOrderSltGift(e);
                break;
            case "SltOrderGift":
                ShoppingCart.SltOrderGift(e);
                break;
            case "SltPlusBuyProduct":
                ShoppingCart.SltPlusBuyProduct(e);
                break;
            case "DelPlusBuyProduct":
                ShoppingCart.DelPlusBuyProduct(e);
                break;
            case "ClearAll":
                ShoppingCart.ClearAll(e);
                break;
            case "ProductChecked":
                ShoppingCart.ProductChecked(e);
                break;
        }
    },
    //结算
    GotoCheckOut: function (e) {
        $.utility.settings.showLoading = false;
        $.ajax({
            url: "/home/checklogin",
            type: "POST",
            dataType: "json",
            success: function (data) {
                var checkOutUrl = $(e).attr("Url");
                if (data > 0) {
                    $("#CanSubmitBtn").hide();
                    $("#SumbitingPanel").show();
                    ShoppingCart.ShowMaskLayer();
                    //location.href = checkOutUrl;
                    var PackageTypeSingleList = getPackageTypeSingleList();
                    var PackageTypeGroupList = getPackageTypeGroupList();
                    if (PackageTypeSingleList == "" && PackageTypeGroupList == "") {
                        alert("请选择需要购买的商品！！！");
                        $("#SumbitingPanel").hide();
                        $("#CanSubmitBtn").show();
                        ShoppingCart.HideMaskLayer();
                    } else {
                        //ShoppingCart.GotoCheckOutUrl(PackageTypeSingleList, PackageTypeGroupList, checkOutUrl);
                        location.href = checkOutUrl + "?PackageTypeSingle=" + PackageTypeSingleList + "&PackageTypeGroup=" + PackageTypeGroupList;
                    }
                }
                else {
                    //MiniLogin.Open(checkOutUrl);                   
                    ShoppingCart.GoToLoginAndSucessToCheckout();
                }
            }
        });
    },
    //显示遮罩层
    ShowMaskLayer: function () {
        $("#shoppingCartPannelMask").css("width", $("#shoppingCartPannelMain").innerWidth().toString() + "px");
        $("#shoppingCartPannelMask").css("height", ($("#shoppingCartPannelMain").innerHeight() + 370).toString() + "px");
        $("#shoppingCartPannelMask").show();
    },
    //隐藏遮罩层
    HideMaskLayer: function (){
        $("#shoppingCartPannelMask").hide();
    },
    //更改购物车商品数量
    UpdateProduct: function (e, action) {
        var pattern = /^\d+$/;
        var currParentDom = $(e).parent();
        var currQty = currParentDom.find("input[type=text]").val();
        var oldQty = currParentDom.find("input[type=text]").attr("OldQty");
        if (!pattern.test(currQty)) {
            currParentDom.find("input[type=text]").val(oldQty);
            currParentDom.parent().find("#ModifyFailLabel").text("只能输入正整数");
            currParentDom.parent().find("#ModifyFailLabel").show();
            setTimeout(function () { currParentDom.find("#ModifyFailLabel").hide(); }, 2000);
            return;
        }
        currQty = parseInt(currQty);

        var minPerOrder = parseInt($(currParentDom).attr("MinPerOrder"));
        var maxPerOrder = parseInt($(currParentDom).attr("MaxPerOrder"));

        if (maxPerOrder == 0) {
            currParentDom.find("input[type=text]").val(oldQty);
            currParentDom.parent().find("#ModifyFailLabel").text("库存不足");
            currParentDom.parent().find("#ModifyFailLabel").show();
            setTimeout(function () {
                    currParentDom.parent().find("#ModifyFailLabel").hide();
                },
                2000
            );
            return;
        }

        if (action == "ProductQtySubtract") {
            if (currQty - 1 < minPerOrder) {// || currQty - 1 > maxPerOrder
                if (minPerOrder == maxPerOrder) {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder);
                }
                else {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                }
                currParentDom.parent().find("#ModifyFailLabel").show();
                setTimeout(function () { currParentDom.parent().find("#ModifyFailLabel").hide(); }, 2000);
                return;
            }
            currQty--;
        }
        else if (action == "ProductQtyAdd") {
            if (currQty + 1 < minPerOrder || currQty + 1 > maxPerOrder) {
                if (minPerOrder == maxPerOrder) {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder);
                }
                else {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                }
                currParentDom.parent().find("#ModifyFailLabel").show();
                setTimeout(function () { currParentDom.parent().find("#ModifyFailLabel").hide(); }, 2000);
                return;
            }
            currQty++;
        }
        else {
            if (currQty < minPerOrder || currQty > maxPerOrder) {
                currParentDom.find("input[type=text]").val(oldQty);
                if (minPerOrder == maxPerOrder) {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder);
                }
                else {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                }
                currParentDom.parent().find("#ModifyFailLabel").show();
                setTimeout(function () { currParentDom.parent().find("#ModifyFailLabel").hide(); }, 2000);
                return;
            }
        }

        if (currQty == 0) {
            currParentDom.find("input[type=text]").val(oldQty);
            return;
        }

        var sysNo = currParentDom.attr("ProductSysNo");
        var param = "?ProductSysNo=" + sysNo + "&Qty=" + currQty;
        ShoppingCart.AsynCallHandleModify(e, "UpdateProduct", param);
    },
    //更改购物车套餐数量
    UpdatePackage: function (e, action) {
        var pattern = /^\d+$/;
        var currParentDom = $(e).parent();
        var currQty = currParentDom.find("input[type=text]").val();
        var oldQty = currParentDom.find("input[type=text]").attr("OldQty");
        if (!pattern.test(currQty)) {
            currParentDom.find("input[type=text]").val(oldQty);
            currParentDom.parent().find("#ModifyFailLabel").text("只能输入正整数");
            currParentDom.parent().find("#ModifyFailLabel").show();
            setTimeout(function () { currParentDom.find("#ModifyFailLabel").hide(); }, 2000);
            return;
        }
        currQty = parseInt(currQty);

        var minPerOrder = parseInt($(currParentDom).attr("MinPerOrder"));
        var maxPerOrder = parseInt($(currParentDom).attr("MaxPerOrder"));

        if (maxPerOrder == 0) {
            currParentDom.find("input[type=text]").val(oldQty);
            currParentDom.parent().find("#ModifyFailLabel").text("库存不足");
            currParentDom.parent().find("#ModifyFailLabel").show();
            setTimeout(function () { currParentDom.parent().find("#ModifyFailLabel").hide(); }, 2000);
            return;
        }

        if (action == "PackageQtySubtract") {
            if (currQty - 1 < minPerOrder || currQty - 1 > maxPerOrder) {
                if (minPerOrder == maxPerOrder) {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder);
                }
                else {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                }
                currParentDom.parent().find("#ModifyFailLabel").show();
                setTimeout(function () { currParentDom.parent().find("#ModifyFailLabel").hide(); }, 2000);
                return;
            }
            currQty--;
        }
        else if (action == "PackageQtyAdd") {
            if (currQty + 1 < minPerOrder || currQty + 1 > maxPerOrder) {
                if (minPerOrder == maxPerOrder) {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder);
                }
                else {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                }
                currParentDom.parent().find("#ModifyFailLabel").show();
                setTimeout(function () { currParentDom.parent().find("#ModifyFailLabel").hide(); }, 2000);
                return;
            }
            currQty++;
        }
        else {
            if (currQty < minPerOrder || currQty > maxPerOrder) {
                currParentDom.find("input[type=text]").val(oldQty);
                if (minPerOrder == maxPerOrder) {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder);
                }
                else {
                    currParentDom.parent().find("#ModifyFailLabel").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                }
                currParentDom.parent().find("#ModifyFailLabel").show();
                setTimeout(function () { currParentDom.parent().find("#ModifyFailLabel").hide(); }, 2000);
                return;
            }
        }

        if (currQty == 0) {
            currParentDom.find("input[type=text]").val(oldQty);
            return;
        }
        var sysNo = currParentDom.attr("PackageNo");
        var param = "?PackageSysNo=" + sysNo + "&Qty=" + currQty;
        ShoppingCart.AsynCallHandleModify(e, "UpdatePackage", param);
    },
    //删除购物车中指定的商品
    DelProduct: function (e) {
        if (confirm("您确定要删除该商品吗？")) {
            var param = "?ProductSysNo=" + $(e).attr("ProductSysNo");
            ShoppingCart.AsynCallHandleModify(e, "DelProduct", param);
        };
    },
    //删除购物车中指定的套餐
    DelPackage: function (e) {
        if (confirm("您确定要删除该套餐吗？")) {
            var param = "?PackageSysNo=" + $(e).attr("PackageNo");
            ShoppingCart.AsynCallHandleModify(e, "DelPackage", param);
        };
    },
    //删除购物车中指定套餐中的某商品
    DelPackageProduct: function (e) {
        if (confirm("您确定要删除该商品吗？")) {
            var param = "?PackageSysNo=" + $(e).attr("PackageNo") + "&ProductSysNo=" + $(e).attr("ProductSysNo");
            ShoppingCart.AsynCallHandleModify(e, "DelPackageProduct", param);
        };
    },
    //删除购物车中某商品的某赠品
    DelGift: function (e) {
        if (confirm("您确定要删除该赠品吗？")) {
            var activityNo = $(e).attr("ActivityNo");
            var packageSysNo = $(e).attr("PackageNo");
            var productSysNo = $(e).attr("ProductSysNo");
            var giftSysNo = $(e).attr("GiftSysNo");
            var param = "?ActivityNo=" + activityNo + "&PackageSysNo=" + packageSysNo + "&ProductSysNo=" + productSysNo + "&GiftSysNo=" + giftSysNo;
            ShoppingCart.AsynCallHandleModify(e, "DelGift", param);
        };
    },
    //选择购物车中某商品的某赠品
    SltGift: function (e) {
        var activityNo = $(e).attr("ActivityNo");
        var packageSysNo = $(e).attr("PackageNo");
        var productSysNo = $(e).attr("ProductSysNo");
        var giftSysNo = $(e).val();
        if ($.trim(giftSysNo).length == 0) {
            giftSysNo = $(e).attr("CurrSltSysNo");
            var param = "?ActivityNo=" + activityNo + "&PackageSysNo=" + packageSysNo + "&ProductSysNo=" + productSysNo + "&GiftSysNo=" + giftSysNo;
            ShoppingCart.AsynCallHandleModify(e, "DelSltGift", param);
        }
        else {
            var param = "?ActivityNo=" + activityNo + "&PackageSysNo=" + packageSysNo + "&ProductSysNo=" + productSysNo + "&GiftSysNo=" + giftSysNo;
            ShoppingCart.AsynCallHandleModify(e, "SltGift", param);
        }
    },
    //删除购物车中某商品选择的某赠品
    DelSltGift: function (e) {
        var activityNo = $(e).attr("ActivityNo");        
        var packageSysNo = $(e).attr("PackageNo");
        var productSysNo = $(e).attr("ProductSysNo");
        var giftSysNo = $(e).attr("GiftSysNo");
        var param = "?ActivityNo=" + activityNo + "&PackageSysNo=" + packageSysNo + "&ProductSysNo=" + productSysNo + "&GiftSysNo=" + giftSysNo;
        ShoppingCart.AsynCallHandleModify(e, "DelSltGift", param);
    },
    //删除购物车中订单上某活动的某赠品
    DelOrderGift: function (e) {
        if (confirm("您确定要删除该赠品吗？")) {
            var activityNo = $(e).attr("ActivityNo");
            var giftSysNo = $(e).attr("GiftSysNo");
            var param = "?ActivityNo=" + activityNo + "&GiftSysNo=" + giftSysNo;
            ShoppingCart.AsynCallHandleModify(e, "DelOrderGift", param);
        };
    },
    //删除购物车中订单上某活动选择的某赠品
    DelOrderSltGift: function (e) {
        if (confirm("您确定要删除该赠品吗？")) {
            var activityNo = $(e).attr("ActivityNo");
            var giftSysNo = $(e).attr("GiftSysNo");
            var param = "?ActivityNo=" + activityNo + "&GiftSysNo=" + giftSysNo;
            ShoppingCart.AsynCallHandleModify(e, "DelOrderSltGift", param);
        };
    },
    //选择购物车中订单上某活动的某赠品
    SltOrderGift: function (e) {
        var limitCount = parseInt($(e).attr("LimitCount"));
        var activityNo = $(e).attr("ActivityNo");
        var giftSysNos = "";
        var totalSltCnt = 0;
        var hit = "";
        $(e).parent().parent().find(".mover").find("li").each(function () {
            if ($(this).find("input").attr("checked") == "checked") {
                if (giftSysNos.length > 0)
                    giftSysNos += ",";
                giftSysNos += $(this).find("input").attr("GiftSysNo");
                totalSltCnt++;
            }
        });
        if (totalSltCnt == 0) {
            $(e).parent().find("span").eq(0).text("请选择赠品！");
            $(e).parent().find("span").eq(0).show();
            return;
        }
        if (totalSltCnt > limitCount) {
            $(e).parent().find("span").eq(0).text("您勾选的赠品超过了规定的赠送数量！");
            $(e).parent().find("span").eq(0).show();
            return;
        }

        $(e).parent().find("span").eq(0).hide();
        $(e).parent().find("span").eq(1).show();
        $(e).parent().find("a").hide();

        var param = "?ActivityNo=" + activityNo + "&GiftSysNos=" + giftSysNos;
        ShoppingCart.AsynCallHandleModify(e, "SltOrderGift", param);
    },
    //选择加够商品
    SltPlusBuyProduct: function (e) {
        var giftSysNos = "";
        $(e).parents(".pop_exchange").find("input").each(function () {
            if ($(this).attr("checked") == "checked") {
                if (giftSysNos.length > 0)
                    giftSysNos += ",";
                giftSysNos += $(this).attr("ProductSysNo");
            }
        });
        if (giftSysNos.length == 0) {
            alert("请选择加够商品！");
            return;
        }

        var param = "?&ProductSysNos=" + giftSysNos;
        ShoppingCart.AsynCallHandleModify(e, "SltPlusBuyProduct", param);
    },
    //移除加够商品
    DelPlusBuyProduct: function (e) {
        var giftSysNo = $(e).attr("ProductSysNo");
        var param = "?ProductSysNo=" + giftSysNo;
        ShoppingCart.AsynCallHandleModify(e, "DelPlusBuyProduct", param);
    },
    //清空购物车
    ClearAll: function (e) {
        if(confirm("您确定要清空购物车吗？")){
            ShoppingCart.AsynCallHandleModify(e, "ClearAll", "");
            $("#RecommendContainer").hide();
        };
    },
    //勾选商品
    ProductChecked: function (e) {
        var ProductSysNo = $(e).attr("value");
        var param = "?ProductSysNo=" + ProductSysNo;
        ShoppingCart.AsynCallHandleModify(e, "ProductChecked", param);
    },


    GoToLoginAndSucessToCart: function () {
        var url = $("#GoToLoginAndSucessToCartUrl").val();
        window.location.href = url;
    },
    GoToLoginAndSucessToCheckout: function () {
        var url = $("#GoToLoginAndSucessToCheckoutUrl").val();
        window.location.href = url;
    },
    AsynCallHandleModify: function (e, action, param) {
        var href = "";
        var PackageTypeSingleList = getPackageTypeSingleList();
        var PackageTypeGroupList = getPackageTypeGroupList();
        if (param != "") {
            href = "/ShoppingCart/" + action + param + "&PackageTypeSingle=" + PackageTypeSingleList + "&PackageTypeGroup=" + PackageTypeGroupList;
        }
        else {
            href = "/ShoppingCart/" + action;
        }
        $.ajax({
            type: "POST",
            dataType: "html",
            //url: "/ShoppingCart/" + action + param + "&PackageTypeSingle=" + PackageTypeSingleList + "&PackageTypeGroup=" + PackageTypeGroupList,
            url: href,
            cache: false,
            beforeSend: function (XMLHttpRequest) {
                ShoppingCart.ShowMaskLayer();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data, textStatus, jqXHR) {
                var promID = "ModifySuccessLabel";
                if (action == "UpdateProduct") {
                    promID += $(e).parent().attr("PackageNo") + $(e).parent().attr("ProductSysNo");
                }
                if (action == "UpdatePackage") {
                    promID += $(e).parent().attr("PackageNo");
                }

                $("#shoppingCartPannel").html(data);

                if (action == "UpdateProduct" || action == "UpdatePackage") {
                    prompDom = $("#" + promID);
                    prompDom.show();
                }

                //刷新UI的js
                $(".delslider").hover(function () {
                    $(".abtnp", this).stop(true, true).fadeIn()
                }, function () {
                    $(".abtnp", this).stop(true, true).fadeOut()
                });
                UI.Xslider(".sliderC", {
                    numtoMove: 4,
                    showNav: ".nav a"
                });
                $(".prom_exchange .opener").hover(function () {
                    $(".pop_exchange_show").hide().removeClass("pop_exchange_show");
                    $(this).parent().find(".pop_exchange").show().addClass("pop_exchange_show");
                }, function () { });
                $(".pop_exchange .close").live("click", function () {
                    $(this).parent().hide().removeClass("pop_exchange_show");
                });
                $(".pop_exchange .btn_gray24").live("click", function () {
                    $(this).parent().parent().hide().removeClass("pop_exchange_show");
                });
            },
            complete: function (XMLHttpRequest, textStatus) {
                ShoppingCart.HideMaskLayer();
                ShoppingCartMini.GetPopCartPrdCnt();
            }
        });
    },
    //去结算
    GotoCheckOutUrl: function (PackageTypeSingleList,PackageTypeGroupList, url) {
        $.ajax({
            type: "POST",
            dataType: "JSON",
            url: url,
            data: {
                SysNo: PackageTypeSingleList,
                Group: PackageTypeGroupList,
            },
            beforeSend: function () {
                ShoppingCart.ShowMaskLayer();
            },
            success: function (response) {
            },
            complete: function () {
                $("#SumbitingPanel").hide();
                $("#CanSubmitBtn").show();
                ShoppingCart.hideLoading();
            }
        });
    }
}

function getPackageTypeSingleList() {
    var ckList = $(".PackageTypeSingle");
    var selectList = new Array();
    for (var i = 0 ; i < ckList.length; i++) {
        if (ckList[i].checked == true) {
            selectList.push(ckList[i]);
        }
    }

    var PackageTypeSingleList = '';
    for (var i = 0 ; i < selectList.length; i++) {
        PackageTypeSingleList += selectList[i].value;
        if (i != selectList.length - 1) {
            PackageTypeSingleList += ',';
        }
    }
    return PackageTypeSingleList;
}

function getPackageTypeGroupList() {
    var ckList = $(".PackageTypeGroup");
    var selectList = new Array();
    for (var i = 0 ; i < ckList.length; i++) {
        if (ckList[i].checked == true) {
            selectList.push(ckList[i]);
        }
    }

    var PackageTypeGroupList = '';
    for (var i = 0 ; i < selectList.length; i++) {
        PackageTypeGroupList += selectList[i].value;
        if (i != selectList.length - 1) {
            PackageTypeGroupList += ',';
        }
    }
    return PackageTypeGroupList;
}
