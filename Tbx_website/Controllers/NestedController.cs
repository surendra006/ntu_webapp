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
    [RoutePrefix("Api/first/{sessionid}/{id}/nested")]
    public class NestedController : ApiController
    {

        //[GET("")]
        //public IEnumerable<string> GetAll()
        //{
        //    return new string[] { "value1", "nested" };
        //}

        [GET("{resolution}")]
        public string getentry(String sessionid, int id, String resolution)
        {
            return sessionid + " " + id + " " + resolution;
        }
        

        //[GET("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

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
