$(document).ready(function () {
    if ($('.checklistbox').html() == '' || $('.checklistbox').html() == null)
    {
        $('.checklistbox').html('<p class="text-center text-muted" id="notfound-txt">No item found</p>');
    }
    if ($('.completed-checklistbox').html() == '' || $('.completed-checklistbox').html() == null) {
        $('.completed-checklistbox').html('<p class="text-center text-muted" id="comp-notfound-txt">No item found</p>');
    }
});

$.fn.enterKey = function (fnc) {
    return this.each(function () {
        $(this).keypress(function (ev) {
            var keycode = (ev.keyCode ? ev.keyCode : ev.which);
            if (keycode == '13') {
                fnc.call(this, ev);
            }
        })
    })
}

$('#checklist-todo').enterKey(function (e) {
    debugger;
    if ($(this).val() != '') {
        $('#notfound-txt').css('display', 'none');
        var markup = '';
        markup = '<div class="checklist-item checkItem' + $('.checklist-item').length + '">';
        markup += '<label style="font-size:14px;"><input type="checkbox" onclick="return OnCheckedChange(' + $('.checklist-item').length + ')" class="checkbox-inline" /> &nbsp; ' + $(this).val() + ' </label> ';
        markup += '<span style="margin-left:5px;"><a href="javascript://" onclick="return EditCheckItem(' + $('.checklist-item').length + ')" data-ids="' + $(this).val() + '" style="font-size:16px; margin:6px;"><i class="fa fa-edit"></i></a><a href="javascript://" style="font-size:16px; margin:6px;" onclick="return DelCheckItem(' + $('.checklist-item').length + ')" class="del-checkItem" data-id=' + $('.checklist-item').length + '><i class="fa fa-close"></i></a>';
        markup += '</span></div><div class="clearfix"></div>'
        $(this).val('');
        $('.checklistbox').append(markup);
        $('#incomplete').html($('.checklistbox .checklist-item').length);
        $('#completed').html($('.completed-checklistbox .checklist-item').length);
    }
});

$('#pure-check').on('click', function (e) {
    if ($('#checklist-todo').val() != '') {
        $('#notfound-txt').css('display', 'none');
        var markup = '';
        markup = '<div class="checklist-item checkItem' + $('.checklist-item').length + '">';
        markup += '<label style="font-size:14px;"><input type="checkbox" onclick="return OnCheckedChange(' + $('.checklist-item').length + ')" class="checkbox-inline" /> &nbsp; ' + $('#checklist-todo').val() + ' </label> ';
        markup += '<span style="margin-left:65px;"><a href="javascript://" onclick="return EditCheckItem(' + $('.checklist-item').length + ')" data-ids="' + $('#checklist-todo').val() + '" style="font-size:16px; margin:6px;"><i class="fa fa-edit"></i></a><a href="javascript://" style="font-size:16px; margin:6px;" onclick="return DelCheckItem(' + $('.checklist-item').length + ')" class="del-checkItem" data-id=' + $('.checklist-item').length + '><i class="fa fa-close"></i></a>';
        markup += '</span></div><div class="clearfix"></div>'
        $('#checklist-todo').val('');
        $('.checklistbox').append(markup);
        $('#incomplete').html($('.checklistbox .checklist-item').length);
        $('#completed').html($('.completed-checklistbox .checklist-item').length);
    }
});
    