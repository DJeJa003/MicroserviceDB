using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
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

        public PricesController(MyDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _electricityDataUrl = configuration.GetValue<string>("ExternalServiceUrls:ElectricityDataUrl");
        }


        //[HttpPost("AddSpotPrice")]
        //public async Task<IActionResult> AddSpotPrice([FromBody] Prices spotPrice)
        //{
        //    try
        //    {
        //        _context.Prices.Add(spotPrice);
        //        await _context.SaveChangesAsync();
        //        return Ok("Spot price added successfully.");
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, $"Error adding spot price: {e.Message}");
        //    }
        //}


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


        [HttpPost("FetchAndSaveElectricityData")]
        public async Task<IActionResult> FetchAndSaveElectricityData()
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
                            EndDate = DateTime.Parse(entry.EndDate)
                        };

                        if (!_context.Prices.Any(p => p.PriceValue == spotPrice.PriceValue && p.StartDate == spotPrice.StartDate && p.EndDate == spotPrice.EndDate))
                        {
                            _context.Prices.Add(spotPrice);
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





        public class ElectricityData
        {
            public List<ElectricityPriceData> Prices { get; set; }
        }
        public class ElectricityPriceData : BaseEntity
        {
            public decimal Price { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            // Add other properties if needed
        }


    }
}

