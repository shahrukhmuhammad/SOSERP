using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace BaseApp.System
{
    public static class AspNetExtensions
    {
        public static bool IsPost(this HttpRequestBase request)
        {
            return string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase);
        }

        #region US States List

        private static readonly string[] USStatesName = new string[]
        {
            "Alabama",
            "Alaska",
            "Arizona",
            "Arkansas",
            "California",
            "Colorado",
            "Connecticut",
            "Delaware",
            "Florida",
            "Georgia",
            "Hawaii",
            "Idaho",
            "Illinois",
            "Indiana",
            "Iowa",
            "Kansas",
            "Kentucky",
            "Louisiana",
            "Maine",
            "Maryland",
            "Massachusetts",
            "Michigan",
            "Minnesota",
            "Mississippi",
            "Missouri",
            "Montana",
            "Nebraska",
            "Nevada",
            "New Hampshire",
            "New Jersey",
            "New Mexico",
            "New York",
            "North Carolina",
            "North Dakota",
            "Ohio",
            "Oklahoma",
            "Oregon",
            "Pennsylvania",
            "Rhode Island",
            "South Carolina",
            "South Dakota",
            "Tennessee",
            "Texas",
            "Utah",
            "Vermont",
            "Virginia",
            "Washington",
            "West Virginia",
            "Wisconsin",
            "Wyoming",
        };
        #endregion

        public static IEnumerable<string> USStatesList(this HtmlHelper helper)
        {
            return USStatesName;
        }

        #region Country Names
        private static readonly string[] CountryNames = new string[]
    {
    "Afghanistan",
    "Albania",
    "Algeria",
    "American Samoa",
    "Andorra",
    "Angola",
    "Anguilla",
    "Antarctica",
    "Antigua and Barbuda",
    "Argentina",
    "Armenia",
    "Aruba",
    "Australia",
    "Austria",
    "Azerbaijan",
    "Bahamas",
    "Bahrain",
    "Bangladesh",
    "Barbados",
    "Belarus",
    "Belgium",
    "Belize",
    "Benin",
    "Bermuda",
    "Bhutan",
    "Bolivia",
    "Bosnia and Herzegovina",
    "Botswana",
    "Bouvet Island",
    "Brazil",
    "British Indian Ocean Territory",
    "Brunei Darussalam",
    "Bulgaria",
    "Burkina Faso",
    "Burundi",
    "Cambodia",
    "Cameroon",
    "Canada",
    "Cape Verde",
    "Cayman Islands",
    "Central African Republic",
    "Chad",
    "Chile",
    "China",
    "Christmas Island",
    "Cocos (Keeling) Islands",
    "Colombia",
    "Comoros",
    "Congo",
    "Congo, the Democratic Republic of the",
    "Cook Islands",
    "Costa Rica",
    "Cote D'Ivoire",
    "Croatia",
    "Cuba",
    "Cyprus",
    "Czech Republic",
    "Denmark",
    "Djibouti",
    "Dominica",
    "Dominican Republic",
    "Ecuador",
    "Egypt",
    "El Salvador",
    "Equatorial Guinea",
    "Eritrea",
    "Estonia",
    "Ethiopia",
    "Falkland Islands (Malvinas)",
    "Faroe Islands",
    "Fiji",
    "Finland",
    "France",
    "French Guiana",
    "French Polynesia",
    "French Southern Territories",
    "Gabon",
    "Gambia",
    "Georgia",
    "Germany",
    "Ghana",
    "Gibraltar",
    "Greece",
    "Greenland",
    "Grenada",
    "Guadeloupe",
    "Guam",
    "Guatemala",
    "Guinea",
    "Guinea-Bissau",
    "Guyana",
    "Haiti",
    "Heard Island and Mcdonald Islands",
    "Holy See (Vatican City State)",
    "Honduras",
    "Hong Kong",
    "Hungary",
    "Iceland",
    "India",
    "Indonesia",
    "Iran, Islamic Republic of",
    "Iraq",
    "Ireland",
    "Israel",
    "Italy",
    "Jamaica",
    "Japan",
    "Jordan",
    "Kazakhstan",
    "Kenya",
    "Kiribati",
    "Korea, Democratic People's Republic of",
    "Korea, Republic of",
    "Kuwait",
    "Kyrgyzstan",
    "Lao People's Democratic Republic",
    "Latvia",
    "Lebanon",
    "Lesotho",
    "Liberia",
    "Libyan Arab Jamahiriya",
    "Liechtenstein",
    "Lithuania",
    "Luxembourg",
    "Macao",
    "Macedonia, the Former Yugoslav Republic of",
    "Madagascar",
    "Malawi",
    "Malaysia",
    "Maldives",
    "Mali",
    "Malta",
    "Marshall Islands",
    "Martinique",
    "Mauritania",
    "Mauritius",
    "Mayotte",
    "Mexico",
    "Micronesia, Federated States of",
    "Moldova, Republic of",
    "Monaco",
    "Mongolia",
    "Montserrat",
    "Morocco",
    "Mozambique",
    "Myanmar",
    "Namibia",
    "Nauru",
    "Nepal",
    "Netherlands",
    "Netherlands Antilles",
    "New Caledonia",
    "New Zealand",
    "Nicaragua",
    "Niger",
    "Nigeria",
    "Niue",
    "Norfolk Island",
    "Northern Mariana Islands",
    "Norway",
    "Oman",
    "Pakistan",
    "Palau",
    "Palestinian Territory, Occupied",
    "Panama",
    "Papua New Guinea",
    "Paraguay",
    "Peru",
    "Philippines",
    "Pitcairn",
    "Poland",
    "Portugal",
    "Puerto Rico",
    "Qatar",
    "Reunion",
    "Romania",
    "Russian Federation",
    "Rwanda",
    "Saint Helena",
    "Saint Kitts and Nevis",
    "Saint Lucia",
    "Saint Pierre and Miquelon",
    "Saint Vincent and the Grenadines",
    "Samoa",
    "San Marino",
    "Sao Tome and Principe",
    "Saudi Arabia",
    "Senegal",
    "Serbia and Montenegro",
    "Seychelles",
    "Sierra Leone",
    "Singapore",
    "Slovakia",
    "Slovenia",
    "Solomon Islands",
    "Somalia",
    "South Africa",
    "South Georgia and the South Sandwich Islands",
    "Spain",
    "Sri Lanka",
    "Sudan",
    "Suriname",
    "Svalbard and Jan Mayen",
    "Swaziland",
    "Sweden",
    "Switzerland",
    "Syrian Arab Republic",
    "Taiwan, Province of China",
    "Tajikistan",
    "Tanzania, United Republic of",
    "Thailand",
    "Timor-Leste",
    "Togo",
    "Tokelau",
    "Tonga",
    "Trinidad and Tobago",
    "Tunisia",
    "Turkey",
    "Turkmenistan",
    "Turks and Caicos Islands",
    "Tuvalu",
    "Uganda",
    "Ukraine",
    "United Arab Emirates",
    "United Kingdom",
    "United States",
    "United States Minor Outlying Islands",
    "Uruguay",
    "Uzbekistan",
    "Vanuatu",
    "Venezuela",
    "Viet Nam",
    "Virgin Islands, British",
    "Virgin Islands, US",
    "Wallis and Futuna",
    "Western Sahara",
    "Yemen",
    "Zambia",
    "Zimbabwe",
    };
        #endregion
        public static IEnumerable<string> CountriesList(this HtmlHelper helper)
        {
            return CountryNames;
        }

        public static IEnumerable<SelectListItem> CountriesListWithCode(this HtmlHelper helper)
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                #region All Countries with Short Codes
                new SelectListItem{ Value ="AF", Text = "Afghanistan"},
                new SelectListItem{ Value ="AX", Text = "Åland Islands"},
                new SelectListItem{ Value ="AL", Text = "Albania"},
                new SelectListItem{ Value ="DZ", Text = "Algeria"},
                new SelectListItem{ Value ="AS", Text = "American Samoa"},
                new SelectListItem{ Value ="AD", Text = "Andorra"},
                new SelectListItem{ Value ="AO", Text = "Angola"},
                new SelectListItem{ Value ="AI", Text = "Anguilla"},
                new SelectListItem{ Value ="AQ", Text = "Antarctica"},
                new SelectListItem{ Value ="AG", Text = "Antigua and Barbuda"},
                new SelectListItem{ Value ="AR", Text = "Argentina"},
                new SelectListItem{ Value ="AM", Text = "Armenia"},
                new SelectListItem{ Value ="AW", Text = "Aruba"},
                new SelectListItem{ Value ="AU", Text = "Australia"},
                new SelectListItem{ Value ="AT", Text = "Austria"},
                new SelectListItem{ Value ="AZ", Text = "Azerbaijan"},
                new SelectListItem{ Value ="BS", Text = "Bahamas"},
                new SelectListItem{ Value ="BH", Text = "Bahrain"},
                new SelectListItem{ Value ="BD", Text = "Bangladesh"},
                new SelectListItem{ Value ="BB", Text = "Barbados"},
                new SelectListItem{ Value ="BY", Text = "Belarus"},
                new SelectListItem{ Value ="BE", Text = "Belgium"},
                new SelectListItem{ Value ="BZ", Text = "Belize"},
                new SelectListItem{ Value ="BJ", Text = "Benin"},
                new SelectListItem{ Value ="BM", Text = "Bermuda"},
                new SelectListItem{ Value ="BT", Text = "Bhutan"},
                new SelectListItem{ Value ="BO", Text = "Bolivia"},
                new SelectListItem{ Value ="BA", Text = "Bosnia and Herzegovina"},
                new SelectListItem{ Value ="BW", Text = "Botswana"},
                new SelectListItem{ Value ="BV", Text = "Bouvet Island"},
                new SelectListItem{ Value ="BR", Text = "Brazil"},
                new SelectListItem{ Value ="IO", Text = "British Indian Ocean Territory"},
                new SelectListItem{ Value ="BN", Text = "Brunei Darussalam"},
                new SelectListItem{ Value ="BG", Text = "Bulgaria"},
                new SelectListItem{ Value ="BF", Text = "Burkina Faso"},
                new SelectListItem{ Value ="BI", Text = "Burundi"},
                new SelectListItem{ Value ="KH", Text = "Cambodia"},
                new SelectListItem{ Value ="CM", Text = "Cameroon"},
                new SelectListItem{ Value ="CA", Text = "Canada"},
                new SelectListItem{ Value ="CV", Text = "Cape Verde"},
                new SelectListItem{ Value ="KY", Text = "Cayman Islands"},
                new SelectListItem{ Value ="CF", Text = "Central African Republic"},
                new SelectListItem{ Value ="TD", Text = "Chad"},
                new SelectListItem{ Value ="CL", Text = "Chile"},
                new SelectListItem{ Value ="CN", Text = "China"},
                new SelectListItem{ Value ="CX", Text = "Christmas Island"},
                new SelectListItem{ Value ="CC", Text = "Cocos (Keeling) Islands"},
                new SelectListItem{ Value ="CO", Text = "Colombia"},
                new SelectListItem{ Value ="KM", Text = "Comoros"},
                new SelectListItem{ Value ="CG", Text = "Congo"},
                new SelectListItem{ Value ="CD", Text = "Congo, The Democratic Republic of The"},
                new SelectListItem{ Value ="CK", Text = "Cook Islands"},
                new SelectListItem{ Value ="CR", Text = "Costa Rica"},
                new SelectListItem{ Value ="CI", Text = "Cote D'ivoire"},
                new SelectListItem{ Value ="HR", Text = "Croatia"},
                new SelectListItem{ Value ="CU", Text = "Cuba"},
                new SelectListItem{ Value ="CY", Text = "Cyprus"},
                new SelectListItem{ Value ="CZ", Text = "Czech Republic"},
                new SelectListItem{ Value ="DK", Text = "Denmark"},
                new SelectListItem{ Value ="DJ", Text = "Djibouti"},
                new SelectListItem{ Value ="DM", Text = "Dominica"},
                new SelectListItem{ Value ="DO", Text = "Dominican Republic"},
                new SelectListItem{ Value ="EC", Text = "Ecuador"},
                new SelectListItem{ Value ="EG", Text = "Egypt"},
                new SelectListItem{ Value ="SV", Text = "El Salvador"},
                new SelectListItem{ Value ="GQ", Text = "Equatorial Guinea"},
                new SelectListItem{ Value ="ER", Text = "Eritrea"},
                new SelectListItem{ Value ="EE", Text = "Estonia"},
                new SelectListItem{ Value ="ET", Text = "Ethiopia"},
                new SelectListItem{ Value ="FK", Text = "Falkland Islands (Malvinas)"},
                new SelectListItem{ Value ="FO", Text = "Faroe Islands"},
                new SelectListItem{ Value ="FJ", Text = "Fiji"},
                new SelectListItem{ Value ="FI", Text = "Finland"},
                new SelectListItem{ Value ="FR", Text = "France"},
                new SelectListItem{ Value ="GF", Text = "French Guiana"},
                new SelectListItem{ Value ="PF", Text = "French Polynesia"},
                new SelectListItem{ Value ="TF", Text = "French Southern Territories"},
                new SelectListItem{ Value ="GA", Text = "Gabon"},
                new SelectListItem{ Value ="GM", Text = "Gambia"},
                new SelectListItem{ Value ="GE", Text = "Georgia"},
                new SelectListItem{ Value ="DE", Text = "Germany"},
                new SelectListItem{ Value ="GH", Text = "Ghana"},
                new SelectListItem{ Value ="GI", Text = "Gibraltar"},
                new SelectListItem{ Value ="GR", Text = "Greece"},
                new SelectListItem{ Value ="GL", Text = "Greenland"},
                new SelectListItem{ Value ="GD", Text = "Grenada"},
                new SelectListItem{ Value ="GP", Text = "Guadeloupe"},
                new SelectListItem{ Value ="GU", Text = "Guam"},
                new SelectListItem{ Value ="GT", Text = "Guatemala"},
                new SelectListItem{ Value ="GG", Text = "Guernsey"},
                new SelectListItem{ Value ="GN", Text = "Guinea"},
                new SelectListItem{ Value ="GW", Text = "Guinea-bissau"},
                new SelectListItem{ Value ="GY", Text = "Guyana"},
                new SelectListItem{ Value ="HT", Text = "Haiti"},
                new SelectListItem{ Value ="HM", Text = "Heard Island and Mcdonald Islands"},
                new SelectListItem{ Value ="VA", Text = "Holy See (Vatican City State)"},
                new SelectListItem{ Value ="HN", Text = "Honduras"},
                new SelectListItem{ Value ="HK", Text = "Hong Kong"},
                new SelectListItem{ Value ="HU", Text = "Hungary"},
                new SelectListItem{ Value ="IS", Text = "Iceland"},
                new SelectListItem{ Value ="IN", Text = "India"},
                new SelectListItem{ Value ="ID", Text = "Indonesia"},
                new SelectListItem{ Value ="IR", Text = "Iran, Islamic Republic of"},
                new SelectListItem{ Value ="IQ", Text = "Iraq"},
                new SelectListItem{ Value ="IE", Text = "Ireland"},
                new SelectListItem{ Value ="IM", Text = "Isle of Man"},
                new SelectListItem{ Value ="IL", Text = "Israel"},
                new SelectListItem{ Value ="IT", Text = "Italy"},
                new SelectListItem{ Value ="JM", Text = "Jamaica"},
                new SelectListItem{ Value ="JP", Text = "Japan"},
                new SelectListItem{ Value ="JE", Text = "Jersey"},
                new SelectListItem{ Value ="JO", Text = "Jordan"},
                new SelectListItem{ Value ="KZ", Text = "Kazakhstan"},
                new SelectListItem{ Value ="KE", Text = "Kenya"},
                new SelectListItem{ Value ="KI", Text = "Kiribati"},
                new SelectListItem{ Value ="KP", Text = "Korea, Democratic People's Republic of"},
                new SelectListItem{ Value ="KR", Text = "Korea, Republic of"},
                new SelectListItem{ Value ="KW", Text = "Kuwait"},
                new SelectListItem{ Value ="KG", Text = "Kyrgyzstan"},
                new SelectListItem{ Value ="LA", Text = "Lao People's Democratic Republic"},
                new SelectListItem{ Value ="LV", Text = "Latvia"},
                new SelectListItem{ Value ="LB", Text = "Lebanon"},
                new SelectListItem{ Value ="LS", Text = "Lesotho"},
                new SelectListItem{ Value ="LR", Text = "Liberia"},
                new SelectListItem{ Value ="LY", Text = "Libyan Arab Jamahiriya"},
                new SelectListItem{ Value ="LI", Text = "Liechtenstein"},
                new SelectListItem{ Value ="LT", Text = "Lithuania"},
                new SelectListItem{ Value ="LU", Text = "Luxembourg"},
                new SelectListItem{ Value ="MO", Text = "Macao"},
                new SelectListItem{ Value ="MK", Text = "Macedonia, The Former Yugoslav Republic of"},
                new SelectListItem{ Value ="MG", Text = "Madagascar"},
                new SelectListItem{ Value ="MW", Text = "Malawi"},
                new SelectListItem{ Value ="MY", Text = "Malaysia"},
                new SelectListItem{ Value ="MV", Text = "Maldives"},
                new SelectListItem{ Value ="ML", Text = "Mali"},
                new SelectListItem{ Value ="MT", Text = "Malta"},
                new SelectListItem{ Value ="MH", Text = "Marshall Islands"},
                new SelectListItem{ Value ="MQ", Text = "Martinique"},
                new SelectListItem{ Value ="MR", Text = "Mauritania"},
                new SelectListItem{ Value ="MU", Text = "Mauritius"},
                new SelectListItem{ Value ="YT", Text = "Mayotte"},
                new SelectListItem{ Value ="MX", Text = "Mexico"},
                new SelectListItem{ Value ="FM", Text = "Micronesia, Federated States of"},
                new SelectListItem{ Value ="MD", Text = "Moldova, Republic of"},
                new SelectListItem{ Value ="MC", Text = "Monaco"},
                new SelectListItem{ Value ="MN", Text = "Mongolia"},
                new SelectListItem{ Value ="ME", Text = "Montenegro"},
                new SelectListItem{ Value ="MS", Text = "Montserrat"},
                new SelectListItem{ Value ="MA", Text = "Morocco"},
                new SelectListItem{ Value ="MZ", Text = "Mozambique"},
                new SelectListItem{ Value ="MM", Text = "Myanmar"},
                new SelectListItem{ Value ="NA", Text = "Namibia"},
                new SelectListItem{ Value ="NR", Text = "Nauru"},
                new SelectListItem{ Value ="NP", Text = "Nepal"},
                new SelectListItem{ Value ="NL", Text = "Netherlands"},
                new SelectListItem{ Value ="AN", Text = "Netherlands Antilles"},
                new SelectListItem{ Value ="NC", Text = "New Caledonia"},
                new SelectListItem{ Value ="NZ", Text = "New Zealand"},
                new SelectListItem{ Value ="NI", Text = "Nicaragua"},
                new SelectListItem{ Value ="NE", Text = "Niger"},
                new SelectListItem{ Value ="NG", Text = "Nigeria"},
                new SelectListItem{ Value ="NU", Text = "Niue"},
                new SelectListItem{ Value ="NF", Text = "Norfolk Island"},
                new SelectListItem{ Value ="MP", Text = "Northern Mariana Islands"},
                new SelectListItem{ Value ="NO", Text = "Norway"},
                new SelectListItem{ Value ="OM", Text = "Oman"},
                new SelectListItem{ Value ="PK", Text = "Pakistan"},
                new SelectListItem{ Value ="PW", Text = "Palau"},
                new SelectListItem{ Value ="PS", Text = "Palestinian Territory, Occupied"},
                new SelectListItem{ Value ="PA", Text = "Panama"},
                new SelectListItem{ Value ="PG", Text = "Papua New Guinea"},
                new SelectListItem{ Value ="PY", Text = "Paraguay"},
                new SelectListItem{ Value ="PE", Text = "Peru"},
                new SelectListItem{ Value ="PH", Text = "Philippines"},
                new SelectListItem{ Value ="PN", Text = "Pitcairn"},
                new SelectListItem{ Value ="PL", Text = "Poland"},
                new SelectListItem{ Value ="PT", Text = "Portugal"},
                new SelectListItem{ Value ="PR", Text = "Puerto Rico"},
                new SelectListItem{ Value ="QA", Text = "Qatar"},
                new SelectListItem{ Value ="RE", Text = "Reunion"},
                new SelectListItem{ Value ="RO", Text = "Romania"},
                new SelectListItem{ Value ="RU", Text = "Russian Federation"},
                new SelectListItem{ Value ="RW", Text = "Rwanda"},
                new SelectListItem{ Value ="SH", Text = "Saint Helena"},
                new SelectListItem{ Value ="KN", Text = "Saint Kitts and Nevis"},
                new SelectListItem{ Value ="LC", Text = "Saint Lucia"},
                new SelectListItem{ Value ="PM", Text = "Saint Pierre and Miquelon"},
                new SelectListItem{ Value ="VC", Text = "Saint Vincent and The Grenadines"},
                new SelectListItem{ Value ="WS", Text = "Samoa"},
                new SelectListItem{ Value ="SM", Text = "San Marino"},
                new SelectListItem{ Value ="ST", Text = "Sao Tome and Principe"},
                new SelectListItem{ Value ="SA", Text = "Saudi Arabia"},
                new SelectListItem{ Value ="SN", Text = "Senegal"},
                new SelectListItem{ Value ="RS", Text = "Serbia"},
                new SelectListItem{ Value ="SC", Text = "Seychelles"},
                new SelectListItem{ Value ="SL", Text = "Sierra Leone"},
                new SelectListItem{ Value ="SG", Text = "Singapore"},
                new SelectListItem{ Value ="SK", Text = "Slovakia"},
                new SelectListItem{ Value ="SI", Text = "Slovenia"},
                new SelectListItem{ Value ="SB", Text = "Solomon Islands"},
                new SelectListItem{ Value ="SO", Text = "Somalia"},
                new SelectListItem{ Value ="ZA", Text = "South Africa"},
                new SelectListItem{ Value ="GS", Text = "South Georgia and The South Sandwich Islands"},
                new SelectListItem{ Value ="ES", Text = "Spain"},
                new SelectListItem{ Value ="LK", Text = "Sri Lanka"},
                new SelectListItem{ Value ="SD", Text = "Sudan"},
                new SelectListItem{ Value ="SR", Text = "Suriname"},
                new SelectListItem{ Value ="SJ", Text = "Svalbard and Jan Mayen"},
                new SelectListItem{ Value ="SZ", Text = "Swaziland"},
                new SelectListItem{ Value ="SE", Text = "Sweden"},
                new SelectListItem{ Value ="CH", Text = "Switzerland"},
                new SelectListItem{ Value ="SY", Text = "Syrian Arab Republic"},
                new SelectListItem{ Value ="TW", Text = "Taiwan, Province of China"},
                new SelectListItem{ Value ="TJ", Text = "Tajikistan"},
                new SelectListItem{ Value ="TZ", Text = "Tanzania, United Republic of Tanzania"},
                new SelectListItem{ Value ="TH", Text = "Thailand"},
                new SelectListItem{ Value ="TL", Text = "Timor-leste"},
                new SelectListItem{ Value ="TG", Text = "Togo"},
                new SelectListItem{ Value ="TK", Text = "Tokelau"},
                new SelectListItem{ Value ="TO", Text = "Tonga"},
                new SelectListItem{ Value ="TT", Text = "Trinidad and Tobago"},
                new SelectListItem{ Value ="TN", Text = "Tunisia"},
                new SelectListItem{ Value ="TR", Text = "Turkey"},
                new SelectListItem{ Value ="TM", Text = "Turkmenistan"},
                new SelectListItem{ Value ="TC", Text = "Turks and Caicos Islands"},
                new SelectListItem{ Value ="TV", Text = "Tuvalu"},
                new SelectListItem{ Value ="UG", Text = "Uganda"},
                new SelectListItem{ Value ="UA", Text = "Ukraine"},
                new SelectListItem{ Value ="AE", Text = "United Arab Emirates"},
                new SelectListItem{ Value ="GB", Text = "United Kingdom"},
                new SelectListItem{ Value ="US", Text = "United States"},
                new SelectListItem{ Value ="UM", Text = "United States Minor Outlying Islands"},
                new SelectListItem{ Value ="UY", Text = "Uruguay"},
                new SelectListItem{ Value ="UZ", Text = "Uzbekistan"},
                new SelectListItem{ Value ="VU", Text = "Vanuatu"},
                new SelectListItem{ Value ="VE", Text = "Venezuela"},
                new SelectListItem{ Value ="VN", Text = "Viet Nam"},
                new SelectListItem{ Value ="VG", Text = "Virgin Islands, British"},
                new SelectListItem{ Value ="VI", Text = "Virgin Islands, U.S."},
                new SelectListItem{ Value ="WF", Text = "Wallis and Futuna"},
                new SelectListItem{ Value ="EH", Text = "Western Sahara"},
                new SelectListItem{ Value ="YE", Text = "Yemen"},
                new SelectListItem{ Value ="ZM", Text = "Zambia"},
                new SelectListItem{ Value ="ZW", Text = "Zimbabwe"}
                #endregion
            };

            return items;
        }

        public static IEnumerable<SelectListItem> CurrenciesList(this HtmlHelper helper)
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                #region All Currencies
                new SelectListItem{ Value = "USD-$", Text = "US Dollar"},
                new SelectListItem{ Value = "ALL-Lek", Text = "Albanian Lek"},
                new SelectListItem{ Value = "DZD-DZD", Text = "Algerian Dinar"},
                new SelectListItem{ Value = "AOA-Kz", Text = "Angolan Kwanza"},
                new SelectListItem{ Value = "ARS-$", Text = "Argentine Peso"},
                new SelectListItem{ Value = "AMD-դր.", Text = "Armenian Dram"},
                new SelectListItem{ Value = "AWG-Afl", Text = "Aruban Florin"},
                new SelectListItem{ Value = "AUD-$", Text = "Australian Dollar"},
                new SelectListItem{ Value = "AZN-man.", Text = "Azerbaijani Manat"},
                new SelectListItem{ Value = "BSD-$", Text = "Bahamian Dollar"},
                new SelectListItem{ Value = "BHD-د.ب.‏", Text = "Bahraini Dinar"},
                new SelectListItem{ Value = "BDT-৳", Text = "Bangladeshi Taka"},
                new SelectListItem{ Value = "BBD-$", Text = "Barbadian Dollar"},
                new SelectListItem{ Value = "BYR-р.", Text = "Belarusian Ruble"},
                new SelectListItem{ Value = "BEF-fr.", Text = "Belgian Franc"},
                new SelectListItem{ Value = "BZD-$", Text = "Belize Dollar"},
                new SelectListItem{ Value = "BMD-$", Text = "Bermudan Dollar"},
                new SelectListItem{ Value = "BTN-Nu.", Text = "Bhutanese Ngultrum"},
                new SelectListItem{ Value = "BTC-฿", Text = "Bitcoin"},
                new SelectListItem{ Value = "BOB-$b", Text = "Bolivian Boliviano"},
                new SelectListItem{ Value = "BAM-KM", Text = "Bosnia-Herzegovina Convertible Mark"},
                new SelectListItem{ Value = "BWP-P", Text = "Botswanan Pula"},
                new SelectListItem{ Value = "BRL-R$", Text = "Brazilian Real"},
                new SelectListItem{ Value = "GBP-£", Text = "British Pound"},
                new SelectListItem{ Value = "BND-$", Text = "Brunei Dollar"},
                new SelectListItem{ Value = "BGN-лв.", Text = "Bulgarian Lev"},
                new SelectListItem{ Value = "BIF-FBu", Text = "Burundian Franc"},
                new SelectListItem{ Value = "KHR-៛", Text = "Cambodian Riel"},
                new SelectListItem{ Value = "CAD-$", Text = "Canadian Dollar"},
                new SelectListItem{ Value = "CVE-$", Text = "Cape Verdean Escudo"},
                new SelectListItem{ Value = "KYD-$", Text = "Cayman Islands Dollar"},
                new SelectListItem{ Value = "XAF-FCFA", Text = "Central African CFA Franc"},
                new SelectListItem{ Value = "XPF-F", Text = "CFP Franc"},
                new SelectListItem{ Value = "CLP-$", Text = "Chilean Peso"},
                new SelectListItem{ Value = "CNY-¥", Text = "Chinese Yuan"},
                new SelectListItem{ Value = "COP-$", Text = "Colombian Peso"},
                new SelectListItem{ Value = "KMF-CF", Text = "Comorian Franc"},
                new SelectListItem{ Value = "CDF-FC", Text = "Congolese Franc"},
                new SelectListItem{ Value = "CRC-₡", Text = "Costa Rican Colón"},
                new SelectListItem{ Value = "HRK-kn", Text = "Croatian Kuna"},
                new SelectListItem{ Value = "CUC-₱", Text = "Cuban Convertible Peso"},
                new SelectListItem{ Value = "CZK-Kč", Text = "Czech Republic Koruna"},
                new SelectListItem{ Value = "DKK-kr.", Text = "Danish Krone"},
                new SelectListItem{ Value = "DJF-Fdj", Text = "Djiboutian Franc"},
                new SelectListItem{ Value = "DOP-RD$", Text = "Dominican Peso"},
                new SelectListItem{ Value = "XCD-$", Text = "East Caribbean Dollar"},
                new SelectListItem{ Value = "EGP-ج.م.", Text = "Egyptian Pound"},
                new SelectListItem{ Value = "ERN-ናቕፋ", Text = "Eritrean Nakfa"},
                new SelectListItem{ Value = "EEK-kr", Text = "Estonian Kroon"},
                new SelectListItem{ Value = "ETB-ETB", Text = "Ethiopian Birr"},
                new SelectListItem{ Value = "EUR-€", Text = "Euro"},
                new SelectListItem{ Value = "FKP-£", Text = "Falkland Islands Pound"},
                new SelectListItem{ Value = "FJD-FJ$", Text = "Fijian Dollar"},
                new SelectListItem{ Value = "GMD-D", Text = "Gambian Dalasi"},
                new SelectListItem{ Value = "GEL-Lari", Text = "Georgian Lari"},
                new SelectListItem{ Value = "DEM-M", Text = "German Mark"},
                new SelectListItem{ Value = "GHS-GH₵", Text = "Ghanaian Cedi"},
                new SelectListItem{ Value = "GIP-£", Text = "Gibraltar Pound"},
                new SelectListItem{ Value = "GRD-₯", Text = "Greek Drachma"},
                new SelectListItem{ Value = "GTQ-Q", Text = "Guatemalan Quetzal"},
                new SelectListItem{ Value = "GNF-FG", Text = "Guinean Franc"},
                new SelectListItem{ Value = "GYD-$", Text = "Guyanaese Dollar"},
                new SelectListItem{ Value = "HTG-G", Text = "Haitian Gourde"},
                new SelectListItem{ Value = "HNL-L.", Text = "Honduran Lempira"},
                new SelectListItem{ Value = "HKD-HK$", Text = "Hong Kong Dollar"},
                new SelectListItem{ Value = "HUF-Ft", Text = "Hungarian Forint"},
                new SelectListItem{ Value = "ISK-kr.", Text = "Icelandic Króna"},
                new SelectListItem{ Value = "INR-रु", Text = "Indian Rupee"},
                new SelectListItem{ Value = "IDR-Rp", Text = "Indonesian Rupiah"},
                new SelectListItem{ Value = "IRR-ريال", Text = "Iranian Rial"},
                new SelectListItem{ Value = "IQD-د.ع.", Text = "Iraqi Dinar"},
                new SelectListItem{ Value = "ILS-₪", Text = "Israeli New Sheqel"},
                new SelectListItem{ Value = "ITL-₤", Text = "Italian Lira"},
                new SelectListItem{ Value = "JMD-J$", Text = "Jamaican Dollar"},
                new SelectListItem{ Value = "JPY-¥", Text = "Japanese Yen"},
                new SelectListItem{ Value = "JOD-د.ا.‏", Text = "Jordanian Dinar"},
                new SelectListItem{ Value = "KZT-Т", Text = "Kazakhstani Tenge"},
                new SelectListItem{ Value = "KES-S", Text = "Kenyan Shilling"},
                new SelectListItem{ Value = "KWD-د.ك.‏", Text = "Kuwaiti Dinar"},
                new SelectListItem{ Value = "KGS-сом", Text = "Kyrgystani Som"},
                new SelectListItem{ Value = "LAK-₭", Text = "Laotian Kip"},
                new SelectListItem{ Value = "LVL-Ls", Text = "Latvian Lats"},
                new SelectListItem{ Value = "LBP-ل.ل.", Text = "Lebanese Pound"},
                new SelectListItem{ Value = "LSL-L", Text = "Lesotho Loti"},
                new SelectListItem{ Value = "LRD-L$", Text = "Liberian Dollar"},
                new SelectListItem{ Value = "LYD-د.ل.", Text = "Libyan Dinar"},
                new SelectListItem{ Value = "LTL-Lt", Text = "Lithuanian Litas"},
                new SelectListItem{ Value = "MOP-MOP", Text = "Macanese Pataca"},
                new SelectListItem{ Value = "MKD-ден.", Text = "Macedonian Denar"},
                new SelectListItem{ Value = "MGA-Ar", Text = "Malagasy Ariary"},
                new SelectListItem{ Value = "MWK-MK", Text = "Malawian Kwacha"},
                new SelectListItem{ Value = "MYR-RM", Text = "Malaysian Ringgit"},
                new SelectListItem{ Value = "MVR-ރ.", Text = "Maldivian Rufiyaa"},
                new SelectListItem{ Value = "MRO-UM", Text = "Mauritanian Ouguiya"},
                new SelectListItem{ Value = "MUR-₨", Text = "Mauritian Rupee"},
                new SelectListItem{ Value = "MXN-$", Text = "Mexican Peso"},
                new SelectListItem{ Value = "MDL-lei", Text = "Moldovan Leu"},
                new SelectListItem{ Value = "MNT-₮", Text = "Mongolian Tugrik"},
                new SelectListItem{ Value = "MAD-د.م.", Text = "Moroccan Dirham"},
                new SelectListItem{ Value = "MMK-K", Text = "Myanmar Kyat"},
                new SelectListItem{ Value = "NAD-N$", Text = "Namibian Dollar"},
                new SelectListItem{ Value = "NPR-रु", Text = "Nepalese Rupee"},
                new SelectListItem{ Value = "ANG-NAƒ", Text = "Netherlands Antillean Guilder"},
                new SelectListItem{ Value = "TWD-NT$", Text = "New Taiwan Dollar"},
                new SelectListItem{ Value = "NZD-$", Text = "New Zealand Dollar"},
                new SelectListItem{ Value = "NIO-N", Text = "Nicaraguan Córdoba"},
                new SelectListItem{ Value = "NGN-₦", Text = "Nigerian Naira"},
                new SelectListItem{ Value = "KPW-₩", Text = "North Korean Won"},
                new SelectListItem{ Value = "NOK-kr", Text = "Norwegian Krone"},
                new SelectListItem{ Value = "OMR-ر.ع.", Text = "Omani Rial"},
                new SelectListItem{ Value = "PAB-B/.", Text = "Panamanian Balboa"},
                new SelectListItem{ Value = "PGK-K", Text = "Papua New Guinean Kina"},
                new SelectListItem{ Value = "PYG-Gs", Text = "Paraguayan Guarani"},
                new SelectListItem{ Value = "PEN-S/.", Text = "Peruvian Nuevo Sol"},
                new SelectListItem{ Value = "PHP-PhP", Text = "Philippine Peso"},
                new SelectListItem{ Value = "PLN-zł", Text = "Polish Zloty"},
                new SelectListItem{ Value = "QAR-ر.ق.", Text = "Qatari Rial"},
                new SelectListItem{ Value = "RON-lei", Text = "Romanian Leu"},
                new SelectListItem{ Value = "RUB-р.", Text = "Russian Ruble"},
                new SelectListItem{ Value = "RWF-RWF", Text = "Rwandan Franc"},
                new SelectListItem{ Value = "SVC-₡", Text = "Salvadoran Colón"},
                new SelectListItem{ Value = "WST-WS$", Text = "Samoan Tala"},
                new SelectListItem{ Value = "SAR-ر.س.", Text = "Saudi Riyal"},
                new SelectListItem{ Value = "RSD-Din.", Text = "Serbian Dinar"},
                new SelectListItem{ Value = "SCR-SR", Text = "Seychellois Rupee"},
                new SelectListItem{ Value = "SLL-Le", Text = "Sierra Leonean Leone"},
                new SelectListItem{ Value = "SGD-$", Text = "Singapore Dollar"},
                new SelectListItem{ Value = "SKK-Sk", Text = "Slovak Koruna"},
                new SelectListItem{ Value = "SBD-$", Text = "Solomon Islands Dollar"},
                new SelectListItem{ Value = "SOS-Sh.So.", Text = "Somali Shilling"},
                new SelectListItem{ Value = "ZAR-R", Text = "South African Rand"},
                new SelectListItem{ Value = "KRW-₩", Text = "South Korean Won"},
                new SelectListItem{ Value = "LKR-රු.", Text = "Sri Lankan Rupee"},
                new SelectListItem{ Value = "SHP-£", Text = "St. Helena Pound"},
                new SelectListItem{ Value = "SDG-ج.س.‏", Text = "Sudanese Pound"},
                new SelectListItem{ Value = "SRD-$", Text = "Surinamese Dollar"},
                new SelectListItem{ Value = "SZL-L", Text = "Swazi Lilangeni"},
                new SelectListItem{ Value = "SEK-kr", Text = "Swedish Krona"},
                new SelectListItem{ Value = "CHF-fr.", Text = "Swiss Franc"},
                new SelectListItem{ Value = "SYP-ل.س.‏", Text = "Syrian Pound"},
                new SelectListItem{ Value = "STD-Db", Text = "São Tomé & Príncipe Dobra"},
                new SelectListItem{ Value = "TJS-т.р.", Text = "Tajikistani Somoni"},
                new SelectListItem{ Value = "TZS-x/y", Text = "Tanzanian Shilling"},
                new SelectListItem{ Value = "THB-฿", Text = "Thai Baht"},
                new SelectListItem{ Value = "TOP-T$", Text = "Tongan Paʻanga"},
                new SelectListItem{ Value = "TTD-TT$", Text = "Trinidad & Tobago Dollar"},
                new SelectListItem{ Value = "TND-د.ت.", Text = "Tunisian Dinar"},
                new SelectListItem{ Value = "TRY-TL", Text = "Turkish Lira"},
                new SelectListItem{ Value = "TMT-m.", Text = "Turkmenistani Manat"},
                new SelectListItem{ Value = "UGX-USh", Text = "Ugandan Shilling"},
                new SelectListItem{ Value = "UAH-₴", Text = "Ukrainian Hryvnia"},
                new SelectListItem{ Value = "AED-د.إ.‏", Text = "United Arab Emirates Dirham"},
                new SelectListItem{ Value = "UYU-$U", Text = "Uruguayan Peso"},
                new SelectListItem{ Value = "UZS-so'm", Text = "Uzbekistani Som"},
                new SelectListItem{ Value = "VUV-VT", Text = "Vanuatu Vatu"},
                new SelectListItem{ Value = "VEF-Bs. F.", Text = "Venezuelan Bolívar"},
                new SelectListItem{ Value = "VND-₫", Text = "Vietnamese Dong"},
                new SelectListItem{ Value = "XOF-XOF", Text = "West African CFA Franc"},
                new SelectListItem{ Value = "YER-ر.ي.", Text = "Yemeni Rial"},
                new SelectListItem{ Value = "PKR-Rs", Text = "Pakistani Rupee"}
                #endregion
            };

            return items;
        }

        public static IEnumerable<SelectListItem> FontAwesomeIcons(this HtmlHelper helper)
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                #region All Font Awesome Icons (more can be added)
                new SelectListItem{ Value="fa fa-adjust", Text = "fa fa-adjust" },
                new SelectListItem{ Value="fa fa-asterisk", Text = "fa fa-asterisk" },
                new SelectListItem{ Value="fa fa-ban", Text = "fa fa-ban" },
                new SelectListItem{ Value="fa fa-bar-chart", Text = "fa fa-bar-chart" },
                new SelectListItem{ Value="fa fa-barcode", Text = "fa fa-barcode" },
                new SelectListItem{ Value="fa fa-flask", Text = "fa fa-flask" },
                new SelectListItem{ Value="fa fa-beer", Text = "fa fa-beer" },
                new SelectListItem{ Value="fa fa-bell", Text = "fa fa-bell" },
                new SelectListItem{ Value="fa fa-bell-slash", Text = "fa fa-bell-slash" },
                new SelectListItem{ Value="fa fa-bell-o", Text = "fa fa-bell-o" },
                new SelectListItem{ Value="fa fa-bell-slash-o", Text = "fa fa-bell-slash-o" },
                new SelectListItem{ Value="fa fa-bolt", Text = "fa fa-bolt" },
                new SelectListItem{ Value="fa fa-book", Text = "fa fa-book" },
                new SelectListItem{ Value="fa fa-bookmark", Text = "fa fa-bookmark" },
                new SelectListItem{ Value="fa fa-bookmark-o", Text = "fa fa-bookmark-o" },
                new SelectListItem{ Value="fa fa-briefcase", Text = "fa fa-briefcase" },
                new SelectListItem{ Value="fa fa-bullhorn", Text = "fa fa-bullhorn" },
                new SelectListItem{ Value="fa fa-calendar", Text = "fa fa-calendar" },
                new SelectListItem{ Value="fa fa-camera", Text = "fa fa-camera" },
                new SelectListItem{ Value="fa fa-camera-retro", Text = "fa fa-camera-retro" },
                new SelectListItem{ Value="fa fa-certificate", Text = "fa fa-certificate" },
                new SelectListItem{ Value="fa fa-check", Text = "fa fa-check" },
                new SelectListItem{ Value="fa fa-check-circle", Text = "fa fa-check-circle" },
                new SelectListItem{ Value="fa fa-check-circle-o", Text = "fa fa-check-circle-o" },
                new SelectListItem{ Value="fa fa-check-square", Text = "fa fa-check-square" },
                new SelectListItem{ Value="fa fa-check-square-o", Text = "fa fa-check-square-o" },
                new SelectListItem{ Value="fa fa-circle", Text = "fa fa-circle" },
                new SelectListItem{ Value="fa fa-circle-o", Text = "fa fa-circle-o" },
                new SelectListItem{ Value="fa fa-circle-thin", Text = "fa fa-circle-thin" },
                new SelectListItem{ Value="fa fa-circle-o-notch", Text = "fa fa-circle-o-notch" },
                new SelectListItem{ Value="fa fa-cloud", Text = "fa fa-cloud" },
                new SelectListItem{ Value="fa fa-cloud-download", Text = "fa fa-cloud-download" },
                new SelectListItem{ Value="fa fa-cloud-upload", Text = "fa fa-cloud-upload" },
                new SelectListItem{ Value="fa fa-coffee", Text = "fa fa-coffee" },
                new SelectListItem{ Value="fa fa-cog", Text = "fa fa-cog" },
                new SelectListItem{ Value="fa fa-cogs", Text = "fa fa-cogs" },
                new SelectListItem{ Value="fa fa-comment", Text = "fa fa-comment" },
                new SelectListItem{ Value="fa fa-comment-o", Text = "fa fa-comment-o" },
                new SelectListItem{ Value="fa fa-comments", Text = "fa fa-comments" },
                new SelectListItem{ Value="fa fa-comments-o", Text = "fa fa-comments-o" },
                new SelectListItem{ Value="fa fa-credit-card", Text = "fa fa-credit-card" },
                new SelectListItem{ Value="fa fa-dashboard", Text = "fa fa-dashboard" },
                new SelectListItem{ Value="fa fa-desktop", Text = "fa fa-desktop" },
                new SelectListItem{ Value="fa fa-download", Text = "fa fa-download" },
                new SelectListItem{ Value="fa fa-edit", Text = "fa fa-edit" },
                new SelectListItem{ Value="fa fa-envelope", Text = "fa fa-envelope" },
                new SelectListItem{ Value="fa fa-envelope-o", Text = "fa fa-envelope-o" },
                new SelectListItem{ Value="fa fa-exchange", Text = "fa fa-exchange" },
                new SelectListItem{ Value="fa fa-exclamation", Text = "fa fa-exclamation" },
                new SelectListItem{ Value="fa fa-exclamation-triangle", Text = "fa fa-exclamation-triangle" },
                new SelectListItem{ Value="fa fa-exclamation-circle", Text = "fa fa-exclamation-circle" },
                new SelectListItem{ Value="fa fa-external-link", Text = "fa fa-external-link" },
                new SelectListItem{ Value="fa fa-eye", Text = "fa fa-eye" },
                new SelectListItem{ Value="fa fa-eye-slash", Text = "fa fa-eye-slash" },
                new SelectListItem{ Value="fa fa-fighter-jet", Text = "fa fa-fighter-jet" },
                new SelectListItem{ Value="fa fa-film", Text = "fa fa-film" },
                new SelectListItem{ Value="fa fa-filter", Text = "fa fa-filter" },
                new SelectListItem{ Value="fa fa-fire", Text = "fa fa-fire" },
                new SelectListItem{ Value="fa fa-flag", Text = "fa fa-flag" },
                new SelectListItem{ Value="fa fa-folder", Text = "fa fa-folder" },
                new SelectListItem{ Value="fa fa-folder-open", Text = "fa fa-folder-open" },
                new SelectListItem{ Value="fa fa-folder-o", Text = "fa fa-folder-o" },
                new SelectListItem{ Value="fa fa-folder-open-o", Text = "fa fa-folder-open-o" },
                new SelectListItem{ Value="fa fa-cutlery", Text = "fa fa-cutlery" },
                new SelectListItem{ Value="fa fa-gift", Text = "fa fa-gift" },
                new SelectListItem{ Value="fa fa-glass", Text = "fa fa-glass" },
                new SelectListItem{ Value="fa fa-globe", Text = "fa fa-globe" },
                new SelectListItem{ Value="fa fa-group", Text = "fa fa-group" },
                new SelectListItem{ Value="fa fa-hdd-o", Text = "fa fa-hdd-o" },
                new SelectListItem{ Value="fa fa-headphones", Text = "fa fa-headphones" },
                new SelectListItem{ Value="fa fa-heartbeat", Text = "fa fa-heartbeat" },
                new SelectListItem{ Value="fa fa-heart", Text = "fa fa-heart" },
                new SelectListItem{ Value="fa fa-heart-o", Text = "fa fa-heart-o" },
                new SelectListItem{ Value="fa fa-home", Text = "fa fa-home" },
                new SelectListItem{ Value="fa fa-inbox", Text = "fa fa-inbox" },
                new SelectListItem{ Value="fa fa-info", Text = "fa fa-info" },
                new SelectListItem{ Value="fa fa-info-circle", Text = "fa fa-info-circle" },
                new SelectListItem{ Value="fa fa-key", Text = "fa fa-key" },
                new SelectListItem{ Value="fa fa-leaf", Text = "fa fa-leaf" },
                new SelectListItem{ Value="fa fa-laptop", Text = "fa fa-laptop" },
                new SelectListItem{ Value="fa fa-legal", Text = "fa fa-legal" },
                new SelectListItem{ Value="fa fa-lemon-o", Text = "fa fa-lemon-o" },
                new SelectListItem{ Value="fa fa-lightbulb-o", Text = "fa fa-lightbulb-o" },
                new SelectListItem{ Value="fa fa-lock", Text = "fa fa-lock" },
                new SelectListItem{ Value="fa fa-unlock", Text = "fa fa-unlock" },
                new SelectListItem{ Value="fa fa-magic", Text = "fa fa-magic" },
                new SelectListItem{ Value="fa fa-magnet", Text = "fa fa-magnet" },
                new SelectListItem{ Value="fa fa-map-marker", Text = "fa fa-map-marker" },
                new SelectListItem{ Value="fa fa-minus", Text = "fa fa-minus" },
                new SelectListItem{ Value="fa fa-minus-circle", Text = "fa fa-minus-circle" },
                new SelectListItem{ Value="fa fa-minus-square", Text = "fa fa-minus-square" },
                new SelectListItem{ Value="fa fa-minus-square-o", Text = "fa fa-minus-square-o" },
                new SelectListItem{ Value="fa fa-mobile-phone", Text = "fa fa-mobile-phone" },
                new SelectListItem{ Value="fa fa-money", Text = "fa fa-money" },
                new SelectListItem{ Value="fa fa-music", Text = "fa fa-music" },
                new SelectListItem{ Value="fa fa-power-off", Text = "fa fa-power-off" },
                new SelectListItem{ Value="fa fa-pencil", Text = "fa fa-pencil" },
                new SelectListItem{ Value="fa fa-picture-o", Text = "fa fa-picture-o" },
                new SelectListItem{ Value="fa fa-plane", Text = "fa fa-plane" },
                new SelectListItem{ Value="fa fa-plus", Text = "fa fa-plus" },
                new SelectListItem{ Value="fa fa-plus-circle", Text = "fa fa-plus-circle" },
                new SelectListItem{ Value="fa fa-plus-square", Text = "fa fa-plus-square" },
                new SelectListItem{ Value="fa fa-plus-square-o", Text = "fa fa-plus-square-o" },
                new SelectListItem{ Value="fa fa-print", Text = "fa fa-print" },
                new SelectListItem{ Value="fa fa-qrcode", Text = "fa fa-qrcode" },
                new SelectListItem{ Value="fa fa-question", Text = "fa fa-question" },
                new SelectListItem{ Value="fa fa-question-circle", Text = "fa fa-question-circle" },
                new SelectListItem{ Value="fa fa-question-circle-o", Text = "fa fa-question-circle-o" },
                new SelectListItem{ Value="fa fa-quote-left", Text = "fa fa-quote-left" },
                new SelectListItem{ Value="fa fa-quote-right", Text = "fa fa-quote-right" },
                new SelectListItem{ Value="fa fa-random", Text = "fa fa-random" },
                new SelectListItem{ Value="fa fa-refresh", Text = "fa fa-refresh" },
                new SelectListItem{ Value="fa fa-remove", Text = "fa fa-remove" },
                new SelectListItem{ Value="fa fa-times", Text = "fa fa-times" },
                new SelectListItem{ Value="fa fa-reorder", Text = "fa fa-reorder" },
                new SelectListItem{ Value="fa fa-reply", Text = "fa fa-reply" },
                new SelectListItem{ Value="fa fa-arrows-h", Text = "fa fa-arrows-h" },
                new SelectListItem{ Value="fa fa-arrows-v", Text = "fa fa-arrows-v" },
                new SelectListItem{ Value="fa fa-retweet", Text = "fa fa-retweet" },
                new SelectListItem{ Value="fa fa-road", Text = "fa fa-road" },
                new SelectListItem{ Value="fa fa-rss", Text = "fa fa-rss" },
                new SelectListItem{ Value="fa fa-search", Text = "fa fa-search" },
                new SelectListItem{ Value="fa fa-share", Text = "fa fa-share" },
                new SelectListItem{ Value="fa fa-share-alt", Text = "fa fa-share-alt" },
                new SelectListItem{ Value="fa fa-shopping-cart", Text = "fa fa-shopping-cart" },
                new SelectListItem{ Value="fa fa-signal", Text = "fa fa-signal" },
                new SelectListItem{ Value="fa fa-sign-in", Text = "fa fa-sign-in" },
                new SelectListItem{ Value="fa fa-sign-out", Text = "fa fa-sign-out" },
                new SelectListItem{ Value="fa fa-sitemap", Text = "fa fa-sitemap" },
                new SelectListItem{ Value="fa fa-sort", Text = "fa fa-sort" },
                new SelectListItem{ Value="fa fa-sort-down", Text = "fa fa-sort-down" },
                new SelectListItem{ Value="fa fa-sort-up", Text = "fa fa-sort-up" },
                new SelectListItem{ Value="fa fa-spinner", Text = "fa fa-spinner" },
                new SelectListItem{ Value="fa fa-star", Text = "fa fa-star" },
                new SelectListItem{ Value="fa fa-star-o", Text = "fa fa-star-o" },
                new SelectListItem{ Value="fa fa-star-half", Text = "fa fa-star-half" },
                new SelectListItem{ Value="fa fa-star-half-o", Text = "fa fa-star-half-o" },
                new SelectListItem{ Value="fa fa-tablet", Text = "fa fa-tablet" },
                new SelectListItem{ Value="fa fa-tag", Text = "fa fa-tag" },
                new SelectListItem{ Value="fa fa-tags", Text = "fa fa-tags" },
                new SelectListItem{ Value="fa fa-tasks", Text = "fa fa-tasks" },
                new SelectListItem{ Value="fa fa-thumbs-down", Text = "fa fa-thumbs-down" },
                new SelectListItem{ Value="fa fa-thumbs-up", Text = "fa fa-thumbs-up" },
                new SelectListItem{ Value="fa fa-clock-o", Text = "fa fa-clock-o" },
                new SelectListItem{ Value="fa fa-tint", Text = "fa fa-tint" },
                new SelectListItem{ Value="fa fa-trash", Text = "fa fa-trash" },
                new SelectListItem{ Value="fa fa-trophy", Text = "fa fa-trophy" },
                new SelectListItem{ Value="fa fa-truck", Text = "fa fa-truck" },
                new SelectListItem{ Value="fa fa-umbrella", Text = "fa fa-umbrella" },
                new SelectListItem{ Value="fa fa-upload", Text = "fa fa-upload" },
                new SelectListItem{ Value="fa fa-user", Text = "fa fa-user" },
                new SelectListItem{ Value="fa fa-user-md", Text = "fa fa-user-md" },
                new SelectListItem{ Value="fa fa-volume-off", Text = "fa fa-volume-off" },
                new SelectListItem{ Value="fa fa-volume-down", Text = "fa fa-volume-down" },
                new SelectListItem{ Value="fa fa-volume-up", Text = "fa fa-volume-up" },
                new SelectListItem{ Value="fa fa-wrench", Text = "fa fa-wrench" },
                new SelectListItem{ Value="fa fa-search-plus", Text = "fa fa-search-plus" },
                new SelectListItem{ Value="fa fa-search-minus", Text = "fa fa-search-minus" },
                new SelectListItem{ Value="fa fa-file", Text = "fa fa-file" },
                new SelectListItem{ Value="fa fa-file-o", Text = "fa fa-file-o" },
                new SelectListItem{ Value="fa fa-cut", Text = "fa fa-cut" },
                new SelectListItem{ Value="fa fa-copy", Text = "fa fa-copy" },
                new SelectListItem{ Value="fa fa-paste", Text = "fa fa-paste" },
                new SelectListItem{ Value="fa fa-save", Text = "fa fa-save" },
                new SelectListItem{ Value="fa fa-undo", Text = "fa fa-undo" },
                new SelectListItem{ Value="fa fa-repeat", Text = "fa fa-repeat" },
                new SelectListItem{ Value="fa fa-text-height", Text = "fa fa-text-height" },
                new SelectListItem{ Value="fa fa-text-width", Text = "fa fa-text-width" },
                new SelectListItem{ Value="fa fa-align-left", Text = "fa fa-align-left" },
                new SelectListItem{ Value="fa fa-align-center", Text = "fa fa-align-center" },
                new SelectListItem{ Value="fa fa-align-right", Text = "fa fa-align-right" },
                new SelectListItem{ Value="fa fa-align-justify", Text = "fa fa-align-justify" },
                new SelectListItem{ Value="fa fa-indent", Text = "fa fa-indent" },
                new SelectListItem{ Value="fa fa-font", Text = "fa fa-font" },
                new SelectListItem{ Value="fa fa-bold", Text = "fa fa-bold" },
                new SelectListItem{ Value="fa fa-italic", Text = "fa fa-italic" },
                new SelectListItem{ Value="fa fa-strikethrough", Text = "fa fa-strikethrough" },
                new SelectListItem{ Value="fa fa-underline", Text = "fa fa-underline" },
                new SelectListItem{ Value="fa fa-link", Text = "fa fa-link" },
                new SelectListItem{ Value="fa fa-paperclip", Text = "fa fa-paperclip" },
                new SelectListItem{ Value="fa fa-columns", Text = "fa fa-columns" },
                new SelectListItem{ Value="fa fa-table", Text = "fa fa-table" },
                new SelectListItem{ Value="fa fa-th-large", Text = "fa fa-th-large" },
                new SelectListItem{ Value="fa fa-th", Text = "fa fa-th" },
                new SelectListItem{ Value="fa fa-th-list", Text = "fa fa-th-list" },
                new SelectListItem{ Value="fa fa-list", Text = "fa fa-list" },
                new SelectListItem{ Value="fa fa-list-ol", Text = "fa fa-list-ol" },
                new SelectListItem{ Value="fa fa-list-ul", Text = "fa fa-list-ul" },
                new SelectListItem{ Value="fa fa-list-alt", Text = "fa fa-list-alt" },
                new SelectListItem{ Value="fa fa-angle-left", Text = "fa fa-angle-left" },
                new SelectListItem{ Value="fa fa-angle-right", Text = "fa fa-angle-right" },
                new SelectListItem{ Value="fa fa-angle-up", Text = "fa fa-angle-up" },
                new SelectListItem{ Value="fa fa-angle-down", Text = "fa fa-angle-down" },
                new SelectListItem{ Value="fa fa-arrow-down", Text = "fa fa-arrow-down" },
                new SelectListItem{ Value="fa fa-arrow-left", Text = "fa fa-arrow-left" },
                new SelectListItem{ Value="fa fa-arrow-right", Text = "fa fa-arrow-right" },
                new SelectListItem{ Value="fa fa-arrow-up", Text = "fa fa-arrow-up" },
                new SelectListItem{ Value="fa fa-caret-down", Text = "fa fa-caret-down" },
                new SelectListItem{ Value="fa fa-caret-left", Text = "fa fa-caret-left" },
                new SelectListItem{ Value="fa fa-caret-right", Text = "fa fa-caret-right" },
                new SelectListItem{ Value="fa fa-caret-up", Text = "fa fa-caret-up" },
                new SelectListItem{ Value="fa fa-chevron-down", Text = "fa fa-chevron-down" },
                new SelectListItem{ Value="fa fa-chevron-left", Text = "fa fa-chevron-left" },
                new SelectListItem{ Value="fa fa-chevron-right", Text = "fa fa-chevron-right" },
                new SelectListItem{ Value="fa fa-chevron-up", Text = "fa fa-chevron-up" },
                new SelectListItem{ Value="fa fa-arrow-circle-down", Text = "fa fa-arrow-circle-down" },
                new SelectListItem{ Value="fa fa-arrow-circle-left", Text = "fa fa-arrow-circle-left" },
                new SelectListItem{ Value="fa fa-arrow-circle-right", Text = "fa fa-arrow-circle-right" },
                new SelectListItem{ Value="fa fa-arrow-circle-up", Text = "fa fa-arrow-circle-up" },
                new SelectListItem{ Value="fa fa-angle-double-left", Text = "fa fa-angle-double-left" },
                new SelectListItem{ Value="fa fa-angle-double-right", Text = "fa fa-angle-double-right" },
                new SelectListItem{ Value="fa fa-angle-double-up", Text = "fa fa-angle-double-up" },
                new SelectListItem{ Value="fa fa-angle-double-down", Text = "fa fa-angle-double-down" },
                new SelectListItem{ Value="fa fa-hand-o-down", Text = "fa fa-hand-o-down" },
                new SelectListItem{ Value="fa fa-hand-o-left", Text = "fa fa-hand-o-left" },
                new SelectListItem{ Value="fa fa-hand-o-right", Text = "fa fa-hand-o-right" },
                new SelectListItem{ Value="fa fa-hand-o-up", Text = "fa fa-hand-o-up" },
                new SelectListItem{ Value="fa fa-circle", Text = "fa fa-circle" },
                new SelectListItem{ Value="fa fa-play-circle", Text = "fa fa-play-circle" },
                new SelectListItem{ Value="fa fa-play", Text = "fa fa-play" },
                new SelectListItem{ Value="fa fa-pause", Text = "fa fa-pause" },
                new SelectListItem{ Value="fa fa-stop", Text = "fa fa-stop" },
                new SelectListItem{ Value="fa fa-step-backward", Text = "fa fa-step-backward" },
                new SelectListItem{ Value="fa fa-fast-backward", Text = "fa fa-fast-backward" },
                new SelectListItem{ Value="fa fa-backward", Text = "fa fa-backward" },
                new SelectListItem{ Value="fa fa-forward", Text = "fa fa-forward" },
                new SelectListItem{ Value="fa fa-fast-forward", Text = "fa fa-fast-forward" },
                new SelectListItem{ Value="fa fa-step-forward", Text = "fa fa-step-forward" },
                new SelectListItem{ Value="fa fa-eject", Text = "fa fa-eject" },
                new SelectListItem{ Value="fa fa-arrows-alt", Text = "fa fa-arrows-alt" },
                new SelectListItem{ Value="fa fa-compress", Text = "fa fa-compress" },
                new SelectListItem{ Value="fa fa-phone", Text = "fa fa-phone" },
                new SelectListItem{ Value="fa fa-phone-square", Text = "fa fa-phone-square" },
                new SelectListItem{ Value="fa fa-facebook", Text = "fa fa-facebook" },
                new SelectListItem{ Value="fa fa-facebook-official", Text = "fa fa-facebook-official" },
                new SelectListItem{ Value="fa fa-facebook-square", Text = "fa fa-facebook-square" },
                new SelectListItem{ Value="fa fa-twitter", Text = "fa fa-twitter" },
                new SelectListItem{ Value="fa fa-twitter-square", Text = "fa fa-twitter-square" },
                new SelectListItem{ Value="fa fa-github", Text = "fa fa-github" },
                new SelectListItem{ Value="fa fa-github-alt", Text = "fa fa-github-alt" },
                new SelectListItem{ Value="fa fa-github-square", Text = "fa fa-github-square" },
                new SelectListItem{ Value="fa fa-linkedin", Text = "fa fa-linkedin" },
                new SelectListItem{ Value="fa fa-linkedin-square", Text = "fa fa-linkedin-square" },
                new SelectListItem{ Value="fa fa-pinterest", Text = "fa fa-pinterest" },
                new SelectListItem{ Value="fa fa-pinterest-square", Text = "fa fa-pinterest-square" },
                new SelectListItem{ Value="fa fa-pinterest-p", Text = "fa fa-pinterest-p" },
                new SelectListItem{ Value="fa fa-google-plus", Text = "fa fa-google-plus" },
                new SelectListItem{ Value="fa fa-google-plus-square", Text = "fa fa-google-plus-square" },
                new SelectListItem{ Value="fa fa-skype", Text = "fa fa-skype" },
                new SelectListItem{ Value="fa fa-instagram", Text = "fa fa-instagram" },
                new SelectListItem{ Value="fa fa-snapchat", Text = "fa fa-snapchat" },
                new SelectListItem{ Value="fa fa-snapchat-ghost", Text = "fa fa-snapchat-ghost" },
                new SelectListItem{ Value="fa fa-snapchat-square", Text = "fa fa-snapchat-square" },
                new SelectListItem{ Value="fa fa-whatsapp", Text = "fa fa-whatsapp" },
                new SelectListItem{ Value="fa fa-flicker", Text = "fa fa-flicker" },
                new SelectListItem{ Value="fa fa-paypal", Text = "fa fa-paypal" },
                new SelectListItem{ Value="fa fa-soundcloud", Text = "fa fa-soundcloud" },
                new SelectListItem{ Value="fa fa-tumblr", Text = "fa fa-tumblr" },
                new SelectListItem{ Value="fa fa-tumblr-square", Text = "fa fa-tumblr-square" },
                new SelectListItem{ Value="fa fa-twitch", Text = "fa fa-twitch" },
                new SelectListItem{ Value="fa fa-vimeo", Text = "fa fa-vimeo" },
                new SelectListItem{ Value="fa fa-vimeo-square", Text = "fa fa-vimeo-square" },
                new SelectListItem{ Value="fa fa-vine", Text = "fa fa-vine" },
                new SelectListItem{ Value="fa fa-youtube", Text = "fa fa-youtube" },
                new SelectListItem{ Value="fa fa-youtube-play", Text = "fa fa-youtube-play" },
                new SelectListItem{ Value="fa fa-youtube-square", Text = "fa fa-youtube-square" },
                new SelectListItem{ Value="fa fa-ambulance", Text = "fa fa-ambulance" },
                new SelectListItem{ Value="fa fa-header", Text = "fa fa-hheader" },
                new SelectListItem{ Value="fa fa-hospital", Text = "fa fa-hospital" },
                new SelectListItem{ Value="fa fa-medkit", Text = "fa fa-medkit" },
                new SelectListItem{ Value="fa fa-plus-square", Text = "fa fa-plus-square" },
                new SelectListItem{ Value="fa fa-stethoscope", Text = "fa fa-stethoscope" },
                new SelectListItem{ Value="fa fa-user-md", Text = "fa fa-user-md" },
                #endregion
            };

            return items;
        }
    }


    public class DisplayExAttribute : DescriptionAttribute
    {
        public string Name { get; private set; }

        public DisplayExAttribute(string name, string description)
            : base(description)
        {
            Name = name;
        }
    }


    public static class EnumHelperEx
    {
        public static string ToDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T ParseEnum<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string ToSpacedTitleCase(this Enum value)
        {
            return value.ToString().ToSpacedTitleCase();
        }

    }

    public static class HttpPostedFileBaseExtensions
    {
        public static bool HasValue(this HttpPostedFileBase file)
        {
            return file != null && file.ContentLength > 0;
        }

        public static Byte[] GetBytes(this HttpPostedFileBase file)
        {
            using (var ms = new MemoryStream())
            {
                file.InputStream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static string GetBase64(this HttpPostedFileBase file)
        {
            var bytes = GetBytes(file);
            return Convert.ToBase64String(bytes);
        }

        public static string GetDataUri(this HttpPostedFileBase file)
        {
            var dataUri = new StringBuilder();
            dataUri.AppendFormat("data:{0};base64,", file.ContentType);
            var base64 = GetBase64(file);
            dataUri.Append(base64);
            return dataUri.ToString();
        }

        public static string FileExtension(this HttpPostedFileBase file)
        {
            var ext = Path.GetExtension(file.FileName).Replace(".", null);
            return ext;
        }
    }



    public static class GuidEx
    {
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        public static Guid NewSequentialId()
        {
            byte[] destinationArray = Guid.NewGuid().ToByteArray();
            DateTime time = new DateTime(0x76c, 1, 1);
            DateTime now = DateTime.Now;
            TimeSpan span = new TimeSpan(now.Ticks - time.Ticks);
            TimeSpan timeOfDay = now.TimeOfDay;
            byte[] bytes = BitConverter.GetBytes(span.Days);
            byte[] array = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));
            Array.Reverse(bytes);
            Array.Reverse(array);
            Array.Copy(bytes, bytes.Length - 2, destinationArray, destinationArray.Length - 6, 2);
            Array.Copy(array, array.Length - 4, destinationArray, destinationArray.Length - 4, 4);
            return new Guid(destinationArray);
        }

        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }

    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime? date)
        {
            return date.ToString(null, DateTimeFormatInfo.CurrentInfo);
        }

        public static string ToString(this DateTime? date, string format)
        {
            return date.ToString(format, DateTimeFormatInfo.CurrentInfo);
        }

        public static string ToString(this DateTime? date, IFormatProvider provider)
        {
            return date.ToString(null, provider);
        }

        public static string ToString(this DateTime? date, string format, IFormatProvider provider)
        {
            if (date.HasValue)
                return date.Value.ToString(format, provider);
            else
                return string.Empty;
        }

        public static string ToRelativeDateString(this DateTime date)
        {
            return GetRelativeDateValue(date, DateTime.Now);
        }
        public static string ToRelativeDateStringUtc(this DateTime date)
        {
            return GetRelativeDateValue(date, DateTime.UtcNow);
        }
        private static string GetRelativeDateValue(DateTime date, DateTime comparedTo)
        {
            TimeSpan diff = comparedTo.Subtract(date);
            if (diff.TotalDays >= 365)
                return string.Concat("on ", date.ToString("MMMM d, yyyy"));
            if (diff.TotalDays >= 7)
                return string.Concat("on ", date.ToString("MMMM d"));
            else if (diff.TotalDays > 1)
                return string.Format("{0:N0} days ago", diff.TotalDays);
            else if (diff.TotalDays == 1)
                return "yesterday";
            else if (diff.TotalHours >= 2)
                return string.Format("{0:N0} hours ago", diff.TotalHours);
            else if (diff.TotalMinutes >= 60)
                return "more than an hour ago";
            else if (diff.TotalMinutes >= 5)
                return string.Format("{0:N0} minutes ago", diff.TotalMinutes);
            if (diff.TotalMinutes >= 1)
                return "a few minutes ago";
            else
                return "less than a minute ago";
        }

        public static string GetTimeSpan(this DateTime dateTime)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }

    public static class StringExtensions
    {
        public static string Singularize(this string text)
        {
            return PluralizationService
                  .CreateService(CultureInfo.CurrentUICulture)
                  .Singularize(text);
        }

        public static string Pluralize(this string text)
        {
            if (text.Contains('-'))
            {
                string[] hldr;
                hldr = text.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var cnt = hldr.Length;
                text = hldr[cnt - 1];
                var plr = PluralizationService
                  .CreateService(CultureInfo.CurrentUICulture)
                  .Pluralize(text);
                hldr[cnt - 1] = plr;
                text = string.Join("-", hldr);
                return text;
            }
            return PluralizationService
                  .CreateService(CultureInfo.CurrentUICulture)
                  .Pluralize(text);
        }

        public static string Ellipsis(this string originalText, int length, bool fullWord = false)
        {
            if (originalText == null || originalText.Length <= length) return originalText;
            var chars = new[] { " ", "-", "_" };

            if (!fullWord) return originalText.Substring(0, length) + "...";

            foreach (var ch in chars)
            {
                int pos = originalText.IndexOf(ch, length);
                if (pos >= 0)
                    return originalText.Substring(0, pos) + "...";
            }
            return originalText;
        }

        public static string StripHtml(this string text)
        {
            var _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
            return _htmlRegex.Replace(text, string.Empty);
        }

        public static string Shuffle(this string str)
        {
            Random num = new Random();
            string rand = new string(str.ToCharArray().OrderBy(s => (num.Next(2) % 2) == 0).ToArray());
            return rand;
        }
        public static bool HasValue(this string s, bool checkWhiteSpace = true)
        {
            return checkWhiteSpace ? !string.IsNullOrWhiteSpace(s) : !string.IsNullOrEmpty(s);
        }

        public static string FormatString(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string ToTitleCase(this string s)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s);
        }

        public static string ToSpacedTitleCase(this string s)
        {
            var str = Regex.Replace(s, "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            return ToTitleCase(str);
        }


        public static string ToUrlFriendly(this string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            string s;
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    s = c.ToString().ToLowerInvariant();
                    if ("àåáâäãåą".Contains(s))
                    {
                        sb.Append("a");
                    }
                    else if ("èéêëę".Contains(s))
                    {
                        sb.Append("e");
                    }
                    else if ("ìíîïı".Contains(s))
                    {
                        sb.Append("i");
                    }
                    else if ("òóôõöø".Contains(s))
                    {
                        sb.Append("o");
                    }
                    else if ("ùúûü".Contains(s))
                    {
                        sb.Append("u");
                    }
                    else if ("çćč".Contains(s))
                    {
                        sb.Append("c");
                    }
                    else if ("żźž".Contains(s))
                    {
                        sb.Append("z");
                    }
                    else if ("śşš".Contains(s))
                    {
                        sb.Append("s");
                    }
                    else if ("ñń".Contains(s))
                    {
                        sb.Append("n");
                    }
                    else if ("ýŸ".Contains(s))
                    {
                        sb.Append("y");
                    }
                    else if (c == 'ł')
                    {
                        sb.Append("l");
                    }
                    else if (c == 'đ')
                    {
                        sb.Append("d");
                    }
                    else if (c == 'ß')
                    {
                        sb.Append("ss");
                    }
                    else if (c == 'ğ')
                    {
                        sb.Append("g");
                    }
                    prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }
    }
}
