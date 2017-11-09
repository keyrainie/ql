//Mini购物车操作
$(document).ready(function () {
    ShoppingCartMini.Init();
});
var ShoppingCartMini = {
    Init: function () {
        ShoppingCartMini.RefreshUIJs();
        ShoppingCartMini.GetMiniShoppingCart();
    },
    //显示遮罩层
    ShowMaskLayer: function () {
        if ($("#miniShoppingCartPannelMask") != undefined) {
            if ($(".cart").attr("InnerHeight") != undefined && $(".cart").attr("InnerHeight") != 15) {
                $("#miniShoppingCartPannelMask").css("width", (parseInt($(".cart").innerWidth().toString()) - 15) + "px");
                $("#miniShoppingCartPannelMask").css("height", (parseInt($(".cart").attr("InnerHeight")) - 25) + "px");
                $("#miniShoppingCartPannelMask").show();
            }
        }
    },
    //隐藏遮罩层
    HideMaskLayer: function () {
        $("#miniShoppingCartPannelMask").hide();
    },
    //获取Mini购物车
    GetMiniShoppingCart: function () {
        $.utility.settings.showLoading = false;
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/ShoppingCart/GetMiniShoppingCart",
            cache: false,
            beforeSend: function (XMLHttpRequest) {
                ShoppingCartMini.ShowMaskLayer();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data, textStatus, jqXHR) {
                if ($(".mycart").find("dd").is(':visible')){
                    data = data.replace('<dt>', '<dt class="dt_hover">');
                    data = data.replace('<dd>', '<dd style="display:block;">');
                }
                $(".shoppingCart").html(data);
                ShoppingCartMini.RefreshUIJs();
                ShoppingCartMini.RefreshDelUIJs();
                if ($(".cart").innerHeight() != undefined) {
                    $(".cart").attr("InnerHeight", $(".cart").innerHeight().toString());
                }
            },
            complete: function (XMLHttpRequest, textStatus) {
                ShoppingCartMini.HideMaskLayer();
                ShoppingCartMini.GetPopCartPrdCnt();
            }
        });
    },
    //浮动条购物车商品数量
    GetPopCartPrdCnt: function () {
        $.utility.settings.showLoading = false;
        $.ajax({
            url: "/ShoppingCart/GetMiniShoppingCartCount",
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (!data.error) {
                    $("#PopCartPrdCnt").text(data);
                }
            }
        });
        ShoppingCartMini.RefreshUIJs();
    },
    //Mini购物车删除商品
    DelMiniShoppingCartProduct: function (packageNo, productNo) {
        $.utility.settings.showLoading = false;
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/ShoppingCart/DelMiniShoppingCartProduct" + "?PackageSysNo=" + packageNo + "&ProductSysNo=" + productNo,
            cache: false,
            beforeSend: function (XMLHttpRequest) {
                ShoppingCartMini.ShowMaskLayer();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data, textStatus, jqXHR) {
                if ($(".mycart").find("dd").is(':visible')) {
                    data = data.replace('<dt>', '<dt class="dt_hover">');
                    data = data.replace('<dd>', '<dd style="display:block;">');
                }
                $(".mycart").html(data);
                ShoppingCartMini.RefreshDelUIJs();
                $(".cart").attr("InnerHeight", $(".cart").innerHeight().toString());
            },
            complete: function (XMLHttpRequest, textStatus) {
                ShoppingCartMini.HideMaskLayer();
                ShoppingCartMini.GetPopCartPrdCnt();
            }
        });
    },
    //刷新UI的JS
    RefreshUIJs: function () {
        //Mycart Popup;
        $(".mycart").unbind("mouseenter").unbind("mouseleave");
        $(".mycart").hover(function () {
            var $this = $(this);
            UI.laterEvent(function () {
                $this.find("dt").addClass("dt_hover");
                if (!$(".mycart").find("dd").is(':visible')) {
                    ShoppingCartMini.GetMiniShoppingCart();
                }
                $this.find("dd").show();
            }, 200, this);
        }, function () {
            var $this = $(this);
            UI.laterEvent(function () {
                $this.find("dt").removeClass("dt_hover");
                $this.find("dd").hide();
            }, 200, this);

        });
    },
    RefreshDelUIJs: function () {
        /* Mycart Product line Hover Highlight */
        $(".mycart .prolist li").hover(
            function () {
                $this = $(this);
                $this.addClass("cur");
            },
            function () {
                $this = $(this);
                $this.removeClass("cur");
            }
        );
        $(".mycart .prolist .btn_del").hover(
            function () {
                $(this).parents("li").addClass("hover");
            },
            function () {
                $(this).parents("li").removeClass("hover");
            }
        );

        /*  Mycart Popup XSlider*/
        $(".mycart .listwrap").each(function (k, v) {
            var $this = $(this);
            var li = $this.find(".prolist").children("li");
            if (li.length > 3) {
                $this.find(".slidewrap").height(207);
                $this.find(".abtn").css("display", "block");
                $this.Xslider({
                    dir: "V",
                    viewedSize: 207,
                    unitLen: 69,
                    numtoMove: 3,
                    speed: 600
                });
            }
        });
    }
}