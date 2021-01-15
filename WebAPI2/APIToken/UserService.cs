using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAPI2.DTO;

namespace WebAPI2.APIToken
{
    public class UserService: IUserService
    {
        //模拟测试，默认都是人为验证有效
        public bool IsValid(LoginRequestDTO req)
        {
            string md5password = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(req.Password.Trim()));
                var strResult = BitConverter.ToString(result);
                md5password = strResult.Replace("-", "");
            }
            var password = "E10ADC3949BA59ABBE56E057F20F883E"; //123456 MD5 32位大写加密
            if (req.Username.Trim() == "admin" && md5password == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
