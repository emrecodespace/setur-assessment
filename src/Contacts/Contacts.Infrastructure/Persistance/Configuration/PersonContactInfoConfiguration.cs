using Contacts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contacts.Infrastructure.Persistance.Configuration;

public class PersonContactInfoConfiguration : IEntityTypeConfiguration<PersonContactInfo>
{
    public void Configure(EntityTypeBuilder<PersonContactInfo> builder)
    {
        builder.ToTable("PersonContactInfos");
        
        builder.HasKey(ci => ci.Id);
        
        builder.Property(ci => ci.InfoType)
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(ci => ci.Content)
            .IsRequired()
            .HasMaxLength(100);
    }
}