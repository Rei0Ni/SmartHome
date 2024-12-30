using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using SmartHome.Application.Exceptions;

namespace SmartHome.Application.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                Log.Error(error, $"Something went wrong: {error.Message}");
                Log.Error(error, $"Inner Exception: {error.InnerException}");

                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case NullReferenceException:
                    case AppException:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
