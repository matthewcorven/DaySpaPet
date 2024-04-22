using DaySpaPet.Core.ClientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaySpaPet.Infrastructure.Data.Config;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
  public void Configure(EntityTypeBuilder<Client> builder)
  {
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
    builder.Property(c => c.Status)
      .HasMaxLength(DataSchemaConstants.DEFAULT_ENUM_STRING_LENGTH)
      .IsRequired();
  }
}
