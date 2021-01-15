using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI2.DTO;

namespace WebAPI2.APIToken
{
    public interface IUserService
    {
        bool IsValid(LoginRequestDTO req);
    }
}
