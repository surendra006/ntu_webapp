using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using AttributeRouting.Web.Http;
//using AttributeRoutingSample.Models;
using System.Configuration;

namespace MvcApplication13.Controllers
{
    public class SessionController : ParentController
    {
        [GET("session/{id}")]
        // GET api/<controller>
        public IEnumerable<string> getDevice(string id)
        {
            string uid = "";
            string gid = "";
            if (id == null)
            {
                return new List<String>();//"Invalid Session";
            }

            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
                return new List<String>();//"Session Invalid";
            }   
            string siteOut = "";
            var siteList = new List<String>();
            myCommand = new SqlCommand("select * from oline_devices where groupid = " + gid, conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        siteOut += reader["siteid"] + ",";
                        siteList.Add(reader["siteid"].ToString());
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //return siteOut;
            return siteList;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}