//using Serilog;
//using System;
//using System.Collections.Generic;
//using Serilog.Sinks.Elasticsearch;
//using Serilog.Formatting.Compact;
//using System.Reflection;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using Serilog.Formatting.Elasticsearch;
//using Serilog.Events;
//using Serilog.Exceptions;
//using Elastic.Apm.SerilogEnricher;
//using Serilog.Core;

//namespace UspsOpenApi.Base
//{
//    public static class Logging
//    {
//        internal static Logger _logger = new LoggerConfiguration()
//                            .Enrich.WithMachineName()
//                            .Enrich.WithProperty("appVersion", Assembly.GetEntryAssembly().GetName().Version.ToString())
//                            .Enrich.WithExceptionDetails()
//                            .Enrich.WithElasticApmCorrelationInfo()
//                            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elastic:Usa%4012345678@elasticsearch:9200/"))
//                            {
//                                BufferCleanPayload = (failingEvent, statuscode, exception) =>
//                                {
//                                    dynamic e = JObject.Parse(failingEvent);
//                                    return JsonConvert.SerializeObject(new Dictionary<string, object>()
//                                    {
//                                        { "@timestamp", e["@timestamp"] },
//                                        { "level", "Error" },
//                                        { "message","Error: " + e.message },
//                                        { "messageTemplate", e.messageTemplate },
//                                        { "failingStatusCode", statuscode },
//                                        { "failingException", exception }
//                                    });
//                                },
//                                MinimumLogEventLevel = LogEventLevel.Verbose,
//                                AutoRegisterTemplate = true,
//                                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
//                                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
//                                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-Production-{DateTime.UtcNow:yyyy-MM}",
//                                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
//                                           EmitEventFailureHandling.WriteToFailureSink |
//                                           EmitEventFailureHandling.RaiseCallback
//                            })
//                            .MinimumLevel.Debug()
//                            .WriteTo.Console()
//                            .WriteTo.File(new CompactJsonFormatter(), @"C:/Apps/Logs/UspsOpenApi..log", rollingInterval: RollingInterval.Day)
//                            .CreateLogger();
//    }
//}
