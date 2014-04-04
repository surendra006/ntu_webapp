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
	[RoutePrefix("Api/sample")]
    public class sampleController : ApiController
    {
        [GET("")]
        public IEnumerable<string> GetAll()
        {
            return new string[] { "value1", "value2" };
        }

        [GET("{id}")]
        public int Get(int id)
        {
            return id;
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
