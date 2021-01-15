using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI2.DTO;

namespace WebAPI2.APIToken
{
    public interface ITokenAuthenticationService
    {
        bool IsAuthenticated(LoginRequestDTO request, out string token);
    }
}
