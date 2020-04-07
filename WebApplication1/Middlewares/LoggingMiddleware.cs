using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Middlewares
{
    public class LoggingMiddleware
    {

        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            string method = httpContext.Request.Method;
            string endpoint = httpContext.Request.Path;
            var body = string.Empty; 
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                body = await reader.ReadToEndAsync();
            }
            string query = httpContext.Request.QueryString.Value;

            string path = @"C:\Users\User\Desktop\requestsLog.txt";
            using (var writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                await writer.WriteLineAsync(method + " " + endpoint + " " + body + " " + query);
            }
            await _next(httpContext);
        }
    }
}