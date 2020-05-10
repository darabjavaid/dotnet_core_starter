using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebApi.Helpers;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using DT.Interfaces;
using DT.Services.Services;
using DT.Infrastructure.Data;
using SD.BuildingBlocks.Infrastructure;
using SD.BuildingBlocks.Repository;
using DT.Domain.Interfaces;
using DT.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using DT.Interfaces.CMS;
using DT.Services.Services.CMS;
using System.Security.Claims;

namespace WebApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // use sql server db in production and sqlite db in development
            //if (_env.IsProduction())
            //    services.AddDbContext<DataContext>();
            //else
            //    services.AddDbContext<DataContext, SqliteDataContext>();

            // connect to sql server database
            string connectionString = _configuration.GetConnectionString("WebApiDatabase");
            services.AddDbContext<AppDBContext>(opt => opt.UseSqlServer(connectionString));
            

            services.AddCors();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // configure strongly typed settings objects
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        //var userId = new Guid(context.Principal.Identity.Name);
                        //var user = userService.GetById(userId);

                        var username = context.Principal.Identity.Name;
                        var userTask = userService.GetByUserName(username);
                        if (userTask == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("V1", new Microsoft.OpenApi.Models.OpenApiInfo { 
                    Version = "1.0",
                    Description = "DenTUTOR API",
                    Title = "DenTUTOR API"
                });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter \'Bearer\' followed by space and JWT",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
            //services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()

            // configure DI for application repositories
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryEF<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            //  user defined repositories
            services.AddScoped<IQuestionnaireRepository, QuestionnaireRepository>();

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IQuestionnaireService, QuestionnnaireService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)//, AppDBContext dataContext)
        {
            //migrate any database changes on startup(includes initial db creation)
            //dataContext.Database.Migrate();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();                
            });

            var url = _configuration["DevAPI:APIBaseURL"];
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("V1/swagger.json", "DenTUTOR API");
            });
        }
    }
}
