using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI2.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        [JsonProperty("username")]  //在属性上使用了 JsonProperty 特性，使得序列化时根据 JsonProperty 特性传入的字符串进行序列化
        public string Username { get; set; }


        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
