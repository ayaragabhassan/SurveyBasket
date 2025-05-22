namespace SurveyBasket.Api.Entities;

public sealed class Poll : AuditableEntity //becuse we didn't inherit from it 
{
    public int Id { get; set; }
    
    public string Title { get; set; }  = string.Empty;  
    
    public string Summery { get; set; } =string.Empty;
    
    public bool IsPublished { get; set; }
    
    public DateOnly StartedAt { get; set; }
    
    public DateOnly EndAt { get; set; }

    public ICollection<Question> Questions { get; set; } = [];
    public ICollection<Vote> Votes { get; set; } = [];

}
