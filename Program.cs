
using EventPay.API.Data;
using EventPay.API.DTOs.Events;
using EventPay.API.DTOs.Payments;
using EventPay.API.Services.Auth;
using EventPay.API.Services.EventService;
using EventPay.API.Services.IEvent;
using EventPay.API.Services.Maps;
using EventPay.API.Services.Messaging;
using EventPay.API.Services.Payments;
using EventPay.API.Services.Reports;
using EventPay.API.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace EventPay.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<TwilioService>();
            builder.Services.AddScoped<TelegramService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<OtpService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddHttpClient<IMapsService, MapsService>();
            builder.Services.AddScoped<IValidator<CreatePaymentDto>, CreatePaymentValidator>();
            builder.Services.AddScoped<IValidator<CreateEventDto>, CreateEventValidator>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReact", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
            var app = builder.Build();
            app.UseCors("AllowReact");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // لازم يكون قبل Authorization
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
