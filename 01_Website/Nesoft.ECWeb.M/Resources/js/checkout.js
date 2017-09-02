
(function (win) {

    var UILoading = new UI.loading(),
        ajaxRequestCount = 0;

    UILoading.mark.$el.css("z-index", "10000");

    var context = {
        OrderMemo: "",
        PayTypeID: 0,
        ShippingAddressID: 0,
        PromotionCode: "",
        ValidateKey: "",
        ShoppingItemParam: ""
    };

    function setContext() {
        var remarkinfoDom = $(".remarkinfo .inarea:eq(0)");
        var remarkinfo = remarkinfoDom.val();
        if (remarkinfo == remarkinfoDom.attr("placeholder")) {
            remarkinfo = "";
        }
        context.OrderMemo = remarkinfo;
        context.PayTypeID = $(".paytype div:eq(0)").attr("PayTypeID");
        context.ShippingAddressID = $(".checkout_address .myaddrlist:eq(0)").attr("ShipTypeID");
        context.PromotionCode = $(".couponSec div:eq(0)").attr("CouponCode");
    }

    function modifyCheckout(c) {
        setContext();
        $.extend(context, c, {});
        ajaxProcessor("Build", context, function (data) {
            $("#checkoutPannel").html(data);
        });
    }

    function expandNewAddressPanel() {
        if (!$(".newaddr").hasClass("newaddr_show")) {
            $(".newaddr").addClass("newaddr_show");
        }
        $("#catepro_iscroll").height(window.innerHeight - 45); //计算并设置可滑动区域的高度
        setTimeout(function () {
            $("#catepro_iscroll").get(0).opener.refresh();
            $("#catepro_iscroll").get(0).opener.scrollTo(0, -1 * (($("#myaddrlist").height() - window.innerHeight) < 0 ? 0 : $("#myaddrlist").height() - window.innerHeight + 45));
        }, 150);
    }

    function collapseNewAddressPanel() {
        if ($(".newaddr").hasClass("newaddr_show")) {
            $(".newaddr").removeClass("newaddr_show");
        }
    }

    function ajaxProcessor(action, postData, callback, dataType) {
        dataType = dataType || "html";
        $.ajax({
            type: "post",
            dataType: dataType,
            url: win["baseurl"] + "/Checkout/" + action,
            data: postData,
            cache: false,
            beforeSend: function (XMLHttpRequest) {
                if (++ajaxRequestCount <= 1) {
                    UILoading.show();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                UI.alert("系统错误", 1200);
            },
            success: function (data, textStatus, jqXHR) {
                if (data.error) {
                    if (data.code == 401) {
                        window.location.href = $("#__LoginUrl__").val();
                    }
                    else if (data.message) {
                        window.scrollTo(0, 0);
                        UI.alert(data.message, 1200);
                    }
                    else if (data.messages && data.messages.length > 0) {
                        var errormessage = "";
                        for (var i = 0; i < data.messages.length; i++) {
                            errormessage += data.messages[i] + "\r\n";
                        }
                        window.scrollTo(0, 0);
                        UI.alert(errormessage, 1200);
                    }
                    return;
                }
                if (typeof (callback) == "function") {
                    callback(data);
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                if (--ajaxRequestCount <= 0) {
                    UILoading.hide();
                }
            }
        });
    }

    var Checkout = win["Checkout"] = win["Checkout"] || {

        context: context,

        applyCoupon: function (e) {
            var $this = $(e);
            var prePromotionCode = $this.parents(".inputCoupon").find("input").eq(0).val();
            if (prePromotionCode.length <= 0) {
                UI.alert("请输入优惠券编码！", 1200);
                return;
            }
            setContext();
            context.PromotionCode = prePromotionCode;
            ajaxProcessor("ApplyCoupon", context, function (data) {
                if (data.success) {
                    $(".couponSec div:eq(0)").attr("CouponCode", data.couponCode);
                    $(".coupon_pop").get(0).opener.mark.callback();
                    modifyCheckout();
                }
                else {
                    $this.parents(".inputCoupon").find("input").eq(0).val("");
                    $(".inputCoupon .applyMsg").remove();
                    $("<span class=\"applyMsg\">优惠券应用失败：" + data.message + "</span>").insertBefore(".inputCoupon .inputwrap");
                }
            }, "json");
        },
        cancelApplyCoupon: function (e) {
            modifyCheckout({ PromotionCode: "" });
        },
        selShippingAddress: function (e) {
            var $this = $(e);
            var sysno = parseInt($this.find("input:eq(0)").prop("checked", true).val());
            if (sysno <= 0) {
                $("#form_newaddr input").each(function () {
                    $(this).val("");
                    if ($(this).attr("name") == "SysNo") {
                        $(this).val("0");
                    }
                })
                $("#form_newaddr select").each(function () {
                    $(this).attr("selectVal", "0");
                })
                window.Area.init();
                expandNewAddressPanel();
            }
            else {
                $(".checkout_address .myaddrlist:eq(0)").attr("ShipTypeID", sysno);
                collapseNewAddressPanel();
                //收起选择地址浮层
                $(".checkout_address").get(0).opener.mark.callback();
                modifyCheckout();
            }
        },
        editShippingAddress: function (e) {
            var $this = $(e);
            $("#myaddrlist ul>li").prop("checked", false);
            $this.parent().find("input:eq(0)").prop("checked", true);
            var sysno = parseInt($this.parent().find("input:eq(0)").val());
            if (sysno > 0) {
                ajaxProcessor("ShippingAddress/Get", { id: sysno }, function (data) {
                    $(".newaddr").eq(0).html(data);
                    expandNewAddressPanel();
                });
            }
        },
        saveShippingAddress: function (e) {
            var postData = $("#form_newaddr").serialize();
            ajaxProcessor("ShippingAddress/Submit", postData, function (data) {
                //收起选择地址浮层
                $(".checkout_address").get(0).opener.mark.callback();
                modifyCheckout({ ShippingAddressID: data.SysNo });
            }, "json");
        },
        submit: function (e) {
            setContext();
            if (isNaN(context.ShippingAddressID) || parseInt(context.ShippingAddressID) <= 0) {
                UI.alert("请先保存收货人信息！");
                window.scrollTo(0, 0);
                return false;
            }
            $(e).html("正在提交...").attr("onclick", "javascript:void(0)");
            ajaxProcessor("Submit", context, function (data) {
                if (data.url) {
                    window.location = data.url;
                    return;
                }
            }, "json");
        }
    };

})(window);



