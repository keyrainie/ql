//Thankyou
var Thankyou = {
    //支付
    Pay: function (e) {
        var soSysNo = $(e).attr("SOSysNo");

        var payhelpbox = PopWin("#payFinishedMsg");
        payhelpbox.fn.popIn();

        var PayUrl = $(e).attr("PayUrl");

        window.open(PayUrl);
    },

    ChangePayType: function (e, payTypeID) {
        var soSysNo         = $(e).attr("SOSysNo");
        var payTypeSysNo    = $(e).attr("PaySysNo")
        var ChangPayUrl     = $(e).attr("ChangPayUrl");
        var PayTypeCategory = $(e).attr("PayTypeCategory");

        //if (payTypeID != 201) {//如果有使用网银积分则选择支付方式为非泰隆银行支付则跳出提示
        //    var changePayMethodMsg = PopWin("#changePayMethodMsg");
        //    changePayMethodMsg.fn.popIn();
        //}

        if (PayTypeCategory == "3") {
            $(".other_pay_btn").hide();
        }
        else {
            $(".other_pay_btn").show();
        }

        $.ajax({
            type: "post",
            dataType: 'JSON',
            url: ChangPayUrl,
            cache: false,
            data: { so: soSysNo, pay: payTypeSysNo },
            beforeSend: function (XMLHttpRequest) {
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data, textStatus, jqXHR) {
                if (data.error) {
                    return;
                }
                $(".action").show();
            },
            complete: function (XMLHttpRequest, textStatus) {
                if (PayTypeCategory == "3") {
                    $(".other_pay_btn").hide();
                }
                else {
                    $(".other_pay_btn").show();
                }
            }
        });
    },
    //虚拟团购支付
    PayVirualGroupBuy: function (e) {
        var payhelpbox = PopWin("#payFinishedMsg");
        payhelpbox.fn.popIn();

        var payUrl = $(e).attr("PayUrl");
        window.open(payUrl);
    }
}