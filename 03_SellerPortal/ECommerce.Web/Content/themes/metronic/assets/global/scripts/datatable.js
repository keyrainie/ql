/***
Wrapper/Helper Class for datagrid based on jQuery Datatable Plugin
***/
var Datatable = function () {

    var tableOptions; // main options
    var dataTable; // datatable object
    var table; // actual table jquery object
    var tableContainer; // actual table container object
    var tableWrapper; // actual table wrapper jquery object
    var tableInitialized = false;
    var ajaxParams = {}; // set filter mode
    var the;

    var countSelectedRecords = function () {
        var selected = $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).size();
        var text = tableOptions.dataTable.language.metronicGroupActions;
        if (selected > 0) {
            $('.table-group-actions > span', tableWrapper).text(text.replace("_TOTAL_", selected));
        } else {
            $('.table-group-actions > span', tableWrapper).text("");
        }
    }

    return {

        //main function to initiate the module
        init: function (options) {

            if (!$().dataTable) {
                return;
            }

            the = this;


            // default settings
            options = $.extend(true, {
                src: "", // actual table  
                filterApplyAction: "filter",
                filterCancelAction: "filter_cancel",
                resetGroupActionInputOnSuccess: true,
                loadingMessage: 'Loading...',
                dataTable: {
                    "dom": "<'row pagerHeader'<'col-md-5'<'pull-left'>><'col-md-7'<'pull-right'p>>><'table-responsive't><'row pagerFooter'<'col-md-5'<'pull-left'li>><'col-md-7'<'pull-right'p>>>", // datatable layout
                    "oColVis": {
                        "buttonText": "自定义列",
                        "bRestore": true,
                        "sRestore": "重置",
                        "sAlign": "left"
                    },
                    "pageLength": 10, // default records per page
                    "language": { // language settings
                        // metronic spesific
                        "metronicGroupActions": "共选中了 _TOTAL_ 条记录:  ",
                        "metronicAjaxRequestGeneralError": "Could not complete request. Please check your internet connection",

                        // data tables spesific
                        "lengthMenu": "<span class='seperator'></span>&nbsp;&nbsp;_MENU_",
                        "info": "<span class='seperator'></span>&nbsp;&nbsp;共 _TOTAL_ 条记录",
                        "infoEmpty": "暂无相关查询记录",
                        "emptyTable": "暂无相关查询记录",
                        "zeroRecords": "暂无相关查询记录",
                        "paginate": {
                            "previous": "上一页",
                            "next": "下一页",
                            "last": "最后一页",
                            "first": "首页",
                            "page": "页码",
                            "pageOf": "/"
                        }
                    },

                    "orderCellsTop": true,
                    "pagingType": "bootstrap_full_number", // pagination type(bootstrap, bootstrap_full_number or bootstrap_extended)
                    "autoWidth": false, // disable fixed width and enable fluid table
                    "processing": false, // enable/disable display message box on record load
                    "serverSide": true, // enable/disable server side ajax loading
                    "ajax": { // define ajax settings
                        "url": "", // ajax URL
                        "type": "POST", // request type
                        "timeout": 30000,
                        "data": function (data) { // add request parameters before submit
                            $.each(ajaxParams, function (key, value) {
                                data[key] = value;
                            });
                        }
                    },
                    "fnDrawCallback": function (oSettings) { // run some code on table redraw
                        if (tableInitialized === false) { // check if table has been initialized
                            tableInitialized = true; // set table initialized
                            table.show(); // display table
                        }
                        Metronic.initUniform($('input[type="checkbox"]', table)); // reinitialize uniform checkboxes on each table reload
                        countSelectedRecords(); // reset selected records indicator
                    }
                    , "bProcessing": true
                    , "bServerSide": true
                    , "fnServerData": function (sSource, aoData, fnCallback, oSettings) {

                        if (!this._first) {
                            $("#" + table.attr("id") + "_wrapper .pagerHeader").hide();
                            $("#" + table.attr("id") + "_wrapper .pagerFooter").hide();
                            this._first = true;
                            return;
                        }
                        $("#" + table.attr("id") + "_wrapper .pagerHeader").show();
                        $("#" + table.attr("id") + "_wrapper .pagerFooter").show();
                        var cfg = {};
                        $.extend(cfg, oSettings.ajax);
                        cfg.data = oSettings.oAjaxData;
                        $.extend(cfg.data, ajaxParams);
                        cfg.success = fnCallback;
                        oSettings.jqXHR = $.ajax(cfg);
                        if (table.find("input").attr("checked") == "checked") {
                            table.find("input").click();
                        }
                    }
                }
            }, options);

            tableOptions = options;

            // create table's jquery object
            table = $(options.src);
            tableContainer = table.parents(".table-container");

            // apply the special class that used to restyle the default datatable
            var tmp = $.fn.dataTableExt.oStdClasses;

            $.fn.dataTableExt.oStdClasses.sWrapper = $.fn.dataTableExt.oStdClasses.sWrapper + " dataTables_extended_wrapper";
            $.fn.dataTableExt.oStdClasses.sFilterInput = "form-control input-small input-sm input-inline";
            $.fn.dataTableExt.oStdClasses.sLengthSelect = "form-control input-xsmall input-sm input-inline";

            dataTable = table.DataTable(options.dataTable);
            // revert back to default
            $.fn.dataTableExt.oStdClasses.sWrapper = tmp.sWrapper;
            $.fn.dataTableExt.oStdClasses.sFilterInput = tmp.sFilterInput;
            $.fn.dataTableExt.oStdClasses.sLengthSelect = tmp.sLengthSelect;

            // get table wrapper
            tableWrapper = table.parents('.dataTables_wrapper');

            // build table group actions panel
            if ($('.table-actions-wrapper', tableContainer).size() === 1) {
                $('.table-group-actions', tableWrapper).html($('.table-actions-wrapper', tableContainer).html()); // place the panel inside the wrapper
                $('.table-actions-wrapper', tableContainer).remove(); // remove the template container
            }
            // handle group checkboxes check/uncheck
            $('.group-checkable', table).change(function () {
                var set = $('tbody > tr > td:nth-child(1) input[type="checkbox"]', table);
                var checked = $(this).is(":checked");
                $(set).each(function () {
                    $(this).attr("checked", checked);
                });
                $.uniform.update(set);
                countSelectedRecords();
            });

            // handle row's checkbox click
            table.on('change', 'tbody > tr > td:nth-child(1) input[type="checkbox"]', function () {
                countSelectedRecords();
            });

            // handle filter submit button click
            table.on('click', '.filter-submit', function (e) {
                e.preventDefault();
                the.submitFilter();
            });

            // handle filter cancel button click
            table.on('click', '.filter-cancel', function (e) {
                e.preventDefault();
                the.resetFilter();
            });
        },

        getSelectedRowsCount: function () {
            return $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).size();
        },

        getSelectedRows: function () {
            var rows = [];
            $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                rows.push($(this).val());
            });

            return rows;
        },
        getSelectedRowsData: function (isRadio) {
            var rows = [];
            if (!isRadio) {
                $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                    rows.push(dataTable.row($(this).parents('tr')).data());
                });
            }
            else {
                var radio = $('tbody > tr > td:nth-child(1) input[type="radio"]:checked', table);
                if (radio) {
                    rows.push(dataTable.row($(radio).parents('tr')).data());
                }
            }
            return rows;
        },


        setAjaxParam: function (name, value) {
            ajaxParams[name] = value;
        },

        addAjaxParam: function (name, value) {
            if (!ajaxParams[name]) {
                ajaxParams[name] = [];
            }

            skip = false;
            for (var i = 0; i < (ajaxParams[name]).length; i++) { // check for duplicates
                if (ajaxParams[name][i] === value) {
                    skip = true;
                }
            }

            if (skip === false) {
                ajaxParams[name].push(value);
            }
        },

        clearAjaxParams: function (name, value) {
            ajaxParams = {};
        },

        getDataTable: function () {
            return dataTable;
        },

        getTableWrapper: function () {
            return tableWrapper;
        },

        gettableContainer: function () {
            return tableContainer;
        },

        getTable: function () {
            return table;
        }

    };

};