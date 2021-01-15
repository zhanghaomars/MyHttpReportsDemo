using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace WebAPI1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly RestClient Client = null;
        private readonly string Token = null;

        public HomeController(ILoggerFactory logFactory)
        {
            _logger = logFactory.CreateLogger<HomeController>();
            if (Client == null)
            {
                Client = new RestClient("http://localhost:5002/");
            }

            if (Token == null)
            {
                var loginRequest = new RestRequest("/api/Authentication/requestToken", Method.POST);
                var json = JsonConvert.SerializeObject(new { username = "admin", password = "123456" });
                loginRequest.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse loginResponse = Client.Execute(loginRequest);
                Token = loginResponse.Content;
            }
        }

        [Route("AllPersons")]
        [HttpGet]
        public ActionResult GetAllPersons()
        {
            try
            {
                Client.AddDefaultHeader("Authorization", Token.Replace("\"", ""));
                var requestGet = new RestRequest("api/Values/AllPersons", Method.GET);
                IRestResponse response = Client.Execute(requestGet);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(response.Content);
                }
                else
                {
                    return Ok(null);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "GetAllPersons错误：" + e.ToString());
                return Ok(e.ToString());
            } 
        }


        [Route("Person")]
        [HttpGet]
        public ActionResult GetPerson()
        {
            try
            {
                Client.AddDefaultHeader("Authorization", Token);
                var requestGet = new RestRequest("api/Values/Person", Method.GET);
                requestGet.AddParameter("Id", 1);
                IRestResponse response = Client.Execute(requestGet);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //var jsondata = JsonSerializer.Deserialize<Person>(response.Content);
                    return Ok(response.Content);
                }
                else
                {
                    return Ok(null);
                }
            }
            catch (Exception e)
            {
                return Ok(e.ToString());
            }
        }

        [Route("AddPerson")]
        [HttpGet]
        public ActionResult AddPerson()
        {
            try
            {
                Person p = new Person()
                {
                    Id = 3,
                    Name = "Test",
                    Createtime = DateTime.Now
                };
                Client.AddDefaultHeader("Authorization", Token);
                var requestPost = new RestRequest("api/Values/AddPerson", Method.POST);
                var json = JsonConvert.SerializeObject(p);
                requestPost.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse responsePost = Client.Execute(requestPost);
                if (responsePost.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(responsePost.Content);
                }
                else
                {
                    return Ok(null);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }

    public class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Createtime { get; set; }
    }
}
