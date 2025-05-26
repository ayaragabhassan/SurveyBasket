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

//app.UseOutputCache();
app.UseCors();

app.UseAuthorization();

//Built in Endpoints for Identity Functions
//app.MapIdentityApi<ApplicationUser>();

app.MapControllers();
app.UseExceptionHandler();
app.Run();
