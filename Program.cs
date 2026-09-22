using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Repositories;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using StudyHubAPI.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

string? apiToken = Environment.GetEnvironmentVariable("MY_API_TOKEN");


if (string.IsNullOrWhiteSpace(apiToken))
{
    throw new InvalidOperationException(
        "CRITICAL STARTUP ERROR: The environment variable 'MY_API_TOKEN' is missing or empty. " +
        "Please set it on your machine and restart your IDE.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // TokenValidationParameters define how incoming JWTs will be validated.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "StudyHubApi",
            ValidAudience = "StudyHubApiUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiToken))
        };
    });

builder.Services.AddAuthorization();

// =========================================================
// 1. Add Controllers
// =========================================================


builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.AddPolicy("GeneralPolicy", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30, // 30 requests per minute per IP
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 2
            });
    });
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeCustomConverter());
}); ;

// =========================================================
// 2. Add Swagger / OpenAPI
// =========================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // The name of the HTTP header where the token will be sent.
        Name = "Authorization",


        // Indicates this is an HTTP authentication scheme.
        Type = SecuritySchemeType.Http,


        // Specifies the authentication scheme name.
        // Must be exactly "Bearer" for JWT Bearer tokens.
        Scheme = "Bearer",


        // Optional metadata to describe the token format.
        BearerFormat = "JWT",


        // Specifies that the token is sent in the request header.
        In = ParameterLocation.Header,


        // Text shown in Swagger UI to guide the user.
        Description = "Enter: Bearer {your JWT token}"
    });


    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                // Reference the previously defined "Bearer" security scheme.
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },

            // No scopes are required for JWT Bearer authentication.
            // This array is empty because JWT does not use OAuth scopes here.
            new string[] {}
        }
    });
});

// =========================================================
// 3. Add Database Context
// =========================================================
builder.Services.AddDbContext<StudyHubDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("StudyHubConnection")
    ));

// =========================================================
// 4. Register Repositories
// =========================================================
builder.Services.AddScoped<PersonRepository>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<AdministratorRepository>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<WorkspaceRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<WorkspaceImagesRepository>();


// =========================================================
// 5. Register Services
// =========================================================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AdministratorService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<WorkspaceService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<WorkspaceImagesService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("StudyHubApiCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7022",
                "http://localhost:5203"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// 1. Development Tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Transport Security
app.UseHttpsRedirection();

// 3. CORS (Must be before RateLimiter and Auth)
app.UseCors("StudyHubApiCorsPolicy");

// 4. Rate Limiting
app.UseRateLimiter();

// 5. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6. Routing
app.MapControllers().RequireRateLimiting("GeneralPolicy");

app.Run();