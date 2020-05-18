using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.DTO;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/login")]
    public class LoginsController : ControllerBase
    {
        private IStudentsDbService _dbService;
        private IConfiguration _configuration;

        public LoginsController(IStudentsDbService service, IConfiguration configuration) 
        {
            _dbService = service;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {
            if (!_dbService.Login(loginRequest))
                return Unauthorized("Błąd logowania");
          
            var token = CreateToken();

            var RefreshToken = Guid.NewGuid();
            _dbService.UpdateRefreshToken(RefreshToken.ToString(), loginRequest.Login);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = RefreshToken
            });
        }

        [HttpPost("refresh-token/{refreshToken}")]
        public IActionResult RefreshToken(string refreshToken)
        {
            if (!_dbService.GetRefreshToken(refreshToken))
                return Unauthorized();

            var token = CreateToken();

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        private JwtSecurityToken CreateToken()
        {
            var claims = new[]
            {
                new Claim (ClaimTypes.NameIdentifier, "0"),
                new Claim (ClaimTypes.Role, "Employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );
        }

    }
}