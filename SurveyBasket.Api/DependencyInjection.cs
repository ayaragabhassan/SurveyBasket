using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Persistance;
using SurveyBasket.Api.Settings;
using SurveyBasket.Services;
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
        services.AddHybridCache();
        services.AddCorsConfg(configuration);

        services.AddAuthorizationConfig(configuration);

        services.AddDBContextConfig(configuration);

        services
            .AddSwagerConfig()
            .AddMapsterConfig()
            .AddFluentValidationConfig();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IPollService, PollService>();

        services.AddScoped<IQuestionService, QuestionService>();

        services.AddScoped<IVoteService, VoteService>();

        services.AddScoped<IResultService, ResultService>();

        services.AddScoped<IEmailSender, EmailService>();


        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        services.AddHttpContextAccessor();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }

    private static IServiceCollection AddSwagerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        //AddMapster
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(implementationInstance: new Mapper(mappingConfig));

        services.AddMapster();

        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        //Instide of writing the same line for each validation  builder.Services.AddScoped<IValidator<Poll>,CreatePollRequestValidator>();
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
    private static IServiceCollection AddDBContextConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var connecrionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("connesction string not found !");

        services.AddDbContext<ApplicationDBContext>(options =>
        options.UseSqlServer(connecrionString));
        return services;
    }
    private static IServiceCollection AddAuthorizationConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();
        ;

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

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;


        });

        return services;
    }
    private static IServiceCollection AddCorsConfg(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()!;

        //services.AddCors(Options =>
        //{
        //    Options.AddPolicy("AllowedPolicy",
        //        builder => builder.AllowAnyHeader()
        //                .AllowAnyMethod()
        //                .WithOrigins(allowedOrigins));
        //});
        services.AddCors(Options =>
        {
            Options.AddDefaultPolicy(builder =>
                 builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(allowedOrigins));
        });
        return services;
    }

}
