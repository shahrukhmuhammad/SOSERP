$(document).ready(function () {
    $('#payperiod-closing').css('display', 'none');
    $('#firsthalf-closing-date').css('display', 'none');
    $('#closemonthly').css('display', 'none');

    var dataVal = $("#timesheetPaidTimeoff").attr("data-val");
    if (dataVal == "true") {
        $('#paid-time-off').css('display', 'block');
    }
    else {
        $('#paid-time-off').css('display', 'none');
    }

    var customdataVal = $("#employeeCustomDataFields").attr("data-val");
    if (customdataVal == "true") {
        $('#custom-data-fields').css('display', 'block');
    }
    else {
        $('#custom-data-fields').css('display', 'none');
    }

    var employeeInsurance = $("#employeeInsurance").attr("data-val");
    if (employeeInsurance == "true") {
        $('#custom-insurance-fields').css('display', 'block');
    }
    else {
        $('#custom-insurance-fields').css('display', 'none');
    }

    var employeeDeductTaxes = $("#employeeDeductTaxes").attr("data-val");
    if (employeeDeductTaxes == "true") {
        $('#custom-taxes-fields').css('display', 'block');
    }
    else {
        $('#custom-taxes-fields').css('display', 'none');
    }

    var employeeBenefitsType = $("#employeeBenefitsType").attr("data-val");
    if (employeeBenefitsType === "true") {
        $('#custom-benefitstype-fields').css('display', 'block');
    }
    else {
        $('#custom-benefitstype-fields').css('display', 'none');
    }

    var employeeBonus = $("#employeeBonus").attr("data-val");
    if (employeeBonus === "true") {
        $('#custom-employeeBonus-fields').css('display', 'block');
    }
    else {
        $('#custom-employeeBonus-fields').css('display', 'none');
    }

    var employeeCertificates = $("#employeeCertificates").attr("data-val");
    if (employeeCertificates === "true") {
        $('#custom-employeeCertificates-fields').css('display', 'block');
    }
    else {
        $('#custom-employeeCertificates-fields').css('display', 'none');
    }

    //var FLSAdataVal = $("#payrollIsFLSAStandard").attr("data-val");
    //if (FLSAdataVal == "true") {
    //    $('.employeement-terms').css('display', 'block');
    //}
    //else {
    //    $('.employeement-terms').css('display', 'none');
    //}

});
$("body").on("click", ".ptoRecord-Add", function () {
    var uuid = guid();

    var markup = '<tr class="row-Container" data-id="' + uuid + '">';
    markup += '<td><input type="text" class="form-control input-sm ptoTitle" placeholder="PTO Code Title" value="" required /></td>';
    markup += '<td></td>';
    markup += '<td class="text-right"><a href="javascript://" class="btn btn-xs btn-success saveCF"><i class="fa fa-check"></i></a><a href="javascript://" class="btn btn-xs btn-danger remCF"><i class="fa fa-times"></i></a></td>';
    markup += '</tr>';
    $("#ptoRecord-Table tbody").append(markup);
});

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
};

$("#timesheetGeolocation").click(function (ev) {
    var dataVal = $("#timesheetGeolocation").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/SaveGeolocationSettings",
        data: {
            Geolocation: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#timesheetGeolocation").attr("data-val", dataVal);
        }
    });
});

$("#timesheetApproval").click(function (ev) {
    var dataVal = $("#timesheetApproval").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/SaveApprovalSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#timesheetApproval").attr("data-val", dataVal);
        }
    });
});

$("body").on("click", ".updateIsAssignedToAll", function () {
    var itemId = $(this).attr("data-id");

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/UpdateAssignedPtoRecord",
        data: {
            Id: itemId
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            // load the url on success
            $("#PtoCodes-Container").load("/Secure/Employee/_AllPtoCodes", function () { });
        }
    });
});

$("body").on("click", ".saveCF", function () {
    var closestRow = $(this).closest('.row-Container');
    var itemTitle = closestRow.find('.ptoTitle').val();

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/AddPtoRecord",
        data: {
            Title: itemTitle
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            // load the url on success
            $("#PtoCodes-Container").load("/Secure/Employee/_AllPtoCodes", function () { });
        }
    });
});

$(".ptoAssignees").click(function (ev) {
    $('#loading-mask').show();
    ev.preventDefault();
    var dataId = $(this).attr("data-id");
    var target = "";
    if (dataId === "") {
        target = "/Secure/Employee/_PtoAssignees/";
    }
    else {
        target = "/Secure/Employee/_PtoAssignees/" + dataId;
    }


    // load the url and show modal on success
    $("#ptoAssigneesN-Modal .modal-dialog").load(target, function () {
        $("#ptoAssigneesN-Modal").modal("show");
        filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

$("body").on("click", ".remCF", function () {
    var item = $(this);

    bootbox.dialog({
        message: "Are you sure you want to delete this record, It may contain assignees which will also be deleted ?",
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
                    item.parent().parent().remove();
                    var itemId = item.attr("data-id");

                    $.ajax({
                        type: "POST",
                        url: "/Secure/Employee/DeletePtoRecord",
                        data: {
                            Id: itemId,
                        },
                        error: function (xhr, status, error) {
                            alert(error);
                        },
                        success: function (response) {
                            $("#PtoCodes-Container").load("/Secure/Employee/_AllPtoCodes", function () { });
                        }
                    });
                }
            }
        }
    });
});

$('#overtimeruleBtn').on('click', function () {
    if ($('#overtimeMethod').val() == '' || $('#overtimeWeekly').val() == '' || $('#overtimeDaily').val() == '') {
        bootbox.alert("Please fill all fields");
        return false;
    }
    else {
        //$.post("~/Secure/Employee/SaveSettings", )
        $.ajax({
            type: "POST",
            url: "/Secure/Employee/SaveSettings",
            data: {
                Id: itemId,
            },
            error: function (xhr, status, error) {
                alert(error);
            },
            success: function (response) {
                $("#PtoCodes-Container").load("/Secure/Employee/_AllPtoCodes", function () { });
            }
        })
        //bootbox.alert("Yes i'm succeeded");
    }
});

$("#standardOvertimeRules").click(function (ev) {
    var dataVal = $("#standardOvertimeRules").attr("data-val");
    //alert(dataVal);
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/SaveIsOvertimeRuleSettings",
        data: {
            OvertimeRule: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#standardOvertimeRules").attr("data-val", dataVal);
        }
    });
});

$("#timesheetPaidTimeoff").click(function (ev) {
    var dataVal = $("#timesheetPaidTimeoff").attr("data-val");
    //alert(dataVal);
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#paid-time-off').fadeIn().css('display', 'block');
    }
    else {
        $('#paid-time-off').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/SaveTimesheetPaidTimeOff",
        data: {
            PaidTimeOff: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#standardOvertimeRules").attr("data-val", dataVal);
        }
    });
});

$('#WeekStartDay').on('change', function () {
    $('#loading-mask').show();
    if ($(this).val() == "Weekly" || $(this).val() == "Bi-Weekly" || $(this).val() == "Every-4-Weeks") {
        $('#payperiod-closing').css('display', 'block');
        $('#firsthalf-closing-date').css('display', 'none');
        $('#closemonthly').css('display', 'none');
        $('#loading-mask').hide();
        return false;
    }
    else if ($(this).val() == "Semi-Monthly") {
        $('#payperiod-closing').css('display', 'none');
        $('#firsthalf-closing-date').css('display', 'block');
        $('#closemonthly').css('display', 'none');
        $('#loading-mask').hide();
        return false;
    }
    else if ($(this).val() == "Monthly") {
        $('#payperiod-closing').css('display', 'none');
        $('#firsthalf-closing-date').css('display', 'none');
        $('#closemonthly').css('display', 'block');
        $('#loading-mask').hide();
        return false;
    }
    else {
        $('#loading-mask').hide();
    }
    //else if ($(this).val() == "Semi-Monthly")
    //{
    //    $('#firsthalf-closing-date').css('display', 'none');
    //}
});

$('.customize-overtimeSetup').on('click', function () {
    var target = "";
    target = "/Secure/Employee/_OvertimeRules";


    // load the url and show modal on success
    $("#overtimeRules-Modal .modal-dialog").load(target, function () {
        $("#overtimeRules-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});




//--- Dynamic Sections
$("body").on("click", ".dynamic-Section-Add", function () {
    var uuid = guid();

    var markup = '<div class="row tb-padding"><div class="col-sm-12 sectionContainer" data-id="' + uuid + '">';
    markup += '<div class="page-header"><input type="text" class="sectionName" placeholder="Section Title" value="New Section" required /><div class="btn-group btn-group-xs" role="group"><a href="javascript://" class="text-muted remCF"><i class="fa fa-times"></i></a></div></div>';
    markup += '<div class="page-body"><div class="btn-group btn-group-xs" style="position:absolute; bottom:-30px; left:170px;" role="group"><div class="btn-group btn-group-xs" role="group"><button type="button" class="btn btn-link dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="fa fa-plus"></i> Add New field</button><ul class="dropdown-menu" style="width:auto;"><li><a href="javascript://" class="dynamic-Field-AddString">String</a></li><li><a href="javascript://" class="dynamic-Field-AddDate">Date</a></li><li><a href="javascript://" class="dynamic-Field-AddCheckbox">Yes / No</a></li><li><a href="javascript://" class="dynamic-Field-AddNumber">Number</a></li></ul></div></div></div>';
    markup += '</div></div>';
    $("#dynamic-Sections").append(markup);
    $.ajax({
        type: "POST",
        url: "/Secure/Employee/AddSection",
        data: {
            Id: uuid,
            Section: "New Section",
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });
});
$("#dynamic-Sections").on('click', '.remCF', function () {
    $(this).parent().parent().parent().parent().remove();

    var itemId = $(this).closest('.sectionContainer').attr("data-id");

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/DeleteSection",
        data: {
            Id: itemId,
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });
});
//---

//--- Dynamic Fields
$("body").on("click", ".dynamic-Field-AddString", function () {
    var uuid = guid();
    var markup = '<div class="row form-group fieldContainer" data-id="' + uuid + '" data-type="string"><div class="col-sm-2 no-padding"><input type="text" class="form-control input-sm text-right transparent-input fieldName" value="New String Field" /></div><div class="col-sm-4"></div><div class="col-sm-1"><a href="javascript://" class="text-muted remField"><i class="fa fa-times"></i></a></div></div>';
    $(this).closest('.page-body').append(markup);

    var parentId = $(this).closest('.sectionContainer').attr("data-id");

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/AddField",
        data: {
            ParentId: parentId,
            Id: uuid,
            FieldType: "string",
            FieldName: "New String Field"
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });
});
$("body").on("click", ".dynamic-Field-AddDate", function () {
    var uuid = guid();
    var markup = '<div class="row form-group fieldContainer" data-id="' + uuid + '" data-type="date"><div class="col-sm-2 no-padding"><input type="text" class="form-control input-sm text-right transparent-input fieldName" value="New Date Field" /></div><div class="col-sm-4"></div><div class="col-sm-1"><a href="javascript://" class="text-muted remField"><i class="fa fa-times"></i></a></div></div>';
    $(this).closest('.page-body').append(markup);

    var parentId = $(this).closest('.sectionContainer').attr("data-id");

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/AddField",
        data: {
            ParentId: parentId,
            Id: uuid,
            FieldType: "date",
            FieldName: "New Date Field"
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });
});
$("body").on("click", ".dynamic-Field-AddCheckbox", function () {
    var uuid = guid();
    var markup = '<div class="row form-group fieldContainer" data-id="' + uuid + '" data-type="checkbox"><div class="col-sm-2 no-padding"><input type="text" class="form-control input-sm text-right transparent-input fieldName" value="New Checkbox Field" /></div><div class="col-sm-4"></div><div class="col-sm-1"><a href="javascript://" class="text-muted remField"><i class="fa fa-times"></i></a></div></div>';
    $(this).closest('.page-body').append(markup);

    var parentId = $(this).closest('.sectionContainer').attr("data-id");

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/AddField",
        data: {
            ParentId: parentId,
            Id: uuid,
            FieldType: "checkbox",
            FieldName: "New Checkbox Field"
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });
});
$("body").on("click", ".dynamic-Field-AddNumber", function () {
    var uuid = guid();
    var markup = '<div class="row form-group fieldContainer" data-id="' + uuid + '" data-type="number"><div class="col-sm-2 no-padding"><input type="text" class="form-control input-sm text-right transparent-input fieldName" value="New Number Field" /></div><div class="col-sm-4"></div><div class="col-sm-1"><a href="javascript://" class="text-muted remField"><i class="fa fa-times"></i></a></div></div>';
    $(this).closest('.page-body').append(markup);

    var parentId = $(this).closest('.sectionContainer').attr("data-id");

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/AddField",
        data: {
            ParentId: parentId,
            Id: uuid,
            FieldType: "number",
            FieldName: "New Number Field"
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });
});

$("#dynamic-Sections").on('click', '.remField', function () {
    $(this).parent().parent().remove();

    var itemId = $(this).closest('.fieldContainer').attr("data-id");

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/DeleteField",
        data: {
            Id: itemId,
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });

});
//---

//--- Ajax Update Section On Leave
$("body").on("focusout", ".sectionName", function () {
    var itemId = $(this).closest('.sectionContainer').attr("data-id");
    var itemName = $(this).val();

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/UpdateSection",
        data: {
            Id: itemId,
            Section: itemName,
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });
});
//---

//--- Ajax Update Field On Leave
$("body").on("focusout", ".fieldName", function () {
    var itemId = $(this).closest('.fieldContainer').attr("data-id");
    var itemType = $(this).closest('.fieldContainer').attr("data-type");
    var itemName = $(this).val();

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/UpdateField",
        data: {
            Id: itemId,
            FieldType: itemType,
            FieldName: itemName,
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            //alert(response);
        }
    });
});
//---


$("#employeeCustomDataFields").click(function (ev) {
    var dataVal = $("#employeeCustomDataFields").attr("data-val");
    //alert(dataVal);
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#custom-data-fields').fadeIn().css('display', 'block');
    }
    else {
        $('#custom-data-fields').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/SaveCustomDataFields",
        data: {
            CustomFields: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#standardOvertimeRules").attr("data-val", dataVal);
        }
    });
});

$("#payrollIsFLSAStandard").click(function (ev) {
    var dataVal = $("#payrollIsFLSAStandard").attr("data-val");
    if (dataVal === "false") { dataVal = "true"; }
    else { dataVal = "false"; }

    //if ($(this).val() === "false") { $('.employeement-terms').fadeIn().css('display', 'block'); }
    //else { $('.employeement-terms').fadeOut(600); }

    $.ajax({
        type: "POST",
        url: "/Secure/Employee/SaveIsFLSAStandardSetting",
        data: { FlsaSetting: dataVal },
        error: function (xhr, status, error) { alert(error); },
        success: function (response) {
            $("#payrollIsFLSAStandard").attr("data-val", dataVal);
        }
    });
});

$('.overtime-rates').on('click', function () {
    var target = "";
    target = "/Secure/Employee/_OvertimeRates";


    // load the url and show modal on success
    $("#overtimeRates-Modal .modal-dialog").load(target, function () {
        $("#overtimeRates-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

//Task Manager Settings

$("#employeeTaskCreation").click(function (ev) {
    var dataVal = $("#employeeTaskCreation").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Setting/SaveEmployeeTaskCreationSettings",
        data: {
            TaskCreation: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#employeeTaskCreation").attr("data-val", dataVal);
        }
    });
});

$("#employeeAcceptNRejectTask").click(function (ev) {
    var dataVal = $("#employeeAcceptNRejectTask").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Setting/SaveEmployeeAcceptNRejectTask",
        data: {
            AcceptNReject: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#employeeAcceptNRejectTask").attr("data-val", dataVal);
        }
    });
});

//End Task Manager Settings

// Payroll Settings

$("#employeeInsurance").click(function (ev) {
    var dataVal = $("#employeeInsurance").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#custom-insurance-fields').fadeIn().css('display', 'block');
    }
    else {
        $('#custom-insurance-fields').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Setting/SaveEmployeeInsurance",
        data: {
            EmployeeInsurance: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#employeeInsurance").attr("data-val", dataVal);
        }
    });
});

$(".insurance-Add").on("click", function (w) {
    $('.insurance-Save').css('display', 'block');
    var i = $('#insurance-records tr').length;
    var uuid = guid();
    var markup = '<tr class="row-Container" data-id="' + uuid + '"><td colspan="8"><div class="col-sm-12"><a href="javascript://" class="btn btn-xs btn-danger delInsuranceSection pull-right" style="display:none"><i class="fa fa-times"></i></a></div>'
    markup += '<div class="form-group col-sm-4"><label><strong>Insurance Name</strong></label><input type="text" name="Insurance['+ i +'].Title" class="form-control input-sm" required /></div>'
    markup += '<div class="form-group col-sm-4"><label><strong>Insurance Type</strong></label><select required name="Insurance[' + i + '].InsuranceType" class="form-control input-sm"><option value="">-- Select --</option><option value="WorkersCompensation">Workers Compensation</option><option value="Disability">Disability</option><option value="HealthInsurance">Health Insurance</option><option value="LifeInsurance">Life Insurance</option><option value="Other">Other</option></select></div>';
    markup += '<div class="form-group col-sm-4 type-box" style="display:none"><label><strong>Type</strong></label><input type="text" class="form-control input-sm" /></div>';
    markup += '<div class="form-group col-sm-4"><label><strong>Plan</strong></label><input type="text" name="Insurance[' + i + '].InsurancePlan" required class="form-control input-sm" /></div>';
    markup += '<div class="form-group col-sm-6"><label><strong>Policy Number</strong></label><input type="text" name="Insurance[' + i + '].PolicyNumber" class="form-control input-sm" required /></div>';
    markup += '<div class="form-group col-sm-6"><label><strong>Group Insurance Number</strong></label><input type="text" name="Insurance[' + i + '].GroupInsuranceNumber" class="form-control input-sm" required /></div>';
    markup += '<div class="form-group col-sm-4"><label class="col-sm-4 no-padding"><strong>Coverage</strong></label><div class="col-sm-8 no-padding"> <input type="number" required max="100" min="0" name="Insurance[' + i + '].Coverage" class="form-control input-sm" /> </div></div>';
    markup += '<div class="form-group col-sm-4"><label class="col-sm-4 no-padding"><strong>Premium</strong></label><div class="col-sm-8 no-padding"><input type="number" name="Insurance[' + i + '].Premium" max="100" min="0" class="form-control input-sm" /></div></div>';
    markup += '<div class="form-group col-sm-4"><label class="col-sm-6 no-padding"><strong>% Paid by Employee</strong></label><div class="col-sm-6 no-padding"><input name="Insurance[' + i + '].PercentageByEmployee" type="number" max="100" min="0" class="form-control input-sm" /></div></div>';
    markup += '<div class="form-group"><label class="col-sm-12"><strong>Apply On</strong></label><div class="col-sm-4 col-sm-offset-3"> <label><input type="checkbox" value="true" /> To All</label> <br /> <label><input type="checkbox" id="all-check' + i + '" value="true" name="Insurance[' + i + '].FullTime" /> Full Time - 40 Hour per week</label> <label><input value="true" type="checkbox" name="Insurance[' + i + '].PartTime" /> Part time - Less then 40 hours per week</label> <label><input type="checkbox" value="true" name="Insurance[' + i + '].VaryingWeekly" /> Varying weekly hours (no fixed hours)</label> <label><input value="true" type="checkbox" name="Insurance[' + i + '].Contractor" /> Contractor </label> <br /> <label><input type="checkbox" value="true" name="Insurance[' + i + '].Freelance" /> Freelance </label> </div> </div></td></tr>';
    $("#insuranceRecord-Table tbody").append(markup);

    $('.row-Container').mouseover(function () {
        $('#insurance-records .delInsuranceSection').css('display', 'block');
    }).mouseout(function () {
        $('#insurance-records .delInsuranceSection').hide();
    });
});

//function InsuranceType() {
//    debugger;
//    var ab = $(this).val();
//    alert($(this).val());
//    if ($(this).val() == "Other") {
//        $('.type-box').show();
//    }
//    else {
//        $('.type-box').hide();
//    }
//}

//$('.insurance-type').on('change', function () {
//    alert($(this).val());
//    if ($(this).val() == "Other") {
//        $('.type-box').show();
//    }
//    else {
//        $('.type-box').hide();
//    }
//});

$("body").on("click", ".delInsuranceSection", function () {
    var item = $(this);

    bootbox.dialog({
        message: "Are you sure you want to delete this record?",
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
                    item.parent().parent().parent().remove();
                    var itemId = item.attr("data-id");

                    $.ajax({
                        type: "POST",
                        url: "/Secure/Employee/DeleteInsuranceRecord",
                        data: {
                            Id: itemId,
                        },
                        error: function (xhr, status, error) {
                            alert(error);
                        },
                        success: function (response) {
                            $("#Insurance-Container").load("/Secure/Employee/_EmployeeInsurance", function () { });
                        }
                    });
                }
            }
        }
    });
});

// Tax Types
$("#employeeDeductTaxes").click(function (ev) {
    var dataVal = $("#employeeDeductTaxes").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#custom-taxes-fields').fadeIn().css('display', 'block');
    }
    else {
        $('#custom-taxes-fields').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Setting/SaveDeductTaxTypes",
        data: {
            DeductTaxes: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#employeeDeductTaxes").attr("data-val", dataVal);
        }
    });
});

$(".taxType-Add").on("click", function (w) {
    $('.taxType-Save').css('display', 'block');
    var i = $('#DeductTaxes-records tr').length;
    var uuid = guid();
    var markup = '<tr class="row-Container" data-id="' + uuid + '"><td colspan="8"><div class="col-sm-12"><a href="javascript://" class="btn btn-xs btn-danger delDeductTaxesSection pull-right" style="display:none"><i class="fa fa-times"></i></a></div>'
    markup += '<div class="form-group col-sm-6"><label><strong>Tax Name</strong></label><input type="text" name="tTypes['+ i +'].Taxname" class="form-control input-sm" required /></div>'
    markup += '<div class="form-group col-sm-6"><label><strong>% Deduction</strong></label><input name="tTypes[' + i + '].PercentageDeduction" type="number" max="100" min="0" class="form-control input-sm" /></div>';
    markup += '<div class="form-group"><label class="col-sm-12"><strong>Apply On</strong></label><div class="col-sm-4 col-sm-offset-3"> <label><input type="checkbox" value="true" /> To All</label> <br /> <label><input type="checkbox" id="all-check' + i + '" value="true" name="tTypes[' + i + '].FullTime" /> Full Time - 40 Hour per week</label> <label><input value="true" type="checkbox" name="tTypes[' + i + '].PartTime" /> Part time - Less then 40 hours per week</label> <label><input type="checkbox" value="true" name="tTypes[' + i + '].VaryingWeekly" /> Varying weekly hours (no fixed hours)</label> <label><input value="true" type="checkbox" name="tTypes[' + i + '].Contractor" /> Contractor </label> <br /> <label><input type="checkbox" value="true" name="tTypes[' + i + '].Freelance" /> Freelance </label> </div> </div></td></tr>';
    $("#DeductTaxes-Table tbody").append(markup);

    $('.row-Container').mouseover(function () {
        $('#DeductTaxes-records .delDeductTaxesSection').css('display', 'block');
    }).mouseout(function () {
        $('#DeductTaxes-records .delDeductTaxesSection').hide();
    });
});

$("body").on("click", ".delDeductTaxesSection", function () {
    var item = $(this);

    bootbox.dialog({
        message: "Are you sure you want to delete this record?",
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
                    item.parent().parent().parent().remove();
                    var itemId = item.attr("data-id");

                    $.ajax({
                        type: "POST",
                        url: "/Secure/Setting/DeleteTaxTypesRecord",
                        data: {
                            Id: itemId,
                        },
                        error: function (xhr, status, error) {
                            alert(error);
                        },
                        success: function (response) {
                            $("#TaxTypes-Container").load("/Secure/Employee/_TaxTypes", function () { });
                        }
                    });
                }
            }
        }
    });
});

// End Tax Types

// Benefits Type

$("#employeeBenefitsType").click(function (ev) {
    var dataVal = $("#employeeBenefitsType").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#custom-benefitstype-fields').fadeIn().css('display', 'block');
    }
    else {
        $('#custom-benefitstype-fields').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Setting/SaveBenefitsType",
        data: {
            BenefitsType: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#employeeBenefitsType").attr("data-val", dataVal);
        }
    });
});

$(".benefitstype-Add").on("click", function (w) {
    $('.benefitstype-Save').css('display', 'block');
    var i = $('#BenefitType-records tr').length;
    var uuid = guid();
    var markup = '<tr class="row-Container" data-id="' + uuid + '"><td colspan="8"><div class="col-sm-12"><a href="javascript://" class="btn btn-xs btn-danger delBenefitTypeSection pull-right" style="display:none"><i class="fa fa-times"></i></a></div>'
    markup += '<div class="form-group col-sm-4"><label><strong>Benefit Name</strong></label><input type="text" name="bTypes[' + i + '].Title" class="form-control input-sm" required /></div>'
    markup += '<div class="form-group col-sm-4"><label><strong>Frequency</strong></label><select required name="bTypes[' + i + '].Frequency" class="form-control input-sm"><option value="">-- Select --</option><option value="Yearly">Yearly</option><option value="SemiYearly">Semi-Yearly</option><option value="Quarter">Quarter</option><option value="Monthly">Monthly</option><option value="BiWeekly">Bi-Weekly</option></select></div>';
    markup += '<div class="form-group col-sm-4"><label><strong>Amount</strong></label><input name="bTypes[' + i + '].Amount" type="number" min="0" class="form-control input-sm" /></div>';
    markup += '<div class="form-group"><label class="col-sm-12"><strong>Apply On</strong></label><div class="col-sm-4 col-sm-offset-3"> <label><input type="checkbox" value="true" /> To All</label> <br /> <label><input type="checkbox" id="all-check' + i + '" value="true" name="bTypes[' + i + '].FullTime" /> Full Time - 40 Hour per week</label> <label><input value="true" type="checkbox" name="bTypes[' + i + '].PartTime" /> Part time - Less then 40 hours per week</label> <label><input type="checkbox" value="true" name="bTypes[' + i + '].VaryingWeekly" /> Varying weekly hours (no fixed hours)</label> <label><input value="true" type="checkbox" name="bTypes[' + i + '].Contractor" /> Contractor </label> <br /> <label><input type="checkbox" value="true" name="bTypes[' + i + '].Freelance" /> Freelance </label> </div> </div></td></tr>';
    $("#BenefitType-Table tbody").append(markup);

    $('.row-Container').mouseover(function () {
        $('#BenefitType-records .delBenefitTypeSection').css('display', 'block');
    }).mouseout(function () {
        $('#BenefitType-records .delBenefitTypeSection').hide();
    });
});

$("body").on("click", ".delBenefitTypeSection", function () {
    var item = $(this);

    bootbox.dialog({
        message: "Are you sure you want to delete this record?",
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
                    item.parent().parent().parent().remove();
                    var itemId = item.attr("data-id");

                    $.ajax({
                        type: "POST",
                        url: "/Secure/Setting/DeleteBenefitTypeRecord",
                        data: {
                            Id: itemId,
                        },
                        error: function (xhr, status, error) {
                            alert(error);
                        },
                        success: function (response) {
                            $("#BenefitsType-Container").load("/Secure/Setting/_BenefitsType", function () { });
                        }
                    });
                }
            }
        }
    });
});

// End Benefits Type

// Employee Bonus Type

$("#employeeBonus").click(function (ev) {
    var dataVal = $("#employeeBonus").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#custom-employeeBonus-fields').fadeIn().css('display', 'block');
    }
    else {
        $('#custom-employeeBonus-fields').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Setting/SaveBonusType",
        data: {
            BonusType: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#employeeBonus").attr("data-val", dataVal);
        }
    });
});

$(".employeeBonus-Add").on("click", function (w) {
    $('.employeeBonus-Save').css('display', 'block');
    var i = $('#employeeBonus-records tr').length;
    var uuid = guid();
    var markup = '<tr class="row-Container" data-id="' + uuid + '"><td colspan="8"><div class="col-sm-12"><a href="javascript://" class="btn btn-xs btn-danger delemployeeBonusSection pull-right" style="display:none"><i class="fa fa-times"></i></a></div>'
    markup += '<div class="form-group col-sm-4"><label><strong>Bonus Name</strong></label><input type="text" name="bonusTypes[' + i + '].Title" class="form-control input-sm" required /></div>'
    markup += '<div class="form-group col-sm-4"><label><strong>Frequency</strong></label><select required name="bonusTypes[' + i + '].Frequency" class="form-control input-sm"><option value="">-- Select --</option><option value="Yearly">Yearly</option><option value="SemiYearly">Semi-Yearly</option><option value="Quarter">Quarter</option><option value="Monthly">Monthly</option><option value="BiWeekly">Bi-Weekly</option></select></div>';
    markup += '<div class="form-group col-sm-4"><label><strong>% Bonus</strong></label><input name="bonusTypes[' + i + '].BonusPercentage" type="number" min="0" class="form-control input-sm" /></div>';
    markup += '<div class="form-group"><label class="col-sm-12"><strong>Apply On</strong></label><div class="col-sm-4 col-sm-offset-3"> <label><input type="checkbox" value="true" /> To All</label> <br /> <label><input type="checkbox" id="all-check' + i + '" value="true" name="bonusTypes[' + i + '].FullTime" /> Full Time - 40 Hour per week</label> <label><input value="true" type="checkbox" name="bonusTypes[' + i + '].PartTime" /> Part time - Less then 40 hours per week</label> <label><input type="checkbox" value="true" name="bonusTypes[' + i + '].VaryingWeekly" /> Varying weekly hours (no fixed hours)</label> <label><input value="true" type="checkbox" name="bonusTypes[' + i + '].Contractor" /> Contractor </label> <br /> <label><input type="checkbox" value="true" name="bonusTypes[' + i + '].Freelance" /> Freelance </label> </div> </div></td></tr>';
    $("#employeeBonus-Table tbody").append(markup);

    $('.row-Container').mouseover(function () {
        $('#employeeBonus-records .delemployeeBonusSection').css('display', 'block');
    }).mouseout(function () {
        $('#employeeBonus-records .delemployeeBonusSection').hide();
    });
});

$("body").on("click", ".delemployeeBonusSection", function () {
    var item = $(this);

    bootbox.dialog({
        message: "Are you sure you want to delete this record?",
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
                    item.parent().parent().parent().remove();
                    var itemId = item.attr("data-id");

                    $.ajax({
                        type: "POST",
                        url: "/Secure/Setting/DeleteEmployeeBonusRecord",
                        data: {
                            Id: itemId,
                        },
                        error: function (xhr, status, error) {
                            alert(error);
                        },
                        success: function (response) {
                            $("#employeeBonus-Container").load("/Secure/Setting/_EmployeeBonus", function () { });
                        }
                    });
                }
            }
        }
    });
});

// End Employee Bonus Type

//End Payroll Settings


// Certificate Settings

$("#employeeCertificates").click(function (ev) {
    var dataVal = $("#employeeCertificates").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#custom-employeeCertificates-fields').fadeIn().css('display', 'block');
    }
    else {
        $('#custom-employeeCertificates-fields').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Setting/SaveEmployeeCertificateSetting",
        data: {
            certificate: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#employeeCertificates").attr("data-val", dataVal);
        }
    });
});

$(".employeeCertificates-Add").on("click", function (w) {
    $('.employeeCertificates-Save').css('display', 'block');
    var i = $('#employeeCertificates-records tr').length;
    var uuid = guid();
    var markup = '<tr class="row-Container" data-id="' + uuid + '"><td colspan="8"><div class="col-sm-12"><input type="hidden" name="certificateTypes[' + i + '].AppliesTo" id="AppliesTo_' + i + '" /> <a href="javascript://" class="btn btn-xs btn-danger delemployeeCertificatesSection pull-right" style="display:none"><i class="fa fa-times"></i></a></div><div class="row"><div class="col-md-8"><div class="row"><div class="col-md-6"><div class="form-group"><label><strong>Title</strong></label><input type="text" name="certificateTypes[' + i + '].Title" class="form-control input-sm" autocomplete="off" value="" required /></div></div><div class="col-md-6"><div class="form-group"><label><strong>Impact</strong></label><select class="form-control input-sm" name="certificateTypes[' + i + '].Impact"><option value="Compulsory">Compulsory</option><option value="Important">Important</option><option value="GoodToHave">Good To Have</option></select></div></div>'
    //markup += '<div class="form-group col-sm-4"><label><strong>Bonus Name</strong></label><input type="text" name="bonusTypes[' + i + '].Title" class="form-control input-sm" required /></div>'
    markup += '<div class="col-md-6"><div class="form-group"><label><strong>Actions to perform if this certificate is expired?</strong></label><select name="certificateTypes[' + i + '].PerformAction" class="form-control input-sm"><option value="BlockUserAccess">Block User Access</option><option value="ShowAlert">Show Alert</option><option value="NoAction">No Action</option></select></div></div>';
    markup += '<div class="col-md-6"><div class="form-group"><label><strong>Does this certificate have an expiry date?</strong></label><div><input type="checkbox" value="true" name="certificateTypes[' + i + '].IsExpiryDate" /></div></div></div>';
    markup += '<div class="col-md-12"><div class="form-group"><label><strong>Description</strong></label><textarea type="text" name="certificateTypes[' + i + '].Description" rows="4" class="form-control input-sm"></textarea></div></div></div></div>';
    //markup += '<div class="col-md-4"><div class="form-group"><label class="small">Applies To</label><input type="hidden" id="AppliesTo" name="AppliesTo" /><div class="row"></div></div></div></div>';
    markup += '<div class="col-md-4"><div class="form-group"><label class="col-sm-12"><strong>Apply On</strong></label><div class="col-sm-12"> <label><input type="checkbox" value="true" /> To All</label> <br /> <label><input type="checkbox" id="all-check' + i + '" class="appliesToContact-' + i + '" name="certificateTypes[' + i + '].FullTime" value="FullTime" /> Full Time - 40 Hour per week</label> <label><input type="checkbox" class="appliesToContact-' + i + '" name="certificateTypes[' + i + '].PartTime" value="PartTime" /> Part time - Less then 40 hours per week</label> <label><input type="checkbox" value="VaryingWeekly" name="certificateTypes[' + i + '].VaryingWeekly" class="appliesToContact-' + i + '" /> Varying weekly hours (no fixed hours)</label> <label><input class="appliesToContact-' + i + '" type="checkbox" name="certificateTypes[' + i + '].Contractor" value="Contractor" /> Contractor </label> <br /> <label><input type="checkbox" class="appliesToContact-' + i + '" value="Freelance" name="certificateTypes[' + i + '].Freelance" /> Freelance </label> </div> </div></td></tr>';
    $("#employeeCertificates-Table tbody").append(markup);

    $('.row-Container').mouseover(function () {
        $('#employeeCertificates-records .delemployeeCertificatesSection').css('display', 'block');
    }).mouseout(function () {
        $('#employeeCertificates-records .delemployeeCertificatesSection').hide();
    });

    $('.appliesToContact-'+i).on('click', function () {
        $('#AppliesTo_'+i).val($('.appliesToContact-'+i+':checked').map(function () { return this.value; }).get().join(','));
    });
});

$("body").on("click", ".delemployeeCertificatesSection", function () {
    var item = $(this);

    bootbox.dialog({
        message: "Are you sure you want to delete this record?",
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
                    item.parent().parent().parent().remove();
                    var itemId = item.attr("data-id");

                    $.ajax({
                        type: "POST",
                        url: "/Secure/Setting/DeleteEmployeeCertificatesRecord",
                        data: {
                            Id: itemId,
                        },
                        error: function (xhr, status, error) {
                            alert(error);
                        },
                        success: function (response) {
                            $("#employeeCertificates-Container").load("/Secure/Setting/_Certificates", function () { });
                        }
                    });
                }
            }
        }
    });
});

// End Certificate Settings