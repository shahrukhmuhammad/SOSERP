$('.change-status').on('click', function () {
    $('#loading-mask').show();
    $.get("/Secure/TaskManagement/_UpdateTask", function (data) {
        $('#updateTask-Modal').html(data);
        $("#updateTask-Modal").modal("show");
        $('#loading-mask').hide();
    });
});



$('#priority-check').on('change', function () {
    if ($('#priority-check').is(':checked')) {
        $('.fa-tint').css("color", "#de1f1f");
    }
    else {
        $('.fa-tint').css("color", "#555555");
    }
});

$('#upload-options').on('click', function () {
    $('#upload-box').toggle(300);
});

$('#checklist-options').on('click', function () {
    $('#loading-mask').show();
    $('.checklist-box').toggle(300, function () {
        $.get("/Secure/TaskManagement/_Checklist", function (d) {
            $('#loading-mask').hide();
            $('.checklist-box').html(d);
        });
    });
});

$('#location-options').on('click', function () {
    $('#loading-mask').show();
    $('.location-box').toggle(300, function () {
        $.get("/Secure/TaskManagement/_LocationTracking", function (d) {
            $('#loading-mask').hide();
            $('.location-box').html(d);
        });
    });
});

$('#managedby-link').on('click', function () {
   $('#select-ManagedBy').selectize({
        plugins: ['remove_button'],
        openOnFocus: false
    }); 
    $('#managed-by').toggle(300);
});

$('#observer-link').on('click', function () {
    $('#select-Observers').selectize({
        plugins: ['remove_button'],
        openOnFocus: false
    });
    $('#observer-by').toggle(300);
});

$('#participants-link').on('click', function () {
    $('#select-Participants').selectize({
        plugins: ['remove_button'],
        openOnFocus: false
    });
    $('#participants').toggle(300);
});

//function SearchMore() {
//    $('#loading-mask').show();
//    $('#employeeList-Modal .modal-dialog').load("/Secure/TaskManagement/_EmployeesList", function () {
//        debugger;
//        $('#employeeList-Modal').modal("show");
//        $('#loading-mask').hide();
//        filterDataTable("#employeelist-dataTable", [1, 2, 3, 4, 5, 6], [0, 6]);
//    });
//}

function SuggestedBox() {
    $('#suggested-box').toggle(300, function () {
        $.get("/Secure/TaskManagement/_SuggestedList", function (d) {
            $('#browse-box').hide();
            $('#suggested-box').html(d);
            filterDataTable("#tasks-dataTable", [0, 1, 2, 3, 4]);
        });
    });
}

$('.crmsearch').on('click', function () {
    debugger;
    $('#managedBy-box-panel').html('');
    $('#managedBy-box-panel').css('display', 'none');
    $('#observer-box-panel').html('');
    $('#observer-box-panel').css('display', 'none');
    $('#crmSection-box-panel').html('');
    $('#crmSection-box-panel').css('display', 'none');
    $('#crm-box-panel').toggle(300, function () {
        $('#loading-mask-inner').show();
        $.get("/Secure/TaskManagement/_EmployeesList", function (d) {
            $('#loading-mask-inner').hide();
            $('#crm-box-panel').html(d);
            $('.box-section').val('AssignTo');
            SuggestedBox();
        });
    });
});

$('#managedbySearch').on('click', function () {
    debugger;
    $('#crm-box-panel').html('');
    $('#observer-box-panel').html('');
    $('#crmSection-box-panel').html('');
    $('#managedBy-box-panel').toggle(300, function () {
        $('#loading-mask-inner').show();
        $.get("/Secure/TaskManagement/_EmployeesList", function (d) {
            $('#loading-mask-inner').hide();
            $('#managedBy-box-panel').html(d);
            $('.box-section').val('ManagedBy');
            SuggestedBox();
        });
    });
});

$('#observerSearch').on('click', function () {
    $('#crm-box-panel').html('');
    $('#managedBy-box-panel').html('');
    $('#crmSection-box-panel').html('');
    $('#observer-box-panel').toggle(300, function () {
        $('#loading-mask-inner').show();
        $.get("/Secure/TaskManagement/_EmployeesList", function (d) {
            $('#loading-mask-inner').hide();
            $('#observer-box-panel').html(d);
            $('.box-section').val('Observers');
            SuggestedBox();
        });
    });
});

$('#crmSectionSearch').on('click', function () {
    $('#crm-box-panel').html('');
    $('#managedBy-box-panel').html('');
    $('#observer-box-panel').html('');
    $('#crmSection-box-panel').toggle(300, function () {
        $('#loading-mask-inner').show();
        $.get("/Secure/TaskManagement/_EmployeesList", function (d) {
            $('#loading-mask-inner').hide();
            $('#crmSection-box-panel').html(d);
            $('.box-section').val('CRMSection');
            SuggestedBox();
        });
    });
});

$('#create-event').on('click', function () {
    $('#create-event-box').toggle(300, function () {
        $.get("/Secure/Schedular/_CreateEventBox", function (d) {
            $('#create-event-box').html(d);
        });
    });
});