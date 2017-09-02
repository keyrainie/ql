<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchMail.aspx.cs" Inherits="Newegg.Oversea.Silverlight.ControlPanel.WebHost.Pages.BatchMail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../WebResources/style/jquery-ui/jquery-ui-1.8.16.custom.css"
        rel="Stylesheet" />
    <link type="text/css" href="../WebResources/style/jquery-loading/showLoading.css"
        rel="Stylesheet" />
    <link type="text/css" href="../WebResources/style/batchmail-1.0.css" rel="Stylesheet" />
    <link href="../WebResources/jquery.alerts-1.1/jquery.alerts.css" rel="stylesheet"
        type="text/css" />
</head>
<body>
    <div id="container">
        <div>
            <table class="layout-grid" cellpadding="0" cellspacing="0">
                <tr>
                    <td rowspan="5" valign="top" align="center" class="left-button">
                        <input type="button" style="height: 90px; width: 80px;" id="btnSend" />
                    </td>
                </tr>
                <tr>
                    <td class="right-input">
                        <table width="100%">
                            <tr>
                                <td style="width: 80px;">
                                    <span class="table-text" id="text-from"></span>
                                </td>
                                <td>
                                    <input type="text" id="input-from" class="text" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="right-input">
                        <table width="100%">
                            <tr>
                                <td style="width: 80px;">
                                    <span class="table-text" id="text-to"></span>
                                </td>
                                <td>
                                    <textarea rows="2" id="input-to" class="text"></textarea>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="row-cc">
                    <td class="right-input">
                        <table width="100%">
                            <tr>
                                <td style="width: 80px;">
                                    <span class="table-text" id="text-cc"></span>
                                </td>
                                <td>
                                    <textarea rows="2" id="input-cc" class="text"></textarea>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="row-bcc">
                    <td class="right-input">
                        <table width="100%">
                            <tr>
                                <td style="width: 80px;">
                                    <span class="table-text" id="text-bcc"></span>
                                </td>
                                <td>
                                    <textarea rows="2" id="input-bcc" class="text"></textarea>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="tabs">
            <ul id="headers">
            </ul>
        </div>
    </div>
    <script type="text/javascript" src="../WebResources/javascript/jquery-1.6.2.min.js"></script>
    <script type="text/javascript" src="../WebResources/javascript/jquery-ui-1.8.16.custom.min.js"></script>
    <script type="text/javascript" src="../WebResources/javascript/jquery.showLoading.js"></script>
    <script type="text/javascript" src="../WebResources/javascript/jquery.ui.draggable.js"></script>
    <script type="text/javascript" src="../WebResources/jquery.alerts-1.1/jquery.alerts.js"></script>
    <script type="text/javascript" src="../WebResources/javascript/jquery.upload-1.0.2.min.js"></script>
    <script type="text/javascript" src="../WebResources/kindeditor-4.0.4/kindeditor-min.js"></script>
    <script type="text/javascript" src="../WebResources/javascript/json2.js"></script>
    <script type="text/javascript">
        var editors = {};
        var isLoadScript = false;
        var addtionHeight = 0;
        var msgs = [];
        var plugins = ['source', '|', 'fullscreen', 'fontname', 'fontsize', '|', 'textcolor', 'bgcolor', 'bold', 'italic', 'underline',
				'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
				'insertunorderedlist', '|', 'link'];
        var pattern = /^([.a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]+){1,2})$/;

        var btnAtt = '<%=  this.GetLocalResourceObject("LbAddAttachment") %>';
        var btnDelete = '<%=  this.GetLocalResourceObject("LbDelete") %>';

        $(function () {
            //load data from service.
            preload();

            init();
        });

        window.onbeforeunload = function () {
            var messageIDs = '<%= Request.QueryString["MessageID"] %>'.split(',');
            if (messageIDs.length > 0) {
                $.ajaxSettings.contentType = "application/json";
                $.post("../Service/Framework/V50/MailRestService.svc/CloseMailPage", '{ "messageID": "' + messageIDs[0] + '" }', null, "json");
            }
        };


        function preload() {
            //set the loading spin for ajax request.
            $("#container").bind("ajaxSend", function () {
                $("body").showLoading();
            }).bind("ajaxComplete", function () {
                $("body").hideLoading();
            });
            //load mail messages from service.
            var messages = '<%= Request.QueryString["MessageID"] %>';
            $.get("../Service/Framework/V50/MailRestService.svc/GetMailMessage/" + messages, function (data) {
                if (data.length > 0) {
                    var message = data[0].MailMessage;

                    handle(message, data[0].MailPageSetting);
                }

                for (var i = 0; i < data.length; i++) {
                    msgs.push(data[i].MailMessage);
                    addToTab(data[i].MailMessage, data[i].MailPageSetting);
                }

                $("#tabs").tabs();


            });
        }


        function addToTab(message, setting) {
            if (message != undefined) {
                $("#headers").append("<li><a href='#tab-" + message.MessageID + "'>" + message.Subject + "</a></li>");
                $("#tabs").append("<div id='tab-" + message.MessageID + "'>" +
				"<table width='100%'>" +
					"<tr>" +
						"<td>" +
							'<div style="height:25px; line-height:25px;"><%=  this.GetLocalResourceObject("LbSubject.Text") %></div>' +
							"<input type='text' class='text' id='input-subject-" + message.MessageID + "' value='" + message.Subject + "' style='width:100%' />" +
						"</td>" +
					"</tr>" +
					"<tr>" +
						"<td>" +
							'<div style="height:25px; line-height:25px;"><%=  this.GetLocalResourceObject("LbBody.Text") %></div>' +
							"<textarea id='input-body-" + message.MessageID + "' style='width:100%'>" + message.Body + "</textarea>" +
						"</td>" +
					"</tr>" +
					"<tr>" +
						"<td>" +
							 "<div>" +
								"<form action='#' method='post'>" +
									"<div id='uploader-" + message.MessageID + "'>" +
										"<input type='hidden' id='flag-" + message.MessageID + "' value='0' />" +
										"<div>" +
											"<input id='add-button-" + message.MessageID + "' value='" + btnAtt + "' type='button' />" +
										"</div>" +
										"<input type='hidden' name='subdomain' value='" + message.MessageID + "' />" +
									"</div>" +
								"</form>" +
							"</div>" +
						"</td>" +
					"</tr>" +

				"</table>" +
			"</div>");

                if (message.BodyType == 0) {
                    var readyonly = false;

                    if (setting != undefined) {

                        if (setting.IsAllowEdit == false || setting.IsAllowChangeMailBody == false) {
                            readyonly = true;
                        }
                    }

                    editors[message.MessageID] = KindEditor.create("#input-body-" + message.MessageID + "", {
                        items: plugins,
                        minHeight: 510 - addtionHeight,
                        wellFormatMode: false,
                        resizeType: 0,
                        langType: '<%= this.LanguageCode %>',
                        readonlyMode: readyonly
                    });
                }
                else {
                    $("#input-body-" + message.MessageID).css("height", 510 - addtionHeight);
                }

                if (setting != undefined) {
                    if (setting.IsAllowEdit == false) {
                        $("#input-subject-" + message.MessageID).attr("disabled", "disabled");

                        if (message.BodyType != 0) {
                            $("#input-body-" + message.MessageID).attr("disabled", "disabled");
                        }
                    }
                    else {
                        if (setting.IsAllowChangeMailSubject == false) {
                            $("#input-subject-" + message.MessageID).attr("disabled", "disabled");
                        }

                        if (message.BodyType != 0 && setting.IsAllowChangeMailBody == false) {
                            $("#input-body-" + message.MessageID).attr("disabled", "disabled");
                        }
                    }
                }
                if (setting.IsAllowAttachment == true) {
                    $("#add-button-" + message.MessageID).button();

                    $("#add-button-" + message.MessageID).click(function () {
                        var ram = Math.floor(Math.random() * 1000);
                        $("#uploader-" + message.MessageID).append("<div id='dv-" + ram + "' style='margin:5px,0'>" +
																		"<input type='file' name='file' />" +
																		"<input type='button' id='btn-del-" + ram + "' value='" + btnDelete + "' />" +
																	"</div>");

                        $("#flag-" + message.MessageID).val("1");
                        $("#btn-del-" + ram).button();
                        $("#btn-del-" + ram).click(function () {
                            $("#dv-" + ram).remove();
                        });
                    });
                }
                else {
                    $("#uploader-" + message.MessageID).hide();
                }
            }
        }

        function handle(message, setting) {
            if (message != undefined) {
                $("#input-from").val(message.From);
                $("#input-to").val(message.To);
                if (setting != undefined) {
                    if (setting.IsAllowCC == false) {
                        $(".row-cc").hide();
                    }
                    else {
                        $("#input-cc").val(message.CC);
                        addtionHeight += 55;
                    }

                    if (setting.IsAllowBCC == false) {
                        $(".row-bcc").hide();
                    }
                    else {
                        $("#input-bcc").val(message.BCC);
                        addtionHeight += 55;
                    }

                    if (setting.IsAllowEdit == false) {
                        $("#input-from,#input-to,#input-cc").attr("disabled", "disabled");
                    }
                    else {
                        if (setting.IsAllowChangeMailFrom == false) {
                            $("#input-from").attr("disabled", "disabled");
                        }
                        if (setting.IsAllowChangeMailTo == false) {
                            $("#input-to").attr("disabled", "disabled");
                        }
                    }
                }
            }
        }

        function onSend() {
            if (validate() == true) {
                var messages = [];
                //上传附件。
                uploadAttach(function () {
                    for (var i = 0; i < msgs.length; i++) {
                        var msg = msgs[i];

                        msg.From = $("#input-from").val();
                        msg.To = $("#input-to").val();
                        msg.CC = $("#input-cc").val();
                        msg.BCC = $("#input-bcc").val();
                        msg.Subject = $("#input-subject-" + msg.MessageID).val();

                        //同步编辑器数据
                        if (editors[msg.MessageID] != undefined) {
                            editors[msg.MessageID].sync();
                        }

                        msg.Body = $("#input-body-" + msg.MessageID).val();

                        messages.push(msg);
                    }

                    $.ajaxSettings.contentType = "application/json";
                    $.post("../Service/Framework/V50/MailRestService.svc/Send", '{ "msgs": ' + JSON.stringify(messages) + ' }', function (data) {
                        if (data != undefined && data.SendResult != undefined) {
                            if (data.SendResult.IsSuccess == true) {
                                jAlert('<%=  this.GetLocalResourceObject("Info_SendSuccessfully")   %>', '<%= this.GetLocalResourceObject("Info_Alert_Title")   %>', function () {
                                    closeWindow();
                                });
                            }
                            else { 
                                jAlert('' + data.SendResult.Description + '', '<%= this.GetLocalResourceObject("Info_Alert_Title")   %>', null);
                            }
                        }
                    });
                });
            }
        }

        function uploadAttach(callback) {
            var count = msgs.length;
            $("body").showLoading();
            for (var i = 0; i < msgs.length; i++) {
                var message = msgs[i];
                var flag = $("#flag-" + message.MessageID).val();
                if (flag != 0) {
                    $("#uploader-" + message.MessageID).upload("../HttpHandler/FileHandler.ashx", function (res) {
                        if (res == "Y") {
                            count--;
                            if (count == 0) {
                                $("body").hideLoading();
                                callback();
                            }
                        }
                        else {
                            $("body").hideLoading();
                            jAlert('<%=  this.GetLocalResourceObject("Info_UploadFailed")%>', '<%= this.GetLocalResourceObject("Info_Alert_Title")   %>');
                        }
                    }, 'text');
                }
                else {
                    count--;
                    if (count == 0) {
                        $("body").hideLoading();

                        callback();
                    }
                }
            }


        }

        function init() {
            $("#btnSend").button();
            $("#btnSend").val('<%=  this.GetLocalResourceObject("BtnSend.Text")   %>');
            $("#btnSend").bind("click", onSend);

            $("#text-from").text('<%=  this.GetLocalResourceObject("LbFrom.Text")   %>');
            $("#text-to").text('<%=  this.GetLocalResourceObject("LbTo.Text")   %>');
            $("#text-cc").text('<%=  this.GetLocalResourceObject("LbCC.Text")   %>');
            $("#text-bcc").text('<%=  this.GetLocalResourceObject("LbBCC.Text")   %>');
        }

        function validate() {
            var from = $("#input-from");
            var to = $("#input-to");
            var cc = $("#input-cc");
            var bcc = $("#input-bcc");

            var msg = "";
            var b = true;

            if (from.val() == "") {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Required_From").ToString() %></br>';
                b = false;
            }
            if (to.val() == "") {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Required_To").ToString() %></br>';
                b = false;
            }

            if (validateAddress(from.val()) == false) {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Invalid_From").ToString() %></br>';
                b = false;
            }

            if (validateAddress(to.val()) == false) {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Invalid_To").ToString() %></br>';
                b = false;
            }

            if (validateAddress(cc.val()) == false) {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Invalid_CC").ToString() %></br>';
                b = false;
            }

            if (validateAddress(bcc.val()) == false) {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Invalid_BCC").ToString() %></br>';
                b = false;
            }

            for (var i = 0; i < msgs.length; i++) {
                if ($("#input-subject-" + msgs[i].MessageID).val() == "") {
                    msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Required_Subject").ToString() %></br>';
                    b = false;
                    break;
                }
            }

            updateEmailAddress(from);
            updateEmailAddress(to);
            updateEmailAddress(cc);
            updateEmailAddress(bcc);

            if (!b) {
                jAlert(msg, "Warning");
            }
            return b;
        }

        function validateAddress(value) {
            if (value != undefined) {
                var emails = value.split(";");
                var b = true;
                for (var i = 0; i < emails.length; i++) {
                    var mail = $.trim(emails[i]);

                    if (mail == "") continue;

                    if (pattern.test(mail) == false) {
                        b = false;
                        break;
                    }
                }
                return b;
            }
            else {
                return true;
            }
        }

        function updateEmailAddress(obj) {
            if (obj.val() != undefined) {
                var emails = obj.val().split(";");
                var result = "";

                for (var i = 0; i < emails.length; i++) {
                    var mail = $.trim(emails[i]);

                    if (mail == "") continue;

                    if (pattern.test(mail) == true) {
                        result += mail + ";";
                    }
                }
                obj.val(result);
            }
        }

        function closeWindow() {
            if ($.browser.msie) {
                var ie7 = (document.all && !window.opera && window.XMLHttpRequest) ? true : false;
                if (ie7) {
                    //This method is required to close a window without any prompt for IE7
                    window.open('', '_parent', '');
                    window.close();
                }
                else {

                    //This method is required to close a window without any prompt for IE6
                    this.focus();
                    self.opener = this;
                    self.close();
                }
            }
            else if ($.browser.mozilla) {
                window.open('', '_parent', '');
                window.close();
            }
            else {
                window.open('', '_self', '');
                window.close();
            }
        }

    </script>
</body>
</html>
