namespace Stenn.TestModel.Domain.Tests.Entities
{
    /// <summary>
    /// US country state
    /// </summary>
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicConstructors)]
#endif
    public class CountryState
    {
        protected CountryState()
        {
        }

        public CountryState(string id, string description, bool isAvailable, string countryAlpha2Code)
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

        public virtual Country Country { get; init; }

        public bool IsAvailable { get; init; }
    }
}