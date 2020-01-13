
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using datingapp.API.Data;
using datingapp.API.Dtos;
using datingapp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace datingapp.API.Controllers
{
    [Route("api/[controller]")]
     [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserToRegisterDto userToRegisterDto) //parameter will be taken from the body
        {
            userToRegisterDto.UserName = userToRegisterDto.UserName.ToLower();
            if (await _repo.IsUserExists(userToRegisterDto.UserName))
            {
                return BadRequest("Already exists");

            }
            User UsertoCreate = new User()
            {
                UserName = userToRegisterDto.UserName
            };

            var createdUser = await _repo.Register(UsertoCreate, userToRegisterDto.Password);
            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.UserName.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                Unauthorized();

            var claims = new[]{
                 new Claim(ClaimTypes.NameIdentifier, userFromRepo.ID.ToString()),
                 new Claim(ClaimTypes.Name,userFromRepo.UserName)
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature); 

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler  = new JwtSecurityTokenHandler();
           var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(
                new {
                    token = tokenHandler.WriteToken(token)
                }
            );

        }

    }
}
