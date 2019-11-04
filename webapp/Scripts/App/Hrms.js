
//HRMS



/*------------------- Employee Form -------------------*/
function bindDropdown(ddlCustomers, postedUrl, Id) {
    debugger;
    //var ddlCustomers = $('#Center');
    ddlCustomers.empty().append('<option selected="selected" value="0" disabled = "disabled">Loading.....</option>');
    $.ajax({
        type: "POST",
        url: postedUrl,
        data: JSON.stringify({ Id: Id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            ddlCustomers.empty().append('<option selected="selected" value="0"> --Please select-- </option>');
            $.each(response, function () {
                ddlCustomers.append($("<option></option>").val(this['Value']).html(this['Text']));
            });
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}