//购物车操作
var ShoppingCart = {
    //遮罩层
    UILoading: new UI.loading(),
    PopGiftPoolTitle: '<a class="returnico" href="javascript:void(0)" onclick="ShoppingCart.GiftPoolPanelBack()"><span>返回</span></a><h1>领取赠品</h1><span class="rightbtn"><a Action="SltOrderGift" href="javascript:void(0)" onclick="ShoppingCart.ModifyCart(this)">立即领取</a></span>',
    ShoppingCartTitle: '<h1 style="text-overflow: ellipsis; white-space: nowrap; height: 44px; overflow: hidden;">购物车</h1>',
    //收藏商品
    AddFavorites: function (e, sysno) {
        var $this = $(e);
        if ($this.hasClass("btn_favored")) {
            UI.alert("已经收藏！", 1200);
            return;
        }
        $.ajax({
            url: $("#__AddFavoritesUrl__").val() + "?ProductSysNo=" + sysno,
            dataType: 'json',
            beforeSend: function () {
                ShoppingCart.ShowMaskLayer();
            }, success: function (res) {
                if (res.error && res.code == 401) {
                    window.location.href = $("#__LoginUrl__").val();
                }
                else if (res.Success) {
                    $this.addClass("btn_favored");
                    UI.alert("收藏成功！", 1200);
                }
                else {
                    UI.alert(res.Message, 1200);
                }
            }, complete: function () {
                ShoppingCart.HideMaskLayer();
            }
        });
    },
    //修改购物车
    ModifyCart: function (e) {
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
            case "ClearAll":
                ShoppingCart.ClearAll(e);
                break;
        }
    },
    //显示遮罩层
    ShowMaskLayer: function () {
        ShoppingCart.UILoading.show();
    },
    //隐藏遮罩层
    HideMaskLayer: function () {
        ShoppingCart.UILoading.hide();
    },
    //更改购物车商品数量
    UpdateProduct: function (e, action) {
        var pattern = /^\d+$/;
        var currParentDom = $(e).parents(".volumeSet");
        var currQty = currParentDom.find("input[type=text]").val();
        var oldQty = currParentDom.find("input[type=text]").attr("OldQty");
        if (!pattern.test(currQty)) {
            currParentDom.find("input[type=text]").val(oldQty);
            currParentDom.find(".volTip>i").text("只能输入正整数");
            currParentDom.find(".volTip").show();
            setTimeout(function () { currParentDom.find(".volTip").hide(); }, 2000);
            return;
        }
        currQty = parseInt(currQty);

        var minPerOrder = parseInt($(currParentDom).attr("MinPerOrder"));
        var maxPerOrder = parseInt($(currParentDom).attr("MaxPerOrder"));

        if (action == "ProductQtySubtract") {
            if (currQty - 1 < minPerOrder) {
                currParentDom.find(".volTip>i").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                currParentDom.find(".volTip").show();
                setTimeout(function () { currParentDom.find(".volTip").hide(); }, 2000);
                return;
            }
            currQty--;
        }
        else if (action == "ProductQtyAdd") {
            if (currQty + 1 < minPerOrder || currQty + 1 > maxPerOrder) {
                currParentDom.find(".volTip>i").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                currParentDom.find(".volTip").show();
                setTimeout(function () { currParentDom.find(".volTip").hide(); }, 2000);
                return;
            }
            currQty++;
        }
        else {
            if (currQty < minPerOrder || currQty > maxPerOrder) {
                currParentDom.find("input[type=text]").val(oldQty);
                currParentDom.find(".volTip>i").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                currParentDom.find(".volTip").show();
                setTimeout(function () { currParentDom.find(".volTip").hide(); }, 2000);
                return;
            }
        }

        var sysNo = currParentDom.attr("ProductSysNo");
        var param = "?ProductSysNo=" + sysNo + "&Qty=" + currQty;
        ShoppingCart.AsynCallHandleModify(e, "UpdateProduct", param);
    },
    //更改购物车套餐数量
    UpdatePackage: function (e, action) {
        var pattern = /^\d+$/;
        var currParentDom = $(e).parents(".volumeSet");
        var currQty = currParentDom.find("input[type=text]").val();
        var oldQty = currParentDom.find("input[type=text]").attr("OldQty");
        if (!pattern.test(currQty)) {
            currParentDom.find("input[type=text]").val(oldQty);
            currParentDom.find(".volTip>i").text("只能输入正整数");
            currParentDom.find(".volTip").show();
            setTimeout(function () { currParentDom.find(".volTip").hide(); }, 2000);
            return;
        }
        currQty = parseInt(currQty);

        var minPerOrder = parseInt($(currParentDom).attr("MinPerOrder"));
        var maxPerOrder = parseInt($(currParentDom).attr("MaxPerOrder"));

        if (action == "PackageQtySubtract") {
            if (currQty - 1 < minPerOrder || currQty - 1 > maxPerOrder) {
                currParentDom.find(".volTip>i").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                currParentDom.find(".volTip").show();
                setTimeout(function () { currParentDom.find(".volTip").hide(); }, 2000);
                return;
            }
            currQty--;
        }
        else if (action == "PackageQtyAdd") {
            if (currQty + 1 < minPerOrder || currQty + 1 > maxPerOrder) {
                currParentDom.find(".volTip>i").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                currParentDom.find(".volTip").show();
                setTimeout(function () { currParentDom.find(".volTip").hide(); }, 2000);
                return;
            }
            currQty++;
        }
        else {
            if (currQty < minPerOrder || currQty > maxPerOrder) {
                currParentDom.find("input[type=text]").val(oldQty);
                currParentDom.find(".volTip>i").text("每单限购" + minPerOrder + "-" + maxPerOrder);
                currParentDom.find(".volTip").show();
                setTimeout(function () { currParentDom.find(".volTip").hide(); }, 2000);
                return;
            }
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
    //选择购物车中订单上活动的赠品
    SltOrderGift: function (e) {
        var giftSysNos = "";
        var activityNos = "";
        var totalSltCnt = 0;
        var giftCntOverLimitCnt = false;
        $(".popGiftPoolSection .giftSection").each(function () {
            var thisSltCnt = 0;
            var thisGiftSysNos = "";
            var $this = $(this);
            var limitCount = parseInt($this.attr("LimitCount"));
            var activityNo = $this.attr("ActivityNo");

            $this.find("dd").each(function () {
                if ($(this).is(".proBox_selected")) {
                    if (thisGiftSysNos.length > 0)
                        thisGiftSysNos += ",";
                    thisGiftSysNos += $(this).attr("GiftSysNo");
                    totalSltCnt++;
                    thisSltCnt++;
                }
            });
            if (thisSltCnt <= 0) {
                $this.find(".flagship>i").hide();
                return true;
            }
            if (thisSltCnt > limitCount) {
                giftCntOverLimitCnt = true;
                $this.find(".flagship>i").show();
            } else {
                $this.find(".flagship>i").hide();
            }
            if (activityNos.length > 0)
                activityNos += ",";
            activityNos += activityNo;

            if (giftSysNos.length > 0)
                giftSysNos += "|";
            giftSysNos += thisGiftSysNos;
        });
        if (totalSltCnt == 0) {
            UI.alert("请选择赠品！");
            return;
        }
        if (giftCntOverLimitCnt) {
            return;
        }
        ShoppingCart.GiftPoolPanelBack();
        var param = "?ActivityNos=" + activityNos + "&GiftSysNos=" + giftSysNos;
        ShoppingCart.AsynCallHandleModify(e, "SltOrderGift", param);
    },
    //清空购物车
    ClearAll: function (e) {
        if (confirm("您确定要清空购物车吗？")) {
            ShoppingCart.AsynCallHandleModify(e, "ClearAll", "");
        };
    },
    GiftPoolPanelBack: function () {
        $(".popGiftPoolSection").get(0).opener.mark.callback();
    },
    PopGiftPoolPanel: function () {
        var $filter_pop = $(".popGiftPoolSection");
        var $mainheader = $(".mainheader");
        $(".popGiftPoolSection").get(0).opener = this;
        if (this.mark) {
            if ($filter_pop.is(".popGiftPoolSection_cur")) {
                this.mark.hide();
            } else {
                this.mark.show();
            }
        } else {
            this.mark = new UI.mark();
            this.mark.setcallback(function () {
                this.hide();
                setTimeout(function () {
                    $("#main").css("position", "relative");
                    $(".mainheader").html(ShoppingCart.ShoppingCartTitle);
                }, 150);
                $(".popGiftPoolSection").removeClass("popGiftPoolSection_cur");
                $(".mainSection").show();
            })
            this.mark.show();
        }
        if (!$filter_pop.is(".popGiftPoolSection_cur")) {
            $("#main").css("position", "static");
            setTimeout(function () {
                $mainheader.html(ShoppingCart.PopGiftPoolTitle);
            }, 150);
            $(".mainSection").hide();
        }
        $filter_pop.toggleClass("popGiftPoolSection_cur");
        return false;
    },
    AsynCallHandleModify: function (e, action, param) {
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/ShoppingCart/" + action + param,
            cache: false,
            beforeSend: function (XMLHttpRequest) {
                ShoppingCart.ShowMaskLayer();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data, textStatus, jqXHR) {
                $("#shoppingCartPannel").html(data);
            },
            complete: function (XMLHttpRequest, textStatus) {
                ShoppingCart.HideMaskLayer();
            }
        });
    }
}
