<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendMail.aspx.cs" ValidateRequest="false"
    Inherits="Newegg.Oversea.Silverlight.ControlPanel.WebHost.SendMail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%= this.Title %></title>
    <link href="../WebResources/style/sendmail-1.0.css?v=1.0.0.1" type="text/css" rel="Stylesheet" />
    <link href="../WebResources/jquery.alerts-1.1/jquery.alerts.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../WebResources/javascript/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" src="../WebResources/javascript/jquery.ui.draggable.js"></script>
    <script type="text/javascript" src="../WebResources/kindeditor-3.5.5/<%=this.LanguageCode %>/kindeditor-min.js"></script>
    <script type="text/javascript" src="../WebResources/javascript/AttachmentComponent-1.0.js?v=1.0.0.1"></script>
    <script type="text/javascript" src="../WebResources/jquery.alerts-1.1/jquery.alerts.js"></script>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <div>
        <div style="width: 100%;">
            <div id="templateSelector" style="margin-top: 5px; text-align: right">
                <asp:Label meta:resourcekey="LbTemplate" runat="server" ID="LbTemplate"></asp:Label>
                <asp:DropDownList ID="ddlTemplates" DataTextField="TemplateText" AutoPostBack="true"
                    DataValueField="TemplateID" Width="400px" runat="server" OnSelectedIndexChanged="ddlTemplates_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div style="float: left; width: 60px; padding-top: 5px;">
                <asp:Button meta:resourcekey="BtnSend" OnClientClick="return validate();" ID="BtnSend"
                    Height="75px" Width="60px" runat="server" OnClick="BtnSend_Click" />
            </div>
            <div style="padding: 0 0 10px 70px; margin: 0;">
                <table width="100%">
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="left" style="width: 60px;">
                                        <asp:Label meta:resourcekey="LbFrom" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtFrom" MaxLength="500" CssClass="general-text" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="left" style="width: 60px;">
                                        <asp:Label meta:resourcekey="LbTo" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTo" TextMode="MultiLine" Height="35" MaxLength="500" CssClass="general-text"
                                            runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="row_CC">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="left" style="width: 60px;">
                                        <asp:Label meta:resourcekey="LbCC" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCC" MaxLength="500" CssClass="general-text" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="row_BCC">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="left" style="width: 60px;">
                                        <asp:Label meta:resourcekey="LbBCC" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtBCC" MaxLength="500" CssClass="general-text" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="left" style="width: 60px;">
                                        <asp:Label meta:resourcekey="LbSubject" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtSubject" MaxLength="200" CssClass="general-text" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <asp:TextBox ID="txtBody" CssClass="general-textarea" TextMode="MultiLine" Wrap="true"
                    Height="390px" Width="100%" runat="server" />
            </div>
            <div id="attachmentContainer" style="margin-top: 5px; text-align: left">
                <div style="width: 100%; border-bottom: 1px dashed #ccc; height: 20px; line-height: 20px;">
                    [<a href="javascript:void(0)" onclick="addAttachment()"><%= this.GetLocalResourceObject("LbAddAttachment.Text")%></a>]
                </div>
                <ul id="attachmentList">
                </ul>
            </div>
        </div>
    </div>
    <div class="MaskSpin">
    </div>
    <div class="LoadingSpin">
        <%= this.GetLocalResourceObject("LbSending")%>
    </div>
    <script type="text/javascript">
        var component;
        var previous_win_height;
        var previous_editor_height;
        var is_init_editor;
        var _isSubmit = false;
        var pattern = /^([.a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;

        $(function () {
            component = new AttachmentComponent("attachmentList", '<%= this.MailConfiguration.MaxAttachmentCount %>');
            component.Init('<%= this.LanguageCode %>');

            previous_win_height = $(window).height();
        });

        window.onbeforeunload = function () {
            if (!_isSubmit) {
                $.ajaxSettings.contentType = "application/json";
                $.post("../Service/Framework/V50/MailRestService.svc/CloseMailPage", '{ "messageID": "<%=this.MessageID %>" }', null, "json");
            }
        };


        $(document).ready(function () {
            document.forms[0].onsubmit = function () { _isSubmit = true; };
        });

        $(window).resize(function () {
            var height = $(window).height();
            previous_editor_height = previous_editor_height + (height - previous_win_height);

            if (is_init_editor) {
                KE.util.resize('txtBody', '', previous_editor_height + "px", true, false);
            }
            else {
                $("#<%=this.txtBody.ClientID %>").css("height", previous_editor_height + "px");
            }
            previous_win_height = height;
        });

        function addAttachment() {
            component.AddAttachment();
        }

        function init_editor(isInit, readOnly, height) {
            if ($(document).height() != 600) {
                height = height + ($(window).height() - 600);
            }
            previous_editor_height = height;
            this.is_init_editor = isInit;

            if (isInit == true) {
                var plugins = ['source', '|', 'fullscreen', 'fontname', 'fontsize', '|', 'textcolor', 'bgcolor', 'bold', 'italic', 'underline',
				'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
				'insertunorderedlist', '|', 'image', 'link'];

                if (readOnly == false) {
                    KE.show({
                        id: 'txtBody',
                        resizeMode: 0,
                        allowUpload: false,
                        items: plugins,
                        height: height + "px",
                        width: "100%"
                    });
                }
                else {
                    KE.show({
                        id: 'txtBody',
                        resizeMode: 0,
                        allowUpload: false,
                        items: plugins,
                        height: height + "px",
                        width: "100%",
                        afterCreate: function (id) {
                            KE.toolbar.disable(id, []);
                            KE.readonly(id);
                            KE.g[id].newTextarea.disabled = true;
                        }
                    });
                }
            }
            else {
                $("#<%=this.txtBody.ClientID %>").css("height", height + "px");
            }
        }
        function validate() {
            var from = $("#<%=this.txtFrom.ClientID %>").val();
            var to = $("#<%=this.txtTo.ClientID %>").val();
            var cc = $("#<%=this.txtCC.ClientID %>").val();
            var bcc = $("#<%=this.txtBCC.ClientID %>").val();
            var subject = $("#<%=this.txtSubject.ClientID %>").val();

            var msg = "";
            var b = true;

            if (from == "") {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Required_From").ToString() %></br>';
                b = false;
            }
            if (to == "") {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Required_To").ToString() %></br>';
                b = false;
            }

            if (validateAddress(from) == false) {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Invalid_From").ToString() %></br>';
                b = false;
            }

            if (validateAddress(to) == false) {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Invalid_To").ToString() %></br>';
                b = false;
            }

            if (validateAddress(cc) == false) {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Invalid_CC").ToString() %></br>';
                b = false;
            }

            if (validateAddress(bcc) == false) {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Invalid_BCC").ToString() %></br>';
                b = false;
            }


            if (subject == "") {
                msg += '<%=this.GetGlobalResourceObject("InfoMessage", "Validation_Required_Subject").ToString() %></br>';
                b = false;
            }

            if (b) {
                this.showLoading();
            }
            else {
                jAlert(msg, "Warning", function () {
                    updateEmailAddress($("#<%=this.txtFrom.ClientID %>"));
                    updateEmailAddress($("#<%=this.txtTo.ClientID %>"));
                    updateEmailAddress($("#<%=this.txtCC.ClientID %>"));
                    updateEmailAddress($("#<%=this.txtBCC.ClientID %>"));
                });
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

        function showLoading() {
            var height = $(document).height();
            var width = $(document).width();

            $(".MaskSpin").css({ width: width, height: height, display: "block" });

            $(".LoadingSpin").css("left", (($(document).width()) / 2 - (parseInt($(".LoadingSpin").width()) / 2)) + "px");
            $(".LoadingSpin").css("top", (($(document).height()) / 2 - (parseInt($(".LoadingSpin").height()) / 2)) + "px");
            $(".LoadingSpin").css("display", "block");
        }
    </script>
    </form>
</body>
</html>
