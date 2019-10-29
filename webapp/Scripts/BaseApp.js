//var bootstrapButton = $.fn.button.noConflict(); // return $.fn.button to previously assigned value
//$.fn.bootstrapBtn = bootstrapButton;            // give $().bootstrapBtn the Bootstrap functionality

//--- Variable to use in Restrict Redirect Function
var checkUnsavedChanges = false;
//---


//--- Detect Browser Width & Append Sidebar
$(function () {
    var $window = $(window);

    function appendSidebarAccordingToWidth() {
        var windowsize = $window.width();
        if (windowsize <= 667) {
            //if the window is less than or equals to 667px wide then append in mobileMenu
            $("#sideBar-accordion").appendTo("#mobile-Module-Menu");
        }
        else {
            $("#sideBar-accordion").appendTo("#sideBar-Menu-Container");
        }
    }
    // Execute on load
    appendSidebarAccordingToWidth();

    // Bind event listener
    $(window).resize(appendSidebarAccordingToWidth);
});
//---

// Upcoming Events

var getUpcomingEvents = function () {
    $.get('/Secure/Schedular/Reader').done(function (html) {
        $('#mobile-upcomming-event-reader').html(html);
        $('#upcomming-event-reader').html(html);
        setTimeout(getUpcomingEvents, 15000);
    });
};

// End Upcoming Events

//--- Notification Reader
var getNotifications = function () {
    $.get('/Secure/AppNotification/Reader').done(function (html) {
        $('#mobile-notification-reader').html(html);
        $('#notification-reader').html(html);
        setTimeout(getNotifications, 15000);
    });
};
var getNotificationWarnings = function () {
    $.get('/Secure/AppNotification/WarningReader').done(function (html) {
        $('#mobile-notification-warning-reader').html(html);
        $('#notification-warning-reader').html(html);
        setTimeout(getNotificationWarnings, 15000);
    });
};

$(function () {
    getNotifications();
    getNotificationWarnings();
    getUpcomingEvents();

    $(document).on("click", ".notification-item", function (e) {
        e.preventDefault();
        var iId = $(this).data("id");
        var iHref = $(this).attr("href");

        $('#loading-mask').show();
        $.ajax({
            url: '/Secure/AppNotification/MarkRead/' + iId,
            cache: false,
            type: 'POST',
            success: function (data) {
                window.location.replace(iHref);
                //$('#loading-mask').fadeOut();
            },
            error: function (reponse) {
                alert("error : " + reponse);
            }
        });
    });

    $(document).on("click", ".markAllRead-notification", function (e) {
        e.preventDefault();
        $('#loading-mask').show();
        $.ajax({
            url: '/Secure/AppNotification/MarkReadAll',
            cache: false,
            type: 'POST',
            success: function (data) {
                getNotifications();
                $('#loading-mask').fadeOut();
            },
            error: function (reponse) {
                alert("error : " + reponse);
            }
        });
    });
});
//---

//--- Messages Reader
var getMessages = function () {
    $.get('/Secure/MessageCenter/Reader').done(function (html) {
        $('#mobile-message-reader').html(html);
        $('#message-reader').html(html);
        setTimeout(getMessages, 15000);
    });
};

$(function () {
    getMessages();

    $(document).on("click", ".message-item", function (e) {
        e.preventDefault();
        var iId = $(this).data("id");
        var iHref = $(this).attr("href");

        $('#loading-mask').show();
        $.ajax({
            url: '/Secure/MessageCenter/MarkRead/' + iId,
            cache: false,
            type: 'POST',
            success: function (data) {
                window.location.replace(iHref);
            },
            error: function (reponse) {
                alert("error : " + reponse);
            }
        });
    });

    $(document).on("click", ".markAllRead-message", function (e) {
        e.preventDefault();
        $('#loading-mask').show();
        $.ajax({
            url: '/Secure/MessageCenter/MarkReadAll',
            cache: false,
            type: 'POST',
            success: function (data) {
                getMessages();
            },
            error: function (reponse) {
                alert("error : " + reponse);
            }
        });
    });
});
//---

//--- Loading Mask
window.onload = function () { $('#loading-mask').fadeOut(500); };
$('a:not([href^="#"]):not([href^="javascript://"]):not([target="_blank"]):not([tabindex="0"]):not([href^="javascript:if(window.print)window.print()"])').click(function (e) {
    $('#loading-mask').show();
});

$('.navmenu-nav a:not([href^="#"])').click(function () {
    // Close offcanvas nav
    // offcanvas_selector is any valid css selector to get your offcanvas element
    $(".navmenu").offcanvas('hide');
});

$('button:submit').click(function (e) {
    var isValid = $(this).parents('form:first').valid();
    if (isValid) {
        $('#loading-mask').show();
    }
});
//---

//--- Sticky Module Navigation
var nav = $('#fix-moduleNav');
$(window).scroll(function () {
    if ($(this).scrollTop() > 40) {
        nav.addClass("module-Nav-scrolled");
        $('body').addClass("stickyNavPadding");
    }
    else {
        nav.removeClass("module-Nav-scrolled");
        $('body').removeClass("stickyNavPadding");
    }
});
//---

//--- Get Office Departments
function GetDepartmentsList(officeId, placeId, props) {
    $('#loading-mask').show();
    $.ajax({
        url: '/Secure/Office/GetDepartments',
        data: { Id: $(officeId).val() },
        cache: false,
        type: 'POST',
        success: function (data) {
            var markup = '<label>Department</label>';
            markup += '<select name="DepartmentId" id="DepartmentId" class="form-control input-sm" ' + props + '>';
            markup += '<option value="">-- No Department --</option>'
            for (var x = 0; x < data.length; x++) {
                markup += "<option value='" + data[x].Id + "'>" + data[x].Code + ' ' + data[x].Title + "</option>";
            }
            markup += '</select>';
            $(placeId).html(markup);


            var officeTimeZone = $("#OfficeTimeZone").val();
            var startDateInserted = $('#add-FromDateTime').val();
            var endDateInserted = $('#add-ToDateTime').val();

            $('#add-FromDateTime').val(moment(moment(startDateInserted).format("DD/MM/YYYY HH:mm A")).add(officeTimeZone, 'hours').format("DD/MM/YYYY HH:mm A"));
            $('#add-ToDateTime').val(moment(moment(endDateInserted).format("DD/MM/YYYY HH:mm A")).add(officeTimeZone, 'hours').format("DD/MM/YYYY HH:mm A"));

            $('#loading-mask').fadeOut();
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
};

function GetOfficeData(officeId) {
    $('#loading-mask').show();
    $.ajax({
        url: '/Secure/Office/GetOffice',
        data: { Id: $(officeId).val() },
        cache: false,
        type: 'POST',
        success: function (data) {
            $('#OfficeTimeZone').val(data.TimeZone);
            $('#loading-mask').fadeOut();
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
};
//---

//--- Validate Form
$('#frm').validate();
$('.frm').validate();
function carouselFormValidate(btn, carousel, activeStepNumber) {
    var frm = $(btn).closest("form");
    if (frm.valid()) {
        var carouselStepNumber = parseInt(activeStepNumber);
        $('#carousel-Step' + carouselStepNumber).addClass('active');
        if (carouselStepNumber >= 2) {
            $('#carousel-Step' + (carouselStepNumber - 1)).addClass('done-last');
            $('#carousel-Step' + (carouselStepNumber - 1)).removeClass('active');
            if (carouselStepNumber >= 3) {
                $('#carousel-Step' + (carouselStepNumber - 2)).addClass('done');
                $('#carousel-Step' + (carouselStepNumber - 2)).removeClass('done-last');
            }
        };
        $(carousel).carousel('next');
    }
    else {
    }
};
function carouselFormValidatePrev(carousel, activeStepNumber) {
    var carouselStepNumber = parseInt(activeStepNumber);
    $('#carousel-Step' + carouselStepNumber).removeClass('active');
    $('#carousel-Step' + carouselStepNumber).removeClass('done');
    $('#carousel-Step' + carouselStepNumber).removeClass('done-last');
    if (carouselStepNumber >= 2) {
        $('#carousel-Step' + (carouselStepNumber - 1)).removeClass('done');
        $('#carousel-Step' + (carouselStepNumber - 1)).removeClass('done-last');
        $('#carousel-Step' + (carouselStepNumber - 1)).addClass('active');
        if (carouselStepNumber >= 3) {
            $('#carousel-Step' + (carouselStepNumber - 2)).removeClass('done');
            $('#carousel-Step' + (carouselStepNumber - 2)).addClass('done-last');
        }
    };
    $(carousel).carousel('prev');
};
//--- 

//--- Alloy Editor
function makeAlloyEditor(Id) {
    var alloyEditor = AlloyEditor.editable(Id, {
        toolbars: {
            add: {
                buttons: ['camera', 'image', 'oembed', 'hline', 'table']
            },
            styles: {
                selections: [
                    {
                        name: 'text',
                        buttons: [{
                            name: 'styles',
                            cfg: {
                                styles: [
                                    {
                                        name: 'Head 1',
                                        style: { element: 'h1' }
                                    },
                                    {
                                        name: 'Head 2',
                                        style: { element: 'h2' }
                                    },
                                    {
                                        name: 'Big',
                                        style: { element: 'big' }
                                    },
                                    {
                                        name: 'Small',
                                        style: { element: 'small' }
                                    },
                                    {
                                        name: 'Code',
                                        style: { element: 'code' }
                                    }
                                ]
                            }
                        },
                        'Font',
                        'FontSize',
                        'bold',
                        'italic',
                        'underline',
                        'link',
                        'strike',
                        'subscript',
                        'superscript',
                        'h1',
                        'h2',
                        'ul',
                        'ol',
                        'quote',
                        'indentBlock',
                        'outdentBlock',
                        'paragraphLeft',
                        'paragraphCenter',
                        'paragraphRight',
                        'paragraphJustify',
                        {
                            name: 'twitter',
                            cfg: {
                                url: 'www.alloyeditor.com/features/twitter',
                                via: '@@alloyeditor'
                            }
                        },
                        'removeFormat',
                        'code',
                        'Sourcedialog'],
                        test: AlloyEditor.SelectionTest.text
                    },
                    {
                        name: 'image',
                        buttons: ['imageLeft', 'imageCenter', 'imageRight'],
                        test: AlloyEditor.SelectionTest.image
                    },
                    {
                        name: 'link',
                        buttons: ['linkEdit'],
                        test: AlloyEditor.SelectionTest.link
                    },
                    {
                        name: 'table',
                        buttons: ['tableCell', 'tableColumn', 'tableHeading', 'tableRow', 'tableRemove'],
                        test: AlloyEditor.SelectionTest.table
                    }
                ]
            }
        },
        extraPlugins: AlloyEditor.Core.ATTRS.extraPlugins.value + ',ae_uibridge,ae_buttonbridge,ae_richcombobridge,font,oembed,widget,dialog,sourcedialog,codemirror'
    });
    var nativeEditor = alloyEditor.get('nativeEditor');

    nativeEditor.on('change', function (event) { checkUnsavedChanges = true; });
};
//---

//--- Changing Bootstrap items
$(function () {
    $('a').tooltip({ trigger: 'hover' });
    $('button[type="submit"]').tooltip({ trigger: 'hover' });
    $('[data-toggle="popover"]').popover()

    //--- Change Checkbox to Bootstrap Switch
    $('.switch').bootstrapSwitch();
    $('.switch').on('switchChange.bootstrapSwitch', function (event, state) {
        var getAttr = $(this).attr('data-id');
        $("#" + getAttr).val(state);
    });

    $('.switch-Role').bootstrapSwitch();
    $('.switch-Role').on('switchChange.bootstrapSwitch', function (event, state) {
        var getAttr = $(this).attr('data-id');

        if (state) {
            $("#" + getAttr).addClass('role-Permission');
        }
        else {
            $("#" + getAttr).removeClass('role-Permission');
        }

        var rolePermission = '';
        $(".role-Permission").each(function () {
            var value = $(this).val();
            rolePermission += value + ',';
            document.getElementById("RolePermissions").value = rolePermission;
        });
    });
    //--- 

    $("#management-popover").popover({
        trigger: 'focus',
        placement: 'bottom',
        html: true,
        content: function () {
            return $('#management-popover-content').html();
        }
    });

    $("#management-popover-mobile").popover({
        trigger: 'click',
        placement: 'top',
        html: true,
        content: function () {
            return $('#management-popover-content-mobile').html();
        }
    });
});
//--- 

//--- Enabling checkbox to post their data
$('input:checkbox').click(function () {
    var getAttr = $(this).attr('value');
    if (getAttr == 'false') {
        $(this).attr('checked', true);
        $(this).attr('value', true);
    }
    if (getAttr == 'true') {
        $(this).removeAttr('checked');
        $(this).attr('value', false);
    }
});
//--- 

//--- Check All Checkboxes
function checkAll(e, checkboxClass) {
    var checkboxes = document.getElementsByClassName(checkboxClass);
    if (e.checked) {
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].type == 'checkbox' && checkboxes[i].style.display != 'none') {
                checkboxes[i].checked = true;
                $(checkboxes[i]).attr('checked', true);
            }
        }
    } else {
        for (var i = 0; i < checkboxes.length; i++) {
            //console.log(i)
            if (checkboxes[i].type == 'checkbox') {
                checkboxes[i].checked = false;
                $(checkboxes[i]).removeAttr('checked');
            }
        }
    }
};
//---

//--- Date Time Picker
$(".datePicker").datetimepicker({
    showTodayButton: true,
    //pickTimeButton: false,
    format: 'DD/MM/YYYY'
});
$(".timePicker").datetimepicker({
    format: 'hh:mm A'
});
$(".dateTimePicker").datetimepicker({
    showTodayButton: true,
    format: 'DD/MM/YYYY hh:mm A',
    sideBySide: true
});
//--- 

//--- Image Preview on selection
function previewImage(input, previewerId) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewerId).attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
};
//---

//--- Changing dropdown to SelectPicker
$(".selectpicker").selectpicker({ iconBase: 'fa', tickIcon: 'fa-check' });
//--- 

//--- General Input Masks
$(function () {
    $(".phoneMask").inputmask({
        "alias": "phone",
        "url": "../../../Scripts/Input Mask/phone-codes/phone-codes.js"
    });
    $(".numericMask").inputmask({ "alias": "numeric", "allowMinus": false, "allowPlus": false });
    $(".decimalMask").inputmask({
        "alias": "decimal",
        "groupSeparator": ",",
        "autoGroup": true,
        "allowMinus": false,
        "allowPlus": false,
        "digits": "4",
        "removeMaskOnSubmit": true
    });
    $(".urlMask").inputmask('Regex', { regex: "^(http\:\/\/|https\:\/\/)?([a-z0-9][a-z0-9\-]*\.)+[a-z0-9][a-z0-9\-]*$" });
    $(".emailMask").inputmask({ "alias": "email" });
    $(".zipCodeMask").inputmask({ "alias": "numeric", "allowMinus": false, "allowPlus": false });
    $(".alphanumericMask").inputmask('Regex', { regex: "[a-zA-Z0-9 ]{1,50}" });
    $(".alphanumericNoSpaceMask").inputmask('Regex', { regex: "[a-zA-Z0-9\-_]{1,50}" });
    $(".cnicMask").inputmask({
        "mask": "#####-#######-#",
    });
});
//--- 

//--- Jquery Datatable Filterable
function filterDataTable(dataTable, filterColumns, nonFilterColumns, defaultPageLength) {
    var dt = $(dataTable).DataTable({
        lengthMenu: [[-1, 10, 25, 50, 100], ["All", 10, 25, 50, 100]],
        columnDefs: [
            { targets: filterColumns, type: "string" },
            { targets: nonFilterColumns, sortable: false }
        ],
        order: [],
        "pageLength": defaultPageLength,
    });
    new $.fn.dataTable.CustomSearch(dt, {
        container: 'thead:after',
        fields: filterColumns
    });

    $(dataTable + ' tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            dt.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });
    $(dataTable).on('length.dt', function (e, settings, len) {
        $.ajax({
            type: "POST",
            url: "/Secure/Setting/SavePageLength",
            data: {
                length: len
            },
            error: function (xhr, status, error) {
                alert(error);
            },
            success: function (response) {
                //alert(response);
            }
        });
    });
};
//--- 


//--- Change Theme Function
function changeTheme(themeName) {
    $('#loading-mask').show();
    $.ajax({
        url: '/Secure/Account/ChangeTheme/?ThemeName=' + themeName,
        type: 'POST'
    }).always(function () {
        window.location.reload(true)
    });
};
//--- 

//--- Change Status Functions
function googleDisableService(title, msg, btnLabel, btnClass, targetUrl, recordId, serviceName) {
    bootbox.dialog({
        message: msg,
        title: title,
        buttons: {
            main: {
                label: "Cancel",
                className: "btn-default",
                callback: function () {
                    show: false;
                }
            },
            danger: {
                label: btnLabel,
                className: btnClass,
                callback: function () {
                    $.ajax({
                        url: targetUrl + '/' + recordId,
                        type: 'POST'
                    }).always(function () {
                        if (serviceName === "Webmaster") {
                            deleteWebmasterSite();
                        }
                        window.location.reload(true);
                    });
                }
            }
        }
    });
};

function customChangeStatus(title, msg, targetUrl, recordId) {
    bootbox.dialog({
        message: msg,
        title: title,
        buttons: {
            main: {
                label: "Cancel",
                className: "btn-default",
                callback: function () {
                    show: false;
                }
            },
            danger: {
                label: "Make Primary",
                className: "btn-warning",
                callback: function () {
                    $.ajax({
                        url: targetUrl + '/' + recordId,
                        type: 'POST'
                    }).always(function () {
                        window.location.reload(true)
                    });
                }
            }
        }
    });
};

function changeStatus(targetUrl, recordId) {
    bootbox.dialog({
        message: "Are you sure you want to make changes to this record ?",
        title: "Change Status",
        buttons: {
            main: {
                label: "Cancel",
                className: "btn-default",
                callback: function () {
                    show: false;
                }
            },
            danger: {
                label: "Change",
                className: "btn-warning",
                callback: function () {
                    $.ajax({
                        url: targetUrl + '/' + recordId,
                        type: 'POST'
                    }).always(function () {
                        window.location.reload(true)
                    });
                }
            }
        }
    });
};

function changeMultipleRecordsStatus(targetUrl, recordsClass, returnUrl) {
    var values = [];
    $(recordsClass + "[type=checkbox]:checked").each(function () {
        values.push($(this).val());
    });

    if (values.length === 0) {
        bootbox.dialog({
            message: "No record selected, please select a record first.",
            title: "Error",
            buttons: {
                main: {
                    label: "Close",
                    className: "btn-default",
                    callback: function () {
                        show: false;
                    }
                }
            }
        });
    }
    else {
        bootbox.dialog({
            message: "Are you sure you want to make changes to selected records ?",
            title: "Change Status",
            buttons: {
                main: {
                    label: "Cancel",
                    className: "btn-default",
                    callback: function () {
                        show: false;
                    }
                },
                danger: {
                    label: "Change",
                    className: "btn-warning",
                    callback: function () {
                        $.ajax({
                            url: targetUrl,
                            data: { ids: values.join() },
                            type: 'POST'
                        }).always(function () {
                            if (returnUrl == null) {
                                window.location.reload(true);
                            }
                            else {
                                window.location.replace(returnUrl);
                            }
                        });
                    }
                }
            }
        });
    }
};
//--- 

//--- Delete Record Functions
function deleteMultipleRecords(targetUrl, recordsClass, returnUrl) {
    var values = [];
    $(recordsClass + "[type=checkbox]:checked").each(function () {
        values.push($(this).val());
    });

    if (values.length === 0) {
        bootbox.dialog({
            message: "No record selected, please select a record first.",
            title: "Error",
            buttons: {
                main: {
                    label: "Close",
                    className: "btn-default",
                    callback: function () {
                        show: false;
                    }
                }
            }
        });
    }
    else {
        bootbox.dialog({
            message: "Are you sure you want to delete selected records ?",
            title: "Delete Records",
            buttons: {
                main: {
                    label: "Cancel",
                    className: "btn-default",
                    callback: function () {
                        show: false;
                    }
                },
                danger: {
                    label: "Delete",
                    className: "btn-danger",
                    callback: function () {
                        $.ajax({
                            url: targetUrl,
                            data: { ids: values.join() },
                            type: 'POST'
                        }).always(function () {
                            if (returnUrl == null) {
                                window.location.reload(true);
                            }
                            else {
                                window.location.replace(returnUrl);
                            }
                        });
                    }
                }
            }
        });
    }
};
function deleteRecord(targetUrl, recordId, returnUrl) {
    bootbox.dialog({
        message: "Are you sure you want to delete this record ?",
        title: "Delete Record",
        buttons: {
            main: {
                label: "Cancel",
                className: "btn-default",
                callback: function () {
                    show: false;
                }
            },
            danger: {
                label: "Delete",
                className: "btn-danger",
                callback: function () {
                    $.ajax({
                        url: targetUrl + '/' + recordId,
                        type: 'POST'
                    }).always(function () {
                        if (returnUrl == null) {
                            window.location.reload(true);
                        }
                        else {
                            window.location.replace(returnUrl);
                        }
                    });
                }
            }
        }
    });
};
function deleteDelableRecord(delableUrl, targetUrl, recordId) {
    $.ajax({
        type: "POST",
        url: delableUrl,
        data: {
            id: recordId,
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            if (response != true) {
                bootbox.dialog({
                    message: "Are you sure you want to delete this record ?",
                    title: "Delete Record",
                    buttons: {
                        main: {
                            label: "Cancel",
                            className: "btn-default",
                            callback: function () {
                                show: false;
                            }
                        },
                        danger: {
                            label: "Delete",
                            className: "btn-danger",
                            callback: function () {
                                $.ajax({
                                    url: targetUrl + '/' + recordId,
                                    type: 'POST'
                                }).always(function () {
                                    window.location.reload(true)
                                });
                            }
                        }
                    }
                });
            }
            else {
                bootbox.dialog({
                    message: "This record cannot be deleted. Either it's reference is used by another entries or it has some dependencies.",
                    title: "Message",
                    buttons: {
                        main: {
                            label: "Close",
                            className: "btn-default",
                            callback: function () {
                                show: false;
                            }
                        }
                    }
                });
            }
        }
    });
};
//--- 

//--- Iphone Browser back button issue
$(window).bind("pageshow", function (event) {
    if (event.originalEvent.persisted) {
        window.location.reload()
    }
});
//---

//Restrict Redirect Function//
$(function () {
    $(":input").change(function () { //trigers change in all input fields including text type
        checkUnsavedChanges = true;
    });

    function unloadPage() {
        if (checkUnsavedChanges == true) {
            $('#loading-mask').hide();
            return "You have unsaved changes on this page. Any unsaved changes will be discarded if you leave this page.";
        }
    };
    $(window).on('beforeunload', unloadPage);
    $("button[type='submit'], .forceRedirect").click(function () {
        checkUnsavedChanges = false;
        $(window).on('beforeunload', unloadPage);
    });
});
//END Restrict Redirect Function//

//Settings
$('input:radio[name="email:Header"]').change(
    function () {
        if ($(this).is(':checked') && $(this).val() == 'customHeader') {
            $('#divCustomHeader').show();
        }
        else {
            $('#divCustomHeader').hide();
        }
    });
$('input:radio[name="email:Footer"]').change(
    function () {
        if ($(this).is(':checked') && $(this).val() == 'customFooter') {
            $('#divCustomFooter').show();
        }
        else {
            $('#divCustomFooter').hide();
        }
    });

//Email Notificaitons Settings
$(document).ready(function () {
    $("div.bhoechie-tab-menu>div.list-group>a").click(function (e) {
        e.preventDefault();
        $(this).siblings('a.active').removeClass("active");
        $(this).addClass("active");
        var index = $(this).index();
        $("div.bhoechie-tab>div.bhoechie-tab-content").removeClass("active");
        $("div.bhoechie-tab>div.bhoechie-tab-content").eq(index).addClass("active");
    });
});

/*------------------- SMTP Settings -------------------*/
//Configuration
$('.btnConfiguration').on('click', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Config/_Configuration/" + Id;
    // load the url and show modal on success
    $("#smtp-Modal .modal-dialog").load(target, function () {
        $("#smtp-Modal").modal("show");
        $('#loading-mask').hide();
    });
});

//Module Configuration 
$('.btnModuleConfigure').on('click', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Config/_ConfigureModule/" + Id;
    // load the url and show modal on success
    $("#configure-Modal .modal-dialog").load(target, function () {
        $("#configure-Modal").modal("show");
        $('#loading-mask').hide();
    });
});
/*--------------------------------------*/
$(document).ready(function () {
    $("div.bhoechie-tab-menu>div.list-group>a").click(function (e) {
        e.preventDefault();
        $(this).siblings('a.active').removeClass("active");
        $(this).addClass("active");
        var index = $(this).index();
        $("div.bhoechie-tab>div.bhoechie-tab-content").removeClass("active");
        $("div.bhoechie-tab>div.bhoechie-tab-content").eq(index).addClass("active");
    });
});

/*---------------Notification Settings-----------------------*/
//Configure Notification
$(document.body).on('click', '.btn_configure', function (ev) {
    //ev.preventDefault();
    //debugger;
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Notification/_NotificationsRecord/" + Id;

    // load the url and show modal on success
    $("#NotifyConfigure-Modal .modal-dialog").load(target, function () {
        $("#NotifyConfigure-Modal").modal("show");
        $('#loading-mask').hide();
    });
});

$(document.body).on('change', 'input[type=radio][name=ApplyFor]', function (e) {
    e.preventDefault();
    var dataItem = this.value;
    if (dataItem == "Custom") {
        //alert(dataItem);
        $('#divUserRole').show();
    }
    else {
        $('#divUserRole').hide();
    }
});

/*---------------End Notification Settings-----------------------*/

/*---------------Access Control Management-----------------------*/
//Manage Access
$(document.body).on('click', '.managAccess', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/AccessControl/_ManageAccess/" + Id;

    // load the url and show modal on success
    $("#manageAccess-Modal .modal-dialog").load(target, function () {
        $("#manageAccess-Modal").modal("show");
        $('#loading-mask').hide();
    });
});

//Load Permissions of Single Role
$(document.body).on("click", ".roleTitle", function (ev) {
    ev.preventDefault();
    $('#loading-mask').show();
    $("#modalBody").html('');
    //debugger;
    var Id = $(this).attr("data-id");
    $.ajax({
        type: "GET",
        url: "/Secure/AccessControl/_RolePermissions",
        data: {
            Id: Id
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#modalBody").html(response);
            $('#loading-mask').fadeOut();
        }

    });
});

/*---------------End Access Control Management-----------------------*/

