using Exchange.API.Extensions;
using Exchange.Application.Dtos.Requests;
using Exchange.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly IConvertCurrencyUseCase _convertCurrencyUseCase;
        private readonly IGetConversionHistoryUseCase _getConversionHistoryUseCase;
        private readonly IGetExchangeRateUseCase _getExchangeRateUseCase;
        private readonly IGetSupportedCurrenciesUseCase _getSupportedCurrenciesUseCase;

        public CurrencyController(
            IConvertCurrencyUseCase convertCurrencyUseCase,
            IGetConversionHistoryUseCase getConversionHistoryUseCase,
            IGetExchangeRateUseCase getExchangeRateUseCase,
            IGetSupportedCurrenciesUseCase getSupportedCurrenciesUseCase)
        {
            _convertCurrencyUseCase = convertCurrencyUseCase;
            _getConversionHistoryUseCase = getConversionHistoryUseCase;
            _getExchangeRateUseCase = getExchangeRateUseCase;
            _getSupportedCurrenciesUseCase = getSupportedCurrenciesUseCase;
        }

        [HttpPost("convert")]
        public async Task<IActionResult> Convert([FromBody] ConvertCurrencyRequest request)
        {
            var result = await _convertCurrencyUseCase.ExecuteAsync(request);
            return this.ToActionResult(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] string? fromCurrency,
            [FromQuery] string? toCurrency,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var request = new GetConversionHistoryRequest(fromCurrency, toCurrency, startDate, endDate, page, pageSize);
            var history = await _getConversionHistoryUseCase.ExecuteAsync(request);
            return this.ToActionResult(history);
        }

        [HttpGet("history/{id:guid}")]
        public async Task<IActionResult> GetHistoryById(Guid id)
        {
            var item = await _getConversionHistoryUseCase.GetByIdAsync(id);
            return this.ToActionResult(item);
        }

        [HttpGet("rate")]
        public async Task<IActionResult> GetRate([FromQuery] string currency, [FromQuery] DateOnly dateQuotation)
        {
            var result = await _getExchangeRateUseCase.ExecuteAsync(currency, dateQuotation);
            return this.ToActionResult(result);
        }

        [HttpGet("supported")]
        public async Task<IActionResult> GetSupportedCurrencies()
        {
            var currencies = await _getSupportedCurrenciesUseCase.ExecuteAsync();
            return this.ToActionResult(currencies);
        }
    }
}
