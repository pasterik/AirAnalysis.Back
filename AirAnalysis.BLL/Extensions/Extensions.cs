
using AirAnalysis.BLL.Mapping;
using AirAnalysis.BLL.Services.Fuzzy_LogicService;
using AirAnalysis.BLL.Services.Fuzzy_LogicService.Interfaces;
using AirAnalysis.BLL.Services.MLService;
using AirAnalysis.DAL.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace AirAnalysis.DAL.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddBLL(this IServiceCollection services, IConfiguration configuration)
        {
            var bllAssembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(bllAssembly);
            });

            services.AddSingleton<MlService>();
            services.AddScoped<IFuzzy_Logic, Fuzzy_Logic>();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<PhenomenProfile>();
                cfg.AddProfile<RecordDataProfile>();
                cfg.AddProfile<PlaceProfile>();
            });
            return services;

        }
    }
}
