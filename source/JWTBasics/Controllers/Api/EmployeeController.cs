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
    public class EmployeeController : ControllerBase
    {
        // GET: api/Employee
        [HttpGet]
        public object Get()
        {
            TokenManager tokenManager = new TokenManager();
            var authorizationHeader = Request.Headers.FirstOrDefault(h => h.Key == "Authorization");
            User user = tokenManager.GetUser(authorizationHeader.Value.ToString().Replace("Bearer ", ""));

            if (user.Name == "John")
            {
                if (user.Privilege == "admin")
                {
                    List<Employee> employees = new List<Employee>()
                    {
                        new Employee(){ Name = "Mark", Age = 31, Salary = 4500},
                        new Employee(){ Name = "Steve", Age = 25, Salary = 3200},
                        new Employee(){ Name = "Conner", Age = 49, Salary = 8320},
                    };

                    return new
                    {
                        message = "Successful.",
                        employees
                    };
                }
                else
                {
                    return new
                    {
                        message = "Authorization Failed."
                    };
                }
            }
            else
            {
                return new
                {
                    message = "Authentication Failed."
                };
            }
        }
    }
}
