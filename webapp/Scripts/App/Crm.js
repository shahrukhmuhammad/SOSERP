var dataVal = $("#AutoInactive").attr("data-val");
if (dataVal == "true") {
    $('#auto-inactive').css('display', 'block');
}
else {
    $('#auto-inactive').css('display', 'none');
}
var dataVal = $("#AccountSecurity").attr("data-val");
if (dataVal == "true") {
    $('#account-security').css('display', 'block');
}
else {
    $('#auto-inactive').css('display', 'none');
}
//--

$("#AutoInactive").click(function (ev) {
    var dataVal = $("#AutoInactive").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#auto-inactive').fadeIn().css('display', 'block');
    }
    else {
        $('#auto-inactive').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/CRM/Settings/SaveAutoInactive",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#AutoInactive").attr("data-val", dataVal);
        }
    });
});

$("#AccountSecurity").click(function (ev) {
    var dataVal = $("#AccountSecurity").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#account-security').fadeIn().css('display', 'block');
    }
    else {
        $('#account-security').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/CRM/Settings/SaveAccountSecurity",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#AccountSecurity").attr("data-val", dataVal);
        }
    });
});

$("#SecurityQuestions").click(function (ev) {
    var dataVal = $("#SecurityQuestions").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/CRM/Settings/SaveSecurityQuestions",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#SecurityQuestions").attr("data-val", dataVal);
        }
    });
});

$("#EmailCode").click(function (ev) {
    var dataVal = $("#EmailCode").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/CRM/Settings/SaveEmailCode",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#EmailCode").attr("data-val", dataVal);
        }
    });
});

$("#SMSCode").click(function (ev) {
    var dataVal = $("#SMSCode").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/CRM/Settings/SaveSMSCode",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#SMSCode").attr("data-val", dataVal);
        }
    });
});