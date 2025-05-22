
namespace SurveyBasket.Api.Persistance.EntitiesConfigrations;

public class VoteAnswerConfiguration : IEntityTypeConfiguration<VoteAnswer>
{
    public void Configure(EntityTypeBuilder<VoteAnswer> builder)
    {
        //User can't have the same 2 polls voting 
        builder.HasIndex(p => new { p.VoteId , p.QuestionId }).IsUnique();
    }
}
