using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Car_API.DTO;
using Car_API.Interfaces;
using Car_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Car_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _auth;
        public AuthController(IAuth auth)
        {
            _auth = auth;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDto)
        {
            userDto.UserName = userDto.UserName.ToLower();

            if (await _auth.UserExists(userDto.UserName))
            {
                return BadRequest("Username already exists");
            }

            var user = new User { UserName = userDto.UserName, TelephoneNumber = userDto.TelephoneNumber };

            var createdUser = await _auth.Register(user, userDto.Password);

            userDto.UserId = createdUser.UserId;

            return Created(Request.Path + $"/{createdUser.UserId}", userDto);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            var user = await _auth.Login(userDTO.UserName.ToLower(), userDTO.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            userDTO.UserId = user.UserId;

            return Ok(userDTO);
        }
    }
}