using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace web.IoC
{
    public static class MvcExtensions
    {
        public static IServiceCollection AddConfiguredMvc(this IServiceCollection services) 
        {
            services
                .AddControllersWithViews();
            services
                .AddRazorPages(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account");
                    options.Conventions.AuthorizeFolder("/Consent");
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            return services;
        }
    }
}
