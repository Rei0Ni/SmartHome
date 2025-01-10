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
using FluentValidation;
using SmartHome.Application.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Json;

namespace SmartHome.Application.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<JsonOptions> _jsonOptions;

        public ExceptionHandler(RequestDelegate next, IOptions<JsonOptions> jsonOptions)
        {
            _next = next;
            _jsonOptions = jsonOptions;
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
                    case LoginFailedException e:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        var loginFailedResult = new ApiResponse<object>
                        {
                            Status = e.Status,
                            Message = e.Message,
                        };
                        await response.WriteAsJsonAsync(loginFailedResult);
                        return;
                    case FluentValidationException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        var validationErrors = e.Data!.Select(e => new
                        {
                            field = e.PropertyName,
                            error = e.ErrorMessage
                        });
                        var validationResult = new ApiResponse<object>()
                        {
                            Status = e.Status,
                            Message = e.Message,
                            Data = validationErrors
                        };
                        await response.WriteAsJsonAsync(validationResult);
                        return;

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
