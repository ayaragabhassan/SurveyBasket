using System.Reflection;

namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers();

        services
            .AddSwagerServices()
            .AddMapsterServices()
            .AddFluentValidationServices();


        services.AddScoped<IPollService, PollService>();

       
        return services;
    }

    public static IServiceCollection AddSwagerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddMapsterServices(this IServiceCollection services)
    {
        //AddMapster
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(implementationInstance: new Mapper(mappingConfig));

        services.AddMapster();

        return services;
    }

    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        //Instide of writing the same line for each validation  builder.Services.AddScoped<IValidator<Poll>,CreatePollRequestValidator>();
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

}
