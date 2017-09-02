$(document).ready(function () {
    var grid = new Datatable();
    grid.init({
        src: $("#datatable_ajax"),
        dataTable: {
            //绑定列
            "columns": [
            { "orderable": false },
            { "mData": "Province.ProvinceSysNo", "orderable": false },
            { "mData": "Province.ProvinceName", "orderable": false },
            { "mData": "Province.CitySysNo", "orderable": false },
            { "mData": "Province.CityName", "orderable": false },
            { "mData": "Stock.StockName", "orderable": false },
            { "mData": "Stock.SysNo", "orderable": false }
            ],
            "columnDefs": [
            //操作:
           {
               "render": function (data, type, row) {
                   return '<a href="javascript:void(0);" id="btnDelete_' + row.Province.CitySysNo + '" onclick="doDelete(' + row.Province.CitySysNo + ')">删除</a>';
               },
               "targets": 0
           },
           { "name": "Province.ProvinceSysNo", "targets": 1 },
           { "name": "Province.ProvinceName", "targets": 2 },
           { "name": "Province.CitySysNo", "targets": 3 },
           { "name": "Province.ProvinceSysNo", "targets": 4 },
            ],
            "pageLength": 10, // default record count per page
            "ajax": {
                "url": "/ProductMaintain/AjaxGetProductSalesAreaInfo",
                "type": "POST"
            },
            "order": [[0, "desc"]],
            "paging": false,
        }
    });
    $(".EditSalesAreaInfo").click(function () {
        $("#ProductTitle").html($(this).attr("ProductTitle"));
        $("#ProductTitle").attr("data", $(this).attr("ProductSysNo"));
        grid.clearAjaxParams();
        var queryFilter = {};
        queryFilter.PageIndex = 0;
        queryFilter.PageSize = 10000;
        queryFilter.ProductSysNo = $(this).attr("ProductSysNo");
        grid.addAjaxParam("queryfilter", $.toJSON(queryFilter));
        grid.getDataTable().ajax.reload();
        $("#SaveSalesArea").show();
    });
    $("#SaveSalesArea").click(function () {
        SalesAreaInfo.SaveSalesArea();
    });
    $("#btnCreate").click(function () {
        SalesAreaInfo.ReadySaveSalesArea();
    });
});
function doDelete(obj) {
    $("#btnDelete_" + obj + "").parent().parent().remove();
}
var SalesAreaInfo = {
    //保存
    SaveSalesArea: function () {
        var data = [];
        data = SalesAreaInfo.GetCreatSalesArea('save');
        if (data == undefined) {
            return;
        }
        $.confirm("保存商品销售区域吗？", function (result) {
            if (!result)
                return;
            $.ajax({
                url: "AjaxSaveProductSalesAreaInfo?ProductSysNo=" + $("#ProductTitle").attr("data"),
                type: "POST",
                dataType: "json",
                data: { data: encodeURI(JSON.stringify(data)) },
                beforeSend: function (XMLHttpRequest) {
                    $.showLoading();
                },
                success: function (data) {
                    if (!data.message) {
                        $.confirm("保存商品销售区域成功！", function () {
                            $("#SaveSalesArea").hide();
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    $.hideLoading();
                }
            });
        });

    },

    //预保存
    ReadySaveSalesArea: function () {
        var Readydata = [];
        Readydata = SalesAreaInfo.GetSelectedCreatReadySalesArea();
        if (Readydata.length == 0) {
            $.alert("请先选择需要新增的地区！！！");
        }
        var data = [];
        data = SalesAreaInfo.GetCreatSalesArea('Ready');
        for (var i = 0; i < data.length; i++) {
            for (var k = 0; k < Readydata.length; k++) {
                if (Readydata[k].CitySysNo == data[i].CitySysNo) {
                    //Readydata.pop(Readydata[k]);
                    Readydata.splice(k, 1);
                }
            }
        }
        var StockSysNo = $("#StockSysNo").val();
        var StockSysName = $("#StockSysNo option:selected").text();
        for (var i = 0; i < Readydata.length; i++) {
            $("#datatable_ajax tbody").append("<tr role='row' class='even'>" +
                                                          "<td class='sorting_1'><a onclick='doDelete(" + Readydata[i].CitySysNo + ")' id='btnDelete_" + Readydata[i].CitySysNo + "' href='javascript:void(0);'>删除</a></td>" +
                                                          "<td>" + Readydata[i].ProvinceSysNo + "</td>" +
                                                          "<td>" + Readydata[i].ProvinceName + "</td>" +
                                                          "<td>" + Readydata[i].CitySysNo + "</td>" +
                                                          "<td>" + Readydata[i].CityName + "</td>" +
                                                          "<td>" + StockSysName + "</td>" +
                                                          "<td>" + StockSysNo + "</td>" +
                                                      "</tr>");
        }
    },
    //获得要新增的区域编号
    GetCreatSalesArea: function (action) {
        if ($("#datatable_ajax tbody tr").children().eq(0).html() == "暂无相关查询记录" && action == "Ready") {
            $("#datatable_ajax tbody tr").children().eq(0).remove();
            var data = [];
            $("#datatable_ajax tbody tr").each(function () {
                data.push({
                    ProvinceSysNo: $(this).children().eq(1).html(),
                    ProvinceName: $(this).children().eq(2).html(),
                    CitySysNo: $(this).children().eq(3).html(),
                    CityName: $(this).children().eq(4).html(),
                    StockName: $(this).children().eq(5).html(),
                    SysNo: $(this).children().eq(6).html()
                });
            });
            return data;
        }
        if ($("#datatable_ajax tbody tr").children().eq(0).html() == "暂无相关查询记录" && action == "save") {
            $.alert("请先新增需要保存的地区！！！");
            return;
        }
        else {
            var data = [];
            $("#datatable_ajax tbody tr").each(function () {
                data.push({
                    ProvinceSysNo: $(this).children().eq(1).html(),
                    ProvinceName: $(this).children().eq(2).html(),
                    CitySysNo: $(this).children().eq(3).html(),
                    CityName: $(this).children().eq(4).html(),
                    StockName: $(this).children().eq(5).html(),
                    SysNo: $(this).children().eq(6).html()
                });
            });
            return data;
        }
    },
    //获取预新增的区域编号
    GetSelectedCreatReadySalesArea: function () {
        var data = [];
        $('#areaquery_datatable_ajax > tbody > tr > td:nth-child(1) input[type="checkbox"]:checked').each(function () {
            data.push({
                ProvinceSysNo: $(this).parents('tr').children().eq(2).html(),
                ProvinceName: $(this).parents('tr').children().eq(3).html(),
                CitySysNo: $(this).parents('tr').children().eq(4).html(),
                CityName: $(this).parents('tr').children().eq(5).html()
            });
        });
        return data;
    }
}