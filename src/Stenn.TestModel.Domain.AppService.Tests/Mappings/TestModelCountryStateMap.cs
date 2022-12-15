using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.Domain.AppService.Tests.Mappings
{
    internal sealed class TestModelCountryStateMap : IEntityTypeConfiguration<TestModelCountryState>
    {
        public void Configure(EntityTypeBuilder<TestModelCountryState> builder)
        {
            builder.ToTable("CountryStates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsUnicode(false).IsFixedLength().HasMaxLength(2).IsRequired();
            builder.Property(x => x.CountryId).IsRequired();
            builder.HasOne(x => x.Country).WithMany();
        }
    }
}