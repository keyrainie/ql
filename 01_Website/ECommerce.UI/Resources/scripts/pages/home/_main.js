/*******************母版页上需要的少量函数******************/
function setSubscription() {
    var email = $("#txtSubscription").val();
    if(!Biz.Common.Validation.isEmail(email))
    {
        alert("请输入正确的Email格式！");
        return;
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        url: "/Topic/SubscriptEmail?email=" + email,
        cache: false,

        beforeSend: function () { },
        error: function () { alert("订阅失败！"); },
        success: function (data) {
            if (data) {
                if (data.error) {
                    alert(data.message);
                }
                else {
                    alert("订阅成功！");
                    $("#txtSubscription").val("");
                }
            }
        },
        complete: function () { ; }
    });
}