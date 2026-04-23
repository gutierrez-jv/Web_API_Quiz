
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Web_API_Quiz.Utils;

namespace Web_API_Quiz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        var detail = context.ErrorDescription;

                        if (string.IsNullOrWhiteSpace(detail) && context.AuthenticateFailure != null)
                        {
                            detail = context.AuthenticateFailure.Message;
                        }

                        if (string.IsNullOrWhiteSpace(detail))
                        {
                            detail = "JWT is missing, invalid, expired, or malformed.";
                        }

                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            status = 401,
                            message = "Unauthorized",
                            detail
                        });
                    }
                };
            });

            builder.Services.AddRateLimiter(options =>
            {
                // Code block for the Custom 429 response
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();

                    logger.LogWarning("Rate limit violation on {Path} by {User}.",
                        context.HttpContext.Request.Path,
                        context.HttpContext.User.Identity?.Name ?? "anonymous");

                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = 429,
                        message = "Rate limit exceeded",
                        endpoint = context.HttpContext.Request.Path.ToString(),
                        detail = "Too many requests. Please try again later."
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken: token);
                };

                options.AddFixedWindowLimiter("loginPolicy", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 3;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueLimit = 0;
                });


                options.AddFixedWindowLimiter("tasksPolicy", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 10;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueLimit = 0;
                });
            });

            builder.Services.AddAuthorization();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<JwtService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}
