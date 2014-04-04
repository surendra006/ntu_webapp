using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Data.SqlClient;
using System.Configuration;
using AttributeRouting.Web.Http;

namespace MvcApplication13.Controllers
{
    public class DeviceController : ParentController
    {

        public static string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
        public SqlCommand myCommand = default(SqlCommand);
        public SqlConnection conn = new SqlConnection(connectionString);
        public SqlDataReader reader = default(SqlDataReader);
        public auth access = new auth();
        SqlCommand nestedCommand = default(SqlCommand);
        SqlCommand myCommand2 = default(SqlCommand);
        SqlConnection nestedConn = new SqlConnection(connectionString);
        SqlDataReader nestedReader = default(SqlDataReader);

        [GET("session/{id}/device/isgroup/{sitename}")]
        public string getisgroup(String id, String sitename)
        {
            //string query = "select DATEDIFF(SECOND, 25567, SWITCHOFFSET(TODATETIMEOFFSET(dateInfo, '+00:00'), '+08:00')), entryCount from " + tablePrefix + deviceId.Key + tableSuffix + " where dateInfo > CONVERT(datetimeoffset, '" + startdt + "T00:08:00+08:00', 127) and dateInfo <= CONVERT(datetimeoffset, '" + enddt + "T00:08:00+08:00', 127)";
            string uid = "";
            string gid = "";
            if (id == null)
            {
                //return new Dictionary<string, string>();
                return "";
            }
            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                // return new Dictionary<string, string>();
                return "";
            }

            string output = "";
            var deviceid = "";
            var isgroup = "";

            String query = "";
            query = "select isgroup from oline_devices where sitename = '" + sitename + "'";

            myCommand = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        isgroup = reader["isgroup"].ToString();
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return isgroup;
        }

        [GET("session/{id}/device/ischild/{sitename}")]
        public string getischild(String id, String sitename)
        {
            //string query = "select DATEDIFF(SECOND, 25567, SWITCHOFFSET(TODATETIMEOFFSET(dateInfo, '+00:00'), '+08:00')), entryCount from " + tablePrefix + deviceId.Key + tableSuffix + " where dateInfo > CONVERT(datetimeoffset, '" + startdt + "T00:08:00+08:00', 127) and dateInfo <= CONVERT(datetimeoffset, '" + enddt + "T00:08:00+08:00', 127)";
            string uid = "";
            string gid = "";
            if (id == null)
            {
                //return new Dictionary<string, string>();
                return "";
            }
            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                // return new Dictionary<string, string>();
                return "";
            }

            string output = "";
            var deviceid = "";
            var ischild = "";

            String query = "";
            query = "select ischild from oline_devices where sitename = '" + sitename + "'";

            myCommand = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        ischild = reader["ischild"].ToString();
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return ischild;
        }
        
        [GET("session/{id}/devicelist")]
        public List<string> GetDeviceIds(String id)
        {
            string uid = "";
            string gid = "";
            if (id == null)
            {
                return new List<string>();
                //return "";
            }
            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                return new List<string>();
                //return "";
            }

            List<string> deviceIds = new List<string>();

            myCommand = new SqlCommand("select sitename, siteID from oline_devices where groupID = " + gid + "and isChild='no'", conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        deviceIds.Add(reader["sitename"].ToString());
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return deviceIds;
        }

        [GET("session/{id}/getsiteid/{sitename}")]
        public string getsiteid(String id, String sitename)
        {
            //string query = "select DATEDIFF(SECOND, 25567, SWITCHOFFSET(TODATETIMEOFFSET(dateInfo, '+00:00'), '+08:00')), entryCount from " + tablePrefix + deviceId.Key + tableSuffix + " where dateInfo > CONVERT(datetimeoffset, '" + startdt + "T00:08:00+08:00', 127) and dateInfo <= CONVERT(datetimeoffset, '" + enddt + "T00:08:00+08:00', 127)";
            string uid = "";
            string gid = "";
            if (id == null)
            {
                //return new Dictionary<string, string>();
                return "";
            }
            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                // return new Dictionary<string, string>();
                return "";
            }

            string output = "";
            var deviceid = "";
            var siteid = "";

            String query = "";
            query = "select siteid from oline_devices where sitename = '" + sitename.Replace("'", "''") + "'";

            myCommand = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                       siteid = reader["siteid"].ToString();
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return siteid;
        }

        [GET("session/{id}/getchildren/{sitename}")]
        public List<string> getchildren(String id, String sitename)
        {
            string uid = "";
            string gid = "";
            if (id == null)
            {
                return new List<string>();
                //return "";
            }
            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                return new List<string>();
                //return "";
            }
            var siteid_group = "";

            String query = "";
            query = "select siteid from oline_devices where sitename = '" + sitename + "'";

            myCommand = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        siteid_group = reader["siteid"].ToString();
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            List<string> siteid_device1 = new List<string>();

            myCommand = new SqlCommand("select siteid_device from oline_device_group_bindings where siteid_group = " + siteid_group, conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        siteid_device1.Add(reader["siteid_device"].ToString());
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            List<string> sitenames_device = new List<string>();

            foreach (var siteid_device in siteid_device1)
            {
                myCommand = new SqlCommand("select sitename from oline_devices where siteID = " + siteid_device, conn);

                try
                {
                    conn.Open();
                    reader = myCommand.ExecuteReader();
                    if ((reader.HasRows))
                    {
                        while (reader.Read())
                        {
                            sitenames_device.Add(reader["sitename"].ToString());
                        }
                    }
                    reader.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return sitenames_device;
            }
        }
    }