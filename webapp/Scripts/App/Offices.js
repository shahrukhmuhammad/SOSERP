$('.crmsearch').on('click', function () {
    debugger;
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