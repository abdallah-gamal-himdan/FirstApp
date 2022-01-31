using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using API.DTOs;
using API.Interfaces;
namespace API.Controllers
{
    public class AccountController : CommonController
    {
        private readonly DataContext _data;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext data,ITokenService TokenService)
        {
          _data=data;
          _tokenService = TokenService;
        }
        
        [HttpPost("register")]
        public async Task <ActionResult<UserDto>> Register(RegisterDTO dto)
        {
            if(await CheckExistance(dto.Username)) return BadRequest("user is already exist");
          
            using var mac=new HMACSHA512();
            var user=new AppUser{
                UserName=dto.Username.ToLower(),
                passwordHash =mac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                passwordSalt = mac.Key
            };
            _data.Users.Add(user);
             await _data.SaveChangesAsync();

             return new UserDto{
                 Username = user.UserName,
                 Token = _tokenService.CreateToken(user)
             };
        }

       [HttpPost("login")]
        public async Task <ActionResult<UserDto>> Login(LoginDTO dto)
        {
            var user=  await _data.Users.SingleOrDefaultAsync(x=>x.UserName==dto.Username);

            if(user==null)return Unauthorized("wrong username or password");
            using var mac=new HMACSHA512(user.passwordSalt);
            var passwordHash =mac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
             for(int i=0;i<passwordHash.Length;i++)
             {
                 if(passwordHash[i]!=user.passwordHash[i]) 
                  return Unauthorized("wrong username or password");
             }
           

             return new UserDto{
                 Username = user.UserName,
                 Token = _tokenService.CreateToken(user)
             };;
        }

        public async Task<bool>CheckExistance(string username)
        {
           return await _data.Users.AnyAsync(x => x.UserName==username.ToLower());
        }
    }
}