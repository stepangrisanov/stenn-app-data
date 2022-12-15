using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.Domain.AppService.Tests.Mappings
{
    internal sealed class TestModelCountryMap : IEntityTypeConfiguration<TestModelCountry>
    {
        public void Configure(EntityTypeBuilder<TestModelCountry> builder)
        {
            builder.ToTable("Countries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsUnicode(false).IsFixedLength().HasMaxLength(2).IsRequired();
            builder.Property(x => x.Alpha3Code).IsUnicode(false).IsFixedLength().HasMaxLength(3);
            builder.Property(x => x.Numeric3Code).IsUnicode(false).IsFixedLength().HasMaxLength(3);
            builder.Property(x => x.Name).HasMaxLength(80);
        }
    }
}