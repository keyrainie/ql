(function ($) {

    $.showLoading = function (target) {
        Metronic.blockUI({
            target: target ? target : document.parent,
            iconOnly: true
        });
    };

    $.hideLoading = function (target) {
        if (target) {
            Metronic.unblockUI(target);
        }
        else {
            Metronic.unblockUI();
        }
    };

    $.initPlaceholder = function () {
        $("input,textarea,select[multiple]").each(function () {
            var label = $(this).parent().prev(":first");
            if (label.is("label")) {
                if ($(this).is("select")) {
                    $(this).attr("title", label.text());
                } else {
                    $(this).attr("placeholder", label.text());
                }
            }
        });
    };

    //form : Jquery Object
    //data : JSON
    $.bindEntity = function (form, data) {
        if (!data) return;
        function _getValue(dataModel) {
            var name = dataModel.shift();
            if (data[name] && (typeof (data[name]) == "object")) {
                return _getValue(dataModel);
            } else {
                return data[name];
            }
        }

        form.find("[data-model]").each(function () {
            var model = $(this).attr("data-model").split(".");
            var val = _getValue(model);
            if ($(this).is("select") && $(this).prop("multiple")) {
                if (val) {
                    $(this).val(val.split(","));
                }
            } else {
                $(this).val(val);
            }
        });
        var bsSelect = $('.bs-select');
        if (bsSelect.length > 0 && bsSelect.selectpicker) {
            bsSelect.selectpicker('refresh');
        }

    };

    //form : Jquery Object
    function buildEntity(form) {
        var obj = {};
        form.find("[data-model]").each(function () {
            var name = $(this).attr("data-model");
            var names = name.split(".");
            var temp;
            var val;
            if ($(this).attr("type") == "checkbox"
                || $(this).attr("type") == "radio") {
                if ($(this).prop("checked")) {
                    val = $(this).val();
                }
            }
            else {
                val = $.trim($(this).val());
            }
            if (names.length > 1) {
                for (var i = 0; i < names.length; i++) {
                    if (temp) {
                        if (!temp[names[i]]) {
                            if (i == (names.length - 1)) temp[names[i]] = val;
                            else temp[names[i]] = {};
                        }
                        temp = temp[names[i]];
                    } else {
                        if (!obj[names[i]]) {
                            if (i == (names.length - 1)) obj[names[i]] = val;
                            else obj[names[i]] = {};
                        }
                        temp = obj[names[i]];
                    }
                }
            } else {
                obj[name] = val;
            }

        });
        return obj;
    }

    $.buildEntity = buildEntity;

    //页面导出Excel:
    $.exportExcel = function (action, params) {
        var form = $("<form>");//定义一个form表单
        form.attr("style", "display:none");
        form.attr("target", "");
        form.attr("method", "post");
        form.attr("action", action);
        $("body").append(form);//将表单放置在web中
        if (params && params.length > 0) {

            for (var i = 0 ; i < params.length; i++) {
                var input1 = $("<input>");
                input1.attr("type", "hidden");
                input1.attr("name", params[i].name);
                input1.attr("value", params[i].value);
                form.append(input1);
            }
        }
        form.submit();//表单提交 
    };

    $.alert = function (msg, callback) {
        bootbox.alert(msg, function () {
            if (callback) {
                callback();
            }

        });
    };

    $.confirm = function (msg, callback) {
        bootbox.confirm(msg, function (result) {
            if (callback) {
                callback(result);
            }
        });
    };

    $.prompt = function (msg, okCallback, cancelCallback) {
        bootbox.prompt(msg, function (result) {
            if (result === null) {
                if (cancelCallback) {
                    cancelCallback();
                }
                cancelCallback();
            } else {
                if (okCallback) {
                    okCallback(result);
                }
            }
        });
    };

    //不带分页的普通的Grid
    $.grid = function (src, options) {

        options = $.extend(true, {
            autoWidth: false,
            paging: false,
            serverSide: false,
            searching: false,
            info: false,
            ordering: false,
            language: { // language settings
                "infoEmpty": "暂无相关记录",
                "emptyTable": "暂无相关记录",
                "zeroRecords": "暂无相关记录"
            }
        }, options);

        var table = src.DataTable(options);
        $("#" + src.attr("id") + " .group-checkable").change(function () {
            var set = $("#" + src.attr("id") + ' tbody > tr > td input[type="checkbox"]');
            var checked = $(this).is(":checked");
            $(set).each(function () {
                $(this).attr("checked", checked);
            });
            $.uniform.update(set);
        })
        return {

            getTable: function () {
                return table;
            },
            addRow: function (rowData) {
                table.row.add(rowData).draw();

                Metronic.initUniform($('input[type="checkbox"]', $(src)));

            },
            clear: function () {
                return table.clear().draw();
            },
            getAllDatas: function () { return table.data(); },

            getSelectedRowsCount: function () {
                return $("#" + src.attr("id") + ' tbody > tr > td input[type="checkbox"]:checked').size();
            },

            getSelectedRowsValue: function () {
                var rows = [];
                $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                    rows.push($(this).val());
                });

                return rows;
            },
            getSelectedRowsData: function () {
                var rows = [];
                $('tbody > tr > td:nth-child(1) input[type="checkbox"]:checked', table).each(function () {
                    rows.push(table.row($(this).parents('tr')).data());
                });
                return rows;
            },
            deleteSelectedRows: function () {
                var rows = [];
                $("#" + src.attr("id") + ' tbody > tr > td:nth-child(1) input[type="checkbox"]:checked').each(function () {
                    rows.push($(this));
                });
                if (rows && rows.length > 0) {
                    for (var i = 0; i < rows.length; i++) {
                        table.row(rows[i].parents('tr')).remove().draw();
                    }
                }
                else {
                    $.alert('请先选中要删除的行!');
                }
            }
        };


    };

    //分页AjaxGrid
    $.ajaxGrid = function (options) {
        var table;
        var tableOptions = options;
        return {
            loadData: function (queryEntity) {
                if (table) {
                    table.clearAjaxParams();
                    table.addAjaxParam("queryfilter", queryEntity);
                    table.getDataTable().ajax.reload();
                }
                else {
                    table = new Datatable();
                    table.clearAjaxParams();
                    table.addAjaxParam("queryfilter", queryEntity);
                    table.init(tableOptions);
                }
            },
            getSelectedRows: function () {
                return table.getSelectedRows();
            },
            getSelectedRowsCount: function () {
                return table.getSelectedRowsCount();
            },
            getTable: function () {
                return table.getTable();
            }
        };
    };

    $.fn.defaultDateRangePicker = function () {

        var defaultCallback = function (start, end) {
            var startVal = '', endVal = '', rangeVal = [];
            if (start) {
                startVal = start.format('YYYY/MM/DD');
                rangeVal.push(startVal);
            }
            if (end) {
                endVal = end.format('YYYY/MM/DD');
                rangeVal.push(endVal);
            }
            $("span", this.element).html(rangeVal.join(' - '));
            $(".date-start", this.element).val(startVal);
            $(".date-end", this.element).val(endVal);
        }

        this.each(function () {
            var defaultOption = {
                opens: (Metronic.isRTL() ? 'left' : 'right'),
                startDate: null ,// moment().subtract('days', 29),
                endDate: null, // moment(),
                minDate: '01/01/1900',
                maxDate: '12/31/2100',
                showDropdowns: true,
                showWeekNumbers: true,
                ranges: {
                    '今天': [moment(), moment()],
                    '最近3天': [moment().subtract('days', 3), moment()],
                    '最近7天': [moment().subtract('days', 6), moment()],
                    '最近30天': [moment().subtract('days', 29), moment()],
                },
                buttonClasses: ['btn'],
                applyClass: 'green',
                cancelClass: 'default',
                separator: ' to ',
                locale: {
                    applyLabel: '确定',
                    cancelLabel: '取消',
                    clearLabel: '清空',
                    fromLabel: '从',
                    toLabel: '到',
                    customRangeLabel: '自定义',
                    firstDay: 1
                }
            },
                el = $(this),
                setStartDate = moment($('.date-start', el).val()),
                setEndDate = moment($('.date-end', el).val());

            if (setStartDate.isValid() && setEndDate.isValid() && (setEndDate.isSame(setStartDate) || setEndDate.isAfter(setStartDate))) {
                defaultOption.startDate = setStartDate;
                defaultOption.endDate = setEndDate;
            }
            el.daterangepicker(defaultOption, defaultCallback);

            if (defaultOption.startDate == null && defaultOption.endDate == null) {
                $('span', el).css({ "display": "inline-block", "width": "160px" }).html('');
                $('.date-start', el).val('');
                $('.date-end', el).val('');
            } else {
                if (defaultOption.startDate != null && defaultOption.endDate == null) {
                    defaultOption.endDate = defaultOption.startDate;
                }
                else if (defaultOption.startDate == null && defaultOption.endDate != null) {
                    defaultOption.startDate = defaultOption.endDate;
                }
                $('span', el).css({ "display": "inline-block", "width": "160px" })
                    .html(defaultOption.startDate.format('YYYY/MM/DD') + ' - ' + defaultOption.endDate.format('YYYY/MM/DD'));
                $('.date-start', el).val(defaultOption.startDate.format('YYYY/MM/DD'));
                $('.date-end', el).val(defaultOption.endDate.format('YYYY/MM/DD'));
            }
        });
        return this;
    }

    $.daterangePicker = function (id) {

        var defaultOption = {
            opens: (Metronic.isRTL() ? 'left' : 'right'),
            startDate: null, //moment().subtract('days', 29),
            endDate: null, //moment(),
            minDate: '01/01/1900',
            maxDate: '12/31/2100',
            showDropdowns: true,
            showWeekNumbers: true,
            ranges: {
                '今天': [moment(), moment()],
                '最近3天': [moment().subtract('days', 3), moment()],
                '最近7天': [moment().subtract('days', 6), moment()],
                '最近30天': [moment().subtract('days', 29), moment()],
            },
            buttonClasses: ['btn'],
            applyClass: 'green',
            cancelClass: 'default',
            separator: ' to ',
            locale: {
                applyLabel: '确定',
                cancelLabel: '取消',
                clearLabel: '清空',
                fromLabel: '从',
                toLabel: '到',
                customRangeLabel: '自定义',
                firstDay: 1
            }
        },
        setStartDate = moment($(id + ' .date-start').val()),
        setEndDate = moment($(id + ' .date-end').val());

        if (setStartDate.isValid() && setEndDate.isValid() && (setEndDate.isSame(setStartDate) || setEndDate.isAfter(setEndDate))) {
            defaultOption.startDate = setStartDate;
            defaultOption.endDate = setEndDate;
        }

        var el = $(id).daterangepicker(defaultOption,
                    function (start, end) {
                        var startVal = '', endVal = '', rangeVal = [];
                        if (start) {
                            startVal = start.format('YYYY/MM/DD');
                            rangeVal.push(startVal);
                        }
                        if (end) {
                            endVal = end.format('YYYY/MM/DD');
                            rangeVal.push(endVal);
                        }
                        $("span", this.element).html(rangeVal.join(' - '));
                        $(".date-start", this.element).val(startVal);
                        $(".date-end", this.element).val(endVal);
                    });

        if (defaultOption.startDate == null && defaultOption.endDate == null) {
            $('span', el).css({ "display": "inline-block", "width": "160px" }).html('');
            $('.date-start', el).val('');
            $('.date-end', el).val('');
        } else {
            if (defaultOption.startDate != null && defaultOption.endDate == null) {
                defaultOption.endDate = defaultOption.startDate;
            }
            else if (defaultOption.startDate == null && defaultOption.endDate != null) {
                defaultOption.startDate = defaultOption.endDate;
            }
            $(id + 'span').css({ "display": "inline-block", "width": "160px" })
                 .html(defaultOption.startDate.format('YYYY/MM/DD') + ' - ' + defaultOption.endDate.format('YYYY/MM/DD'));
            $(id + ' .date-start').val(defaultOption.startDate.format('YYYY/MM/DD'));
            $(id + ' .date-end').val(defaultOption.endDate.format('YYYY/MM/DD'));
        }

        var daterangepicker = el.data('daterangepicker');

        return {

            getStartDate: function () {
                return daterangepicker.startDate.format("YYYY/MM/DD");
            },
            getEndDate: function () {
                return daterangepicker.endDate.format("YYYY/MM/DD");
            }
        };
    };

    //绑定下拉选择框数据//parm = { id: "", showAll: true, data: [], value: "" }
    $.bindSelecter = function (param) {
        var $select = $("#" + param.id);
        $select.empty();
        if (param.showAll == undefined || param.showAll) {
            $select.append("<option value=''>-请选择-</option>");
        }
        if (param.data) {
            $.each(param.data, function (i, item) {
                $select.append("<option value='" + item.Code + "'" + (param.value == item.Code ? " selected='selected'" : "") + ">" + item.Name + "</option>");
            });
        }
        if (typeof (param.callback) == "function") {
            param.callback();
        }
    };
    //Ajax获取下拉选择框数据并绑定到指定select标签上面//parm = { id: "", showAll: true, value: "", url: "" }
    $.selecter = function (param) {
        if (param.id == undefined || param.id.length <= 0) {
            return;
        }
        if (param.url == undefined || param.url.length <= 0) {
            return;
        }
        $.ajax({
            url: param.url,
            type: "POST",
            dataType: "json",
            data: {},
            success: function (data) {
                if (data) {
                    param.data = data;
                    $.bindSelecter(param);
                } else {
                    if (typeof (param.callback) == "function") {
                        param.callback();
                    }
                }
            }
        });
    };
})(jQuery);

//ajax post 
$(document).ajaxSend(onAjaxSend)
.ajaxComplete(onComplete)
.ajaxSuccess(onSuccess)
.ajaxError(onError);

function onAjaxSend(event, xhr, settings) {
    if (settings.type.toUpperCase() == "GET") {
        if (settings.url.indexOf("?") > 0) {
            settings.url += "&t=" + new Date().getTime();
        }
        else {
            settings.url += "?t=" + new Date().getTime();
        }
    }
    $.showLoading();
}

function onComplete(event, xhr, settings) {
    $.hideLoading();
}
function onSuccess(event, xhr, settings) {
    if (xhr.responseText != "") {
        if (settings.dataType == 'json') {
            var jsonValue = jQuery.parseJSON(xhr.responseText);
            if (jsonValue.error == true) {
                $.alert(jsonValue.message);
            }
        }
        else if (settings.dataType == 'html') {
            var jqError = $(xhr.responseText).filter("#service_Error_Message_Panel");
            if (jqError.length > 0) {
                $.alert(jqError.find(" #errorMessage").val());
            }
        }
        else if (settings.dataType == 'xml') {
            var error = $(xhr.responseText).find('message');
            if (error && error.length > 0) {
                $.alert($(error).text());
            }
        }
    }
}

function onError(event, xhr, settings) {
    if (xhr.status != 0) {
        var error = $(xhr.responseText).find('message');
        if (error && error.length > 0) {
            $.alert("请求发生错误 :" + $(error).text());
        }
        else {
            $.alert("请求发生错误 :" + xhr.responseText);
        }
    }
}

$.formatMoney = function(amount) {
    try {
        return parseFloat(amount).toFixed(2);
    } catch(e) {
        return amount;
    }
};

if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function(elem, startFrom) {
        var startFrom = startFrom || 0;
        if (startFrom > this.length) return -1;

        for (var i = 0; i < this.length; i++) {
            if (this[i] == elem && startFrom <= i) {
                return i;
            } else if (this[i] == elem && startFrom > i) {
                return -1;
            }
        }
        return -1;
    };
}