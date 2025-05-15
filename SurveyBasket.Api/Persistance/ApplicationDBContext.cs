
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace SurveyBasket.Api.Persistance;

public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : 
    IdentityDbContext<ApplicationUser>(options) // send options to Base class "DBContext"

{
    public DbSet<Poll> Polls {  get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //For each configration Class this line will be executed
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
