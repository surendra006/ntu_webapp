using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting.Web.Http;
//using AttributeRoutingSample.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MvcApplication13.Controllers
{
    public class StatusController : ParentController
    {

        // GET api/<controller>/5
        [GET("session/{id}/{locations}/status")]

        public string getStatus(String id, String locations)
        {
            string uid = "";
            string gid = "";
            if (id == null)
            {
                return "Invalid Session";
            }

            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                return "Session Invalid";
            }
            string siteOut = "";

            string status = "[";
            string[] ids = locations.Split('-');
            System.DateTime currTime = DateTime.UtcNow;
            System.DateTime deviceTime = default(System.DateTime);
            foreach (string input in ids)
            {
                myCommand = new SqlCommand("select lastUpdateTime from oline_devices where siteID=" + input, conn);
                try
                {
                    conn.Open();
                    reader = myCommand.ExecuteReader();
                    if ((reader.HasRows))
                    {
                        while (reader.Read())
                        {
                            deviceTime = (System.DateTime)reader[0];
                            if ((deviceTime - currTime).TotalMinutes > 60)
                                status += "0,";
                            else if (reader[0].ToString() == "")
                            {
                                status += "0,";
                            }
                            else
                            {
                                status += "1,";
                            }
                        }
                    }
                    reader.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                }
            }
            status =  status.Substring(0, status.Length - 1) + "]";
            return status;

        }
    }
}