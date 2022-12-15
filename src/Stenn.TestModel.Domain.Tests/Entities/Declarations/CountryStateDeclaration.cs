using System.Collections.Generic;
using Stenn.Csv;

namespace Stenn.TestModel.Domain.Tests.Entities.Declarations
{
    public static class CountryStateDeclaration
    {
        public static IEnumerable<CountryState> GetActual()
        {
            //NOTE: Generate new db migration after change csv file

            return CsvConverter.ReadCsvFromResFile<CountryStateData, CountryState>(@"Entities\Declarations\CountryStatesDeclaration.csv",
                data => new CountryState
                (
                    data.StateCode,
                    data.Description,
                    CsvConverter.Bool(data.IsAvailable),
                    CountryDeclaration.UsaId
                ));
        }
#if NET6_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
        private sealed class CountryStateData
        {
            public string Id { get; set; }
            public string StateCode { get; set; }
            public string Description { get; set; }
            public string IsAvailable { get; set; }
        }
    }
}