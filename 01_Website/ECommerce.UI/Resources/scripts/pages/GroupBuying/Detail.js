var GroupBuyingDetail = {
    //计算剩余时间
    CalcTime: function () {
        var totalSeconds = parseInt($("#TotalSeconds").val());
        if (totalSeconds <= 0) {
            var $GroupBuyingRunningPanel = $("#GroupBuyingRunningPanel"),
                count = $GroupBuyingRunningPanel.attr("count"),
                html = "<ul class=\"cls\">\
                                <li>\
                                    <div class=\"probtn\">\
                                        <div class=\"textinfo textinfo_failed fr\">\
                                            <s class=\"icon_gpresult\"></s>\
                                            <h3>本次团购已结束</h3>\
                                            <h4>共有<strong class=\"red num\">" + count + "</strong>人购买，下次抓紧时间哦！</h4>\
                                        </div>\
                                        <a href=\"javascript:void(0);\" class=\"inblock btn_groupend mr10 fl\">团购结束</a>\
                                    </div>\
                                </li>\
                            </ul>";
            $GroupBuyingRunningPanel.html(html);
            $(".second").text(GroupBuyingDetail.FormatTime(0));
            return;
        }

        var calcTotalSeconds = totalSeconds;
        var seconds = calcTotalSeconds % 60;
        calcTotalSeconds -= seconds;
        calcTotalSeconds /= 60;
        var minute = calcTotalSeconds % 60;
        calcTotalSeconds -= minute;
        calcTotalSeconds /= 60;
        var hour = calcTotalSeconds % 24;
        calcTotalSeconds -= hour;
        calcTotalSeconds /= 24;
        var day = calcTotalSeconds;

        $(".day").text(GroupBuyingDetail.FormatTime(parseInt(day)));
        $(".hour").text(GroupBuyingDetail.FormatTime(parseInt(hour)));
        $(".minute").text(GroupBuyingDetail.FormatTime(parseInt(minute)));
        $(".second").text(GroupBuyingDetail.FormatTime(parseInt(seconds)));
        ////设置三位数天的样式
        //if (parseInt(day) > 99) {
        //    $(".timelimit_group").addClass("timelimit_group_3day");
        //}
        if (totalSeconds > 0)
            $("#TotalSeconds").val(totalSeconds - 1);
        else
            $("#TotalSeconds").val("0");
        setTimeout(function () {
            GroupBuyingDetail.CalcTime();
        }, 1000);
    },
    //格式化时间
    FormatTime: function (n) {
        if (parseInt(n) < 10) {
            return "0" + n;
        }
        else {
            return n;
        }
    },
    //更改购买数量
    ChangeQty: function (e) {
        var currParentDom = $(e).parent();
        var aciton = $(e).attr("Action");
        var oldValue = currParentDom.find("input[type=text]").attr("OldValue");
        var maxPerOrder = parseInt(currParentDom.find("input").attr("MaxPerOrder"));
        var currQty = currParentDom.find("input[type=text]").val();
        var pattern = /^\d+$/;
        if (!pattern.test(currQty)) {
            currParentDom.find("input[type=text]").val(oldValue);
            $("#ChangQtyHintMsg").text("只能输入正整数");
            $("#ChangQtyHintMsg").show();
            setTimeout(function () { $("#ChangQtyHintMsg").hide(); }, 2000);
            return;
        }
        currQty = parseInt(currQty);
        switch (aciton)
        {
            case "Reduse":
                currQty--;
                break;
            case "Add":
                currQty++;
                break;
        }
        if (currQty < 1)
            currQty = 1;
        if (currQty > maxPerOrder) {
            currParentDom.find("input[type=text]").val(oldValue);
            $("#ChangQtyHintMsg").show();
            setTimeout(function () { $("#ChangQtyHintMsg").hide(); }, 2000);
            return;
        }
        currParentDom.find("input[type=text]").val(currQty);
        currParentDom.find("input[type=text]").attr("OldValue", currQty);
    },
    //切换详情描述
    ChangeDetail: function (e) {
        $(e).parent().find("a").removeClass("now");
        $(e).addClass("now");
        $(".tabc").hide();
        $(".tabc").eq($(e).attr("rel")).show();
    },
    //购买
    Buy: function () {
        var qty = $("#txtBuyCount").val();
        var url = $("#BtnBuy").attr("BuyUrl");
        url = url.replace("#qty#", qty);
        location.href = url;
    }
}

$(function () {
    GroupBuyingDetail.CalcTime();
});