$(document).ready(function () {
    $("#coverFileUploader").uploadify({
        "swf": "/Content/third/uploadify/uploadify.swf",
        "buttonText": "选择商品图片",
        "uploader": ImageInfo.ImageServer + "/UploadHandler.ashx?appName=product",
        "auto": false,
        "onSelect": function (file) {
            ImageInfo.SelectImageCount += 1;
            $("#btnUpload").show();
        },
        "onUploadSuccess": function (file, data, response) {
            var data = JSON.parse(data);
            if (data.state != "SUCCESS") {
                $.alert(data.message);
            }
            else {
                ImageInfo.SaveImageInfo(data.url);
            }
        },
        "onUploadError": function (file, errorCode, errorMsg, errorString) {
            if (errorCode != "-280") {
                //$("#btnUpload").hide();
                $.alert("上传失败请重试。", function () {
                    window.location.reload();
                });
            }
            else {
                ImageInfo.SelectImageCount -= 1;
            }
        }
    });
    $("#btnUpload").click(function () {
        if (!ImageInfo.CheckSelectProduct()) {
            $.alert("请选择商品！");
            return;
        }
        $("#coverFileUploader").uploadify("upload");        
    });
    $("input[CbxType=AllProduct]").click(function () {
        if ($(this).attr("checked") == "checked") {
            $("#BasicInfoForm").find("input").attr("checked", true);
        }
        else {
            $("#BasicInfoForm").find("input").attr("checked", false);
        }
        Metronic.updateUniform();
    });
});

var ImageInfo = {
    //Image服务器地址
    ImageServer: "",
    //商品组编号
    ProductGroupSysNo: 0,
    //选择的图片数
    SelectImageCount: 0,
    //选择的商品编号
    SelectProductSysNos: [],
    //检查是否选择商品
    CheckSelectProduct: function () {
        ImageInfo.SelectProductSysNos = [];
        $("#BasicInfoForm").find("input").each(function () {
            if ($(this).attr("checked") == "checked" && $(this).attr("CbxType") == "NormalProduct") {
                ImageInfo.SelectProductSysNos.push(parseInt($(this).val()));
            }
        });
        if (ImageInfo.SelectProductSysNos.length > 0)
            return true;
        return false;
    },
    //保存上传的图片
    SaveImageInfo: function (imagePath) {
        var dataList = [];
        for (var i = 0; i < ImageInfo.SelectProductSysNos.length; i++) {
            dataList.push({
                ProductSysNo: ImageInfo.SelectProductSysNos[i],
                ResourceUrl: imagePath
            });
        }
        $.ajax({
            url: "AjaxSaveImageInfo",
            type: "POST",
            dataType: "json",
            data: { data: encodeURI(JSON.stringify(dataList)) },
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    ImageInfo.ContinueUpload();
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    },
    //继续上传
    ContinueUpload: function () {
        ImageInfo.SelectImageCount -= 1;
        if (ImageInfo.SelectImageCount > 0) {
            $("#coverFileUploader").uploadify("upload");
        }
        else {
            $.alert("上传商品图片成功！", function () {
                location.reload();
            });
        }
    },
    //更新优先级
    UpdatePriority: function (obj) {
        var reg = /^[0-9]*$/;
        var strPriority = $(obj).val();
        if ($.trim(strPriority).length == 0) {
            $(obj).val($(obj).attr("OldValue"));
            $.alert("请输入排序值！");
            return;
        }
        if (!reg.test(strPriority)) {
            $(obj).val($(obj).attr("OldValue"));
            $.alert("商品图片排序顺序值只能输入正整数！");
            return;
        }
        if ($(obj).attr("OldValue") == strPriority)
            return;
        var postData = {
            SysNo: parseInt($(obj).attr("SysNo")),
            Priority: parseInt(strPriority)
        };
        $.ajax({
            url: "AjaxUpdateProductImagePriority",
            type: "POST",
            dataType: "json",
            data: { data: encodeURI(JSON.stringify(postData)) },
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    $(obj).attr("OldValue", strPriority);
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    },
    //全选/反选商品图片
    SelectAllImage: function (obj) {
        var productSysNo = parseInt($(obj).attr("ProductSysNo"));
        if ($(obj).attr("checked") == "checked") {
            $("input[SelectProduct=" + productSysNo + "]").attr("checked", true);
        }
        else {
            $("input[SelectProduct=" + productSysNo + "]").attr("checked", false);
        }
        Metronic.updateUniform();
    },
    //批量删除图片
    BatchDelete: function (obj) {
        var sysNos = [];
        sysNos.push(ImageInfo.ProductGroupSysNo);
        var productSysNo = parseInt($(obj).attr("ProductSysNo"));
        $("input[SelectProduct=" + productSysNo + "]").each(function () {
            if ($(this).attr("checked") == "checked")
                sysNos.push(parseInt($(this).attr("SysNo")));
        });
        if (sysNos.length == 1) {
            $.alert("请选择图片！");
            return;
        }
        
        $.confirm("您确定要删除选中的图片吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "AjaxDeleteProductImageBySysNo",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(sysNos)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.alert("批量删除图片成功！", function () {
                            location.reload();
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    $.hideLoading();
                }
            });
        });
    },
    //设置商品默认图片
    SetDefaultImage: function (obj) {
        var sysNos = [];
        var productSysNo = parseInt($(obj).attr("ProductSysNo"));
        $("input[SelectProduct=" + productSysNo + "]").each(function () {
            if ($(this).attr("checked") == "checked")
                sysNos.push(parseInt($(this).attr("SysNo")));
        });
        if (sysNos.length == 0) {
            $.alert("请选择图片！");
            return;
        }
        if (sysNos.length > 1) {
            $.alert("只能选择1张图片！");
            return;
        }
        var postData = {
            ProductSysNo: productSysNo,
            SysNo: sysNos[0]
        };

        $.confirm("您确定要将该图片设为默认图片吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "AjaxSetProductDefaultImage",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(postData)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.alert("设置默认图片成功！", function () {
                            location.reload();
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    $.hideLoading();
                }
            });
        });
    }
}