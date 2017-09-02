$(function () {
    //初始化元素选择模态窗口
    $("#ProductEditModal").modal({
        show: false
    }).on("shown.bs.modal", function () {
        $.ajax({
            type: "GET",
            url: "/ProductMaintain/AjaxLoadEditProduct?ProductSysNo=" + ProductEdit.ProductSysNo,
            dataType: "html",
            success: function (data) {
                $("#ProductEditModal").find(".modal-content").html(data);
                $('.date-picker').datepicker({
                    dateFormat: 'yyyy-mm-dd',
                    rtl: Metronic.isRTL(),
                    orientation: "left",
                    autoclose: true,
                    language: "zh-CN"
                });
                $(".date-range").defaultDateRangePicker();
            }
        });
    }).on("hide.bs.modal", function (e) {
        $("#ProductEditModal").find(".modal-content").html("");
    });
});

//商品编辑
var ProductEdit = {
    CallbackFunction: null,
    //商品编号
    ProductSysNo: 0,
    //打开编辑窗口
    Open: function (productSysNo, callback) {
        ProductEdit.CallbackFunction = callback;
        ProductEdit.ProductSysNo = productSysNo;
        $("#ProductEditModal").modal("show");
    },
    Save: function () {
        var data = $.buildEntity($("#MaintainForm"));
        var regNumber = /^[0-9]*$/;
        var reg = /^\d{0,8}\.{0,1}(\d{1,2})?$/;
        var strRegUrl = "^(https://|http://)"
                        + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //ftp的user@
                        + "(([0-9]{1,3}.){3}[0-9]{1,3}" // IP形式的URL- 199.194.52.184
                        + "|" // 允许IP和DOMAIN（域名）
                        + "([0-9a-z_!~*'()-]+.)*" // 域名- www.
                        + "([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]." // 二级域名
                        + "[a-z]{2,6})" // first level domain- .com or .museum
                        + "(:[0-9]{1,4})?" // 端口- :80
                        + "((/?)|" // a slash isn't required if there is no file name
                        + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
        var regUrl = new RegExp(strRegUrl);
        if ($.trim(data.ProductMode).length == 0) {
            $.alert("请输入型号！");
            return false;
        }
        if (!regNumber.test(data.Weight)) {
            $.alert("重量必须输入大于等于0的整数！");
            return false;
        }
        if (parseInt(data.Weight) <= 0)
        {
            $.alert("重量必须输入大于等于0的整数！");
            return false;
        }
        if (!reg.test(data.Length)) {
            $.alert("长度必须输入大于等于0的整数或者2位小数！");
            return false;
        }
        if (!reg.test(data.Width)) {
            $.alert("宽度必须输入大于等于0的整数或者2位小数！");
            return false;
        }
        if (!reg.test(data.Height)) {
            $.alert("高度必须输入大于等于0的整数或者2位小数！");
            return false;
        }
        if ($.trim(data.ProductOutUrl).length > 0 && !regUrl.test(data.ProductOutUrl)) {
            $.alert("请填写正确的网址，以http://开头！");
            return false;
        }

        $.ajax({
            url: "/ProductMaintain/AjaxSaveSingleProductInfo",
            type: "POST",
            dataType: "json",
            data: { data: encodeURI(JSON.stringify(data)) },
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    $.confirm("保存商品信息成功！", function () {
                        if (ProductEdit.CallbackFunction) {
                            $("#ProductEditModal").modal("hide");
                            ProductEdit.CallbackFunction();
                        }
                        else {
                            location.reload();
                        }
                    });
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    }
}