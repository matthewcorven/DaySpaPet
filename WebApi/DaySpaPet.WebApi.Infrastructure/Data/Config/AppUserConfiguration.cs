using DaySpaPet.WebApi.Core.AppUserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaySpaPet.Infrastructure.Data.Config;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser> {
  public void Configure(EntityTypeBuilder<AppUser> builder) {
    builder.HasKey(c => c.Id);

    builder.Property(u => u.PasswordHash).IsRequired();
    
    builder.Property(u => u.PasswordSalt).IsRequired();
    
    builder.Property(u => u.HashingAlgorithm).IsRequired();
    
    builder.Property(u => u.Username).IsRequired().HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    builder.HasIndex(u => u.Username).IsUnique();
    
    builder.Property(u => u.EmailAddress).IsRequired().HasMaxLength(DataSchemaConstants.DEFAULT_EMAIL_LENGTH);
    builder.HasIndex(u => u.EmailAddress).IsUnique();

    builder.Property(u => u.TimeZoneId).IsRequired().HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    builder.Property(u => u.Locale).IsRequired().HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    builder.Property(u => u.Currency).IsRequired().HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    builder.Property(u => u.FirstName).IsRequired().HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    builder.Property(u => u.LastName).IsRequired().HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    
    builder.Property(c => c.MiddleName).HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    builder.Property(c => c.DateOfBirth);

    builder.Property(c => c.ProfileImageUrl).HasMaxLength(DataSchemaConstants.DEFAULT_URL_LENGTH);

    builder.Property(c => c.PhoneNumber).HasMaxLength(DataSchemaConstants.DEFAULT_PHONE_LENGTH);

    builder.Property(c => c.AddressLine1).HasMaxLength(DataSchemaConstants.DEFAULT_ADDRESS_LINE_LENGTH);

    builder.Property(c => c.AddressLine2).HasMaxLength(DataSchemaConstants.DEFAULT_ADDRESS_LINE_LENGTH);
    
    builder.Property(c => c.State).HasMaxLength(DataSchemaConstants.DEFAULT_SHORT_NAME_LENGTH);
    
    builder.Property(c => c.PostalCode).HasMaxLength(DataSchemaConstants.DEFAULT_POSTAL_CODE_LENGTH);
    
    builder.Property(c => c.CountryCode).HasMaxLength(DataSchemaConstants.DEFAULT_SHORT_NAME_LENGTH);
    
    builder.Property(c => c.CreatedAtServerInstantUtc)
            .IsRequired();
    builder.Property(c => c.CreatedAtDaylightSavingTime)
            .IsRequired();
    builder.Property(c => c.CreatedAtTimeZoneId)
            .IsRequired();
    builder.Property(c => c.CreatedAtOriginLocalDateTime)
            .IsRequired();

    builder.Property(c => c.ModifiedAtServerInstantUtc);
    builder.Property(c => c.ModifiedAtDaylightSavingTime);
    builder.Property(c => c.ModifiedAtTimeZoneId);
    builder.Property(c => c.ModifiedAtOriginLocalDateTime);

    builder.HasMany(c => c.UserRoles)
            .WithOne()
            .HasForeignKey(p => p.AppUserId);
  }
}