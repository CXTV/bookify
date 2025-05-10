using Bookify.Application.Abstractions.Authentications;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //DataNow服务提供
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        //邮件服务
        services.AddTransient<IEmailService, EmailService>();

        AddPersistence(services, configuration);

        AddAuthentication(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        //数据库连接字符串
        string connectionString =
            configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));
        //数据库Snake命名法
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
        //User的Repository
        services.AddScoped<IUserRepository, UserRepository>();
        //Apartment的Repository
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        //Booking的Repository
        services.AddScoped<IBookingRepository, BookingRepository>();
        //UnitOfWork
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        //Dapper的连接服务
        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));
        //Dapper DateOnly类型处理器
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        //配置 JWT Bearer Token 认证机制
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        //配置 JWT Bearer Token 认证选项
        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));
        //配置WT Bearer Token 
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        //配置Keycloak选项
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));
        //配置Keycloak的身份验证服务
        services.AddTransient<AdminAuthorizationDelegatingHandler>();
        //
        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
            {
                KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
            })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();



        //services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        //{
        //    var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

        //    httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        //});
    }
}
