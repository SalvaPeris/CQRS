using API;
using ApplicationCore;
using ApplicationCore.Infrastructure.Persistence.Seed;
using ApplicationCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Services from API Project
builder.Services.AddApplicationCore();
builder.Services.AddSwagger();
builder.Services.AddScoped<ICurrentUserService,CurrentUserService>();

//Services from ApplicationCore Project
builder.Services.AddPersistence(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddIdentityPersistence();
builder.Services.AddAutoMapperAndMediatR();
builder.Services.AddSecurity(builder.Configuration);

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

await Seed.SeedProducts(app.Services);

app.Run();