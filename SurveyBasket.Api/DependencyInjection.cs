using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Persistance;
using System.Reflection;
using System.Text;

namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add services to the container.
        services.AddControllers();

        services.AddAuthorizationService(configuration);

        services.AddDBContextService(configuration);

        services
            .AddSwagerServices()
            .AddMapsterServices()
            .AddFluentValidationServices();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IPollService, PollService>();

       
        return services;
    }

    private static IServiceCollection AddSwagerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection AddMapsterServices(this IServiceCollection services)
    {
        //AddMapster
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(implementationInstance: new Mapper(mappingConfig));

        services.AddMapster();

        return services;
    }

    private static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        //Instide of writing the same line for each validation  builder.Services.AddScoped<IValidator<Poll>,CreatePollRequestValidator>();
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
    private static IServiceCollection AddDBContextService(this IServiceCollection services, IConfiguration configuration)
    {
        var connecrionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("connesction string not found !");

        services.AddDbContext<ApplicationDBContext>(options =>
        options.UseSqlServer(connecrionString));
        return services;
    }
    private static IServiceCollection AddAuthorizationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDBContext>();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddSingleton<IJwtProvider, JwtProvider>();


        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));


      
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience
            };
        });
        return services;
    }

}
