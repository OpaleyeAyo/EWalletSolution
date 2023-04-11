using AutoMapper;
using EWallet.DataLayer.AutoMapper;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.Implementations;
using EWallet.Test.MockData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EWallet.Test.Systems.Services
{
    public class TestWalletRepository : IDisposable
    {
        private readonly AppDbContext _context;

        public TestWalletRepository()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSingleWallet()
        {
            //Arrange
            _context.Wallets.Add(WalletMockData.GetWallet());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<ICustomerRepository> mockCustomerRepo = new Mock<ICustomerRepository>();

            Mock<ILogger<WalletRepository>> logger = new Mock<ILogger<WalletRepository>>();

            var sut = new WalletRepository(_context, mapper, mockCustomerRepo.Object, logger.Object);

            //Act
            Guid id = new Guid();

            var result = await sut.GetByIdAsync(id);

            //Assert
            result.Should().Equals(WalletMockData.GetWallet());
        }

        [Fact]
        public async Task GetByAccountNumberAsync_ShouldReturnSingleWallet()
        {
            //Arrange
            _context.Wallets.Add(WalletMockData.GetWallet());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<ICustomerRepository> mockCustomerRepo = new Mock<ICustomerRepository>();

            Mock<ILogger<WalletRepository>> logger = new Mock<ILogger<WalletRepository>>();

            var sut = new WalletRepository(_context, mapper, mockCustomerRepo.Object, logger.Object);

            //Act
            string actNum = "334223231231";

            var result = await sut.GetByAccountNumberAsync(actNum);

            //Assert
            result.Should().Equals(WalletMockData.GetWallet());
        }

        [Fact]
        public async Task GetUserTransactionsAsync_ShouldBeNull()
        {
            //Arrange
            _context.Wallets.AddRange(WalletMockData.GetWallets());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<ICustomerRepository> mockCustomerRepo = new Mock<ICustomerRepository>();

            Mock<ILogger<WalletRepository>> logger = new Mock<ILogger<WalletRepository>>();

            var sut = new WalletRepository(_context, mapper, mockCustomerRepo.Object, logger.Object);

            var WParams = new WalletParams();
            var id = new Guid();

            //Act
            var result = await sut.GetUserWalletsAsync(WParams, id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ReturnTransactionList()
        {
            //Arrange
            _context.Wallets.AddRange(WalletMockData.GetWallets());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<ICustomerRepository> mockCustomerRepo = new Mock<ICustomerRepository>();

            Mock<ILogger<WalletRepository>> logger = new Mock<ILogger<WalletRepository>>();

            var sut = new WalletRepository(_context, mapper, mockCustomerRepo.Object, logger.Object);

            //Act
            WalletParams walletParams = new WalletParams();

            var result = await sut.GetAllAsync(walletParams);

            //Assert
            result.Should().HaveCount(WalletMockData.GetWallets().Count);
        }

        [Fact]
        public async Task GetCustomerWalletsAsync_ShouldReturnNull()
        {
            //Arrange
            _context.Wallets.AddRange(WalletMockData.GetWallets());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<ICustomerRepository> mockCustomerRepo = new Mock<ICustomerRepository>();

            Mock<ILogger<WalletRepository>> logger = new Mock<ILogger<WalletRepository>>();

            var sut = new WalletRepository(_context, mapper, mockCustomerRepo.Object, logger.Object);

            //Act
            Guid id = new Guid();
            WalletParams walletParams = new WalletParams();

            var result = await sut.GetCustomerWalletsAsync(walletParams, id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateNewWalletAsync_ShouldReturnNull()
        {
            //Arrange
            _context.Wallets.Add(WalletMockData.GetWallet());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<ICustomerRepository> mockCustomerRepo = new Mock<ICustomerRepository>();

            Mock<ILogger<WalletRepository>> logger = new Mock<ILogger<WalletRepository>>();

            var sut = new WalletRepository(_context, mapper, mockCustomerRepo.Object, logger.Object);

            //Act
            Guid id = new Guid();
            WalletParams walletParams = new WalletParams();

            CreateWalletRequestDto model = new CreateWalletRequestDto
            {
                CurrencyCode = "GHU"
            };

            var result = await sut.CreateNewWalletAsync(model, id);

            //Assert
            result.Should().Equals(WalletMockData.GetWallet());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            //GC.SuppressFinalize(true);
        }
    }
}
