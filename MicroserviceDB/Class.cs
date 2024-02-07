namespace MicroserviceDB
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TestAttribute : Attribute
    {
        public string Message { get; }

        public TestAttribute(string message)
        {
            Message = message;
        }
    }
    public class PriceDifference
    {
        public decimal PriceDifferenceValue { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ElectricityContractType CheaperContract { get; set; }

        public PriceDifference(decimal priceDifference, DateTime startDate, DateTime endDate, ElectricityContractType cheaperContract)
        {
            PriceDifferenceValue = priceDifference;
            StartDate = startDate;
            EndDate = endDate;
            CheaperContract = cheaperContract;
        }
    }
}
