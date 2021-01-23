using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide.Operations;
using Raven.Client.ServerWide;
using Raven.Client.Exceptions;
using Raven.DependencyInjection;
using CPTruckAPI.src.Caminhoneiro.Interfaces;
using CPTruckAPI.src.Caminhoneiro.Services;
using CPTruckAPI.src.Caminhoneiro.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CPTruckAPI.src.Caminhoneiro.Models;

namespace CPTruckAPI
{
    public class Startup
    {
        public void EnsureDatabaseExists(IDocumentStore store, string database = null, bool createDatabaseIfNotExists = true)
        {
            database = database ?? store.Database;

            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

            try
            {
                store.Maintenance.ForDatabase(database).Send(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {
                if (createDatabaseIfNotExists == false)
                    throw;

                try
                {
                    store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(database)));
                }
                catch (ConcurrencyException)
                {
                    // The database was already created before calling CreateDatabaseOperation
                }

            }
        }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());
            });

            services.AddControllers();

            services.AddScoped<ICaminhoneiroService, CaminhoneiroService>();
            services.AddScoped<ICaminhoneiroRepository,CaminhoneiroRepository>();
            services.AddScoped<ICaminhoneiroAuth, CaminhoneiroAuth>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CPTruckAPI", Version = "v1" });
            });

            services.AddRavenDbDocStore(options => {
                var DatabaseName = options.Settings.DatabaseName;
            });

            services.AddRavenDbAsyncSession();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("TokenSettings").GetSection("Secret").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("user", policy => policy.RequireClaim("cptruck", "user"));
            });

            var urlAppConfig = Configuration.GetSection("RavenSettings").GetSection("Urls:0").Value;
            var dataBaseAppConfig = Configuration.GetSection("RavenSettings").GetValue<string>("DatabaseName");
            IDocumentStore store = new DocumentStore(){
                Urls = new[] {urlAppConfig} ,
                Database = dataBaseAppConfig
            }.Initialize();
            EnsureDatabaseExists(store, dataBaseAppConfig);

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<TokenSettings>(Configuration.GetSection("Secret"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CPTruckAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
