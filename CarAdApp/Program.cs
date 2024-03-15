using CarAdApp.Data;
using CarAdApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarAdApp API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("CarAdDB"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed the in-memory database with some data
using (var scope = app.Services.CreateScope())
{
    var scopedServices = scope.ServiceProvider;
    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    
    if (!db.Cars.Any())
    {
        db.Cars.AddRange(new Car
        {
            Brand = "Tesla",
            Model = "Model S",
            ProductionYear = 2020,
            EngineVolume = 0,
            Mileage = 15000,
            Price = 75000,
            Color = "Red",
            BanType = BanType.Sedan,
            FuelType = FuelType.Benzine,
            Gearbox = Gearbox.Automatic,
            PictureUrl = "https://turbo.azstatic.com/uploads/full/2024%2F03%2F12%2F00%2F20%2F12%2F066c569e-5eb7-407c-b8c5-52580e4f8b19%2F32444_OikwS_ZIYofkU92pw8zouQ.jpg",
            UserId = "nihad12345"
        },
        new Car
        {
            Brand = "Ford",
            Model = "Mustang",
            ProductionYear = 2021,
            EngineVolume = 5.0,
            Mileage = 5000,
            Price = 55000,
            Color = "Blue",
            BanType = BanType.Bus,
            FuelType = FuelType.Dizel,
            Gearbox = Gearbox.Mechanic,
            PictureUrl = "https://turbo.azstatic.com/uploads/full/2024%2F03%2F12%2F00%2F20%2F12%2F066c569e-5eb7-407c-b8c5-52580e4f8b19%2F32444_OikwS_ZIYofkU92pw8zouQ.jpg",
            UserId = "nihad12345"
        });
        db.SaveChanges();
    }
    
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Username = "nihad12345",
            Password = "nihad12345nihad12345", // This is for demonstration only. We should use password hashing in production.
            Role = "Admin"
        });
        db.SaveChanges();
    }
    
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Username = "AISTGroup",
            Password = "AISTGroup123456",
            Role = "User"
        });
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
