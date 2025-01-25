$(document).ready(function () {
    $("#AddDate, #reminderdate").datepicker({
        dateFormat: "dd/mm/yy"
    });
    $("#docDate").datepicker({
        dateFormat: "dd/mm/yy"
    });
    $("#meetingDate").datepicker({
        dateFormat: "dd/mm/yy"
    });
    $("#nextAppointmentDate").datepicker({
        dateFormat: "dd/mm/yy"
    });
    $("#reminderDateTime").datepicker({
        dateFormat: "dd/mm/yy"
    })

    //Create Lead 
    $("#btnAddNewLead").click(function(){
        var leadData = createPayLoad();
        $.ajax({
            url: "/SalesTracker/AddLeadNew",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(leadData),
            success: function (response) {
                $("#responseMessage").text("LeadNumber: "+response[1]+" has created successfully");
                // Show the modal 
                $("#responseModal").modal('show');
                getLeadDetailByIDAfterPost(response[0]);
                resetForm();
            },
            error: function (error) {
                console.error("Error adding lead:", error);
                $("#responseMessage").text("An error occurred while adding the lead.");
                $("#responseModal").modal('show');
            }
        });
    });
   
    $('#salesTable').DataTable({ 
    "order": [[0, "asc"]],
        // Default sorting on the first column (ContactNo) 
        "columnDefs": [
            { "orderable": true, "targets": 0 }, // ContactNo 
            { "orderable": true, "targets": 1 }, // Key Person 
            { "orderable": true, "targets": 2 }, // Category 
            { "orderable": true, "targets": 3 }, // Lead Status 
            { "orderable": true, "targets": 4 }, // Lead Date 
            { "orderable": true, "targets": 5 }, // Reminder Date 
            { "orderable": false, "targets": 6 } // Action (disable sorting)
        ]
    });

    // get today's actionable leads
    getLeadBySelectedKPI('total-leads-details','ActionableLeads');

    $("#btnNewLead").click(function () {
        $("#addLeadModal").toggle();
        $("#details-container").toggle();
    });
    $("#btnCancel").click(function () {
        $("#addLeadModal").toggle();
        $("#details-container").toggle();
    })
    $("#btnModalClose").click(function () {
        $("#addLeadModal").toggle();
        $("#details-container").toggle();
    })
    
});
function resetForm() {
    $("#myForm")[0].reset();
}
function getLeadBySelectedKPI(id, selectedKPI) {
    var details = document.getElementById(id);
    details.style.display = "block";
    if(selectedKPI=='TotalLeads')
    $("#LeadSection").text("Total Leads");
    else if (selectedKPI == 'ActiveLeads')
        $("#LeadSection").text("Active Leads");
    else if (selectedKPI == 'ConvertedLeads')
        $("#LeadSection").text("Converted Leads");
    else 
        $("#LeadSection").text("Today's Actionable Leads");
    $.ajax({
        url: '/SalesTracker/GetLeadbySelectedKPI',
        type: 'GET',
        data: { selectedKPI: selectedKPI },
        success: function (data) {
            // Check if DataTable is already initialized
            if ($.fn.DataTable.isDataTable('#salesTable')) {
                $('#salesTable').DataTable().clear().destroy();
            }
            var rows = '';
            $.each(data, function (index, leads) {
                var style = '';
                //style = leads.lead.isOverdue == true ? "backgroudcolor:red":'';
                rowClass = leads.lead.isOverdue == true ? "overdue" : '';
                rows += '<tr id="row-' + leads.lead.leadId + '" class="' + rowClass + '">';
                rows += '<td>' + leads.lead.leadNumber + '</td>';
                rows += '<td>' + leads.lead.contactNo + '</td>';
                rows += '<td>' + leads.lead.contactPerson + '</td>';
                rows += '<td>' + leads.lead.leadCategory + '</td>';
                rows += '<td>' + leads.leadActivity.leadStatus + '</td>';
                rows += '<td>' + leads.leadActivity.leadDate + '</td>';
                rows += '<td>' + leads.leadActivity.meetingDate + '</td>';
                rows += '<td><button class="action-btn" onclick="getLeadDetailByID(' + leads.lead.leadId + ')"><i class="fas fa-edit"></i></button></td>';
                rows += '</tr>';
            });
            $('#salesTable tbody').empty().append(rows);

            // Initialize DataTable after data has been added
            $('#salesTable').DataTable({
                //"destroy": true, // Allows reinitialization
                "order": [[5, "desc"]], // Default sorting on the first column (ContactNo)
                "columnDefs": [
                    { "orderable": true, "targets": 0 }, // Contact No
                    { "orderable": true, "targets": 1 }, // Key Person
                    { "orderable": true, "targets": 2 }, // Category
                    { "orderable": true, "targets": 3 }, // Lead Status
                    { "orderable": true, "targets": 4 }, // Lead Date
                    { "orderable": true, "targets": 5 }, // Reminder Date
                    { "orderable": true, "targets": 6 },
                    { "orderable": false, "targets": 7 } // Action (disable sorting)
                ]
            });
        },
        error: function (error) {
            console.error("Error fetching data: ", error);
        }
    });
}

function getLeadDetailByID(id) {
    $.ajax({
        url: '/SalesTracker/GetLeadDetailByIdNew',
        type: 'GET',
        data: { LeadId: id },
        success: function (data) {
            // bind Lead Info
            setLeadActivity(data.leadComposite.lead);
            //bind Lead Activity info
            setLeadActivityDetails(data.leadComposite.leadActivity);
            //bind recent activities history
            bindRecentactivities(data.recentActivities);
            //toggle Divs
            $("#addLeadModal").toggle();
            $("#details-container").toggle();
        }
    });
}

function getLeadDetailByIDAfterPost(id) {
    $.ajax({
        url: '/SalesTracker/GetLeadDetailByIdNew',
        type: 'GET',
        data: { LeadId: id },
        success: function (data) {
            var lead = data.leadComposite.lead;
            var leadActivity = data.leadComposite.leadActivity;

            var rowId = 'row-' + id;
            var row = document.getElementById(rowId);
            if (row) {
                // Update the columns with new values 
                row.children[0].textContent = lead.leadNumber;
                row.children[1].textContent=  lead.contactNo;
                row.children[2].textContent = lead.contactPerson;
                row.children[3].textContent = lead.leadCategory;
                row.children[4].textContent = leadActivity.leadStatus;
                row.children[5].textContent = leadActivity.leadDate;
                row.children[6].textContent = leadActivity.reminderDate;
            }
            else {
                row += '<tr id=row-' + lead.leadId + '>';
                row += '<td>' + lead.leadNumber + '</td>';
                row += '<td>' + lead.contactNo + '</td>';
                row += '<td>' + lead.contactPerson + '</td>';
                row += '<td>' + lead.leadCategory + '</td>';
                row += '<td>' + leadActivity.leadStatus + '</td>';
                row += '<td>' + leadActivity.leadDate + '</td>';
                row += '<td>' + leadActivity.reminderDate + '</td>';
                row += '<td><button class="action-btn" onclick="getLeadDetailByID(' + lead.leadId + ')"><i class="fas fa-edit"></i></button></td>';
                row += '</tr>';
                $('#salesTable tbody').append(row);
            }
            //bind recent activities history
            bindRecentactivities(data.recentActivities);
        }
    });
}
function bindRecentactivities(recentActivitiesList) {
    let activitiesList = $('#recent-activities-list');
    activitiesList.empty(); // Clear existing content 
    recentActivitiesList.forEach(item => {
        activitiesList.append(`
                            <div class="chat-message">
                                <p class="activity-date"><strong>${item.updatedDate}</strong></p>
                                <div class="message-content">
                                    <p><strong>${item.updatedBy}:</strong> ${item.responseDesc} <i class="fas fa-envelope-open"></i>.</p>
                                </div>
                            </div>
                        `);
    });
}
function createPayLoad() {
    var leadData = {
        Lead: {
            LeadId: $("#leadId").val(),
            //DocNo: $("#docNo").val(),
            //DocDate: $("#docDate").val(),
            LeadStatus: $("#leadStatus").val(),
            LeadSource: $("#leadSource").val(),
            LeadOwner: $("#leadOwner").val(),
            LeadPriority: $("#leadPriority").val(),
            LeadOwnerContactNo: $("#leadOwnerContactNo").val(),
            LeadTitle: $("#leadTitle").val(),
            LeadCategory: $("#leadCategory").val(),
            LeadDesc: $("#leadDesc").val(),
            SpecialRequirement: $("#specialRequirement").val(),
            PotentialDealValue: $("#potentialDealValue").val(),
            ProbabilityofConversion: $("#probabilityofConversion").val(),
            ClosureForecast: $("#closureForecast").val(),
            CompanyName: $("#companyName").val(),
            BusinessSector: $("#businessSector").val(),
            ContactPerson: $("#contactPerson").val(),
            ContactNo: $("#contactNo").val(),
            Designation: $("#designation").val(),
            Email: $("#email").val(),
            Address: $("#address").val(),
            Website: $("#website").val()
        },
        LeadActivity: {
            //LeadActivityId: $("#leadActivityId").val(),
            LeadId: $("#leadId").val(),
            LeadStatus: $("#leadStatus").val(),
            MeetingMode: $("#meetingMode").val(),
            MeetingDate: $("#meetingDate").val(),
            ResponseDesc: $("#responseDesc").val(),
            NextAppointmentDate: $("#nextAppointmentDate").val(),
            IsReminderSet: $("#reminder").is(":checked"),
            ReminderDate: $("#reminderDateTime").val(),
            RemarkSlf: $("#remark").val(),

        }

    }
    return leadData;
}

// Function to assign values to form controls 
function setLeadActivity(lead) {
    $("#leadId").val(lead.leadId);
    $("#docNo").val(lead.docNo);
    $("#docDate").val(lead.docDate);
    $("#leadStatus").val(lead.leadStatus);
    $("#leadSource").val(lead.leadSource);
    $("#leadOwner").val(lead.leadOwner);
    $("#leadPriority").val(lead.leadPriority);
    $("#leadOwnerContactNo").val(lead.leadOwnerContactNo);
    $("#leadTitle").val(lead.leadTitle);
    $("#leadCategory").val(lead.leadCategory);
    $("#leadDesc").val(lead.leadDesc);
    $("#specialRequirement").val(lead.specialRequirement);
    $("#potentialDealValue").val(lead.potentialDealValue);
    $("#probabilityofConversion").val(lead.probabilityOfConversion);
    $("#closureForecast").val(lead.closureForecast);
    $("#companyName").val(lead.companyName);
    $("#businessSector").val(lead.businessSector);
    $("#contactPerson").val(lead.contactPerson);
    $("#contactNo").val(lead.contactNo);
    $("#designation").val(lead.designation);
    $("#email").val(lead.email);
    $("#address").val(lead.address);
    $("#website").val(lead.website);
}
// set Lead Activity Details
function setLeadActivityDetails(activity) {
    $("#leadActivityId").val(activity.leadActivityId);
    //$("#leadId").val(activity.leadId);
    $("#leadStatus").val(activity.leadStatus);
    $("#meetingMode").val(activity.meetingMode);
    $("#meetingDate").val(activity.meetingDate);
    $("#responseDesc").val(activity.responseDesc);
    $("#nextAppointmentDate").val(activity.nextAppointmentDate);
    if (activity.isReminderSet) {
        $("#reminder").prop('checked', true);
    }
    else {
        $("#reminder").prop('checked',false)
    }
    $("#reminder").val(activity.isReminderSet);
    $("#reminderDateTime").val(activity.reminderDate);
    $("#remark").val(activity.remarkSlf);
}



