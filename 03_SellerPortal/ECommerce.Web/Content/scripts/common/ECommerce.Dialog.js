(function () {
    if (!window["ECommerce"]) window["ECommerce"] = {};
	if (!window["dialogContext"]) window["dialogContext"] = [];

	//dialog
	function Dialog(ops) {
		var me = this;
		this.ops = ops;
		//this._id = (new Date()).valueOf();
		//create dialog container
		this.container = $('<div class="modal fade bs-modal-lg" data-keyboard="false" data-backdrop="static" aria-hidden="true"><div class="clearfix"></div><div class="page-loading page-loading-boxed"><img src="/Content/themes/metronic/assets/global/img/loading-spinner-grey.gif" alt="loading" /><span>&nbsp;&nbsp;Loading... </span></div><div class="modal-dialog modal-lg"><div class="modal-content"></div></div></div>');
		this.container.appendTo(document.body);
		this.container.modal({
			show: false
		}).on("shown.bs.modal", function (evt) {
			//var bannerData = $(this).data("hidden").val();
			//console.info("Banner Data : " + bannerData);
			if (ops.onPreShow) {
				ops.onPreShow(me);
			}
			$.ajax({
				type: "GET",
				url: '/Common/AjaxShowDialog',
				data: { url: me.ops.url },
				dataType: "html",
				success: function (data) {
				    dialogContext.push(me);
				    me.container.find(".modal-content").html(data);
				    //me.container.find(".modal-content").append("<input/>")
					if (ops.onShown) {
						ops.onShown(me);
					}
					
				}
			});
		}).on("hide.bs.modal", function (e) {
		    me.container.find(".modal-content").html("");
		    dialogContext.pop();
		});

	};

	Dialog.prototype = {
		show: function () {
			this.container.modal("show");
		},
		close: function (data) {
		    this.container.modal("hide");
		    if (this.ops.onClosed) {
		        this.ops.onClosed(data);
		    }
		}
	};
	Dialog.current = function () {
	    if (dialogContext.length > 0) {
	        return dialogContext[dialogContext.length - 1];
	    }
	    else {
	        return null;
	    }
	};

	window["ECommerce"]["Dialog"] = Dialog;

})();