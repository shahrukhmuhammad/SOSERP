function getSelection() {
    return $('.selectitem:checked').map(function () { return $(this).data("id"); }).get().join(",");
}

function toolsAvailability() {
    $('#dmsTbDelete, #dmsTbDownload, #dmsTbEmailLinks, #dmsBrowseChooseBtn, #dmsTbShare').attr("disabled", getSelection() ? false : true);
}

function syncParentFrameVerbs() {
    parent.parentId = parentId;
    parent.p = p;
    parent.frameName = window.name;
}

$(function () {
    $('#dmsSearchForm').validate({
        messages: {
            q: {
                required: null
            }
        }
    });

    parent.dmsPath = dmsPath;
    parent.mode = mode;

    $('#dmsTbRefresh').click(function () {
        $('#dms-mask').fadeIn();
        window.location.reload();
    });

    $('[data-toggle="tooltip"]').tooltip({ container: 'body' });

    $('#selectall').click(function (event) {
        if (this.checked) {
            $('.selectitem').each(function () {
                this.checked = true;
            });
        } else {
            $('.selectitem').each(function () {
                this.checked = false;
            });
        }
        toolsAvailability();
    });

    $('.selectitem').click(function () {
        toolsAvailability();
    });

    $('#dmsTbDownload').click(function () {
        if ($('.selectitem:checked').length < 1) return;
        window.location.href = dmsPath + "/download/?id=" + getSelection() + "&dir=" + $('.breadcrumb li:last').text();
    });

    //$('#DmsTbUpload').click(function () {
    //    $('#dmsUploadDialog').modal('show');
    //});

    $('#docUploadDock').filedrop({
        url: dmsPath + '/drupload/?parentId=' + parentId,
        paramname: 'files',
        maxfiles: 5,
        queuefiles: 2,
        maxfilesize: 4096,
        error: function (err, file) {
            switch (err) {
                case 'BrowserNotSupported':
                    $('#docUploadDock').stop().fadeOut(300);
                    break;
                case 'FileTooLarge':
                    $('#docUploadDock').stop().fadeOut(300);
                    alert('Too heavy files.');
                    break;
            }
        },
        dragOver: function () {
            $('#docUploadDock').stop().fadeIn(150);
        },
        dragLeave: function () {
            $('#docUploadDock').stop().fadeOut(300);
        },
        docOver: function () {
            $('#docUploadDock').stop().fadeIn(150);
        },
        docLeave: function () {
            $('#docUploadDock').stop().fadeOut(300);
        },
        uploadStarted: function (i, file, len) {
            $('#docUploadDock .fa').removeClass('fa-cloud-upload').addClass('fa-refresh').addClass('fa-spin');
            $('#docUploadDock h4').text('Uploading...');
        },
        afterAll: function () {
            window.location.reload();
        },
    });

    $('#DmsTbUpload input[name="files"]').change(function () {
        $('#dms-mask').fadeIn();
        $(this).parent('form').submit();
    });

    $('.dms-title-btn').click(function () {
        syncParentFrameVerbs();
        parent.$('#dmsTitleDialog .modal-title').text($(this).data("title"));
        parent.$('#dmsTitleDialog .modal-body [name="title"]').val($(this).data("text"));
        parent.$('#dmsTitleDialog .modal-body [name="itemId"]').val($(this).data("id"));
        parent.$('#dmsTitleDialog .modal-body .help-block small').text($(this).data("hint"));
        parent.$('#dmsTitleDialog').modal('show');
        parent.$("#dmsTitleForm #title").focus();
    });

    $('.dms-item').click(function () {
        parent.$('#dmsPreviewDialog .modal-body').html("<div style='margin-top:200px' class='help-block'><i class='fa fa-refresh fa-spin fa-5x'></i></div>");
        var iType = $(this).data("type");
        var iId = $(this).data("id");
        var iFilename = $(this).html();
        switch (iType) {
            case "Folder":
                window.location.href = dmsPath + "/index/" + iId + "?p=" + p + "&mode=" + mode;
                return;
                break;
            case "jpg": case "jpeg": case "gif": case "png": case "bmp":
                parent.$('#dmsPreviewDialog .modal-body').html("<img src='" + dmsPath + "/get/" + iId + "' />");
                break;
            case "pdf":
                parent.$('#dmsPreviewDialog .modal-body').html("<iframe src='" + dmsPath + "/get/" + iId + "' frameborder='0'></iframe>");
                break;
            case "txt": case "xml": case "js": case "html": case "css": case "csv": case "aspx": case "sql": case "php": case "cshtml":
                $.get(dmsPath + "/getcontent/" + iId, function (d) {
                    parent.$('#dmsPreviewDialog .modal-body').html("<pre>" + d + "</pre>");
                });
                break;
            case "doc": case "docx": case "xls": case "xlsx": case "ppt": case "pptx": case "pps": case "rtf":
                parent.$('#dmsPreviewDialog .modal-body').html("<iframe src='https://drive.google.com/viewerng/viewer?embedded=true&chrome=false&url=" + dmsPath + "/get/" + iId + "' frameborder='0'></iframe>");
                break;
            case "mp3": case "ogg": case "wav":
                parent.$('#dmsPreviewDialog .modal-body').html("<div style='margin-top:150px' class='help-block'><i class='fa fa-file-audio-o fa-5x'></i><br><br><br><audio controls autoplay><source src='" + dmsPath + "/get/" + iId + "'></audio></div>");
                break;
            case "mp4": case "3gp": case "flv": case "webm":
                parent.$('#dmsPreviewDialog .modal-body').html("<a href='" + dmsPath + "/get/" + iId + "' id='dmsPlayer'></a>");
                parent.$f("dmsPlayer", {
                    src: "/Scripts/flowplayer/flowplayer-3.2.18.swf",
                    wmode: "transparent"
                });
                break;
            default:
                parent.$('#dmsPreviewDialog .modal-body').html("<div style='margin-top:150px' class='help-block'><span class='fa fa-frown-o fa-5x'></span><p>No preview available.</p><a href='" + dmsPath + "/download/" + iId + "' class='btn btn-primary btn-lg'><span class='fa fa-download'></span> Download</a></div>");
                break;
        }
        parent.$('#dmsPreviewDialog .dms-preview-download').attr("href", dmsPath + "/download/" + iId);
        parent.$('#dmsPreviewDialog .modal-title').html(iFilename);
        parent.$('#dmsPreviewDialog').modal('show');
    });

    $('.dms-browse-item').click(function () {
        var iType = $(this).data("type");
        var iId = $(this).data("id");
        var iFilename = $(this).html();
        switch (iType) {
            case "Folder":
                window.location.href = dmsPath + "/index/" + iId + "?p=" + p + "&mode=Browse";
                return;
                break;
            default:
                $('.selectitem').each(function () { $(this).prop('checked', false); });
                $(this).parent().parent().find('.selectitem').prop('checked', true);
                toolsAvailability();
                $('.dms-browse-item').css({ fontWeight: 'normal' });
                $(this).css({ fontWeight: 'bold' });
                break;
        }
    });

    $('#dmsBrowseChooseBtn').click(function () {
        var iId = getSelection();
        var iFileName = $('.dms-browse-item[data-id="' + iId + '"]').text().trim();
        parent.browseEventFunc({ Id: iId, FileName: iFileName });
        parent.$('#dmsBrowseDialog').modal('hide');
    });

    $('#dmsTbDelete').click(function () {
        syncParentFrameVerbs();
        parent.$('#dmsConfirmDialog .modal-body span').text($('.selectitem:checked').length);
        parent.$('#dmsConfirmDialog').modal('show');
        parent.getSelection = getSelection();
    });

    $('#dms-close-modal').click(function () {
        parent.$('#dmsBrowseDialog').modal('hide');
    });

    $('.dms-revisions-btn').click(function () {
        syncParentFrameVerbs();
        var iId = $(this).data("id");
        var iFilename = $(this).data('filename');
        parent.$('#dmsRevisionsDialog .modal-body').html("<div class='help-block text-center'><i class='fa fa-refresh fa-spin fa-5x'></i></div>");
        parent.$('#dmsRevisionsDialog .modal-body').load(dmsPath + '/getrevisions/' + iId);
        parent.$('#dmsRevisionsDialog .modal-title').text(iFilename + " - Revisions");
        parent.$('#dmsRevisionsDialog').modal('show');
    });

    $('.dms-permissions-btn').click(function () {
        syncParentFrameVerbs();
        var iId = $(this).data("id");
        var iFilename = $(this).data('filename');
        parent.$('#dmsRevisionsDialog .modal-body').html("<div class='help-block text-center'><i class='fa fa-refresh fa-spin fa-5x'></i></div>");
        parent.$('#dmsRevisionsDialog .modal-body').load(dmsPath + '/getpermissions/' + iId);
        parent.$('#dmsRevisionsDialog .modal-title').text(iFilename + " - Permissions");
        parent.$('#dmsRevisionsDialog').modal('show');
    });

    $('.dmsTbShare').click(function () {
        if ($('.selectitem:checked').length < 1) return;

        var iType = $(this).data("type");

        syncParentFrameVerbs();
        parent.getSelection = getSelection();
        parent.$('#dmsRevisionsDialog .modal-title').text("Email " + iType);
        var markup = "<form id='DmsShareForm' onsubmit='return false;'>";
        markup += "<div class='form-group'><input type='text' class='form-control' id='DmsEmailReceivers' name='DmsEmailReceivers' placeholder='Type email addresses comma seperated' required></div>";
        markup += "<div class='form-group'><textarea class='form-control' id='DmsEmailMessage' name='DmsEmailMessage' placeholder='Type your message...'></textarea></div>";
        markup += "<div class='text-center'><button class='btn btn-success' onclick='dmsShare(\"" + iType + "\")'><i class='fa fa-envelope'></i> Send</button></div>";
        markup += "</form>";
        parent.$('#dmsRevisionsDialog .modal-body').html(markup);
        parent.revalidateShareForm();
        parent.$('#dmsRevisionsDialog').modal('show');
        parent.$("#DmsShareForm #DmsEmailReceivers").focus();
    });

    $('#dms-mask').fadeOut();
});

function reloadDMS() {
    $('#dms-mask').fadeIn();
    window.location.reload();
};