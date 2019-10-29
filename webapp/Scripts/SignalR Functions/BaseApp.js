//--- SignalR Functions
$(function () {
    var realTimeHub = $.connection.realTimeHub;

    //--- Updating Notifications
    realTimeHub.client.updateNotifications = function (content) {
        getNotifications();
        getNotificationWarnings();

        var encodedResult = $('<div />').text(content).html();
        $('#notifyMe').notify({
            message: { text: encodedResult },
            transition: 'fade',
            type: 'success'
        }).show();
    };
    //---

    //--- Updating Messages
    realTimeHub.client.updateMessages = function (content) {
        getMessages();

        if (content.length > 0)
        {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---



    /*------------------- Base App -------------------*/
    //--- Access Control Users
    realTimeHub.client.updateAppUsers = function (content) {
        getAppUsers("#acl-Users-dataTable", getValueAtIndex(5));

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- Access Control Roles
    realTimeHub.client.updateAppUserRoles = function (content) {
        getAppUserRoles("#acl-Roles-dataTable");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- Configuration SMTP Controls
    realTimeHub.client.updateSmtpControls = function (content) {
        getSmtpControls("#config-Smtp-dataTable");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---
    /*--------------------------------------*/


    
    /*------------------- CMS -------------------*/
    //--- CMS Pages
    realTimeHub.client.updateCmsPages = function (content) {
        getCmsPages("#cms-dataTable");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- CMS Slides
    realTimeHub.client.updateCmsSlides = function (content) {
        getCmsSlides("#cms-AllSlides");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- CMS News
    realTimeHub.client.updateCmsNews = function (content) {
        getCmsNews("#cms-News-dataTable");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- CMS Newsletter Subscribers
    realTimeHub.client.updateCmsNewsletterSubscribers = function (content) {
        getCmsNewsletterSubscribers("#cms-Newsletter-Subscribers-dataTable");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- CMS Newsletter Posts
    realTimeHub.client.updateCmsNewsletterPosts = function (content) {
        getCmsNewsletterPosts("#cms-Newsletter-Posts-dataTable");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- CMS Social Media Icons
    realTimeHub.client.updateCmsSocialMediaIcons = function (content) {
        getCmsSocialMediaIcons("#cms-SocialMedia-Icons");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- CMS File Manager
    realTimeHub.client.updateCmsFileManager = function (content) {
        getCmsFileManager("#cms-FileManager");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---

    //--- CMS Contents
    realTimeHub.client.updateCmsContents = function (content) {
        getCmsContents("#cms-Contents-dataTable");

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---
    /*--------------------------------------*/


    /*------------------- DMS -------------------*/
    //--- DMS List
    realTimeHub.client.updateDMS = function (content) {
        document.getElementsByClassName('dms-frame')[0].contentWindow.reloadDMS();

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---
    /*--------------------------------------*/



    /*------------------- Employee Management -------------------*/
    //--- Clinicul Patients
    realTimeHub.client.updateEmployees = function (content) {
        $("#employees-Container").html("");
        $("#employees-Container").load('/Secure/Employee/_AllEmployees');
        
        filterDataTable("#employees-dataTable", [1, 2, 3, 4, 5, 6], [0, 7]);

        if (content.length > 0) {
            var encodedResult = $('<div />').text(content).html();
            $('#notifyMe').notify({
                message: { text: encodedResult },
                transition: 'fade',
                type: 'success'
            }).show();
        }
    };
    //---
    /*--------------------------------------*/



    $.connection.hub.start().done(function () {
        // Logic for which should be shown on first start
    });
});
//---


/*------------------- Json Requests for Base App -------------------*/
function getAppUsers(dataTable, officeId) {
    var ofcId = officeId;
    if (officeId == null)
    {
        ofcId = "";
    }

    $.get('/Secure/AccessControl/GetAllUsers/' + ofcId).success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td><a href="~/Secure/AccessControl/Details/' + data[x].Id + '">' + data[x].Code + '</a></td>';

            if (ofcId == "")
            {
                markup += '<td><a href="~/Secure/AccessControl/Details/' + data[x].OfficeId + '">' + data[x].Office.Title + '</a></td>';
            }

            markup += '<td><a href="~/Secure/AccessControl/Roles">' + data[x].Role.Code + ' ' + data[x].Role.Title + '</a></td>';
            markup += '<td>' + data[x].Username + '</td>';
            markup += '<td>' + data[x].FullName + '</td>';

            if (data[x].Status == "Active") {
                markup += '<td><label class="label label-success">Active</label></td>';
            }
            else {
                markup += '<td><label class="label label-warning">Suspended</label></td>';
            }
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="/Secure/AccessControl/Details/' + data[x].Id + '" class="btn btn-primary" title="Details"><i class="fa fa-eye"></i></a>';
            markup += '<a href="/Secure/AccessControl/Record/' + data[x].Id + '" class="btn btn-info" title="Edit"><i class="fa fa-edit"></i></a>';

            var changeRecordData = "changeStatus('/Secure/AccessControl/ChangeStatus', '" + data[x].Id + "')";
            var deleteRecordData = "deleteRecord('/Secure/AccessControl/Delete', '" + data[x].Id + "')";
            if (data[x].Status == "Active") {
                markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-warning" title="Active Now"><i class="fa fa-ban"></i></a>';
            }
            else {
                markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-success" title="Suspend Now"><i class="fa fa-check"></i></a>';
            }
            //markup += '<a href="javascript://" data-id="' + data[x].Id + '" class="btn btn-default managAccess" title="Manage Access"><i class="fa fa-gear"></i></a>';
            markup += '<a href="/Secure/AccessControl/AccessManagement/' + data[x].Id + '" class="btn btn-default" title="Manage Access"><i class="fa fa-gear"></i></a>';
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

function getAppUserRoles(dataTable) {
    $.get('/Secure/AccessControl/GetAllRoles/').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td>';

            if (!data[x].IsSystem)
            {
                if (data[x].Contacts.length == 0)
                {
                    markup += '<input type="checkbox" class="checkIt" value="' + data[x].Id + '" />';
                }
            }
            markup += '</td>';
            if (data[x].IsSystem) {
                markup += '<td>' + data[x].Code + '</td>';
            }
            else {
                markup += '<td><a href="/Secure/AccessControl/Role/' + data[x].Id + '">' + data[x].Code + '</a></td>';
            }
            markup += '<td>' + data[x].Title + '</td>';
            markup += '<td>' + data[x].Description + '</td>';

            if (data[x].Permissions.length != 0) {
                if (data[x].Permissions == "All") {
                    markup += '<td>All</td>';
                }
                else {
                    markup += '<td><ul class="no-padding no-margin" style="list-style:none;">';
                    for (var y = 0; y < data[x].PermissionsList.length; y++) {
                        if (data[x].PermissionsList[y].indexOf("View") == -1) {
                            markup += '<li><i class="fa fa-check text-success"></i> Control ' + data[x].PermissionsList[y].replace(/([A-Z]+)/g, " $1").replace(/([A-Z][a-z])/g, " $1") + '</li>';
                        }
                        else {
                            markup += '<li><i class="fa fa-check text-success"></i> ' + data[x].PermissionsList[y].replace(/([A-Z]+)/g, " $1").replace(/([A-Z][a-z])/g, " $1") + '</li>';
                        }
                    }
                    markup += '</ul></td>';
                }
            }
            else {
                markup += '<td>No Permission</td>';
            }
            markup += '<td>' + formatDateTime(data[x].CreatedOn) + '</td>';
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            if (data[x].IsSystem) {
                markup += '<a href="javascript://" class="btn btn-default" title="System Role"><i class="fa fa-lock"></i></a>';
            }
            else {
                markup += '<a href="/Secure/AccessControl/Role/' + data[x].Id + '" class="btn btn-info" title="Edit"><i class="fa fa-edit"></i></a>';
                if (data[x].Contacts.length == 0) {
                    var deleteRecordData = "deleteRecord('/Secure/AccessControl/DeleteRole', '" + data[x].Id + "')";
                    markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';
                }
            }

            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

function getSmtpControls(dataTable) {
    $.get('/Secure/Config/GetAllSmtpControls/').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + data[x].Title + '</td>';
            markup += '<td>' + data[x].ServerType + '</td>';
            markup += '<td>' + data[x].IncomingServer + '</td>';
            markup += '<td>' + data[x].OutgoingServer + '</td>';
            markup += '<td>' + data[x].DefaultUsername + '</td>';
            markup += '<td>' + data[x].ModuleName + '</td>';

            if (data[x].IsDefault) {
                markup += '<td><label class="label label-success">Default</label></td>';
            }
            else {
                markup += '<td> &nbsp; </td>';
            }
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="/Secure/Config/Smtp/' + data[x].Id + '" class="btn btn-info" title="Edit"><i class="fa fa-edit"></i></a>';
            var deleteRecordData = "deleteRecord('/Secure/Config/DeleteSmtp', '" + data[x].Id + "')";
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};
/*--------------------------------------*/


/*------------------- Json Requests for CMS -------------------*/
function getCmsPages(dataTable) {
    $.get('/CMS/WebPage/GetAllPages').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + data[x].Name + '</td>';
            markup += '<td>' + data[x].ParentName + '</td>';
            if (data[x].IsLink) {
                markup += '<td><a href="' + data[x].LinkUrl + '" target="_blank">' + data[x].LinkUrl + '</a></td>';
            }
            else {
                markup += '<td><a href="/' + data[x].Slug + '" target="_blank">/' + data[x].Slug + '</a></td>';
            }

            if (data[x].Status == "Published") {
                markup += '<td><label class="label label-success">Published</label></td>';
            }
            else {
                markup += '<td><label class="label label-warning">Draft</label></td>';
            }
            markup += '<td>' + formatDateTime(data[x].UpdatedOn) + '</td>';
            markup += '<td>' + formatDateTime(data[x].CreatedOn) + '</td>';
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="/' + data[x].Id + '" class="btn btn-primary" title="View" target="_blank"><i class="fa fa-share"></i></a>';
            markup += '<a href="/CMS/WebPage/Record/' + data[x].Id + '" class="btn btn-info" title="Edit"><i class="fa fa-edit"></i></a>';

            if (!data[x].IsSystem) {
                if (data[x].ChildPages.length == 0) {
                    var changeRecordData = "changeStatus('/CMS/WebPage/ChangePageStatus', '" + data[x].Id + "')";
                    var deleteRecordData = "deleteRecord('/CMS/WebPage/DeletePage', '" + data[x].Id + "')";
                    if (data[x].Status == "Published") {
                        markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-warning" title="Publish Now"><i class="fa fa-ban"></i></a>';
                    }
                    else {
                        markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-success" title="Draft Now"><i class="fa fa-check"></i></a>';
                    }
                    markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';
                }
            }

            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

function getCmsSlides(divId) {
    $.get('/CMS/WebSlide/GetAllSlides').success(function (data) {
        var markup = "";

        if (data.length == 0) {
            markup += '<div class="col-md-12 text-center" style="padding:170px 15px;"><h5>There is no slide to show. Added slides will show here.</h5></div>'
        }
        else {
            var checkBoxClass = "checkAll(this, 'checkIt')";
            markup += '<div class="col-md-12"><label><input type="checkbox" onchange="' + checkBoxClass + '" /> Select All</label></div>';
            for (var x = 0; x < data.length; x++) {
                markup += '<div class="col-sm-6 col-md-4"><div class="thumbnail">';
                markup += '<img src="/Content/Uploads/Slides/' + data[x].FileName + '" alt="' + data[x].Title + '" style="max-height:180px;">'
                markup += '<div class="caption text-center">';
                markup += '<h4>' + data[x].Title + '</h4>';
                markup += '<p>';
                if (data[x].Status == "Published") {
                    markup += '<label class="label label-success">Published</label>';
                }
                else {
                    markup += '<label class="label label-warning">Draft</label>';
                }
                markup += '</p><p><div class="btn-group btn-group-xs" role="group">'
                markup += '<div class="btn btn-default" style="padding-top:3px; height:22px;"><input type="checkbox" class="checkIt" style="margin:0;" value="' + data[x].Id + '" /></div>'
                markup += '<a href="/CMS/WebSlide/Record/' + data[x].Id + '" class="btn btn-info"><i class="fa fa-edit"></i> Edit</a>';

                var changeRecordData = "changeStatus('/CMS/WebSlide/ChangeSlideStatus', '" + data[x].Id + "')";
                var deleteRecordData = "deleteRecord('/CMS/WebSlide/Delete', '" + data[x].Id + "')";
                if (data[x].Status == "Published") {
                    markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-warning" title="Publish Now"><i class="fa fa-ban"></i></a>';
                }
                else {
                    markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-success" title="Draft Now"><i class="fa fa-check"></i></a>';
                }
                markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

                markup += '</div></p></div></div></div>';
            }
        }

        
        $(divId).html(markup);
    });
};

function getCmsNews(dataTable) {
    $.get('/CMS/WebNews/GetAllNews').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + formatDate(data[x].DateTime) + '</td>';
            markup += '<td>' + data[x].Title + '</td>';
            if (data[x].Status == "Published") {
                markup += '<td><label class="label label-success">Published</label></td>';
            }
            else {
                markup += '<td><label class="label label-warning">Draft</label></td>';
            }
            markup += '<td>' + formatDateTime(data[x].UpdatedOn) + '</td>';
            markup += '<td>' + formatDateTime(data[x].CreatedOn) + '</td>';
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="/CMS/WebNews/Record/' + data[x].Id + '" class="btn btn-info" title="Edit"><i class="fa fa-edit"></i></a>';
            var changeRecordData = "changeStatus('/CMS/WebNews/ChangeNewsStatus', '" + data[x].Id + "')";
            var deleteRecordData = "deleteRecord('/CMS/WebNews/Delete', '" + data[x].Id + "')";
            if (data[x].Status == "Published") {
                markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-warning" title="Publish Now"><i class="fa fa-ban"></i></a>';
            }
            else {
                markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-success" title="Draft Now"><i class="fa fa-check"></i></a>';
            }
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

            markup += '</div></td>';

            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

function getCmsNewsletterSubscribers(dataTable) {
    $.get('/CMS/Newsletter/GetAllSubscribers').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + data[x].Email + '</td>';
            markup += '<td>' + formatDateTime(data[x].CreatedOn) + '</td>';
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            var deleteRecordData = "deleteRecord('/CMS/Newsletter/DeleteEmail', '" + data[x].Id + "')";
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';
            markup += '</div></td>';
            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

function getCmsNewsletterPosts(dataTable) {
    $.get('/CMS/Newsletter/GetAllPosts').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + data[x].Subject + '</td>';
            if (data[x].Status == "Sent") {
                markup += '<td><label class="label label-success">Sent</label></td>';
            }
            else {
                markup += '<td><label class="label label-warning">Draft</label></td>';
            }
            markup += '<td>' + formatDateTime(data[x].CreatedOn) + '</td>';
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="/CMS/Newsletter/Details/' + data[x].Id + '" class="btn btn-primary" title="View" target="_blank"><i class="fa fa-eye"></i></a>';
            if (data[x].Status == "Draft") {
                markup += '<a href="/CMS/Newsletter/Record/' + data[x].Id + '" class="btn btn-info" title="Edit"><i class="fa fa-edit"></i></a>';
                var deleteRecordData = "deleteRecord('/CMS/Newsletter/Delete', '" + data[x].Id + "')";
                markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';
            }
            markup += '</div></td>';
            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};

function getCmsSocialMediaIcons(divId) {
    $.get('/CMS/SocialMedia/GetAllSocialMediaIcons').success(function (data) {
        var markup = "";

        if (data.length == 0) {
            markup += '<div class="col-md-12 text-center" style="padding:100px 15px;"><h5>There is no social media to show. Added files will show here.</h5></div>'
        }
        else {
            var checkBoxClass = "checkAll(this, 'checkIt')";
            markup += '<div class="col-md-12"><label><input type="checkbox" onchange="' + checkBoxClass + '" /> Select All</label></div>';
            for (var x = 0; x < data.length; x++) {
                markup += '<div class="col-sm-4 col-md-3"><div class="thumbnail text-center">';
                markup += '<i class="' + data[x].Icon + '" style="font-size:70px;"></i>';
                markup += '<div class="caption text-center">';
                markup += '<h6>' + data[x].Name + '</h6>';
                markup += '<a href="' + data[x].LinkURL + '" target="_blank">' + data[x].LinkURL + '</a>';
                markup += '<p><div class="btn-group btn-group-xs" role="group">'
                markup += '<div class="btn btn-default" style="padding-top:3px; height:22px;"><input type="checkbox" class="checkIt" style="margin:0;" value="' + data[x].Id + '" /></div>'
                markup += '<a href="/CMS/SocialMedia/Index/' + data[x].Id + '" class="btn btn-info"><i class="fa fa-edit"></i> Edit</a>';
                var deleteRecordData = "deleteRecord('/CMS/SocialMedia/Delete', '" + data[x].Id + "')";
                markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';
                markup += '</div></p></div></div></div>';
            }
        }


        $(divId).html(markup);
    });
};

function getCmsFileManager(divId) {
    $.get('/CMS/FileManager/GetAllFiles').success(function (data) {
        var markup = "";

        if (data.length == 0) {
            markup += '<div class="col-md-12 text-center" style="padding:100px 15px;"><h5>There is no file to show. Added files will show here.</h5></div>'
        }
        else {
            var checkBoxClass = "checkAll(this, 'checkIt')";
            markup += '<div class="col-md-12"><label><input type="checkbox" onchange="' + checkBoxClass + '" /> Select All</label></div>';
            for (var x = 0; x < data.length; x++) {
                markup += '<div class="col-sm-4 col-md-4"><div class="thumbnail text-center">';

                if (data[x].ContentType.indexOf("image") > -1) {
                    markup += '<img src="/Content/Uploads/Files/' + data[x].FileName + '" alt="' + data[x].Name + '" style="max-height:140px;">';
                }
                else {
                    if (data[x].Extension.toLowerCase() == "docx" || data[x].Extension.toLowerCase() == "doc") {
                        markup += '<i class="fa fa-file-word-o" style="font-size:140px;"></i>';
                    }
                    else if (data[x].Extension.toLowerCase() == "xlsx" || data[x].Extension.toLowerCase() == "xls") {
                        markup += '<i class="fa fa-file-excel-o" style="font-size:140px;"></i>';
                    }
                    else if (data[x].Extension.toLowerCase() == "pptx" || data[x].Extension.toLowerCase() == "ppt") {
                        markup += '<i class="fa fa-file-powerpoint-o" style="font-size:140px;"></i>';
                    }
                    else if (data[x].Extension.toLowerCase() == "zip") {
                        markup += '<i class="fa fa-file-zip-o" style="font-size:140px;"></i>';
                    }
                    else if (data[x].Extension.toLowerCase() == "txt") {
                        markup += '<i class="fa fa-file-text-o" style="font-size:140px;"></i>';
                    }
                    else {
                        markup += '<i class="fa fa-file" style="font-size:140px;"></i>';
                    }
                }
                
                markup += '<div class="caption text-center">';
                markup += '<h6>' + data[x].Name + '</h6>';
                markup += '<p>/Content/Uploads/Files/' + data[x].FileName + '</p>';
                markup += '<p><div class="btn-group btn-group-xs" role="group">'
                markup += '<div class="btn btn-default" style="padding-top:3px; height:22px;"><input type="checkbox" class="checkIt" style="margin:0;" value="' + data[x].Id + '" /></div>'
                markup += '<a href="/Content/Uploads/Files/' + data[x].Id + '" target="_blank" class="btn btn-primary"><i class="fa fa-eye"></i> View</a>';
                var deleteRecordData = "deleteRecord('/CMS/FileManager/Delete', '" + data[x].Id + "')";
                markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';
                markup += '</div></p></div></div></div>';
            }
        }

        $(divId).html(markup);
    });
};

function getCmsContents(dataTable) {
    $.get('/CMS/WebContent/GetAllContents').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + data[x].Name + '</td>';
            if (data[x].Status == "Published") {
                markup += '<td><label class="label label-success">Published</label></td>';
            }
            else {
                markup += '<td><label class="label label-warning">Draft</label></td>';
            }
            markup += '<td>' + formatDateTime(data[x].UpdatedOn) + '</td>';
            markup += '<td>' + formatDateTime(data[x].CreatedOn) + '</td>';
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="/CMS/WebContent/Record/' + data[x].Id + '" class="btn btn-info" title="Edit"><i class="fa fa-edit"></i></a>';
            
            var changeRecordData = "changeStatus('/CMS/WebContent/ChangeContentStatus', '" + data[x].Id + "')";
            var deleteRecordData = "deleteRecord('/CMS/WebContent/Delete', '" + data[x].Id + "')";
            if (data[x].Status == "Published") {
                markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-warning" title="Publish Now"><i class="fa fa-ban"></i></a>';
            }
            else {
                markup += '<a href="javascript://" onclick="' + changeRecordData + '" class="btn btn-success" title="Draft Now"><i class="fa fa-check"></i></a>';
            }
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

            markup += '</div></td>';
            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};
/*--------------------------------------*/


/*------------------- Json Requests for DMS -------------------*/

/*--------------------------------------*/



/*------------------- Json Requests for Clinicul -------------------*/
function getCliniculPatients(dataTable) {
    $.get('/Clinicul/Patient/GetAllPatients').success(function (data) {
        var markup = "";
        for (var x = 0; x < data.length; x++) {
            markup += '<tr>';
            markup += '<td><input type="checkbox" class="checkIt" value="' + data[x].Id + '" /></td>';
            markup += '<td>' + data[x].Code + '</td>';
            markup += '<td>' + data[x].FullName + '</td>';
            markup += '<td>' + data[x].Email + '</td>';
            markup += '<td>' + data[x].Phone + '</td>';
            markup += '<td>' + data[x].City + '</td>';
            markup += '<td>' + formatDateTime(data[x].UpdatedOn) + '</td>';
            markup += '<td class="text-center"><div class="btn-group btn-group-xs" role="group">';
            markup += '<a href="/Clinicul/Patient/Summary/' + data[x].Id + '" class="btn btn-primary" title="Details"><i class="fa fa-eye"></i></a>';
            markup += '<a href="/Clinicul/Patient/Record/' + data[x].Id + '" class="btn btn-info" title="Edit"><i class="fa fa-edit"></i></a>';

            var deleteRecordData = "deleteRecord('/Clinicul/Patient/Delete', '" + data[x].Id + "')";
            markup += '<a href="javascript://" onclick="' + deleteRecordData + '" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>';

            markup += '</div></td>';
            markup += "</tr>";
        }
        $(dataTable + ' tbody').html(markup);
    });
};
/*--------------------------------------*/



/*------------------- Formatting Date Time -------------------*/
//--- Formatting Date Time
function formatDateTime(d) {
    var date = new Date(Date.parse(d));

    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + " " + strTime;
};
//---

//--- Formatting Date
function formatDate(d) {
    var date = new Date(Date.parse(d));

    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear();
};
//---
/*--------------------------------------*/


/*------------------- Getting Route Values -------------------*/
function getValueAtIndex(index) {
    var str = window.location.href; 
    return str.split("/")[index];
}
/*--------------------------------------*/