using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoApi.Models;
using ToDoApi.Service;
using ToDoApi.Validation;

namespace ToDoApi.Config
{
    public static class ApplicationServiceConfiguration
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IValidator<Team>, TeamValidation>();
            services.AddScoped<IValidator<Player>, PlayerValidation>();
            services.AddScoped<IToDoApiService, ToDoApiService>();
        }
    }
}
