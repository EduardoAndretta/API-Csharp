using Catalog.Repositories;
using Catalog.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            var mongoDbsettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            services.AddSingleton<IMongoClient>(serviceProvider =>
                { 
                    return new MongoClient(mongoDbsettings.ConnectionString);
                }
            );

            services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();

            services.AddControllers(options =>
                {
                    options.SuppressAsyncSuffixInActionNames = false;
                }
            );
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" });
            });

            services.AddHealthChecks()
                    .AddMongoDb
                    (mongoDbsettings.ConnectionString, 
                    name: "mongodb", 
                    timeout: TimeSpan.FromSeconds(3),
                    tags: new[] { "ready" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                    {
                        Predicate = (check) => check.Tags.Contains("ready"),
                        ResponseWriter = async(Context, report) =>
                        {
                            var result = JsonSerializer.Serialize
                            (
                                new
                                {
                                    status = report.Status.ToString(),
                                    checks = report.Entries.Select(entry => new 
                                        {
                                            name = entry.Key,
                                            status = entry.Value.Status.ToString(),
                                            exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                                            duration = entry.Value.Duration.ToString()
                                        }
                                    )
                                }
                            );
                            Context.Response.ContentType = MediaTypeNames.Application.Json;
                            await Context.Response.WriteAsync(result);
                        }
                    }
                );

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                    {
                        Predicate = (_) => false
                    }
                );
            });
        }
    }
}
