var currentProductSelectorFocusID;
var grid;
$(function () {

    $(".date-range").defaultDateRangePicker();
    $('.bs-select').selectpicker({
        iconBase: 'fa',
        tickIcon: 'fa-check'
    });

    var $conForm = $('#condForm').bootstrapValidator({
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            ActivitySysNo: {
                validators: {
                    digits: {
                        message: '活动编号格式不正确'
                    }
                }
            }
        }
    }).on("success.form.bv", function (e, data) {

        //var $parent = data.element.parents('.form-group');
        //// Remove the has-success class
        //$parent.removeClass('has-success');
        //// Hide the success icon
        //$parent.find('.form-control-feedback[data-bv-icon-for="' + data.field + '"]').hide();
        grid.clearAjaxParams();
        var queryFilter = $.buildEntity($("#condForm"));
        if (queryFilter.ActivityStatus == "-999") {
            queryFilter.ActivityStatus = null;
        }
        if (queryFilter.ActivityType == "-999") {
            queryFilter.ActivityType = null;
        }
        queryFilter.MainProductSysNo = $("#searchProductNo_btnSearchMainProduct").val();
        queryFilter.GiftProductSysNo = $("#searchProductNo_btnSearchGiftProduct").val();
        grid.addAjaxParam("queryfilter", $.toJSON(queryFilter));
        grid.getDataTable().ajax.reload();
    });


    grid = new Datatable();
    grid.init({
        src: $("#giftPromotionList_Grid"),
        dataTable: {
            //绑定列
            "columns": [
            { "orderable": false },
            { "mData": "SysNo", "orderable": true },
            { "mData": "PromotionName", "orderable": true },
            { "mData": "TypeName", "orderable": true },
            { "mData": "StatusName", "orderable": true },
            { "mData": "BeginDateString", "orderable": true },
            { "mData": "EndDateString", "orderable": true },
            { "mData": "InDateString", "orderable": true },
            { "mData": "InUser", "orderable": true }
            ],
            "columnDefs": [
              //CheckBox:
                   {
                       "render": function (data, type, row) {
                           return '<input type="checkbox" value="' + row.SysNo + '" />';
                       },
                       "targets": 0
                   },

           {
               "name": "SysNo",
               "render": function (data, type, row) {
                   return '<a href="/Promotion/GiftPromotionEdit?sysNo=' + row.SysNo + '" id="btnEdit_' + row.SysNo + '" target="_blank">' + row.SysNo + '</a>';
               },
               "targets": 1
           },
           { "name": "PromotionName", "targets": 2 },
           { "name": "Type", "targets": 3 },
           { "name": "Status", "targets": 4 },
           { "name": "BeginDate", "targets": 5 },
           { "name": "EndDate", "targets": 6 },
           { "name": "InDate", "targets": 7 },
           { "name": "InUser", "targets": 8 }
            ],
            "ajax": {
                "url": "/Promotion/GetGiftPromotionList",
                "type": "POST"
            },
            //默认排序
            "order": [[1, "desc"]]
        }
    });

    $("#btnSearch").click(function () {
        $('#condForm').bootstrapValidator('validate');
    });

    $("#btnReset").click(function () {
        $conForm.data('bootstrapValidator').resetForm(true);
        $("input[name='ActivityName']").val('');
        $('.bs-select option[value=-999]').attr("selected", "selected");
        $(".bs-select").change();
        $(".date-range").daterangepicker("clear");

        $("#searchProductNo_btnSearchMainProduct").val('');
        $("#searchProductTitle_btnSearchMainProduct").val('');
        $("#searchProductNo_btnSearchMainProduct").val('');
        $("#searchProductNo_btnSearchGiftProduct").val('');
        $("#searchProductTitle_btnSearchGiftProduct").val('');

    })

});

function batchPublish() {
    var selectedSysNoList = grid.getSelectedRows();
    if (selectedSysNoList.length <= 0) {
        $.alert('请勾选至少一个活动!');
        return;
    }

    $.confirm('你确定要批量发布所勾选的活动吗?', function (r) {
        if (r) {
            $.ajax({
                url: "/Promotion/AjaxBatchPublish",
                type: "POST",
                dataType: "json",
                data: { sysNoList: $.toJSON(selectedSysNoList) },
                success: function (data) {
                    if (!data.error) {
                        var resultMsg = '操作完成！成功条数:' + data.successCount + ',失败条数:' + data.failCount + (data.resultMsg.length > 0 ? '<br/>失败原因:<br/>' + data.resultMsg.toString() : '');

                        $.alert(resultMsg, function () {
                            $("#btnSearch").click();
                        });
                    }
                }
            });
        }
    });
}
function batchAbandon() {
    var selectedSysNoList = grid.getSelectedRows();
    if (selectedSysNoList.length <= 0) {
        $.alert('请勾选至少一个活动!');
        return;
    }
    $.confirm('你确定要批量作废所勾选的活动吗?', function (r) {
        if (r) {
            $.ajax({
                url: "/Promotion/AjaxBatchAbandon",
                type: "POST",
                dataType: "json",
                data: { sysNoList: $.toJSON(selectedSysNoList) },
                success: function (data) {
                    if (!data.error) {
                        var resultMsg = '操作完成！成功条数:' + data.successCount + ',失败条数:' + data.failCount + (data.resultMsg.length > 0 ? '<br/>失败原因:<br/>' + data.resultMsg.toString() : '');

                        $.alert(resultMsg, function () {
                            $("#btnSearch").click();
                        });
                    }

                }
            });
        }
    });
}
function batchTeminate() {
    var selectedSysNoList = grid.getSelectedRows();
    if (selectedSysNoList.length <= 0) {
        $.alert('请勾选至少一个活动!');
        return;
    }
    $.confirm('你确定要批量终止所勾选的活动吗?', function (r) {
        if (r) {
            $.ajax({
                url: "/Promotion/AjaxBatchTerminate",
                type: "POST",
                dataType: "json",
                data: { sysNoList: $.toJSON(selectedSysNoList) },
                success: function (data) {
                    if (!data.error) {
                        var resultMsg = '操作完成！成功条数:' + data.successCount + ',失败条数:' + data.failCount + (data.resultMsg.length > 0 ? '<br/>失败原因:<br/>' + data.resultMsg.toString() : '');

                        $.alert(resultMsg, function () {
                            $("#btnSearch").click();
                        });
                    }

                }
            });
        }
    });
}