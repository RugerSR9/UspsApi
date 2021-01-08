using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Elastic.Apm.SerilogEnricher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace UspsApi.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                            .Enrich.WithMachineName()
                            .Enrich.WithProperty("appVersion", Assembly.GetEntryAssembly().GetName().Version.ToString())
                            .Enrich.WithExceptionDetails()
                            .Enrich.WithElasticApmCorrelationInfo()
                            .MinimumLevel.Verbose()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                            .WriteTo.File(new CompactJsonFormatter(), Configuration["SeriLog:FilePath"], rollingInterval: RollingInterval.Day)
                            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elastic:Usa%4012345678@elasticsearch:9200/"))
                            {
                                BufferCleanPayload = (failingEvent, statuscode, exception) =>
                                {
                                    dynamic e = JObject.Parse(failingEvent);
                                    return JsonConvert.SerializeObject(new Dictionary<string, object>()
                                    {
                                        { "@timestamp",e["@timestamp"]},
                                        { "level","Error"},
                                        { "message","Error: "+e.message},
                                        { "messageTemplate",e.messageTemplate},
                                        { "failingStatusCode", statuscode},
                                        { "failingException", exception}
                                    });
                                },
                                MinimumLogEventLevel = LogEventLevel.Verbose,
                                AutoRegisterTemplate = true,
                                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                                IndexFormat = $"UspsApi.Web-{Configuration["Environment"]}-{DateTime.UtcNow:yyyy-MM}",
                                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                           EmitEventFailureHandling.WriteToFailureSink |
                                           EmitEventFailureHandling.RaiseCallback
                            })
                            .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
