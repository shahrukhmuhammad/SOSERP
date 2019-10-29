//--- Loading Mask
window.onload = function () { $('#loading-mask').fadeOut(500); };
$('a:not([href^="#"]):not([href^="javascript://"]):not([target="_blank"]):not([tabindex="0"]):not([href^="javascript:if(window.print)window.print()"])').click(function (e) {
    $(".loader").fadeIn("slow");
});
//---

//-- Form Validate
$('#frm').validate();
$('.frm').validate();
$('button:submit').click(function (e) {
    var isValid = $(this).parents('form:first').valid();
    if (isValid) {
        $(".loader").fadeIn("slow");
    }
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
});
//--- 