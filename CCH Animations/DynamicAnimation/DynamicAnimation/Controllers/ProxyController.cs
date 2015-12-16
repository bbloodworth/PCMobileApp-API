using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Mvc;
using DynamicAnimation.Common;
using DynamicAnimation.Models;

namespace DynamicAnimation.Controllers
{
    public class ProxyController : ApiController
    {
        // GET: api/Proxy
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET: api/Proxy/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Proxy
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Proxy/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Proxy/5
        public void Delete(int id)
        {
        }

        [HttpPost]
        public HttpResponseMessage GetAuthMemberData([FromBody] AuthMemberDataRequest hsRequest)
        {
            var httpClient = new HttpClient();

            string requestUri = string.Format("{0}/v1/PComm/VideoAuth/MemberData/{1}", 
                "APIBaseAddress".GetConfigurationValue(), 
                "HandshakeId".GetConfigurationValue());

            Request.RequestUri = new Uri(requestUri);

            HttpResponseMessage response = httpClient.SendAsync(Request).Result;

            if (!response.IsSuccessStatusCode)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            return response;
        }
    }
}
