using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;

namespace trakomatic.webapi.Controllers
{
	[RoutePrefix("Api/Livestats")]
    public class LivestatsController : ApiController
    {
        [GET("")]
        public IEnumerable<string> GetAll()
        {
            return new string[] { "value1", "value2" };
        }

        [GET("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [POST("")]
        public void Post([FromBody]string value)
        {
        }

        [PUT("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [DELETE("{id}")]
        public void Delete(int id)
        {
        }
    }
}
