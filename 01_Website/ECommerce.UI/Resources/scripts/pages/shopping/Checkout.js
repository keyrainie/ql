/// <reference path="../../../common/jquery-1.9.0.min.js"/>
(function () {

    var jqMask = null,
	ajaxRequestCount = 0,
	$summitBtnTemplate = null;

    setTimeout(function () {
        jqMask = PopWin("#ajaxLoad", { olOpacity: 0 });
    }, 100);

    function _setContext() {
        if ($("#ckUsedPrePay").attr("checked")) {
            CheckOut.context.IsUsedPrePay = 1;
        } else {
            CheckOut.context.IsUsedPrePay = 0;
        }
        var orderMemo = $("#memo").val();
        if (orderMemo == $("#memo").attr("tip")) {
            orderMemo = "";
        }
        CheckOut.context.OrderMemo = orderMemo;
        CheckOut.context.PaymentCategoryID = parseInt($("#payAndShipTypeContent").attr('paycateid'));
        CheckOut.context.ShipTypeID = parseInt($("#payAndShipTypeContent").attr('shiptypeid'));
        CheckOut.context.ShippingAddressID = parseInt($("#shippingAddressContent").attr('shippingaddrsysno'));
        CheckOut.context.NeedInvoice = parseInt($("#customerInvoiceContent").attr('needInvoice'));
        CheckOut.context.PromotionCode = "";
        if ($("#txtCounponCode").attr("useCouponCode")) {
            CheckOut.context.PromotionCode = $("#txtCounponCode").attr("useCouponCode");
        }
        CheckOut.context.PointPay = parseInt($("#txtUsePointPay").val());
        CheckOut.context.BankPointPay = parseInt($("#txtUseBankPointPay").val());
    }

    function _showMask() {
        jqMask.fn.popIn();
    }

    function _hideMask() {
        jqMask.fn.popOut();
    }

    function disableSubmitBtn() {
        $("a[class^=btn_booklist_sub]:eq(0)").attr({ "class": "btn_booklist_sub_disabled", "onclick": "" });
    }

    function enableSubmitBtn() {
        $("a[class^=btn_booklist_sub]:eq(0)").attr({ "class": $summitBtnTemplate.attr('class'), "onclick": $summitBtnTemplate.attr('onclick') });
    }

    function setCheckOutSuccessed(checkOutSuccessed) {
        if (checkOutSuccessed) {
            $summitBtnTemplate = $("<a href='javascript:void(0)' class='btn_booklist_sub' onclick='CheckOut.submit(this)'>提交订单</a>");
        } else {
            $summitBtnTemplate = $("<a href='javascript:void(0)' class='btn_booklist_sub_disabled'>提交订单</a>");
        }
    }

    function _scrollToAnchor(anchor) {
        var top = $(anchor).offset().top + 120 - (window.screen.height - $(anchor).height()) / 2;
        $($.browser.safari || document.compatMode == 'BackCompat' ? document.body : document.documentElement).animate({
            scrollTop: top
        }, 200);
        return false;
    }

    function _ajaxProcessor(postData, dataType, url, callback, showMask) {

        if (typeof showMask === 'undefined') {
            showMask = true;
        }
        var innerProcessor = function () {
            $.ajax({
                type: "post",
                dataType: dataType,
                url: url,
                cache: false,
                data: $.param(postData, true),
                beforeSend: function (XMLHttpRequest) {
                    if (++ajaxRequestCount == 1 && showMask) {
                        _showMask();
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) { },
                success: function (data, textStatus, jqXHR) {
                    if (data.error) {
                        return;
                    }
                    if (callback && typeof (callback) === "function") {
                        callback(data);
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    if (--ajaxRequestCount <= 0 && showMask) {
                        _hideMask();
                    }
                }
            });
        }
        $.ajax({
            url: "/home/checklogin",
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data > 0) {
                    innerProcessor();
                }
                else {
                    window.location.href = Resources.loginReturnURL;
                }
            }
        });
    }

    var CheckOut = {
        jqValidator: null,
        context: {
            PointPay: 0,
            OrderMemo: "",
            IsUsedPrePay: 0,
            PaymentCategoryID: 0,
            ShipTypeID: 0,
            ShippingAddressID: 0,
            PromotionCode: "",
            ValidateKey: "",
            ShoppingItemParam: "",
            NeedInvoice: 0
        },

        initPayAndShipTypePanel: function (paycateid, shiptypeid) {
            $(".payTypeContent").delegate('input[control-role="paytypeItem"]', 'click', function () {
                var $this = $(this);
                $("#payAndShipTypeContent").attr('paycateid', $this.val()).attr('shiptypeid', 0);
                $this.parents('.pay_cod').find('li').removeClass('curr');
                $this.parent().parent().addClass('curr');
                _ajaxProcessor({ paymentcateid: $this.val(), shipaddrsysno: $("#shippingAddressContent").attr('shippingaddrsysno') }, 'html', Resources.ajaxGetShipTypeURL, function (data) {
                    $("#shipTypeContent").html(data);
                    CheckOut.initShipTypePanel();
                })
            })
            $("#payAndShipTypeContent").attr('paycateid', paycateid).attr('shiptypeid', shiptypeid);
            CheckOut.initShipTypePanel();
        },

        initShipTypePanel: function () {
            $(".shipTypeContent").delegate('input[control-role="shiptypeItem"]', 'click', function () {
                var $this = $(this);
                $("#payAndShipTypeContent").attr('shiptypeid', $this.val());
                $('.post_cod li').removeClass('curr');
                $this.parents('li').addClass('curr');
                if (!$this.attr("isGetBySelf")) {
                    $('.self_post_r').removeClass('curr');
                    $('.self_post_inner').removeClass('curr').hide();
                }
            })
            $(".shipTypeContent").delegate('input[control-role="shiptypeGroup"]', 'click', function () {
                var $parent = $(this).parents('.self_post'),
                    $firstGetBySelfItem = $parent.find(".self_post_inner input[control-role='shiptypeItem']:eq(0)");
                $("#payAndShipTypeContent").attr('shiptypeid', $firstGetBySelfItem.val());
                $firstGetBySelfItem.prop('checked', true);
                $('.post_cod li').removeClass('curr');
                $parent.find(".self_post_r,.self_post_inner").addClass("curr").show();
            })
        },

        initShippingAddressPanel: function () {
            $(".myaddrlist").delegate('input[name="addSel"]', 'click', function () {
                var $this = $(this);
                if ($this.parents("li").hasClass("last")) {
                    $("#shippingAddressEditPanel").slideDown("normal", function () {
                        $(this).addClass('expand');
                    });
                    if (CheckOut.jqValidator) {
                        CheckOut.jqValidator.resetForm();
                    }
                    $(':input', '.form_newaddr')
                        .not(':button, :submit, :reset')
                        .val('')
                        .removeAttr('checked')
                        .removeAttr('selected');
                    $("#contactId").val(0);
                    Biz.Common.Area2.clearData();
                }
                else {
                    $("#shippingAddressEditPanel").slideUp("normal", function () {
                        $(this).removeClass('expand');
                    });
                    $(".myaddrlist input[name='addSel']").parents("li").removeClass("curr");
                    $this.parents("li").addClass("curr");
                }
            })
        },

        initCustInvoicePanel: function () {
            $(".invest_need").click(function (e) {
                if ($(this).find("input").attr("checked")) {
                    $(this).siblings(".investc").show();
                    $(this).parent(".invest").find(".action").removeClass("action-state2");
                } else {
                    $(this).siblings(".investc").hide();
                    $(this).parent(".invest").find(".action").addClass("action-state2");
                }
            });
            $(".invest_noneed").click(function (e) {
                if ($(this).find("input").attr("checked")) {
                    $(this).siblings(".investc").hide();
                    $(this).parent(".invest").find(".action").addClass("action-state2");
                } else {
                    $(this).siblings(".investc").show();
                    $(this).parent(".invest").find(".action").removeClass("action-state2");
                }
            });
        },

        init: function (checkOutSuccessed) {
            //close default ajax load style
            $.utility.settings.showLoading = false;
            setCheckOutSuccessed(checkOutSuccessed);
        },

        usePrePay: function () {
            CheckOut.editCheckout();
        },

        usePointPay: function (e) {
            e = e || window.event;
            if (e.type == 'keydown') {
                var key = e.which;
                if (key != 13) {
                    return false;
                }
            }
            var pointPay = parseInt($("#txtUsePointPay").val());
            if (isNaN(pointPay)) {
                alert("请输入数字(如：1,2,3...)！")
                return;
            }
            var customerPointPay = parseInt($("#customerPointPay").text());
            if (pointPay > customerPointPay) {
                alert("积分余额不足，不能使用！")
                return;
            }
            var maxPointPay = parseInt($("#maxPointPay").text());
            if (pointPay > maxPointPay) {
                alert("本单最多只能使用" + maxPointPay + "积分！")
                return;
            }
            CheckOut.editCheckout();
        },
        useBankPointPay: function (e) {
            e = e || window.event;
            if (e.type == 'keydown') {
                var key = e.which;
                if (key != 13) {
                    return false;
                }
            }
            var pointPay = parseInt($("#txtUseBankPointPay").val());
            if (isNaN(pointPay)) {
                alert("请输入数字(如：1,2,3...)！")
                return;
            }
            var customerPointPay = parseInt($("#customerBankPointPay").text());
            if (pointPay > customerPointPay) {
                alert("积分余额不足，不能使用！")
                return;
            }
            var maxPointPay = parseInt($("#maxPointPay").text());
            if (pointPay > maxPointPay) {
                alert("本单最多只能使用" + maxPointPay + "积分！")
                return;
            }
            CheckOut.editCheckout();
        },
        GetCouponCode: function (obj0, obj1, obj2, obj3) {
            $.ajax({
                type: "post",
                url: "/Home/CreateCouponCode",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: {
                    customerID: obj1,
                    couponSysNo: obj2
                },
                success: function (data) {
                    alert(data.ContentType);
                    $("#txtCounponCode").val(data.Data);
                    PopWin('#dialogPlatformCouponList', { action: 'out' })
                    $.ajax({
                        type: "Get",
                        url: "/ShoppingPurchase/GetCouponPopContent",
                        dataType: "html",
                        async: false,
                        timeout: 30000,
                        data: {
                            MerchantSysNo: obj3
                        },
                        success: function (evt) {
                            $('#couponcontent').html(evt);//替换成新的数据
                        }
                    })
                }
            });

        },
        selectPlatCouponCode: function (obj) {
            var code = $.trim($(obj).parent().parent().find('td.code').text());
            //alert(code);
            $("#txtCounponCode").val(code);
            PopWin('#dialogPlatformCouponList', { action: 'out' })
        },
        applyCoupon: function (e) {

            e = e || window.event;
            if (e.type == 'keydown') {
                var key = e.which;
                if (key != 13) {
                    return false;
                }
            }
            if ($("#txtCounponCode").val().length <= 0) {
                alert("请输入优惠券编码！")
                return;
            }
            $("#txtCounponCode").attr("useCouponCode", $.trim($("#txtCounponCode").val()));
            CheckOut.editCheckout();
        },

        cancelApplyCoupon: function () {
            $("#applyedCounponDiv").remove();
            $("#promotionCodeError").remove();
            $("#txtCounponCode").val('').attr("useCouponCode", '');
            CheckOut.editCheckout();
        },

        editCheckout: function (callback) {
            _setContext();
            if (isNaN(CheckOut.context.ShippingAddressID) || parseInt(CheckOut.context.ShippingAddressID) <= 0) {
                if (callback && typeof callback === "function") { callback(); }
                alert("请先保存收货人信息！");
                CheckOut.expandShippingAddress();
                return false;
            }
            if (isNaN(CheckOut.context.PaymentCategoryID) || parseInt(CheckOut.context.PaymentCategoryID) <= 0) {
                if (callback && typeof callback === "function") { callback(); }
                alert("请选择一种支付方式！");
                CheckOut.expandPayAndShipType();
                return false;
            }
            if (isNaN(CheckOut.context.ShipTypeID) || parseInt(CheckOut.context.ShipTypeID) <= 0) {
                if (callback && typeof callback === "function") { callback(); }
                alert("请选择一种配送方式！");
                CheckOut.expandPayAndShipType();
                return false;
            }
            _ajaxProcessor(CheckOut.context, "html", Resources.ajaxBuildCheckoutURL, function (data) {
                if (callback && typeof callback === "function") { callback(data); }

                $("#cartProductContent").html($("#cart_productlist", data));
                $("#gatherInfoContent").html($("#checkout_summary", data));
                $("#errorContent").html($("#cart_errorlist", data));
                $("#applyCouponContent").html($("#checkout_applycoupon", data));
                //$("#usePointPayContent").html($("#checkout_usePointPay", data));
                $("#useBankPointPayContent").html($("#checkout_useBankPointPay", data));
                $("#CheckoutTotalAmount").html($("#CheckoutTotalAmount", data));
                setCheckOutSuccessed($("#checkOutSuccessed", data).val() == "true");
            });
        },

        expandShippingAddress: function () {
            disableSubmitBtn();
            _setContext();
            $("[data-role='section']>h2>a").hide();
            $("[data-role='section']>h2>.tip").html("如需修改，请先保存收货人信息").show();
            $('.step-box').removeClass('step-box-cur');
            _ajaxProcessor(CheckOut.context, "html", Resources.ajaxGetShippingAddressListURL, function (data) {
                $("#shippingAddressContent").html(data).addClass('step-box-cur');
                CheckOut.initShippingAddressPanel();
                _scrollToAnchor("#shippingAddressContent");
            });
        },

        saveShippingAddress: function () {
            var $checkedItem = $(".myaddrlist li input[name='addSel']:checked");
            if ($("#shippingAddressEditPanel").is('.expand')) {
                CheckOut.jqValidator.submitForm(false);
            }
            else {
                $("#shippingAddressContent").attr('shippingaddrsysno', $checkedItem.val());
                var $container = $checkedItem.parent();
                var html = "<h2 class=\"mt30\">收货人信息<a href=\"javascript:void(0)\" onclick=\"CheckOut.expandShippingAddress()\" class=\"fz12 blue ml20\">[修改]</a><span class=\"tip\"></span></h2> \
                    <div class=\"selectads_seleced mb20\" style=\"padding-left: 32px;\"> \
                        <p class=\"p5_0\"> \
                            <span>" + $('span[class="rectitle"]:eq(0)', $container).text() + "</span><span class=\"ml10\">" + $('span[class="recname"]:eq(0)', $container).text()
                                + "</span><span class=\"ml10\">" + $('span[class="phone"]:eq(0)>span', $container).text() + "</span> \
                        </p> \
                        <p class=\"p5_0\">" + $('span[class^="addrinfo"]:eq(0)', $container).text() + "</p> \
                    </div>"
                $("#shippingAddressContent").html(html);
                $("[data-role='section']>h2>a").show();
                $("[data-role='section']>h2>.tip").html("").hide();
                CheckOut.expandPayAndShipType();
            }
        },

        expandPayAndShipType: function () {
            $("#payAndShipTypeContent").attr('paycateid', 0).attr('shiptypeid', 0);
            disableSubmitBtn();
            _setContext();
            $("[data-role='section']>h2>a").hide();
            $("[data-role='section']>h2>.tip").html("如需修改，请先保存支付及配送方式").show();
            $('.step-box').removeClass('step-box-cur');
            _ajaxProcessor(CheckOut.context, "html", Resources.ajaxGetPayAndShipTypeURL, function (data) {
                $("#payAndShipTypeContent").html(data).addClass('step-box-cur');
                _scrollToAnchor("#payAndShipTypeContent");
            });
            if (window.Customer_NoShippingAddress && window.Customer_NoShippingAddress == 1) {
                $('.tipmsg').hide();
            }
        },

        savePayAndShipType: function () {
            $('.step-box').removeClass('step-box-cur');
            var selPayTypeName = $('#payTypeContent input[control-role="paytypeItem"]:checked').parent().text();
            var selShipTypeName = '';
            var $selShipTypeCtrl = $('#shipTypeContent input[control-role="shiptypeItem"]:checked:first');
            if ($selShipTypeCtrl.attr('isGetBySelf')) {
                selShipTypeName = '上门自提' + '<span class="ml10">' + $selShipTypeCtrl.siblings('span.name').text() + '</span>';
            } else {
                selShipTypeName = $selShipTypeCtrl.parent().text();
            }
            var html = "<h2 class=\"mt20\">支付及配送方式<a href=\"javascript:void(0)\" onclick=\"CheckOut.expandPayAndShipType()\" class=\"fz12 blue ml20\">[修改]</a><span class=\"tip\"></span></h2> \
            <div class=\"article paytype paytype_selected\"> \
                <div class=\"inner\"><p>" + $.trim(selPayTypeName) + "</p><p>" + $.trim(selShipTypeName) + "</p> \
                </div> \
            </div>";
            $("#payAndShipTypeContent").html(html);
            $("[data-role='section']>h2>a").show();
            $("[data-role='section']>h2>.tip").html("").hide();
            _setContext();
            _ajaxProcessor(CheckOut.context, 'html', Resources.ajaxEditPayAndShipTypeURL, function (data) {
                ++ajaxRequestCount;
                CheckOut.editCheckout(function () { enableSubmitBtn(); --ajaxRequestCount; });
            })
        },

        expandCustInvoice: function () {
            $("#customerInvoiceContent>h2").html('发票信息');
            $("[data-role='section']>h2>a").hide();
            $("[data-role='section']>h2>.tip").html("如需修改，请先保存发票信息").show();
            $('.step-box').removeClass('step-box-cur');
            disableSubmitBtn();
            _setContext();
            _ajaxProcessor(CheckOut.context, "html", Resources.ajaxGetCustomerInvoiceURL, function (data) {
                $("#customerInvoiceContent").html(data).addClass('step-box-cur');
                CheckOut.initCustInvoicePanel();
                _scrollToAnchor("#customerInvoiceContent");
            }, false);
        },

        saveCustInvoice: function () {
            $('.step-box').removeClass('step-box-cur');
            var invoiceTitle = '不开具发票';
            var needInvoice = false;
            if ($("#invest").prop('checked')) {
                if ($("#invoiceTitle").val().length <= 0) {
                    $('#invoiceTitle').siblings('.Validform_wrong').show();
                    return;
                }
                $('#invoiceTitle').siblings('.Validform_wrong').hide();
                needInvoice = true;
                invoiceTitle = "发票抬头：" + $("#invoiceTitle").val();
            }
            $("#customerInvoiceContent").attr('needInvoice', needInvoice ? '1' : '0');
            if (needInvoice) _ajaxProcessor({ invoiceTitle: $("#invoiceTitle").val() }, "json", Resources.ajaxUpdateCustomerInvoiceURL, null, false);
            var html = '<h2 class=\"mt30\">发票信息<a href=\"javascript:void(0)\" onclick=\"CheckOut.expandCustInvoice()\" class=\"fz12 blue ml20\">[修改]</a><span class=\"tip\"></span></h2>\
                <div class=\"invest invest_selected\">\
                    <div class=\"investc\">\
                        <div>\
                            <p class=\"common mb10\">' + invoiceTitle + '</p>\
                        </div>\
                    </div>\
                </div>';
            $("[data-role='section']>h2>a").show();
            $("[data-role='section']>h2>.tip").html("").hide();
            $("#customerInvoiceContent").html(html);
            enableSubmitBtn();
        },

        editShippingAddress: function (element) {
            var $this = $(element);
            var sysno = parseInt($this.parents("li").attr("sysno"));
            if (isNaN(sysno)) return;

            $("ul.myaddrlist li:first input[type='radio']").prop("checked", false);
            $this.parents("li").find("input[type='radio']:eq(0)").prop("checked", true);
            var postData = { cmd: "get", id: sysno };
            _ajaxProcessor(postData, "html", Resources.ajaxEditShippingAddressURL, function (data) {
                $("#shippingAddressEditPanel").html(data);
                $("#shippingAddressEditPanel").slideDown("normal", function () { $(this).addClass('expand'); });
            });
        },

        setAsDefaultShipping: function (element) {
            var $this = $(element);
            var sysno = parseInt($this.parents("li").attr("sysno"));
            if (isNaN(sysno)) return;

            var postData = { cmd: "default", id: sysno };
            _ajaxProcessor(postData, "text", Resources.ajaxEditShippingAddressURL, function (data) {
                if (data == "y") alert('设置成功！');
            }, false);
        },

        deleteShippingAddress: function (element) {
            var sysno = $(element).parents("li").attr("sysno");

            sysno = parseInt(sysno);
            if (isNaN(sysno)) return;

            $.ShowConfirm("确定要删除该收货地址吗？", {
                callback: function () {
                    var postData = { cmd: "del", id: sysno };
                    _ajaxProcessor(postData, "json", Resources.ajaxEditShippingAddressURL, function (data) {
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
                                    $(this).addClass('expand');
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

        submit: function (element) {
            _setContext();
            if (isNaN(CheckOut.context.ShippingAddressID) || parseInt(CheckOut.context.ShippingAddressID) <= 0) {
                alert("请先保存收货人信息！");
                CheckOut.expandShippingAddress();
                return false;
            }
            if (isNaN(CheckOut.context.PaymentCategoryID) || parseInt(CheckOut.context.PaymentCategoryID) <= 0) {
                alert("请选择一种支付方式！");
                CheckOut.expandPayAndShipType();
                return false;
            }
            if (isNaN(CheckOut.context.ShipTypeID) || parseInt(CheckOut.context.ShipTypeID) <= 0) {
                alert("请选择一种配送方式！");
                CheckOut.expandPayAndShipType();
                return false;
            }
            $(element).hide();
            $(element).siblings(".loading").show();

            $(".tipmsg>ul").html("");

            _ajaxProcessor(CheckOut.context, "json", Resources.ajaxSubmitCheckoutURL, function (data) {
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
        }
    }
    CheckOut.ui = {
        showMask: _showMask,
        hideMask: _hideMask
    };
    window.CheckOut = CheckOut;
})()





