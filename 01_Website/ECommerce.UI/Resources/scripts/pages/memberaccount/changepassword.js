$(document).ready(function () {
    $(".menulist.ie6png").hide();
    $(".Validform_checktip").hide();
    $("#showmsg").hide();

    $(".formsub input.intxt").focusin(function () {
        $(this).parent().find(".Validform_checktip").hide();
    }).focusout(function () {
        if ($(this).val() == "") {
            $(this).parent().find(".Validform_checktip").show();
        }
    });
});


usingNamespace("Biz.Customer")["Changepassword"] = {
    Submit: function () {
        $("#showmsg").hide();
        $("#showmsg").find(".Validform_checktip").hide();
        var num = 0;
        if ($("#input_oldpassword").val() == "") {
            num++;
            $("#input_oldpassword").parent().find(".Validform_checktip").html("密码不能为空").show();
        }
        if ($("#input_password").val() == "") {
            num++;
            $("#input_password").parent().find(".Validform_checktip").html("新密码不能为空").show();
        }
        else if ($("#input_password").val().length < 6 || $("#input_password").val().length > 20) {
            num++;
            $("#input_password").parent().find(".Validform_checktip").html("请输入6-20位大小写英文字母、符号或数字").show();
        }
        else if (!($("#input_password").val().match(/([a-zA-Z])/) && $("#input_password").val().match(/([0-9])/))) {
            num++;
            $("#input_password").parent().find(".Validform_checktip").html("请输入6-20位大小写英文字母、符号或数字").show();
        }

        if ($("#input_repassword").val() == "") {
            num++;
            $("#input_repassword").parent().find(".Validform_checktip").html("确认密码不能为空").show();
        }
        if ($("#input_repassword").val() != $("#input_password").val()) {
            num++;
            $("#input_repassword").parent().find(".Validform_checktip").html("确认密码与密码不相同").show();
        }
        if ($("#input_oldpassword").val() == $("#input_password").val()) {
            num++;
            $("#input_password").parent().find(".Validform_checktip").html("旧密码和新密码相同").show();
        }
        if (num == 0) {
            $.ajax({
                type: "post",
                url: "/MemberAccount/AjaxChangePassword",
                dataType: "json",
                async: false,
                timeout: 30000,
                data: { OldPassword: $("#input_oldpassword").val(), Password: $("#input_password").val(), RePassword: $("#input_repassword").val() },
                beforeSend: function (XMLHttpRequest) { },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                success: function (data) {
                    if (data.error) return;
                    if (data != "s") {
                        $("#showmsg").find(".Validform_checktip").html(data).show();
                        $("#showmsg").show();
                    }
                    else {
                        alert("新密码重置成功！");
                    }
                },
                complete: function (XMLHttpRequest, textStatus) { }
            });
        }
    }
}
//处理键盘事件  
function doKey(e) {
    var ev = e || window.event;//获取event对象  
    var obj = ev.target || ev.srcElement;//获取事件源  
    var t = obj.type || obj.getAttribute('type');//获取事件源类型  
    if (ev.keyCode == 8 && t != "password" && t != "text" && t != "textarea") {
        return false;
    }
}
//禁止后退键 作用于Firefox、Opera  
document.onkeypress = doKey;
//禁止后退键  作用于IE、Chrome  
document.onkeydown = doKey;