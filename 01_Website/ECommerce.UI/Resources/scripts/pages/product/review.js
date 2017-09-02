
//一分钟检查
var preTimePointForVote = 0;
function checkOneMiniteBeforePublishForVote(postTimeLimit) {
    var nowTime = new Date();
    var nowMinitePoint = nowTime.getHours() * 3600 + nowTime.getMinutes() * 60 + nowTime.getSeconds();
    if (nowMinitePoint - preTimePointForVote < postTimeLimit) {
        return false;
    }
    return true;

}
//重置时间
function resetPublishTimePointForVote() {
    var nowTime = new Date();
    var nowMinitePoint = nowTime.getHours() * 3600 + nowTime.getMinutes() * 60 + nowTime.getSeconds();
    preTimePointForVote = nowMinitePoint;
}
var reviewQuery = {

    init: function () {

        $("#subTab_0").click(function () {
            $("#productReviewTab0").attr("style", "display:block");
            $("#productReviewTab1").attr("style", "display:none");
            $("#productReviewTab2").attr("style", "display:none");
            $("#productReviewTab3").attr("style", "display:none");
            $("#productReviewTab4").attr("style", "display:none");
            $("#productReviewTab5").attr("style", "display:none");

            $("#subTab_" + 0).addClass("now");
            $("#subTab_" + 1).removeClass("now");
            $("#subTab_" + 2).removeClass("now");
            $("#subTab_" + 3).removeClass("now");
            $("#subTab_" + 4).removeClass("now");
            $("#subTab_" + 5).removeClass("now");


            $("#SearchType").val('');
            $("#indexTab").val("productReviewTab0");
        });

        $("#subTab_1").click(function () {
            $("#productReviewTab0").attr("style", "display:none");
            $("#productReviewTab1").attr("style", "display:block");
            $("#productReviewTab2").attr("style", "display:none");
            $("#productReviewTab3").attr("style", "display:none");
            $("#productReviewTab4").attr("style", "display:none");
            $("#productReviewTab5").attr("style", "display:none");


            $("#subTab_" + 0).removeClass("now");
            $("#subTab_" + 1).addClass("now");
            $("#subTab_" + 2).removeClass("now");
            $("#subTab_" + 3).removeClass("now");

            $("#indexTab").val("productReviewTab1");
            $("#SearchType").val('11+12');
        });


        $("#subTab_2").click(function () {
            $("#productReviewTab0").attr("style", "display:none");
            $("#productReviewTab1").attr("style", "display:none");
            $("#productReviewTab2").attr("style", "display:block");
            $("#productReviewTab3").attr("style", "display:none");
            $("#productReviewTab4").attr("style", "display:none");
            $("#productReviewTab5").attr("style", "display:none");


            $("#subTab_" + 0).removeClass("now");
            $("#subTab_" + 1).removeClass("now");
            $("#subTab_" + 2).addClass("now");
            $("#subTab_" + 3).removeClass("now");

            $("#indexTab").val("productReviewTab2");
            $("#SearchType").val('13');
        });


        $("#subTab_3").click(function () {
            $("#productReviewTab0").attr("style", "display:none");
            $("#productReviewTab1").attr("style", "display:none");
            $("#productReviewTab2").attr("style", "display:none");
            $("#productReviewTab3").attr("style", "display:block");
            $("#productReviewTab4").attr("style", "display:none");
            $("#productReviewTab5").attr("style", "display:none");


            $("#subTab_" + 0).removeClass("now");
            $("#subTab_" + 1).removeClass("now");
            $("#subTab_" + 2).removeClass("now");
            $("#subTab_" + 3).addClass("now");

            $("#indexTab").val("productReviewTab3");
            $("#SearchType").val('14+15');
        });

    },
    //发表评论
    createReview: function () {
        if (!checkOneMiniteBeforePublishForVote(60)) {
            window.alert("很抱歉，您发表评论的频率过快，请稍后再试。")
            return;
        }
        var btnsubmit = document.getElementById("btnsubmit");
        var spansubmit = document.getElementById("spansubmit");
        btnsubmit.style.display = "none";
        spansubmit.style.display = "block";



        reviewQuery.CheckLogin();
        var info = {
            ProductSysNo: $("#ProductSysNo").val(),
            Title: $("#title").val(),
            Prons: $("#txtProns").val(),
            Cons: $("#txtCons").val(),
            Service: $("#txtService").val(),
            Score1: $("#score1").text(),
            Score2: $("#score2").text(),
            Score3: $("#score3").text(),
            Score4: $("#score4").text(),
            SoSysNo: $("#SoSysNo").val(),
            Image: $("#fileToUpload").val()
        };



        if (info.Title.length <= 0) {
            window.alert("评论标题长度不能少于0！");
            btnsubmit.style.display = "block";
            spansubmit.style.display = "none";
            return;
        }
        else if (info.Title.length > 40) {
            window.alert("评论标题长度不能大于40！");
            btnsubmit.style.display = "block";
            spansubmit.style.display = "none";
            return;
        }
        if (info.Prons.length <= 0) {
            window.alert("评论内容长度不能少于0！");
            btnsubmit.style.display = "block";
            spansubmit.style.display = "none";
            return;
        }
        else if (info.Prons.length > 300) {
            window.alert("评论内容长度不能大于300！");
            btnsubmit.style.display = "block";
            spansubmit.style.display = "none";
            return;
        }
        //if (info.Cons.length > 300) {
        //    window.alert("评论缺点长度不能大于300！");
        //    btnsubmit.style.display = "block";
        //    spansubmit.style.display = "none";
        //    return;
        //}
        //if (info.Service.length > 300) {
        //    window.alert("评论服务质量长度不能大于300！");
        //    btnsubmit.style.display = "block";
        //    spansubmit.style.display = "none";
        //    return;
        //}



        $.ajax({
            url: $("#CreateReview").val(),
            type: 'POST',
            timeout: 30000,
            data: info,
            dataType: "json",
            beforeSend: function (XMLHttpRequest) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                spansubmit.style.display = "none";
                btnsubmit.style.display = "block";
            },
            success: function (data) {
                //window.alert(data);
                if (data.error) return;
                if (data != "1") {
                    //window.alert("发表评论失败！");
                    btnsubmit.style.display = "block";
                    spansubmit.style.display = "none";
                }
                else {
                    window.alert("发表评论成功,24小时内显示！");
                    spansubmit.style.display = "none";
                    btnsubmit.style.display = "block";
                    resetPublishTimePointForVote();
                }
            }, complete: function () {
                btnsubmit.style.display = "block";
                spansubmit.style.display = "none";
            }
        });
    },
    //查询评论
    queryProductReview: function (obj) {

        var reviewCount = parseInt($(obj).attr("count"));
        var searchType = $(obj).attr("searchType");
        var productGroupSysNo = $("#ProductGroupSysNo").val();
        reviewCount = 1;
        if (reviewCount <= 0) {
            return;
        }
        var ReviewQueryInfo = {
            ProductGroupSysNo: productGroupSysNo,
            ProductSysNo: $("#ProductSysNo").val(),
            SearchType: searchType
        }

        $.ajax({
            url: $("#QueryProductReview").val(),
            type: 'POST',
            dataType: 'html',
            timeout: 30000,
            data: ReviewQueryInfo,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                $($.format("#productReviewTab{0}", $(obj).attr("rel"))).html(data);
            }
        });


    },
    CreateProductReviewReply: function () {
        reviewQuery.CheckLogin();
        if (!checkOneMiniteBeforePublishForVote(60)) {
            window.alert("很抱歉，您发表评论的频率过快，请稍后再试。")
            return;
        }
        var replyinfo = {
            ReviewSysNo: $("#ReviewSysNo").val(),
            Content: $("#txtReplyContent").val(),
            ProductSysNo: $("#ProductSysNo").val(),
            Mail: $("#Mail").val(),
            SOSysNo: $("#SOSysNo").val()
        };
        $.ajax({
            url: $("#CreateReviewPly").val(),
            type: 'POST',
            data: replyinfo,
            dataType: "json",
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data.error) return;
                if (data != "1") {
                    window.alert("发表回复失败！");
                }
                else {
                    window.alert("发表回复成功！");
                    resetPublishTimePointForVote();
                }
            }
        });
    },

    ///评论列表页隐藏回复
    createListReply: function () {
        reviewQuery.CheckLogin();
        if (!checkOneMiniteBeforePublishForVote(60)) {
            window.alert("很抱歉，您发表评论的频率过快，请稍后再试。")
            return;
        }
        $("@divHidden").attr("display", "block");
        var replyinfo = {
            ReviewSysNo: $("#ReviewSysNo").val(),
            Content: $("#txtHidden").val(),
            ProductSysNo: $("#ProductSysNo").val()
        };

        $.ajax({
            url: $("#CreateReviewPly").val(),
            type: 'POST',
            timeout: 30000,
            data: replyinfo,
            dataType: "json",
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data.error) return;
                if (data <= 0) {
                    window.alert("发表回复失败！");
                }
                else {
                    window.alert("发表回复成功！");
                    resetPublishTimePointForVote();
                }
            }
        });
    },

    ///评论详情页页隐藏回复
    createDetailReply1: function () {
        reviewQuery.CheckLogin();
        if (!checkOneMiniteBeforePublishForVote(60)) {
            window.alert("很抱歉，您发表评论的频率过快，请稍后再试。")
            return;
        }
        var replyinfo = {
            ReviewSysNo: $("#ReviewSysNo").val(),
            Content: $("#txtHidden1").val(),
            ProductSysNo: $("#ProductSysNo").val()
        };

        $.ajax({
            url: $("#CreateReviewPly").val(),
            type: 'POST',
            timeout: 30000,
            data: replyinfo,
            dataType: "json",
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data.error) return;
                if (data <= 0) {
                    window.alert("发表回复失败！");
                }
                else {
                    window.alert("发表回复成功！");
                    resetPublishTimePointForVote();
                }
            }
        });
    },



    ///评论详情页隐藏回复
    createDetailReply2: function () {
        reviewQuery.CheckLogin();
        if (!checkOneMiniteBeforePublishForVote(60)) {
            window.alert("很抱歉，您发表评论的频率过快，请稍后再试。")
            return;
        }
        var replyinfo = {
            ReviewSysNo: $("#ReviewSysNo").val(),
            Content: $("#txtHidden2").val(),
            ProductSysNo: $("#ProductSysNo").val()
        };

        $.ajax({
            url: $("#CreateReviewPly").val(),
            type: 'POST',
            timeout: 30000,
            data: replyinfo,
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data.error) return;
                if (data <= 0) {
                    window.alert("发表回复失败！");
                }
                else {
                    window.alert("发表回复成功！");
                    resetPublishTimePointForVote();
                }
            }
        });
    },

    //检查登录
    CheckLogin: function () {
        $.ajax({
            url: $("#CheckLogin").val(),
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data < 0) {
                    window.alert("请先登录！");
                    return;
                }
            }
        });
    },

    //加入购物车
    addProductToCart: function () {
        window.location.href = $.format("{0}?Category=Product&SysNo={1}&Qty={2}",
            $("#ShoppingCartUrl").val(),
            $("#ProductGroupSysNo").val(),
           1);
    },

    //加入收藏
    addToWish: function (obj) {
        $.ajax({
            url: $("#AddToWishListUrl").val(),
            type: 'post',
            dataType: 'json',
            data: { ProductSysNo: $("#ProductSysNo").val() },
            success: function (data) {
                if (!data.error) {
                    $(obj).removeClass("btn_addcartB").addClass("btn_favoredB").removeAttr("onclick");
                    PopWin('#addwish').fn.popIn();
                }
                else
                {
                    window.location.href = $("#LoginUrl").val() + "?ReturnUrl=" + $("#ReturnUrl").val();
                }
            }
        })
    },
    wish: function () {
        PopWin('#wish').fn.popIn();
    },
    Login: function () {
        if (confirm("咨询需要登录，是否立即登录？")) {
            var LoginUrl = $("#LoginUrl").val();
            var ReturnUrl = $("#ReturnUrl").val();
            location.href = LoginUrl + "?returnurl=" + ReturnUrl;
        };
    },
    reviewVote: function (obj) {

        var reviewSysNo = $(obj).attr("reviewSysNo");
        var isUsefull = $(obj).attr("usefull");

        //一分钟检查
        if (!checkOneMiniteBeforePublishForVote(60)) {
            $(obj).parent().find("span").remove();
            $(obj).parent().find("a").first().before("<span>很抱歉，您发表的频率过快，请稍后再试。</span>")
            setTimeout(function () { $(obj).parent().find("span").remove(); }, 3000);

            return;
        }
        if ($(obj).parent("span").attr("isvote") == "1") {
            $(obj).parent().find("span").remove();
            $(obj).parent().find("a").first().before("<span>您已经对该评论投过票，谢谢。</span>")
            setTimeout(function () { $(obj).parent().find("span").remove(); }, 3000);

            return;
        }

        $.ajax({
            url: $("#ReviewVoteUrl").val(),
            dataType: 'json',
            type: 'post',
            data: { ReviewSysNo: reviewSysNo, IsUsefull: isUsefull },
            success: function (data) {
                if (data.error) {
                    //跳转到登录页面
                    location.href = $("#LoginUrl").val() + "?ReturnUrl=" + $("#ReturnUrl").val();
                }
                else {
                    if (data == 1) {
                        $(obj).parent().find("span").remove();
                        $(obj).parent().find("a").first().before("<span>投票成功，谢谢你的参与。</span>")
                        //防止投票成功后多次投票
                        $(obj).parent().attr("isvote", "1");
                        var count = parseInt($(obj).attr("count"));
                        if (isUsefull == "1") {
                            $(obj).html($.format("有用({0})", count + 1));
                        }
                        else {
                            $(obj).html($.format("没用({0})", count + 1));
                        }
                        setTimeout(function () { $("#isGod").parent().find("span").remove(); }, 3000);
                        resetPublishTimePointForVote();
                    }
                    else {
                        $(obj).parent().find("span").remove();
                        $(obj).parent().find("a").first().before("<span>您已经对该评论投过票，谢谢。</span>")
                        setTimeout(function () { $(obj).parent().find("span").remove(); }, 3000);

                    }
                }
            }

        })
    }
} 