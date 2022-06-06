using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Data.Utils;

[assembly: OwinStartup(typeof(APIProject.Startup))]

namespace APIProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //app.UseJwtBearerAuthentication(
            //    new JwtBearerAuthenticationOptions
            //    {
            //        AuthenticationMode = AuthenticationMode.Active,
            //        TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = SystemParam.JWT_ISSUER, //some string, normally web url,  
            //            ValidAudience = SystemParam.JWT_ISSUER,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SystemParam.JWT_KEY))
            //        }
            //    });
        }
    }
}
