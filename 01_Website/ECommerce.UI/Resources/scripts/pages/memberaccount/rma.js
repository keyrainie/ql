var RMARequest = {
    QuertSOList: function () {

        var domSearch = $("#txtSearch");
        var searchText = domSearch.val();

        var valueIsTip = false;
        if (searchText.length <= 0 || searchText == domSearch.attr('tip')) {
            valueIsTip = true;
        }

        var selected = $("#aSelectedValue").text();
        if (selected == "订单编号") {
            if (valueIsTip) {
                alert("请填写订单编号");
                return;
            }
            if (isNaN(parseInt(searchText))) {
                alert("订单编号必须是数字");
                return;
            }
            if (searchText.length > 8) {
                alert("订单编号长度不能超过8位");
                return;
            }
        } else {
            if (valueIsTip) {
                domSearch.val("");
            }
        }
        $('#formOrderQuery').submit();
    }

}

$(function () {
    

    var jqMask = null,
	ajaxRequestCount = 0,
	$summitBtnTemplate = null;

    setTimeout(function () {
        jqMask = PopWin("#ajaxLoad", { olOpacity: 0 });
    }, 100);

    function _showMask() {
        jqMask.fn.popIn();
    }

    function _hideMask() {
        jqMask.fn.popOut();
        $("#overlay").hide();
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
        context: {
            ShippingAddressID: 0
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
        //修改收货地址
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
        //删除收货地址
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
        //设置默认地址
        setAsDefaultShipping: function (element) {
            var $this = $(element);
            var sysno = parseInt($this.parents("li").attr("sysno"));
            if (isNaN(sysno)) return;

            var postData = { cmd: "default", id: sysno };
            _ajaxProcessor(postData, "text", Resources.ajaxEditShippingAddressURL, function (data) {
                if (data == "y") alert('设置成功！');
            }, false);
        },
        //保存地址
        saveShippingAddress: function () {
            var $checkedItem = $(".myaddrlist li input[name='addSel']:checked");
            if ($("#shippingAddressEditPanel").is('.expand')) {
                CheckOut.jqValidator.submitForm(false);
            }
            else {
                $("#shippingAddressContent").attr('shippingaddrsysno', $checkedItem.val());
                var $container = $checkedItem.parent();
                $("#DistrictId").val($container.parent().attr("aid"));//返回地区
                $("#BackAddress").val($('span[class^="addrinfo"]:eq(1)', $container).text());//返还联系地址
                $("#BackTelephone").val($('span[class="phone"]:eq(0)>span', $container).text());//返还联系电话
                $("#Contact").val($('span[class="recname"]:eq(0)', $container).text());//返还联系人
                var html = "<h2 class=\"mt30\">收货人信息<a href=\"javascript:void(0)\" onclick=\"CheckOut.expandShippingAddress()\" class=\"fz12 blue ml20\">[修改]</a><span class=\"tip\"></span></h2> \
                    <div class=\"selectads_seleced mb20\" style=\"padding-left: 32px;\"> \
                        <p class=\"p5_0\"> \
                            <span>" + $('span[class="rectitle"]:eq(0)', $container).text() + "</span><span class=\"ml10\">" + $('span[class="recname"]:eq(0)', $container).text()
                                + "</span><span class=\"ml10\">" + $('span[class="phone"]:eq(0)>span', $container).text() + "</span> \
                        </p> \
                        <p class=\"p5_0\">" + $('span[class^="addrinfo"]:eq(0)', $container).text() + $('span[class^="addrinfo"]:eq(1)', $container).text() + "</p> \
                    </div>"
                $("#shippingAddressContent").html(html);

                $("[data-role='section']>h2>a").show();
                $("[data-role='section']>h2>.tip").html("").hide();
                $("#shippingAddressContent").removeClass("step-box-cur");
            }
        },

        //显示收货地址列表
        expandShippingAddress: function () {
            CheckOut.context.ShippingAddressID = parseInt($("#shippingAddressContent").attr('shippingaddrsysno'));
            $("[data-role='section']>h2>a").hide();
            $("[data-role='section']>h2>.tip").html("如需修改，请先保存收货人信息").show();
            $('.step-box').removeClass('step-box-cur');
            _ajaxProcessor(CheckOut.context, "html", Resources.ajaxGetShippingAddressListURL, function (data) {
                $("#shippingAddressContent").html(data).addClass('step-box-cur');
                CheckOut.initShippingAddressPanel();
                _scrollToAnchor("#shippingAddressContent");
            });
        }


    }
    CheckOut.ui = {
        showMask: _showMask,
        hideMask: _hideMask
    };
    window.CheckOut = CheckOut;

    
})()