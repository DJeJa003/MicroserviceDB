using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroserviceDB.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _electricityDataUrl;
        private ILogger<PricesController> _logger;

        public PricesController(MyDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PricesController> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _electricityDataUrl = configuration.GetValue<string>("ExternalServiceUrls:ElectricityDataUrl");
            _logger = logger;
        }


        [HttpGet("GetPricesFromRange")]
        public async Task<IActionResult> GetPricesFromRange([FromQuery] DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            if(startDate == null || endDate == null)
            {
                return BadRequest("Please set correct dates");
            }

            try
            {
                var prices = await _context.Prices
                    .Where(x => x.StartDate >= startDate && x.EndDate <= endDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(prices);
            }

            catch (Exception) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Virhe tietoja haettaessa");
                throw;
            }
        }


        [HttpGet("GetPriceSumAndDifference")]
        public async Task<IActionResult> GetPriceAndSumDifference([FromQuery] DateTime? startDate, DateTime? endDate, decimal? fixedPrice)
        {
            if(startDate == null || endDate == null || fixedPrice == null)
            {
                return BadRequest("Please set correct dates and a price");
            }

            try
            {
                var prices = await _context.Prices
                    .Where(x => x.StartDate >= startDate && x.EndDate <= endDate)
                    .ToListAsync();

                if (prices.Count == 0)
                {
                    return NotFound("No prices found within the specified dates");
                }

                var comparer = new Comparer();
                var sum = comparer.CalculateSum(prices);
                var difference = comparer.CalculateDifference(sum, fixedPrice, prices.Count);

                return Ok(new { SpotPriceSum = sum, DifferenceToFixed = difference });
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while calculating price sum and difference.");
                throw;
            }
        }


        [HttpGet("GetPriceDifferenceHourly")]
        public async Task<IActionResult> GetPriceDifferenceHourly([FromQuery] DateTime? startDate, DateTime? endDate, decimal? price)
        {
            if(startDate == null || endDate == null || price == null) 
            { 
                return BadRequest("Please set correct dates and a price"); 
            }

            try
            {
                var prices = await _context.Prices
                    .Where(x => x.StartDate >= startDate && x.EndDate <= endDate)
                    .ToListAsync();

                if (prices.Count == 0)
                {
                    return NotFound("No prices found within the specified dates");
                }

                var comparer = new Comparer();
                var priceDifference = comparer.ComparePrices(prices, price);

                return Ok(priceDifference);
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while fetching price differences.");
                throw;
            }
        }

   
        [HttpPost("GetAndSaveElectricityData")]
        public async Task<IActionResult> GetAndSaveElectricityData()
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var response = await httpClient.GetAsync(_electricityDataUrl);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var electricityData = JsonConvert.DeserializeObject<ElectricityData>(content);

                    foreach (var entry in electricityData.Prices)
                    {
                        var spotPrice = new Prices
                        {
                            PriceValue = entry.Price,
                            StartDate = DateTime.Parse(entry.StartDate),
                            EndDate = DateTime.Parse(entry.EndDate),
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        };

                        if (!_context.Prices.Any(p => p.PriceValue == spotPrice.PriceValue && p.StartDate == spotPrice.StartDate && p.EndDate == spotPrice.EndDate))
                        {
                            _context.Prices.Add(spotPrice);
                            UpdateEntityDates(spotPrice);
                            await _context.SaveChangesAsync();
                        }
                    }

                    return Ok("Data fetched and saved successfully.");
                }
            }

            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Error fetching and saving data: {e.Message}");
            }
        }


        [HttpDelete("DeleteDuplicatePrices")]
        public async Task<IActionResult> DeleteDuplicatePrices()
        {
            try
            {
                var duplicateEntries = _context.Prices
                    .GroupBy(p => new { p.PriceValue, p.StartDate, p.EndDate })
                    .Where(g => g.Count() > 1)
                    .Select(g => new { Key = g.Key, Count = g.Count() })
                    .ToList();


                foreach (var entry in duplicateEntries)
                {
                    var duplicatesToDelete = _context.Prices
                        .Where(p => p.PriceValue == entry.Key.PriceValue && p.StartDate == entry.Key.StartDate && p.EndDate == entry.Key.EndDate)
                        .OrderByDescending(p => p.Id)
                        .Skip(entry.Count - 1);

                    _context.Prices.RemoveRange(duplicatesToDelete);
                }

                await _context.SaveChangesAsync();

                return Ok("Duplicate prices deleted successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error deleting duplicate prices: {e.Message}");
            }
        }

        private void UpdateEntityDates(BaseEntity entity)
        {
            entity.UpdatedDate = DateTime.Now;
        }

        public class ElectricityData
        {
            public List<ElectricityPriceData> Prices { get; set; }
        }

        public class ElectricityPriceData : BaseEntity
        {
            public decimal Price { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
        }
    }
}

