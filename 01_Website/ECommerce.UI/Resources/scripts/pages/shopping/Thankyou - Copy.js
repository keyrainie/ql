//Thankyou
var Thankyou = {
    //支付
    Pay: function (e) {        
        var soSysNo = $(e).attr("SOSysNo");

        var payhelpbox = PopWin(".payhelpbox");
        payhelpbox.fn.popIn();

        var PayUrl = $(e).attr("PayUrl");

        window.open(PayUrl);
    },

    ChangePayType: function (e) {

        var soSysNo = $(e).attr("SOSysNo");
        var payTypeSysNo = $(e).attr("PaySysNo")
        var ChangPayUrl = $(e).attr("ChangPayUrl");
        
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
            },
            complete: function (XMLHttpRequest, textStatus) {
            }
        });

    },
    //虚拟团购支付
    PayVirualGroupBuy: function (e) {
        var payhelpbox = PopWin(".payhelpbox");
        payhelpbox.fn.popIn();

        var payUrl = $(e).attr("PayUrl");
        window.open(payUrl);
    }
}