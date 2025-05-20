
namespace SurveyBasket.Api.Persistance.EntitiesConfigrations;

public class QuestionConfigrations : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(p=> new {p.PollId, p.Content }).IsUnique();
        builder.Property(p => p.Content).HasMaxLength(1000);

    }
}
