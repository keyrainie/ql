//分享
var Share = {
    //以链接的方式分享
    Link: function (e) {
        if (!e) {
            return;
        }
        var link = $(e);
        var id = link.attr("id");
        id = id.replace("shareSite_", "");
        var docTitle = document.title;
        var docUrl = document.location.href;
        var docTitle = encodeURIComponent(docTitle);
        var docUrl = encodeURIComponent(docUrl);
        var imgUrl = $(e).attr("ImgUrl");
        var url = "";
        switch (id) {
            case "kaixin001":
                url = "http://www.kaixin001.com/~repaste/repaste.php?&rurl={0}&rtitle={1}&rcontent={1}";
                break;
            case "renren":
                url = "http://share.renren.com/share/buttonshare.do?link={0}&title={1}";
                break;
            case "tsina":
                url = "http://v.t.sina.com.cn/share/share.php?title={1}&url={0}"
                break;
            case "tqq":
                url = "http://v.t.qq.com/share/share.php?url={0}&title={1}&appkey=f50899c2573f45f198d152283055b879";
                break;
            case "douban":
                url = "http://www.douban.com/recommend/?url={0}&title={1}";
                break;
            case "taobao":
                url = "http://share.jianghu.taobao.com/share/addShare.htm?url={0}";
                break;
            case "xianguo":
                url = "http://xianguo.com/service/submitdigg?link={0}&title={1}";
                break;
            case "digu":
                url = "http://www.diguff.com/diguShare/bookMark_FF.jsp?title={1}&url={0}";
                break;
            case "buzz":
                url = "http://www.google.com/buzz/post?url={0}";
                break;
            case "baidu":
                url = "http://cang.baidu.com/do/add?it={1}&iu={0}";
                break;
            case "google":
                url = "http://www.google.com/bookmarks/mark?op=edit&output=popup&bkmk={0}&title={1}";
                break;
            case "youdao":
                url = "http://shuqian.youdao.com/manage?a=popwindow&title={1}&url={0}";
                break;
            case "qq":
                url = "http://sns.qzone.qq.com/cgi-bin/qzshare/cgi_qzshare_onekey?url={0}?FPA=0&title={1}";
                break;
            case "yahoo":
                url = "http://myweb.cn.yahoo.com/popadd.html?url={0}&title={1}";
                break;
        }

        url = $.format(url, docUrl, docTitle);
        if (id === "tsina") {
            if (imgUrl) {
                url += "&pic=" + encodeURIComponent(imgUrl);
            }
        }
        link.attr("href", url);
        link.attr("target", "_blank");
    }
}