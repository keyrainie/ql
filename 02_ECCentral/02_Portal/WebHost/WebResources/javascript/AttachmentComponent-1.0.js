
function AttachmentComponent(containerId, maxFileCount) {
    this._containerId = containerId;
    this._fileIndex = 0;
    this._currentFileCount = 0;
    this._maxFileCount = maxFileCount;
    this._wrapper = null;
    this.browser = "";
    this.del = "";
    this.msg = "";

    this.lang = {
        browser: ["Browse...", "浏览...", "瀏覽..."],
        del: ["Delete", "删除", "刪除"],
        msg: ["the max count of attachment is " + this._maxFileCount,
    "最大允许发送附件数量为：" + this._maxFileCount,
    "最大允許發送附件數量為：" + this._maxFileCount]
    };
}

//初始化
AttachmentComponent.prototype.Init = function (languageCode) {
    this._wrapper = $("#" + this._containerId);
    if (languageCode == "en-US") {
        this.message = this.lang.msg[0];
        this.browser = this.lang.browser[0];
        this.del = this.lang.del[0];
    }
    if (languageCode == "zh-CN") {
        this.message = this.lang.msg[1];
        this.browser = this.lang.browser[1];
        this.del = this.lang.del[1];
    }
    if (languageCode == "zh-TW") {
        this.message = this.lang.msg[2];
        this.browser = this.lang.browser[2];
        this.del = this.lang.del[2];
    }
}

//新增附件
AttachmentComponent.prototype.AddAttachment = function () {
    if (this._currentFileCount > this._maxFileCount - 1) {
        alert(this.message);
        return;
    }

    this._fileIndex++;

    var ht = [];
    ht.push("<li id='li_" + this._fileIndex + "'>");
    ht.push("<input type='text' class='file_text' readonly='readonly' id='txtFile_" + this._fileIndex + "' />");
    ht.push("<input type='button' value='" + this.browser + "' />");
    ht.push("<input type='file' name='file' id='file_" + this._fileIndex + "' class='fileupload' onchange='filechange(" + this._fileIndex + ",this.value)' />");
    ht.push("<span class='link_delete'>[<a href='javascript:void(0)' onclick='deleteAttachment(" + this._fileIndex + ")'>" + this.del + "</a>]</span>");
    ht.push("</li>");

    $(ht.join('')).appendTo(this._wrapper);

    this._currentFileCount++;

}

AttachmentComponent.prototype.DeleteAttachment = function () {
    this._currentFileCount--;
}


function deleteAttachment(fileIndex) {
    $("#li_" + fileIndex).remove();
    component.DeleteAttachment();
}

function filechange(fileIndex, val) {
    $("#txtFile_" + fileIndex).val(val);
}