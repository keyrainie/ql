
(function () {

    var key = {
        dataSource: "datasource",
        handler: "handler",
        dataTemplate: "data-template",
        dataItem: "data-template-item",
        dataBindModel: "data-bind-model"
    };
    var bindModel = {
        objDynamic: "dynamicobject"
    };

    function bindArray(ds, item, hdl) {
        var i, html = "";
        for (i = 0; i < ds.length; i++) {
            if (!ds[i].push) {
                html += bindObject(ds[i], item, hdl);
            }
            else {
                bindArray(ds[i], item, hdl);
            }
        }
        return html;
    }

    function analysis(text, dataItem, hdl, pro) {
        var syntax = [];
        var temp = "";
        var customSyntaxStart = false;
        var i, k, c;
        for (i = 0; i < text.length;) {

            if (text[i] == "#" && text[i + 1] == "[") {
                syntax.push(temp);
                temp = "#[";
                i += 2;
                customSyntaxStart = true;
            }
            else if (text[i] == "]" && customSyntaxStart) {
                temp += "]";
                //一个自定义语法接触可以进行处理
                //获得语法内容
                c = temp.match(/^\#\[(.*)\]$/)[1];
                if (/^[_a-z][_a-z0-9]*$/i.test(c)) {
                    //field
                    temp = dataItem[c];
                }
                else if (/^\{pro\}$/i.test(c)) {
                    //dynamic field
                    temp = dataItem[pro];
                }
                else if (/^\{\#pro\}$/i.test(c)) {
                    //
                    temp = pro;
                }
                else if (/^\{item\}$/i.test(c)) {
                    //{item}
                    temp = dataItem;
                }
                else {
                    //#item.fn
                    var mats = c.match(/\#item\.([_a-z][_a-z0-9]*)\((.*)\)/i);

                    var fnname = mats[1];
                    //处理参数
                    var params = mats[2].match(/([_a-z][_a-z0-9]*)|(\{item\})|(\{pro\})|(\{\#pro\})/gi);
                    var args = {};
                    for (k = 0; k < params.length; k++) {
                        //field
                        if (/^[_a-z][_a-z0-9]*$/i.test(params[k])) {
                            if (dataItem[params[k]]) {
                                args[params[k]] = dataItem[params[k]];
                            }
                            else if (params[k]) {
                                try {
                                    args[params[k]] = eval(params[k]);
                                }
                                catch (ex) {
                                    args[params[k]] = null;
                                }
                            }
                            else {
                                args[params[k]] = "";
                            }
                        }
                            //{pro}
                        else if (/^\{pro\}$/i.test(params[k])) {

                            args["pro"] = dataItem[pro];
                        }
                            //{#pro}
                        else if (/^\{\#pro\}$/i.test(params[k])) {
                            args["_pro"] = pro;
                        }
                            //{item}
                        else if (/^\{item\}$/i.test(params[k])) {
                            args["item"] = dataItem;
                        }
                        else {
                            args[params[k]] = "";
                        }
                    }

                    temp = hdl[fnname](args);
                    args = null;
                    mats = null;
                    fnname = null;
                    params = null;
                }
                //-------------
                customSyntaxStart = false;
                syntax.push(temp);
                i += 1;
                temp = "";
            }
            else {
                temp += text[i];
                i += 1;
            }
        }
        syntax.push(temp);
        var result = syntax.join("");
        syntax = null;
        return result;
    }

    function bindObject(ds, item, hdl) {
        var text;
        var model = item.attr(key.dataBindModel);
        var i, html = "";
        if (model == bindModel.objDynamic) {
            for (i in ds) {
                text = item.text();
                html += analysis($.trim(text), ds, hdl, i);
            }
        }
        else {
            text = item.text();
            html = analysis($.trim(text), ds, hdl);
        }
        return html;
    }

    $.extend($.fn, {
        dataBind: function (ds, handler) {

            if (!ds) return;
            var item = $(this).find("[" + key.dataItem + "=true]");

            if (item.length <= 0) {
                item = null;
                return;
            }
            item = item.eq(0);
            var hdl = handler;
            if (!hdl) hdl = {};

            var html = "";
            if (ds.push) {
                html = bindArray(ds, item, hdl);
            }
            else {
                html = bindObject(ds, item, hdl);
            }

            var container = item.parent();
            container.append(html);
            item = null;
        },
        emptyTmpl: function () {
            //if ($(this).attr(key.dataTemplate) != "true") return;
            $(this).find("[data-template-item=true]").parent().find(":not([data-template-item=true])").remove();
        },
        jtemplate: function (ds, handler) {
            var item = $(this);
            var html;
            if (ds.push) {
                html = bindArray(ds, item, handler);
            }
            else {
                html = bindObject(ds, item, handler);
            }
            item = null;
            return $(html);
        }
    });

})()