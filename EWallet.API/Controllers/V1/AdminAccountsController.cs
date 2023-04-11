using EWallet.DataLayer.DTO.Errors;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Utility.JwtHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.API.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
    public class AdminAccountsController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminAccountsController> _logger;
        private readonly IJwtTokenMethod _jwtTokenMethod;

        public AdminAccountsController(UserManager<IdentityUser> userManager, IJwtTokenMethod jwtTokenMethod,
            RoleManager<IdentityRole> roleManager, ILogger<AdminAccountsController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _jwtTokenMethod = jwtTokenMethod;
        }

        [HttpGet]
        [Route("getAllRoles")]
        public async Task<ActionResult<Result<List<IdentityRole>>>> GetAllAsync()
        {
            var response = new Result<List<IdentityRole>>();

            try
            {
                var roles = await _roleManager.Roles.ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");

                response.Error = new Error()
                {
                    Code = 500,
                    Type = "Server error."
                };

                response.Message = ex.StackTrace.ToString();

                return response;
            }
        }

        [HttpPost]
        [Route("registerAdmin")]
        public async Task<ActionResult<Result<AdminRegistrationResponseDto>>> RegisterAdminAsync([FromBody] AdminRegistrationRequestDto model)
        {
            var response = new Result<AdminRegistrationResponseDto>();

            try
            {
                var adminExist = await _userManager.FindByEmailAsync(model.Email);

                if (adminExist != null)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Email already exist.";

                    _logger.LogError("Admin registration failed. Email provided is already used.");

                    return BadRequest(response);
                }

                IdentityUser newAdmin = new IdentityUser()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    EmailConfirmed = true
                };

                var registerAdmin = await _userManager.CreateAsync(newAdmin, model.Password);

                if (!registerAdmin.Succeeded)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Admin registration failed.";
                    _logger.LogError("Admin registration failed.");

                    return BadRequest(response);
                }

                var addRole = await _userManager.AddToRoleAsync(newAdmin, "Admin");

                if (!addRole.Succeeded)
                {
                    await _userManager.DeleteAsync(newAdmin);

                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Admin registration failed. Could not add role.";
                    _logger.LogError("Admin registration failed. Could not add role.");

                    return BadRequest(response);
                }

                response.Data = new AdminRegistrationResponseDto() {Data = true };
                response.IsSuccess = true;
                response.Message = "Admin registration successful.";
                _logger.LogInformation("Admin registration successful.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");

                response.Error = new Error()
                {
                    Code = 500,
                    Type = "Server error."
                };

                response.Message = ex.StackTrace.ToString();

                return response;
            }
        }

        [HttpGet]
        [Route("getAllIdentityUsers")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users              
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("createRole")]
        public async Task<ActionResult<Result<bool>>> CreateAsync(string name)
        {
            var response = new Result<bool>();
            try
            {
                //check if the role exist
                bool roleExist = await _roleManager.RoleExistsAsync(name);

                if (roleExist)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Role already exist.";
                    _logger.LogError($"Cannot the create role {name}. Role already exist.");
                    return BadRequest(response);
                }

                var role = await _roleManager.CreateAsync(new IdentityRole(name));

                if (!role.Succeeded)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Failed. Cannot create role.";
                    _logger.LogError($"Cannot create the role {name}. Failed.");
                    return BadRequest(response);
                }

                response.Message = "The role has been added successfully.";
                response.IsSuccess = true;
                response.Data = true;
                _logger.LogInformation($"The role {name} has been added successfully");
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");

                response.Error = new Error()
                {
                    Code = 500,
                    Type = "Server error."
                };

                response.Message = ex.StackTrace.ToString();

                return response;
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<AdminLoginResponseDto>>> LoginAsync([FromBody] AdminLoginRequestDto model)
        {
            var response = new Result<AdminLoginResponseDto>();

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);

                if (existingUser == null)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Invalid credential.";
                    _logger.LogError("Invalid Credential.");
                    return BadRequest(response);
                };

                var IsPasswordValid = await _userManager.CheckPasswordAsync(existingUser, model.Password);

                if (!IsPasswordValid)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Invalid credential.";
                    _logger.LogError("Invalid Credential.");
                    return BadRequest(response);
                };

                var token = await _jwtTokenMethod.GenerateJwtToken(existingUser);

                response.IsSuccess = true;
                response.Message = "Login successful";
                response.Data = new AdminLoginResponseDto() { Token = token };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");

                response.Error = new Error()
                {
                    Code = 500,
                    Type = "Server error."
                };

                response.Message = ex.StackTrace.ToString();
                return response;
            }
        }
    }
}
