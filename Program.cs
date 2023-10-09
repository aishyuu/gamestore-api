// Imports GameStore Endpoints
using GameStore.Api.Endpoints;
using GameStore.Api.Repositories;

// builds the app
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IGamesRepository, InMemGamesRepository>();

var app = builder.Build();

// builds the Game Endpoints to the app.
app.MapGamesEndpoints();

app.Run();