using EWallet.API.AsyncDataTransfer;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Utility.HttpContex;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.API.Controllers.V1
{
    public class WalletController : BaseController
    {
        private readonly ILogger<WalletController> _logger;
        private readonly IWalletRepository _walletRepository;
        private readonly IBackgroundServiceRepository _backgroundServiceRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IUserContext _userContext;
        private readonly IMessageBusClient _messageBusClient;

        public WalletController(IWalletRepository walletRepository, ILogger<WalletController> logger,
            ICurrencyRepository currencyRepository, IMessageBusClient messageBusClient,
            IBackgroundServiceRepository backgroundServiceRepository, IUserContext userContext)
        {
            _backgroundServiceRepository = backgroundServiceRepository;
            _walletRepository = walletRepository;
            _userContext = userContext;
            _currencyRepository = currencyRepository;
            _messageBusClient = messageBusClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("getUserWallets")]
        public async Task<IActionResult> GetUserWalletsAsync([FromQuery] WalletParams walletParams)
        {
            try
            {
                Guid loggedInUserId = new Guid(_userContext.User.Claims.ToList()
                    .FirstOrDefault(x => x.Type == "Id").Value);

                if (string.IsNullOrEmpty(loggedInUserId.ToString()))
                {
                    return BadRequest("Invalid user.");
                }

                var result = await _walletRepository.GetUserWalletsAsync(walletParams, loggedInUserId);

                if (result == null)
                {
                    NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("getAllWallets")]
        public async Task<IActionResult> GetAllWalletsAsync([FromQuery] WalletParams walletParams)
        {
            try
            {
                var result = await _walletRepository.GetAllAsync(walletParams);

                if (result == null)
                {
                    NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //[HttpGet]
        //[Route("getAllCurrencies")]
        //public async Task<IActionResult> GetAllCurrenciesAsync()
        //{
        //    try
        //    {
        //        var result = await _currencyRepository.GetAllAsync();

        //        if (result == null)
        //        {
        //            NotFound();
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}
