using Exchange.Application.Dtos.Requests;
using Exchange.Application.UseCases.GetConversionHistory;
using Exchange.Domain.Entities;
using Exchange.Domain.Enums;
using Exchange.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace Exchange.Unit.Tests.Application.UseCases
{
    public class GetConversionHistoryUseCaseTests
    {
        private readonly Mock<IConversionRepository> _repositoryMock;
        private readonly GetConversionHistoryUseCase _useCase;
        private readonly IMemoryCache _memoryCache;

        public GetConversionHistoryUseCaseTests()
        {
            _repositoryMock = new Mock<IConversionRepository>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _useCase = new GetConversionHistoryUseCase(_repositoryMock.Object, _memoryCache);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoHistoryExists()
        {
            _repositoryMock.Setup(r => r.GetHistoryAsync("", "", null, null, 1, 20))
                .ReturnsAsync((new List<ConversionRecord>(), 0));

            var result = await _useCase.ExecuteAsync(new GetConversionHistoryRequest(null, null, null, null));

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value!.Items);
            Assert.Equal(0, result.Value.TotalCount);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnHistory_WhenRecordsExist()
        {
            var records = new List<ConversionRecord>
            {
                new ConversionRecord("BRL", "EUR", 1000m, 166.67m, 6m, ExchangeQuotationEnum.Buy, new DateOnly(2025, 8, 13), "BACEN"),
                new ConversionRecord("BRL", "USD", 500m, 83.33m, 6m, ExchangeQuotationEnum.Sell, new DateOnly(2025, 8, 14), "BACEN")
            };

            _repositoryMock.Setup(r => r.GetHistoryAsync("", "", null, null, 1, 20))
                .ReturnsAsync((records, 2));

            var result = await _useCase.ExecuteAsync(new GetConversionHistoryRequest(null, null, null, null));

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value!.Items.Count());
            Assert.Equal(2, result.Value.TotalCount);
            Assert.Contains(result.Value.Items, r => r.ToCurrency == "EUR" && r.OriginalAmount == 1000m);
            Assert.Contains(result.Value.Items, r => r.ToCurrency == "USD" && r.OriginalAmount == 500m);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenPageIsInvalid()
        {
            var result = await _useCase.ExecuteAsync(new GetConversionHistoryRequest(null, null, null, null, 0, 20));
            Assert.True(result.IsFailure);
            Assert.Equal("VALIDATION_ERROR", result.Error?.Code);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenPageSizeIsInvalid()
        {
            var result = await _useCase.ExecuteAsync(new GetConversionHistoryRequest(null, null, null, null, 1, 101));
            Assert.True(result.IsFailure);
            Assert.Equal("VALIDATION_ERROR", result.Error?.Code);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUseCache_WhenSameRequestIsCalledTwice()
        {
            var records = new List<ConversionRecord>
            {
                new ConversionRecord("BRL", "EUR", 100m, 16.67m, 6m, ExchangeQuotationEnum.Buy, new DateOnly(2025, 8, 13), "BACEN")
            };

            _repositoryMock.Setup(r => r.GetHistoryAsync("BRL", "EUR", null, null, 1, 20))
                .ReturnsAsync((records, 1));

            var request = new GetConversionHistoryRequest("brl", "eur", null, null, 1, 20);

            var first = await _useCase.ExecuteAsync(request);
            var second = await _useCase.ExecuteAsync(request);

            Assert.True(first.IsSuccess);
            Assert.True(second.IsSuccess);
            Assert.Single(first.Value!.Items);
            Assert.Single(second.Value!.Items);
            _repositoryMock.Verify(r => r.GetHistoryAsync("BRL", "EUR", null, null, 1, 20), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_WhenItemDoesNotExist()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ConversionRecord?)null);

            var result = await _useCase.GetByIdAsync(Guid.NewGuid());

            Assert.True(result.IsFailure);
            Assert.Equal("NOT_FOUND", result.Error?.Code);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldMapItem_WhenItemExists()
        {
            var id = Guid.NewGuid();
            var record = new ConversionRecord("BRL", "USD", 100m, 20m, 5m, ExchangeQuotationEnum.Sell, new DateOnly(2025, 8, 14), "BACEN");

            _repositoryMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(record);

            var result = await _useCase.GetByIdAsync(id);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("USD", result.Value!.ToCurrency);
            Assert.Equal(ExchangeQuotationEnum.Sell, result.Value.ExchangeType);
            Assert.Equal("BACEN", result.Value.Provider);
        }
    }
}
