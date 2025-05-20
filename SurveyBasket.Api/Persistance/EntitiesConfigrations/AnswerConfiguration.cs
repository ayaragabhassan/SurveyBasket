
namespace SurveyBasket.Api.Persistance.EntitiesConfigrations;

public class AnswerConfigrations : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasIndex(p => new { p.QuestionId , p.Content }).IsUnique();
        builder.Property(p => p.Content).HasMaxLength(1000);

    }
}
