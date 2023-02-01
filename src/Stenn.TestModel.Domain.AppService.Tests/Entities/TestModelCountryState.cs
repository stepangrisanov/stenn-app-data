namespace Stenn.TestModel.Domain.AppService.Tests.Entities
{
    /// <summary>
    /// US country state
    /// </summary>
    public class TestModelCountryState : ITestModelEntity
    {
        public TestModelCountryState()
        {
        }

        public TestModelCountryState(string id, string description, bool isAvailable, string countryAlpha2Code)
        {
            Id = id;
            Description = description;
            IsAvailable = isAvailable;
            CountryId = countryAlpha2Code;
        }

        /// <summary>
        /// State code
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// State name
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Country id
        /// </summary>
        public string CountryId { get; init; }

        public virtual TestModelCountry Country { get; init; }

        public bool IsAvailable { get; init; }
    }
}