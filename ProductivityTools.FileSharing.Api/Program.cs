using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using ProductivityTools.MasterConfiguration;



string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddMasterConfiguration(force: true);

builder.Services.Configure<KestrelServerOptions>(options =>
{
    // Set the limit to 1000 MB.
    options.Limits.MaxRequestBodySize = 1073741824;
});

builder.Services.Configure<FormOptions>(options =>
{
    // Set the multipart body limit to 1000 MB
    options.MultipartBodyLengthLimit = 1073741824;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    // Set the limit to 1000 MB for IIS in-process hosting
    options.MaxRequestBodySize = 1073741824;
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:3000", "https://productivitytools-filesharing-web-93484780890.europe-west1.run.app").AllowAnyHeader().AllowAnyMethod();
                                  });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://securetoken.google.com/ptauthumbrella";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "https://securetoken.google.com/ptauthumbrella",
        ValidateAudience = true,
        ValidAudience = "ptauthumbrella",
        ValidateLifetime = true
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();
