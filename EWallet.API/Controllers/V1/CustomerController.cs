using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.DTO.Errors;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Utility.HttpContex;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.API.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Customer")]
    public class CustomerController : BaseController
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserContext _userContext;
        private readonly IWalletRepository _walletRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerRepository customerRepository, 
            IWalletRepository walletRepository, ITransactionRepository transactionRepository,
            IPhotoRepository photoRepository, ILogger<CustomerController> logger,
            IUserContext userContext)
        {
            _customerRepository = customerRepository;
            _userContext = userContext;
            _photoRepository = photoRepository;
            _logger = logger;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPut]
        [Route("uploadProfilePhoto")]
        public async Task<ActionResult<Result<bool>>> UploadAsync([FromForm] UploadPhotoRequestDto model)
        {
            var response = new Result<bool>();
            
            try
            {
                Guid loggedInUserId = new Guid(_userContext.User.Claims.ToList()
                    .FirstOrDefault(x => x.Type == "Id").Value);

                if (string.IsNullOrEmpty(loggedInUserId.ToString()))
                {
                    return BadRequest("Invalid user.");
                }

                bool uploadPhoto = await _photoRepository.UploadPhotoAsync(model, loggedInUserId);

                if (!uploadPhoto)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Failed. Cannot upload photo.";

                    _logger.LogError($"Failed. Cannot upload photo for user with identity id of {loggedInUserId}.");

                    return BadRequest(response);
                }

                response.Data = uploadPhoto;
                response.IsSuccess = true;
                response.Message = "Photo upload succesful.";
                _logger.LogInformation($"Photo has been successully uploaded to user account with identity id of {loggedInUserId}.");
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
        [Route("getProfile")]
        public async Task<ActionResult<Result<GetCustomerResponseDto>>> GetByIdAsync()
        {
            var response = new Result<GetCustomerResponseDto>();

            try
            {
                var loggedInUserId = new Guid(_userContext.User.Claims.ToList()
                    .FirstOrDefault(x => x.Type == "Id").Value);

                if (string.IsNullOrEmpty(loggedInUserId.ToString()))
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Invalid user.";

                    _logger.LogError("Invalid user.");

                    return BadRequest(response);
                }

                _logger.LogInformation($"Loggedin user identity id : {loggedInUserId}.");

                var customer = await _customerRepository.GetByIdentityIdAsync(loggedInUserId);

                if (customer == null)
                {
                    response.Data = customer;
                    response.IsSuccess = true;
                    response.Message = "No record found";
                    _logger.LogInformation($"No customer record found for customer with identity id of {loggedInUserId}.");
                    return NotFound(response);
                }

                response.Data = customer;
                response.IsSuccess = true;
                response.Message = "Customer record retrieved";
                _logger.LogInformation($"Customer record with identity id of {loggedInUserId} retrieved.");
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
        [Route("getAllCustomerTransactions")]
        public async Task<ActionResult<Result<PagedResult<GetTransactionResponseDto>>>> GetAllTransactionsAsync([FromQuery] TransactionParams transactionParams)
        {
            var response = new Result<PagedResult<GetTransactionResponseDto>>();

            try
            {
                var loggedInUserId = new Guid(_userContext.User.Claims.ToList()
                    .FirstOrDefault(x => x.Type == "Id").Value);

                if (string.IsNullOrEmpty(loggedInUserId.ToString()))
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Invalid user.";

                    _logger.LogError("Invalid user.");

                    return BadRequest(response);
                }

                _logger.LogInformation($"Loggedin user identity id : {loggedInUserId}.");

                var result = await _transactionRepository.GetUserTransactionsAsync(transactionParams, loggedInUserId);

                if (result == null)
                {
                    response.Error = new Error()
                    {
                        Code = 404,
                        Type = "Not found."
                    };
                    response.Message = "No transaction found.";

                    _logger.LogError($"NO transaction found for the user with identity id of {loggedInUserId}.");
                    return NotFound(response);
                }

                response.Message = "All transaction records retrieved.";
                response.IsSuccess = true;
                response.Data = result;
                _logger.LogInformation($"All transaction records retrieved for the user with identity id of {loggedInUserId}.");
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

        [HttpGet]
        [Route("getAllCustomerWallets")]
        public async Task<ActionResult<Result<PagedResult<GetWalletResponseDto>>>> GetAllWalletsAsync([FromQuery] WalletParams walletParams)
        {
            var response = new Result<PagedResult<GetWalletResponseDto>>();

            try
            {
                var loggedInUserId = new Guid(_userContext.User.Claims.ToList()
                    .FirstOrDefault(x => x.Type == "Id").Value);

                if (string.IsNullOrEmpty(loggedInUserId.ToString()))
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Invalid user.";

                    _logger.LogError("Invalid user.");

                    return BadRequest(response);
                }

                _logger.LogInformation($"Loggedin user identity id : {loggedInUserId}.");

                var result = await _walletRepository.GetUserWalletsAsync(walletParams, loggedInUserId);

                if (result == null)
                {
                    response.Error = new Error()
                    {
                        Code = 404,
                        Type = "Not found."
                    };
                    response.Message = "No wallet found.";

                    _logger.LogError($"No wallet found for the user with identity id of {loggedInUserId}.");
                    return NotFound(response);
                }

                response.Message = "All wallet records retrieved.";
                response.IsSuccess = true;
                response.Data = result;
                _logger.LogInformation($"All wallet records retrieved for the user with identity id of {loggedInUserId}.");
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

        [HttpGet]
        [Route("getWallet/{walletId}", Name = "getWalletById")]
        public async Task<ActionResult<Result<GetWalletResponseDto>>> GetByWalletByIdAsync(Guid walletId)
        {
            var response = new Result<GetWalletResponseDto>();

            try
            {
                var wallet = await _walletRepository.GetByIdAsync(walletId);

                if (wallet == null)
                {
                    response.Error = new Error()
                    {
                        Code = 404,
                        Type = "Not found."
                    };
                    response.Message = "Wallet not found.";
                    _logger.LogError($"Wallet with id of {walletId} not found.");
                    return NotFound(response);
                }

                response.Message = "Wallet record retrieved.";
                response.IsSuccess = true;
                response.Data = wallet;
                _logger.LogInformation($"Wallet with id of {wallet.Id} retrieved.");
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
        [Route("createUserWallet")]
        public async Task<ActionResult<Result<GetWalletResponseDto>>> CreateWalletAsync([FromBody] CreateWalletRequestDto model)
        {
            var response = new Result<GetWalletResponseDto>();

            try
            {
                Guid loggedInUserId = new Guid(_userContext.User.Claims.ToList()
                    .FirstOrDefault(x => x.Type == "Id").Value);

                if (string.IsNullOrEmpty(loggedInUserId.ToString()))
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Invalid user.";

                    _logger.LogError("Invalid user.");

                    return BadRequest(response);
                }

                var result = await _walletRepository.CreateNewWalletAsync(model, loggedInUserId);

                if (result == null)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Cannot create wallet.";

                    _logger.LogError("Cannot create wallet.");

                    return BadRequest(response);
                }

                response.Message = "Wallet created successully.";
                response.IsSuccess = true;
                response.Data = result;
                _logger.LogInformation($"Wallet with id of {result.Id} created successfully.");
                return CreatedAtRoute("getWalletById", new { walletId = result.Id }, response);
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
    }
}
