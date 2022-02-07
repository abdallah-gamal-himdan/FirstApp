using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.MiddleWare
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleWare> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleWare(RequestDelegate next ,
         ILogger<ExceptionMiddleWare> logger,
         IHostEnvironment env)
        {
            this.env = env;
            this.logger = logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context){
            try
            {
              await next(context);
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                context.Response.ContentType="application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = env.IsDevelopment()
                ? new ApiExceptions(context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString()) :
                 new ApiExceptions(context.Response.StatusCode,"Internal Server Error");

                 var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                 var json = JsonSerializer.Serialize(response,options);

                 await context.Response.WriteAsync(json);

            }
        }
    }
}