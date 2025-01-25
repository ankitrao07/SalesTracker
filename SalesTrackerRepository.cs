using Microsoft.Data.SqlClient;
using SalesTracker.Models;
using SalesTracker.Models.DTO;
using System.Data;
using System.Reflection;
using static NuGet.Client.ManagedCodeConventions;

namespace SalesTracker
{
    public class SalesTrackerRepository
    {
        private readonly string _connectionString;
        public SalesTrackerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<List<vwLeadDetail>> GetTotalLeads()
        {
            var vwLeadDetail = new List<vwLeadDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetTotalLeads", connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lead = new Lead
                        {
                            TrId = (int)reader["TrId"],
                            Catg = reader["Catg"].ToString(),
                            //CompName = reader["CompName"].ToString(),
                            KeyPerson = reader["KeyPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),

                        };
                        var leadDetail = new LeadDetail
                        {
                            TrDetId = Convert.ToInt32(reader["TrDetId"]),
                            LeadStatus = Convert.ToString(reader["LeadStatus"]),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToShortDateString(),
                            RemDate = Convert.ToDateTime(reader["RemDate"]).ToShortDateString()
                        };

                        vwLeadDetail.Add(new vwLeadDetail
                        {
                            objLead = lead,
                            objLeadDetail = leadDetail
                        });

                    }
                }
            }
            return vwLeadDetail;
        }

        public async Task<List<LeadCompositeDTO>> GetTotalLeadsNew()
        {
            var leads = new List<LeadCompositeDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetTotalLeadsNew", connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lead = new LeadDTO
                        {
                            LeadId = (int)reader["LeadId"],
                            LeadNumber = reader["LeadNumber"].ToString(),
                            LeadCategory = reader["LeadCategory"].ToString(),
                            //CompName = reader["CompName"].ToString(),
                            ContactPerson = reader["ContactPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),

                        };
                        var leadActivity = new LeadActivityDTO
                        {
                            LeadActivityId = Convert.ToInt32(reader["LeadActivityId"]),
                            LeadStatus = Convert.ToString(reader["LeadStatus"]),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToShortDateString(),
                            MeetingDate = Convert.ToDateTime(reader["MeetingDate"]).ToShortDateString()
                        };

                        leads.Add(new LeadCompositeDTO
                        {
                            Lead = lead,
                            LeadActivity = leadActivity
                        });

                    }
                }
            }
            return leads;
        }
        public async Task<List<LeadCompositeDTO>> GetLeadbySelectedKPI(string selectedKPI)
        {
            var leads = new List<LeadCompositeDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetLeadsBySelectedKPI", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SelectedKPI", selectedKPI);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lead = new LeadDTO
                        {
                            LeadId = (int)reader["LeadId"],
                            LeadNumber = reader["LeadNumber"].ToString(),
                            LeadCategory = reader["LeadCategory"].ToString(),
                            //CompName = reader["CompName"].ToString(),
                            ContactPerson = reader["ContactPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),
                            IsOverdue =Convert.ToBoolean(reader["IsOverdue"])

                        };
                        var leadActivity = new LeadActivityDTO
                        {
                            LeadActivityId = Convert.ToInt32(reader["LeadActivityId"]),
                            LeadStatus = Convert.ToString(reader["LeadStatus"]),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToShortDateString(),
                            MeetingDate = Convert.ToDateTime(reader["MeetingDate"]).ToShortDateString()
                        };

                        leads.Add(new LeadCompositeDTO
                        {
                            Lead = lead,
                            LeadActivity = leadActivity
                        });

                    }
                }
            }
            return leads;
        }


        public int AddLead(Lead lead, LeadDetail leadDetail)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_AddLeads", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var parameterTracker = new Dictionary<string, object>();
                AddParametersFromObject(command, lead,parameterTracker);
                AddParametersFromObject(command, leadDetail, parameterTracker);

                command.Parameters.AddWithValue("@AddBy", "Ankit");
                command.Parameters.AddWithValue("@UpdBy", "Ankit");

                connection.Open();
                int trId = (int)command.ExecuteScalar();
                return trId;
            }
        }

        public object AddLeadNew(LeadNew lead, LeadActivity leadActivity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_AddLeadNew", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var parameterTracker = new Dictionary<string, object>();
                AddParametersFromObject(command, lead,parameterTracker);
                AddParametersFromObject(command, leadActivity, parameterTracker);

                //command.Parameters.AddWithValue("@AddedBy", "Ankit");
                //command.Parameters.AddWithValue("@UpdatedBy", "Ankit");

                connection.Open();
                var trId =Convert.ToString(command.ExecuteScalar());
                var strResult = trId.Split("#", StringSplitOptions.None);
                return strResult;
            }
        }

        private void AddParametersFromObject(SqlCommand command, object obj, Dictionary<string, object> parameterTracker)
        {
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                List<string> dateTypeProperties = new List<string>()
                {
                    "DocDate",
                    "ClosureForecast",
                    "NextReminderDate",
                    "LeadDate",
                    "ReminderDate",
                    "MeetingDate",
                    "FollowupDate",
                    "NextAppointmentDate"
                };
                List<string> avoidProperties = new List<string>()
                {
                    "IsOverdue"
                };
               
                    
                
                string parameterName = "@" + property.Name;
                object value = property.GetValue(obj);
                

                if (!parameterTracker.ContainsKey(parameterName) && !avoidProperties.Contains(property.Name))
                {
                    if (value == null)
                    {
                        value = DBNull.Value;
                    }
                    else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        DateTime dateValue;
                        if (!DateTime.TryParse(value.ToString(), out dateValue))
                        {
                            throw new ArgumentException($"Invalid date format for property {property.Name}");
                        }
                        value = dateValue;
                    }
                    else if (dateTypeProperties.Contains(property.Name))
                    {
                       
                        value = ParseDateTime(Convert.ToString(value));
                    }

                    parameterTracker[parameterName] = value;
                    command.Parameters.AddWithValue(parameterName, value);
                }
            }
        }

        //private void AddParametersFromObject(SqlCommand command, object obj, Dictionary<string, object> parameterTracker)
        //{
        //    foreach (PropertyInfo property in obj.GetType().GetProperties()) 
        //    { string parameterName = "@" + property.Name; 
        //        object value = property.GetValue(obj); 
        //        if (!parameterTracker.ContainsKey(parameterName)) 
        //        { if (value == null) { value = DBNull.Value; } 
        //            parameterTracker[parameterName] = value; 
        //            command.Parameters.AddWithValue(parameterName, value); 
        //        } 
        //    }
        //}

        //public int AddLead(Lead lead, LeadDetail leadDetail)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        var command = new SqlCommand("sp_AddLeads", connection);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.AddWithValue("@TrId", lead.TrId);
        //        command.Parameters.AddWithValue("@Catg", lead.Catg);
        //        command.Parameters.AddWithValue("@CompName", lead.CompName);
        //        command.Parameters.AddWithValue("@KeyPerson", lead.KeyPerson);
        //        command.Parameters.AddWithValue("@ContactNo", lead.ContactNo);
        //        command.Parameters.AddWithValue("@Location", string.IsNullOrEmpty(lead.Location) ? "Delhi" : lead.Location);
        //        command.Parameters.AddWithValue("@OEmail", lead.OEmail);
        //        command.Parameters.AddWithValue("@PEmail", !string.IsNullOrEmpty(lead.PEmail) ? lead.PEmail : "ankit@gmail.com");
        //        command.Parameters.AddWithValue("@Remark", lead.Remark);
        //        command.Parameters.AddWithValue("@Add1", lead.Add1);
        //        command.Parameters.AddWithValue("@Add2", lead.Add2);
        //        command.Parameters.AddWithValue("@Add3", lead.Add3);
        //        command.Parameters.AddWithValue("@City", lead.City);
        //        command.Parameters.AddWithValue("@DistName", lead.DistName);
        //        command.Parameters.AddWithValue("@StName", lead.StName);
        //        command.Parameters.AddWithValue("@LeadStatus", leadDetail.LeadStatus);
        //        command.Parameters.AddWithValue("@LeadDate",Convert.ToDateTime(leadDetail.AddDate));
        //        command.Parameters.AddWithValue("@RemarkBP", leadDetail.RemarkBP);
        //        command.Parameters.AddWithValue("@RemarkSlf", leadDetail.RemarkSlf);
        //        command.Parameters.AddWithValue("@RemarkSpl", leadDetail.RemarkSpl);
        //        command.Parameters.AddWithValue("@RemDate",Convert.ToDateTime(leadDetail.RemDate));
        //        command.Parameters.AddWithValue("@AddBy", "Ankit");
        //        command.Parameters.AddWithValue("@AddDate",Convert.ToDateTime(lead.AddDate));
        //        command.Parameters.AddWithValue("@UpdBy", "Ankit");
        //        connection.Open();
        //        //command.ExecuteNonQuery();
        //        // Get the newly inserted ID
        //        int trId = (int) command.ExecuteScalar();
        //        return trId;
        //    }
        //}

        public async Task<vwLeadActivities> GetLeadDetailByIdAsync(int leadId)
        {
            vwLeadActivities vwLeadActivities = new vwLeadActivities();
            vwLeadActivities.vwLeadDetail = new vwLeadDetail();
            vwLeadActivities.RecentActivities = new List<LeadDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetLeadByID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TrId", leadId);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {


                        var objLead = new Lead
                        {
                            TrId = (int)reader["TrId"],
                            Catg = reader["Catg"].ToString(),
                            KeyPerson = reader["KeyPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),
                            CompName = reader["CompName"].ToString(),
                            City = reader["City"].ToString(),
                            OEmail = reader["OEmail"].ToString(),
                            Remark = reader["Remark"].ToString(),

                        };
                        var objLeadDetail = new LeadDetail
                        {
                            LeadStatus = reader["LeadStatus"].ToString(),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToString("d"),
                            RemarkBP = reader["RemarkBP"].ToString(),
                            RemarkSlf = reader["Remark"].ToString(),
                            RemarkSpl = reader["RemarkSpl"].ToString(),
                            RemDate = Convert.ToDateTime(reader["RemDate"]).ToString("d")
                        };
                        vwLeadActivities.vwLeadDetail = new vwLeadDetail
                        {
                            objLead = objLead,
                            objLeadDetail = objLeadDetail
                        };

                    }
                    if (await reader.NextResultAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var leadDetail = new LeadDetail
                            {
                                TrDetId = Convert.ToInt32(reader["TrDetId"]),
                                LeadStatus = Convert.ToString(reader["LeadStatus"]),
                                //LeadDate = Convert.ToDateTime(reader["LeadDate"]),
                                //RemDate = Convert.ToDateTime(reader["RemDate"]),
                                RemarkSlf = reader["RemarkSlf"].ToString(),
                                UpdBy = reader["UpdBy"].ToString(),
                                UpdDate = Convert.ToDateTime(reader["UpdDate"]).ToString("g")

                            };
                            vwLeadActivities.RecentActivities.Add(leadDetail);
                        }
                    }
                }

            }
            return vwLeadActivities;
        }

        public async Task<vwLead_RecentActivities> GetLeadDetailByIdAsyncNew(int leadId)
        {
            vwLead_RecentActivities leads = new vwLead_RecentActivities();
            leads.RecentActivities = new List<LeadActivityDTO>();
            
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("sp_GetLeadByIDNew", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@LeadId", leadId);
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {


                            var leadDTO = new LeadDTO
                            {
                                LeadId = (int)reader["LeadId"],
                                LeadNumber = reader["LeadNumber"].ToString(),
                                DocNo = reader["DocNo"].ToString(),
                                DocDate = Convert.ToDateTime(reader["DocDate"]).ToString("d"),
                                EnteredBy = reader["AddedBy"].ToString(),
                                ContactPerson = reader["ContactPerson"].ToString(),
                                ContactNo = reader["ContactNo"].ToString(),
                                Email = reader["Email"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                City = reader["City"].ToString(),
                                LeadSource = reader["LeadSource"].ToString(),
                                LeadOwner = reader["LeadOwner"].ToString(),
                                LeadPriority = reader["LeadPriority"].ToString(),
                                LeadOwnerContactNo = reader["LeadOwnerContactNo"].ToString(),
                                LeadTitle = reader["LeadTitle"].ToString(),
                                LeadCategory = reader["LeadCategory"].ToString(),
                                LeadDesc = reader["LeadDesc"].ToString(),
                                SpecialRequirement = reader["SpecialRequirement"].ToString(),
                                PotentialDealValue = reader["PotentialDealValue"].ToString(),
                                ProbabilityOfConversion = reader["ProbabilityOfConversion"].ToString(),
                                ClosureForecast = reader["ClosureForecast"].ToString(),
                                BusinessSector = reader["BusinessSector"].ToString(),
                                Designation = reader["Designation"].ToString(),
                                Address = reader["Address"].ToString(),
                                Website = reader["Website"].ToString()
                            };
                            var leadActivity = new LeadActivityDTO
                            {
                                MeetingMode = reader["MeetingMode"].ToString(),
                                MeetingDate = Convert.ToDateTime(reader["MeetingDate"]).ToString("dd/MM/yyyy"),
                                ResponseDesc = reader["ResponseDesc"].ToString(),
                                NextAppointmentDate = Convert.ToDateTime(reader["NextAppointmentDate"]).ToString("dd/MM/yyyy"),
                                IsReminderSet = Convert.ToBoolean(reader["IsReminderSet"]),
                                ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? string.Empty : Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("ReminderDate"))).ToString("dd/MM/yyyy"),
                                LeadStatus = reader["LeadStatus"].ToString(),
                                LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToString("dd/MM/yyyy"),
                                //RemarkBP = reader["RemarkBP"].ToString(),
                                RemarkSlf = reader["Remark"].ToString(),
                                //RemarkSpl = reader["RemarkSpl"].ToString(),
                            };
                            var leadCompositeDTO = new LeadCompositeDTO
                            {
                                Lead = leadDTO,
                                LeadActivity = leadActivity
                            };
                            leads.LeadComposite = leadCompositeDTO;

                        }
                        if (await reader.NextResultAsync())
                        {
                            List<LeadActivityDTO> recentActivitiesList = new List<LeadActivityDTO>();
                            while (await reader.ReadAsync())
                            {
                                var recentActivities = new LeadActivityDTO
                                {
                                    LeadActivityId = Convert.ToInt32(reader["LeadActivityId"]),
                                    LeadStatus = Convert.ToString(reader["LeadStatus"]),
                                    ResponseDesc = reader["ResponseDesc"].ToString(),
                                    RemarkSlf = reader["Remark"].ToString(),
                                    UpdatedBy = reader["UpdatedBy"].ToString(),
                                    UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"]).ToString("g")

                                };
                                leads.RecentActivities.Add(recentActivities);
                            }
                            
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
           
            return leads;
        }

        public LeadCounts GetLeadCounts()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetLeadCountsNew", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                LeadCounts leadCounts = new LeadCounts();

                if (reader.Read())
                {
                    leadCounts.TotalLeads = reader.GetInt32(0);
                }
                if (reader.NextResult() && reader.Read())
                {
                    leadCounts.ActiveLeads = reader.GetInt32(0);
                }
                if (reader.NextResult() && reader.Read())
                {
                    leadCounts.ConvertedLeads = reader.GetInt32(0);
                }
                if (reader.NextResult() && reader.Read())
                {
                    leadCounts.TodaysActionableLeads = reader.GetInt32(0);
                }

                return leadCounts;
            }
        }

        public async Task<List<vwLeadDetail>> GetActivelLeads()
        {
            var vwLeadDetail = new List<vwLeadDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetActiveLeads", connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lead = new Lead
                        {
                            TrId = (int)reader["TrId"],
                            Catg = reader["Catg"].ToString(),
                            //CompName = reader["CompName"].ToString(),
                            KeyPerson = reader["KeyPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),

                        };
                        var leadDetail = new LeadDetail
                        {
                            TrDetId = Convert.ToInt32(reader["TrDetId"]),
                            LeadStatus = Convert.ToString(reader["LeadStatus"]),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToShortDateString(),
                            RemDate = Convert.ToDateTime(reader["RemDate"]).ToShortDateString()
                        };

                        vwLeadDetail.Add(new vwLeadDetail
                        {
                            objLead = lead,
                            objLeadDetail = leadDetail
                        });

                    }
                }
            }
            return vwLeadDetail;
        }

        public async Task<List<LeadCompositeDTO>> GetActivelLeadsNew()
        {
            var leadCompositeDTO = new List<LeadCompositeDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetActiveLeads", connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lead = new LeadDTO
                        {
                            LeadId = (int)reader["LeadId"],
                            LeadNumber = reader["LeadNumber"].ToString(),
                            LeadCategory = reader["LeadCategory"].ToString(),
                            //CompName = reader["CompName"].ToString(),
                            ContactPerson = reader["ContactPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),

                        };
                        var leadActivity = new LeadActivityDTO
                        {
                            LeadActivityId = Convert.ToInt32(reader["LeadActivityId"]),
                            LeadStatus = Convert.ToString(reader["LeadStatus"]),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToShortDateString(),
                            ReminderDate = Convert.ToDateTime(reader["ReminderDate"]).ToShortDateString()
                        };

                        leadCompositeDTO.Add(new LeadCompositeDTO
                        {
                            Lead = lead,
                            LeadActivity = leadActivity
                        });

                    }
                }
            }
            return leadCompositeDTO;
        }

        public async Task<List<LeadCompositeDTO>> GetConvertedLeads()
        {
            var leadCompositeDTO = new List<LeadCompositeDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetConvertedLeads", connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lead = new LeadDTO
                        {
                            LeadId = (int)reader["LeadId"],
                            LeadNumber = reader["LeadNumber"].ToString(),
                            LeadCategory = reader["LeadCategory"].ToString(),
                            //CompName = reader["CompName"].ToString(),
                            ContactPerson = reader["ContactPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),

                        };
                        var leadActivity = new LeadActivityDTO
                        {
                            LeadActivityId = Convert.ToInt32(reader["LeadActivityId"]),
                            LeadStatus = Convert.ToString(reader["LeadStatus"]),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToShortDateString(),
                            ReminderDate = Convert.ToDateTime(reader["ReminderDate"]).ToShortDateString()
                        };

                        leadCompositeDTO.Add(new LeadCompositeDTO
                        {
                            Lead = lead,
                            LeadActivity = leadActivity
                        });

                    }
                }
            }
            return leadCompositeDTO;
        }

        public async Task<List<vwLeadDetail>> GetActionablelLeads()
        {
            var vwLeadDetail = new List<vwLeadDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetActionableLeads", connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lead = new Lead
                        {
                            TrId = (int)reader["TrId"],
                            Catg = reader["Catg"].ToString(),
                            //CompName = reader["CompName"].ToString(),
                            KeyPerson = reader["KeyPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),

                        };
                        var leadDetail = new LeadDetail
                        {
                            TrDetId = Convert.ToInt32(reader["TrDetId"]),
                            LeadStatus = Convert.ToString(reader["LeadStatus"]),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToShortDateString(),
                            RemDate = Convert.ToDateTime(reader["RemDate"]).ToShortDateString()
                        };

                        vwLeadDetail.Add(new vwLeadDetail
                        {
                            objLead = lead,
                            objLeadDetail = leadDetail
                        });

                    }
                }
            }
            return vwLeadDetail;
        }

        public async Task<List<LeadCompositeDTO>> GetActionablelLeadsNew()
        {
            var leadCompositeDTO = new List<LeadCompositeDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetActionableLeads", connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var lead = new LeadDTO
                        {
                            LeadId = (int)reader["LeadId"],
                            LeadNumber = reader["LeadNumber"].ToString(),
                            LeadCategory = reader["LeadCategory"].ToString(),
                            //CompName = reader["CompName"].ToString(),
                            ContactPerson = reader["ContactPerson"].ToString(),
                            ContactNo = reader["ContactNo"].ToString(),

                        };
                        var leadActivity = new LeadActivityDTO
                        {
                            LeadActivityId = Convert.ToInt32(reader["LeadActivityId"]),
                            LeadStatus = Convert.ToString(reader["LeadStatus"]),
                            LeadDate = Convert.ToDateTime(reader["LeadDate"]).ToShortDateString(),
                            ReminderDate = Convert.ToDateTime(reader["ReminderDate"]).ToShortDateString()
                        };

                        leadCompositeDTO.Add(new LeadCompositeDTO
                        {
                            Lead = lead,
                            LeadActivity = leadActivity
                        });

                    }
                }
            }
            return leadCompositeDTO;
        }
        private DateTime? ParseDateTime(string dateString)
        {
            DateTime result;
            // Include "dd-MM-yyyy" in the formats array
            string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy" };
            if (DateTime.TryParseExact(dateString, formats, null, System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

    }

}

