using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Claims;
using web.Data;
using web.Services;
using web.Utilities;

namespace web
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services
                .AddMvc(options => {
                    options.EnableEndpointRouting = false;
                })
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
                .AddRazorPagesOptions(options => {
                    options.Conventions.AuthorizeFolder("/Account");
                    options.Conventions.AuthorizePage("/Admin/ConventionPolicyProtected", "AdminPolicy");
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("nl")
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });


            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("AuthDbContext"));
                // options.EnableSensitiveDataLogging();
            });

            services.AddIdentity<PlayBallUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                //TODO: uncomment after some tests
                // options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options => {
                options.AddPolicy("AdminPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));
            });
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.AccessDeniedPath = "/AccessDenied";
            });

            services.AddSingleton<IEmailSender, DummyEmailSender>();
            services.AddSingleton<IBase64QrCodeGenerator, Base64QrCodeGenerator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();

        }
    }
}
