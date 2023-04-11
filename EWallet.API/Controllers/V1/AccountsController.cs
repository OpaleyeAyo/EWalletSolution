using AutoMapper;
using EWallet.API.AsyncDataTransfer;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.DTO.Errors;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Entities.DbEntities;
using EWallet.Utility.HttpContex;
using EWallet.Utility.JwtHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EWallet.API.Controllers.V1
{
    public class AccountsController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IJwtTokenMethod _jwtTokenMethod;
        private readonly ILogger<AccountsController> _logger;
        private readonly IMessageBusClient _messageBusClient;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(UserManager<IdentityUser> userManager, 
            IMessageBusClient messageBusClient, RoleManager<IdentityRole> roleManager,
            ICustomerRepository customerRepository, IPhotoRepository photoRepository,
            IJwtTokenMethod jwtTokenMethod, ILogger<AccountsController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _customerRepository = customerRepository;
            _jwtTokenMethod = jwtTokenMethod;
            _logger = logger;
            _photoRepository = photoRepository;
            _messageBusClient = messageBusClient;
        }

        [HttpPost]
        [Route("registerCustomer")]
        public async Task<ActionResult<Result<CustomerRegistrationResponseDto>>> RegisterAsync
            ([FromForm] CustomerRegistrationRequestDto model)
        {
            Result<CustomerRegistrationResponseDto> response = new Result<CustomerRegistrationResponseDto>();

            try
            {
                IdentityUser customerExist = await _userManager.FindByEmailAsync(model.Email);

                if (customerExist != null)
                {
                    response.Error = new Error()
                    {
                        Code = 400,

                        Type = "Bad Request."
                    };

                    response.Message = "Email already exist.";

                    _logger.LogError("Customer registration failed. Email provided is already used.");

                    return BadRequest(response);
                }

                GetCustomerResponseDto existingCustomer = await _customerRepository.GetByEmailAsync(model.Email);

                if (existingCustomer != null)
                {
                    response.Error = new Error()
                    {
                        Code = 400,

                        Type = "Bad Request."
                    };

                    response.Message = "Email already exist.";

                    _logger.LogError("Customer registration failed. Email provided is already used.");

                    return BadRequest(response);
                }

                IdentityUser newCustomer = new IdentityUser()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    EmailConfirmed = true
                };

                Customer _newCustomer = new Customer()
                {
                    IdentityId = new Guid(newCustomer.Id),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };

                IdentityResult isCreated = await _userManager.CreateAsync(newCustomer, model.Password);

                if (!isCreated.Succeeded)
                {
                    response.Error = new Error()
                    {
                        Code = 400,

                        Type = "Bad Request."
                    };

                    response.Message = "Customer registration failed.";

                    _logger.LogError("Regsiter customer endpoint---> Customer identity registration failed.");

                    response.Data = new CustomerRegistrationResponseDto();

                    return BadRequest(response);
                };

                GetCustomerResponseDto customer = await _customerRepository.AddAsync(_newCustomer);

                if (customer == null)
                {
                    await _userManager.DeleteAsync(newCustomer);

                    response.Error = new Error()
                    {
                        Code = 400,

                        Type = "Bad Request."
                    };

                    response.Message = "Customer registration failed.";

                    _logger.LogError("Customer registration failed.");

                    response.Data = new CustomerRegistrationResponseDto();

                    return BadRequest(response);
                }

                bool roleExist = await _roleManager.RoleExistsAsync("Customer");
                
                if (roleExist)
                {
                    _logger.LogInformation($"Role customer exist. Adding role...");

                    var userRole = await _userManager.AddToRoleAsync(newCustomer, "Customer");

                    if (!userRole.Succeeded)
                    {
                        _logger.LogError("Cannot add role customer to user.");
                    }

                    _logger.LogInformation($"Role has been added to the customer account...");
                }

                if (model.ProfilePhoto != null)
                {
                    var uploadModel = new UploadPhotoRequestDto() { Photo = model.ProfilePhoto };

                    var uploadPhoto = await _photoRepository.UploadPhotoAsync(uploadModel, customer.Id);
                }

                try
                {
                    var eventModel = new GenericEventDto<GetCustomerResponseDto>()
                    {
                        EventType = AsyncEventType.CreateWalletsForCustomer,
                        EventModel = customer
                    };

                    _messageBusClient.PubMessage(eventModel);

                    _logger.LogInformation("New Notification message published.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Could not publish message.");
                    _logger.LogError($"Something went wrong with the message bus: {ex.Message}");
                }

                string token = await _jwtTokenMethod.GenerateJwtToken(newCustomer);

                response.Data = new CustomerRegistrationResponseDto()
                {
                    Token = token
                };

                response.IsSuccess = true;

                response.Message = "Customer registration successful.";

                _logger.LogInformation("Customer registration successful.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");

                response.Error = new Error()
                {
                    Code = 500,
                    Type = "Bad Request."
                };

                response.Message = ex.StackTrace.ToString();

                return response;
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<Result<CustomerLoginResponseDto>>> LoginAsync([FromBody] CustomerLoginRequestDto model)
        {
            var response = new Result<CustomerLoginResponseDto>();

            try
            {
                if (ModelState.IsValid)
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

                    var userExist = await _customerRepository.GetByEmailAsync(model.Email);

                    if (userExist == null)
                    {
                        response.Error = new Error()
                        {
                            Code = 400,
                            Type = "Bad Request."
                        };

                        response.Message = "Invalid credential.";

                        response.Data = new CustomerLoginResponseDto();
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
                        response.Data = new CustomerLoginResponseDto();
                        _logger.LogError("Invalid Credential.");
                        return BadRequest(response);
                    };

                    var token = await _jwtTokenMethod.GenerateJwtToken(existingUser);

                    response.IsSuccess = true;
                    response.Message = "Login successful";
                    response.Data = new CustomerLoginResponseDto() { Token = token };
                    return Ok(response);
                }
                else
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Invalid payload.";
                    response.Data = new CustomerLoginResponseDto();

                    return BadRequest(response);
                }
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
