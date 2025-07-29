using ContactReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactReports.Infrastructure.Persistance.Configuration;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RequestedAt)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.HasMany(r => r.ReportDetails)
            .WithOne(d => d.Report)
            .HasForeignKey(d => d.ReportId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}