using DaySpaPet.WebApi.Core.ClientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaySpaPet.Infrastructure.Data.Config;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client> {
  public void Configure(EntityTypeBuilder<Client> builder) {
    builder.HasKey(c => c.Id);
    builder.HasIndex(u => u.EmailAddress).IsUnique();
    builder.Property(c => c.FirstName)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();
    builder.Property(c => c.LastName)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();
    builder.Property(c => c.EmailAddress)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    builder.Property(c => c.PhoneNumber)
            .IsRequired();
    builder.Property(p => p.Status)
            .HasConversion(
                            p => p.Value,
                            p => ClientAccountStatus.FromValue(p))
            .IsRequired();
    builder.Property(c => c.CreatedAtServerInstantUtc)
            .IsRequired();
    builder.Property(c => c.CreatedAtDaylightSavingTime)
            .IsRequired();
    builder.Property(c => c.CreatedAtTimeZoneId)
            .IsRequired();
    builder.Property(c => c.CreatedAtOriginLocalDateTime)
            .IsRequired();
    builder.HasMany(c => c.Pets)
            .WithOne()
            .HasForeignKey(p => p.ClientId);
  }
}