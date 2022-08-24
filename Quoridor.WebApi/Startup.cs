using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Quoridor.DataAccess.Repositories;
using Quoridor.Service.Services;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.Configuration;
using Quoridor.WebApi.Hubs;
using Quoridor.WebApi.Infrastructure.MappingProfiles;
using Serilog;

namespace Quoridor.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.Configuration = configuration;
            this.Env = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            this.SetupDependencyInjection(services);

            services.AddCors(options =>
            {
                options.AddPolicy("OpenPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var connectionStrings =
                this.Configuration.GetSection("ConnectionStrings").Get<ConnectionStringConfiguration>();
            var jwtSettings =
                this.Configuration.GetSection("Jwt").Get<JwtTokenConfiguration>();

            services
                .AddControllers()
                .AddNewtonsoftJson((op) =>
                {
                    op.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Quoridor.WebApi", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorizationheader using the Bearer scheme. \n\n
                        Enter 'Bearer[space]'and then your token in the text  input below.\n\n
                        Example:  'Bearer tes2543t432to243ke324n'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*
            app.UseRequestResponseLogging();
            */

            /*
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VideoTranslate v1"));
            }
            */

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quoridor v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("OpenPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TestHub>("/testhub");
            });
        }

        private void SetupDependencyInjection(IServiceCollection services)
        {
            var connectionStrings = this.Configuration.GetSection("ConnectionStrings").Get<ConnectionStringConfiguration>();
            services.AddSingleton(connectionStrings);

            var jwtSettings = this.Configuration.GetSection("Jwt").Get<JwtTokenConfiguration>();
            services.AddSingleton(jwtSettings);

            // AutoMapper Configuration
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IGameUserRepository, GameUserRepository>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IGameService, GameService>();
        }

        private Task WriteResponseAsync(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            var defaultLogLevel = this.Configuration["Logging:LogLevel:Default"];
            var connectionString = this.Configuration["ConnectionStrings:Main"];
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            if (!string.IsNullOrEmpty(connectionStringBuilder.Password))
            {
                connectionStringBuilder.Password = $"{connectionStringBuilder.Password[0]}***{connectionStringBuilder.Password[connectionStringBuilder.Password.Length - 1]}";
            }

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("configuration", new JObject(
                    new JProperty("defaultLogLevel", defaultLogLevel),
                    new JProperty("hostEnvironment", this.Env.EnvironmentName))),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                   new JProperty(pair.Key, new JObject(
                       new JProperty("status", pair.Value.Status.ToString()),
                       new JProperty("description", pair.Value.Description),
                       new JProperty("data", new JObject(pair.Value.Data.Select(p =>
                           new JProperty(p.Key, p.Value))))))))));

            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}
