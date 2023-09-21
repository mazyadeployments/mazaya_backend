using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MMA.WebApi.Bootstrap;
using MMA.WebApi.Extenstions;
using MMA.WebApi.Security;
using MMA.WebApi.Security.AntiXSS.Extensions;
using MMA.WebApi.Services;
using MMA.WebApi.Shared.Exceptions.Extensions;
using MMA.WebApi.Shared.Hubs;
using MMA.WebApi.Shared.Interfaces.Configuration;
using MyWebApi.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi
{
    public class Startup
    {
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var dataProtect = services.AddDataProtection();

            _logger.LogInformation("Configuring Telemetry.");

            services.AddApplicationInsightsTelemetry();

            _logger.LogInformation("Configuring Services.");

            services.AddTransient<IIdentityProviderService, IdentityProvidersService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddSingleton<IAuthorizationHandler, IsTokenExpiredHandler>();

            services.AddWebApi(_configuration);

            _logger.LogInformation("Configuring Authorization.");

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(
                    "Bearer",
                    new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                        .AddRequirements(new IsTokenExpiredRequirement())
                        .RequireAuthenticatedUser()
                        .Build()
                );
            });

            services.AddAzureActiveDirectoryAuthorization(_configuration);

            _logger.LogInformation("Configuring Authentication.");

            services
                .AddAuthentication(
                    options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme
                )
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
                        ) //Configuration["Tokens:Key"]
                    };

                    cfg.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (
                                !string.IsNullOrEmpty(accessToken)
                                && (path.StartsWithSegments("/notificationHub"))
                            )
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            _logger.LogInformation("Configuring Cors.");
            services.AddCors(
                o =>
                    o.AddPolicy(
                        "CorsPolicy",
                        builder =>
                        {
                            // TODO: Allow specific origin, depends from environment
                            // var someCondition = "";
                            builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
                            //.SetIsOriginAllowed(origin =>
                            //{
                            //    if(origin == someCondition) return true;
                            //    return false;
                            //})
                            ;
                        }
                    )
            );

            services.AddSignalR();
            services
                .AddMvc(o =>
                {
                    o.UseGeneralRoutePrefix("api/");
                })
                .AddMvcOptions(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(
                    setup =>
                        setup.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                            .Json
                            .ReferenceLoopHandling
                            .Ignore
                );

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            _logger.LogInformation("Configuring Cache.");

            services.AddMemoryCache();

            services.AddHangfire(
                x => x.UseSqlServerStorage(_configuration["ConnectionStrings:Database"])
            );
            services.AddHangfireServer();

            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.ValueCountLimit = 10; //default 1024
                options.ValueLengthLimit = int.MaxValue; //not recommended value
                options.MultipartBodyLengthLimit = long.MaxValue; //not recommended value
            });

            _logger.LogInformation("Building Services.");

            var provider = services.BuildServiceProvider();

            _logger.LogInformation("Configuring services completed.");

            services.AddDataProtection().SetApplicationName("offers-app");

            return provider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _logger.LogInformation($"Environment {env.EnvironmentName}");

            _logger.LogInformation("Configuring Custom exception middleware.");
            app.ConfigureCustomExceptionMiddleware();

            _logger.LogInformation("Apply migrations and seed database.");
            app.SeedDatabase();
            _logger.LogInformation("Apply migrations and seed database completed.");

            //app.Use((ctx, next ) =>
            //{
            //    var headers = ctx.Response.Headers;
            //    headers.Add("X-Frame-Options", "SAMEORIGIN");
            //    headers.Add("X-XSS-Protection", "1; mode=block");
            //    headers.Add("X-Content-Type-Options", "nosniff");
            //    headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            //    //Below not working
            //    //headers.Add("Content-Security-Policy", "script-src 'self'");

            //    headers.Remove("X-Powered-By");


            //    return next();
            //});

            _logger.LogInformation("Configure other services.");

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiXSSMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });

            // websocket start
            var serviceScopeFactory =
                app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // handle client side routes
            app.Run(
                async (context) =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync(
                        Path.Combine(env.WebRootPath, "index.html")
                    );
                }
            );

            if (env.EnvironmentName != "DEV")
            {
                app.UseHsts();
            }

            _logger.LogInformation("Configuration completed.");
        }
    }
}
