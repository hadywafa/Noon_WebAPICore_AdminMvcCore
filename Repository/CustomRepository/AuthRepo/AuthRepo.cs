using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BL.Helper;
using BL.ViewModels.RequestVModels;
using BL.ViewModels.ResponseVModels;
using EFModel.Enums;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.GenericRepository;
using Repository.UnitWork;

namespace Repository.CustomRepository.AuthRepo
{
    public class AuthRepo : IAuthRepo
    {
        #region Inject UserManager Context in UserRepo

        private readonly UserManager<User> _userManager;
        private readonly Jwt _jwt;

        public AuthRepo(UserManager<User> userManager, IOptions<Jwt> jwt )
        {
            _userManager = userManager;
            _jwt = jwt.Value;
        }
        #endregion

        //this function Register user in database and return that user in VM form with main information + its Token that will inject in any request he asked again
        public async Task<VmAuthUser> RegisterAsync(VmRegisterUser vmRegisterUser)
        {
            //var duplicatedUserEmail = _userManager.FindByEmailAsync(vmRegisterUser.Email);
            //var duplicatedUserName = _userManager.FindByNameAsync(vmRegisterUser.UserName);
            if (await _userManager.FindByEmailAsync(vmRegisterUser.Email) is not null)
            {
                return new VmAuthUser() { Message = "Email is Already Existed", IsAuthenticated = false };
            }
            else if (await _userManager.FindByNameAsync(vmRegisterUser.UserName) is not null)
            {
                return new VmAuthUser() { Message = "UserName is Already Existed", IsAuthenticated = false };
            }
            else
            {
                User user = vmRegisterUser.ToUser();
                user.IsActive = true;
                var result = await _userManager.CreateAsync(user, vmRegisterUser.Password);
                if (!result.Succeeded)
                {
                    string errors = string.Empty;
                    foreach (var e in result.Errors)
                    {
                        errors += $"{e.Description}, ";
                    }

                    return new VmAuthUser() { Message = errors, IsAuthenticated = false };
                }
                else
                {
                    //Congratulation => User is Created Successfully , now assign him to specific role as front end told you in header body
                    await _userManager.AddToRoleAsync(user, vmRegisterUser.Role);

                    #region assign user to type [admin / customer / seller / shipper] depend on its role
                    switch (vmRegisterUser.Role)
                    {
                        case AuthorizeRoles.Admin:
                        {
                            var admin = new Admin();
                            user.Admin = admin;
                            await _userManager.UpdateAsync(user);
                        }
                            break;
                        case AuthorizeRoles.Customer:
                        {
                            var customer = new Customer();
                            user.Customer = customer;
                            await _userManager.UpdateAsync(user);
                        }
                            break;
                        case AuthorizeRoles.Seller:
                        {
                            var seller = new Seller() ;
                            user.Seller = seller;
                            await _userManager.UpdateAsync(user);
                        }
                            break;
                        case AuthorizeRoles.Shipper:
                        {
                            var shipper = new Shipper();
                            user.Shipper = shipper;
                            await _userManager.UpdateAsync(user);
                        }
                            break;
                    }
                    #endregion

                    //create token for user and storing it in Db and return it to user in VM form
                    var jwtSecurityToken = await CreateJwtToken(user);
                    return new VmAuthUser
                    {
                        Username = user.UserName,
                        Email = user.Email,
                        IsAuthenticated = true,
                        ExpiresOn = jwtSecurityToken.ValidTo,
                        Roles = new List<string> { vmRegisterUser.Role },
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
                    };
                }
            }
        }

        public async Task<VmAuthUser> SignInAsync(VmSignInUser model)
        {
            var authModel = new VmAuthUser();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            else
            {
                var jwtSecurityToken = await CreateJwtToken(user);
                var rolesListString = await _userManager.GetRolesAsync(user);
                List<string> rolesList = new List<string>();
                foreach (string s in rolesListString)
                {
                    rolesList.Add(s);
                }
                authModel.IsAuthenticated = true;
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.Email = user.Email;
                authModel.Username = user.UserName;
                authModel.ExpiresOn = jwtSecurityToken.ValidTo;
                authModel.Roles = rolesList;
                return authModel;
            }
        }

        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            //create claims [ main information that will be encrypted in token ]
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in roles) roleClaims.Add(new Claim("roles", role));
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email), new Claim("uid", user.Id)
            }.Union(userClaims).Union(roleClaims);

            //Encryption your claims with hmaSha256 algorithms
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(issuer: _jwt.Issuer, audience: _jwt.Audience, claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays), signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }


    }
}