using DinkToPdf;
using DinkToPdf.Contracts;
using EWallet.API.AsyncDataTransfer;
using EWallet.API.Utility;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.DTO.Errors;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Utility.HttpContex;
using EWallet.Utility.JwtHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EWallet.API.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SuperAdmin" )]

    public class AdminController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserContext _userContext;
        private readonly IJwtTokenMethod _jwtTokenMethod;
        private readonly ILogger<AdminController> _logger;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IMessageBusClient _messageBusClient;
        private readonly IConverter _converter;

        public AdminController(UserManager<IdentityUser> userManager, IWalletRepository walletRepository,
            ICustomerRepository customerRepository, ITransactionRepository transactionRepository,
            IPhotoRepository photoRepository, IConverter converter,
            IMessageBusClient messageBusClient, ICurrencyRepository currencyRepository,
           IUserContext userContext, IJwtTokenMethod jwtTokenMethod, ILogger<AdminController> logger)
        {
            _userContext = userContext;
            _userManager = userManager;
            _customerRepository = customerRepository;
            _jwtTokenMethod = jwtTokenMethod;
            _logger = logger;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _messageBusClient = messageBusClient;
            _currencyRepository = currencyRepository;
            _photoRepository = photoRepository;
            _converter = converter;
        }

        [HttpGet]
        [Route("getAllCustomers")]
        public async Task<ActionResult<Result<PagedResult<GetCustomerResponseDto>>>> GetAllCustomersAsync([FromQuery] CustomerParams customerParams)
        {
            var response = new Result<PagedResult<GetCustomerResponseDto>>();
            try
            {
                var customers = await _customerRepository.GellAllAsync(customerParams);

                if (customers == null)
                {
                    response.Error = new Error()
                    {
                        Code = 404,
                        Type = "Not found."
                    };

                    response.Message = "No customer record found.";
                    _logger.LogError($"NO customer record found.");

                    return NotFound(response);
                }

                response.Data = customers;
                response.IsSuccess = true;
                response.Message = "All customer records retrieved";
                _logger.LogInformation("All customer records retrieved.");
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
        [Route("getAllTransactions")]
        public async Task<ActionResult<Result<PagedResult<GetTransactionResponseDto>>>> GetAllTransactionsAsync([FromQuery] TransactionParams transactionParams)
        {
            var response = new Result<PagedResult<GetTransactionResponseDto>>();
            try
            {
                var result = await _transactionRepository.GetAllAsync(transactionParams);

                if (result == null)
                {
                    response.Error = new Error()
                    {
                        Code = 404,
                        Type = "Not found."
                    };
                    response.Message = "No transaction found.";

                    _logger.LogError($"NO transaction found.");
                    return NotFound(response);
                }

                response.Data = result;
                response.IsSuccess = true;
                response.Message = "All transaction records retrieved";
                _logger.LogInformation("All transaction records retrieved.");
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
        [Route("getCustomer/{customerId}")]
        public async Task<ActionResult<Result<GetCustomerResponseDto>>> GetByIdAsync(Guid customerId)
        {
            var response = new Result<GetCustomerResponseDto>();

            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);

                if (customer == null)
                {
                    response.Data = customer;
                    response.IsSuccess = true;
                    response.Message = "No record found";
                    _logger.LogInformation($"No customer record found for customer with id of {customerId}.");
                    return NotFound(response);
                }

                response.Data = customer;
                response.IsSuccess = true;
                response.Message = "Customer record retrieved";
                _logger.LogInformation($"Customer record with id of {customerId} retrieved.");
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
        [Route("getCustomerWallets/{customerId}")]
        public async Task<ActionResult<Result<PagedResult<GetWalletResponseDto>>>> GetCustomerWalletsAsync([FromQuery] WalletParams walletParams, Guid customerId)
        {
            var response = new Result<PagedResult<GetWalletResponseDto>>();

            try
            {
                var result = await _walletRepository.GetCustomerWalletsAsync(walletParams, customerId);

                if (result == null)
                {
                    response.Data = result;
                    response.IsSuccess = true;
                    response.Message = "No record found";
                    _logger.LogInformation($"No wallets found for the customer with id of {customerId}.");
                    return NotFound(response);
                }

                response.Data = result;
                response.IsSuccess = true;
                response.Message = "All wallet records retrieved";
                _logger.LogInformation($"All wallets records retrieved for customer with id of {customerId}.");
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

        [HttpDelete]
        [Route("delete/{customerId}")]
        public async Task<ActionResult<Result<bool>>> DeleteAsync(Guid customerId)
        {
            try
            {
                bool delete = await _customerRepository.DeleteAsync(customerId);

                if (delete)
                {
                    return NoContent();
                }

                return BadRequest("Cannot delete customer");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut]
        [Route("activate/{customerId}")]
        public async Task<ActionResult<Result<bool>>> ActivateUserAsync(Guid customerId)
        {
            try
            {
                bool response = await _customerRepository.ActivateAsync(customerId);

                if (response)
                {
                    return NoContent();
                }

                return BadRequest("Cannot activate customer");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut]
        [Route("deactivate/{customerId}")]
        public async Task<ActionResult<Result<bool>>> DeactivateUserAsync(Guid customerId)
        {
            try
            {
                bool response = await _customerRepository.DeactivateAsync(customerId);

                if (response)
                {
                    return NoContent();
                }

                return BadRequest("Cannot activate customer");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("createCurrency")]
        public async Task<IActionResult> CreateCurrencyAsync([FromForm] CreateCurrencyRequestDto model)
        {
            try
            {
                var response = await _currencyRepository.AddAsync(model);

                if (response == null)
                {
                    return BadRequest("Cannot create currency.");
                }

                try
                {
                    var eventModel = new GenericEventDto<GetCurrencyResponseDto>()
                    {
                        EventType = AsyncEventType.CreateNewSettlementAccount,
                        EventModel = response
                    };

                    _messageBusClient.PubMessage(eventModel);

                    _logger.LogInformation("New Notification message published.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Could not publish message.");
                    _logger.LogError($"Something went wrong with the message bus: {ex.Message}");
                }

                return CreatedAtRoute("getCurrencyById", new { currencyId = response.Id }, response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("updateCurrency/{currencyId}")]
        public async Task<ActionResult<Result<bool>>> UpdateCurrencyAsync([FromBody] UpdateCurrencyRequestDto model, Guid currencyId)
        {
            var response = new Result<bool>();
            try
            {
                bool result = await _currencyRepository.UpdateAsync(model, currencyId);

                if (result)
                {
                    _logger.LogInformation($"Currency with the id of {currencyId} has been updated successfully.");

                    return NoContent();
                }

                response.Error = new Error()
                {
                    Code = 400,
                    Type = "Bad Request."
                };

                response.Message = "Cannot update currency.";
                _logger.LogError($"Cannot update currency with the id of {currencyId}.");
                return BadRequest(response);
                
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
        [Route("uploadCurrencyLogo/{currencyId}")]
        public async Task<ActionResult<Result<bool>>> UploadCurrencyLogoAsync([FromForm] UploadPhotoRequestDto model, Guid currencyId)
        {
            var response = new Result<bool>();

            try
            {
                var currency = await _currencyRepository.GetByIdAsync(currencyId);

                if (currency == null)
                {
                    response.Error = new Error()
                    {
                        Code = 400,
                        Type = "Bad Request."
                    };

                    response.Message = "Failed. Currency does not exist.";
                    _logger.LogError($"Failed. Currency with the id of {currencyId} does not exist.");
                    return BadRequest(response);
                }

                bool result = await _photoRepository.UploadLogoAsync(model, currency.Id);

                if (result)
                {
                    _logger.LogInformation($"Logo had been addedd to th currency with the id of {currencyId} successfully.");

                    return CreatedAtRoute("getCurrencyById", new { currencyId = currency.Id}, currency);
                }

                response.Error = new Error()
                {
                    Code = 400,
                    Type = "Bad Request."
                };

                response.Message = "Failed. Cannot upload logo to the currency.";
                _logger.LogError($"Failed. Cannot upload load to currency with the id of {currencyId}.");
                return BadRequest(response);

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
        [Route("getCurrency/{currencyId}", Name = "getCurrencyById")]
        public async Task<IActionResult> GetCurrencyByIdAsync(Guid currencyId)
        {
            try
            {
                var wallet = await _currencyRepository.GetCurrencyById(currencyId);

                if (wallet == null)
                {
                    return NotFound();
                }

                return Ok(wallet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("exportTransactionsToCSV")]
        public FileResult ExportCSV()
        {
            try
            {
                byte[] bytes = _transactionRepository.ExportToCsv();

                return File(bytes, "text/csv", "Transactions.csv");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("exportTransactionsToPDF")]
        public IActionResult ExportPDF()
        {
            try
            {
                var HtmlGenerator = new TemplateGenerator(_transactionRepository);

                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10},
                    DocumentTitle = "Transactions in PDF", Out = @"C:\Users\PC\Desktop\Transactions.pdf"
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = HtmlGenerator.GetHtmlString(),
                    WebSettings = 
                    { 
                        DefaultEncoding = "utf-8", 
                        UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "style.css") 
                    },
                    HeaderSettings = 
                    {
                        FontName ="Arial", FontSize= 9, Right = "Page [page] of [toPage]", Line = true
                    },
                    //FooterSettings = 
                    //{
                    //    FontName ="Arial", FontSize= 9, Line = true, Center = "E"
                    //}
                };

                var pdf = new HtmlToPdfDocument
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings}
                };

                _converter.Convert(pdf);


                return Ok("Successfully created the PDF document.");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
