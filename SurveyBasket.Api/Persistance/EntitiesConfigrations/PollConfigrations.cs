
namespace SurveyBasket.Api.Persistance.EntitiesConfigrations;

public class PollConfigrations : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasIndex(p=>p.Title).IsUnique();
        builder.Property(p => p.Title).HasMaxLength(100);
        builder.Property(p => p.Summery).HasMaxLength(100);
    //    builder.Property(p => p.CreatedOn).HasDefaultValueSql("GETDATE()");

    //    builder.Property(p=>p.CreatedOn).HasDefaultValue(DateTime.UtcNow);
    //
    }
}
