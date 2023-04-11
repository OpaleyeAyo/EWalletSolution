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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]

    public class TransactionController : BaseController
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserContext _userContext;

        public TransactionController(ILogger<TransactionController> logger,
            ITransactionRepository transactionRepository, IUserContext userContext,
            IWalletRepository walletRepository)
        {
            _logger = logger;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _userContext = userContext;
        }

        //[HttpGet]
        //[Route("getAllTransactions")]
        //public async Task<ActionResult<Result<PagedResult<GetTransactionResponseDto>>>> GetAllAsync([FromQuery] TransactionParams transactionParams)
        //{
        //    var response = new Result<PagedResult<GetTransactionResponseDto>>();

        //    try
        //    {
        //        var transactions = await _transactionRepository.GetAllAsync(transactionParams);

        //        if (transactions == null)
        //        {
        //            response.Error = new Error()
        //            {
        //                Code = 404,
        //                Type = "Not found."
        //            };
        //            response.Message = "No transaction records found.";
        //            _logger.LogError($"No transaction records found.");
        //            return NotFound(response);
        //        }

        //        response.Message = "All transaction records retrieved.";
        //        response.IsSuccess = true;
        //        response.Data = transactions;
        //        _logger.LogInformation($"All transaction records retrieved.");
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong: {ex}");

        //        response.Error = new Error()
        //        {
        //            Code = 500,
        //            Type = "Bad Request."
        //        };

        //        response.Message = ex.StackTrace.ToString();

        //        return response;
        //    }
        //}

        //[HttpGet]
        //[Route("getTransaction/{transactionId}")]
        //public async Task<ActionResult<Result<GetTransactionResponseDto>>> GetByIdAsync(Guid transactionId)
        //{
        //    var response = new Result<GetTransactionResponseDto>();
        //    try
        //    {
        //        var transaction = await _transactionRepository.GetAsync(transactionId);

        //        if (transaction == null)
        //        {
        //            response.Error = new Error()
        //            {
        //                Code = 404,
        //                Type = "Not found."
        //            };
        //            response.Message = "Transaction not found.";
        //            _logger.LogError($"Transaction with id of {transactionId} not found.");
        //            return NotFound(response);
        //        }

        //        response.Message = "Transaction record retrieved.";
        //        response.IsSuccess = true;
        //        response.Data = transaction;
        //        _logger.LogInformation($"Transaction with id of {transactionId} retrieved.");
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong: {ex}");

        //        response.Error = new Error()
        //        {
        //            Code = 500,
        //            Type = "Bad Request."
        //        };

        //        response.Message = ex.StackTrace.ToString();

        //        return response;
        //    }
        //}

        [HttpPost]
        [Route("depost")]
        public async Task<ActionResult<Result<PaymentDepositResponseDto>>> DepositAsycn(PaymentDepositRequestDto model)
        {
            var response = new Result<PaymentDepositResponseDto>();

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

                var deposit = await _walletRepository.MakeDepositAsync(model, loggedInUserId);

                if (deposit != null)
                {
                    response.Message = "Deposit successully completed.";
                    response.IsSuccess = true;
                    response.Data = deposit;
                    return Ok(response);
                }

                response.Error = new Error()
                {
                    Code = 400,
                    Type = "Bad Request."
                };

                response.Message = "Cannot complete transaction.";

                _logger.LogError("Cannot complete transaction.");

                return BadRequest(response);

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
        [Route("withdraw")]
        public async Task<ActionResult<Result<PaymentWithdrawalResponseDto>>> WithdrawAsycn(PaymentWithdrawalRequestDto model)
        {
            var response = new Result<PaymentWithdrawalResponseDto>();
           
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

                var withdraw = await _walletRepository.MakeWithdrawalAsync(model, loggedInUserId);

                if (withdraw != null)
                {
                    response.Message = "Withdrawal successully completed.";
                    response.IsSuccess = true;
                    response.Data = withdraw;
                    return Ok(response);
                }

                response.Error = new Error()
                {
                    Code = 400,
                    Type = "Bad Request."
                };

                response.Message = "Failed. Cannot complete withdrawal.";

                _logger.LogError("Failed. Cannot complete withdrawal.");

                return BadRequest(response);
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
