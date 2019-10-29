var parentId;
var dmsPath;
var getSelection;
var p;
var frameName;
var browseEventFunc;
var mode;

$(function () {
    var modaldmsbrowse = '<div class="modal" id="dmsBrowseDialog" tabindex="-1" data-backdrop="false" role="dialog" aria-labelledby="myLargeModalLabel" aria-hidden="true"> <div class="modal-dialog modal-lg"> <div class="modal-content"> <div class="modal-header clearfix"> <a href="#" data-dismiss="modal" aria-hidden="true" class="close">×</a> <h4 class="modal-title">Choose File</h4> </div> <div class="modal-body"> </div> </div> </div> </div>';
    var modaldmsrevisions = '<div class="modal" id="dmsRevisionsDialog" tabindex="-1" data-backdrop="false" role="dialog" aria-labelledby="myLargeModalLabel" aria-hidden="true"> <div class="modal-dialog"> <div class="modal-content"> <div class="modal-header clearfix"> <a href="#" data-dismiss="modal" aria-hidden="true" class="close">×</a> <h4 class="modal-title"></h4> </div> <div class="modal-body"></div> </div> </div> </div>';
    var modaldmsconfirm = '<div class="modal" id="dmsConfirmDialog" tabindex="-1" data-backdrop="false" role="dialog" aria-labelledby="myLargeModalLabel" aria-hidden="true"> <div class="modal-dialog"> <div class="modal-content"> <div class="modal-header clearfix"> <a href="#" data-dismiss="modal" aria-hidden="true" class="close">×</a> <h4 class="modal-title">Confirm</h4> </div> <div class="modal-body">Are you sure you want to delete <span></span> item(s)?</div> <div class="modal-footer"> <button type="button" data-dismiss="modal" class="btn btn-default" aria-hidden="true">Cancel</button> <button type="button" class="btn btn-success">Yes</button> </div> </div> </div> </div>';
    var modaldmstitle = '<div class="modal" id="dmsTitleDialog" data-backdrop="false" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel" aria-hidden="true"> <div class="modal-dialog modal-sm"> <div class="modal-content"> <div class="modal-header"> <a href="#" data-dismiss="modal" aria-hidden="true" class="close">×</a> <h4 class="modal-title"></h4> </div> <form class="form form-horizontal" id="dmsTitleForm"> <div class="modal-body"> <div class="control-group"> <div class="controls"> <div class="help-block"><small></small></div> <input required class="form-control" name="title" id="title"> <input type="hidden" name="itemId" id="itemId" /> </div> </div> </div> <div class="modal-footer"> <button type="button" data-dismiss="modal" class="btn btn-default" aria-hidden="true">Cancel</button> <button type="submit" class="btn btn-success">Save</button> </div> </form> </div> </div> </div>';
    var modaldmspreview = '<div class="modal" id="dmsPreviewDialog" tabindex="-1" data-backdrop="false" role="dialog" aria-labelledby="myLargeModalLabel" aria-hidden="true"> <div class="modal-dialog modal-lg"> <div class="modal-content"> <div class="modal-header clearfix"> <a href="#" data-dismiss="modal" aria-hidden="true" class="close">×</a> <strong class="modal-title pull-left"></strong> <div class="pull-right dms-preview-buttons"> <a href="javascript://" class="dms-preview-download btn btn-primary btn-xs"> <span class="fa fa-download"></span> Download </a> </div> </div> <div class="modal-body text-center dms-preview-wrapper"></div> </div> </div> </div>';

    $('body').append(modaldmsbrowse);
    $('body').append(modaldmsrevisions);
    $('body').append(modaldmspreview);
    $('body').append(modaldmsconfirm);
    $('body').append(modaldmstitle);

    //$('#dmsTitleForm').validate({
    //    rules: {
    //        title: {
    //            required: true,
    //            pattern: /^[-\w^&'@@{}[\],$=!#().%+~ ]+$/
    //        }
    //    },
    //    messages: {
    //        title: {
    //            pattern: "These signs are not allowed: \\ / : ? * < > \" |"
    //        }
    //    }
    //});

    $('#dmsConfirmDialog .btn-success').click(function () {
        $('#dmsConfirmDialog').modal('hide');
        document.getElementsByName(frameName)[0].src = dmsPath + '/delete/?id=' + getSelection + '&parentId=' + parentId + "&prnt=" + p + "&mode=" + mode;
    });

    $('#dmsTitleDialog form').on('submit', function (e) {
        e.preventDefault();
        var elm = $(this);
        if (elm.valid()) {
            $.post(dmsPath + '/title/?parentId=' + parentId, elm.serialize(), function (d) {
                if (d)
                    alert("An item with this name already exists.");
                else {
                    $('#dmsTitleDialog').modal('hide');
                    document.getElementsByName(frameName)[0].contentWindow.location.reload();
                }
            });
        }
    });

    $('#dmsTitleDialog').on('hide.bs.modal', function (event) {
        var modal = $(this);
        modal.find('.modal-body #title-error').hide();
    });

    $('#dmsPreviewDialog, #dmsRevisionsDialog, #dmsBrowseDialog').on('hidden.bs.modal', function (e) {
        $(this).find('.modal-body').empty();
    });
});

function setPermissions(id) {
    var m = $('.dms-doc-user:checked').map(function () { return $(this).data("user"); }).get().join(",");
    $.post(dmsPath + '/setpermissions/' + id + '/?ids=' + m + "&mode=" + mode, function () {
        $('#dmsRevisionsDialog').modal('hide');
    });
}

function restoreRevision(id) {
    document.getElementsByName(frameName)[0].src = dmsPath + '/restore/' + id + '/?prnt=' + p;
    $('#dmsRevisionsDialog').modal('hide');
}

function revalidateShareForm() {
    $('#DmsShareForm').validate({
        rules: {
            DmsEmailReceivers: {
                required: true,
                pattern: /^([A-Z0-9.%+-]+@[A-Z0-9.-]+.[A-Z]{2,6})*([,;][\s]*([A-Z0-9.%+-]+@[A-Z0-9.-]+.[A-Z]{2,6}))*$/i
            }
        },
        messages: {
            DmsEmailReceivers: {
                required: null,
                pattern: 'Invalid list of email addresses.'
            }
        }
    });
}

function dmsShare(t) {
    if ($('#DmsShareForm').valid()) {
        $('#DmsShareForm .btn-success').attr("disabled", true).html("<i class='fa fa-refresh fa-spin'></i> Sending...");
        $.post(dmsPath + '/share' + t + '/?ids=' + getSelection, $('#DmsShareForm').serialize(), function () {
            $('#dmsRevisionsDialog').modal('hide');
        });
    }
}

function resizeIframe(obj) {
    obj.style.height = (obj.contentWindow.document.body.scrollHeight > 0 ? obj.contentWindow.document.body.scrollHeight : 510) + 'px';
}

(function ($) {
    $.dmsBrowse = $.fn.dmsBrowse = function (i, elm) {
        browseEventFunc = elm;
        $('#dmsBrowseDialog .modal-body').html("<div class='help-block text-center'><i class='fa fa-refresh fa-spin fa-5x'></i></div>");
        $('#dmsBrowseDialog .modal-body').load('/dms/browse/' + i);
        $('#dmsBrowseDialog').modal('show');
    };
}(jQuery));