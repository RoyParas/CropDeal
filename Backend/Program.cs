using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Backend.Data;
using Backend.Models;
using System.Text.Json.Serialization;
using Backend.Interfaces;
using CropDeal.API.Middleware;
using Backend.Repositories;
using Backend.Services;
using QuestPDF.Infrastructure;
using Backend.Exceptions;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

// builder.Services.AddEndpointsApiExplorer();
// an extension method used to discover and expose metadata that Swagger needs to create documentation about your API endpoints, specifically for Minimal APIs
// If you have both standard controllers and some Minimal APIs in the same project, Swagger will only "see" the controllers if you omit it.
// If there are only standard controllers and no minimal APIs then you can omit it


// Swagger for API documentation (optional, for development)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CropDeal API",
        Version = "v1"
    });
    

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter token without \"\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    options.UseAllOfToExtendReferenceSchemas();
});


// Add DbContext for database access
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();  // For password reset, email confirmation, etc.

//Authentication
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;

    var secretKey = builder.Configuration["Jwt:Secret"];
    if (string.IsNullOrWhiteSpace(secretKey))
        throw new InvalidException("JWT secret is missing in configuration.");

    options.TokenValidationParameters = new TokenValidationParameters(){
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepo, UserRepository>();
builder.Services.AddScoped<IPasswordResetRepo, PasswordResetRepository>();
builder.Services.AddScoped<IBankRepo, BankRepository>();
builder.Services.AddScoped<IAddressRepo, AddressRepository>();
builder.Services.AddScoped<ICropRepo, CropRepository>();
builder.Services.AddScoped<ICropListRepo, CropListRepository>();
builder.Services.AddScoped<ISubscriptionRepo, SubscriptionRepository>();
builder.Services.AddScoped<ITransactionRepo, TransactionRepository>();
builder.Services.AddScoped<IReviewRepo, ReviewRepository>();
builder.Services.AddScoped<INotificationRepo, NotificationRepository>();
builder.Services.AddScoped<IReceiptPdfService, ReceiptPdfService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddHttpContextAccessor();

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAngularClient",
//         policy =>
//         {
//             policy.WithOrigins("http://localhost:4200")
//                   .AllowAnyHeader()
//                   .AllowAnyMethod();
//         });
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seeding roles here
using (var scope = app.Services.CreateScope()) 
{
    var services = scope.ServiceProvider; 
    await AppDbInitializer.SeedRolesAsync(services);
}

// app.MapGet("/", () => "Hello World!"); //Minimal API
// Minimal API is a streamlined way to build HTTP APIs in ASP.NET Core with significantly less code and fewer dependencies compared to traditional controller-based APIs.
// Instead of creating separate classes, inheriting from ControllerBase, and using various attributes, you define your endpoints directly in Program.cs using simple lambda expressions.

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

// app.UseCors("AllowAngularClient");
app.UseCors("AllowAll");

app.UseHttpsRedirection();   // Redirects HTTP to HTTPS
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();     // Enables authentication
app.UseAuthorization();      // Enables authorization

app.MapControllers();        // Maps controller routes


app.Run();