using ContactReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactReports.Infrastructure.Persistance.Configuration;

public class ReportDetailConfiguration : IEntityTypeConfiguration<ReportDetail>
{
    public void Configure(EntityTypeBuilder<ReportDetail> builder)
    {
        builder.ToTable("ReportDetails");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Location)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.PersonCount)
            .IsRequired();

        builder.Property(d => d.PhoneNumberCount)
            .IsRequired();
    }
}