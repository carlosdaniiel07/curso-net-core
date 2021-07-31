using CursoNetCore.Service.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CursoNetCore.Application.Handlers
{
    public class GlobalErrorHandler
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;

                response.ContentType = "application/json";
                response.StatusCode = ex is ApiException ? (ex as ApiException).StatusCode : (int)HttpStatusCode.InternalServerError;

                var message = response.StatusCode == 500 ? "Ocorreu um erro desconhecido. Tente novamente mais tarde" : ex.Message;
                var jsonResponse = JsonSerializer.Serialize(new { message });

                await response.WriteAsync(jsonResponse);
            }
        }
    }
}
