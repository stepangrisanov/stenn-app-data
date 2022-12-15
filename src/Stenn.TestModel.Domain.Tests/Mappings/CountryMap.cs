using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.TestModel.Domain.Tests.Entities;
using Stenn.TestModel.Domain.Tests.Entities.Declarations;

namespace Stenn.TestModel.Domain.Tests.Mappings
{
    internal sealed class CountryMap : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsUnicode(false).IsFixedLength().HasMaxLength(2).IsRequired();
            builder.Property(x => x.Alpha3Code).IsUnicode(false).IsFixedLength().HasMaxLength(3);
            builder.Property(x => x.Numeric3Code).IsUnicode(false).IsFixedLength().HasMaxLength(3);
            builder.Property(x => x.Name).HasMaxLength(80);

            builder.HasData(CountryDeclaration.GetActual());
        }
    }
}