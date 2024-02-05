using Microsoft.AspNetCore.Mvc;
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

        //[HttpGet("GetSahko")]
        //public IActionResult GetSpotPrice()
        //{
        //    var spotPrices = _context.Prices.ToList();
        //    return Ok(spotPrices);
        //}

        [HttpPost("AddSpotPrice")]
        public async Task<IActionResult> AddSpotPrice([FromBody] Prices spotPrice)
        {
            try
            {
                _context.Prices.Add(spotPrice);
                await _context.SaveChangesAsync();
                return Ok("Spot price added successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error adding spot price: {e.Message}");
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

                    // Deserialize the received content to your ElectricityPriceData model
                    var electricityData = JsonConvert.DeserializeObject<ElectricityData>(content);

                    // Iterate through each entry in the "prices" array
                    foreach (var entry in electricityData.Prices)
                    {
                        // Map the ElectricityPriceData to your Prices model for each entry
                        var spotPrice = new Prices
                        {
                            PriceValue = entry.Price,
                            StartDate = DateTime.Parse(entry.StartDate),
                            EndDate = DateTime.Parse(entry.EndDate)
                            // You may need additional mapping based on your model
                        };

                        // Save the spotPrice to the database
                        _context.Prices.Add(spotPrice);
                        await _context.SaveChangesAsync();
                    }

                    return Ok("Data fetched and saved successfully.");
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Error fetching and saving data: {e.Message}");
            }
        }

        //public async Task<IActionResult> FetchAndSaveElectricityData()
        //{
        //    try
        //    {
        //        using (var httpClient = _httpClientFactory.CreateClient())
        //        {
        //            var response = await httpClient.GetAsync(_electricityDataUrl);
        //            response.EnsureSuccessStatusCode();

        //            var content = await response.Content.ReadAsStringAsync();

        //            // Deserialize the received content to your ElectricityPriceData model
        //            var electricityData = JsonConvert.DeserializeObject<ElectricityPriceData>(content);

        //            // Map the ElectricityPriceData to your Prices model
        //            //var spotPrice = new Prices
        //            //{
        //            //    PriceValue = electricityData.Price,
        //            //    StartDate = electricityData.StartDate != null ? DateTime.Parse(electricityData.StartDate) : DateTime.MinValue,
        //            //    EndDate = electricityData.EndDate != null ? DateTime.Parse(electricityData.EndDate) : DateTime.MinValue
        //            //};
        //            var spotPrice = new Prices
        //            {
        //                PriceValue = electricityData.Price,
        //                StartDate = DateTime.TryParseExact(electricityData.StartDate, "dd.MM.yyyy HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate) ? startDate : DateTime.MinValue,
        //                EndDate = DateTime.TryParseExact(electricityData.EndDate, "dd.MM.yyyy HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate) ? endDate : DateTime.MinValue
        //                // You may need additional mapping based on your model
        //            };






        //            // Save the spotPrice to the database
        //            _context.Prices.Add(spotPrice);
        //            await _context.SaveChangesAsync();

        //            return Ok("Data fetched and saved successfully.");
        //        }
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        return StatusCode(500, $"Error fetching and saving data: {e.Message}");
        //    }
        //}

        public class ElectricityData
        {
            public List<ElectricityPriceData> Prices { get; set; }
        }
        public class ElectricityPriceData
        {
            public decimal Price { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            // Add other properties if needed
        }


    }
}

