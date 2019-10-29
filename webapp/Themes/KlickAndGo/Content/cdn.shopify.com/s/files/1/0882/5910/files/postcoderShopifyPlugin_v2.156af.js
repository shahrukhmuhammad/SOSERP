/*
	Copyright Allies Computing Ltd 2014	
    http://www.alliescomputing.com
	version: 2.1
	
	PostCoder Shopify Address Lookup plugin	
	
	*** IMPORTANT: BEFORE USE CHANGE THE SEARCH KEY ***	
    
    This plugin implements an address lookup function
    that can be added to shopify pages.
    
    It checks for the presence of address data and
    inserts the necessary elements
	
	It uses the Postcoder Web V3 service for address lookup.
	
	http://www.alliescomputing.com/products/postcoder-web-api
	
	Before using this service it is necessary to sign up for a
	trial account at
	
	http://www.alliescomputing.com/products/postcoder-web-api/trial	
	
	All customisation options are at the top of the page
	
	Changes:
	version 2.0 date: 30/10/14 
	- added support for Shopify's new "Responsive" checkout layout;
	- added code to trigger widget reload if it is removed
	
*/
function postcoderpluginloader($, SEARCHKEY) {

// common settings
var ACLPCWShopifySettings = {
	gbonly: false, // GB only search
	defaultCountry: 'UK', // default country for search,
	installcss: true,  // whether to install the css contained in the settings: if false, styles should be in external stylesheet
	adaptStyleToPage: true, // attempt further styling modifications to further mimic form style
};

// declare postcoder.shopify namespace 
window.postcoder = window.postcoder || {}; postcoder.shopify = postcoder.shopify || {};

/********************************************************************************
	All Settings
	
	contains amongst other things the html and css used to render the
	address search "widgets"
	
	
*/
if (typeof postcoder.shopify.PluginSettings == 'undefined')
	postcoder.shopify.PluginSettings = {
		searchKey: '', // key may be set here or provided via a parameter
		gbonly: ACLPCWShopifySettings.gbonly,
		defaultCountry: ACLPCWShopifySettings.defaultCountry,
		// settings relating to the rest-call to the postcoder web service
		request: {
			// optional query parameter to distinguish call sites in customer search logs
			identifier: 'ShopifyPlugin',
			// the url of the postcoder service
			// for description of the options, see developer docs at 
			// http://developers.alliescomputing.com/postcoder-web-api/address-lookup/premise
			serviceUrl: 'https://ws.postcoder.com/pcw/{SEARCHKEY}/address/{DATASET}/{SEARCHVAL}/{STATUS}?lines=2&exclude=organisation,country,postcode,posttown,county&identifier={IDENTIFIER}&include=country,fullprovincename&forcemixed=true',
			// use https rather than http
			securehttp: true,
			// timeout (ms)
			timeout: 5000
		},
		// text for labels/messages etc.
		messages: {
		    searchbuttonlabel: "Find Address", // search-box label
			searchboxplaceholdertext: "Postcode or Address", // background search-box text (i.e. placeholder text)			
			selectaddress: '-- Select an Address --', // text to prompt user to select an address from address drop-down list
			noaddresses: '-- No addresses --', // text to inform user that search returned no results
			serviceerror: '-- Error calling address service: {err} --', // error calling service
			invalidsearchkey: "-- Invalid postcoder searchkey -- " // invalid search key
		},

		// settings related to the layout of the search box on the page
		// the plugin inserts a block of html on the page.
		// this can be customised in 2 ways: modifying either the html or the style (css) used
		// the html used can be modified either globally or a per-form basis by creating multiple templates
		// and selecting from within the "targets" section
		layout: {
			// html used to insert the search box into the page
			// layout can be altered at will and elements added/removed, 
			// but the id's and class values should be preserved as they are used internally and in the css styling				
			// several templates may be defined: apply via html property of the target specs
			defaultHtmlTemplate: 'template1',
			
			// list of available templates
			htmlTemplates: {
				// default template			
				'template1': (function() {
					return [
						'<tr id="{id}" class="addresssearch addresssearchcontainer">',
						'<td class="addressSearchLabelContainer">',
						'<input id="{id}-SearchButton" type="button" value="{searchbuttonlabel}" class="addressSearchButton" style="width: auto !important"></input>',						
						'</td>',
						'<td>',
						'<div>',
						'<input type="text" id="{id}-SearchBox" class="addressSearchBox" x-autocompletetype="postal-code" results=5></input>',						
						'</div>',
						'<span id="{id}-AddressListContainer" class="addressSearchListContainer" style="display:none;">',
						'<select id="{id}-AddressList" class="addressSearchList" size=1></select>',
						'</span>',
						'</td>',
						'</tr>',
						'<tr class="addresssearch"><td id="{id}-Divider" colspan=2 class="addresssearchdivider"></td></tr>',
						
					].join('');
					// n.b. this funky code is just a multiline string: 
					// the array notation and join are just a way of easily creating multi-line strings in javascript
				})(),
				'checkout2_shipping_template': (function() {
					return [	
						'<div id="{id}" class="fieldset addresssearch addresssearchcontainer">',
							'<div class="field">',
								'<label class="addressSearchLabelContainer">',
									'<input id="{id}-SearchButton" type="button" value="{searchbuttonlabel}" class="btn addressSearchButton"  style="width: auto !important; padding: 5px; float: left;"></input>',						
								'</label>',
								'<input type="text" id="{id}-SearchBox" class="addressSearchBox" x-autocompletetype="postal-code" results=5></input>',						
								'<span id="{id}-AddressListContainer" class="addressSearchListContainer" style="display:none;  padding-top: 1em; ">',
									'<select id="{id}-AddressList" class="addressSearchList" size=1 style="width: 100%"></select>',
								'</span>',								
							'</div>',							
						'</div>',
					].join('');
					// n.b. this funky code is just a multiline string: 
					// the array notation and join are just a way of easily creating multi-line strings in javascript
				})(),
				'checkout2_billing_template': (function() {
					return [	
						'<div id="{id}" class="field-group addresssearch addresssearchcontainer">',
							'<div class="field">',
								'<label class="addressSearchLabelContainer">',
									'<input id="{id}-SearchButton" type="button" value="{searchbuttonlabel}" class="btn addressSearchButton"  style="width: auto !important; padding: 5px; float: left;"></input>',						
								'</label>',
								'<input type="text" id="{id}-SearchBox" class="addressSearchBox" x-autocompletetype="postal-code" results=5></input>',						
							'<span id="{id}-AddressListContainer" class="addressSearchListContainer" style="display:none; padding-top: 1em;">',
								'<select id="{id}-AddressList" class="addressSearchList" size=1 style="width: 100%"></select>',
							'</span>',								
							'</div>',							
						'</div>',
					].join('');
					// n.b. this funky code is just a multiline string: 
					// the array notation and join are just a way of easily creating multi-line strings in javascript
				})(),
				'checkout3_shipping_template': (function() {
					return [	
						'<div id="{id}" class="fieldset addresssearch addresssearchcontainer">',
							'<div class="field">',
								'<div class="field--two-thirds field field--show-floating-label">',
									'<div class="field__input-wrapper">',
										'<label class="field__label" for="{id}-SearchBox">Postcode or address</label>',
										'<input type="text" id="{id}-SearchBox" class="addressSearchBox" x-autocompletetype="postal-code" results=5></input>',
									'</div>',
								'</div>',
								'<div class="field--third field">',
									'<div class="field__input-wrapper addressSearchLabelContainer">',
										'<input id="{id}-SearchButton" type="button" value="{searchbuttonlabel}" class="btn addressSearchButton"  style="width: 100% !important; padding: .9em 1.7em;"></input>',
									'</div>',
								'</div>',
								'<div class="field field--show-floating-label">',
									'<div class="field__input-wrapper">',
										'<div class="field__input-wrapper field__input-wrapper--select" id="{id}-AddressListContainer">',
											'<label class="field__label" for="{id}-AddressList">Select your address</label>',
											'<select id="{id}-AddressList" class="field__input field__input--select" size="1"></select>',
										'</div>',
									'</div>',
								'</div>',					
							'</div>',							
						'</div>',
					].join('');
					// n.b. this funky code is just a multiline string: 
					// the array notation and join are just a way of easily creating multi-line strings in javascript
				})(),			
			},

			// install the css below: alternatively set to false and add css in an external stylesheet
			installcss: ACLPCWShopifySettings.installcss,
			// css style used with the html
			css: (function() {
				return [
					// these define the various css styles for the plugin's UI
					'@media screen {',
					'.addresssearchcontainer { padding: 3px; background: #f5f5f5; -webkit-border-radius: 4px; -moz-border-radius: 4px; border-radius: 4px; margin-bottom: 15px; border: 1px solid #ccc;}',
					'.addresssearchcontainer .addressSearchLabelContainer {vertical-align: top; text-align: center}',
					'.addresssearchcontainer .addressSearchButton { height: inherit; width: auto; padding: 2px}',
					'.addresssearchcontainer .addressSearchListContainer { width: 100%; padding-top: 2px; display: block; overflow: hidden;  }',
					'#content .addresssearchcontainer select.addressSearchList { width: 90%;  }',					
					'.addresssearchdivider { border-top-style: solid; border-top-width: 1px; border-top-color: gray; }',					
					'}',
				].join('');
			})(),
			// further adapt the style to the page
			adaptStyleToPage: ACLPCWShopifySettings.adaptStyleToPage,
		},
		addressformat: {
			// casing settings: options are 'UPPER', 'LOWER'
			casing: {
				city: {
					// for GB/UK upppercase the town/city line
					'GB': 'UPPER', 
					'UK': 'UPPER'
				}
			}
		},

		// identifies forms on the Shopify page requiring an address lookup
		// i.e. defines target address forms.
		// 
		// - "context" identifies the target
		// - "where" specifies where to put the widget
		// - "fields" defines the address fields which should be populated
		// - "html" (optional) specifies a particular html template to use.  
		//   If none is specified then the default it used (see layout.defaultHtmlTemplate)
		targets: [
			// checkout page, billing
			{
				name: "billing",
				// context matching "selector" for identifying a section of document requiring a "widget": 
				// may be a jquery selector or function.  
				// If the selector or function doesn't match a DOM element then the lookup is not placed on the document
				context: "#addresses #billing",
				
				// where to insert the 'widget'
				where: {					
					insertionpt: function() {
						return $("#addresses #billing_address_first_name").closest('tr');
					},
					mode: "before" // or after
				},
				fields: {
					org: 'billing_address_company',
					addline1: 'billing_address_address1',
					addline2: 'billing_address_address2',
					city: 'billing_address_city',
					province: 'billing_address_province',
					postcode: 'billing_address_zip',
					country: 'billing_address_country'
				},
			},
			// checkout, shipping
			{
				name: "shipping",
				context: "#addresses #shipping",
				where: {
					insertionpt: function() {
						return $("#addresses #shipping_address_first_name").closest('tr');
					},
					mode: "before"
				},
				fields: {
					org: 'shipping_address_company',
					addline1: 'shipping_address_address1',
					addline2: 'shipping_address_address2',
					city: 'shipping_address_city',
					province: 'shipping_address_province',
					postcode: 'shipping_address_zip',
					country: 'shipping_address_country'
				},
			},
			// customer address
			{
				name: "customer address 1",
				context: ".customer_address_table", // "[id^=address_form_]", 
				where: {
					insertionpt: function(context) {
						var e = $('[id^=address_first_name].address_form', context);
						if(e.parent().is('td')) {
							return e.closest('tr');
						}
						else {
							return e.parent();
						}
					},
					mode: "before"
				},
				//html: 'template2', // use a different html template
				fields: {
					org: "[name='address[company]']", //'address_company_new', 
					addline1: 'input[id^=address_address1]',
					addline2: 'input[id^=address_address2]',
					city: 'input[id^=address_city]',
					province: '[id^=address_province]',
					postcode: 'input[id^=address_zip]',
					country: '[id^=address_country]'
				},
			},
			// checkout 2 - shipping address
			{
				name: "checkout2_shipping",
				// for the new (Oct 2014) Checkout page
				context: "div#shipping-address",
				
				// where to insert the 'widget'
				where: {									
					insertionpt: function() {					
						return $("div#shipping-address");
					},
					mode: "before" // or after
				},
				fields: {
					org: 'checkout_shipping_address_company',
					addline1: 'checkout_shipping_address_address1',
					addline2: 'checkout_shipping_address_address2',
					city: 'checkout_shipping_address_city',
					province: 'checkout_shipping_address_province',
					postcode: 'checkout_shipping_address_zip',
					country: 'checkout_shipping_address_country'
				},
				html: 'checkout2_shipping_template', // use a different html template
			},

			// checkout 2 - billing address
			{
				name: "checkout2_billing",
				// for the new (Oct 2014) Checkout page
				context: "div#billing-address",
				
				// where to insert the 'widget'
				where: {									
					insertionpt: function() {					
						return $("#billing-address  > div:nth-child(1)");
					},
					mode: "before" // or after
				},
				fields: {
					org: 'checkout_billing_address_company',
					addline1: 'checkout_billing_address_address1',
					addline2: 'checkout_billing_address_address2',
					city: 'checkout_billing_address_city',
					province: 'checkout_billing_address_province',
					postcode: 'checkout_billing_address_zip',
					country: 'checkout_billing_address_country'
				},
				html: 'checkout2_billing_template', // use a different html template
			},	
			
			// checkout 3 - shipping address
			{
				name: "checkout3_shipping",
				// for the shopify payments address forms
				context: "div.section--shipping-address div.section__content div.fieldset",
				
				// where to insert the 'widget'
				where: {									
					insertionpt: function() {					
						return $("div.section--shipping-address div.section__content");
					},
					mode: "before" // or after
				},
				fields: {
					org: 'checkout_shipping_address_company',
					addline1: 'checkout_shipping_address_address1',
					addline2: 'checkout_shipping_address_address2',
					city: 'checkout_shipping_address_city',
					province: 'checkout_shipping_address_province',
					postcode: 'checkout_shipping_address_zip',
					country: 'checkout_shipping_address_country'
				},
				html: 'checkout3_shipping_template', // use a different html template
			}
		],
		// list of countries (from shopify country-list) to their ISO-3166-1 2-letter codes
		countries: '"Afghanistan"|"AF"|"Albania"|"AL"|"Antarctica"|"AQ"|"Algeria"|"DZ"|"American Samoa"|"AS"|"Andorra"|"AD"|"Angola"|"AO"|"Antigua and Barbuda"|"AG"|"Azerbaijan"|"AZ"|"Argentina"|"AR"|"Australia"|"AU"|"Austria"|"AT"|"Bahamas"|"BS"|"Bahrain"|"BH"|"Bangladesh"|"BD"|"Armenia"|"AM"|"Barbados"|"BB"|"Belgium"|"BE"|"Bermuda"|"BM"|"Bhutan"|"BT"|"Bolivia"|"BO"|"Bosnia and Herzegovina"|"BA"|"Botswana"|"BW"|"Brazil"|"BR"|"Belize"|"BZ"|"British Indian Ocean Territory"|"IO"|"Solomon Islands"|"SB"|"Virgin Islands, British"|"VG"|"Brunei Darussalam"|"BN"|"Brunei"|"BN"|"Bulgaria"|"BG"|"Myanmar"|"MM"|"Burma"|"MM"|"Burundi"|"BI"|"Belarus"|"BY"|"Cambodia"|"KH"|"Cameroon"|"CM"|"Republic of Cameroon"|"CM"|"Canada"|"CA"|"Cape Verde"|"CV"|"Cayman Islands"|"KY"|"Central African Republic"|"CF"|"Sri Lanka"|"LK"|"Chad"|"TD"|"Chile"|"CL"|"China"|"CN"|"Taiwan"|"TW"|"Christmas Island"|"CX"|"Cocos (Keeling) Islands"|"CC"|"Colombia"|"CO"|"Comoros"|"KM"|"Mayotte"|"YT"|"Congo"|"CG"|"Congo, The Democratic Republic Of The"|"CD"|"Cook Islands"|"CK"|"Costa Rica"|"CR"|"Croatia"|"HR"|"Cuba"|"CU"|"Cyprus"|"CY"|"Czech Republic"|"CZ"|"Benin"|"BJ"|"Denmark"|"DK"|"Dominica"|"DM"|"Dominican Republic"|"DO"|"Ecuador"|"EC"|"El Salvador"|"SV"|"Equatorial Guinea"|"GQ"|"Ethiopia"|"ET"|"Eritrea"|"ER"|"Estonia"|"EE"|"Faröe Islands"|"FO"|"Faroe Islands"|"FO"|"Falkland Islands (Malvinas)"|"FK"|"South Georgia and The South Sandwich Islands"|"GS"|"Fiji"|"FJ"|"Finland"|"FI"|"Aland Islands"|"AX"|"France"|"FR"|"French Guiana"|"GF"|"French Polynesia"|"PF"|"French Southern Territories"|"TF"|"Djibouti"|"DJ"|"Gabon"|"GA"|"Georgia"|"GE"|"Gambia"|"GM"|"Palestinian Territory"|"PS"|"Palestinian Territory, Occupied"|"PS"|"Germany"|"DE"|"Ghana"|"GH"|"Gibraltar"|"GI"|"Kiribati"|"KI"|"Greece"|"GR"|"Greenland"|"GL"|"Grenada"|"GD"|"Guadeloupe"|"GP"|"Guam"|"GU"|"Guatemala"|"GT"|"Guinea"|"GN"|"Guyana"|"GY"|"Haiti"|"HT"|"Vatican City"|"VA"|"Holy See (Vatican City State)"|"VA"|"Honduras"|"HN"|"Hong Kong"|"HK"|"Hungary"|"HU"|"Iceland"|"IS"|"India"|"IN"|"Indonesia"|"ID"|"Iran"|"IR"|"Iran, Islamic Republic Of"|"IR"|"Iraq"|"IQ"|"Ireland"|"IE"|"Israel"|"IL"|"Italy"|"IT"|"Côte d\'Ivoire"|"CI"|"Jamaica"|"JM"|"Japan"|"JP"|"Kazakhstan"|"KZ"|"Jordan"|"JO"|"Kenya"|"KE"|"Korea, Democratic People\'s Republic Of"|"KP"|"South Korea"|"KR"|"Kuwait"|"KW"|"Kyrgyzstan"|"KG"|"Laos"|"LA"|"Lao People\'s Democratic Republic"|"LA"|"Lebanon"|"LB"|"Lesotho"|"LS"|"Latvia"|"LV"|"Liberia"|"LR"|"Libya"|"LY"|"Libyan Arab Jamahiriya"|"LY"|"Liechtenstein"|"LI"|"Lithuania"|"LT"|"Luxembourg"|"LU"|"Macao"|"MO"|"Madagascar"|"MG"|"Malawi"|"MW"|"Malaysia"|"MY"|"Maldives"|"MV"|"Mali"|"ML"|"Malta"|"MT"|"Martinique"|"MQ"|"Mauritania"|"MR"|"Mauritius"|"MU"|"Mexico"|"MX"|"Monaco"|"MC"|"Mongolia"|"MN"|"Moldova"|"MD"|"Moldova, Republic Of"|"MD"|"Montenegro"|"ME"|"Montserrat"|"MS"|"Morocco"|"MA"|"Mozambique"|"MZ"|"Oman"|"OM"|"Namibia"|"NA"|"Nauru"|"NR"|"Nepal"|"NP"|"Netherlands"|"NL"|"Curacao"|"CW"|"Curaçao"|"CW"|"Aruba"|"AW"|"Sint Maarten"|"SX"|"Bonaire, Sint Eustatius And Saba"|"BQ"|"New Caledonia"|"NC"|"Vanuatu"|"VU"|"New Zealand"|"NZ"|"Nicaragua"|"NI"|"Niger"|"NE"|"Nigeria"|"NG"|"Niue"|"NU"|"Norfolk Island"|"NF"|"Norway"|"NO"|"Northern Mariana Islands"|"MP"|"United States Minor Outlying Islands"|"UM"|"Micronesia"|"FM"|"Marshall Islands"|"MH"|"Palau"|"PW"|"Pakistan"|"PK"|"Panama"|"PA"|"Papua New Guinea"|"PG"|"Paraguay"|"PY"|"Peru"|"PE"|"Philippines"|"PH"|"Pitcairn"|"PN"|"Poland"|"PL"|"Portugal"|"PT"|"Guinea Bissau"|"GW"|"Timor Leste"|"TL"|"Puerto Rico"|"PR"|"Qatar"|"QA"|"Réunion"|"RE"|"Reunion"|"RE"|"Romania"|"RO"|"Russia"|"RU"|"Rwanda"|"RW"|"Saint Barthélemy"|"BL"|"Saint Helena"|"SH"|"Saint Kitts and Nevis"|"KN"|"Anguilla"|"AI"|"Saint Lucia"|"LC"|"Saint Martin"|"MF"|"Saint Pierre and Miquelon"|"PM"|"St. Vincent"|"VC"|"San Marino"|"SM"|"Sao Tomé and Principe"|"ST"|"Sao Tome and Principe"|"ST"|"Saudi Arabia"|"SA"|"Senegal"|"SN"|"Serbia"|"RS"|"Seychelles"|"SC"|"Sierra Leone"|"SL"|"Singapore"|"SG"|"Slovakia"|"SK"|"Vietnam"|"VN"|"Slovenia"|"SI"|"Somalia"|"SO"|"South Africa"|"ZA"|"Zimbabwe"|"ZW"|"Spain"|"ES"|"South Sudan"|"SS"|"Sudan"|"SD"|"Western Sahara"|"EH"|"Suriname"|"SR"|"Svalbard and Jan Mayen"|"SJ"|"Swaziland"|"SZ"|"Sweden"|"SE"|"Switzerland"|"CH"|"Syria"|"SY"|"Tajikistan"|"TJ"|"Thailand"|"TH"|"Togo"|"TG"|"Tokelau"|"TK"|"Tonga"|"TO"|"Trinidad and Tobago"|"TT"|"United Arab Emirates"|"AE"|"Tunisia"|"TN"|"Turkey"|"TR"|"Turkmenistan"|"TM"|"Turks and Caicos Islands"|"TC"|"Tuvalu"|"TV"|"Uganda"|"UG"|"Ukraine"|"UA"|"Macedonia, TFYR"|"MK"|"Macedonia, Republic Of"|"MK"|"Egypt"|"EG"|"United Kingdom"|"UK"|"Guernsey"|"GG"|"Jersey"|"JE"|"Isle of Man"|"IM"|"Tanzania"|"TZ"|"Tanzania, United Republic Of"|"TZ"|"United States of America"|"US"|"United States"|"US"|"United States Virgin Islands"|"VI"|"Burkina Faso"|"BF"|"Uruguay"|"UY"|"Uzbekistan"|"UZ"|"Venezuela"|"VE"|"Wallis and Futuna"|"WF"|"Samoa"|"WS"|"Yemen"|"YE"|"Zambia"|"ZM"|"Netherlands Antilles"|"AN"|"Kosovo"|"XK"'
	};


/********************************************************************************
	define the shopify plugin class	
*/
if (typeof postcoder.shopify.Plugin == 'undefined')
	postcoder.shopify.Plugin = function(settings) {
		//*** SETTINGS ***	
		this.settings = settings;
		this.accountValid = true;
		var widgets = [];
		this.widgets = widgets;

		//if( this.settings.searchKey == 'PCW9L-S6CL7-ZQV2D-YHMRP' ){
		//	this.accountValid = false;
		//	if (typeof(console) != "undefined"){
		//		console.error("please enter a valid postcoder search key");
		//	} 
		//}	
		// load countries
		this.countryiso = {};		
		(function(dest, src) {
			var countriesandcodes = src.split('|');
			for (var i = 0; i < countriesandcodes.length; i += 2) {
				dest[countriesandcodes[i].replace(/\"/g, '').toLowerCase()] = countriesandcodes[i + 1].replace(/\"/g, '').toUpperCase();
			}
		})(this.countryiso, settings.countries);

		this.countrysearchenabled = {};
		
		// install the standard css styling: can be turned off with installcss flag in settings
		// alternaively set in an external stylesheet and set settings.layout.installcss to false
		if (settings.layout.installcss) {
			$("<style>" + settings.layout.css + "</style>").appendTo("head");
		}

		/// function to add the various search widgets to the page
		this.addSearchWidgets = function() {
			// iterate through the widget specs in the settings			 
			var plugin = this;
			$.each(this.settings.targets, function(index, widgetSpec) {
				addWidgetsForSpec(widgetSpec, plugin);
			});
			return this;
		}

		/// add search widgets for all page elements that match the widget spec
		function addWidgetsForSpec(widgetSpec, plugin) {
			var found = false;
			switch (typeof widgetSpec.context) {
				case 'string':
					found = $(widgetSpec.context);
					break;
				case 'function':
					found = widgetSpec.context();
					break;
			}
			if (found) {				
				if (typeof found === 'boolean' || typeof found === 'number' || (typeof found === 'object' && found.length == 1)) {
					// add single address lookup
					addSingleWidget(widgetSpec, plugin);
				} else if (typeof found === 'object' && found.length > 1) {
					// >1 element matchted the context selector: add multiple lookups
					found.each(function(i, context) {
						addSingleWidget(widgetSpec, plugin, context);
					});
				}
			}
		}

		/// add a single address lookup widget
		function addSingleWidget(widgetSpec, plugin, context) {
			var w = new postcoder.shopify.Plugin.Widget(widgetSpec, plugin, context);
			if (w && w.valid) {
				//console.log("added: " + widgetSpec.name);
				plugin.widgets.push(w);
			}
		}
	};


/********************************************************************************
	Address Lookup Widget class
*/
if (typeof postcoder.shopify.Plugin.Widget == 'undefined')
	postcoder.shopify.Plugin.Widget = function(options, plugin, context) {
		this.valid = false;
		this.context = context;
		var idprefix = "pcw-" + options.name.replace(/ /g, '-');
		this.id = idprefix + "-" + pseudoGuid();

		//console.log("trying to add: " + options.name);
		// setup the html we will use in the address form
		function UIElements(widget, options) {
			this.valid = false;
			var re = new RegExp("{id}", 'g');
			this.id = widget.id;
			this.widget = widget;
			var obj = this;
			options.html = options.html && plugin.settings.layout.htmlTemplates.hasOwnProperty(options.html) ?
				options.html : plugin.settings.layout.defaultHtmlTemplate;

			var sblabel = (options.searchbuttonlabel ? options.searchbuttonlabel : plugin.settings.messages.searchbuttonlabel);

			var html = plugin.settings.layout.htmlTemplates[options.html]
				.replace(re, this.id)
				.replace(/{searchbuttonlabel}/g, sblabel);

			this.container = $(html);
			
			var insertionpt = null;

			switch (typeof options.where.insertionpt) {
				case 'string':
					insertionpt = $(options.where.insertionpt, context);
					break;
				case 'function':
					insertionpt = options.where.insertionpt(context);
					break;
			}
			if (!insertionpt || insertionpt.length === 0)
				return;

			// ensure we don't add this widget multiple times
			if (insertionpt.siblings("[id^=" + idprefix + "]").length > 0) {
				//console.log("not adding (duplicates): " + options.name);
				return;
			}

			// finally, insert
			if (options.where.mode == "before") {
				this.container.insertBefore(insertionpt);
			} else if (options.where.mode == "after") {
				this.container.insertAfter(insertionpt);
			} else {
				this.container.insertAfter(insertionpt);
			}

			var searchButton = $((options.searchButtonSelector ? options.searchButtonSelector : "#{id}-SearchButton").replace(re, this.id));
			var addressListContainer = $((options.addressListContainer ? options.addressListContainer : "#{id}-AddressListContainer").replace(re, this.id));
			var addressListDropDown = $((options.addressListDropDownSelector ? options.addressListDropDownSelector : "#{id}-AddressList").replace(re, this.id));
			var searchBox = $((options.searchBoxSelector ? options.searchBoxSelector : "#{id}-SearchBox").replace(re, this.id));

			var addresssearchdivider = $("#{id}-Divider".replace(re, this.id));
			//this.searchBox = searchBox;
			//this.searchButton: searchButton;
			//this.addressListDropDown = addressListDropDown;
			if(searchButton) {
				searchButton.on("DOMNodeRemovedFromDocument", function () {
				//console.log("Widget removed: " + widget.id);
				// may need a reload
				
				setTimeout(function() {
					//console.log("reloading");
					postcoder.shopify.plugin.addSearchWidgets();
					}, 1000);
				});
			}

			this.show = function() {
				this.container.show();
			};
			this.hide = function() {
				this.container.hide();
			};
			this.searchterm = function() {
				return searchBox ? searchBox.val().trim() : '';
			};
			// display the address list
			this.displayAddressList = function(mode) {
				var o = (addressListContainer) ? addressListContainer : addressListDropDown;
				if (typeof mode === 'string' && mode.toLowerCase() === 'show')
					o.show();
				else
					o.hide();
			};

			// clear the address list
			this.clearAddressList = function() {
				addressListDropDown.find('option').remove();
			};

			// add an item to the address list
			this.addToAddressList = function(text, val) {
				var element = addressListDropDown[0];
				element.options.add(new Option(text, val));
			};

			// function to open the address list dropdown (n.b. not all browsers support this)
			this.openAddressList = function() {
				var element = addressListDropDown[0];
				if (document.createEvent) { // all browsers
					var evt = document.createEvent("MouseEvents");
					evt.initMouseEvent("mousedown", true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
					worked = element.dispatchEvent(evt);
				} else if (element.fireEvent) { // ie
					worked = element.fireEvent("onmousedown");
				}
			};
			this.searchBtnClickCallback = function() {};
			this.addressSelectedCallback = function() {};
			this.searchBoxEnterKeyPressCallback = function() {};

			// install an on-change callback to the address list dropdown
			addressListDropDown.change(function() {
				var selectedIndex = addressListDropDown.prop ? addressListDropDown.prop('selectedIndex') : addressListDropDown.attr('selectedIndex');
				if (obj.addressSelectedCallback && typeof obj.addressSelectedCallback === 'function')
					obj.addressSelectedCallback(selectedIndex);
			});
			// install click handler on the search button
			if (searchButton) {
				searchButton.click(function(e) {
					if (obj.searchBtnClickCallback && typeof obj.searchBtnClickCallback === 'function')
						return obj.searchBtnClickCallback(e);
					return true;
				});
			}

			// install keypress handler on searchbox to capture an enter key that alters the Enter Key Behaviour on the Postcode field to do address lookup  
			searchBox.keypress(function(e) {
				if (e.which == 13 && obj.searchBoxEnterKeyPressCallback && typeof obj.searchBoxEnterKeyPressCallback === 'function') {
					obj.searchBoxEnterKeyPressCallback(e);
					return false;
				}
				return true;
			});

			// set searchbox placeholder attribute
			if (!searchBox.val()) {
				searchBox.attr("placeholder", plugin.settings.messages.searchboxplaceholdertext);
			}

			if(plugin.settings.layout.adaptStyleToPage) {	
				// further adapt to local page styling				
				var tf = getField(options.fields.addline1, context);
				
				var classList = tf[0].className.split(/\s+/), c;
				for ( var i = 0; i < classList.length; i++ ) {
					c = classList[i];
					if(!searchBox.hasClass(c)) {
						searchBox.addClass(c);					 
					};
				}
				//searchBox.css('padding', tf.css('padding'));
				//searchBox.css('margin', tf.css('margin'));
				//searchBox.outerHeight(tf.outerHeight());
				//searchBox.css('width',tf.width() + 'px');
				//searchBox.attr('style', function(i,s) { return s + 'width: '+ tf.width() + 'px !important;' });
				//searchButton.outerHeight(searchBox.outerHeight());
				
				addresssearchdivider.css('border-top-color', tf.css('border-top-color'));
			}
			
			// hook into the taborder
			var firstAddrField = this.container.nextAll('input')[0];
			if(firstAddrField) {
				var ti = firstAddrField.tabIndex;
				searchBox.attr('tabindex',ti);
				addressListDropDown.attr('tabindex',ti);
			}									
						
			// hook the widget into the dom object
			this.container.data("pcw", widget);
			this.valid = true;
		}

		this.UI = new UIElements(this, options);
		
		if(!this.UI || !this.UI.valid) {
			return;
		}
		// output object
		var obj = this;

		this.searchpending = false;
		this.name = options.name;

		this.addressSelectedCallback = addressSelectedCallback;
		this.targetFields = options.fields;
		this.lastrequest = null;
		this.selectedcountry = plugin.settings.defaultCountry;

		// setup countries
		this.countryfield = getField(options.fields.country, context);
		
		// give each country in the dropdown its iso-3166-1/2 code
		(function() {
			var country_options = $("option", obj.countryfield);
			$.each(country_options, function(i, o) {
				var countryname = o.value;
				countryname = countryname.trim();
				if (countryname.length > 0) {
					var countrycode = plugin.countryiso[countryname.toLowerCase()];
					if (countrycode) {
						$(o).data("iso2", countrycode);
					}
				}
			});
		})();

		this.countryfield.change(onCountrySelect);

		// hide the address list component
		
		this.UI.displayAddressList('show');
		noaddresses();
		
		// when search button is pressed, do address lookup  
		this.UI.searchBtnClickCallback = initiateAddressLookup;

		// Alters the Enter Key Behaviour on the Postcode field to do address lookup  
		this.UI.searchBoxEnterKeyPressCallback = initiateAddressLookup;

		function initiateAddressLookup() {
			lookupAddress(obj.UI.searchterm(), obj.selectedcountry, onRequestBegin, onRequestSucceeded, onRequestFailed, plugin.settings);
			return false;
		}

		//var form = searchBox.closest('form');
		//if(form && form[0] && ("requestAutocomplete" in form[0])) {
		//	form[0].requestAutocomplete();
		//}

		//searchBox.on('click', function(e) {
		// var searchterm = obj.searchBox.val();
		// lookupAddress
		//});

		// callback on selecting a new county
		function onCountrySelect() {
			var selected = obj.countryfield.find(":selected");
			var countrytext = selected[0].value;
			var iso = selected.data("iso2");
			if (iso) {
				obj.selectedcountry = iso;
			} else {
				// no iso mapping        
				if (countrytext) {
					// there was a country value
					obj.selectedcountry = '';
				} else {
					// no actual country selected
					obj.selectedcountry = plugin.settings.defaultCountry;
				}
			}
			//obj.lastrequest = null;
			//obj.UI.clearAddressList();
			obj.UI.displayAddressList('show');

			if (obj.selectedcountry && ((obj.selectedcountry === 'GB' || obj.selectedcountry === 'UK') || !plugin.settings.gbonly)) {
				// lookupTest(obj.selectedcountry, function(data) {
					// if (data == "Ok") {
						// obj.show();
					// } else {
						// obj.hide();
					// }
				// }, function() {
					// obj.hide();
				// }, plugin.settings);
				obj.show();
			} else {
				obj.hide();
			}
		}

		// called when the request is about to begin
		function onRequestBegin(request) {
			var lastrequest = obj.lastrequest;
			// conditins for doing a search
			// - not already doing a search
			// - request has a search term
			// - country or search term different from previous one
			var dosearch = (!obj.searchpending && request && request.country && request.searchTerm 
				&& (!lastrequest || (lastrequest.country != request.country) || (lastrequest.country == request.country && lastrequest.searchTerm != request.searchTerm))) ? true : false;
			if (dosearch) {
				obj.searchpending = true;
				obj.UI.clearAddressList();
				obj.lastrequest = request;
			} else if (lastrequest && request && lastrequest.country == request.country && lastrequest.searchTerm == request.searchTerm && lastrequest.addressData) {
				// already did this search, so just re-show the results
				onRequestSucceeded.call(obj.lastrequest, obj.lastrequest.addressData);
			} else {
				noaddresses();
			}
			return dosearch;
		}

		// no addresses to show
		function noaddresses() {
			var ui = obj.UI;
			obj.lastrequest = null;
			ui.clearAddressList();
			ui.addToAddressList(plugin.settings.messages.noaddresses, '-1');
			ui.displayAddressList('show');
		}

		// called if/when the service request returns successfully
		function onRequestSucceeded(data) {
			var request = this;
			var newdata = request.addressData != data;
			var ui = obj.UI;

			if (newdata) {
				request.addressData = data; // save data returned from the service
				
				// Clear the current dropdown
				ui.clearAddressList();

				// sets the function that sets the other address fields when an address is selected from the drop down
				ui.addressSelectedCallback = function(selectedIndex) {
					obj.addressSelectedCallback(request, selectedIndex - 1);
				};
				ui.addToAddressList(plugin.settings.messages.selectaddress, '-1');
				// add an entry to dropdown for each address returned
				$.each(data, function(index, value) {
					ui.addToAddressList(value.summaryline, index);
				});
			}

			if (data.length == 1) { // single address found so use it
				//ui.displayAddressList('hide');
				obj.addressSelectedCallback(request, 0);
			} else if (data.length > 0) { // multiple records so display a select box.
				ui.displayAddressList('show');
				ui.openAddressList();
			} else { // no entries
				noaddresses();
			}
			obj.searchpending = false;
		}

		// called if/when the service request returns failure
		function onRequestFailed(jqxhr, textStatus, error) {
			//addressListContainer.show();	
			//var errstring = textStatus != 'error' || !error ? textStatus : error;	
			//var msg = plugin.settings.messages.serviceerror.replace('{err}', errstring);
			//addressListDropDown[0].options.add(new Option(msg,'-1'));
			noaddresses();
			if (typeof(console) != "undefined") {
				console.error("getJSON failed, status: " + textStatus + ", error: " + error);
			}
			obj.searchpending = false;
		}

		// callback when an address is selected in the address-list drop down
		// populates the page form fields
		function addressSelectedCallback(request, selectedIndex) {
			var addressData = request.addressData;
			if (isNaN(selectedIndex) || selectedIndex < 0 || selectedIndex >= addressData.length) {
				return;
			}			

			var selectedAddress = addressData[selectedIndex];
			
			var targetFields = obj.targetFields;
			var casingsettings = plugin.settings.addressformat.casing;
			// insert the data
			setField(getField(targetFields.org, obj.context), doCasing("org", selectedAddress.organisation, casingsettings, request.country));
			setField(getField(targetFields.addline1, obj.context), doCasing("addline1", selectedAddress.addressline1, casingsettings, request.country));
			setField(getField(targetFields.addline2, obj.context), doCasing("addline2", selectedAddress.addressline2, casingsettings, request.country));
			setField(getField(targetFields.city, obj.context), doCasing("city", selectedAddress.posttown, casingsettings, request.country));
			setField(getField(targetFields.province, obj.context),
				doCasing("province", selectedAddress.fullprovincename ? selectedAddress.fullprovincename : selectedAddress.county ? selectedAddress.county : selectedAddress.state ? selectedAddress.state : '',
					casingsettings, request.country));					
			setField(getField(targetFields.postcode, obj.context), doCasing("postcode", selectedAddress.postcode, casingsettings, request.country));
			
			// set country too
			(function() {
				var country_options = $("option", obj.countryfield);
				var cntrytext = '';
				$.each(country_options, function(i, o) {
					var countryMatch = false;					
					if (o.text && typeof o.text === 'string' && selectedAddress.country && o.text.toLowerCase() == selectedAddress.country.toLowerCase()) {
						countryMatch = true;
					} else if ($(o).data("iso2") == request.country) {
						countryMatch = true;
					}
					if (countryMatch) {
						cntrytext = o.text;
						return false; // break
					}					
				});
				if(obj.countryfield.val() != cntrytext) {
					setField(obj.countryfield,cntrytext);
				}
			})();
		}

		function getField(fieldselector, context) {
			var fldobj = null;
			if (typeof fieldselector === 'string') {
				fieldselector.trim();
				if (fieldselector == '')
					return;
				fldobj = $(fieldselector, context);
				if (fldobj == null || fldobj.length == 0) {
					fieldselector = (fieldselector[0] === '#' ? fieldselector : "#" + fieldselector);
					fldobj = $(fieldselector, context);
				}
			} else if (typeof fieldselector === 'function') {
				fldobj = fieldselector(context);
			}
			if (fldobj == null || fldobj.length == 0)
				return;
			return fldobj;
		}

		function setField(field, value) {
			if (field == null || field.length == 0)
				return;
			field.val(value);
			field.change();
		}

		// casing support
		function doCasing(fldtype, fldval, settings, country) {

			fldtype = fldtype.toLowerCase();
			if (settings[fldtype]) {
				var fldcaserules = settings[fldtype];
				var rule = null;
				if (fldcaserules) {
					if (typeof fldcaserules === 'string') {
						rule = fldcaserules;
					} else if(typeof fldcaserules === 'object') {
						if (fldcaserules.hasOwnProperty(country)) {
							rule = fldcaserules[country];
						}					
						else if (fldcaserules.hasOwnProperty('*')) {
							rule = fldcaserules['*'];
						}
					}
				}
				if (rule) {
					switch (rule.toLowerCase()) {
						case "upper":
							fldval = fldval.toUpperCase();
							break;
						case "lower":
							fldval = fldval.toLowerCase();
							break;
						default:
							break; // leave as-is (mixed case)
					}
				}
			}
			return fldval;
		}

		// Address Lookup function
		function lookupAddress(searchTerm, country, onRequestBegin, onRequestSucceeded, onRequestFailed, settings) {

			var request = {
				searchKey: SEARCHKEY ? SEARCHKEY : settings.searchKey,
				country: country,
				searchTerm: searchTerm ? searchTerm.trim() : '',
				identifier: settings.request.identifier,
				addressData: null
			};

			if (onRequestBegin) {
				if (!onRequestBegin(request))
					return;
			}

			// if (settings.searchKey == 'PCW9L-S6CL7-ZQV2D-YHMRP' || !plugin.accountValid) {
				// if (onRequestFailed) {
					// onRequestFailed.call(request, null, settings.messages.invalidsearchkey, null);
				// }
				// return;
			// }
			// Build the RESTful URL
			request.url = settings.request.serviceUrl
				.replace('{SEARCHKEY}', request.searchKey)
				.replace('{DATASET}', request.country)
				.replace('{SEARCHVAL}', encodeURIComponent(request.searchTerm))
				.replace('{IDENTIFIER}', encodeURIComponent(request.identifier))
				.replace('{STATUS}', '')
				.replace('https://', settings.request.securehttp ? 'https://' : 'http://');
			// Make the actual JSON request
			//$.getJSON(ajaxURL, onRequestSucceeded ).fail(onRequestFailed);      	
			$.ajax({
				dataType: "json",
				url: request.url,
				context: request,
				timeout: settings.request.timeout,
				success: onRequestSucceeded,
				error: onRequestFailed,
			});
		}

		function lookupTest(country, onRequestSucceeded, onRequestFailed, settings) {
				if(plugin.countrysearchenabled && plugin.countrysearchenabled.hasOwnProperty(country)) {
					if(plugin.countrysearchenabled[country]) {
						onRequestSucceeded("Ok");
					}
					else {
						onRequestFailed();
					}
					return;
				}
				// Build the RESTful URL
				var ajaxURL = settings.request.serviceUrl
					.replace('{SEARCHKEY}', SEARCHKEY ? SEARCHKEY : settings.searchKey)
					.replace('{DATASET}', country)
					.replace('{SEARCHVAL}', encodeURIComponent("test"))
					.replace('{STATUS}', 'Status')
					.replace('{IDENTIFIER}', encodeURIComponent(settings.request.identifier))
					.replace('https://', settings.request.securehttp ? 'https://' : 'http://');

				// Make the actual JSON request
				$.ajax({
					dataType: "json",
					url: ajaxURL,
					timeout: settings.request.timeout,
					success: onsuccess,
					error: onfail,
				});				
				//$.getJSON(ajaxURL, onsuccess).fail(onfail);
				
				function onsuccess(data) {
					plugin.countrysearchenabled[country] = true;
					onRequestSucceeded(data);
				}
				function onfail(jqxhr, textStatus, error) { 	
					plugin.countrysearchenabled[country] = false;
					onRequestFailed(jqxhr, textStatus, error);
				}
			}
			// utility function for returning a pseudo-guid

		function pseudoGuid() {
			function s4() {
				return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
			}
			return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
		}

		this.hide = function() {
			this.UI.hide();
			return this;
		};
		this.show = function() {
			this.UI.show();
			return this;
		};
		this.valid = true;

		onCountrySelect();
	};

	//console.log("postcoder-shopify-plugin: running inner install script");
// define the plugin instance
if (typeof postcoder.shopify.plugin == 'undefined')
	postcoder.shopify.plugin = new postcoder.shopify.Plugin(postcoder.shopify.PluginSettings);
	
// add the widgets
if (typeof postcoder.shopify.plugin != 'undefined')
	postcoder.shopify.plugin.addSearchWidgets();	
	
} // end of postcoderpluginloader function

(function () {
	//console.log("postcoder-shopify-plugin: running outer install script");
	var searchkey = window.postcoder ? window.postcoder.postcoderkey : null;	
    if( window.jQuery ) {
        postcoderpluginloader(window.jQuery, searchkey);
    } else {
		// load jQuery if not already loaded
		var s = document.createElement('script'); s.type = 'text/javascript';s.async = true;s.src = '//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js';
		s.onload = function(script) { jq = jQuery.noConflict(); postcoderpluginloader(jq,searchkey); };	
		var x = document.getElementsByTagName('head')[0];x.parentNode.appendChild(s, x);
    }
})();


