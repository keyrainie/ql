<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="ECCentral.Portal.WebHost.ueditor.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8">
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
    <title>内容编辑</title>
    <script type="text/javascript" src="editor_config.js"></script>
    <script type="text/javascript" src="editor.js"></script>
    <link rel="stylesheet" href="themes/default/ueditor.css" />
    <script type="text/javascript">
	<!--
        window.UEDITOR_HOME_URL = location.pathname.substr(0, location.pathname.lastIndexOf('/')) + '/';
        //-->
        var editor;
        function ShowEditor(content, width, isedit) {
            editor.setContent(content);
            document.getElementById("btnSave").style.display = "block";
            if (!isedit) {
                editor.setDisabled("fullscreen");
                //                document.getElementById("btnSave").style.display = "none";
            }
        }
        function Preview(content) {
            document.getElementById("PreviewDiv").innerHTML = content;
        }
        function SaveContent() {
            opener.SaveRichText(editor.getContent());
            window.close();
        }
        function UploadCallBack(path) {
            //            alert(path);
        }
    </script>
    <style type="text/css">
        .details
        {
            margin-top: 5px;
            overflow: hidden;
            margin: 0 auto;
            padding-bottom: 20px;
        }
        .w900
        {
            margin: 0 auto;
            width: 900px;
        }
        #btnSave
        {
            margin-left: 810px;
        }
        #edui1
        {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="w900">
        <div class="details">
            <script type="text/plain" id="editor" style="width: 860px;"></script>
        </div>
        <div class="con-op">
            <input id="btnSave" onclick="SaveContent()" type="button" value="保存" />
        </div>
    </div>
    <script type="text/javascript">
        editor = new UE.ui.Editor();
        editor.render('editor');
    </script>
    </form>
</body>
</html>
