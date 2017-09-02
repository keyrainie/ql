/// <reference path="../../../common/jquery-1.9.0.min.js"/>

var CheckOut = {

    jqMask: PopWin("#ajaxLoad"),

    jqValidator: null,

    context: {
        PointPay: 0,
        OrderMemo: "",
        IsUsedPrePay: 0,
        PayTypeID: 0,
        ShippingAddressID: 0,
        PromotionCode: "",
        ValidateKey: "",
        ShoppingItemParam: ""
    },

    init: function () {

        //close default ajax load style
        $.utility.settings.showLoading = false;

        $(".myaddrlist li input[name='addSel']").click(function () {
            if ($(this).parents("li").hasClass("last")) {
                $("#shippingAddressEditPanel").slideDown("normal");
                if (CheckOut.jqValidator) {
                    CheckOut.jqValidator.resetForm();
                }
                $(':input', '.form_newaddr')
                    .not(':button, :submit, :reset')
                    .val('')
                    .removeAttr('checked')
                    .removeAttr('selected');
                $("#contactId").val(0);
            }
            else {
                $("#shippingAddressEditPanel").slideUp("normal");
                $(".myaddrlist li[class='curr'] input[name='addSel']").each(function () {
                    $(this).parents("li").removeClass("curr");
                })
                $(this).parents("li").removeClass("curr").addClass("curr");

                CheckOut.editCheckout();
            }
        });
    },

    editCheckout: function () {
        CheckOut._setContext();
        if (isNaN(CheckOut.context.PayTypeID) || parseInt(CheckOut.context.PayTypeID) <= 0) {
            alert("请选择一种支付方式！");
            window.scrollTo(0, 0);
            return false;
        }
        if (isNaN(CheckOut.context.ShippingAddressID) || parseInt(CheckOut.context.ShippingAddressID) <= 0) {
            alert("请先保存收货人信息！");
            window.scrollTo(0, 0);
            return false;
        }
        CheckOut._ajaxProcessor(CheckOut.context, "html", Resources.ajaxBuildCheckoutURL, function (data) {
            $("#EditPanel").html(data);
        });
    },

    editShippingAddress: function (element) {
        var $this = $(element);
        var sysno = $this.parents("li").attr("sysno");
        if (isNaN(sysno)) return;

        sysno = parseInt(sysno);
        if (isNaN(sysno)) return;

        $("ul.myaddrlist li:first input[type='radio']").prop("checked", false);
        $this.parents("li").find("input[type='radio']:eq(0)").prop("checked", true);
        var postData = { cmd: "get", id: sysno };
        CheckOut._ajaxProcessor(postData, "html", Resources.ajaxEditShippingAddressURL, function (data) {
            $("#shippingAddressEditPanel").html(data);
            $("#shippingAddressEditPanel").slideDown("normal");
        });
    },

    deleteShippingAddress: function (element) {
        var sysno = $(element).parents("li").attr("sysno");

        sysno = parseInt(sysno);
        if (isNaN(sysno)) return;

        $.ShowConfirm("确定要删除该收货地址吗？", {
            callback: function () {
                var postData = { cmd: "del", id: sysno };
                CheckOut._ajaxProcessor(postData, "json", Resources.ajaxEditShippingAddressURL, function (data) {
                    $("ul.myaddrlist li[sysno='" + data.sysno + "']").remove();
                    $("ul.myaddrlist li:first input[type='radio']").attr("checked", "checked").click();

                    var addrlen = $("ul.myaddrlist li").not(".last").length;
                    if ($("ul.myaddrlist li.last").length <= 0 && addrlen >= 0 && addrlen < 5) {
                        var newaddress =
"<li class=\"last\"> \
    <label> \
        <input type=\"radio\" name=\"addSel\" value=\"0\"><span class=\"name name_newaddr\">使用新收货地址</span></label> \
</li>";
                        $(newaddress).appendTo("ul.myaddrlist");
                        if (addrlen == 0) {
                            $("#shippingAddressEditPanel").slideDown("normal", function () {
                                CheckOut.init();
                            });
                        } else {
                            CheckOut.init();
                        }
                    }
                });
            }
        });
    },

    updateGiftCard: function (element) {
        var action = $(element).attr("Action");
        var qty = 0;
        var strQty = $.trim($(element).parent().find("input").val());
        var oldQty = $(element).parent().find("input").attr("OldQty");
        if (action == "ModifyQtyChange") {
            var pattern = /^\d+$/;
            if (strQty.length == 0 || !pattern.test(strQty)) {
                $(element).parent().find("p").eq(1).html("必须输入正整数");
                $(element).parent().find("p").eq(1).show();
                $(element).val(oldQty);
                setTimeout(function () {
                    $("p.limitip").hide();
                }, 2000);
                return;
            }
            qty = parseInt(strQty);
        }
        if (action == "ModifyQtySubtract") {
            qty = parseInt(strQty) - 1;
        }
        if (action == "ModifyQtyAdd") {
            qty = parseInt(strQty) + 1;
        }
        var minPerOrder = parseInt($(element).parent().attr("MinCountPerOrder"));
        var maxPerOrder = parseInt($(element).parent().attr("MaxCountPerOrder"));
        if (qty < minPerOrder || qty > maxPerOrder) {
            if (qty < minPerOrder)
                oldQty = minPerOrder;
            else
                oldQty = maxPerOrder;
            $(element).parent().find("p").eq(1).html("每单限购" + minPerOrder + "-" + maxPerOrder);
            $(element).parent().find("p").eq(1).show();
            $(element).val(oldQty);
            setTimeout(function () {
                $("p.limitip").hide();
            }, 2000);
            return;
        }

        $(element).parent().find("input").val(qty);
        var products = [];
        $("tr[ProductList=True]").each(function () {
            var item = {
                ProductSysNo: parseInt($(this).attr("ProductSysNo")),
                Quantity: parseInt($(this).find("input").val())
            };
            products.push(item);
        });

        CheckOut._setContext();
        if (isNaN(CheckOut.context.PayTypeID) || parseInt(CheckOut.context.PayTypeID) <= 0) {
            alert("请选择一种支付方式！");
            window.scrollTo(0, 0);
            return false;
        }
        if (isNaN(CheckOut.context.ShippingAddressID) || parseInt(CheckOut.context.ShippingAddressID) <= 0) {
            alert("请先保存收货人信息！");
            window.scrollTo(0, 0);
            return false;
        }
        $(element).val(oldQty);
        $(element).parent().find("p").eq(0).show();

        var postData = { data: $.toJSON(products), context: $.toJSON(CheckOut.context) };
        CheckOut._ajaxProcessor(postData, "html", Resources.ajaxEditBuyGiftCardURL, function (data) {
            $("#EditPanel").html(data);
        });
    },

    submit: function (element) {

        CheckOut._setContext();
        if (isNaN(CheckOut.context.PayTypeID) || parseInt(CheckOut.context.PayTypeID) <= 0) {
            alert("请选择一种支付方式！");
            window.scrollTo(0, 0);
            return false;
        }
        if (isNaN(CheckOut.context.ShippingAddressID) || parseInt(CheckOut.context.ShippingAddressID) <= 0) {
            alert("请先保存收货人信息！");
            window.scrollTo(0, 0);
            return false;
        }
        $(element).hide();
        $(element).siblings(".loading").show();

        $(".tipmsg>ul").html("");

        CheckOut._ajaxProcessor(CheckOut.context, "json", Resources.ajaxSubmitCheckoutURL, function (data) {

            if (data.url) {
                window.location = data.url;
            }
            else {
                $(element).siblings(".loading").hide();
                $(element).show();

                if (data.errors) {
                    var errormessage = "";
                    for (var i = 0; i < data.errors.length; i++) {
                        errormessage += data.errors[i] + "\r\n";
                    }
                    alert(errormessage)
                }
                window.scrollTo(0, 0);
            }
        });
        return false;
    },

    _setContext: function () {

        CheckOut.context.IsUsedPrePay = 0;
        CheckOut.context.OrderMemo = "";
        CheckOut.context.PayTypeID = $(".banklist input:radio:checked:eq(0)").val();
        CheckOut.context.ShippingAddressID = $(".myaddrlist input:radio:checked:eq(0)").val();
        CheckOut.context.PromotionCode = "";
        CheckOut.context.PointPay = 0;
        CheckOut.context.VirualGroupBuyOrderTel = "";
    },

    _showMask: function () {
        CheckOut.jqMask.fn.popIn();
    },

    _hideMask: function () {
        CheckOut.jqMask.fn.popOut();
    },

    _ajaxProcessor: function (postData, dataType, url, callback) {

        var ____ = function () {
            $.ajax({
                type: "post",
                dataType: dataType,
                url: url,
                cache: false,
                data: postData,
                beforeSend: function (XMLHttpRequest) { CheckOut._showMask(); },
                error: function (XMLHttpRequest, textStatus, errorThrown) { },
                success: function (data, textStatus, jqXHR) {
                    if (data.error) {
                        return;
                    }
                    if (callback && typeof (callback) == "function") {
                        callback(data);
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    CheckOut._hideMask();
                }
            });
        }

        $.ajax({
            url: "/home/checklogin",
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data > 0) {
                    ____();
                }
                else {
                    MiniLogin.Open(null, ____);
                }
            }
        });
    }

}



