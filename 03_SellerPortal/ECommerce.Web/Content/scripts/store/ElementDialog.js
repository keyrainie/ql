(function () {
    window["ElementDialog"] = {
        _pageTemplateType: "",
        _pageType: "",
        _key: "",
        _ops: null,
        _data: null,
        init: function (ops) {
            this._ops = ops;
            //ops.dialog
            //ops.onPreShow()
            //ops.onShown()
            //ops.onSave()
            var me = this;

            ops.dialog.modal({
                show: false
            }).on("shown.bs.modal", function (evt) {
                //var bannerData = $(this).data("hidden").val();
                //console.info("Banner Data : " + bannerData);
                if (ops.onPreShow) {
                    ops.onPreShow(this);
                }
                $.ajax({
                    type: "GET",
                    url: '/Store/AjaxGetDialog',
                    data: { pageTypeKey: me._pageType, key: me._key },
                    dataType: "html",
                    success: function (data) {
                        me._ops.dialog.find(".modal-content").html(data);
                        if (me._pageTemplateType == "Main2" && me._key == "DoubleBanner") {
                            $("#BannerImageTODO").html("建议：图片尺寸500*350<br />最多上传两张图片");
                        }
                        if (me._pageTemplateType == "Main2" && me._key == "RecommendBanner") {
                            $("#BannerImageTODO").html("建议：图片尺寸1000*350<br />");
                        }
                        if (me._pageTemplateType == "Main2" && me._key == "SingleBanner") {
                            $("#BannerImageTODO").html("建议：图片尺寸1000*350<br />最多上传一张图片");
                        }
                        if (me._pageTemplateType == "LeftMain2" && me._key == "DoubleBanner") {
                            $("#BannerImageTODO").html("建议：左侧栏图片尺寸210*110，内容栏图片尺寸390*200<br />最多上传两张图片");
                        }
                        if (me._pageTemplateType == "LeftMain2" && me._key == "RecommendBanner") {
                            $("#BannerImageTODO").html("建议：左侧栏图片尺寸210*92，内容栏图片尺寸780*340<br />");
                        }
                        if (me._pageTemplateType == "LeftMain2" && me._key == "SingleBanner") {
                            $("#BannerImageTODO").html("建议：左侧栏图片尺寸210*105，内容栏图片尺寸780*340<br />最多上传一张图片");
                        }
                        if (me._pageTemplateType == "LeftMiddleRight2" && me._key == "DoubleBanner") {
                            $("#BannerImageTODO").html("建议：左侧栏图片尺寸210*110，中栏图片尺寸325*245，右侧栏图片尺寸210*110<br />最多上传两张图片");
                        }
                        if (me._pageTemplateType == "LeftMiddleRight2" && me._key == "RecommendBanner") {
                            $("#BannerImageTODO").html("建议：左侧栏图片尺寸210*92，中栏图片尺寸650*245，右侧栏图片尺寸210*92<br />");
                        }
                        if (me._pageTemplateType == "LeftMiddleRight2" && me._key == "SingleBanner") {
                            $("#BannerImageTODO").html("建议：左侧栏图片尺寸210*105，中栏图片尺寸650*245，右侧栏图片尺寸210*105<br />最多上传一张图片");
                        }
                        var oTable = null;
                        //保存数据操作
                        me._ops.dialog.find("#SaveBtn").click(function ()
                        {
                            if (me._key == "RecommendBanner"
                                || me._key == "RecommendProduct"
                                || me._key == "RecommendBrand"
                                || me._key == "DoubleBanner"
                                || me._key == "SingleBanner") {
                                oTable = me._ops.dialog.find('#dataTable');
                                if (me._ops.onSave)
                                {
                                    var tableData = BuildDataFromTable(oTable);
                                    me._ops.onSave(tableData);
                                }
                            }
                            else if (me._key == "HtmlEditor" || me._key == "EditorWithTitle")
                            {
                                me._ops.onSave(me._ops.dialog.find('#HtmlEditorDialogContent').val());
                            }
                            else if (me._key == "WeekRanking"
                                || "NewProducts" == me._key) {
                                var count = me._ops.dialog.find('#numCount').val();
                                var reg = /^[1-9]{1}$/;
                                if (!reg.test(count)) {
                                    me._ops.dialog.find("#lblError").show();
                                    return false;
                                }
                                me._ops.onSave(count);
                            }
                            me._ops.dialog.modal("hide");
                        });

                        if (ops.onShown) {
                            ops.onShown();
                        }

                        if (me._data || typeof (me._data) !== "object")
                        {
                            //还原数据,生成表格
                            if (me._key == "RecommendBanner"
                                || me._key == "RecommendBrand"
                                || me._key == "DoubleBanner"
                                || me._key == "SingleBanner")
                            {
                                oTable = me._ops.dialog.find('#dataTable');
                                for (var r = 0; r < me._data.length; r++)
                                {
                                    rowdata = me._data[r];
                                    var oTr = BuildBannerRow(rowdata);
                                    oTable.append(oTr);
                                }
                            }
                            else if (me._key == "RecommendProduct")
                            {
                                oTable = me._ops.dialog.find('#dataTable');
                                for (var r = 0; r < me._data.length; r++)
                                {
                                    rowdata = me._data[r];
                                    var oTr = BuildProductRow(rowdata);
                                    oTable.append(oTr);
                                }
                            }
                            else if (me._key == "HtmlEditor" || me._key == "EditorWithTitle")
                            {
                                me._ops.dialog.find('#HtmlEditorDialogContent').val(me._data)
                            }
                            else if (me._key == "WeekRanking"
                                || "NewProducts" == me._key) {
                                var count = me._ops.dialog.find('#numCount').val(me._data);
                            }
                        }

                    }
                });
            }).on("hide.bs.modal", function (e) {
                me._ops.dialog.find(".modal-content").html("");
            });
        },
        show: function (pageTypeKey, elementKey, data, pageTemplateTypeKey) {
            this._pageTemplateType = pageTemplateTypeKey;
            this._pageType = pageTypeKey;
            this._key = elementKey;
            this._data = data;
            this._ops.dialog.modal("show");
        }
    }



    function BuildDataFromTable(oTable) {
        var trlist = oTable.find("tr");
        var DataList = [];
        for (var r = 1; r < trlist.length; r++) {
            var row = trlist[r];
            var RowData = new Object();
            for (var c = 0; c < row.cells.length; c++) {
                var cell = row.cells[c];
                if (cell.filedName != null) {
                    RowData[cell.filedName] = cell.Value;
                }
            }
            DataList.push(RowData);
        }
        return DataList;
    }

})();

//生成推荐广告表一行的方法，推荐广告管理页面也用的这个方法
function BuildBannerRow(rowdata) {
    var oTr = document.createElement('tr');
    var oTd = null;

    oTd = document.createElement('td');
    oTd.innerHTML = "<img width='50%' src='" + rowdata.ImageUrl + "'>";
    oTd.Value = rowdata.ImageUrl;
    oTd.filedName = "ImageUrl";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.Title;
    oTd.Value = rowdata.Title;
    oTd.filedName = "Title";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.LinkUrl;
    oTd.Value = rowdata.LinkUrl;
    oTd.filedName = "LinkUrl";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.Priority;
    oTd.Value = rowdata.Priority;
    oTd.filedName = "Priority";
    oTr.appendChild(oTd);
    
    oTd = document.createElement('td');
    oTd.innerHTML = "<a href='javascript:void(0)' onclick='$(this).parent().parent().remove()'>删除</a>";

    oTr.appendChild(oTd);
    return oTr;
}

//生成推荐商品表一行的方法，推荐商品管理页面也用的这个方法
function BuildProductRow(rowdata)
{
    var oTr = document.createElement('tr');
    var oTd = null;

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.Sysno;
    oTd.Value = rowdata.Sysno;
    oTd.filedName = "Sysno";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.Title;
    oTd.Value = rowdata.Title;
    oTd.filedName = "Title";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.Price;
    oTd.Value = rowdata.Price;
    oTd.filedName = "Price";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.TariffRatePrice;
    oTd.Value = rowdata.TariffRatePrice;
    oTd.filedName = "TariffRatePrice";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.Priority;
    oTd.Value = rowdata.Priority;
    oTd.filedName = "Priority";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = "<a href='javascript:void(0)' onclick='$(this).parent().parent().remove()'>删除</a>";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.innerHTML = rowdata.OriginalID;
    oTd.style.display = 'none';
    oTd.Value = rowdata.OriginalID;
    oTd.filedName = "OriginalID";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.style.display = 'none';
    oTd.innerHTML = rowdata.OriginalImage;
    oTd.Value = rowdata.OriginalImage;
    oTd.filedName = "OriginalImage";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.style.display = 'none';
    oTd.innerHTML = rowdata.OriginalPrice;
    oTd.Value = rowdata.OriginalPrice;
    oTd.filedName = "OriginalPrice";
    oTr.appendChild(oTd);

    oTd = document.createElement('td');
    oTd.style.display = 'none';
    oTd.innerHTML = rowdata.OriginalTitle;
    oTd.Value = rowdata.OriginalTitle;
    oTd.filedName = "OriginalTitle";
    oTr.appendChild(oTd);

    

    return oTr;
}

