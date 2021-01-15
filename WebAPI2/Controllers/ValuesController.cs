using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private static List<Person> Plist = new List<Person>();
        public ValuesController()
        {
            Person p1 = new Person
            {
                Id = 1,
                Name = "zh",
                Createtime = DateTime.Now
            };
            Person p2 = new Person
            {
                Id = 2,
                Name = "zh1",
                Createtime = DateTime.Now
            };
            if (Plist.Count <= 0)
            {
                Plist.Add(p1);
                Plist.Add(p2);
            }
        }

        [Route("AllPersons")]
        [HttpGet]
        
        public ActionResult GetAllPersons()
        {
            return new JsonResult(Plist);
        }

        [Route("Person")]
        [HttpGet]
        public ActionResult GetPerson(long Id)
        {
            var person = Plist.FirstOrDefault(e => e.Id == Id);
            return new JsonResult(person);
        }

        [Route("AddPerson")]
        [HttpPost]
        public ActionResult AddPerson(Person p)
        {
            if (p != null)
            {
                Plist.Add(p);
            }
            return new JsonResult(Plist);
        }
    }

    public class Person { 
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Createtime { get; set; }
    }
}
