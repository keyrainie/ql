$(function () {
    $("#province_item").find("a").click(function () {
        productDetail.LoadCityList(this);
    });
    $("#city_item").find("a").click(function () {
        productDetail.SelectCity(this);
    });
    $("#AreaClose").click(function () {
        $("#AreaClose").parent().hide();
    });
    $("#SltAreaName").hover(function () {
        $("#SltAreaName").next().show();
    });
    $("#ddlShippingPrice").click(function () {
        if ($(this).text() != '无配送方式') {
            $("#shippingFee").html('预估运费 ' + $(this).val() + ' 元')
        } else {
            $("#shippingFee").html('');
        }
    });
});

//一分钟检查
var preTimePointForVote = 0;
function checkOneMiniteBeforePublishForVote(postTimeLimit) {
    var nowTime = new Date();
    var nowMinitePoint = nowTime.getHours() * 3600 + nowTime.getMinutes() * 60 + nowTime.getSeconds();
    if (nowMinitePoint - preTimePointForVote < postTimeLimit) {
        return false;
    }
    return true;

}
//重置时间
function resetPublishTimePointForVote() {
    var nowTime = new Date();
    var nowMinitePoint = nowTime.getHours() * 3600 + nowTime.getMinutes() * 60 + nowTime.getSeconds();
    preTimePointForVote = nowMinitePoint;
}


var productDetail = {

    addBuyQty: function () {

        var qty = $("#txtBuyQty").val();
        var msg = productDetail.checkBuyQty(parseInt(qty) + 1);
        if (msg == "") {
            $("#txtBuyQty").val(parseInt(qty) + 1);
            productDetail.ChangPrice(parseInt(qty) + 1);

        }
        else {
            $.Showmsg(msg);
        }

    },
    reduceBuyQty: function () {
        var qty = $("#txtBuyQty").val();
        if (parseInt(qty) - 1 <= 0) {
            $("#txtBuyQty").val(1);
            return;
        }
        var msg = productDetail.checkBuyQty(parseInt(qty) - 1);
        if (msg == "") {
            $("#txtBuyQty").val(parseInt(qty) - 1);
            productDetail.ChangPrice(parseInt(qty) - 1);
        }
        else {
            $.Showmsg(msg);
        }
    },

    operateBuyQty: function () {
        var qty = $("#txtBuyQty").val();
        var msg = productDetail.checkBuyQty(qty);
        if (msg != "") {
            $("#txtBuyQty").val(resources_ProductDetail.InitBuyQty);
            $.Showmsg(msg);
        }
        else {
            productDetail.ChangPrice(qty);
        }
    },

    checkBuyQty: function (qty) {

        var buyQty = parseInt(qty, 10);
        var regexNumber = /^[0-9]*[1-9][0-9]*$/;
        if (regexNumber.test(qty) == false || isNaN(buyQty) == true || buyQty <= 0) {
            return "请输入正确的正整数！";
        }

        var overInvertoy = false;
        var overMaxPreOrder = false;
        var lessMinCountPreOrder = false;

        overInvertoy = qty > resources_ProductDetail.Inventory;
        overMaxPreOrder = qty > resources_ProductDetail.MaxPreOrder;
        lessMinCountPreOrder = qty < resources_ProductDetail.MinCountPreOrder;
        if (overInvertoy) {
            return "购买数量超过商品库存！";
        }
        if (overMaxPreOrder) {
            return "购买数量超过每单限购数量！";
        }
        if (lessMinCountPreOrder) {
            return "购买数量低于最小订购数量！";
        }
        return '';

    },
    queryProductReview: function (obj) {
        var reviewCount = parseInt($(obj).attr("count"));
        var searchType = $(obj).attr("searchType");
        var productGroupSysNo = resources_ProductDetail.ProductGroupSysNo;
        //reviewCount = 1;
        if (reviewCount <= 0) {
            return;
        }
        var ReviewQueryInfo = {
            ProductGroupSysNo: productGroupSysNo,
            ProductSysNo: resources_ProductDetail.ProductSysNo,
            SearchType: searchType
        }

        $.ajax({
            url: $("#LoadProductReview").val(),
            type: 'POST',
            dataType: 'html',
            timeout: 30000,
            data: ReviewQueryInfo,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                $($.format("#productReviewTab{0}", $(obj).attr("rel"))).html(data);
            }
        });
    },

    //加入购物车
    addProductToCart: function () {

        window.location.href = $.format("{0}?Category=Product&SysNo={1}&Qty={2}",
            resources_ProductDetail.ShoppingCartUrl,
            resources_ProductDetail.ProductSysNo,
            $("#txtBuyQty").val());

    },
    //浏览大图
    viewBigPic: function (obj) {
        window.open($.format("{0}?Index={1}",
           resources_ProductDetail.BigPicUrl,
           $(obj).attr("index")));
    },
    //加入收藏
    addToWish: function (obj) {
        $.ajax({
            url: resources_ProductDetail.AddToWishListUrl,
            type: 'post',
            dataType: 'json',
            data: { ProductSysNo: resources_ProductDetail.ProductSysNo },
            success: function (data, textStatus, jqXHR) {
                jqXHR.isHandled = true;
                if (!data.error) {
                    $(obj).removeClass("btn_addfavor").addClass("btn_favored").attr("onclick", "javascript:PopWin('#addwish').fn.popIn();");
                    $("#addwish").find(".centerPopT>h3").text('商品已经加入到收藏夹中');
                    PopWin('#addwish').fn.popIn();
                }
                else if (data.code = 401) {
                    //跳转到登录页面
                    alert(data.message);
                    location.href = resources_ProductDetail.LoginUrl + "?ReturnUrl=" + resources_ProductDetail.ReturnUrl;
                }
            }
        });
    },
    addToStoreWish: function (obj) {
        $.ajax({
            url: resources_ProductDetail.AddStoreToWishListUrl,
            type: 'post',
            dataType: 'json',
            data: { sellerSysNo: resources_ProductDetail.SellerSysNo },
            success: function (data, textStatus, jqXHR) {
                jqXHR.isHandled = true;
                if (!data.error) {
                    $(obj).removeClass("btn_addfavor").addClass("btn_favored").removeAttr("onclick");
                    $("#addwish").find(".centerPopT>h3").text('店铺已经加入到收藏夹中');
                    PopWin('#addwish').fn.popIn();
                }
                else if (data.code = 401) {
                    //跳转到登录页面
                    alert(data.message);
                    location.href = resources_ProductDetail.LoginUrl + "?ReturnUrl=" + resources_ProductDetail.ReturnUrl;
                }
            }
        });
    },

    Share: function (e) {
        if (!e) {
            return;
        }
        var link = $(e);
        var id = link.attr("id");
        id = id.replace("shareCtner_", "");
        var docTitle = document.title;
        var docUrl = document.location.href;

        //if (id == "local") {
        //    try {
        //        if (window.sidebar) {
        //            window.sidebar.addPanel(docTitle, docUrl, "");
        //        }
        //        else if (window.external) {
        //            window.external.AddFavorite(docUrl, docTitle);
        //        }
        //        else if (window.opera && window.print) {
        //            window.external.AddFavorite(docUrl, docTitle);
        //        }
        //        else {
        //            alert("操作失败，请手动再试一次！");
        //        }
        //    }
        //    catch (e) {
        //        alert("操作失败，请手动再试一次！");
        //    }

        //    return;
        //}
        var docTitle = encodeURIComponent(docTitle);
        var docUrl = encodeURIComponent(docUrl);
        var url = "";
        switch (id) {
            case "kaixin001":
                url = "http://www.kaixin001.com/~repaste/repaste.php?&rurl={0}&rtitle={1}&rcontent={1}";
                break;
            case "renren":
                url = "http://share.renren.com/share/buttonshare.do?link={0}&title={1}";
                break;
            case "tsina":
                url = "http://v.t.sina.com.cn/share/share.php?title={1}&url={0}"//&ralateUid=1743841222";
                break;
            case "tqq":
                url = "http://v.t.qq.com/share/share.php?url={0}&title={1}&appkey=f50899c2573f45f198d152283055b879";
                break;
            case "douban":
                url = "http://www.douban.com/recommend/?url={0}&title={1}";
                break;
            case "taobao":
                url = "http://share.jianghu.taobao.com/share/addShare.htm?url={0}";
                break;
            case "xianguo":
                url = "http://xianguo.com/service/submitdigg?link={0}&title={1}";
                break;
            case "digu":
                url = "http://www.diguff.com/diguShare/bookMark_FF.jsp?title={1}&url={0}";
                break;
            case "buzz":
                url = "http://www.google.com/buzz/post?url={0}";
                break;
            case "baidu":
                url = "http://cang.baidu.com/do/add?it={1}&iu={0}";
                break;
            case "google":
                url = "http://www.google.com/bookmarks/mark?op=edit&output=popup&bkmk={0}&title={1}";
                break;
            case "youdao":
                url = "http://shuqian.youdao.com/manage?a=popwindow&title={1}&url={0}";
                break;
            case "qq":
                url = "http://sns.qzone.qq.com/cgi-bin/qzshare/cgi_qzshare_onekey?url={0}?FPA=0&title={1}";
                break;
            case "yahoo":
                url = "http://myweb.cn.yahoo.com/popadd.html?url={0}&title={1}";
                break;
        }

        url = $.format(url, docUrl, docTitle);
        if (id === "tsina") {
            var src = resources_ProductDetail.ProductImageP220;
            if (src) {
                url += "&pic=" + encodeURIComponent(src);
            }
        }
        link.attr("href", url);
        link.attr("target", "_blank");
    },


    reviewVote: function (obj) {

        var reviewSysNo = $(obj).attr("reviewSysNo");
        var isUsefull = $(obj).attr("usefull");

        //一分钟检查
        if (!checkOneMiniteBeforePublishForVote(60)) {
            $(obj).parent().find("span").remove();
            $(obj).parent().find("a").first().before("<span>很抱歉，您发表的频率过快，请稍后再试。</span>")
            setTimeout(function () { $(obj).parent().find("span").remove(); }, 3000);

            return;
        }
        if ($(obj).parent("span").attr("isvote") == "1") {
            $(obj).parent().find("span").remove();
            $(obj).parent().find("a").first().before("<span>您已经对该评论投过票，谢谢。</span>")
            setTimeout(function () { $(obj).parent().find("span").remove(); }, 3000);

            return;
        }

        $.ajax({
            url: resources_ProductDetail.ReviewVoteUrl,
            dataType: 'json',
            type: 'post',
            data: { ReviewSysNo: reviewSysNo, IsUsefull: isUsefull },
            success: function (data) {
                if (data.error) {
                    //跳转到登录页面
                    location.href = resources_ProductDetail.LoginUrl + "?ReturnUrl=" + resources_ProductDetail.ReturnUrl;
                }
                else {
                    if (data == 1) {
                        $(obj).parent().find("span").remove();
                        $(obj).parent().find("a").first().before("<span>投票成功，谢谢你的参与。</span>")
                        //防止投票成功后多次投票
                        $(obj).parent().attr("isvote", "1");
                        var count = parseInt($(obj).attr("count"));
                        if (isUsefull == "1") {
                            $(obj).html($.format("有用({0})", count + 1));
                        }
                        else {
                            $(obj).html($.format("没用({0})", count + 1));
                        }
                        setTimeout(function () { $("#isGod").parent().find("span").remove(); }, 3000);
                        resetPublishTimePointForVote();
                    }
                    else {
                        $(obj).parent().find("span").remove();
                        $(obj).parent().find("a").first().before("<span>您已经对该评论投过票，谢谢。</span>")
                        setTimeout(function () { $(obj).parent().find("span").remove(); }, 3000);

                    }
                }
            }

        })
    },

    //计算剩余时间
    CalcTime: function () {
        var totalSeconds = parseInt($("#TotalSeconds").val());
        if (totalSeconds <= 0) {
            $(".second").text(productDetail.FormatTime(0));
            return;
        }
        var day = parseInt(totalSeconds / 86400);
        var hour = parseInt((totalSeconds % 86400) / 3600);
        var minute = parseInt(((totalSeconds % 86400) % 3600) / 60);
        var seconds = parseInt(((totalSeconds % 86400) % 3600) % 60);
        $(".day").text(productDetail.FormatTime(day));
        $(".hour").text(productDetail.FormatTime(hour));
        $(".munite").text(productDetail.FormatTime(minute));
        $(".second").text(productDetail.FormatTime(seconds));
        if (totalSeconds > 0)
            $("#TotalSeconds").val(totalSeconds - 1);
        else
            $("#TotalSeconds").val("0");
        setTimeout(function () {
            productDetail.CalcTime();
        }, 1000);
    },
    //格式化时间
    FormatTime: function (n) {
        if (parseInt(n) < 10) {
            return "0" + n;
        }
        else {
            return n;
        }
    },
    //加载市列表resources_ProductDetail.ProductSysNo
    LoadCityList: function (e) {
        $(".tabitem").eq(0).find("em").text($(e).text());
        $(".tabitem").eq(1).find("em").text("请选择");
        $(".tabitem").eq(1).click();
        $("#city_item").find("ul").html('<p class="tc"><span class="cmnLoadB">正在加载中...</span></p>');
        $.ajax({
            //url: "/Product/AjaxGetCityList?sysno=" + $(e).attr("SysNo"),
            //参数添加商品编号 Asura 2015-7-16
            url: "/Product/AjaxGetCityList?sysno=" + $(e).attr("SysNo") + "&productSysNo=" + resources_ProductDetail.ProductSysNo,
            dataType: 'json',
            type: 'post',
            success: function (data) {
                if (!data.error) {
                    var html = '';
                    for (var i = 0; i < data.length; i++) {
                        html += '<li><a href="javascript:void(0);" SysNo="' + data[i].CitySysNo + '" title="' + data[i].CityName + '">' + data[i].CityName + '</a></li>';
                    }
                    $("#city_item").find("ul").html(html);
                    $("#city_item").find("a").click(function () {
                        productDetail.SelectCity(this);
                    });
                }
            }
        })
    },

    //选择市
    SelectCity: function (e) {
        $(".tabitem").eq(1).find("em").text($(e).text());
        var areaName = $(".tabitem").find("em").text();
        $("#SltAreaName").attr("title", areaName);
        $("#SltAreaName").find("span").text(areaName);
        $("#AreaClose").click();
        productDetail.LoadShippingFee($(e).attr("SysNo"));
        
        //商品运费
        //List<ProductShippingPrice> shippingPrices = null;
        //ProductShippingPrice shippingPrice = null;
        //List<ProductShippingPrice> shippingPrices= ProductFacade.GetProductShippingPrice(productSysNo, int areaSysNo)
        //$.ajax({
        //    url: "/Product/AjaxEntryInfoByArea?psysno=" + resources_ProductDetail.ProductSysNo + "&csysno=" + $(e).attr("SysNo"),
        //    dataType: 'json',
        //    type: 'post',
        //    success: function (data) {
        //        if (!data.error) {
        //            var oldTaxFee = parseFloat($.trim($("#ProductTaxFee").text()));
        //            var oldTotalPrice = parseFloat($.trim($("#ProductTotalPrice").text()));
        //            $("#PolicyDesc").html(data.EntryPort.PolicyDesc);
        //            var taxFee = parseFloat(resources_ProductDetail.CurrentPrice) * parseFloat(data.TariffRate);
        //            $("#ProductTaxFee").text((taxFee + 0.001).toFixed(2));
        //            if (taxFee <= 50) {
        //                $("#ProductFreeTaxFeeFlag").show();
        //                taxFee = 0;
        //            }
        //            else {
        //                $("#ProductFreeTaxFeeFlag").hide();
        //            }
        //            if (oldTaxFee <= 50) {
        //                oldTotalPrice += taxFee;
        //            }
        //            else {
        //                oldTotalPrice -= oldTaxFee;
        //                oldTotalPrice += taxFee;
        //            }
        //            $("#ProductTotalPrice").text((oldTotalPrice + 0.001).toFixed(2));
        //        }
        //    }
        //})
    },

    LoadShippingFee: function (areaSysNo) {
        $("#ddlShippingPrice").html('<option><p class="tc"><span class="cmnLoadB">正在加载中...</span></p></option>');
        $.ajax({
            url: "/Product/AjaxGetShippingPrices?areaSysNo=" + areaSysNo + "&productSysNo=" + resources_ProductDetail.ProductSysNo,
            dataType: 'json',
            type: 'post',
            success: function (data) {
                if (!data.error) {
                    var html = '';
                    for (var i = 0; i < data.length; i++) {
                        html += '<option value = ' + data[i].UnitPrice + '>' + data[i].ShipTypeName + '</option>';
                    }
                    if (data.length == 0) {
                        html = '<option><p class="tc"><span class="cmnLoadB">无配送方式</span></p></option>';
                    }
                    $("#ddlShippingPrice").html(html);
                    if ($("#ddlShippingPrice").text() != '无配送方式') {
                        $("#shippingFee").html('预估运费 ' + $("#ddlShippingPrice").val() + ' 元');
                    } else {
                        $("#shippingFee").html('');
                    }
                }
            }
        })
    },

    ChangPrice: function (qty) {
        var oldPrice = $(".mainPrice").html();
        $.ajax({
            type: "post",
            url: "/Product/AjaxGetProductStepPrice",
            dataType: "json",
            data: { OldPrice: oldPrice, ProductSysNo: resources_ProductDetail.ProductSysNo, ProductCount: qty },
            success: function (data) {
                $(".mainPrice").html(data);
            }
        });
    }
}