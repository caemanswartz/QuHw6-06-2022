using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using QuHWBlazorWasmJwtUse.Server.Models;
using QuHWBlazorWasmJwtUse.Shared.Models;
namespace QuHWBlazorWasmJwtUse.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApiDbContext _context;
        public AuthController(IConfiguration config,
            ApiDbContext context)
        {
            _config=config;
            _context=context;
        }
        #pragma warning disable CS1998
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            User user=new User();
            CreatePaswordHash(request.Password,
                out byte[] passwordHash, out byte[] passwordSalt);
            user.Username=request.Username;
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;
            _context.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            User? user=await _context.Users.FindAsync(request.Username);
            if(user is null)
                return BadRequest("User not found.");
            if(!VerifyPasswordHash(request.Password,
                user.PasswordHash,user.PasswordSalt))
                return BadRequest("Wrong Password");
            string token=CreateToken(user);
            return Ok(token);
        }
#pragma warning restore CS1998
        private string CreateToken(User user)
        {
            List<Claim> claims=new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username)
            };
            var key=new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_config
                    .GetSection("AppSettings:Token").Value));
            var creds=new SigningCredentials(key,
                SecurityAlgorithms.HmacSha512Signature);
            var token=new JwtSecurityToken(
                claims:claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials:creds);
            var jwt=new JwtSecurityTokenHandler()
                .WriteToken(token);
            return jwt;
        }
        private bool VerifyPasswordHash(string password,
            byte[] passwordHash,byte[] passwordSalt)
        {
            using (var hmac=new HMACSHA512(passwordSalt))
            {
                var computedHash=hmac.ComputeHash(
                    System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private void CreatePaswordHash(string password,
            out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac=new HMACSHA512())
            {
                passwordSalt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8
                    .GetBytes(password));
            }
        }
    }
}