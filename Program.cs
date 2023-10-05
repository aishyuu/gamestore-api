// Imports GameStore Endpoints
using GameStore.Api.Endpoints;

// builds the app
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// builds the Game Endpoints to the app.
app.MapGamesEndpoints();

app.Run();