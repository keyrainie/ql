
usingNamespace("Biz.AccountCenter")["ProductNotify"] = {
    CreateProductNotify: function (productSysNo) {
        var email = $("#email").val();
        $("#msg").html("");
        if (!Biz.Common.Validation.isEmail(email)) {
            $("#msg").html("请输入正确的邮箱地址");
            return;
        }
        $.ajax({
            type: "post",
            url: "/MemberAccount/AjaxCreateProductNotify",
            dataType: "json",
            data: { ProductSysNo: productSysNo, Email: encodeURI(email) },
            beforeSend: function (XMLHttpRequest) {
                $("#btnSubmit").hide();
                $("#msg").html("正在提交...");
            },
            success: function (data) {
                $("#btnSubmit").show();
                $("#msg").html("");

                if (!data.Result) {
                    $("#msg").html(data.Message);
                } else {
                    $("#msg").html(data.Message);
                    $("#btnSubmit").remove();
                    $("#successMsg").show();
                }
            }
        });
    },
    CheckedProduct: function () {
        var sysNoList = new Array();
        $(".productNotify").find("input:checked").each(function () {
            var sysNo = $(this).val();
            sysNoList.push(sysNo);
        });
        if (sysNoList.length == 0) {
            alert("请选中操作项！");
            return "";
        }
        return sysNoList.join(",");
    },
    UpdateProductNotify: function () {
        var sysNoStr = Biz.AccountCenter.ProductNotify.CheckedProduct();
        if (sysNoStr == "") return;
        $.ajax({
            type: "post",
            url: "/MemberAccount/AjaxUpdateProductNotify",
            dataType: "json",
            data: { SelectList: sysNoStr },
            success: function (data) {
                alert(data);
                location.reload();
            }
        });
    },
    DeleteProductNotify: function () {
        var sysNoStr = Biz.AccountCenter.ProductNotify.CheckedProduct();
        if (sysNoStr == "") return;
        if (confirm("确定要删除吗？")) {
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxDeleteProductNotify",
                dataType: "json",
                data: { SelectList: sysNoStr },
                success: function (data) {
                    alert(data);
                    location.reload();
                }
            });
        }
    },
    ClearProductNotify: function () {
        if (confirm("确定清除所有吗？")) {
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxClearProductNotify",
                dataType: "json",
                success: function (data) {
                    alert(data);
                    location.reload();
                }
            });
        }
    },
    RefreshProductNotify: function () {
        location.reload();
    }
};

usingNamespace("Biz.AccountCenter")["Pricenotify"] = {
    Create: function (productSysNo, instantPrice) {
        $("#msg").html("");

        var expectedPrice = $("#expectedPrice").val();
        var isFavorite = $("#chkFafavor").is(":checked");

        if (!Biz.AccountCenter.Pricenotify.CheckedInt(expectedPrice) || expectedPrice > instantPrice || expectedPrice < 1) {
            $("#msg").html("请输入正确的价格");
            $("#expectedPrice").focus();
            return;
        }

        $.ajax({
            type: "post",
            url: "/MemberAccount/AjaxCreateProductPriceNotify",
            dataType: "json",
            data: { IsFavorite: isFavorite, ProductSysNo: productSysNo, InstantPrice: instantPrice, ExpectedPrice: expectedPrice },
            beforeSend: function (XMLHttpRequest) {
                $("#btnSubmit").hide();
                $("#msg").html("正在提交...");
            },
            success: function (data) {
                $("#btnSubmit").show();
                $("#msg").html("");

                if (!data.Result) {
                    $("#msg").html(data.Message);
                } else {
                    $("#btnSubmit").remove();
                    $("#msg").html("提交成功！");
                    $("#successMsg").show();
                    if (isFavorite) {
                        $("#btnFavor").removeClass("btn_addfavorB");
                        $("#btnFavor").addClass("btn_favoredB");
                    }
                }
            }
        });
    },
    AddFavor: function (productSysNo) {
        $.ajax({
            url: "/MemberService/AjaxAddProductToWishList",
            type: 'post',
            dataType: 'json',
            data: { ProductSysNo: productSysNo },
            success: function (data) {
                if (!data.error) {
                    $("#btnFavor").removeClass("btn_addfavorB");
                    $("#btnFavor").addClass("btn_favoredB");
                    alert("加入收藏成功！");
                }
            }
        })
    },
    CheckedAll: function (isTrue) {
        $(".tabc").find("input[type=checkbox]").each(function () {
            if (isTrue) {
                $(this).attr("checked", isTrue);
            } else {
                if ($(this).attr("checked")) {
                    $(this).attr("checked", false);
                }
                else {
                    $(this).attr("checked", true);
                }
            }
        });
    },
    CheckedProduct: function () {
        var sysNoList = new Array();
        $(".tabc").find("input:checked").each(function () {
            var sysNo = $(this).val();
            sysNoList.push(sysNo);
        });
        if (sysNoList.length == 0) {
            alert("请选中操作项！");
            return "";
        }
        return sysNoList.join(",");
    },
    Update: function (sysNo, instantPrice) {
        var expectedPrice = $("#exPrice_" + sysNo).val();
        if (!Biz.AccountCenter.Pricenotify.CheckedInt(expectedPrice) || expectedPrice > instantPrice || expectedPrice < 1) {
            alert("请输入正确的价格");
            $(txtObj).focus();
            return;
        }

        $.ajax({
            type: "post",
            url: "/MemberAccount/AjaxUpdateProductPriceNotify",
            dataType: "json",
            data: { SysNo: sysNo, InstantPrice: instantPrice, ExpectedPrice: expectedPrice },
            success: function (data) {
                alert(data);
                location.reload();
            }
        });
    },
    Cancel: function (sysNo) {
        if (confirm("确定要取消订阅吗？")) {
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxCancelProductPriceNotify",
                dataType: "json",
                data: { SysNo: sysNo },
                success: function (data) {
                    alert(data);
                    location.reload();
                }
            });
        }
    },
    Delete: function (sysNo) {
        var sysNoStr = sysNo;
        if (sysNo == 0) {
            sysNoStr = Biz.AccountCenter.Pricenotify.CheckedProduct();
            if (sysNoStr == "") return;
        }

        if (confirm("确定要删除吗？")) {
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxDeleteProductPriceNotify",
                dataType: "json",
                data: { SelectList: sysNoStr },
                success: function (data) {
                    alert(data);
                    location.reload();
                }
            });
        }
    },
    Clear: function () {
        if (confirm("确定清空所有吗？")) {
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxClearProductPriceNotify",
                dataType: "json",
                success: function (data) {
                    alert(data);
                    location.reload();
                }
            });
        }
    },
    CheckedInt: function (value) {
        var strP = /^\d+(\.\d+)?$/;
        return strP.test(value);
    }
};

usingNamespace("Biz.Customer")["GiftCard"] = {
    QueryGiftCard: function () {
        if (Biz.Customer.GiftCard.CheckInput()) {
            var code = $("#input_Code").val();
            var password = $("#input_Pwd").val();
            $.ajax({
                type: "post",
                url: "/GiftCard/AjaxLoadGiftCard",
                dataType: "json",
                data: { Code: code, Password: password },
                success: function (data) {
                    if (!data.Result) {
                        var errorObj = $("#showmsg").find(".Validform_checktip");
                        $(errorObj).html(data.Message).show();
                    } else {
                        $(".lately_order").show();
                        var dataObj = $(".lately_order").find("td");
                        $(dataObj[1]).html(data.Message.Code);
                        $(dataObj[3]).html(data.Message.EndDate);
                        $(dataObj[5]).html(data.Message.AvailAmount);
                        $(dataObj[7]).html(data.Message.Status);
                    }
                }
            });
        }
    },
    BindGiftCard: function () {
        if (Biz.Customer.GiftCard.CheckInput()) {
            var code = $("#input_Code").val();
            var password = $("#input_Pwd").val();
            $.ajax({
                type: "post",
                url: "/GiftCard/AjaxBindGiftCard",
                dataType: "json",
                data: { Code: code, Password: password },
                success: function (data) {
                    if (!data.Result) {
                        var errorObj = $("#showmsg").find(".Validform_checktip");
                        $(errorObj).html(data.Message).show();
                    } else {
                        alert(data.Message);
                    }
                }
            });
        }
    },
    CheckInput: function () {
        var code = $("#input_Code").val();
        var password = $("#input_Pwd").val();
        var errorObj = $("#showmsg").find(".Validform_checktip");
        $(errorObj).hide();
        if (code == "") {
            $(errorObj).html("卡号不能为空").show();
            $("#input_Code").focus();
            return false;
        }
        if (password == "") {
            $(errorObj).html("密码不能为空").show();
            $("#input_Pwd").focus();
            return false;
        }
        return true;
    },
    ChangePwd: function () {
        var code = $("#input_Code").val();
        var password = $("#input_Pwd").val();
        var newPassword = $("#input_NewPwd").val();
        var confirmPassword = $("#input_ConfirmPwd").val();

        var errorObj = $("#showmsg").find(".Validform_checktip");
        $(errorObj).hide();

        if (code == "") {
            $(errorObj).html("卡号不能为空").show();
            $("#input_Code").focus();
            return;
        }
        if (password == "") {
            $(errorObj).html("旧密码不能为空").show();
            $("#input_Pwd").focus();
            return;
        }
        if (newPassword == "") {
            $(errorObj).html("新密码不能为空").show();
            $("#input_NewPwd").focus();
            return;
        }
        var filter = /^[a-zA-Z0-9]{6}$/;
        if (!filter.test(newPassword)) {
            $(errorObj).html("新密码必须为6位字母数字").show();
            $("#input_NewPwd").focus();
            return;
        }
        if (confirmPassword == "") {
            $(errorObj).html("确认密码不能为空").show();
            $("#input_ConfirmPwd").focus();
            return;
        }
        if (newPassword != confirmPassword) {
            $(errorObj).html("新密码与确认密码不相同").show();
            return;
        }
        if (newPassword == password) {
            $(errorObj).html("新密码不能与旧密码相同").show();
            return;
        }
        $.ajax({
            type: "post",
            url: "/GiftCard/AjaxModifyGiftCardPwd",
            dataType: "json",
            data: { Code: code, Password: password, NewPassword: newPassword },
            success: function (data) {
                if (!data.Result) {
                    var errorObj = $("#showmsg").find(".Validform_checktip");
                    $(errorObj).html(data.Message).show();
                } else {
                    alert(data.Message);
                }
            }
        });
    }
}
