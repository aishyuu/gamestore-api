using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;

namespace GameStore.Api.Endpoints
{
    public static class GamesEndpoints
    {
        // The endpoint name
        // Purpose: So we can generate URL's to the endpoint, avoiding hard code paths
        // This is mostly for when we want to redirect to other endpoints from a delete or a post
        const string GetGameEndpointName = "GetGame";

        // Builds routes and returns the group map we make in the function
        public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
        {
            InMemGamesRepository repository = new();

            // We create a map group so every URL starts with "/games"
            // The "WithParameterValidation" does all the validation for us and returns json data with the error if it happens!
            // Power of nuget packages
            var group = routes.MapGroup("/games")
                            .WithParameterValidation();

            // "/games/" endpoint. Returns all games
            group.MapGet("/", () => repository.GetAll());


            // "/games/{id}" id = number
            // Returns the specified game data
            group.MapGet("/{id}", (int id) =>
            {

                // checks if the game even exists and puts it in the "game" variable
                Game? game = repository.Get(id);

                // If the result of the check is nothing, it will return a 404 error!
                if (game is null)
                {
                    return Results.NotFound();
                }

                // Returns a 200 with the game
                return Results.Ok(game);
            })
            .WithName(GetGameEndpointName);

            // Creates a new game
            group.MapPost("/", (Game game) =>
            {
                repository.Create(game);

                // We return the URI of the new thing we made
                // So it would be like "http://localhost:****/games/4"
                return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
            });

            // Updates game with new data
            group.MapPut("/{id}", (int id, Game updatedGame) =>
            {
                // Looks at id given by URL and checks if it exists (standard error checking)
                Game? gameToUpdate = repository.Get(id);
                if (gameToUpdate is null)
                {
                    return Results.NotFound();
                }

                // Takes data from updatedGame and updates the game we want to update
                gameToUpdate.Name = updatedGame.Name;
                gameToUpdate.Genre = updatedGame.Genre;
                gameToUpdate.Price = updatedGame.Price;
                gameToUpdate.ReleaseDate = updatedGame.ReleaseDate;
                gameToUpdate.ImageUri = updatedGame.ImageUri;

                repository.Edit(gameToUpdate);

                // Returns a 200 but with no content (specifies that we did a good job!)
                return Results.NoContent();
            });

            // Delete game given id
            group.MapDelete("/{id}", (int id) =>
            {
                // Check if game even exists (default error checking)
                Game? gameToDelete = repository.Get(id);

                if (gameToDelete is not null)
                {
                    repository.Delete(id);
                }

                // Return a 200 error with no content
                return Results.NoContent();
            });

            // Returns the group map we established.
            return group;
        }
    }
}