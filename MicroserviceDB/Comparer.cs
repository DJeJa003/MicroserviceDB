using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceDB
{
    public enum ElectricityContractType
    {
        FixedPrice,
        MarketPrice
    }
    public class Comparer
    {
        public List<PriceDifference> ComparePrices(List<Prices> exchangePrices, decimal? fixedPrice)
        {
            if (!fixedPrice.HasValue)
            {
                // Handle the case when fixedPrice is null (if needed)
                throw new ArgumentException("Fixed price cannot be null");
            }

            List<PriceDifference> differences = new List<PriceDifference>();

            foreach (var exchangePrice in exchangePrices)
            {
                decimal? exchangePriceValue = exchangePrice.PriceValue;
                if (exchangePriceValue.HasValue)
                {
                    decimal differenceValue = Math.Abs(exchangePriceValue.Value - fixedPrice.Value);
                    ElectricityContractType cheaperContract = exchangePriceValue < fixedPrice
                        ? ElectricityContractType.MarketPrice
                        : ElectricityContractType.FixedPrice;

                    differences.Add(new PriceDifference(differenceValue, exchangePrice.StartDate, exchangePrice.EndDate, cheaperContract));
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }

            return differences;
        }



        public List<Prices> DeserializeJson(List<Prices> prices)
        {
            return prices;
        }
    }
}
