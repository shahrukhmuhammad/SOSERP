(function() { 
//window.isSafari = navigator.vendor.indexOf("Apple")==0 && /\sSafari\//.test(navigator.userAgent);

var nua = navigator.userAgent;
var is_android = ((nua.indexOf('Mozilla/5.0') > -1 && nua.indexOf('Android ') > -1 && nua.indexOf('AppleWebKit') > -1) && !(nua.indexOf('Chrome') > -1));
if(is_android){
    $('body').addClass('native-android');

}


function fitSelect(el,parent) {
    var elem=$(el);
    var parentWidth=elem.parents(parent).css('width');
    elem.next().attr('style', 'width: '+parentWidth+' !important');
}
getHtml = function() {
    return "<div id=\"dialog-confirm\" class=\"modal-box\" title=\"\">    <div class=\"modal-wrap clearfix\">      <p></p>    </div>  </div>  ";
};

getHtmlAlert = function() {
    return "<div id=\"dialog-alert\" class=\"modal-box\" title=\"\">      <div class=\"modal-wrap clearfix\">        <p></p>      </div>    </div>    ";
};

window.alertBox = function(msg, callback) {
    if (!$("#dialog-alert").length) {
        $('body').append(getHtmlAlert());
    } else {
        $("#dialog-alert").dialog('destroy');
    }
    $("#dialog-alert .modal-wrap p").html(msg);
    return $("#dialog-alert").dialog({
        resizable: false,
        height: 140,
        modal: true,
        width: '51%',
        buttons: {
            OK: {
                'class': 'save ',
                click: function() {
                    if (callback === void 0) {
                        return $(this).dialog('close');
                    } else {
                        return callback();
                    }
                },
                text: "OK"
            }
        }
    });
};


jQuery.expr[':'].icontains = function(a, i, m) {
  return jQuery(a).text().toUpperCase()
  .indexOf(m[3].toUpperCase()) >= 0;
};
jQuery(document).ready(function($) {

    if (!Modernizr.svg) {
       $("img[src$='.svg']").attr('src',function(){
           return $(this).data('png');
       });
   }   
   if (!Modernizr.borderradius) {
      $('.primary-button,a.button').corner('5px');
      //$('#search').corner('5px');
      $('.cat-switch li,.calendar-switch li,.box-comm.box a').corner('5px');
      $('.ask,.reserve,.sent,.drive').corner('3px');
      //$('.doctor-photo').corner('50px');
      $('.katalog-letters ul li a').corner('4px');
  }    
  $('.go-back,.goback').not('.in-panel,.back-left').not('.back-left').on('click touchstart',function(e){
     e.preventDefault();
   //console.log(document.referrer);
   if (document.referrer == "") {
     window.location="/";
 }
 else{
     window.history.back();
 }

});
  $('#close-msc').on('click touchstart',function(){
    $('.page-left-panel-inner input').val('');
    $('a.city,.city-list,.city-list li,.popular').show();
    $('#close-msc').hide();
});
  $('#close-msc2').on('click touchstart',function(){
    $('.page-left-panel-inner input').val('');
    $('.specs a').show();
    $('#close-msc2').hide();
});
  $('.page-left-panel-inner input').on('keyup', function() {
    var search_for = $(this).val();
    if (search_for.length > 0) {
        if($(this).hasClass('mobile')){
            $('.popular').hide();
        }
        $('a.city').hide();
        $('#close-msc').show();
        $('a.city:icontains(' + search_for + ')').show();
        $('.city-list').each(function() {
            var show_num = 0;
            $.each($(this).find('li').not('.col-1'), function(num, val) {
                var show_num_a = 0;
                $.each($(val).find('a.city'), function(n, v) {
                    if ($(v).css('display') != 'none')
                        show_num_a++;
                });
                if (show_num_a == 0)
                    $(val).hide();
                else
                    $(val).show();
                if ($(val).css('display') != 'none')
                    show_num++;
            });
            if (show_num == 0)
                $(this).hide();
            else
                $(this).show();
        });
    }
    else {
        $('a.city,.city-list,.city-list li,.popular').show();
        $('#close-msc').hide();
    }
});
$('.page-left-panel-inner .menu-search-spec').on('keyup', function() {
    var search_for = $(this).val();
    if (search_for.length > 0) {
       $('#close-msc2').show();
       $('.specs a').hide();
       $('.specs a:icontains(' + search_for + ')').show();


   }
   else {
    $('.specs a').show();
    $('#close-msc2').hide();
}
});
"use strict";
FastClick.attach(document.body);

if(!Modernizr.placeholder){
    $('input, textarea').placeholder();
}      

//    if($("#carousel").length){ 
//    var carousel = new Carousel("#carousel");
//    carousel.init();
//    }
var w = $(window).width();
if (w <= 360) {
    $('h2.name').insertAfter($('.rating-quantity'));
}


//    /*PUSHER MENU FROM LEFT*/
//    new mlPushMenu(document.getElementById('mp-menu'), document.getElementById('search-spec'));
//    new mlPushMenu(document.getElementById('mp-menu2'), document.getElementById('search-city'));


/*WYSOKOŚĆ I POZYCJA MAPY*/
    /*
     if (w >= 560)
     var y = jQuery('#site-top').innerHeight() + jQuery('#header-top').innerHeight();
     else
     var y = 50;
     var h = $('#map.main-map').innerHeight() - y;
     $('#map.main-map').css({'top': y, 'height': h + 'px'});
     
     
     */
//$('.scroll-pane').jScrollPane();
/*Category switcher*/
$('.cat-switcher').not('.mobile').find('li').on('click touchstart',function(e) {
    e.preventDefault();
    var show_id = $(this).data('show');
    $('.cat-switch').find('li').not($(this)).removeClass('active');
    $(this).addClass('active');
    $('.doctors-list').not('#cat-' + show_id).removeClass('active');
    $('#cat-' + show_id).addClass('active');
});

$('.cat-switcher.mobile').on('change', function(e) {
        //e.preventDefault();
        var show_id = $(this).find(':selected').val();
        $('.doctors-list').not('#cat-' + show_id).removeClass('active');
        $('#cat-' + show_id).addClass('active');
    });
i = 0;
/*DISABLING THE ALPHABET FOR CLOSING MENU  AS THIS IS OUTSIDE .mp-menu (PP) */
//    $('#letters-search').on('touchstart scrollstart scrollend',function(){
//       // console.log('scrolling'+i);
//        i++;
//    });
$('#letters-search').on('touchmove', function(event) {
    event.preventDefault();
    var window_width=$(window).width();
    var touch = event.originalEvent.touches[0] || event.originalEvent.changedTouches[0];
    $.each($('#letters-search li'), function(num, el) {

        var offt = $(el).offset().top;
        var offn = offt + $(el).height();
        var letter = $(el).text();
        if (touch.pageY >= offt && touch.pageY <= offn) {
            $('#letters-search li').not($(el)).removeClass('active');
            $(el).addClass('active');
            var cur_sc = $(el).parent().prev('.scroller').scrollTop();
            var mtop=parseInt($('#letter-' + letter).find('.col-1').css('margin-top'))-30;
            if(window_width<=560)
                var tx = $('#letter-' + letter).offset().top+mtop-21 + cur_sc;
            else
                var tx = $('#letter-' + letter).offset().top+mtop + cur_sc;


            $(el).parent().prev('.scroller').scrollTop(tx);
        }
    });

});
$('#letters-search').find('li').not('.go-to-search').on('touchstart click', function(e) {
    e.preventDefault();
    var window_width=$(window).width();
    $('#letters-search').find('li').not($(this)).removeClass('active');
    $(this).addClass('active');
    var letter = $(this).text();
    var cur_sc = $(this).parent().prev('.scroller').scrollTop();
    var mtop=parseInt($('#letter-' + letter).find('.col-1').css('margin-top'))-30;

    if(window_width<=560)
        var tx = $('#letter-' + letter).offset().top+mtop-21 + cur_sc;
    else
        var tx = $('#letter-' + letter).offset().top+mtop + cur_sc;
        //console.log(tx);

        $(this).parent().prev('.scroller').animate({scrollTop: tx}, 500);
        return false;
    });
$('.go-to-search').on('click touchstart',function(e){
    e.preventDefault();
    $(this).parent().prev('.scroller').animate({scrollTop: 0}, 500);
});
/*SIMPLE LIST SEARCH*/
$('#menu-search-city').keyup(function() {
    var valThis = $(this).val();
    $('.city-list>li>a').each(function() {
        var text = $(this).text().toLowerCase();
        (text.indexOf(valThis) == 0) ? $(this).show() : $(this).hide();
    });
});

/*FUNCTIONS FOR FILTERS*/
if ($('#dp1').length) {
    $('#dp1 input').datepicker({
        format: 'dd.mm.yyyy',
        autoclose: true,
        language: 'pl'


    });
}
if ($('#dp2').length) {
    $('#dp2 input').datepicker({
        format: 'dd.mm.yyyy',
        autoclose: true,
        language: 'pl'


    });
}
    /*$('.filters-mobile .filter-date input').focus(function(){
    document.activeElement.blur();
});*/
 /*$('.filters-mobile .filter-date a').datepicker({
            format: 'dd.mm.yyyy',
            autoclose: true,
            language: 'pl',
            
 

        });*/



$('.filters-mobile .filter-date a').datepicker({format: 'dd.mm.yyyy',            autoclose: true,            language: 'pl'})
.on('changeDate', function(ev){
  startDate = new Date(ev.date);

  var curr_date = startDate.getDate();
                        var curr_month = startDate.getMonth() + 1; //Months are zero based
                        var curr_year = startDate.getFullYear();
                        var newDate=curr_date + "." + curr_month + "." + curr_year;
                        //alert(newDate);
                        $(this).text(newDate);
                        $(this).next('input').val(newDate);
                        //$('date_from[name=date_from]').val(newDate);
                        
                        $('#dp4').datepicker('hide');
                    });
if ($('#spinner').length) {

    $("#spinner").spinner({
        min: 10,
        max: 2500,
        step: 10,
        numberFormat: "C",
        spin: function(event, ui) {
                //console.log(ui.value);

            }
        });
    }/*
    if ($('#spinner-2').length) {

        $("#spinner-2").spinner({
            min: 1,
            max: 2500,
            step: 10,
            numberFormat: "C"
        });
    }
    */

    /*HOURS SWITCH*/
    $('.calendar-switch').find('li').on('click touchstart',function(e) {
        e.preventDefault();
        $('.calendar-switch').find('li').not($(this)).removeClass('active');
        $(this).addClass('active');
        var place_id = $(this).data('show');

        if (place_id != 'all') {
            $('.day-hours').find('li').not('.' + place_id).removeClass('active');
            $('.day-hours').find('li.' + place_id).addClass('active');
        }
        else
            $('.day-hours').find('li').addClass('active');
    });


    /*MAPA W PROFILU*/
    if ($('#map').length) {




//    $('#map').gmap({'zoomControl':false,'streetViewControl':false,'panControl':false}).bind('init', function(ev, map) {    });

$('#mobile-calendar-switch.mobile').on('change', function(e) {

    var place_id = $(this).find(':selected').val();
    if (place_id != 'all') {
        $('.day-hours').find('li').not('.' + place_id).removeClass('active');
        $('.day-hours').find('li.' + place_id).addClass('active');
    }
    else
        $('.day-hours').find('li').addClass('active');
});


}
var la = $('#last-amount');

$('form input[name=payradio]').change(function() {

    var n = $("#select-type :selected").data("price");
    var p = parseFloat(n.replace(",", "."));
    $('#amount').val(p);
    la.fadeOut(function() {
        la.html(n);
        la.fadeIn();
    });

});
/*kontakt form*/
if ($('.validate-form').length) {
    $(".validate-form").validate({
        errorClass: 'message warning',
        errorPlacement: function(error, element) {
            $(element).parent('div').addClass('input-error');
            error.appendTo($(element).parent('div'));
        },
        success: function(label) {
            label.parent('div').removeClass('input-error');
            label.remove();
        }


    });

}


$('.mobile-login').on('click',function(e) {
    e.preventDefault();
    if($('body').hasClass("open-login")){
        $('body').addClass("closing-login");
        setTimeout(function(){$("body").removeClass("closing-login");},300);
    }
    $('body').toggleClass('open-login');
    
});

$('.mobile-open-panel').on('click touchstart',function(e) {
    e.preventDefault();
    $('#home-panel-1').hide();
    $('#home-panel-2').hide();
    show_panel('panel-list');

        //$('#page-main-content').addClass('open');
        //$('#panel-list').addClass('open');

    });
$(window).on('hashchange', function(e) {
    if ($(window).innerWidth() <= 560) {
        var hash = window.location.hash.split('#');
        close_panel();
    } 
});

$('.drive').on('click touchstart',function(e) {
    if($(window).width()<=560){
        e.preventDefault();
        var addr=$(this).parent().parent().find('p[itemprop="address"]').html();

        $('#address-title').html(addr);
        $('#home-panel-1').hide();
        $('#home-panel-2').hide();
        show_panel('panel-list-2');
        if (history.pushState) {
            history.pushState(null, null, '#map');
        }

        //$('#page-main-content').addClass('open');
        //$('#panel-list').addClass('open');
    }
});

$('.go-back').on('click touchstart',function(e) {
    e.preventDefault();
    close_panel();

});
$('.close-panel').on('click touchstart',function(e) {
    e.preventDefault();
    close_panel();

});
$('.mobile-search-icon').on('click touchstart',function(e) {
    e.preventDefault();

    $('#search-for-mobile').addClass('show').find('input').focus();
});
$('#close-sfm').on('click touchstart',function(e) {
    e.preventDefault();

    $('#search-for-mobile').removeClass('show');
});

/*selecty nowe*/
$('.ch-select').find('select').chosen({no_results_text: "Nie znaleziono ", disable_search_threshold: 10});

/*nowe menu*/
$('.search-city').on('click touchstart',function(e) {
    e.preventDefault();
    $('#home-panel-2').show();
    $('#home-panel-1').hide();
    show_panel('panel-1');
});
$('.search-spec').on('click touchstart',function(e) {
    e.preventDefault();
    $('#home-panel-1').show();
    $('#home-panel-2').hide();
    show_panel('panel-2');
});



$('#main-overlay').on('click touchstart',function(e) {
    e.preventDefault();
    close_panel();
    
});
var selScrollable = '.scroller';
$('body').on('touchstart', selScrollable, function(e) {
    if (e.currentTarget.scrollTop === 0) {
        e.currentTarget.scrollTop = 1;
    } else if (e.currentTarget.scrollHeight === e.currentTarget.scrollTop + e.currentTarget.offsetHeight) {
        e.currentTarget.scrollTop -= 1;
    }
});
$('body').on('touchmove', selScrollable, function(e) {
    e.stopPropagation();
});
});
window.close_panel = function() {
    $('.page-left-panel').addClass('closing');

    $('#page-main-content').removeClass('open');
    var window_width=$(window).innerWidth();
    if(window_width<=460){
        $('#page-main-content').css({
            'transform':'translate3d(0,0,0)'
        });
    }
    else{
        $('#page-main-content').removeClass('open');
    }
    $('#fixed-headers').removeClass('open-1').removeClass('open-2');

    $('.page-left-panel,#panel-list,#panel-list-2').removeClass('open');
    $('#main-overlay').hide().removeClass('show');
    $('body').css('overflow', 'auto');
    $(document).unbind();

}
$(window).resize(function(){
    var mapa = $('.mapa-right').detach();
    if($(window).width()<=560){
        $('#panel-list-2').append(mapa);
    }
    else{

        $('#map-wpr').append(mapa);
    }
    var menu_open=$('.page-left-panel').hasClass('open');
    var window_width=$(window).width();
    if(window_width>560 && menu_open){
        $('.page-left-panel.open').css('left','0');
        $('#page-main-content').css('left','56%');
    }
    else if(window_width<560 && !menu_open){
      $('.page-left-panel').css('left','-100%');
  }
  else if(window_width>560 && !menu_open){
      $('.page-left-panel').css('left','-56%');
  }
  fitSelect('.reservation-services-list','.platnosci-left');
});

window.show_panel = function(el) {
    if(el!='panel-list-2'){


        $('#fixed-headers,#page-main-content').addClass('open-1');
        $('#' + el).addClass('open');
    //$('#page-main-content').addClass('open');
    var window_width=$(window).innerWidth();
    if(window_width<=460){
        $('#page-main-content').css({
            'transform':'translate3d('+window_width+'px,0,0)'
        });
    }
    else{
        $('#page-main-content').addClass('open');
    }
    $('#main-overlay').show().addClass('show');
    $('body').css('overflow', 'hidden');
    $(document).bind('touchmove', function(e) {
        e.preventDefault();
    });
}
else{
    $('#fixed-headers,#page-main-content').addClass('open-2');
    $('#' + el).addClass('open');
    //$('#page-main-content').addClass('open');
    var window_width=$(window).innerWidth();
    if(window_width<=460){
        $('#page-main-content').css({
            'transform':'translate3d(-'+window_width+'px,0,0)'
        });
    }
    else{
        $('#page-main-content').addClass('open');
    }
    $('#main-overlay').show().addClass('show');
    $('body').css('overflow', 'hidden');
    $(document).bind('touchmove', function(e) {
        e.preventDefault();
    });

}
return true;

}

$('.scroller').on('touchstart scroll scrollstart scrollstop', function() {
    var window_width=$(window).width();
    $(this).find('.city-list').each(function(index) {
        var tx = $(this).offset().top;
        var h = $(this).height();
        var d = $(this).data('show');
        if(window_width>560){

            if (tx <= 0 && tx > -h) {
                $('#letters-search').find('li').not($('#l-' + d)).removeClass('active');
                $('#l-' + d).addClass('active');
            }
        }
        else{
            if (tx-65 <= 0 && tx > -h+65) {
                $('#letters-search').find('li').not($('#l-' + d)).removeClass('active');
                $('#l-' + d).addClass('active');
            }


        }
        //console.log( index + ": " + $( this ).attr('id')+"scrolltop"+tx );
    });
});


$('.reservation-services-list').on('change', function() {
 fitSelect('.reservation-services-list','.platnosci-left');
});

jQuery.fn.justtext = function() {
  
    return $(this)  .clone()
            .children()
            .remove()
            .end()
            .text();

};

Array.prototype.removeDuplicates = function (){
  var temp=new Array();
  label:for(i=0;i<this.length;i++){
        for(var j=0; j<temp.length;j++ ){//check duplicates
            if(temp[j]==this[i])//skip if already present 
               continue label;      
        }
        temp[temp.length] = this[i];
  }
  return temp;
 } 
Array.prototype.unique = function() {
    var u = this.concat().sort();
    for (var i = 1; i < u.length; ) {
        if (u[i-1] === u[i])
            u.splice(i,1);
        else
            i++;
    }
    return u;
}
function eliminateDuplicates(arr) {
  var i,
      len=arr.length,
      out=[],
      obj={};
 
  for (i=0;i<len;i++) {
    obj[arr[i]]=0;
  }
  for (i in obj) {
    out.push(i);
  }
  return out;
}
$(window).on('resize', function() {
    var ainput = $('#search-text');
    var alist = $('.alist');
    alist.css({ top: ainput.height()-2 });
});

$(document).ready(function(){
    fitSelect('.reservation-services-list','.platnosci-left');

    // var AUTOCOMPLETE_URL='http://ppstudio.pl/mija/wp-content/themes/mija/query.php?q=';
    // var AUTOCOMPLETE_URL='http://l.ripley.pl/json.php?q=';
    var AUTOCOMPLETE_URL='/lekarze/autocomplete/?query=';

    var ainput = $('#search-text');
    var alist = $('<ul class="alist" />').appendTo(ainput.parent());
    var aitems = $('.alist li');
    var amask = $('<div class="amask" />').appendTo(ainput.parent());
    ainput.on('input', function(e) {
        $('.alist').css({ top: ainput.height()-2 });
        alist.show();
        amask.show();
        if(ainput.val() < 1) { alist.hide(); amask.hide(); }
        $.getJSON( AUTOCOMPLETE_URL + ainput.val(), function( data ) {
            var ainputval = ainput.val();

            alist.html(data.map(function(val) { 
                if(val.type == 'query') {
                    var entry = '<li class="ac-entry"><span class="ac-name fw">' + val.query + '</span></li>';
                    return entry; 
                } else if(val.type == 'doctor') {
                    var photo = val.photo ? '<img src="'+val.photo+'" />' :  '';
                    var entry = '<li class="ac-entry"><a href="'+val.url+'"><div class="ac-photo">'+photo+'</div><div class="ac-name nowr">'+val.query+'</div><span class="ac-city nowr">'+val.cities.join(', ')+'</span><span class="ac-specializations nowr">'+val.specializations.join(', ')+', </span></a></li>';
                    return entry; 
                } else {
                    return false;
                }
            }).join(""));

            if ($('.alist li').length>0) {
                if (ainput.val().toLowerCase()==$('.alist li').eq(0).find('.ac-name').text().toLowerCase().substr(0,ainput.val().length)) {
                var str = alist.find('li').eq(0).find('.ac-name').text().substr(ainput.val().length);
                var parts = ainput.val() + str;
                amask.html(parts).show();       
                } else {
                    amask.html("");
                }       
            } else {
                amask.html("");
            }
            if(ainput.val() < 1) { 
                alist.hide(); 
                amask.hide();
            }
        });
    });

    $(document).click(function(e) {
        if ( $(e.target).closest('.alist').length === 0 ) {
            $('.alist').hide();
        }
    });

    ainput.on('keydown', function(e) {
        if(e.keyCode==9) { //tab 
            e.preventDefault();
            ainput.val(amask.text());
            alist.hide();
        } 
        if(e.keyCode==38) { //up
            e.preventDefault();
            if($('ul.alist li.active').length<1) {
                var el=$('.alist li').eq(0).addClass('active').find('.ac-name');
                ainput.val(el.text());
                amask.text(el.text());
            } else {
                if($('ul.alist li.active').prev().length) {
                    var el=$('ul.alist li.active').removeClass('active').prev().addClass('active').find('.ac-name'); 
                    ainput.val(el.text());
                    amask.text(el.text());
                } 
            }
        }
        if(e.keyCode==40) { //down
            if($('ul.alist li.active').length<1) {
                var el=$('.alist li').eq(0).addClass('active').find('.ac-name');
                ainput.val(el.text());
                amask.text(el.text());
            } else {
                if($('ul.alist li.active').next().length) {
                    var el = $('ul.alist li.active').removeClass('active').next().addClass('active').find('.ac-name'); 
                    ainput.val(el.text())
                    amask.text(el.text());
                }
            }
        }
    });

    $('body').on('click', '.alist li .ac-name', function(e) {
        ainput.val($(e.target).text());
        amask.text($(e.target).text());
        alist.hide();
    });

    var mapa;
    mapa = $('.mapa-right').detach();

   if($(window).width()<=560){
       $('#panel-list-2').append(mapa);
   }
   else{
    $('#map-wpr').append(mapa);
}   

});


window.check_letters_height = function(){
  if($('#letters-search').length>0){
    var wh=$(window).innerHeight()
    var step=2;
    var lh=$('#letters-search li').height() * $('#letters-search li').length;
     if($(window).width()<460){
            wh=wh-32;
            step=1;
        }


    if(lh>wh){

        $('#letters-search').css('padding-top','1%');
        $('#letters-search li').each(function(el,num){
            var h=parseInt($(this).height()) -step;
            $(this).height(h+'px');
        });
        check_letters_height();
    }
}
}

$(window).load(function(){
    check_letters_height();
});


// Generated by CoffeeScript 1.6.1
(function() {
  var COOKIE_NAME, NEW_TEXT, SENT_CLASS, getCookieVal;

  COOKIE_NAME = "AFA";

  NEW_TEXT = "Zgłoszenie wysłane";

  SENT_CLASS = 'sent';

  getCookieVal = function() {
    var cookieVal, cookieValStr;
    cookieValStr = $.cookie(COOKIE_NAME);
    if (!cookieValStr) {
      cookieVal = [];
    } else {
      cookieVal = JSON.parse(cookieValStr);
    }
    return cookieVal;
  };

  window.askForAccessButtons = function(container) {
    return container.find('a.ask span').each(function(i, rawEl) {
      var DOCTOR_ID, spanEl;
      spanEl = $(rawEl);
      DOCTOR_ID = parseInt(spanEl.data('doctor-id'));
      if ($.inArray(DOCTOR_ID, getCookieVal()) !== -1) {
        spanEl.text(NEW_TEXT);
        spanEl.parent().addClass(SENT_CLASS);
        spanEl.parent().find("img").attr("src", spanEl.parent().find("img").attr("data-after"));
        return;
      }
      return spanEl.parent().click(function(e) {
        var cookieVal, el;
        e.preventDefault();
        el = $(e.target);
        if (e.target.tagName === "SPAN") {
          el = el.parent();
        }
        if (el.hasClass(SENT_CLASS)) {
          return;
        }
        el.addClass(SENT_CLASS).find('span').text(NEW_TEXT);
        el.find("img").attr("src", el.find("img").attr("data-after"));
        cookieVal = getCookieVal();
        cookieVal.push(DOCTOR_ID);
        return $.cookie(COOKIE_NAME, JSON.stringify(cookieVal), {
          expires: 7,
          path: '/'
        }, $.post(URL_FRONT_SCHEDULE_API_ACCESS_REQUEST.replace('0', '' + DOCTOR_ID), ''));
      });
    });
  };

  $(function() {
    return askForAccessButtons($('body'));
  });

}).call(this);
 }).call(this);