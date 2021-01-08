using Elastic.Apm.SerilogEnricher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf.Grpc.Server;
using UspsApi.gRPC.Services;

namespace UspsApi.gRPC
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
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
                                IndexFormat = $"UspsApi.gRPC-{Configuration["Environment"]}-{DateTime.UtcNow:yyyy-MM}",
                                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                           EmitEventFailureHandling.WriteToFailureSink |
                                           EmitEventFailureHandling.RaiseCallback
                            })
                            .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCodeFirstGrpc(config =>
            {
                config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });

            //services.AddCodeFirstGrpcReflection();

            //services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<RateEstimate>();
                //endpoints.MapCodeFirstGrpcReflectionService();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
