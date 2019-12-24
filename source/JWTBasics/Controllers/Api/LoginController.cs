using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using JWTBasics.Models;
using JWTBasics.JWT;

namespace JWTBasics.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public object Post([FromBody] LoginObj loginObj)
        {
            if(loginObj.UserName == "John" && loginObj.Password == "password")
            {
                TokenManager tokenManager = new TokenManager();
                string token = tokenManager.CreateToken(new User() { Name = loginObj.UserName, Privilege = "user" });
                return new
                {
                    jwtToken = token,
                    message = "Authentication successful."
                };
            }
            else
            {
                return new
                {
                    message = "Authentication failed."
                };
            }
            
        }
    }
}
