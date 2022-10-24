using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PruebaCQRS.Behaviour;
using PruebaCQRS.Domain;
using PruebaCQRS.Filters;
using PruebaCQRS.Infrastructure.Persistence;
using PruebaCQRS.Services;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
    options.Filters.Add<ApiExceptionFilterAttribute>())
    .AddFluentValidation();
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Introduce JWT con Bearer en el campo",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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
                }
            }, new string[] { }
        }
    });
});

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddSqlite<MyAppDbContext>(builder.Configuration.GetConnectionString("Default"));

//Services for Identity
builder.Services
    .AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MyAppDbContext>();

//Services Auth & autorization
builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<ICurrentUserService,CurrentUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await SeedProducts();

app.Run();


async Task SeedProducts()
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    context.Database.EnsureCreated();

    if (!context.Products.Any())
    {
        context.Products.AddRange(new List<Product>
        {
            new Product
            {
                Description = "Product 01",
                Price = 16000
            },
            new Product
            {
                Description = "Product 02",
                Price = 52200
            }
        });

        await context.SaveChangesAsync();
    }

    var testUser = await userManager.FindByNameAsync("other_user");
    if (testUser is null)
    {
        testUser = new IdentityUser
        {
            UserName = "test_user"
        };

        await userManager.CreateAsync(testUser, "Pass1234");
        await userManager.CreateAsync(new IdentityUser
        {
            UserName = "other_user"
        }, "Pass1234");
    }

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var adminRole = await roleManager.FindByNameAsync("Admin");
    if (adminRole is null)
    {
        await roleManager.CreateAsync(new IdentityRole
        {
            Name = "Admin"
        });

        await userManager.AddToRoleAsync(testUser, "Admin");
    }
}