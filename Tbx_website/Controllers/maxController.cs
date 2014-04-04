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
    public class maxController : ParentController
    {
        [GET("session/{id}/max/{siteid}")]
        public string getmaxval(String id, int siteid)
        {
            string uid = "";
            string gid = "";
            string query = "";
            String maxval = "";
            if (id == null)
            {
                return "Invalid";
            }

            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                // return new Dictionary<string, string>();
                return "";
            }
            myCommand = new SqlCommand("select maxval from oline_devices where siteID = " + siteid, conn);

            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        maxval = reader["maxval"].ToString();

                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return maxval;
        }
    }
}
