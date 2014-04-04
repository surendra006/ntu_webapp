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
    public class newController : AccountController
    	{
		// GET api/<controller>/5
        [GET("session/new/{name}")]
		public string getCustomerName(String name)
		{
            return name;
		}
     }
}