/// <reference path="jquery-1.2.6.js"/>
/// <reference path="resources/message.js"/>
/* ----------------------------------------------------------------------------------
* jQuery plugin
*
* 1.1、 utility （$.utility.replaceSelect, $.utility.restoreSelect, $.utility.showOverlay, $.utility.hideOverlay, $.utility.showLoading, $.utility.hideLoading, $.utility.switchLanguage, $.utility.switchTheme, $.utility.resizeContent, $.utility.ajax.onStart, $.utility.ajax.onstop, $.utility.redirectFromWaiting）
* 1.2、 formPost
* 1.4、 ajax post （$.ajaxPost, $.fn.ajaxPost）
* 1.5、 ozzo button disabled （$.buttonDisabled, $.fn.buttonDisabled）
* 1.6、 pop modal （$.popArea, $.fn.popArea, $.popAreaClose, $.fn.popAreaClose, $.popPage, $.alert, $.confirm）
* 1.8、 fill data to select （$.fillDataToSelect, $.fn.fillDataToSelect）
* 1.9、 extra select （$.extraSelect, $.fn.extraSelect）
* 1.11、tab control （$.tabControl, $.fn.tabControl）
* 1.14、Convert to Json string （$.ajax.toJson）
* 1.17、messageBox （$.messageBox,$.fn.messageBox）
* 1.18、format （$.format）
* 2.1、 cookie plugin for jQuery （$.cookie）
* 2.2、 tooltip plugin 1.2 for jQuery  （$.tooltip, $.fn.tooltip）
* 2.3、 bgiframe plugin 2.1 for jQuery  （$.fn.bgiframe）
* 2.4、 loadImg（$.fn.loadthumb）
* ---------------------------------------------------------------------------------- */


// version
if (typeof (oversea) != "undefined") oversea.extension = { version: "1.0.3.13" };

/* 1.1、utility function
* -------------------------------------------------- */
(function ($) {
    // utility tools
    $.utility = {
        // Fix IE 6's bug
        // replace select with text box
        // Obsolete
        replaceSelect: function (container, tag) {
            if ($.browser.msie && $.browser.version == "6.0") {
                var elem = $("body");
                if (container) elem = container;

                $("select:visible", elem).each(function () {
                    var select = $(this);
                    var textId = select.attr("id") + "_textMask";
                    var input = select.next(":text[id=" + textId + "]");

                    if (input.length < 1) {
                        input = $("<input id='" + textId + "' type='text' readonly>").insertAfter(select)
                        .addClass(select.attr("class"))
                        .hide();
                    }
                    if (select.width() > 0) {
                        input.width(select.width());
                    }
                    if (select.height() > 0) {
                        input.height(select.height());
                    }
                    if (tag) {
                        select.attr("tag", tag);
                    }
                    input.val(select.hide().children("option:selected").text()).show();
                });
            }
        },
        // Fix IE 6's bug
        // restore select from text box
        // Obsolete
        restoreSelect: function (container, tag) {
            if ($.browser.msie && $.browser.version == "6.0") {
                var elem = $("body");
                if (container) elem = container;
                $("select:hidden", elem).each(function () {
                    var select = $(this);
                    var textId = select.attr("id") + "_textMask";
                    var input = select.next(":text[id=" + textId + "]");

                    if (input.length > 0 && select.attr("tag") == tag) {
                        select.removeAttr("tag").show();
                        input.hide();
                    }
                });
            }
        },
        // show overlay
        showOverlay: function (options, overlayId, oncallback) {
            var settings = $.extend({
                bgColor: "#fff",
                opacity: 0,
                zIndex: 499
            }, options);

            //            if ($("#formMask_overlay").length == 0) {
            //                this.replaceSelect();
            //            }

            var elem = $("<div id='formMask_overlay'></div>")
                .prependTo("body")
                .attr("overlayId", overlayId)
            //.bgiframe() // for IPP Silverlight Page
                .css({
                    'backgroundColor': settings.bgColor,
                    'opacity': settings.opacity,
                    'z-index': settings.zIndex
                })
                .fadeIn("fast", oncallback);

            if ($.browser.msie && $.browser.version == "6.0") {
                elem.css("position", "absolute")
                .width($("body").width())
                .height($("body").height());
            }
        },
        //hide overlay
        hideOverlay: function (overlayId, oncallback) {
            var elem = $("[id=formMask_overlay][overlayId=" + overlayId + "]").fadeOut("normal", oncallback);

            $(".bgiframe", elem).remove();

            elem.remove();

            //            if ($("#formMask_overlay").length == 0) {
            //                this.restoreSelect();
            //            }
        },
        // show loading
        // Obsolete
        showLoading: function (requestId) {
            var elem = $("<div id=\"loading\"><p><em>" + JR("加载中")+ "</em></p></div>")
                .prependTo("body")
                .attr("requestId", requestId);

            setLocation();

            elem.show();

            $(window).resize(setLocation).scroll(setLocation);

            function setLocation() {
                elem.css({
                    "top": $(window).scrollTop(),
                    "left": $(window).scrollLeft() + ($(window).width() - elem.width()) / 2
                });
            }
        },
        // hide loading
        // Obsolete
        hideLoading: function (requestId) {
            $("[id=loading][requestId=" + requestId + "]").hide().remove();
        },
        // show loading
        showNewLoading: function (requestId, allowCancel) {
            var elem = $('<div id="NewLoading" style="height: 75px; position: absolute; text-align: center; width: 220px; z-index: 999999;"> \
                        <a href="#" class="close" title="' + JR("取消") + '"></a> \
                        <p><img src="/Resources/themes/default/Nest/img/loading.gif"/></p>\
                        <div class="loadImg"></div> \
                      </div>')
            .prependTo("body")
            .attr("requestId", requestId);

            $("a.close", elem).css({
                "display": allowCancel ? "" : "none"
            }).click(function () {
                $.ajaxPost.abort();
                if (typeof (AjaxPro) != "undefined") {
                    AjaxPro.abort();
                }
                return false;
            });

            setLocation();

            elem.show();

            $(window).resize(setLocation).scroll(setLocation);

            function setLocation() {
                elem.css({
                    "position": "absolute",
                    "top": $(window).scrollTop() + ($(window).height() - elem.height()) / 2,
                    "left": $(window).scrollLeft() + ($(window).width() - elem.width()) / 2
                });
            }
        },
        // hide loading
        hideNewLoading: function (requestId) {
            $("[id=NewLoading][requestId=" + requestId + "]").hide().remove();
        },
        //switch language
        switchLanguage: function (key, language) {
            $.cookie(key, language, { expires: 30, path: "/" });
            window.location.replace(window.location.href.split("#")[0]);
            return false;
        },
        //switch theme
        switchTheme: function (key, theme) {
            $.cookie(key, theme, { expires: 30, path: "/" });
            window.location.replace(window.location.href.split("#")[0]);
            return false;
        },
        resizeContent: function () {
            var documentHeight = document.documentElement.clientHeight;
            var bodyHeight = documentHeight - $("#header").height();

            if ($.browser.msie && $.browser.version == "6.0") {
                $("#body>.bodyWrap").height(bodyHeight);
            } else {
                $("#body>.bodyWrap").css({ "min-height": bodyHeight + "px" });
            }
        },
        // ajax global event settings
        ajax: {
            requestCount: 0,
            requestId: "ajax_globalId"
        },
        // global settings
        settings: {
            popDraggable: true,
            showLoading: true
        },
        redirectByLink: function (link) {
            var destPath = $(link).attr("href");
            if (destPath) {
                $.utility.showOverlay({ bgColor: "#fff", opacity: 0, zIndex: 1000 }, -1);
                $.utility.showNewLoading(-1);

                $('<form id="redirectForm" action="' + destPath + '" method="post"></form>').appendTo("body").submit();
            }
            return false;
        },
        redirectFromWaiting: function (destPath) {
            if (destPath != null && destPath != "") {
                document.body.innerHTML =
'<form id="waitingForm" action="' + destPath + '" method="post"> \
    <div id="wrapper"> \
        <div id="header"> \
            <div class="clear"></div> \
            <div class="logo" style="width:100%;"></div> \
            <div class="clear"></div> \
            <div class="headLine"><span><span></span></span></div> \
            <div class="clear"></div> \
        </div> \
        <div class="clear"></div> \
        <div style="padding: 150px 50px; text-align: center;"> \
            <h1>' + JR("加载中") + '</h1> \
            <object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,28,0" width="208" height="14"> \
                <param name="movie" value="/Resources/themes/images/loading.cc.swf" /> \
                <param name="quality" value="high" /> \
                <param name="wmode" value="transparent"> \
                <embed src="/App_Themes/_Theme_/images/loading.swf" quality="high" pluginspage="http://www.adobe.com/shockwave/download/download.cgi?P1_Prod_Version=ShockwaveFlash" type="application/x-shockwave-flash" width="208" height="14" wmode="transparent"></embed> \
            </object> \
        </div> \
    </div> \
</form>';
                document.forms[0].submit();
            }
            return false;
        }
    };
})(jQuery);

/* 1.2、formPost
* -------------------------------------------------- */
(function ($) {
    $.formPost = function () {
        var settings = {
            url: "#",
            target: "_blank",
            method: "post"
        };

        var params = [];
        var options = $.extend(settings);
        if (arguments.length > 0) {
            options = $.extend(settings, arguments[arguments.length - 1]);

            for (var i = 0; i < arguments.length - 1; i++) {
                params.push(arguments[i]);
            }
        }

        var form = $('<form id="__jsonForm__" name="__jsonForm__"></form>').appendTo($("body"))
            .attr("action", settings.url)
            .attr("target", settings.target)
            .attr("method", settings.method);

        if (params.length > 0) {
            var jsonString = $.ajax.toJSON(params);
            var hidden = $('<input id="__jsonString__" name="__jsonString__" type="hidden" value="" />').appendTo(form)
            .val(jsonString);
        }

        form.submit().remove();
    }
})(jQuery);

//1.4、 ajax post 
$(document).ajaxSend(onAjaxSend)
            .ajaxComplete(onComplete)
            .ajaxSuccess(onSuccess)
            .ajaxError(onError);

function onAjaxSend(event, xhr, settings) {
    if ($.utility.ajax.requestCount++ == 0 && $.utility.settings.showLoading) {
        $.utility.showOverlay({ bgColor: "#fff", opacity: 0, zIndex: 1000 }, $.utility.ajax.requestId);
        if ($.utility.settings.showLoading)
            $.utility.showNewLoading($.utility.ajax.requestId, false);
    }
}
function onComplete(event, xhr, settings) {
    if ($.utility.ajax.requestCount-- == 1 && $.utility.settings.showLoading) {
        $.utility.hideOverlay($.utility.ajax.requestId);
        $.utility.hideNewLoading($.utility.ajax.requestId);
    }
}
function onSuccess(event, xhr, settings) {
    if (!xhr.isHandled && xhr.responseText != "") {
        if (settings.dataType == 'json') {
            var jsonValue = jQuery.parseJSON(xhr.responseText);
            if (jsonValue.error == true) {
                //$.alert("<br />" + jsonValue.message, "error");
                alert(jsonValue.message);
            }
        }
        else if (settings.dataType == 'html') {
            var jqError = $(xhr.responseText).filter("#service_Error_Message_Panel");
            if (jqError.length > 0) {
                //$.alert("<br />" + jqError.find(" #errorMessage").val(), "error");
                alert(jqError.find(" #errorMessage").val());
            }
        }
        else if (settings.dataType == 'xml') {

        }
        else if (settings.dataType == 'script') {

        }
    }
}
function onError(event, xhr, settings) {
    if (xhr.status != 0) {
        //$.alert("<br />" + JR("请求发生错误")+" !<br/> Status Code:" + xhr.status + " .<br /> url:" + settings.url, "error");
        alert(JR("请求发生错误"));
    }
}

/* 1.6、pop modal
* -------------------------------------------------- */
(function ($) {
    // pop area
    $.popArea = function (elem, title, onclose, options) {
        //debugger;
        var msg = $($.popModal.resources.pop_area_content);

        if (title) $(".caption h1", msg).text(title);
        $("#popContent", msg).append(elem.after(msg));

        var settings = $.popModal.init(elem, options);
        $.popModal.show(msg, settings, null, elem);

        elem.bind("event_close", function () {
            settings = $.popModal.getSettings(elem);

            $.popModal.close(elem, settings);

            if ($.isFunction(onclose)) onclose.call($);

            elem.unbind();

            return elem;
        });

        $(".close", msg).click(function () {
            elem.trigger("event_close");
            $(document).trigger("click"); // hide combox's menu list

            return false;
        });
        $(document).keyup(function (ev) {
            if (ev.keyCode == 27) {
                $(".close", msg).click();
                return false;
            }
        });
    };
    $.fn.popArea = function (title, onclose, options) {
        return $.popArea(this, title, onclose, options);
    };

    // close pop area
    $.popClose = function (elem) {
        return elem.trigger("event_close");
    };
    $.fn.popClose = function () {
        return this.trigger("event_close");
    };

    // pop page
    $.popPage = function (url, title, width, height, onclose, options) {
        var msg = $($.popModal.resources.pop_iframe_content).appendTo("body");

        width = width || 700;
        height = height || 460;

        var winWidth = $(window).width();
        var winHeight = $(window).height() - 70;
        var hwRate = height / width;

        if (width > winWidth) {
            width = winWidth * 0.9;
            height = width * hwRate;
        }

        height = height > winHeight ? winHeight : height;

        if (title) $(".caption h1", msg).text(title);
        if (url) $("#popContent", msg).data("url", url).width(width).height(height);

        var settings = $.popModal.init(msg, options);
        $.popModal.show(msg, settings);

        var loading = $("#loading", msg);
        loading.children("p").html(JR("加载中"))
        .end().css({ "top": (height - loading.height()) / 2, "left": (width - loading.width()) / 2 });

        var elem = $("iframe[id=popContent]", msg);
        elem.bind("load", function () {
            $("#loading", msg).fadeOut("normal", function () { $(this).remove(); });
        }).bind("event_close", function () {
            settings = $.popModal.getSettings(msg);
            $(this).unbind().attr("src", "javascript:false;").remove();

            $.popModal.close(msg, settings).remove();
            if ($.isFunction(onclose)) onclose.call($);

            elem = null;
            return null;
        });

        $(".close", msg).click(function () {
            elem.trigger("event_close");
            return false;
        });

        $(document).keyup(function (ev) {
            if (ev.keyCode == 27) {
                $(".close", msg).click();
                return false;
            }
        });
    };

    // alert modal
    $.alert = function (message, type, okCallback) {
        var msg = $($.popModal.resources.pop_alert_content).appendTo("body");

        switch (type) {
            case $.popModal.resources.pop_alert_types[0]:
                $(".op_information", msg).attr("class", "op_error");
                $(".caption h1", msg).text(JR("错误"));
                break;
            case $.popModal.resources.pop_alert_types[1]:
                $(".op_information", msg).attr("class", "op_warning");
                $(".caption h1", msg).text(JR("警告"));
                break;
            case $.popModal.resources.pop_alert_types[2]:
            default:
                $(".op_information", msg).attr("class", "op_information");
                $(".caption h1", msg).text(JR("信息"));
                break;
        }
        if (message) $("#popContent", msg).html(message);

        var settings = $.popModal.init(msg, { overlaySettings: { opacity: 0 } });
        $.popModal.show(msg, settings);

        $("#btnOK", msg).text(JR("确定"))
        .focus()
        .click(function () {
            $("#btnOK, .close", msg).unbind();

            settings = $.popModal.getSettings(msg);
            $.popModal.close(msg, settings).remove();

            if ($.isFunction(okCallback)) okCallback.call($);

            return false;
        });

        $(".close", msg).click(function () {
            $("#btnOK, .close", msg).unbind();

            settings = $.popModal.getSettings(msg);
            $.popModal.close(msg, settings).remove();

            return false;
        });
        $(document).keyup(function (ev) {
            if (ev.keyCode == 27) {
                $(".close", msg).click();
                return false;
            }
        });
    };

    // confirm modal
    $.confirm = function (message, yesCallback, noCallback) {
        var msg = $($.popModal.resources.pop_confirm_content).appendTo("body");

        $(".caption h1", msg).text(JR("确定？"));
        if (message) $("#popContent", msg).html(message);

        var settings = $.popModal.init(msg, { overlaySettings: { opacity: 0 } });
        $.popModal.show(msg, settings);

        $("#btnYes", msg).text(JR("是"))
        .focus()
        .click(function () {
            $("#btnYes, #btnNo, .close", msg).unbind();

            settings = $.popModal.getSettings(msg);
            $.popModal.close(msg, settings).remove();

            if ($.isFunction(yesCallback)) yesCallback.call($);

            return false;
        });

        $("#btnNo", msg).text(JR("否"))
        .click(function () {
            $("#btnYes, #btnNo, .close", msg).unbind();

            settings = $.popModal.getSettings(msg);
            $.popModal.close(msg, settings).remove();

            if ($.isFunction(noCallback)) noCallback.call($);

            return false;
        });

        $(".close", msg).click(function () {
            $("#btnYes, #btnNo, .close", msg).unbind();

            settings = $.popModal.getSettings(msg);
            $.popModal.close(msg, settings).remove();

            if ($.isFunction(noCallback)) noCallback.call($);

            return false;
        });
        $(document).keyup(function (ev) {
            if (ev.keyCode == 27) {
                $(".close", msg).click();
                return false;
            }
        });
    };

    // pop modal base implemtation
    $.popModal = {
        currentId: 101,
        getSettings: function (elem) {
            return elem.data("settings");
        },

        clearSettings: function (elem) {
            elem.removeData("settings");
        },

        init: function (elem, options, overwrite) {
            var settings = this.getSettings(elem);
            if (settings != undefined && (overwrite == undefined || overwrite == false)) {
                return settings;
            }

            settings = $.extend({
                modalId: "modelId_" + $.popModal.currentId++,
                isShowing: false,
                overlaySettings: {}
            }, options);

            settings.display = elem.css("display");
            settings.visibility = elem.css("visibility");

            elem.data("settings", settings);

            return this.getSettings(elem);
        },

        show: function (elem, settings, oncallback, content) {
            if (settings && settings.isShowing == false) {
                var popContainer = $($.popModal.resources.pop_container).insertAfter(elem).attr("modalId", settings.modalId).bgiframe();
                $(".body", popContainer).append(elem);

                var popContent = $("#popContent", elem);
                if ($(popContent).is("iframe")) {
                    var url = $(popContent).data("url");
                    $(popContent).attr("src", url).removeData("url");
                }

                if (content) {
                    content.show();
                }
                if ($.ui && $.ui.draggable && $.utility.settings.popDraggable) {
                    $('.caption', popContainer).css({
                        cursor: 'move'
                    });
                    popContainer.draggable({ handle: '.caption', containment: 'document' });
                }

                function setLocation() {
                    var maskTop = ($(window).height() - popContainer.height()) / 3;
                    var maskLeft = ($(window).width() - popContainer.width()) / 2;

                    if ($.browser.mozilla && $.browser.version.substr(0, 3) == "1.8") { // fixed Firefox 2 bugs
                        popContainer.css({ position: "absolute" });

                        window.setTimeout(function () {
                            popContainer.css({
                                position: "fixed",
                                top: maskTop > 0 ? maskTop : 0,
                                left: maskLeft > 0 ? maskLeft : 0
                            }).show();
                        }, 0);
                    } else if ($.browser.msie && $.browser.version == "6.0") { // fixed IE 6 bugs
                        maskTop += $(window).scrollTop();
                        maskLeft += $(window).scrollLeft();

                        popContainer.css({
                            position: "absolute",
                            top: maskTop > 0 ? maskTop : 0,
                            left: maskLeft > 0 ? maskLeft : 0
                        }).show();
                    } else {
                        popContainer.css({
                            position: "fixed",
                            top: maskTop > 0 ? maskTop : 0,
                            left: maskLeft > 0 ? maskLeft : 0
                        }).show();
                    }
                }

                setLocation();
                $(window).resize(setLocation);

                settings.isShowing = true;

                $.utility.restoreSelect(elem);
                $.utility.showOverlay(settings.overlaySettings, settings.modalId, oncallback);
            }

            return elem;
        },

        close: function (elem, settings, oncallback) {
            if (settings && settings.isShowing == true) {
                $.utility.hideOverlay(settings.modalId);

                if ($.isFunction(oncallback)) oncallback.call($);

                var popContainer = $("[id=formMask][modalId=" + settings.modalId + "]");

                $(".bgiframe", popContainer).remove();

                popContainer.after(elem).remove();

                elem.css({ "display": settings.display, "visibility": settings.visibility });

                popContainer = null;
                settings.isShowing = false;
            }

            return elem;
        },

        resources: {
            pop_container:
            '<div id="formMask" style="display:none;"> \
                <table> \
                    <tr><td class="tl"/><td class="b"/><td class="tr"/></tr> \
                    <tr> \
                      <td class="b">&nbsp;</td> \
                      <td class="body"></td> \
                      <td class="b">&nbsp;</td> \
                    </tr> \
                    <tr><td class="bl"/><td class="b"/><td class="br"/></tr> \
                </table> \
            </div>',
            pop_area_content:
            '<table class="pContent"> \
                <tr><td class="caption"><h1>Pop Area</h1><a href="#" class="close">&nbsp;</a></td></tr> \
                <tr><td><div id="popContent"></div></td></tr> \
            </table>',
            pop_iframe_content:
            '<table class="pContent"> \
                <tr><td class="caption"><h1>Pop Page</h1><a href="#" class="close" id="lnkPOPClose">&nbsp;</a></td></tr> \
                <tr><td><div id=\"loading\" class=\"loading\"><p></p><div class="loadImg"></div></div><iframe id="popContent"></iframe></td></tr> \
            </table>',
            pop_alert_types: ['error', 'warning', 'info'],
            pop_alert_content:
            '<table class="pContent fontNormal"> \
                <tr><td class="caption"><h1>Alert</h1><a href="#" class="close">&nbsp;</a></td></tr> \
                <tr> \
                    <td> \
                        <table class="opInfo"> \
    	                    <tr> \
        	                    <td class="op_information"><div style="width: 60px;"></div></td> \
                                <td id="popContent" align="left"></td> \
                            </tr> \
                        </table></td> \
                </tr> \
                <tr> \
                    <td> \
	                    <div class="opBtn"> \
	                        <span class="buttonCompact"><span><a id="btnOK" href="#">OK</a></span></span></div></td> \
                </tr> \
            </table>',
            pop_confirm_content:
            '<table class="pContent fontNormal"> \
                <tr><td class="caption"><h1>Query</h1><a href="#" class="close">&nbsp;</a></td></tr> \
                <tr> \
	                <td> \
                        <table class="opInfo"> \
        	                <tr> \
            	                <td class="op_query"><div style="width: 60px;"></div></td> \
                                <td id="popContent" align="left"></td> \
                            </tr> \
                        </table></td> \
                </tr> \
                <tr> \
                    <td> \
    	                <div class="opBtn"> \
    	                    <span class="buttonCompact"><span><a id="btnYes" href="#">Yes</a></span></span> \
    	                    <span class="buttonCompact"><span><a id="btnNo" href="#">No</a></span></span></div></td> \
                </tr> \
            </table>'
        }
    };
})(jQuery);


/* 1.8、fill data to select
* -------------------------------------------------- */
(function ($) {
    $.fillDataToSelect = function (elems, data, options) {
        return $.fillDataToSelect.impl(elems, data, options);
    };

    $.fn.fillDataToSelect = function (data, options) {
        return $.fillDataToSelect.impl(this, data, options);
    };

    $.fillDataToSelect.impl = function (elems, data, options) {
        var settings = $.extend({
            key: "Code",
            value: "Name"
        }, options);

        return elems.each(function () {
            var elem = $(this);

            if (data && this.tagName == "SELECT") {
                elem.empty();
                for (var i = 0; i < data.length; i++) {
                    elem.append("<option value=\"" + data[i][settings.key] + "\">" + data[i][settings.value] + "</option>");
                }
            }
        });
    };
})(jQuery);


/* 1.9、extra select
* -------------------------------------------------- */
//(function ($) {
//    $.extraSelect = function (elems, options) {
//        return $.extraSelect.impl(elems, options);
//    };

//    $.fn.extraSelect = function (options) {
//        return $.extraSelect.impl(this, options);
//    };

//    $.extraSelect.impl = function (elems, options) {
//        var settings = $.extend({
//            type: null, // select, all, none
//            isSelected: true,
//            isPrepend: true,
//            text: ScriptResources.MessageResource.extraSelect_select_text,
//            value: ScriptResources.MessageResource.extraSelect_select_value
//        }, options);

//        if (settings.type) {
//            var tempText = ScriptResources.MessageResource["extraSelect_" + settings.type.toLowerCase() + "_text"];
//            var tempValue = ScriptResources.MessageResource["extraSelect_" + settings.type.toLowerCase() + "_value"];

//            if (tempText != undefined && tempValue != undefined) {
//                settings.text = tempText;
//                settings.value = tempValue;
//            } else {
//                settings.type = "select";
//            }
//        }

//        return elems.each(function () {
//            var elem = $(this);

//            if (this.tagName == "SELECT") {
//                if (settings.isPrepend) {
//                    elem.prepend("<option value=\"" + settings.value + "\">" + settings.text + "</option>");
//                    if (settings.isSelected) {
//                        try {
//                            $("option:first", elem).attr("selected", true);
//                        } catch (e) { }
//                    }
//                } else {
//                    elem.append("<option value=\"" + settings.value + "\">" + settings.text + "</option>");
//                    if (settings.isSelected) {
//                        try {
//                            $("option:last", elem).attr("selected", true);
//                        } catch (e) { }
//                    }
//                }
//            }
//        });
//    };
//})(jQuery);

/* 1.11 tab control
* example:
*
* var items = [{"tab1":$('#item1')},{"tab2":$('#item2')},{"tab3":$('#item3')}]; //items : tab's information,{'tabName':$('#tabobject')}
* var options = {
*   tabcss:"",             //"<ul>"'s style
*   tabcellcss:"",         //"<li>"'s style
*   tabcellovercss:"",     //"<li>"'s mouseover
*   tabcellselectedcss:"", //"<li>"'s selected
*   onchange:null          //set a method to onchange,example:function(oldTab,newTab){//do something};
* }
* var tabs = $("#tabContainer").tabControl(items,options);
*
* --------------------------------------------------------------*/
(function ($) {
    $.tabControl = function (element, items, options) {
        return $.tabControl.impl(element, items, options);
    };

    $.fn.tabControl = function (items, options) {
        var tab = $(this).data("tabControl");
        if (typeof (tab) == "undefined") {
            tab = new $.tabControl.impl(this, items, options);
            tab.init();
            $(this).data("tabControl", tab);
        }
        return tab;
    };

    $.tabControl.impl = function (elem, items, options) {
        this.options = {
            tabcss: "tabs",
            tabcellcss: "",
            tabcellovercss: "",
            tabcellselectedcss: "over",
            onchange: null
        };
        this.container = $("<ul></ul>");
        this.items = [];
        this.currentItem = null;
        this.init = function () {
            if (typeof (options) != "undefined" && options != null)
                for (var key in options) {
                    if (this.options.hasOwnProperty(key.toLowerCase()))
                        this.options[key.toLowerCase()] = options[key];
                }
            $(elem).prepend(this.container);
            if (typeof (items) != "undefined" && items != null)
                if (items.constructor == Array)
                    this.add(items);
            this.container.addClass(this.options.tabcss);
            if (this.items.length > 0) {
                this.currentItem = this.items[0];
                this.selectTab(this.items[0].tab);
            }
        };
        //items:[{"tabName1":$("#item1")},{"tabName2":$("#item2"},{"tabName3":$("#item3"}]
        this.add = function (items) {
            var _self = this;
            $.each(items, function (i, item) {
                for (var key in item) {
                    var cell = $("<li style='display: inline;list-style-type: none;'><a href='javascript:void(0);'></a></li>").appendTo(_self.container)
                                                  .addClass(_self.options.tabcellcss)
                                                  .click(function (event) { _self.selectTab(event.srcElement || event.target); })
                                                  .hover(function (event) {
                                                      var cell = $(this);
                                                      cell.removeClass(_self.options.tabcellcss)
                                                          .removeClass(_self.options.tabcellselectedcss)
                                                          .addClass(_self.options.tabcellovercss);
                                                  }, function (event) {
                                                      var cell = $(this);
                                                      var dataItem = cell.data("tabItem");
                                                      cell.removeClass(_self.options.tabcellovercss);
                                                      if (_self.currentItem != null) {
                                                          if (_self.currentItem.index == dataItem.index) {
                                                              cell.addClass(_self.options.tabcellselectedcss);
                                                              return;
                                                          }
                                                      }
                                                      cell.addClass(_self.options.tabcellcss);
                                                  });
                    var index = _self.items.push({ 'index': 0, 'tab': cell, 'key': key, 'item': $(item[key]) }) - 1;
                    _self.items[index].index = index;
                    cell.data("tabItem", _self.items[index])
                        .children().text(key);
                }
            });
        };
        this.selectTab = function (tab) {
            var _self = this;
            $.each(this.items, function (index, item) {
                item.tab.removeClass(_self.tabcellovercss)
                         .removeClass(_self.tabcellselectedcss)
                         .addClass(_self.tabcellcss);
                item.item.hide();
            });
            var tabItem;
            if (!isNaN(tab)) {
                tabItem = this.items[tab];
            }
            else {
                if ($(tab)[0].tagName == "A")
                    tab = $(tab).parent();
                tabItem = $(tab).data("tabItem");
            }
            tabItem.tab.removeClass(this.options.tabcellcss);
            tabItem.tab.addClass(this.options.tabcellselectedcss);
            tabItem.item.show();
            if (this.options.onchange != null)
                this.options.onchange(this.currentItem, tabItem);
            this.currentItem = tabItem;
        };
        // tab: tab's element || index
        this.remove = function (tab) {
            var tabItem;
            if (isNaN(tab)) {
                tabItem = this.items[tab];
            }
            else {
                tabItem = $(tab).data("tabItem");
            }
            if (this.currentItem != null)
                if (this.currentItem.index == tabItem.index)
                    this.currentItem = null;
            tabItem.tab.remove();
            this.items.splice(tabItem.index, 1);
        };
    };
})(jQuery);


/* 1.14、Convert to Json string （$.ajax.toJson）
*-------------------------------------------------- */
$.ajax.toJSON = function (jsonObject) {
    var filers = {
        '\b': '\\b',
        '\t': '\\t',
        '\n': '\\n',
        '\f': '\\f',
        '\r': '\\r',
        '"': '\\"',
        '\\': '\\\\'
    };
    var handler = function (o) {
        if (o == null) {
            return "null";
        }
        var v = [];
        var i;
        var c = o.constructor;
        if (c == Number) {
            return isFinite(o) ? o.toString() : handler(null);
        } else if (c == Boolean) {
            return o.toString();
        } else if (c == String) {
            if (/["\\\x00-\x1f]/.test(o)) {
                o = o.replace(/([\x00-\x1f\\"])/g, function (a, b) {
                    var c = filers[b];
                    if (c) {
                        return c;
                    }
                    c = b.charCodeAt();
                    return '\\u00' +
						Math.floor(c / 16).toString(16) +
						(c % 16).toString(16);
                });
            }
            return '"' + o + '"';
        } else if (c == Array) {
            for (i = 0; i < o.length; i++) {
                v.push(handler(o[i]));
            }
            return "[" + v.join(",") + "]";
        } else if (c == Date) {
            return "new Date(" + new Date(Date.UTC(o.getUTCFullYear(), o.getUTCMonth(), o.getUTCDate(), o.getUTCHours(), o.getUTCMinutes(), o.getUTCSeconds(), o.getUTCMilliseconds())).getTime() + ")";
        }
        if (typeof o.toJSON == "function") {
            return o.toJSON();
        }
        if (typeof o == "object") {
            for (var attr in o) {
                if (typeof o[attr] != "function") {
                    v.push('"' + attr + '":' + handler(o[attr]));
                }
            }
            if (v.length > 0) {
                return "{" + v.join(",") + "}";
            }
            return "{}";
        }
        return o.toString();
    };

    return handler(jsonObject);
};

/* 1.17、messageBox （$.messageBox）
*-------------------------- */
(function ($) {
    $.messageBox = function (element, message, options) {
        return new $.messageBox.impl(element, message, options);
    };

    $.fn.messageBox = function (message, options) {
        var element = $(this);
        var box;
        if ((box = element.data("messageBox")) == null) {
            box = $.messageBox(this, message, options);
            element.data("messageBox", box);
        }
        return box;
    };

    $.messageBox.impl = function (element, message, options) {
        var _self = this;
        this.element = element;
        this.options = {
            css: "tipsWarning_top",
            attribute: "innerText",
            type: "warning"
        };

        if (options) {
            $.extend(this.options, options);
        }
        this.setStyle = function () {
            if (this.options.type.toLowerCase() == "success") {
                this.options.css = "tipsInfo_top";
            } else if (this.options.type.toLowerCase() == "warning") {
                this.options.css = "tipsWarning_top";
            } else {
                this.options.css = "tipsError_top";
            }
            this.element.removeClass("tipsWarning_top").removeClass("tipsInfo_top").removeClass("tipsError_top")
            .addClass(this.options.css);
        };
        this.setStyle();
        this.write = function (msg, isAppend, type) {
            if (type) {
                if (type.toLowerCase() == "success") {
                    this.options.type = "success";
                } else if (type.toLowerCase() == "error") {
                    this.options.type = "error";
                } else {
                    this.options.type = "warning";
                }
                this.setStyle();
            }

            if (this.options.attribute.toLowerCase() == "innertext") {
                this.element.text(msg);
            } else {
                this.element.html(msg);
            }
        };
        this.writeError = function (error, isAppend) {
            if (error.IsBizException) {
                this.write(error.Message, isAppend, "warning");
            } else {
                this.write(error.Message, isAppend, "error");
            }
        };
        this.show = function (msg, type) {
            if (msg && !type) {
                this.write(msg, false, "warning");
            } else if (msg && type) {
                this.write(msg, false, type);
            }
            this.element.show();
        };
        this.showError = function (error) {
            this.show(error.Message, error.IsBizException ? "warning" : "error");
        };
        this.hide = function (isClear) {
            if (isClear) {
                this.clear();
            }
            this.element.hide();
        };
        this.clear = function () {
            this.element.html("");
            this.options.type = "warning";
            this.setStyle();
        };
        if (message) {
            this.write(message, this.options.isAppend);
            this.show();
        } else {
            this.hide();
        }
    };
})(jQuery);



/* 1.18、format
*-------------------------- */
(function ($) {
    $.format = function (formatString) {
        var regx;
        if (formatString) {
            args = [];
            for (var index = 1; index < arguments.length; index++) {
                regx = new RegExp("{[" + (index - 1) + "]}");
                formatString = formatString.replace(regx, arguments[index]);
            }
            return formatString;
        }
    };
})(jQuery);

/* 2.1、cookie plugin for jQuery
* Copyright (c) 2006 Klaus Hartl (stilbuero.de)
* Dual licensed under the MIT and GPL licenses:
* http://plugins.jquery.com/project/Cookie
* -------------------------------------------------- */
jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        // CAUTION: Needed to parenthesize options.path and options.domain
        // in the following expressions, otherwise they evaluate to undefined
        // in the packed version for some reason...
        var path = options.path ? '; path=' + (options.path) : '';
        var domain = options.domain ? '; domain=' + (options.domain) : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};


/* 2.2、tooltip plugin 1.2 for jQuery
* http://bassistance.de/jquery-plugins/jquery-plugin-tooltip/
* http://docs.jquery.com/Plugins/Tooltip
* -------------------------------------------------- */
(function ($) {
    // the tooltip element
    var helper = {},
    // the current tooltipped element
		current,
    // the title of the current element, used for restoring
		title,
    // timeout id for delayed tooltips
		tID,
    // IE 5.5 or 6
		IE = $.browser.msie && /MSIE\s(5\.5|6\.)/.test(navigator.userAgent),
    // flag for mouse tracking
		track = false;

    $.tooltip = {
        blocked: false,
        defaults: {
            delay: 200,
            showURL: true,
            extraClass: "",
            top: 15,
            left: 15,
            id: "tooltip"
        },
        block: function () {
            $.tooltip.blocked = !$.tooltip.blocked;
        }
    };

    $.fn.Tooltip = $.fn.tooltip;

    $.fn.extend({
        tooltip: function (settings) {
            settings = $.extend({}, $.tooltip.defaults, settings);
            createHelper(settings);
            var _self = this;
            return this.each(function () {
                $.data(this, "tooltip-settings", settings);
                // copy tooltip into its own expando and remove the title
                this.tooltipText = this.title;
                $(this).removeAttr("title");
                // also remove alt attribute to prevent default tooltip in IE
                this.alt = "";
            })
				.hover(save, function () { window.setTimeout(function () { hide({ srcElement: _self[0] }); }, 0); })
				.click(function () { window.setTimeout(function () { hide({ srcElement: _self[0] }); }, 0); });
        },
        fixPNG: IE ? function () {
            return this.each(function () {
                var image = $(this).css('backgroundImage');
                if (image.match(/^url\(["']?(.*\.png)["']?\)$/i)) {
                    image = RegExp.$1;
                    $(this).css({
                        'backgroundImage': 'none',
                        'filter': "progid:DXImageTransform.Microsoft.AlphaImageLoader(enabled=true, sizingMethod=crop, src='" + image + "')"
                    }).each(function () {
                        var position = $(this).css('position');
                        if (position != 'absolute' && position != 'relative')
                            $(this).css('position', 'relative');
                    });
                }
            });
        } : function () { return this; },
        unfixPNG: IE ? function () {
            return this.each(function () {
                $(this).css({ 'filter': '', backgroundImage: '' });
            });
        } : function () { return this; },
        hideWhenEmpty: function () {
            return this.each(function () {
                $(this)[$(this).html() ? "show" : "hide"]();
            });
        },
        url: function () {
            return this.attr('href') || this.attr('src');
        }
    });

    function createHelper(settings) {
        // there can be only one tooltip helper
        if (helper.parent)
            return;
        // create the helper, h3 for title, div for url
        helper.parent = $('<div id="' + settings.id + '"><h3></h3><div class="body"></div><div class="url"></div></div>')
        // add to document
			.appendTo(document.body)
        // hide it at first
			.hide();

        // apply bgiframe if available
        if ($.fn.bgiframe)
            helper.parent.bgiframe();

        // save references to title and url elements
        helper.title = $('h3', helper.parent);
        helper.body = $('div.body', helper.parent);
        helper.url = $('div.url', helper.parent);
    }

    function settings(element) {
        return $.data(element, "tooltip-settings");
    }

    // main event handler to start showing tooltips
    function handle(event) {
        // show helper, either with timeout or on instant
        if (settings(this).delay)
            tID = setTimeout(show, settings(this).delay);
        else
            show();

        // if selected, update the helper position when the mouse moves
        track = !!settings(this).track;
        $(document.body).bind('mousemove', update);

        // update at least once
        update(event);
    }

    // save elements title before the tooltip is displayed
    function save() {
        // if this is the current source, or it has no title (occurs with click event), stop
        if ($.tooltip.blocked || this == current || (!this.tooltipText && !settings(this).bodyHandler))
            return;

        // save current
        current = this;
        title = this.tooltipText;

        if (settings(this).bodyHandler) {
            helper.title.hide();
            var bodyContent = settings(this).bodyHandler.call(this);
            if (bodyContent.nodeType || bodyContent.jquery) {
                helper.body.empty().append(bodyContent)
            } else {
                helper.body.html(bodyContent);
            }
            helper.body.show();
        } else if (settings(this).showBody) {
            var parts = title.split(settings(this).showBody);
            helper.title.html(parts.shift()).show();
            helper.body.empty();
            for (var i = 0, part; part = parts[i]; i++) {
                if (i > 0)
                    helper.body.append("<br/>");
                helper.body.append(part);
            }
            helper.body.hideWhenEmpty();
        } else {
            helper.title.html(title).show();
            helper.body.hide();
        }

        // if element has href or src, add and show it, otherwise hide it
        if (settings(this).showURL && $(this).url())
            helper.url.html($(this).url().replace('http://', '')).show();
        else
            helper.url.hide();

        // add an optional class for this tip
        helper.parent.addClass(settings(this).extraClass);

        // fix PNG background for IE
        if (settings(this).fixPNG)
            helper.parent.fixPNG();

        handle.apply(this, arguments);
    }

    // delete timeout and show helper
    function show() {
        tID = null;
        helper.parent.show();
        update();
    }

    /**
    * callback for mousemove
    * updates the helper position
    * removes itself when no current element
    */
    function update(event) {
        if ($.tooltip.blocked)
            return;

        // stop updating when tracking is disabled and the tooltip is visible
        if (!track && helper.parent.is(":visible")) {
            $(document.body).unbind('mousemove', update)
        }

        // if no current element is available, remove this listener
        if (current == null) {
            $(document.body).unbind('mousemove', update);
            return;
        }

        // remove position helper classes
        helper.parent.removeClass("viewport-right").removeClass("viewport-bottom");

        var left = helper.parent[0].offsetLeft;
        var top = helper.parent[0].offsetTop;
        if (event) {
            // position the helper 15 pixel to bottom right, starting from mouse position
            left = event.pageX + settings(current).left;
            top = event.pageY + settings(current).top;
            helper.parent.css({
                left: left + 'px',
                top: top + 'px'
            });
        }

        var v = viewport(),
			h = helper.parent[0];
        // check horizontal position
        if (v.x + v.cx < h.offsetLeft + h.offsetWidth) {
            left -= h.offsetWidth + 20 + settings(current).left;
            helper.parent.css({ left: left + 'px' }).addClass("viewport-right");
        }
        // check vertical position
        if (v.y + v.cy < h.offsetTop + h.offsetHeight) {
            top -= h.offsetHeight + 20 + settings(current).top;
            helper.parent.css({ top: top + 'px' }).addClass("viewport-bottom");
        }
    }

    function viewport() {
        return {
            x: $(window).scrollLeft(),
            y: $(window).scrollTop(),
            cx: $(window).width(),
            cy: $(window).height()
        };
    }

    // hide helper and restore added classes and the title
    function hide(event) {
        if ($.tooltip.blocked)
            return;
        // clear timeout if possible
        if (tID)
            clearTimeout(tID);
        // no more current element
        current = null;

        helper.parent.hide().removeClass(settings(event.srcElement).extraClass);

        if (settings(event.srcElement).fixPNG)
            helper.parent.unfixPNG();

        if (helper.parent[0].style.display != "none")
            window.setTimeout(function () { hide(event); }, 10);
    }
})(jQuery);


/* 2.3、bgiframe plugin 2.1 for jQuery
* http://plugins.jquery.com/project/bgiframe
* -------------------------------------------------- */
(function ($) {
    $.fn.bgIframe = $.fn.bgiframe = function (s) {
        // This is only for IE6
        if ($.browser.msie/* && parseInt($.browser.version) <= 6 // for IPP Silverlight Page */) {
            s = $.extend({
                top: 'auto', // auto == .currentStyle.borderTopWidth
                left: 'auto', // auto == .currentStyle.borderLeftWidth
                width: 'auto', // auto == offsetWidth
                height: 'auto', // auto == offsetHeight
                opacity: true,
                src: 'javascript:false;'
            }, s || {});
            var prop = function (n) { return n && n.constructor == Number ? n + 'px' : n; };
            //html = '<iframe class="bgiframe" frameborder="0" tabindex="-1" src="' + s.src + '"' +
            //           ' style="display:block;position:absolute;z-index:-1;' +
            //               (s.opacity !== false ? 'filter:Alpha(Opacity=\'0\');' : '') +
            //		       'top:' + (s.top == 'auto' ? 'expression(((parseInt(this.parentNode.currentStyle.borderTopWidth)||0)*-1)+\'px\')' : prop(s.top)) + ';' +
            //		       'left:' + (s.left == 'auto' ? 'expression(((parseInt(this.parentNode.currentStyle.borderLeftWidth)||0)*-1)+\'px\')' : prop(s.left)) + ';' +
            //		       'width:' + (s.width == 'auto' ? 'expression(this.parentNode.offsetWidth+\'px\')' : prop(s.width)) + ';' +
            //		       'height:' + (s.height == 'auto' ? 'expression(this.parentNode.offsetHeight+\'px\')' : prop(s.height)) + ';' +
            //		'"/>';
            var iframeR = document.createElement("iframe");
            iframeR.setAttribute("class", "bgiframe");
            iframeR.setAttribute("frameborder", 0);
            iframeR.setAttribute("tabindex", -1);
            iframeR.setAttribute("src", s.src);
            iframeR.style.cssText = 'display:block;position:absolute;z-index:-1;' +
			               (s.opacity !== false ? 'filter:Alpha(Opacity=\'0\');' : '') +
					       'top:' + (s.top == 'auto' ? 'expression(((parseInt(this.parentNode.currentStyle.borderTopWidth)||0)*-1)+\'px\')' : prop(s.top)) + ';' +
					       'left:' + (s.left == 'auto' ? 'expression(((parseInt(this.parentNode.currentStyle.borderLeftWidth)||0)*-1)+\'px\')' : prop(s.left)) + ';' +
					       'width:' + (s.width == 'auto' ? 'expression(this.parentNode.offsetWidth+\'px\')' : prop(s.width)) + ';' +
					       'height:' + (s.height == 'auto' ? 'expression(this.parentNode.offsetHeight+\'px\')' : prop(s.height)) + ';';
            return this.each(function () {
                if ($('> iframe.bgiframe', this).length == 0)
                    //this.insertBefore(document.createElement(html), this.firstChild);
                    this.insertBefore(iframeR.cloneNode(true), this.firstChild);
            });
        }
        return this;
    };

    // Add browser.version if it doesn't exist
    if (!$.browser.version)
        $.browser.version = navigator.userAgent.toLowerCase().match(/.+(?:rv|it|ra|ie)[\/: ]([\d.]+)/)[1];

})(jQuery);

/*
2.6  facebox
* -------------------------------------------------- */
(function ($) {
    $.facebox = function (data, klass) {
        $.facebox.loading()

        if (data.ajax) fillFaceboxFromAjax(data.ajax)
        else if (data.image) fillFaceboxFromImage(data.image)
        else if (data.div) fillFaceboxFromHref(data.div)
        else if ($.isFunction(data)) data.call($)
        else $.facebox.reveal(data, klass)
    }

    /*
    * Public, $.facebox methods
    */

    $.extend($.facebox, {
        settings: {
            opacity: 0.2,
            overlay: true,
            loadingImage: '/Resources/themes/default/images/facebox/loading.gif',
            closeImage: '/Resources/themes/default/images/facebox/close.gif',
            imageTypes: ['png', 'jpg', 'jpeg', 'gif'],
            faceboxHtml: '\
    <div id="facebox" class="selectFree" style="display:none;"> \
      <div class="popup"> \
        <table> \
          <tbody> \
            <tr> \
              <td class="tl"/><td class="b"/><td class="tr"/> \
            </tr> \
            <tr> \
              <td class="b"/> \
              <td class="body"> \
                <div class="content"> \
                </div> \
              </td> \
              <td class="b"/> \
            </tr> \
            <tr> \
              <td class="bl"/><td class="b"/><td class="br"/> \
            </tr> \
          </tbody> \
        </table> \
      </div><!--[if lte IE 6.5]><iframe></iframe><![endif]--> \
    </div>'
        },

        loading: function () {
            init()
            if ($('#facebox .loading').length == 1) return true
            showOverlay()

            $('#facebox .content').empty()
            $('#facebox .body').children().hide().end().
        append('<div class="loading"><img src="' + $.facebox.settings.loadingImage + '"/></div>')

            $('#facebox').css({
                top: getPageScroll()[1] + (getPageHeight() / 10),
                left: 385.5
            }).show()

            $(document).bind('keydown.facebox', function (e) {
                if (e.keyCode == 27) $.facebox.close()
                return true
            })
            $(document).trigger('loading.facebox')
        },

        reveal: function (data, klass) {
            $(document).trigger('beforeReveal.facebox')
            if (klass) $('#facebox .content').addClass(klass)
            $('#facebox .content').append(data)
            $('#facebox .loading').remove()
            $('#facebox .body').children().fadeIn('normal')
            $('#facebox').css('left', $(window).width() / 2 - ($('#facebox table').width() / 2))
            $(document).trigger('reveal.facebox').trigger('afterReveal.facebox')
            $('#facebox .close').click($.facebox.close)
        },

        close: function () {
            $(document).trigger('close.facebox')
            return false
        }
    })

    /*
    * Public, $.fn methods
    */

    $.fn.facebox = function (settings) {
        init(settings)

        function clickHandler() {
            $.facebox.loading(true)

            // support for rel="facebox.inline_popup" syntax, to add a class
            // also supports deprecated "facebox[.inline_popup]" syntax
            var klass = this.rel.match(/facebox\[?\.(\w+)\]?/)
            if (klass) klass = klass[1]

            fillFaceboxFromHref(this.href, klass)
            return false
        }

        return this.click(clickHandler)
    }

    /*
    * Private methods
    */

    // called one time to setup facebox on this page
    function init(settings) {
        if ($.facebox.settings.inited) return true
        else $.facebox.settings.inited = true

        $(document).trigger('init.facebox')
        makeCompatible()

        var imageTypes = $.facebox.settings.imageTypes.join('|')
        $.facebox.settings.imageTypesRegexp = new RegExp('\.' + imageTypes + '$', 'i')

        if (settings) $.extend($.facebox.settings, settings)
        $('body').append($.facebox.settings.faceboxHtml)

        var preload = [new Image(), new Image()]
        preload[0].src = $.facebox.settings.closeImage
        preload[1].src = $.facebox.settings.loadingImage

        $('#facebox').find('.b:first, .bl, .br, .tl, .tr').each(function () {
            preload.push(new Image())
            preload.slice(-1).src = $(this).css('background-image').replace(/url\((.+)\)/, '$1')
        })
    }

    // getPageScroll() by quirksmode.com
    function getPageScroll() {
        var xScroll, yScroll;
        if (self.pageYOffset) {
            yScroll = self.pageYOffset;
            xScroll = self.pageXOffset;
        } else if (document.documentElement && document.documentElement.scrollTop) {	 // Explorer 6 Strict
            yScroll = document.documentElement.scrollTop;
            xScroll = document.documentElement.scrollLeft;
        } else if (document.body) {// all other Explorers
            yScroll = document.body.scrollTop;
            xScroll = document.body.scrollLeft;
        }
        return new Array(xScroll, yScroll)
    }

    // Adapted from getPageSize() by quirksmode.com
    function getPageHeight() {
        var windowHeight
        if (self.innerHeight) {	// all except Explorer
            windowHeight = self.innerHeight;
        } else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
            windowHeight = document.documentElement.clientHeight;
        } else if (document.body) { // other Explorers
            windowHeight = document.body.clientHeight;
        }
        return windowHeight
    }

    // Backwards compatibility
    function makeCompatible() {
        var $s = $.facebox.settings

        $s.loadingImage = $s.loading_image || $s.loadingImage
        $s.closeImage = $s.close_image || $s.closeImage
        $s.imageTypes = $s.image_types || $s.imageTypes
        $s.faceboxHtml = $s.facebox_html || $s.faceboxHtml
    }

    // Figures out what you want to display and displays it
    // formats are:
    //     div: #id
    //   image: blah.extension
    //    ajax: anything else
    function fillFaceboxFromHref(href, klass) {
        // div
        if (href.match(/#/)) {
            var url = window.location.href.split('#')[0]
            var target = href.replace(url, '')
            $.facebox.reveal($(target).clone().show(), klass)

            // image
        } else if (href.match($.facebox.settings.imageTypesRegexp)) {
            fillFaceboxFromImage(href, klass)
            // ajax
        } else {
            fillFaceboxFromAjax(href, klass)
        }
    }

    function fillFaceboxFromImage(href, klass) {
        var image = new Image()
        image.onload = function () {
            $.facebox.reveal('<div class="image"><img src="' + image.src + '" /></div>', klass)
        }
        image.src = href
    }

    function fillFaceboxFromAjax(href, klass) {
        $.get(href, function (data) { $.facebox.reveal(data, klass) })
    }

    function skipOverlay() {
        return $.facebox.settings.overlay == false || $.facebox.settings.opacity === null
    }

    function showOverlay() {
        if (skipOverlay()) return

        if ($('facebox_overlay').length == 0)
            $("body").append('<div id="facebox_overlay" class="facebox_hide"></div>')

        $('#facebox_overlay').hide().addClass("facebox_overlayBG")
      .css('opacity', $.facebox.settings.opacity)
        //.click(function() { $(document).trigger('close.facebox') })
      .fadeIn(200)
        return false
    }

    function hideOverlay() {
        if (skipOverlay()) return

        $('#facebox_overlay').fadeOut(200, function () {
            $("#facebox_overlay").removeClass("facebox_overlayBG")
            $("#facebox_overlay").addClass("facebox_hide")
            $("#facebox_overlay").remove()
        })

        return false
    }

    /*
    * Bindings
    */

    $(document).bind('close.facebox', function () {
        $(document).unbind('keydown.facebox')
        $('#facebox').fadeOut(function () {
            $('#facebox .content').removeClass().addClass('content')
            hideOverlay()
            $('#facebox .loading').remove()
        })
    })

})(jQuery);

/*
* jQuery loadImg
* -------------------------------------------------- */
jQuery.fn.loadthumb = function (src) {
    var _self = this;
    var showImage = function (img, imgSrc) {
        imgDem = {};
        imgDem.w = img.width;
        imgDem.h = img.height;
        imgDem = $.imgResize({ "w": _self.parent().width(), "h": _self.parent().height() }, { "w": imgDem.w, "h": imgDem.h });
        var imgMargins = $.imgCenter({ "w": _self.parent().width(), "h": _self.parent().height() }, { "w": imgDem.w, "h": imgDem.h });
        _self.css({ width: imgDem.w, height: imgDem.h, marginLeft: imgMargins.l, marginTop: imgMargins.t });
        _self.attr("src", imgSrc);
        _self.fadeIn("slow");
        _self.parent().css({ "background": "transparent" });
    }
    _self.hide();
    var img = new Image();
    $(img).error(function () {
        showImage(this, "/Resources/themes/default/images/modal/op_error.gif");
    }).load(function () {
        showImage(this, src);
    }).attr("src", src);
    return _self;
}
jQuery.imgResize = function (parentDem, imgDem) {
    if (imgDem.w > 0 && imgDem.h > 0) {
        var rate = (parentDem.w / imgDem.w < parentDem.h / imgDem.h) ? parentDem.w / imgDem.w : parentDem.h / imgDem.h;
        if (rate <= 1) {
            imgDem.w = imgDem.w * rate;
            imgDem.h = imgDem.h * rate;
        } else {
            imgDem.w = imgDem.w;
            imgDem.h = imgDem.h;
        }
    }
    return imgDem;
}
jQuery.imgCenter = function (parentDem, imgDem) {
    var left = (parentDem.w - imgDem.w) * 0.5;
    var top = (parentDem.h - imgDem.h) * 0.5;
    return { "l": left, "t": top };
}
