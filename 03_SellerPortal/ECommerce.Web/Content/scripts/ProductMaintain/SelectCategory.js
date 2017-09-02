var SelectCategory = {
    SelectC3SysNo: 0,
    //选择分类1
    SelectCategory1: function () {
        var sysNo = parseInt($("#C1Select").val());
        if (sysNo > 0) {
            SelectCategory.LoadCategory2(sysNo);
            var html = '<option value=0>--请选择--</option>';
            $("#C3Select").html(html);
        }
        else {
            $("#C2Select").html('<option value=0>--请选择--</option>');
            $("#C3Select").html('<option value=0>--请选择--</option>');
            $('#C2Select').selectpicker('refresh');
            $('#C3Select').selectpicker('refresh');
        }
    },
    //加载分类2
    LoadCategory2: function (sysNo) {
        $.ajax({
            url: "ProductMaintain/AjaxCategory2List?sysno=" + sysNo,
            type: "POST",
            dataType: "json",
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    var html = '<option value=0>--请选择--</option>';
                    for (var i = 0; i < data.length; i++) {
                        html += '<option value=' + data[i].SysNo + '>' + data[i].CategoryName + '</option>';
                    }
                    $("#C2Select").html(html);
                    $('#C2Select').selectpicker('refresh');
                    $('#C3Select').selectpicker('refresh');
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    },
    //选择分类2
    SelectCategory2: function () {
        var sysNo = parseInt($("#C2Select").val());
        if (sysNo > 0) {
            SelectCategory.LoadCategory3(sysNo);
        }
        else {
            $("#C3Select").html('<option value=0>--请选择--</option>');
            $('#C3Select').selectpicker('refresh');
        }
    },
    //加载分类3
    LoadCategory3: function (sysNo) {
        $.ajax({
            url: "ProductMaintain/AjaxCategory3List?sysno=" + sysNo,
            type: "POST",
            dataType: "json",
            beforeSend: function (XMLHttpRequest) {
                $.showLoading();
            },
            success: function (data) {
                if (!data.message) {
                    var html = '<option value=0>--请选择--</option>';
                    for (var i = 0; i < data.length; i++) {
                        html += '<option value=' + data[i].SysNo + '>' + data[i].CategoryName + '</option>';
                    }
                    $("#C3Select").html(html);
                    $('#C3Select').selectpicker('refresh');
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                $.hideLoading();
            }
        });
    },
    //选择分类3
    SelectCategory3: function () {
        SelectCategory.SelectC3SysNo = parseInt($("#C3Select").val());
    },
    //下一步
    NextStep: function () {
        if (SelectCategory.SelectC3SysNo > 0) {
            location.href = '/ProductMaintain/BasicInfo?CategorySysNo=' + SelectCategory.SelectC3SysNo;
        }
        else {
            $.alert("请选择分类！");
        }
    }
}