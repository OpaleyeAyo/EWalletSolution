using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.DTO.Errors;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EWallet.API.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SuperAdmin")]
    public class SettlementAccountController : BaseController
    {
        private readonly ISettlementAccountRepository _settlementAccountRepository;
        private readonly ILogger<SettlementAccountController> _logger;

        public SettlementAccountController(ISettlementAccountRepository settlementAccountRepository,
            ILogger<SettlementAccountController> logger)
        {
            _logger = logger;
            _settlementAccountRepository = settlementAccountRepository;
        }

        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<Result<PagedResult<GetSettlementAccountResponseDto>>>> GetAllAsync([FromQuery] SettlementAccountParams settlementAccountparams)
        {
            var response = new Result<PagedResult<GetSettlementAccountResponseDto>>();

            try
            {
                var result = await _settlementAccountRepository.GetAllAsync(settlementAccountparams);

                if (result == null)
                {
                    response.Error = new Error()
                    {
                        Code = 404,
                        Type = "Not found."
                    };
                    response.Message = "No settlement account record found.";
                    _logger.LogError($"No settlement account record found.");
                    return NotFound(response);
                }

                response.Message = "All settlement accounts retrieved.";
                response.IsSuccess = true;
                response.Data = result;
                _logger.LogInformation($"All settlement accounts retrieved.");
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

    }
}
