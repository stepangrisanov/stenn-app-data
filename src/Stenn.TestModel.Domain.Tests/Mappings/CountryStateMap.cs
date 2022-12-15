using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.TestModel.Domain.Tests.Entities;
using Stenn.TestModel.Domain.Tests.Entities.Declarations;

namespace Stenn.TestModel.Domain.Tests.Mappings
{
    internal sealed class CountryStateMap : IEntityTypeConfiguration<CountryState>
    {
        public void Configure(EntityTypeBuilder<CountryState> builder)
        {
            builder.ToTable("CountryStates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsUnicode(false).IsFixedLength().HasMaxLength(2).IsRequired();
            builder.Property(x => x.CountryId).IsRequired();
            builder.HasOne(x => x.Country).WithMany();

            builder.HasData(CountryStateDeclaration.GetActual());
        }
    }
}