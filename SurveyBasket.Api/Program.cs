using Hangfire;
using HangfireBasicAuthenticationFilter;
using Serilog;
using SurveyBasket;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//    .AddEntityFrameworkStores<ApplicationDBContext>();

builder.Host.UseSerilog((context, configuration) =>
{
    //configuration
    //.MinimumLevel.Information()
    //.WriteTo.Console();
    configuration.ReadFrom.Configuration(context.Configuration);
}
);
//Add Caching 
//builder.Services.AddResponseCaching();
//builder.Services.AddOutputCache(options =>
//{
//    options.AddPolicy("Polls", x => 
//           x.Cache()
//           .Expire(TimeSpan.FromSeconds(60))
//           .Tag("AvailableQuestions"));
//});

//builder.Services.AddMemoryCache();

builder.Services.AddDependencies(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
     // app.MapOpenApi();
    //app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ],
    DashboardTitle = "Survey Basket Dashboard",
    //IsReadOnlyFunc = (DashboardContext conext) => true  to disable detete or select task
});



var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendNewPollsNotification(null), Cron.Daily);

//app.UseOutputCache();
app.UseCors();

app.UseAuthorization();

//Built in Endpoints for Identity Functions
//app.MapIdentityApi<ApplicationUser>();

app.MapControllers();
app.UseExceptionHandler();

app.Run();
