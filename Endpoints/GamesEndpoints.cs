using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Entities;

namespace GameStore.Api.Endpoints
{
    public static class GamesEndpoints
    {
        // The endpoint name
        // Purpose: So we can generate URL's to the endpoint, avoiding hard code paths
        // This is mostly for when we want to redirect to other endpoints from a delete or a post
        const string GetGameEndpointName = "GetGame";

        // Hard coded list of games for testing purposes
        static List<Game> games = new()
        {
            new Game()
            {
                Id = 1,
                Name = "Street Fighter 2",
                Genre = "Fighting",
                Price = 19.99M,
                ReleaseDate = new DateTime(1991, 2, 1),
                ImageUri = "https://placehold.co/100"
            },
            new Game()
            {
                Id = 2,
                Name = "Final Fantasy XIV",
                Genre = "MMORPG",
                Price = 59.99M,
                ReleaseDate = new DateTime(2010, 9, 30),
                ImageUri = "https://placehold.co/100"
            },
            new Game()
            {
                Id = 3,
                Name = "FIFA 23",
                Genre = "Sports",
                Price = 69.99M,
                ReleaseDate = new DateTime(2022, 9, 27),
                ImageUri = "https://placehold.co/100"
            }
        };

        // Builds routes and returns the group map we make in the function
        public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
        {
            // We create a map group so every URL starts with "/games"
            // The "WithParameterValidation" does all the validation for us and returns json data with the error if it happens!
            // Power of nuget packages
            var group = routes.MapGroup("/games")
                            .WithParameterValidation();

            // "/games/" endpoint. Returns all games
            group.MapGet("/", () => games);


            // "/games/{id}" id = number
            // Returns the specified game data
            group.MapGet("/{id}", (int id) =>
            {

                // checks if the game even exists and puts it in the "game" variable
                Game? game = games.Find(game => game.Id == id);

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
                // Sets the id to whatever the max is so we're guaranteed to not have conflicting ID numbers
                game.Id = games.Max(game => game.Id) + 1;
                // Add the game to the list
                games.Add(game);

                // We return the URI of the new thing we made
                // So it would be like "http://localhost:****/games/4"
                return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
            });

            // Updates game with new data
            group.MapPut("/{id}", (int id, Game updatedGame) =>
            {
                // Looks at id given by URL and checks if it exists (standard error checking)
                Game? gameToUpdate = games.Find(game => game.Id == id);
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

                // Returns a 200 but with no content (specifies that we did a good job!)
                return Results.NoContent();
            });

            // Delete game given id
            group.MapDelete("/{id}", (int id) =>
            {
                // Check if game even exists (default error checking)
                Game? gameToDelete = games.Find(game => game.Id == id);

                if (gameToDelete is not null)
                {
                    games.Remove(gameToDelete);
                }

                // Return a 200 error with no content
                return Results.NoContent();
            });

            // Returns the group map we established.
            return group;
        }
    }
}