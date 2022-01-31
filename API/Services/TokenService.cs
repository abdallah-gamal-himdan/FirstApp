using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly  SymmetricSecurityKey _key;
        public TokenService(IConfiguration conf)
        {
          _key =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims=new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.NameId,user.UserName)
            };
                var cred=new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature);

                var tokenDescripter=new SecurityTokenDescriptor
                {
                    Subject =new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = cred
                };
                var TokenHandler=new JwtSecurityTokenHandler();

                var token= TokenHandler.CreateToken(tokenDescripter);

                return TokenHandler.WriteToken(token);
        }
    }
}