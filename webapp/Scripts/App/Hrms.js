
//HRMS



/*------------------- Employee Form -------------------*/
function bindDropdown(ddlToAppend, postedUrl, postedData, selectedVal) {
    //debugger;
    //var ddlToAppend = $('#Center');
    if (postedData != null && postedData != '') {
        ddlToAppend.empty().append('<option selected="selected" value="0" disabled = "disabled">Loading.....</option>');
        $.ajax({
            type: "POST",
            url: postedUrl,
            data: JSON.stringify({ Id: postedData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (selectedVal == '') {
                    ddlToAppend.empty().append('<option selected="selected" value="0"> --Please select-- </option>');
                    $.each(response, function () {
                        ddlToAppend.append($("<option></option>").val(this['Value']).html(this['Text']));
                    });
                }
                else {
                    ddlToAppend.empty().append('<option selected="selected" value="0"> --Please select-- </option>');
                    $.each(response, function () {
                        ddlToAppend.append($("<option " + (this['Value'] === selectedVal ? 'selected="selected"' : 'not selected') + "></option>").val(this['Value']).html(this['Text']));
                    });
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    else {
        ddlToAppend.empty().append('<option selected="selected" value="0"> -- No records found -- </option>');
    }
    
}