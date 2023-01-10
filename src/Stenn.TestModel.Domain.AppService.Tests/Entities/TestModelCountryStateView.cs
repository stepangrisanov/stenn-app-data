namespace Stenn.TestModel.Domain.AppService.Tests.Entities
{
    public class TestModelCountryStateView : ITestModelEntity
    {
        public string StateId { get; set; }
        public string Description { get; set; }
        
        public string CountryName { get; set; }
        public string CountryAlpha2Code { get; set; }
        public string CountryAlpha3Code { get; set; }
        public string CountryNumeric3Code { get; set; }
    }
}