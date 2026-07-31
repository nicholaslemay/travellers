using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Travellers.Trips;

public class TripRowConfiguration : IEntityTypeConfiguration<TripRow>
{
    public void Configure(EntityTypeBuilder<TripRow> builder)
    {
        builder.ToTable("trips");
        builder.HasKey(t => t.TripId);
        builder.Property(t => t.TripId).HasColumnName("trip_id").ValueGeneratedNever();
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");
    }
}
