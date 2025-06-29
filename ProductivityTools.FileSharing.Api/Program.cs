using ProductivityTools.MasterConfiguration;



string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddMasterConfiguration(force: true);

// Increase the maximum request body size. The default is approx. 30 MB.
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Set the limit to 100 MB, for example. You can adjust this value.
    serverOptions.Limits.MaxRequestBodySize = null;
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
                                      builder.WithOrigins("http://localhost:3000", "https://ptfilesharing-45459715128.us-central1.run.app").AllowAnyHeader().AllowAnyMethod();
                                  });
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
