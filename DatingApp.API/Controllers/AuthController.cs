using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower(); // make the inputted username into lowercase.

            if (await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Username already exists");
            }

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password); // It's checking to see if the username and password match what is stored in the DB

            if (userFromRepo == null) // user not found in db
            {
                return Unauthorized();
            }
            // Now, build up a token that we're returning to the user
            var claims = new[] // token is going to contain 2 claims. One's going to be user's id, and the other claim is user's username
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };
            // Now in order to make sure the tokens are valid token when it comes back, the server needs to sign this token,
            // and here, we are creating a security key and then using this key as part of the signing credentials and encrypted this key with a hashing algorithm
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)); // reference the appsettings.json

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // Here, we are using HmacSha512 security algorithm to hash our var key from above

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            // using tokenHandler, we create a token and pass in the tokenDescriptor
            var token = tokenHandler.CreateToken(tokenDescriptor); // this token will contain the jwt token that we want to return to our client

            var user = _mapper.Map<UserForListDto>(userFromRepo);
            // we're returning token to the client as an object.
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}