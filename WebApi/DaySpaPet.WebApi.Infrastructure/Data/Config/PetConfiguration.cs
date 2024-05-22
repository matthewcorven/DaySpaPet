using DaySpaPet.WebApi.Core;
using DaySpaPet.WebApi.Core.PetAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaySpaPet.Infrastructure.Data.Config;

public sealed class PetConfiguration : IEntityTypeConfiguration<Pet>
{
	public void Configure(EntityTypeBuilder<Pet> builder)
	{
		builder.HasKey(p => p.Id);
		builder.Property(p => p.ClientId)
				.IsRequired();
		builder.Property(p => p.Name)
				.HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
				.IsRequired();
		builder.Property(p => p.Type)
				.HasConversion(
								p => p.Value,
								p => AnimalType.FromValue(p))
				.IsRequired();
		builder.Property(p => p.Breed)
				.HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
				.IsRequired();
		builder.Property(p => p.Status)
				.HasConversion(
								p => p.Value,
								p => PetStatus.FromValue(p))
				.IsRequired();
		builder.Property(c => c.CreatedAtServerInstantUtc)
				.IsRequired();
		builder.Property(c => c.CreatedAtDaylightSavingTime)
				.IsRequired();
		builder.Property(c => c.CreatedAtTimeZoneId)
				.IsRequired();
		builder.Property(c => c.CreatedAtOriginLocalDateTime)
				.IsRequired();
	}
}
