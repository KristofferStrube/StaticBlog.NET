var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/hello", () => "world!")
.WithName("Hello");

app.Run();
