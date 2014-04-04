using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;

namespace MvcApplication13.Controllers
{
	[RoutePrefix("Api/First")]
    public class FirstController : ApiController
    {

        [GET("")]
        public IEnumerable<string> GetAll()
        {
            return new string[] { "value1", "value2" };
        }

        [GET("{sessionid}/{id}")]
        public string Get(string sessionid, int id)
        {
            return sessionid + " " + id ;
        }

        //[POST("")]
        //public void Post([FromBody]string value)
        //{
        //}

        //[PUT("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //[DELETE("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
