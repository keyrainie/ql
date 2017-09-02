<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Newegg.Oversea.Silverlight.ControlPanel.WebHost._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title><%= Name %></title>
    <link href="WebResources/style/base-1.0.css?version=1.0.0.1" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="WebResources/javascript/Silverlight.js"></script>
    <script type="text/javascript" src="WebResources/javascript/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" src="WebResources/javascript/common-1.0.js" ></script>
    <script type="text/javascript" src="WebResources/javascript/jquery-hashchange-plugin-1.3.js" ></script>
    <script type="text/javascript">
        function PreviewPageShow(Content) {
            $('#Content').val(Content);
            form1.submit();
        }

        var silverlightHost;

        function reloadPage() {
            window.location.reload();
        }

        function onSilverlightLoaded(sender, e) {
            silverlightHost = sender.getHost();
            silverlightHost.focus();

            $(window).hashchange(hashHandle);
        }  
              
        var hashHandle = function () {
            if (window.location.href.replace(/^[^#]*#?(.*)$/, '$1') != '' && silverlightHost) {
                var pageBrowser = silverlightHost.content.PageBrowser;
                if (pageBrowser != null && pageBrowser != undefined) {
                    pageBrowser.NavigateFromScript(window.location.href);
                }
            }
        };
    </script>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%; width:100%" target="_blank" action="HtmlViewHandler.ashx">
    <div id="silverlightControlHost">
        <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
            id="silverlightControlHostObject" width="100%" height="100%">
            <param name="source" value="ClientBin/ControlPanel.SilverlightUI.xap?V5" />
            <param name="onError" value="onSilverlightError" />
            <param name="onLoad" value="onSilverlightLoaded" />
            <param name="background" value="white" />
            <param name="minRuntimeVersion" value="5.0.61118.0" />
            <param name="autoUpgrade" value="true" />
            <param name="initParams" value="defatulCulture=zh-CN,login=http://eccsvc.tlyh.com.gqc" />
            <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		  </a>
        </object><iframe id="_sl_historyFrame"  style="visibility: hidden; height: 0px; width: 0px; border: 0px;display:none;"></iframe>
    </div>
        <input type="hidden" name="Content" id="Content" value="" />
    </form>
</body>
</html>
