using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var service = builder.Services;
// Add services to the container.

service.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
service.AddEndpointsApiExplorer();
service.AddSwaggerGen();

// Add the authentication services to DI and configures Bearer as the default schema.
service
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5001";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

service.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    });
});

service.AddCors(options =>
{
    // this defines a CORS policy called "default"
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("https://localhost:5003")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("default");

// Add the authentication middleware to the pipline so authentication will be performed automatically on every call into the host.
app.UseAuthentication();
// Add the authorization middleware to make sure, our API endpoint cannot be accessed by anonymous clients.
app.UseAuthorization();

app.MapControllers();

app.Run();
