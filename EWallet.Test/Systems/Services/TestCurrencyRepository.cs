using AutoMapper;
using EWallet.DataLayer.AutoMapper;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.Implementations;
using EWallet.Test.MockData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EWallet.Test.Systems.Services
{
    public class TestCurrencyRepository : IDisposable
    {
        private readonly AppDbContext _context;

        public TestCurrencyRepository()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAll_ReturnListOfCurrency()
        {
            //Arrange
            _context.Currencies.AddRange(CurrencyMockData.GetCurrencies());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<IPhotoRepository> mockPhotoRepo = new Mock<IPhotoRepository>();

            var sut = new CurrencyRepository(mapper, _context, mockPhotoRepo.Object);

            //Act
            var result = await sut.GetAllAsync();

            //Assert
            result.Count.Equals(CurrencyMockData.GetCurrencies().Count);

        }

        [Fact]
        public async Task GetByIdAsync_ReturnASingleCurrency()
        {
            //Arrange
            _context.Currencies.AddRange(CurrencyMockData.GetCurrency());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<IPhotoRepository> mockPhotoRepo = new Mock<IPhotoRepository>();

            var sut = new CurrencyRepository(mapper, _context, mockPhotoRepo.Object);

            //Act
            var id = Guid.NewGuid();

            var result = await sut.GetByIdAsync(id);

            //Assert
            result.Should().Equals(CurrencyMockData.GetCurrency());
        }

        [Fact]
        public async Task GetByCodeAsync_ReturnASingleCurrency()
        {
            //Arrange
            _context.Currencies.AddRange(CurrencyMockData.GetCurrency());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<IPhotoRepository> mockPhotoRepo = new Mock<IPhotoRepository>();

            var sut = new CurrencyRepository(mapper, _context, mockPhotoRepo.Object);

            //Act
            string code = "NGN";

            var result = await sut.GetCurrencyByCode(code);

            //Assert
            result.Should().Equals(CurrencyMockData.GetCurrency());
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallSaveAsyncOnce()
        {
            //Arrange
            _context.Currencies.AddRange(CurrencyMockData.GetCurrency());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            Mock<IPhotoRepository> mockPhotoRepo = new Mock<IPhotoRepository>();

            var sut = new CurrencyRepository(mapper, _context, mockPhotoRepo.Object);

            //Act
            var model = new UpdateCurrencyRequestDto();
            Guid id = Guid.NewGuid();

            var result = await sut.UpdateAsync(model, id);

            //Assert
            result.Should().Equals(CurrencyMockData.GetCurrency());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            //GC.SuppressFinalize(true);
        }
    }
}
