
namespace SurveyBasket.Api.Persistance.EntitiesConfigrations;

public class VoteConfigrations : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        //User can't have the same 2 polls voting 
        builder.HasIndex(p => new { p.PollId , p.UserId }).IsUnique();
    }
}
