using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTableApi.Models;
using System.IdentityModel.Tokens.Jwt;
using TimeTableApi.Settings;
using TimeTableApi.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TimeTableApi.Services
{
    public class UserService:IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITeacherService _teacherService;
        private readonly JWT _jwt;
        public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt,ITeacherService teacherService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _teacherService = teacherService;
        }

        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            var user = new User
            {
                UserName = model.Username,                
            };
            var userWithSameName = await _userManager.FindByNameAsync(model.Username);
            var validRole = Enum.GetValues(typeof(Authorization.Roles)).Cast<Authorization.Roles>().Where(x => x.ToString().ToLower() == model.Role.ToLower()).FirstOrDefault();
            if(validRole == Authorization.Roles.Teacher && (String.IsNullOrEmpty(model.Department) || String.IsNullOrEmpty(model.Faculty)))
            {
                return new BadRequestObjectResult("Department and Faculty are required for a teacher");
            }
            if (userWithSameName == null)
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var roleExists = Enum.GetNames(typeof(Authorization.Roles)).Any(x => x.ToLower() == model.Role.ToLower());
                    if (roleExists)
                    {
                        await _userManager.AddToRoleAsync(user, validRole.ToString());
                        if(validRole == Authorization.Roles.Teacher)
                        {
                            await _teacherService.AddAsync(user,model.Faculty,model.Department);                                    
                        }
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, Authorization.default_role.ToString());
                    }
                    return new OkResult();
                }
                else
                {
                    return new BadRequestObjectResult(result.Errors);
                }
            }
            else
            {
                return new BadRequestObjectResult($"UserName {user.UserName} is already registered.");
            }
        }

        public async Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model)
        {
            var authenticationModel = new AuthenticationModel();
            var user = await _userManager.FindByNameAsync(model.UserName);
            
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"No Accounts Registered with {model.UserName}.";
                return authenticationModel;
            }
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authenticationModel.IsAuthenticated = true;
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
                authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authenticationModel.UserName = user.UserName;
                var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                authenticationModel.Roles = rolesList.ToList();
                var teacher = await _teacherService.FindTeacher(user);
                if(teacher != null)
                {
                    authenticationModel.TeacherId = teacher.Id;
                }
                return authenticationModel;
            }
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Incorrect Credentials for user {user.UserName}.";
            return authenticationModel;
        }
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
    }
}
