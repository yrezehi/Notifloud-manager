using Notifloud_manager.Controllers;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.RegisterControllers();

app.Run();