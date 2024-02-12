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
        public decimal CalculateSum(List<Prices> exchangePrices)
        {
            decimal sum = 0;

            foreach (var exchangePrice in exchangePrices) 
            {
                if (exchangePrice.PriceValue != null)
                {
                    sum += exchangePrice.PriceValue;
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            return sum;
        }

        public decimal CalculateDifference(decimal sum, decimal? fixedPrice, int itemCount)
        {
            if (!fixedPrice.HasValue)
            {
                throw new ArgumentException("Fixed price can't be null");
            }
            decimal fixedPriceSum = fixedPrice.Value * itemCount;

            return sum - fixedPriceSum; 
        }
        public List<PriceDifference> ComparePrices(List<Prices> exchangePrices, decimal? fixedPrice)
        {
            if (!fixedPrice.HasValue)
            {
                throw new ArgumentException("Fixed price cannot be null");
            }

            List<PriceDifference> differences = new List<PriceDifference>();

            foreach (var exchangePrice in exchangePrices)
            {
                decimal? exchangePriceValue = exchangePrice.PriceValue;
                if (exchangePriceValue.HasValue)
                {
                    decimal differenceValue = exchangePriceValue.Value - fixedPrice.Value;
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
