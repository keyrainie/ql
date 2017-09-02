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
                c = temp.match(/^\#\[(.*)\]$/)[1];
                if (/^[_a-z][_a-z0-9]*$/i.test(c)) {
                    temp = dataItem[c];
                }
                else if (/^\{pro\}$/i.test(c)) {
                    temp = dataItem[pro];
                }
                else if (/^\{\#pro\}$/i.test(c)) {
                    temp = pro;
                }
                else if (/^\{item\}$/i.test(c)) {
                    temp = dataItem;
                }
                else {
                    var mats = c.match(/\#item\.([_a-z][_a-z0-9]*)\((.*)\)/i);

                    var fnname = mats[1];
                    var params = mats[2].match(/([_a-z][_a-z0-9]*)|(\{item\})|(\{pro\})|(\{\#pro\})/gi);
                    var args = {};
                    for (k = 0; k < params.length; k++) {
                        if (/^[_a-z][_a-z0-9]*$/i.test(params[k])) {
                            if (dataItem[params[k]]) {
                                args[params[k]] = dataItem[params[k]];
                            }
                            else if (params[k]) {
                                try {
                                    args[params[k]] = eval(params[k]);
                                }
                                catch (ex) {
                                    args[params[k]] = params[k];
                                }
                            }
                            else {
                                args[params[k]] = "";
                            }
                        }
                        else if (/^\{pro\}$/i.test(params[k])) {

                            args["pro"] = dataItem[pro];
                        }
                        else if (/^\{\#pro\}$/i.test(params[k])) {
                            args["_pro"] = pro;
                        }
                        else if (/^\{item\}$/i.test(params[k])) {
                            args["item"] = dataItem;
                        }
                        else {
                            args[params[k]] = "";
                        }
                    }
                    temp = hdl[fnname](args);
                }
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
        return syntax.join("");
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

    $.fn.extend({
        setDataSource: function (ds, handler) {
            $(this).data(key.dataSource, ds);
            $(this).data(key.handler, handler);
            return $(this);
        },
        dataBind: function () {

            var item = $(this).find("[" + key.dataItem + "=true]");

            if (item.length <= 0) return;
            item = item.eq(0);

            var ds = $(this).data(key.dataSource);
            if (!ds) return;

            var hdl = $(this).data(key.handler);
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
        },
        emptyTmpl: function () {
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
            return $(html);
        }
    });
})();