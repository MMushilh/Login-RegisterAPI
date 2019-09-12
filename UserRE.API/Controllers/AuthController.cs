using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UserRE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController :ControllerBase
    {
        private readonly Data.IAuthRepository _repo;
        private readonly  IConfiguration _congif;

        public AuthController (Data.IAuthRepository repo, IConfiguration config)
        {
            _congif=config;
            _repo=repo;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register (Dtos.UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.username = userForRegisterDto.username.ToLower();
            if ( await _repo.UserExsist(userForRegisterDto.username))
            return BadRequest("TheUserExsist");
            var CreateUser = new Model.User{
                UserName = userForRegisterDto.username
            };
            var CreatedUser = await _repo.Register(CreateUser , userForRegisterDto.password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (Dtos.UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.username.ToLower(),userForLoginDto.password);
            if (userFromRepo==null)return Unauthorized();
            var Claims = new []{
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName)
            };
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_congif.GetSection("AppSettings:Token").Value));
            var Card = new SigningCredentials(Key,SecurityAlgorithms.HmacSha512);
            var TokenDescription = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Now.AddHours(4),
                SigningCredentials = Card
            };
            var tokenHandler = new JwtSecurityTokenHandler(); 
            var token = tokenHandler.CreateToken(TokenDescription);
            return Ok(new{
                token = tokenHandler.WriteToken(token)
            });


        }
    }
}