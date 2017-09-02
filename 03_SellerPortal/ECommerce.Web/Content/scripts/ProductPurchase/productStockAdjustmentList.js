var adjustListManager = {
    init: function () {
        $(".date-range").defaultDateRangePicker();
        $('.bs-select').selectpicker({
            iconBase: 'fa',
            tickIcon: 'fa-check'
        });


        var grid = new Datatable();
        grid.init({
            src: $("#productStockAdjustmentList_Grid"),
            dataTable: {
                //绑定列
                "columns": [
                { "orderable": false },
                { "mData": "SysNo", "orderable": true },
                { "mData": "StockName", "orderable": true },
                { "mData": "StatusText", "orderable": true },
                { "mData": "CurrencyCodeText", "orderable": true },
                { "mData": "InUserName", "orderable": true },
                { "mData": "InDateText", "orderable": true },
                { "mData": "EditUserName", "orderable": true },
                { "mData": "EditDateText", "orderable": true },
                { "mData": "AuditUserName", "orderable": true },
                { "mData": "AuditDateText", "orderable": true }
                ],
                "columnDefs": [
                //编辑
               {
                   "render": function (data, type, row) {
                       return '<a href="/ProductPurchase/ProductStockAdjustmentMain?sysNo=' + row.SysNo + '" id="btnEdit_' + row.SysNo + '">查看</a>';
                   },
                   "targets": 0
               },
               { "name": "ps.SysNo", "targets": 1 },
               { "name": "ps.StockSysNo", "targets": 2 },
               { "name": "ps.Status", "targets": 3 },
               { "name": "ps.CurrencyCode", "targets": 4 },
               { "name": "ps.InUserSysNo", "targets": 5 },
               { "name": "ps.InDate", "targets": 6 },
                    { "name": "ps.EditUserSysNo", "targets": 7 },
               { "name": "ps.EditDate", "targets": 8 },
               { "name": "ps.AuditUserSysNo", "targets": 9 },
               { "name": "ps.AuditDate", "targets": 10 }
                ],
                "ajax": {
                    "url": "/ProductPurchase/QueryProductStockAdjustList",
                    "type": "POST"
                },
                //默认排序
                "order": [[1, "desc"]]
            }
        });



        $("#btnSearch").click(function () {
            grid.clearAjaxParams();
            var queryFilter = $.buildEntity($("#condForm"));
            if (queryFilter.StockSysNo == "-999") {
                queryFilter.StockSysNo = null;
            }
            if (queryFilter.Status == "-999") {
                queryFilter.Status = null;
            }
            queryFilter.ProductSysNo = $("#searchProductNo_inputChooseProduct").val();
            grid.addAjaxParam("queryfilter", $.toJSON(queryFilter));
            grid.getDataTable().ajax.reload();
        });

        $("#btnReset").click(function () {
            $("input[name='SysNo']").val('');
            $('.bs-select option[value=-999]').attr("selected", "selected");
            $(".bs-select").change();
            $(".date-range").daterangepicker("clear");
            $("#searchProductNo_inputChooseProduct").val('');
            $("#searchProductTitle_inputChooseProduct").val('');
        })
    },
    getSelectedProductData: function (id, data) {
        //alert(id);
        //alert(data.ProductTitle);
    }
}