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
using Jaeger;
using OpenTracing.Propagation;
using OpenTracing;
using OpenTracing.Noop;
using OpenTracing.Util;
using Prometheus;

namespace Testingdockerapi
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
            //IWebHostEnvironment _environment;
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Plan API",
                    Version = "v1",
                    Description = "Sample service for FP",
                });
            });
            services.AddStackExchangeRedisCache(setupAction =>
            {
                setupAction.Configuration = Configuration.GetConnectionString("RedisCache");
            });
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "AllowOrigin",
                    builder => {
                        builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                    });
            });

            services.AddOpenTracing();

            services.AddSingleton<ITracer>(cli =>
            {
                Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", "PlanService");
               

                //if (_environment.IsDevelopment())
                //{
                //    Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", "localhost");
                //    Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", "6831");
                //    Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "const");
                //}

                var loggerFactory = new LoggerFactory();

                var config = Jaeger.Configuration.FromEnv(loggerFactory);
                var tracer = config.GetTracer();

                if (!GlobalTracer.IsRegistered())
                {
                    // Allows code that can't use DI to also access the tracer.
                    GlobalTracer.Register(tracer);
                }

                return tracer;
            });
        }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors("AllowOrigin");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", "localhost");
                Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", "6831");
                Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "const");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseHttpMetrics(options =>
            //{
            //    // Assume there exists a custom route parameter with this name.
            //    //options.AddRouteParameter("plan");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics("./metrics");
            });

            //app.UseHttpMetrics(options =>
            //{
            //   // Assume there exists a custom route parameter with this name.
            //   options.AddRouteParameter("Plan");
            //});
            app.UseMetricServer();


            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v2/swagger.json", "PlanService"));
        }

    }
}
