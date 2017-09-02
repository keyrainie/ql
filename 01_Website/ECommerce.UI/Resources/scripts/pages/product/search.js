function doSearchInResult(evt) {
    evt = evt || window.event;
    if (evt.type == 'keydown') {
        var key = evt.which;
        if (key != 13) {
            return false;
        }
    }
    var searchObj = $('#btnSearchInResult');
    var tabStoreID = $("#TopSearchBar dl dt a").attr("val");
    var keywords = $.trim(searchObj.val());
    var searchValue = "在结果中搜索";
    if (keywords.length == 0 || keywords == searchValue) {
        //$.Showmsg("搜索关键字不能为空！");
        keywords = "";
        //return;
    }

    keywords = escape(keywords);
    keywords = keywords.replace(/\#/g, "%23").replace(/\&/g, "%26").replace(/\+/g, "%2B");
    var url = $("#hidkeywordurl").val();
    location.href = $.format(url, keywords, $('#txtWithDiscountQuery').val());
    //window.location.href = decodeURI ($.format(resource_ProductSearch.keywordurl, keywords));

}

function addToWishList(obj) {

    if ($(obj).attr("isWished") == "0") {
        $.ajax({
            url: resource_ProductSearch.AddToWishListUrl,
            type: 'post',
            dataType: 'json',
            data: { ProductSysNo: $(obj).attr("productsysno") },
            success: function (data) {
                if (!data.error) {
                    //跳转到我的收藏
                    //location.href = resource_ProductSearch.MyWishLisrUrl;
                    //alert("收藏成功！");
                    if (data == 0) {
                        $.Showmsg($.format("该商品已经在你的收藏夹中！<a href=\"{0}\" target=\"_blank\">【查看收藏夹】</a>", resource_ProductSearch.MyWishLisrUrl));
                    }
                    if (data == 1) {
                        $.Showmsg("收藏成功!");
                        $(obj).attr("isWished", "1");
                    }
                }
                else {
                    //登录页面
                    location.href = resource_ProductSearch.loginUrl + "?ReturnUrl=" + resource_ProductSearch.returnUrl;
                }
            }
        })
    }
    else {
        $.Showmsg("该商品已经在你的收藏夹中！");
    }

}

$(function () {
    var url = $("#hidkeywordurl").val();
    var txtWithSearchWinIn = $('#txtWithSearchWinIn');
    var keywords = $.trim(txtWithSearchWinIn.val());
    $("#btnSearchInResult").val(keywords);
    keywords = escape(keywords);
    keywords = keywords.replace(/\#/g, "%23").replace(/\&/g, "%26").replace(/\+/g, "%2B");

    $('.inputwrap input').keydown(function (e) {
        var key = e.which;
        //fix press shift button
        if (!e.shiftKey
      && ((key >= 48 && key <= 57)
            || (key>=96 && key <= 105)
            || key == 8 || key == 9 || key == 37 || key == 39)) {
            return true;
        }
        return false;
    })
    $('.priceSpan .btn_yes').click(function () {
        var href = window.location.href,
            pf = $('.priceSpan .pf>input').val(),
            pt = $('.priceSpan .pt>input').val();
        if (pf.length > 0) {
            pf = parseFloat(pf);
            if (isNaN(pf) || pf < 0) {
                alert('请输入数字！');
                return;
            }
        }
        if (pt.length > 0) {
            pt = parseFloat(pt );
            if (isNaN(pt) || pt < 0) {
                alert('请输入数字！');
                return;
            }
        }
        pf = parseFloat(pf);
        pt = parseFloat(pt);
        if (!isNaN(pf) && !isNaN(pt) && pf > pt) {
            alert('价格区间起始值不能大于结束值！');
            return;
        }
        if (!isNaN(pf)) {
            if (/pf=([\d\.]+)/i.test(href)) {
                href = href.replace(/pf=([\d\.]+)/i, 'pf=' + pf);
            }
            else {
                if (href.indexOf('?') > 0) {
                    href += '&pf=' + pf;
                } else {
                    href += '?pf=' + pf;
                }
            }
        }
        else {
            href = href.replace(/[&]?pf=([\d\.]+)[&]?/i, '');
        }
        if (!isNaN(pt)) {
            if (/pt=([\d\.]+)/i.test(href)) {
                href = href.replace(/pt=([\d\.]+)/i, 'pt=' + pt);
            }
            else {
                if (href.indexOf('?') > 0) {
                    href += '&pt=' + pt;
                } else {
                    href += '?pt=' + pt;
                }
            }
        } else {
            href = href.replace(/[&]?pt=([\d\.]+)[&]?/i, '');
        }
        if (href.indexOf('?') == href.length - 1) {
            href = href.substr(0, href.length - 1);
        }
        window.location.href = href;
    })

    $(".seprator").delegate('.storetype', 'click', function () {
        var $this = $(this),
            ref = $this.attr('ref'),
            regex = /st=([0-9,]*)/i,
            val = '';
        if ($this.is('.ckbox_checked')) {
            $this.removeClass('ckbox_checked').addClass('ckbox_checked');
            if (regex.test(window.location.href)) {
                var st = regex.exec(window.location.href);
                var vals = st[1].split(',');
                var href = window.location.href;
                if ($.inArray(ref, vals) < 0) {
                    vals.push(ref);
                }
                val = vals.join(',');
            }
            else {
                val = ref;
            }
        } else {
            $this.removeClass('ckbox_checked');
            if (regex.test(window.location.href)) {
                var st = regex.exec(window.location.href);
                var vals = st[1].split(',');
                var href = window.location.href;
                var index = $.inArray(ref, vals);
                if (index >= 0) {
                    vals.splice(index, 1);
                }
                val = vals.join(',');
            }
        }
        if (val.length > 0) {
            if (regex.test(window.location.href)) {
                window.location.href = window.location.href.replace(regex, 'st=' + val);
            }
            else {
                if (window.location.href.indexOf('?') > 0) {
                    window.location.href += '&st=' + val;
                } else {
                    window.location.href += '?st=' + val;
                }
            }
        } else {
            var href = window.location.href.replace(/[&]?st=([0-9,]*)[&]?/i, '');
            if (href.indexOf('?') == href.length - 1) {
                href = href.substr(0, href.length - 1);
            }
            window.location.href = href;
        }
    })

    $(".filtertype a[tag='filtertypeItemLink']").click(
           function () {
               var me = $(this);
               var nValueList = "";
               $(".filtertype a[ngroup='" + me.attr('ngroup') + "']").attr("ischecked", "0");
               me.attr("ischecked", "1");
               $(".filtertype a[ischecked='1']").each(
                   function () {
                       var navigationA = $(this);
                       var currNavigationData = navigationA.attr("NValue");
                       if (nValueList == "") {
                           nValueList = currNavigationData;
                       }
                       else {
                           nValueList = nValueList + "-" + currNavigationData;
                       }
                   }
               );
               var url = me.attr('url');
               var nQStartIndex = url.indexOf("&N=");
               if (nQStartIndex == -1) {
                   nQStartIndex = url.indexOf("?N=");
               }
               if (nQStartIndex == -1) {
                   if (url.indexOf("?") == -1) {
                       url = url + "?";
                   }
                   else {
                       url = url + "&";
                   }
                   url = url + "&N=" + nValueList
               }
               else {
                   var urlRoot = url.substring(0, nQStartIndex);
                   var nSubQuery = url.substring(nQStartIndex);

                   var otherQueryString = "";
                   if (nSubQuery.indexOf("&", 1) != -1) {
                       otherQueryString = nSubQuery.substring(nSubQuery.indexOf("&", 1));
                   }
                   var nValueName = nSubQuery.substring(0, nSubQuery.indexOf("=") + 1);
                   url = urlRoot + nValueName + nValueList + otherQueryString;
               }
               location.href = url;
           })
})

/*产品比较功能*/
var productCompareNew = {
    delString: '',
    compareAlert: '',
    mustSelect: '',
    init: function (categoryID, delString, compareAlert, mustSelect) {
        productCompareNew.delString = delString;
        productCompareNew.compareAlert = compareAlert;
        productCompareNew.mustSelect = mustSelect;

        $(".btnCloseBar").click(function () {
            $("#compareBar,#close").hide();
            $("#extend").show();
        });

        $(".btnExtended").click(function () {
            $("#compareBar,#close").show();
            $("#extend").hide();
            $("#close").show();
        });

        var compareObj = productCompareNew.getCompareObj();
        if (compareObj == null) {
            productCompareNew.initCompareBar(null);
        }
        else {
            if (compareObj.CategoryID != categoryID) {
                productCompareNew.clear();
                productCompareNew.initCompareBar(null);
            }
            else {
                productCompareNew.initCompareBar(compareObj);
            }
        }
    },
    getCompareObj: function () {
        var cookie = $.cookie("ProductCompare");
        if (cookie == '') {
            return null;
        }

        var compareObj = JSON.parse(cookie);
        return compareObj;
    },
    setCompareObj: function (compareObj) {
        var cookie = JSON.stringify(compareObj);
        $.cookie("ProductCompare", cookie);
    },
    addCompare: function (categoryId, productid, url, imgSrc, title) {
        var compareObj = productCompareNew.getCompareObj();
        var product = { id: productid, categoryID: categoryId, url: url, src: imgSrc, title: title };

        if (compareObj == null) {
            compareObj = { CategoryID: categoryId, ProductList: [] };
        }

        var currentList = compareObj.ProductList;
        if (currentList.length >= 4) {
            $.Showmsg(productCompareNew.compareAlert);
            return;
        }

        var find = false;
        $.each(currentList, function (i, n) {
            if (n.id == productid) {
                find = true;
                return false;
            }
        });

        if (find == false) {
            currentList.push(product);
        }

        productCompareNew.setCompareObj(compareObj);
        productCompareNew.initCompareBar(compareObj);
    },
    initCompareBar: function (compareObj) {
        var len;
        var currentList;
        var template;

        if (compareObj == null) {
            len = 0;
        }
        else {
            currentList = compareObj.ProductList;
            len = currentList.length;
            template = "<dt><img src='{0}'></dt><dd><p><a href='{1}' title='{2}'>{3}</a></p><p class='bot'><a href='javascript:productCompareNew.delProductList(\"{4}\");' class='del'>{5}</a></p></dd>";

        }

        $("#compareBar").find("li").each(function (i) {
            if (i < len) {
                var product = currentList[i];
                var html = $.format(template, product.src, product.url, product.title, product.title, product.id, productCompareNew.delString);
                $(this).children("dl").html(html);
                $(this).removeClass("empty");
            }
            else {
                $(this).children("dl").html('<dd>&nbsp;</dd>');
                $(this).addClass("empty");
            }
        });

        if (len != 0) {
            $("#compareBar,#close").show();
            $("#extend").hide();
            $("#close").show();
        }

        //初始化比较工具栏
        if ($('#compareBar .empty').length < 4) {
            var cp = $("#compare");
            var me = $('#compareclose');

            me.html("[收起]");
            cp.stop().animate({ height: 143 }, 1000);
            me.attr("rel", 0)
        }
        else {
            var cp = $("#compare");
            var me = $('#compareclose');

            me.html("[展开]");
            cp.stop().animate({ height: 35 }, 1000);
            me.attr("rel", 1)
        }
    },
    delCompareObj: function (productId, compareObj) {
        if (compareObj != null) {
            var currentList = compareObj.ProductList;

            var index;
            $.each(currentList, function (i, n) {
                if (n.id == productId) {
                    index = i;
                    return false;
                }
            });

            currentList.splice(index, 1);

            if (currentList.length > 0) {
                compareObj.CategoryID = currentList[0].categoryID;
            }
            productCompareNew.setCompareObj(compareObj);
        }
    },
    delProductList: function (productID) {
        var compareObj = productCompareNew.getCompareObj();
        productCompareNew.delCompareObj(productID, compareObj);
        productCompareNew.initCompareBar(compareObj);
    },
    clear: function () {
        $.cookie('ProductCompare', '');
        productCompareNew.initCompareBar(null);
    },
    toCompare: function (url) {
        var compareObj = productCompareNew.getCompareObj();
        var flag = true;
        if (compareObj == null) {
            $.Showmsg(productCompareNew.mustSelect);
            return;
        }

        var currentList = compareObj.ProductList;
        var query = "?Item=";
        var len = currentList.length;

        if (len < 2) {
            $.Showmsg(productCompareNew.mustSelect);
            return;
        }

        $.each(currentList, function (i, n) {
            if (compareObj.CategoryID != n.categoryID) {
                $.Showmsg("所选商品不属于同一个三级类别，请重新选择要比较的商品！");
                flag = false;
                return;
            }
            if (i + 1 == len) {
                query += n.id;
            }
            else {
                query += n.id + ','
            }
        });
        if (flag) {
            window.location = url + query;
        }
    },
    delCompareLink: function (url, categoryID, productID) {
        var compareObj = productCompareNew.getCompareObj();
        productCompareNew.delCompareObj(productID, compareObj);
        window.location = url;
    }
}