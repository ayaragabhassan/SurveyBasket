
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.Api.Persistance;

public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options
    , IHttpContextAccessor httpContextAccessor) :
    IdentityDbContext<ApplicationUser>(options) // send options to Base class "DBContext"

{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Poll> Polls { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //For each configration Class this line will be executed
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys()
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership));
        foreach ( var fk in cascadeFKs )
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        var currentUser = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(e => e.CreatedById).CurrentValue = currentUser; //Assign User Id 
                entry.Property(e => e.CreatedOn).CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.UpdatedById).CurrentValue = currentUser;
                entry.Property(e => e.UpdatedOn).CurrentValue = DateTime.UtcNow;

            }
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
