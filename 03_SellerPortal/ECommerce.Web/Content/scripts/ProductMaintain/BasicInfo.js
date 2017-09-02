$(document).ready(function () {
    BasicInfo.InitEditor();
    $('.bs-select').selectpicker({
        iconBase: 'fa',
        tickIcon: 'fa-check'
    });
    $('#BasicInfoForm').bootstrapValidator({
        message: '输入值无效！',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            ProductName: {
                validators: {
                    notEmpty: {
                        message: '请输入商品名称！'
                    }
                }
            },
            BriefName: {
                validators: {
                    notEmpty: {
                        message: '请输入商品简称！'
                    }
                }
            },
            BrandSysNo: {
                validators: {
                    notEmpty: {
                        message: '请选择品牌！'
                    }
                }
            }
        }
    }).on("success.form.bv", function (e) {
        BasicInfo.Save();
    });
    $("#SaveInfo").click(function () {
        $('#BasicInfoForm').bootstrapValidator('validate');
    });
});

var BasicInfo = {
    ProductGroupSysNo: 0,
    ProductDescLongEditor: null,
    ProductPhotoDescEditor: null,
    //初始化html编辑器
    InitEditor: function () {
        UEDITOR_CONFIG.zIndex = 1;
        BasicInfo.ProductDescLongEditor = new UE.ui.Editor();
        BasicInfo.ProductPhotoDescEditor = new UE.ui.Editor();
        BasicInfo.ProductDescLongEditor.render('ProductDescLongEditor');
        BasicInfo.ProductPhotoDescEditor.render('ProductPhotoDescEditor');
    },
    //保存商品信息
    Save: function () {
        var data = $.buildEntity($("#BasicInfoForm"));
        if ($.trim(data.FrontCategorySysNo).length == 0
            || parseInt(data.FrontCategorySysNo) <= 0) {
            $.alert("请选择前台类别！");
            return;
        }

        $.confirm("您确定要保存商品信息吗？", function (result) {
            if (!result)
                return;

            if (BasicInfo.ProductGroupSysNo > 0) {
                data.ProductGroupSysNo = BasicInfo.ProductGroupSysNo;
            }
            var descData = $.buildEntity($("#DescriptionForm"));
            $.extend(data, descData);
            data.ProductDescLong = BasicInfo.ProductDescLongEditor.getContent();
            data.ProductPhotoDesc = BasicInfo.ProductPhotoDescEditor.getContent();
            //选择属性值
            var properties = [];
            $("select[Property=Normal]").each(function () {
                properties.push({
                    PropertyGroupSysNo: $(this).attr("GroupSysNo"),
                    PropertyGroupName: $(this).attr("GroupName"),
                    PropertySysNo: $(this).attr("PropertySysNo"),
                    PropertyName: $(this).attr("PropertyName"),
                    ValueDescription: $.trim($(this).find("option:selected").text()),
                    IsSplitGroupProperty: $(this).attr("IsSplitGroup"),
                    PropertyValueSysNo: $(this).val()
                });
            });
            data.SelectNormalProperties = properties;
            $.ajax({
                url: "AjaxSaveProductBasicInfo",
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(data)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.confirm("保存商品信息成功！", function () {
                            location.href = 'BasicInfo?ProductGroupSysNo=' + data;
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