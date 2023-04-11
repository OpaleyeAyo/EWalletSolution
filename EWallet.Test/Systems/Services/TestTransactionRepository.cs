using AutoMapper;
using EWallet.DataLayer.AutoMapper;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.Generic;
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
    public class TestTransactionRepository : IDisposable
    {
        private readonly AppDbContext _context;
        
        public TestTransactionRepository()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public void GetAll_ReturnTransactionList()
        {
            //Arrange
            _context.Transactions.AddRange(TransactionMockData.GetTransactions());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            var sut = new TransactionRepository(_context, mapper);

            //Act
            var result =  sut.GetAll();

            //Assert
            result.Should().HaveCount(TransactionMockData.GetTransactions().Count);
        }

        [Fact]
        public async Task GetUserTransactionsAsync_ReturnTransactionList()
        {
            //Arrange
            _context.Transactions.AddRange(TransactionMockData.GetTransactions());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            var sut = new TransactionRepository(_context, mapper);

            var tParams = new TransactionParams();
            var id = new Guid();

            //Act
            var result = await sut.GetUserTransactionsAsync(tParams, id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_ReturnSingleTransaction()
        {
            //Arrange
            _context.Transactions.Add(TransactionMockData.GetTransaction());
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });

            var mapper = mockMapper.CreateMapper();

            var sut = new TransactionRepository(_context, mapper);

            //Act
            Guid id = new Guid();

            var result = await sut.GetAsync(id);

            //Assert
            result.Should().Equals(TransactionMockData.GetTransaction());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            //GC.SuppressFinalize(true);
        }
    }
}
