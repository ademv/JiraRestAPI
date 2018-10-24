using JiraRestAPI.Models.IMS;
using JiraRestAPI.Models.Users;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace JiraRestAPI.Services
{
    public class DataService
    {

        public static string ConnectionString = ConfigurationManager.ConnectionStrings["FlexManager"].ConnectionString;

        public List<UserModel> GetLocalUsers(Filter model)
        {
            return GetUsers("logical_Jira_ReadUsers", model.pagenumber,model.pagesize);
        }



        public List<UserModel> GetUnConnectedLocalUsers(Filter model)
        {
            return GetUsers("logical_Jira_ReadUsersUnConnected",model.pagenumber,model.pagesize);

        }

        public List<UserModel> GetConnectedUsers(Filter model)
        {
            return GetUsers("logical_Jira_ReadConnectedUsers", model.pagenumber, model.pagesize);
        }


        private List<UserModel> GetUsers(string Procedurename,int pagenumber,int pagesize)
        {
            List<UserModel> list = new List<UserModel>();



            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Procedurename, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                 cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value=pagenumber;
                cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value=pagesize;
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var u = new UserModel
                    {
                        Id = reader["Id"].ToString(),
                        accountId = reader["JIRAAccountId"].ToString(),
                        displayName = reader["JIRAdisplayName"].ToString(),
                        emailAddress = reader["JIRAemailAddress"].ToString(),
                        key = reader["JiraKey"].ToString(),
                        name = reader["JIRAName"].ToString(),
                        self = reader["JIRASelf"].ToString(),
                        OrganizationID = reader["OrganizationID"].ToString(),
                        OrganizationName=reader["OrganizationName"].ToString()
                        
                    };

                    list.Add(u);
                }

                return list;


            }

        }


        public bool UpdateUserOrganization(IList<string> keys, string organizationId)
        {

            bool ok = true;
            foreach (var item in keys)
            {
                SqlCommand cmd = new SqlCommand("logical_Jira_UpdateUserOrganization");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@key", item);
                cmd.Parameters.AddWithValue("@organizationId", organizationId);
                ok = ok && ExecuteNonQuery(cmd);
            }


            return ok;

        }

        public  bool UpdateJiraOrganizationId(string ImsId,string JiraName,string JiraId)
        {
            SqlCommand cmd = new SqlCommand("logical_Jira_UpdateJiraOrganizationId");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ImsId", ImsId);
            cmd.Parameters.AddWithValue("@JiraName", JiraName);
            cmd.Parameters.AddWithValue("@JiraId", JiraId);
             return ExecuteNonQuery(cmd);
           
        }


        public bool UpdateUserSync(IList<string> keys,int synctype,bool IncludeIssue,string assignedusser,string nextorg)
        {
            bool ok= true;
            SqlCommand cmd = new SqlCommand("logical_Jira_UpdateUserSyncType");
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var item in keys)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@key", item);
                cmd.Parameters.AddWithValue("@SyncType", synctype);
                cmd.Parameters.AddWithValue("@IncludeIssue", IncludeIssue);
                cmd.Parameters.AddWithValue("@AssignUser", assignedusser);
                cmd.Parameters.AddWithValue("@NextOrganizationId", nextorg);
                ok=ok&& ExecuteNonQuery(cmd);
            }
            return ok;
        }

        public bool DeleteUser(string key)
        {
            SqlCommand command = new SqlCommand("logical_Jira_DeleteUser");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@key", key);

            return ExecuteNonQuery(command);
        }

        public bool DeleteOrganization(string key)
        {
            SqlCommand cmd = new SqlCommand("logical_Jira_DeleteOrganization");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrganizationId", key);
            return ExecuteNonQuery(cmd);
        }
        public object GetTotalALLUsers()
        {

            SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.JIRA_USERS");
            return ExecuteScalar(cmd);
        }

        public object GetTotalUnConnectedUsers()
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.JIRA_USERS WHERE OrganizationID=-1");
            return ExecuteScalar(cmd);

        }

        public object GetTotalConnectedUsers()
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.JIRA_USERS WHERE OrganizationID<>-1");
            return ExecuteScalar(cmd);

        }
        public static List<CompanyIssueComment> GetCompanyComments()
        {
            List<CompanyIssueComment> companycomm = new List<CompanyIssueComment>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand("logical_Jira_GetComments", conn);


                    var reader = comm.ExecuteReader();

                    while (reader.Read())
                    {

                        var o = new CompanyIssueComment
                        {
                            Company = reader["JIRAOrganizationName"].ToString(),
                            Comment = reader["JiraComment"].ToString(),
                            JQL = reader["JQL"].ToString(),
                            OrganizationId = reader["Id"].ToString(),
                            customField_10075 = Convert.ToInt32(reader["customField_10075"].ToString())
                        };
                        companycomm.Add(o);
                    }


                    return companycomm;
                }
            }
            catch (Exception e)
            {

                return companycomm;
            }


        }

        private bool ExecuteNonQuery(SqlCommand cmd)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    cmd.Connection = conn;

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }

            catch (Exception ex)
            {
                return false;
            }
        }

    
        private object ExecuteScalar(SqlCommand cmd)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    cmd.Connection = conn;

                    return cmd.ExecuteScalar();

                    
                }
            }

            catch (Exception ex)
            {
                return null;
            }
        }

       

    }


}