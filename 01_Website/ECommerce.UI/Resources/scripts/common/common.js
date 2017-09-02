
$(function () {
    $("input[waterMark=true]").focus(function () {
        Common.ClearWatermark(this);
    });
    $("input[waterMark=true]").blur(function () {
        Common.SetWatermark(this);
    });
    $("input[Validation=Int]").blur(function () {
        Common.IntValidation(this);
    });
    $("input[Validation=Decimal]").blur(function () {
        Common.DecimalValidation(this);
    });
    $("input[Validation=Email]").blur(function () {
        Common.EmailValidation(this);
    });

    $("textarea[maxlength]").bind('input', function () {
        checkMaxInput(this);
    }).bind('keyup', function () {
        checkMaxInput(this);
    }).bind('blur', function () {
        checkMaxInput(this);
    });

    function checkMaxInput(_textObj) {
        var n;
        if (_textObj.getAttribute('tip') != '') {
            n = _textObj.value.replace(_textObj.getAttribute('tip'), '').lenGBK();
        }
        else {
            n = _textObj.value.lenGBK();
        }

        var maxLen = parseInt(_textObj.getAttribute("maxlength"), 10);
        if (n > maxLen) {
            $(_textObj).val($(_textObj).data("max-data"));
        }
        else {
            $(_textObj).data("max-data", _textObj.value);
        }
    }


});


//通用的一些设置、验证等
var Common = {
    ClearWatermark: function (obj) {
        var val = $.trim($(obj).val());
        var tips = $.trim($(obj).attr("title"));
        if (val == tips) {
            $(obj).val("");
        }
    },
    SetWatermark: function (obj) {
        var val = $.trim($(obj).val());
        var tips = $.trim($(obj).attr("title"));
        if (val == "" || val == tips) {
            $(obj).val(tips);
            $(obj).addClass("waterMark");
        }
        else {
            $(obj).removeClass("waterMark");
        }
    },
    IntValidation: function (obj) {
        var pattern = /^\d+$/;
        var val = $.trim($(obj).val());
        if (val.length > 0 && !pattern.test(val)) {
            $.alert(JR("必须输入正整数"), "error");
            $(obj).val("");
            $(obj).addClass("validate");
        }
        else {
            $(obj).removeClass("validate");
        }
    },
    DecimalValidation: function (obj) {
        var pattern = /^-?\d+(\.\d{1,2})?$/;
        var val = $.trim($(obj).val());
        if (val.length > 0 && !pattern.test(val)) {
            $.alert(JR("必须输入小数位2位以内的小数"), "error");
            $(obj).val("");
            $(obj).addClass("validate");
        }
        else {
            $(obj).removeClass("validate");
        }
    },
    EmailValidation: function (obj) {
        var pattern = /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/;
        var val = $.trim($(obj).val());
        if (val.length > 0 && !pattern.test(val)) {
            $.alert(JR("必须输入有效的邮箱地址"), "error");
            $(obj).val("");
            $(obj).addClass("validate");
        }
        else {
            $(obj).removeClass("validate");
        }
    }
}

var ScriptResource = new Array();
function JR(key) {
    return key;
}

window.usingNamespace = function (a) {
    var ro = window;
    if (!(typeof (a) === "string" && a.length != 0)) {
        return ro;
    }
    var co = ro;
    var nsp = a.split(".");
    for (var i = 0; i < nsp.length; i++) {
        var cp = nsp[i];
        if (!ro[cp]) {
            ro[cp] = {};
        };
        co = ro = ro[cp];
    };
    return co;
};
//Function.prototype.method = function(name, func) {
//  this.prototype[name] = func;
//  return this;
//};
//if (String.prototype.trim) { //判断下浏览器是否自带有trim()方法
//    String.method('trim', function () {
//        return this.replace(/^s+|s+$/g, '');
//    });
//    String.method('ltrim', function () {
//        return this.replace(/^s+/g, '');
//    });
//    String.method('rtrim', function () {
//        return this.replace(/s+$/g, '');
//    });
//}
usingNamespace("Biz.Common")["Validation"] = {

    textCount: function (field, counter, maxLimit) {
        var message = $(field).val();
        if ($(field).val().length > maxLimit)
            $(field).val(message.substring(0, maxLimit))
        //$(counter).text(maxLimit-field.value.length);     
    },
    refreshValidator: function (img, input) {
        $(img).attr('src', $(img).attr('src') + "&r=" + Math.random());
        $(input).focus();
    },
    isUrl: function (s) {
        var strRegex =
                            /^((http(s)?|ftp|telnet|news|rtsp|mms):\/\/)?(((\w(\-*\w)*\.)+[a-zA-Z]{2,4})|(((1\d\d|2([0-4]\d|5[0-5])|[1-9]\d|\d).){3}(1\d\d|2([0-4]\d|5[0-5])|[1-9]\d|\d).?))(:\d{0,5})?(\/+.*)*$/;
        return strRegex.test(s);
    },
    isDecimal: function (d) { var pattern = /^(([1-9]\d{0,12})|0)(\.\d{1,2})?$/; return pattern.test(d); },
    isEmail: function (s) {
        var pattern = /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/;
        return pattern.test(s);
    },
    isLowEmail: function (s) {
        var b, e;
        b = s.indexOf("@");
        e = s.indexOf(".");
        if (b <= 0) return false;
        if (e < 0 || e == (s.length - 1)) { return false; }
        return true;
    },
    clearNoNum: function (event, obj) {
        event = window.event || event;
        if (event.keyCode == 37 | event.keyCode == 39) {
            return;
        }
        obj.value = obj.value.replace(/[^\d.]/g, "");
        obj.value = obj.value.replace(/^\./g, "");
        obj.value = obj.value.replace(/\.{2,}/g, ".");
        obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
    },
    checkNum: function (obj) {
        obj.value = obj.value.replace(/\.$/g, "");
    },
    isInteger: function (value) {
        var integerReg = new RegExp(/^\d+$/);
        return integerReg.test(value);
    },
    isValidateReg: function (value) {
        var validateReg = /^([A-Za-z0-9\s\-\_\~\!\@\#\$\%\^\&\*\(\)\|\<\>\?\:\;\"\'\.\[\]\{\}\,\+\`\/\\\=]){8,16}$/;
        if (validateReg.test(value)) {
            return Biz.Common.Validation.isStrengthReg(value);
        }
        return false;
    },
    isStrengthReg: function (value) {
        if (value.match(/([a-zA-Z])/) && value.match(/([0-9])/)) {
            return true;
        }
        else if (value.match(/([^a-zA-Z0-9])/) && value.match(/([0-9])/)) {
            return true;
        }
        else if (value.match(/([^a-zA-Z0-9])/) && value.match(/([a-zA-Z])/)) {
            return true;
        }
        return false;
    },

    isDate: function (strValue) {
        var objRegExp = /^\d{4}(\-|\/|\.)\d{1,2}\1\d{1,2}$/

        if (!objRegExp.test(strValue))
            return false;
        else {
            var arrayDate = strValue.split(RegExp.$1);
            var intDay = parseInt(arrayDate[2], 10);
            var intYear = parseInt(arrayDate[0], 10);
            var intMonth = parseInt(arrayDate[1], 10);
            if (intMonth > 12 || intMonth < 1) {
                return false;
            }
            var arrayLookup = {
                '1': 31, '3': 31, '4': 30, '5': 31, '6': 30, '7': 31,
                '8': 31, '9': 30, '10': 31, '11': 30, '12': 31
            }
            if (arrayLookup[parseInt(arrayDate[1])] != null) {
                if (intDay <= arrayLookup[parseInt(arrayDate[1])] && intDay != 0)
                    return true;
            }
            if (intMonth - 2 == 0) {
                var booLeapYear = (intYear % 4 == 0 && (intYear % 100 != 0 || intYear % 400 == 0));
                if (((booLeapYear && intDay <= 29) || (!booLeapYear && intDay <= 28)) && intDay != 0)
                    return true;
            }
        }
        return false;
    },
    isZip: function (value) {
        var validateReg = /^[0-9]{6}$/;
        return validateReg.test(value);
    },
    checkSpecialChar: function (value) {
        var validateReg = /([~!@#$%^&*\/\\,.\(\)]){6,16}$/;
        return validateReg.test(value);
    },
    CheckSpecialString: function (value) {
        var validateReg = /[\u0000-\u0008\u000B\u000C\u000E-\u001F\uD800-\uDFFF\uFFFE\uFFFF]/;
        return validateReg.test(value);
    },
    isTel: function (s) {
        var patrn = /^\d{3,4}-\d{7,8}(-\d{3,4})?$/
        if (!patrn.exec(s)) return false
        return true
    },

    isMobile: function (value) {
        var validateReg = /^1\d{10}$/;
        return validateReg.test(value);
    },
    getLength: function (value) {
        return value.replace(/[^\x00-\xff]/g, "**").length;
    },
    isLicence: function (value) {
        var validateReg = /^[A-Za-z]{10}[0-9]{10}$/;
        return validateReg.test(value);
    },
    isPersonalCard: function (value) {
        var validateReg = /(^\d{15}$)|(^\d{17}(\d|[A-Za-z]{1})$)/;
        return validateReg.test(value);
    },
    isOrganizationCodeCard: function (value) {
        var validateReg = /^[A-Za-z0-9]{9}$/;
        return validateReg.test(value);
    },
    isBankAccount: function (value) {
        var validateReg = /^[1-9]{1}[0-9]*$/;
        return validateReg.test(value);
    },
    isStartLetter: function (value) {
        var validateReg = /^[a-zA-Z][a-zA-Z0-9_]*$/;
        return validateReg.test(value);
    },
    MaxLength: function (field, maxlimit) {
        var j = field.value.replace(/[^\x00-\xff]/g, "**").length;
        var tempString = field.value;
        var tt = "";
        if (j > maxlimit) {
            for (var i = 0; i < maxlimit; i++) {
                if (tt.replace(/[^\x00-\xff]/g, "**").length < maxlimit)
                    tt = tempString.substr(0, i + 1);
                else
                    break;
            }
            if (tt.replace(/[^\x00-\xff]/g, "**").length > maxlimit) {
                tt = tt.substr(0, tt.length - 1);
                field.value = tt;
            }
            else {
                field.value = tt;
            }
        }
    }
};

usingNamespace("Web.Utils")["String"] = {
    IsNullOrEmpty: function (v) {
        return !(typeof (v) === "string" && v.length != 0);
    },
    Trim: function (v) {
        return v.replace(/^\s+|\s+$/g, "")
    },
    TrimStart: function (v, c) {
        if ($String.IsNullOrEmpty(c)) {
            c = "\\s";
        };
        var re = new RegExp("^" + c + "*", "ig");
        return v.replace(re, "");
    },
    TrimEnd: function (v, c) {
        if ($String.IsNullOrEmpty(c)) {
            c = "\\s";
        };
        var re = new RegExp(c + "*$", "ig");
        return v.replace(re, "");
    },
    Camel: function (str) {
        return str.toLowerCase().replace(/\-([a-z])/g, function (m, c) { return "-" + c.toUpperCase() })
    },
    Repeat: function (str, times) {
        for (var i = 0, a = new Array(times) ; i < times; i++)
            a[i] = str;
        return a.join();
    },
    IsEqual: function (str1, str2) {
        if (str1 == str2)
            return true;
        else
            return false;
    },
    IsNotEqual: function (str1, str2) {
        if (str1 == str2)
            return false;
        else
            return true;
    },
    MaxLengthKeyUp: function (obj, len) {
        var value = $(obj).val();
        if (value.length > len) {
            $(obj).val(value.substring(0, len));
        }
    },
    MaxLengthKeyDown: function (obj, len) {
        if ($(obj).val().length > len) { return false; }
        return true;
    }
};
var $String = Web.Utils.String;



/*start 通用函数*/
if (!this.JSON) {
    this.JSON = {};
}

/*Json函数*/
(function () {
    function f(n) {
        return n < 10 ? '0' + n : n;
    }
    if (typeof String.prototype.lenGBK !== 'function') {
        String.prototype.lenGBK = function () {
            return this.replace(/[^\x00-\xff]/g, "*").length;
        }
    }
    if (typeof Date.prototype.toJSON !== 'function') {
        Date.prototype.toJSON = function (key) {

            return isFinite(this.valueOf()) ?
                   this.getUTCFullYear() + '-' +
                 f(this.getUTCMonth() + 1) + '-' +
                 f(this.getUTCDate()) + 'T' +
                 f(this.getUTCHours()) + ':' +
                 f(this.getUTCMinutes()) + ':' +
                 f(this.getUTCSeconds()) + 'Z' : null;
        };

        String.prototype.toJSON =
        Number.prototype.toJSON =
        Boolean.prototype.toJSON = function (key) {
            return this.valueOf();
        };
    }

    var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        gap,
        indent,
        meta = {    // table of character substitutions
            '\b': '\\b',
            '\t': '\\t',
            '\n': '\\n',
            '\f': '\\f',
            '\r': '\\r',
            '"': '\\"',
            '\\': '\\\\'
        },
        rep;

    function quote(string) {
        escapable.lastIndex = 0;
        return escapable.test(string) ?
            '"' + string.replace(escapable, function (a) {
                var c = meta[a];
                return typeof c === 'string' ? c :
                    '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
            }) + '"' :
            '"' + string + '"';
    }
    function str(key, holder) {
        var i,          // The loop counter.
            k,          // The member key.
            v,          // The member value.
            length,
            mind = gap,
            partial,
            value = holder[key];

        if (value && typeof value === 'object' &&
                typeof value.toJSON === 'function') {
            value = value.toJSON(key);
        }

        if (typeof rep === 'function') {
            value = rep.call(holder, key, value);
        }

        switch (typeof value) {
            case 'string':
                return quote(value);
            case 'number':
                return isFinite(value) ? String(value) : 'null';
            case 'boolean':
            case 'null':
                return String(value);
            case 'object':
                if (!value) {
                    return 'null';
                }
                gap += indent;
                partial = [];

                if (Object.prototype.toString.apply(value) === '[object Array]') {
                    length = value.length;
                    for (i = 0; i < length; i += 1) {
                        partial[i] = str(i, value) || 'null';
                    }

                    v = partial.length === 0 ? '[]' :
                    gap ? '[\n' + gap +
                            partial.join(',\n' + gap) + '\n' +
                                mind + ']' :
                          '[' + partial.join(',') + ']';
                    gap = mind;
                    return v;
                }

                if (rep && typeof rep === 'object') {
                    length = rep.length;
                    for (i = 0; i < length; i += 1) {
                        k = rep[i];
                        if (typeof k === 'string') {
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ': ' : ':') + v);
                            }
                        }
                    }
                } else {
                    for (k in value) {
                        if (Object.hasOwnProperty.call(value, k)) {
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ': ' : ':') + v);
                            }
                        }
                    }
                }

                v = partial.length === 0 ? '{}' :
                gap ? '{\n' + gap + partial.join(',\n' + gap) + '\n' +
                        mind + '}' : '{' + partial.join(',') + '}';
                gap = mind;
                return v;
        }
    }

    if (typeof JSON.stringify !== 'function') {
        JSON.stringify = function (value, replacer, space) {
            var i;
            gap = '';
            indent = '';

            if (typeof space === 'number') {
                for (i = 0; i < space; i += 1) {
                    indent += ' ';
                }

            } else if (typeof space === 'string') {
                indent = space;
            }
            rep = replacer;
            if (replacer && typeof replacer !== 'function' &&
                    (typeof replacer !== 'object' ||
                     typeof replacer.length !== 'number')) {
                throw new Error('JSON.stringify');
            }

            return str('', { '': value });
        };
    }

    if (typeof JSON.parse !== 'function') {
        JSON.parse = function (text, reviver) {
            var j;
            function walk(holder, key) {
                var k, v, value = holder[key];
                if (value && typeof value === 'object') {
                    for (k in value) {
                        if (Object.hasOwnProperty.call(value, k)) {
                            v = walk(value, k);
                            if (v !== undefined) {
                                value[k] = v;
                            } else {
                                delete value[k];
                            }
                        }
                    }
                }
                return reviver.call(holder, key, value);
            }

            text = String(text);
            cx.lastIndex = 0;
            if (cx.test(text)) {
                text = text.replace(cx, function (a) {
                    return '\\u' +
                        ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                });
            }

            if (/^[\],:{}\s]*$/.
		test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@').
		replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
		replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

                j = eval('(' + text + ')');

                return typeof reviver === 'function' ?
                    walk({ '': j }, '') : j;
            }
            throw new SyntaxError('JSON.parse');
        };
    }
}());


usingNamespace("Web.Utils")["Json"] = {
    // Serializes a javascript object, number, string, or arry into JSON.
    ToJson: function (object) {
        try {
            return JSON.stringify(object);
        } catch (e) { }
        return false;
    },
    // Converts from JSON to Javascript
    FromJson: function (text) {
        try {
            return JSON.parse(text);
        } catch (e) {
            return false;
        };
    }
};

(function () {
    if (typeof Date.prototype.format !== 'function') {
        Date.prototype.format = function (fmt) {
            var o = {
                "M+": this.getMonth() + 1, //月份         
                "d+": this.getDate(), //日         
                "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时         
                "H+": this.getHours(), //小时         
                "m+": this.getMinutes(), //分         
                "s+": this.getSeconds(), //秒         
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度         
                "S": this.getMilliseconds() //毫秒         
            };
            var week = {
                "0": "/u65e5",
                "1": "/u4e00",
                "2": "/u4e8c",
                "3": "/u4e09",
                "4": "/u56db",
                "5": "/u4e94",
                "6": "/u516d"
            };
            if (/(y+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            if (/(E+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[this.getDay() + ""]);
            }
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(fmt)) {
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
                }
            }
            return fmt;
        }
    }
})();