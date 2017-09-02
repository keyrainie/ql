
var ModifyAvtar = {
    //上传控件
    UploadControl: null,
    CustomerSysNo: 0,
    FileBaseUrl: "",
    //初始化
    Init: function (fileBaseUrl) {
        ModifyAvtar.FileBaseUrl = fileBaseUrl;
        //初始化上传控件
        ModifyAvtar.UploadControl = new SWFUpload({
            post_params: {
                "ASPSESSID": "@Session.SessionID"
            },
            // Backend Settings
            upload_url: $('#hiddenUploaderHandlerURL').val(),

            // File Upload Settings
            file_size_limit: "500 KB",
            file_types: "*.jpg;*,gif;",
            file_types_description: "Image Files",
            //file_upload_limit: "10",
            file_upload_limit: 5,    // Zero means unlimited个数

            // Event Handler Settings (all my handlers are in the Handler.js file)
            file_dialog_start_handler: fileDialogStart,
            file_queued_handler: fileQueued,
            file_queue_error_handler: fileQueueError,
            file_dialog_complete_handler: fileDialogComplete,
            upload_start_handler: uploadStart,
            upload_progress_handler: uploadProgress,
            upload_error_handler: uploadError,
            upload_success_handler: uploadSuccess,
            upload_complete_handler: uploadComplete,

            // Button settings
            button_placeholder_id: "UploadImageBtn",
            button_width: 60,
            button_height: 26,

            button_cursor: SWFUpload.CURSOR.HAND,
            button_text: "",
            button_image_url: "/Resources/themes/default/Nest/img/upload.gif",
            button_text_top_padding: 0, //5,
            button_text_left_padding: 0, //11,
            button_text_right_padding: 0, // 3,

            // Flash Settings
            flash_url: "/Resources/scripts/swfupload/swfupload.swf",
            flash9_url: "/Resources/scripts/swfupload/swfupload_fp9.swf",
            custom_settings: {
                progressTarget: "swfu-placeholder",
                cancelButtonId: "btnCancelUpload"
            },
            debug: false
        });
    },
    saveUpload: function () {
        if ($("#fileToUpload").val() == '') {
            alert("请先上传头像，然后保存！");
            return;
        }
        $.ajax({
            type: "post",
            url: "/MemberAccount/AjaxChangeCustomerAvatar",
            dataType: "json",
            async: false,
            timeout: 30000,
            data: { AvatarImg: $("#fileToUpload").val() },
            beforeSend: function (XMLHttpRequest) { },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (data) {
                if (data != "s") {
                    alert(data);
                }
                else {
                    $("#fileToUpload").val("");
                    alert("保存头像成功，后台审核通过后可以显示！");
                    $(".progressWrapper").hide();
                    location.reload();
                }
            },
            complete: function (XMLHttpRequest, textStatus) { }
        });

    }
}
var uploadedImageFilesArr = null;
var totalUploadFilesCount = 0;
var selectProductSysNo = [];
function cancelQueue(instance) {
    document.getElementById(instance.customSettings.cancelButtonId).disabled = true;
    instance.stopUpload();
    var stats;

    do {
        stats = instance.getStats();
        instance.cancelUpload();
    } while (stats.files_queued !== 0);

}

function fileDialogStart() {
    /* I don't need to do anything here */
    //检查是否选择了商品
    //    $(".NoneUploadProductList").each(function () {
    //        if ($(this).find("input").attr("checked") == "checked") {
    //            selectProductSysNo.push(parseInt($(this).attr("ProductSysNo")));
    //        }
    //    });
}
function fileQueued(file) {
    var progress = new FileProgress(file, this.customSettings.progressTarget);
    //    if (selectProductSysNo.length == 0) {
    //        progress.toggleCancel(false);
    //        return false;
    //    }
    try {
        progress.setStatus("图片上传中");
        progress.toggleCancel(true, this);

    } catch (ex) {
        this.debug(ex);
    }

}

function fileQueueError(file, errorCode, message) {
    try {
        if (errorCode === SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED) {
            //alert("You have attempted to queue too many files.\n" + (message === 0 ? "You have reached the upload limit." : "You may select " + (message > 1 ? "up to " + message + " files." : "one file.")));
            alert((message === 0 ? "您已超过上传限制." : "您最多只能上传5张图片."));
            return;
        }

        //var progress = new FileProgress(file, this.customSettings.progressTarget);
        //progress.setError();
        //progress.toggleCancel(false);

        switch (errorCode) {
            case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
                //progress.setStatus("File is too big.");
                //this.debug("Error Code: File too big, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                UploadFileErrorTip("对不起，您上传的图片或文件太大了");
                break;
            case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
                //progress.setStatus("Cannot upload Zero Byte files.");
                //this.debug("Error Code: Zero byte file, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                UploadFileErrorTip("对不起，不支持上传0字节图片或文件");
                break;
            case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
                //progress.setStatus("Invalid File Type.");
                //this.debug("Error Code: Invalid File Type, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                UploadFileErrorTip("对不起，您上传的图片或文件类型不支持");
                break;
            case SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED:
                alert("You have selected too many files.  " + (message > 1 ? "You may only add " + message + " more files" : "You cannot add any more files."));
                break;
            default:
                if (file !== null) {
                    progress.setStatus("Unhandled Error");
                }
                this.debug("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
        }
    } catch (ex) {
        this.debug(ex);
    }
}

function fileDialogComplete(numFilesSelected, numFilesQueued) {
    //检查是否选择了商品
    //    if (selectProductSysNo.length == 0) {
    //        $.alert("请选择所需商品图片的商品", "error", function () { window.location.reload(); });
    //        return false;
    //    }
    //  $("#btnCancelUpload").show();

    totalUploadFilesCount = numFilesSelected;
    uploadedImageFilesArr = new Array();
    uploadedImageFilesArr.length = 0;
    try {
        if (this.getStats().files_queued > 0) {
            document.getElementById(this.customSettings.cancelButtonId).disabled = false;
        }
        this.startUpload();
    } catch (ex) {
        this.debug(ex);
    }
}

function uploadStart(file) {
    try {
        var progress = new FileProgress(file, this.customSettings.progressTarget);
        progress.setStatus("正在上传...");
        progress.toggleCancel(true, this);
    }
    catch (ex) {
    }
    return true;
}

function uploadProgress(file, bytesLoaded, bytesTotal) {
    try {
        var percent = Math.ceil((bytesLoaded / bytesTotal) * 100);

        var progress = new FileProgress(file, this.customSettings.progressTarget);
        progress.setProgress(percent);
        progress.setStatus("正在上传...");
    } catch (ex) {
        this.debug(ex);
    }
}

function uploadSuccess(file, serverData) {
    try {
        $("#btnCancelUpload").hide();
        var progress = new FileProgress(file, this.customSettings.progressTarget);
        progress.setComplete();
        var data = JSON.parse(serverData);
        if (data.state === "SUCCESS") {
            //alert(file.index);
            $("#swfu-placeholder .pic ul img").eq(4 - file.index).attr("src", ModifyAvtar.FileBaseUrl + data.url);
            $("#swfu-placeholder .pic ul li").eq(4 - file.index).css("display", "block");
            $("#swfu-placeholder .pic ul img").eq(4 - file.index).next().html(file.name);
            if ($("#fileToUpload").val() == "" || $("#fileToUpload").val() == null) {
                $("#fileToUpload").val(data.url);
            }
            else {
                data.url += "[upimg]" + $("#fileToUpload").val();
                $("#fileToUpload").val(data.url);
            }
            
            
            progress.setStatus("上传成功!");
        } else {
            progress.setStatus(data.message);
        }
        progress.toggleCancel(false);

        //        uploadedImageFilesArr.push(serverData);

        //        if (uploadedImageFilesArr.length == totalUploadFilesCount) {
        //            var list = [];
        //            for (var i = 0; i < selectProductSysNo.length; i++) {
        //                for (var j = 0; j < uploadedImageFilesArr.length; j++) {
        //                    list.push({
        //                        ProductGroupSysNo: ProductImage.ProductGroupSysNo,
        //                        ProductSysNo: selectProductSysNo[i],
        //                        RelationPath: uploadedImageFilesArr[j],
        //                        Priority: 10000
        //                    });
        //                }
        //            }
        //            $.ajax({
        //                url: "/ProductMaintain/AjaxSaveProductImages",
        //                type: "POST",
        //                dataType: "json",
        //                data: { data: encodeURI($.toJSON(list)) },
        //                success: function (data) {
        //                    if (!data) {
        //                        $.alert("上传商品图片成功,成功上传了" + totalUploadFilesCount + "张图片!", "info", function () {
        //                            window.location.reload();
        //                            return;
        //                        });
        //                    }
        //                }
        //            });

        //        }
    } catch (ex) {
        this.debug(ex);
    }
}

function uploadComplete(file) {
    try {
        if (this.getStats().files_queued === 0) {
            document.getElementById(this.customSettings.cancelButtonId).disabled = true;
        } else {
            this.startUpload();
        }
    } catch (ex) {
        this.debug(ex);
    }

}

function uploadError(file, errorCode, message) {
    try {
        var progress = new FileProgress(file, this.customSettings.progressTarget);
        progress.setError();
        progress.toggleCancel(false);

        switch (errorCode) {
            case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
                progress.setStatus("Upload Error: " + message);
                this.debug("Error Code: HTTP Error, File name: " + file.name + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.MISSING_UPLOAD_URL:
                progress.setStatus("Configuration Error");
                this.debug("Error Code: No backend file, File name: " + file.name + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED:
                progress.setStatus("Upload Failed.");
                this.debug("Error Code: Upload Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.IO_ERROR:
                progress.setStatus("Server (IO) Error");
                this.debug("Error Code: IO Error, File name: " + file.name + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR:
                progress.setStatus("Security Error");
                this.debug("Error Code: Security Error, File name: " + file.name + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
                progress.setStatus("Upload limit exceeded.");
                this.debug("Error Code: Upload Limit Exceeded, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.SPECIFIED_FILE_ID_NOT_FOUND:
                progress.setStatus("File not found.");
                this.debug("Error Code: The file was not found, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED:
                progress.setStatus("Failed Validation.  Upload skipped.");
                this.debug("Error Code: File Validation Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
            case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
                if (this.getStats().files_queued === 0) {
                    document.getElementById(this.customSettings.cancelButtonId).disabled = true;
                }
                progress.setStatus("已取消上传.");
                progress.setCancelled();
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
                progress.setStatus("已终止上传.");
                break;
            default:
                progress.setStatus("Unhandled Error: " + error_code);
                this.debug("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                break;
        }
    } catch (ex) {
        this.debug(ex);
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

function UploadFileErrorTip(msg) {
    if (typeof (msg) != 'undefined') {
        alert(msg);
    }
}