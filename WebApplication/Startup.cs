using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Controllers;

namespace WebApplication {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<DemoDbContext>(o => o.UseInMemoryDatabase("InMemoryDb"));

            var context = services.BuildServiceProvider().GetService<DemoDbContext>();
            SeedDemoData(context);

            //Load global configuration from database and bind them.
            var config = LoadGlobalConfiguration(context);
            var mySection = config.GetSection($"{nameof(DemoOptions)}:{DemoOptions.Global}");
            services.Configure<DemoOptions>(c => mySection.Bind(c));
            services.AddSingleton(config);


            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IOptionsManager<DemoOptions>, DemoOptionsManager>();

            services.AddScoped(typeof(ITestManager<>), typeof(TestManager<>));


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews().ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new GenericControllerFeatureProvider()));
        }



        public static IConfiguration LoadGlobalConfiguration(DemoDbContext context) {
            var builder = new ConfigurationBuilder();
            builder.Sources.Clear();

            var settings = context.DemoSettings.Select(s => new { s.Key, s.Value }).ToDictionary(s => s.Key, s => s.Value);
            builder.AddInMemoryCollection(settings);

            return builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void SeedDemoData(DemoDbContext context) {
            context.DemoSettings.AddRange(new List<DemoSettings>() {
                 new DemoSettings("DemoOptions:Global:Enabled", "true"){Id=1},
                 new DemoSettings("DemoOptions:Global:AutoRetryDelay", "14:58:00"){Id=2},
                 new DemoSettings("DemoOptions:Global:IdentityOptions:MaxUserNameLength", "5"){Id=3},
            });

            context.TenantSettings.AddRange(new List<TenantSettings>() {
                 new TenantSettings("DemoOptions:Tenant:Enabled", "false",1){Id=1},
                 new TenantSettings("DemoOptions:Tenant:AutoRetryDelay", "17:58:00",1){Id=2},
                 new TenantSettings("DemoOptions:Tenant:IdentityOptions:MaxUserNameLength", "5",1){Id=3},

                 new TenantSettings("DemoOptions:Tenant:Enabled", "true",2){Id=4},
                 new TenantSettings("DemoOptions:Tenant:AutoRetryDelay", "12:58:00",2){Id=5},
                 new TenantSettings("DemoOptions:Tenant:IdentityOptions:MaxUserNameLength", "8",2){Id=6},
            });

            context.SaveChanges();
        }
    }



}
