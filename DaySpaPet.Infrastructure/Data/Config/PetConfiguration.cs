using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaySpaPet.Core.PetAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaySpaPet.Infrastructure.Data.Config;

public sealed class PetConfiguration : IEntityTypeConfiguration<Pet>
{
  public void Configure(EntityTypeBuilder<Pet> builder)
  {
    builder.Property(p => p.ClientId)
      .IsRequired();
    builder.Property(p => p.Name)
      .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
      .IsRequired();
    builder.Property(p => p.Type)
      .HasMaxLength(DataSchemaConstants.DEFAULT_ENUM_STRING_LENGTH)
      .IsRequired();
    builder.Property(p => p.Breed)
      .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
      .IsRequired();
    builder.Property(p => p.Status)
      .HasMaxLength(DataSchemaConstants.DEFAULT_ENUM_STRING_LENGTH)
      .IsRequired();
  }
}
