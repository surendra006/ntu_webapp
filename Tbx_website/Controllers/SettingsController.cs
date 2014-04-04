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
    public class SettingsController : ParentController
    {

        // GET api/<controller>/5
        [GET("session/{id}/settings/{siteid}/{maxval}")]
        public string getUpdatemaxval(String id,int siteid, String maxval)
        {
            string uid = "";
            string gid = "";
            string query = "";
            if (id == null)
            {
                return "Invalid";
            }

            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                // return new Dictionary<string, string>();
                return "";
            }
            query = "update oline_devices set maxval='" + maxval + "' where siteID='" + siteid + "'";
            myCommand = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                myCommand.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
            }
            return maxval;
        }
    }
}