
//Ecommerce Settings

var dataVal = $("#MinimumOrderQuantity").attr("data-val");
    if (dataVal == "true") {
        $('#Minimum-Order-Quantity').css('display', 'block');
    }
    else {
        $('#Minimum-Order-Quantity').css('display', 'none');
    }
    var dataVal = $("#ClickAndCollect").attr("data-val");
    if (dataVal == "true") {
        $('#Click-And-Collect').css('display', 'block');
    }
    else {
        $('#Click-And-Collect').css('display', 'none');
    }
    var dataVal = $("#OnlinePayment").attr("data-val");
    if (dataVal == "true") {
        $('#online-payment').css('display', 'block');
    }
    else {
        $('#online-payment').css('display', 'none');
    }
    var dataVal = $("#BankPayment").attr("data-val");
    if (dataVal == "true") {
        $('#bank-details').css('display', 'block');
    }
    else {
        $('#bank-details').css('display', 'none');
    }

$("#MinimumOrderQuantity").click(function (ev) {
        var dataVal = $("#MinimumOrderQuantity").attr("data-val");
        //alert(dataVal);
        if (dataVal === "false") {
            dataVal = "true";
        }
        else {
            dataVal = "false";
        }

        if ($(this).val() === "false") {
            $('#Minimum-Order-Quantity').fadeIn().css('display', 'block');
        }
        else {
            $('#Minimum-Order-Quantity').fadeOut(600);
        }

        $.ajax({
            type: "POST",
            url: "/Secure/Ecommerce/SaveMinimumOrderQuantity",
            data: {
                PaidTimeOff: dataVal
            },
            error: function (xhr, status, error) {
                alert(error);
            },
            success: function (response) {
                $("#MinimumOrderQuantity").attr("data-val", dataVal);
            }
        });
});

$("#ClickAndCollect").click(function (ev) {
    var dataVal = $("#ClickAndCollect").attr("data-val");
    //alert(dataVal);
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#Click-And-Collect').fadeIn().css('display', 'block');
    }
    else {
        $('#Click-And-Collect').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveClickAndCollectSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#ClickAndCollect").attr("data-val", dataVal);
        }
    });
});

$("#OnlinePayment").click(function (ev) {
    var dataVal = $("#OnlinePayment").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#online-payment').fadeIn().css('display', 'block');
    }
    else {
        $('#online-payment').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveOnlinePaymentSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#OnlinePayment").attr("data-val", dataVal);
        }
    });
});

$("#BankPayment").click(function (ev) {
    var dataVal = $("#BankPayment").attr("data-val");
    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    if ($(this).val() === "false") {
        $('#bank-details').fadeIn().css('display', 'block');
    }
    else {
        $('#bank-details').fadeOut(600);
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveBankPaymentSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#BankPayment").attr("data-val", dataVal);
        }
    });
});

$("#includeVATforTrader").click(function (ev) {
    //debugger;
    var dataVal = $("#includeVATforTrader").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveIncludeVATforTraderSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#includeVATforTrader").attr("data-val", dataVal);
        }
    });
});

$("#includeVATforRetailer").click(function (ev) {
    var dataVal = $("#includeVATforRetailer").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveIncludeVATforRetailerSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#includeVATforRetailer").attr("data-val", dataVal);
        }
    });
});
$("#ShippingFreeForAll").click(function (ev) {
    var dataVal = $("#ShippingFreeForAll").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveShippingFreeForAllSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#ShippingFreeForAll").attr("data-val", dataVal);
        }
    });
});
$("#ShippingFreeForEconomy").click(function (ev) {
    var dataVal = $("#ShippingFreeForEconomy").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveShippingFreeForEconomySettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#ShippingFreeForEconomy").attr("data-val", dataVal);
        }
    });
});
$("#ShippingFreeForStandard").click(function (ev) {
    var dataVal = $("#ShippingFreeForStandard").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveShippingFreeForStandardSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#ShippingFreeForStandard").attr("data-val", dataVal);
        }
    });
});
$("#ShippingFreeForExpress").click(function (ev) {
    var dataVal = $("#ShippingFreeForExpress").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveShippingFreeForExpressSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#ShippingFreeForExpress").attr("data-val", dataVal);
        }
    });
});
$("#OutOfStock").click(function (ev) {
    var dataVal = $("#OutOfStock").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveOutOfStockSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#OutOfStock").attr("data-val", dataVal);
        }
    });
});
$("#CashOnDelivery").click(function (ev) {
    var dataVal = $("#CashOnDelivery").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveCashOnDeliverySettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#CashOnDelivery").attr("data-val", dataVal);
        }
    });
});
$("#OnlineOrder").click(function (ev) {
    var dataVal = $("#OnlineOrder").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SaveOnlineOrderSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#OnlineOrder").attr("data-val", dataVal);
        }
    });
});

$("#PaymentAtPickup").click(function (ev) {
    var dataVal = $("#PaymentAtPickup").attr("data-val");

    if (dataVal === "false") {
        dataVal = "true";
    }
    else {
        dataVal = "false";
    }

    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SavePaymentAtPickupSettings",
        data: {
            Approval: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            $("#PaymentAtPickup").attr("data-val", dataVal);
        }
    });
});

//--- Ecommerce Shipping Management

//Add Shipping Company
$('.addNewCompany').on('click', function () {
    var target = "";
    target = "/Secure/Ecommerce/_ShippingCompanyRecord";


    // load the url and show modal on success
    $("#addNewCompany-Modal .modal-dialog").load(target, function () {
        $("#addNewCompany-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

//Edit Shipping Company
$(document.body).on('click','.editsc', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Ecommerce/_ShippingCompanyRecord/"+Id;


    // load the url and show modal on success
    $("#addNewCompany-Modal .modal-dialog").load(target, function () {
        $("#addNewCompany-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

//View Shipping Company
$(document.body).on('click', '.viewsc', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Ecommerce/_ShippingCompany/" + Id;


    // load the url and show modal on success
    $("#addNewCompany-Modal .modal-dialog").load(target, function () {
        $("#addNewCompany-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

//View Shipping Company Report
$(document.body).on('click', '.reportsc', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Ecommerce/_ShippingCompany/" + Id;


    // load the url and show modal on success
    $("#addNewCompany-Modal .modal-dialog").load(target, function () {
        $("#addNewCompany-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});


/*------------------- Json Requests for Shipping Companies -------------------*/
function getshippingCompanies(dataTable) {
    $.get('/Secure/Ecommerce/GetAllCompanies').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].ShippingCompanyId + '" /></td>';
            markup += '<td><a href="javascript://" class="scTitle" data-id="' + data[x].ShippingCompanyId + '">' + data[x].Title + '</a></td>';
            //markup += '<td><a href="' + data[x].CompanyWebAddrress + '" target="_blank">' + data[x].CompanyWebAddrress + '</a></td>';
            //markup += '<td><a href="' + data[x].TrackingUrl + '" target="_blank">' + data[x].TrackingUrl + '</a></td>';
            //markup += '<td>' + formatDateTime(data[x].UpdatedOn) + '</td>';
            //markup += '<td>' + formatDateTime(data[x].CreatedOn) + '</td>';
            var deleteRecordData = "deleteDelableRecord('/Secure/Ecommerce/ShippingCompanyDeleteAble', '/Secure/Ecommerce/ShippingCompanyDelete','" + data[x].ShippingCompanyId + "')";

            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="javascript://" data-id="' + data[x].ShippingCompanyId + '" class="btn btn-primary viewsc" title="View"><i class="fa fa-share"></i></a>';
            markup += '<a href="javascript://" data-id="' + data[x].ShippingCompanyId + '" class="btn btn-info editsc" title="Edit"><i class="fa fa-edit"></i></a>';
            markup += '<a href="javascript://" data-id="' + data[x].ShippingCompanyId + '" class="btn btn-warning reportsc" title="View Report"><i class="fa fa-file-text-o"></i></a>';
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

/*--------------------------------------*/

//--- Ecommerce Shipping Packages Management

//Load Shipping Packages of Single Company
$(document.body).on("click", ".scTitle", function (ev) {
    var Id = $(this).attr("data-id");
    if (Id > 0) {
        getshippingPackages("#shippingPackages-dataTable", Id);
    }

});

/*------------------- Json Requests for Selected Shipping Packages -------------------*/
function getshippingPackages(dataTable, Id) {
    //debugger;
    $.get('/Secure/Ecommerce/GetShippingPackages/' + Id).success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + data[x].Title + '</td>';
            markup += '<td>' + data[x].PackageType + '</td>';
            markup += '<td>' + data[x].DeliveryTime + '</td>';
            markup += '<td>' + data[x].Price + '</td>';

            var deleteRecordData = "deleteDelableRecord('/Secure/Ecommerce/ShippingCompanyDeleteAble', '/Secure/Ecommerce/ShippingCompanyDelete','" + data[x].Id + "')";

            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="javascript://" data-id="' + data[x].Id + '" class="btn btn-primary viewsp" title="View" target="_blank"><i class="fa fa-share"></i></a>';
            markup += '<a href="javascript://" data-id="' + data[x].Id + '" class="btn btn-info editsp" title="Edit"><i class="fa fa-edit"></i></a>';
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

//Add Shipping Package
$('.addNewPackage').on('click', function () {
    var target = "";
    target = "/Secure/Ecommerce/_ShippingPackageRecord";


    // load the url and show modal on success
    $("#addNewPackage-Modal .modal-dialog").load(target, function () {
        $("#addNewPackage-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

//Edit Shipping Package
$(document.body).on('click', '.editsp', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Ecommerce/_ShippingPackageRecord/" + Id;


    // load the url and show modal on success
    $("#addNewPackage-Modal .modal-dialog").load(target, function () {
        $("#addNewPackage-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

//View Shipping Package
$(document.body).on('click', '.viewsp', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Ecommerce/_ShippingPackage/" + Id;


    // load the url and show modal on success
    $("#addNewPackage-Modal .modal-dialog").load(target, function () {
        $("#addNewPackage-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

/*--------------------------------------*/

/*------------------- Json Requests for Selected Shipping PostCodes -------------------*/
function getshippingPostcodes(dataTable) {
    //debugger;
    $.get('/Secure/Ecommerce/GetAllPostcodes').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + data[x].Postcode + '</td>';
            markup += '<td>' + data[x].StrRestrictionLevels + '</td>';
            markup += '<td>' + data[x].FirstItemPrice + '</td>';
            markup += '<td>' + data[x].EachAdditionalItemPrice + '</td>';
            markup += '<td>' + formatDateTime(data[x].CreatedOn) + '</td>';

            var deleteRecordData = "deleteRecord('/Secure/Ecommerce/ShippingPostcodeDelete','" + data[x].Id + "','/Secure/Setting/Ecommerce')";

            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="javascript://" data-id="' + data[x].Id + '" class="btn btn-info editPostcode" title="Edit"><i class="fa fa-edit"></i></a>';
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

/*--------------------------------------*/

//Post Code Managemnt
//Add New PostCode
$('.addNewPostCode').on('click', function () {
    var target = "";
    target = "/Secure/Ecommerce/_ShippingPostcodeRecord";


    // load the url and show modal on success
    $("#addNewPostCode-Modal .modal-dialog").load(target, function () {
        $("#addNewPostCode-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

//Edit PostCode
$(document.body).on('click', '.editPostcode', function (ev) {
    ev.preventDefault();
    var Id = $(this).attr("data-id");
    var target = "";
    target = "/Secure/Ecommerce/_ShippingPostcodeRecord/" + Id;


    // load the url and show modal on success
    $("#addNewPostCode-Modal .modal-dialog").load(target, function () {
        $("#addNewPostCode-Modal").modal("show");
        //filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0]);
        $('#loading-mask').hide();
    });
});

//Payment Gateway
$(".btnApply").click(function (ev) {
    var dataVal = $(this).attr("data-id");
    if (dataVal != "") 
    {
    $.ajax({
        type: "POST",
        url: "/Secure/Ecommerce/SavePaymentGateway",
        data: {
            value: dataVal
        },
        error: function (xhr, status, error) {
            alert(error);
        },
        success: function (response) {
            window.location.reload(true);
        }
    });
    }
});