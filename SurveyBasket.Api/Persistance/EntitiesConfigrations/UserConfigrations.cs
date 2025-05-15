
namespace SurveyBasket.Api.Persistance.EntitiesConfigrations;

public class UserConfigrations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(p => p.FirstName).HasMaxLength(100);
        builder.Property(p => p.LastName).HasMaxLength(100);
    }
}
