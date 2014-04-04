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
	public class AccountController : ParentController
	{

		// GET api/<controller>/5
		[GET("session/{id}/account/{defaultview}")]

		public string getSetdefaultview(String id, String defaultview)
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
			query = "update account set defaultView='" + defaultview + "' where groupID='" + gid + "'";
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
			return defaultview;
		}

		[GET("session/{id}/account")]
		public string getdefaultview(String id)
		{
			string uid = "";
			string gid = "";
			string query = "";
			String defaultview = "";
			bool reply = false;
			if (id == null)
			{
				return "Invalid";
			}

			if (access.Authenticate(id, ref gid, ref uid) == false)
			{
				// return new Dictionary<string, string>();
				return "";
			}
			query = "select defaultView from account where groupID='" + gid + "'";
			myCommand = new SqlCommand(query, conn);
			try
			{
				conn.Open();
				reader = myCommand.ExecuteReader();
				if ((reader.HasRows))
				{
					reply = true;
					while (reader.Read())
					{
						defaultview = reader["defaultview"] + "";

					}
				}
			}
			catch (Exception err)
			{
				// Handle an error by displaying the information.
			}
			conn.Close();

			return defaultview;
		}

        [GET("session/{id}/account/starttime/{siteid}/{starttime}")]

        public string getsetStarttime(string id,string siteid, string starttime)
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
            query = "update oline_devices set startTime='" + starttime + "' where siteID = " + siteid;
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
            return starttime;
        }

        [GET("session/{id}/account/closetime/{siteid}/{closetime}")]

        public string getsetclosetime(string id, string siteid, string closetime)
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
            query = "update oline_devices set closetime='" + closetime + "' where siteID='" + siteid + "'";
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
            return closetime;
        }

        [GET("session/{id}/account/timesetting/getstarttime/{siteid}")]
        public string getsetStarttime(string id,string siteid)
        {
            string uid = "";
            string gid = "";
            string query = "";
            string starttime="";    
            if (id == null)
            {
                return "Invalid";
            }

            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                // return new Dictionary<string, string>();
                return "";
            }
            query = "select starttime from oline_devices where siteID='" + siteid + "'";
            myCommand = new SqlCommand(query, conn);
			try
			{
				conn.Open();
				reader = myCommand.ExecuteReader();
				if ((reader.HasRows))
				{
					while (reader.Read())
					{
                        starttime = reader["starttime"] + "";

					}
				}
			}
			catch (Exception err)
			{
				// Handle an error by displaying the information.
			}
			conn.Close();
            return starttime;
        }

        [GET("session/{id}/account/timesetting/getclosetime/{siteid}")]
        public string getsetclosetime(String id,string siteid)
        {
            string uid = "";
            string gid = "";
            string query = "";
            string closetime = "";
            if (id == null)
            {
                return "Invalid";
            }

            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                // return new Dictionary<string, string>();
                return "";
            }
            query = "select closetime from oline_devices where siteID='" + siteid + "'";
            myCommand = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        closetime = reader["closetime"] + "";

                    }
                }
            }
            catch (Exception err)
            {
                // Handle an error by displaying the information.
            }
            conn.Close();
            return closetime;
        }
	}
}