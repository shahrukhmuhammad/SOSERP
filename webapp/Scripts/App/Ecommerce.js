/// <reference path="../notify.min.js" />

//Ecommerce 

/*------------------- Json Requests for Products -------------------*/
function getProducts(dataTable) {
    $.get('/Ecommerce/Inventory/GetAllProducts').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].ProductId + '" /></td>';
            markup += '<td><img src="../../Content/Uploads/Ecommerce/p_' + data[x].ProductId + '.jpg" class="img-responsive grid-img" /></td>';
            markup += '<td>' + data[x].Sku + '</td>';
            markup += '<td>' + data[x].Title + '</td>';
            markup += '<td>' + data[x].CategoryTitle + '</td>';
            markup += '<td>' + data[x].ManufacturerTitle + '</td>';
            markup += '<td>' + data[x].PurchasePrice + '</td>';
            markup += '<td>' + data[x].SalePrice + '</td>';
            markup += '<td>' + data[x].Quantity + '</td>';

            var deleteRecordData = "deleteDelableRecord('/Secure/Ecommerce/ShippingCompanyDeleteAble', '/Secure/Ecommerce/ShippingCompanyDelete','" + data[x].ShippingCompanyId + "')";

            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="/Ecommerce/Inventory/ProductDetails/' + data[x].ProductId + '" class="btn btn-primary" title="View"><i class="fa fa-share"></i></a>';
            //markup += '<a href="javascript://" data-id="' + data[x].ProductId + '" class="btn btn-info editsc" title="Edit"><i class="fa fa-edit"></i></a>';
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';
            markup += '<div class="dropdown pull-left">';
            markup += '<a role="button" data-toggle="dropdown" class="dropdown-toggle btn btn-primary btn-xs" href="#">';
            markup += ' <span class="caret"></span></a>';
            markup += '<ul class="dropdown-menu gridDropdown">';
            markup += '<li><a href="javascript://">Create Purchase Order</a></li>';
            markup += '<li><a href="javascript://">Create Sale Order</a></li>';
            markup += '<li><a href="javascript://">Manage Stock</a></li>';
            markup += '<li><a href="javascript://">Print This Product</a></li>';
            markup += '<li><a href="javascript://">Send This Product in email</a></li></ul></div>';
            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

/*--------------------------------------*/

// Product Record

$("#btn_addRow").on('click', function (e) {
    e.preventDefault();
    var newTextBoxDiv = $(document.createElement('ul'))
         .attr({ class: "list-inline", id: "ul_" + counter, style: "margin-left: 2px;" });
    var index = counter - 1;
    newTextBoxDiv.after().html('<li>Qty</li>' +
          '<li><select class="form-control input-sm drop" id="Qty' + counter + '" name="BulkPrices[' + index + '].Quantity"> </select></li>' +
          '<li>Rate</li>' +
          '<li><input type="text" class="form-control input-sm" id="Rate' + counter + '" name="BulkPrices[' + index + '].Price" style="width: 100px;"></li> </ul>');

    newTextBoxDiv.appendTo("#rowContainer");

    counter++;
    $(function () {
        var $select = $(".drop");
        for (i = 1; i <= 100; i++) {
            $select.append($('<option></option>').val(i).html(i))
        }
    });
});

$("#btn_removeRow").on('click', function (e) {
    e.preventDefault();
    if (counter == 2) {
        alert("No more textbox to remove");
        return false;
    }
    counter--;
    $("#ul_" + counter).remove();
});

/*------------------- Start Orders -------------------*/
//btn_searchbySku Click
$('a#btn_searchbySku').click(function (e) {
    e.preventDefault();
    var dataItem = $('#inp-sku').val();
    if (dataItem != "") {
        $.post('/Ecommerce/Inventory/GetProductBySku/' + dataItem, function (d) {
            if (d != null) {
                $('input#inp-sku').val(d.Sku);
                $('input#inp-productEAN').val(d.EAN);
                $('input#inp-productId').val(d.ProductId);
                $('input#inp-title').val(d.Title);
                $('input#inp-price').val(d.SalePrice);
                $('input#inp-remaingQty').val(d.Quantity);
                $('input#inp-quantity').val('').focus();
            }
        });
    } else {
        $('form#add-item input').val('');
    }
});

$("#inp-quantity").on('focusout', function (e) {
    e.preventDefault();
    var p = parseFloat($('#inp-price').val());
    var q = parseFloat($('#inp-quantity').val());
    var remainQty = parseFloat($('#inp-remaingQty').val());
    if (q > remainQty) {
        $.notify("Product available Quantity is " + remainQty);
        $('#inp-quantity').val('').focus();
    }
    else {
        var t = p * q;
        $('#inp-total').val(t);
    }
});
$("#btn_AddProduct").click(function (e) {
    var index = $('#mytable tbody').children('tr').length;
    e.preventDefault();
    var rowData = {
        sku: $("#inp-sku").val(),
        EAN: $("#inp-productEAN").val(),
        title: $("#inp-title").val(),
        price: $("#inp-price").val(),
        quantity: $("#inp-quantity").val(),
        total: $("#inp-total").val(),
        productId: $("#inp-productId").val()
    };
    if (rowData.sku != "" && rowData.EAN != "" && rowData.title != "" && rowData.price != "" && rowData.quantity != "" && rowData.total != "") {

        var markup = "<tr><td><input type='hidden' name='Details[" + index + "].EAN' value=" + rowData.EAN + " />" + rowData.EAN + "</td><td><input type='hidden' name='Details[" + index + "].ProductId' value=" + rowData.productId + " /><input type='hidden' name='Details[" + index + "].Sku' value=" + rowData.sku + " /> " + rowData.sku + "</td><td><input type='hidden' name='Details[" + index + "].Description' value=" + rowData.title + " />" + rowData.title + "</td><td><input type='hidden' name='Details[" + index + "].Price' value=" + rowData.price + " />" + rowData.price + "</td><td><input type='hidden' name='Details[" + index + "].Quantity' value=" + rowData.quantity + " />" + rowData.quantity + "</td><td><input type='hidden' name='Details[" + index + "].Total' value=" + rowData.total + " />" + rowData.total
            + "</td><td><input type='button' class='btn btn-xs btn-info edit' value='Edit'/> &nbsp; &nbsp; &nbsp;<input type='button' class='btn btn-xs btn-info delete' value='Delete'/></td></tr>";
        $("#mytable tbody").append(markup);
        var item = parseFloat($('#inp-total').val());
        ClearRowData();
        //var sum = parseFloat($('#SubTotal').val());
        var sum = (isNaN(parseFloat($('#SubTotal').val()))) ? 0 : parseFloat($('#SubTotal').val());

        $('#SubTotal').val(sum + item).change();
        //console.log(item+' sum '+ sum);

    }
});

function ClearRowData() {
    $('input#inp-productEAN').val('');
    $('input#inp-title').val('');
    $('input#inp-price').val('');
    $('input#inp-quantity').val('');
    $('input#inp-total').val('');
    $('input#inp-sku').val('').focus();
}
//radio check changed
$('input:radio[name=PostageType]').change(function () {
    if (this.value == '1') {
        BindPackagesData(this.value)
    }
    else if (this.value == '2') {
        BindPackagesData(this.value)
    }
    else if (this.value == '3') {
        BindPackagesData(this.value)
    }
});

function BindPackagesData(packID) {
    var ddlCustomers = $("#ShippingPackagesId");
    ddlCustomers.empty().append('<option selected="selected" value="0" disabled = "disabled">Loading.....</option>');
    $.ajax({
        type: "POST",
        url: "/Ecommerce/Order/GetShippingPackages?name=" + packID,
        data: '{}',
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

//btn_edit click
$(document).on('click', '.edit', function (e) {
    e.preventDefault();

    //Getting Data from grid
    var tableData = $(this).closest('tr').children("td").map(function () {
        return $(this).text();
    }).get();
    // console.log(tableData);

    //assigning data to Controls for Edit
    if (tableData != null) {
        $('input#inp-sku').val($.trim(tableData[0])).focus();
        $('input#inp-productEAN').val($.trim(tableData[1]));
        $('input#inp-title').val($.trim(tableData[2]));
        $('input#inp-price').val($.trim(tableData[3]));
        $('input#inp-quantity').val($.trim(tableData[4]));
        $('input#inp-total').val($.trim(tableData[5]));

        var sum = $('#SubTotal').val();
        var itm = $('input#inp-total').val();
        $('#SubTotal').val((sum - itm)).change();

        $(this).closest('tr').remove();
    }

});

//btn_delete
$(document).on('click', '.delete', function (e) {
    e.preventDefault();
    var sum = $('#SubTotal').val();
    var itm = $(this).prev('input#inp-total').val();
    $('#SubTotal').val((sum - itm)).change();
    $(this).closest('tr').remove();
});

//Calculation
$('#SubTotal, #Tax').change(function () {
    var it = parseFloat($('#SubTotal').val());
    var tx = parseFloat($('#Tax').val());
    var sp = (isNaN(parseFloat($('#ShippingCost').val()))) ? 0 : parseFloat($('#ShippingCost').val());
    if (isNaN(it)) it = 0;
    if (isNaN(tx)) tx = 0;

    //if (tx == NaN) tx = 0;
    var txa = it * tx / 100;
    $("#TaxAmount").val(txa);
    $('#Total').val((it + txa + sp));
});

//Shipping Package Change Event
$('#ShippingPackagesId').on('change', function (e) {
    e.preventDefault();
    var packageid = this.value;
    if (packageid != "") {
        $.post('/Ecommerce/Order/GetShippingPriceById/' + packageid, function (d) {
            if (d != null) {
                $("#ShippingCost").val(d);
                $('#SubTotal').change();
            }
        });
    } else {
        $('ShippingPackagesId').focus();
    }
});

//Client Dropdown Change Event
$('#ContactId').on('change', function (e) {
    e.preventDefault();
    var dataItem = this.value;
    if (dataItem != "") {
        $.post('@Url.Action("GetAllShippingDetails", "Contact")/' + dataItem, function (d) {
            //$('#ShippingDetails').val(d);
            console.log("response " + d);
            $('input#FirstName').val(d.FirstName);
            $('input#LastName').val(d.LastName);
            $('input#Company').val(d.BusinessName);
            $('input#PostalAddress').val(d.PostalAddress);
            $('input#PostalAddress2').val(d.PostalAddress2);
            $('input#City').val(d.City);
            $('input#County').val(d.County);
            $('input#PostalCode').val(d.PostalCode);
            $('input#Country').val(d.Country);
            $('input#Phone').val(d.Phone);
        });
    } else {
        $('#div_billingDetails').find('input:text').val('');
    }
});

//chechbox address Change
$('#isBillingAddress').change(function (e) {
    e.preventDefault();
    if ($(this).prop('checked')) {
        $('#sp_FirstName').val($("#FirstName").val());
        $('#sp_LastName').val($("#LastName").val());
        $('#sp_company').val($("#Company").val());
        $('#sp_address').val($("#PostalAddress").val());
        $('#sp_address2').val($("#PostalAddress2").val());
        $('#sp_City').val($("#City").val());
        $('#sp_County').val($("#County").val());
        $('#sp_PostalCode').val($("#PostalCode").val());
        $('#sp_Country').val($("#Country").val());
        $('#sp_Phone').val($("#Phone").val());
    }
    else {
        $('#div_shippingDetails').find('input:text').val('');
        $('#sp_Phone').val('');
    }
});

/*------------------- End Orders -------------------*/

/*------------------- Start Catalog -------------------*/
$('#btn_featured').on('click', function (e) {
    e.preventDefault();
    changeMultipleRecords('/Ecommerce/Catalog/BulkFeatured', '.checkIt', 'Are you sure you want to mark as featured selected product(s)?');
});

$('#btn_topSeller').on('click', function (e) {
    e.preventDefault();
    changeMultipleRecords('/Ecommerce/Catalog/BulkTopSeller', '.checkIt', 'Are you sure you want to mark as Top Seller selected product(s)?');
});

$('#btn_Website').on('click', function (e) {
    e.preventDefault();
    changeMultipleRecords('/Ecommerce/Catalog/BulkPublishOnWebsite', '.checkIt', 'Are you sure you want to publish for Website selected product(s)?');
});

$('#btn_Facebook').on('click', function (e) {
    e.preventDefault();
    changeMultipleRecords('/Ecommerce/Catalog/BulkPublishOnTradeOnly', '.checkIt', 'Are you sure you want to publish for Facebook selected product(s)?');
});

$('#btn_Tradeonly').on('click', function (e) {
    e.preventDefault();
    changeMultipleRecords('/Ecommerce/Catalog/BulkPublishOnFacebook', '.checkIt', 'Are you sure you want to publish for Tradeonly selected product(s)?');
});

//
function changeMultipleRecords(targetUrl, recordsClass, msg, returnUrl) {
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
            message: msg,
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
                    label: "Save",
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
/*------------------- End Catalog -------------------*/