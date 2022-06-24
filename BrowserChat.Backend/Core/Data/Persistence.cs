﻿using Microsoft.EntityFrameworkCore;
using BrowserChat.Entity;

namespace BrowserChat.Backend.Core.Data
{
    public static class Persistence
    {
        public static void PrepPopulation(IApplicationBuilder builder, bool isProduction)
        {
            using (var serviceScope = builder.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<BrowserChatDbContext>(), isProduction);
            }
        }

        private static void SeedData(BrowserChatDbContext? context, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }

            if (!context.Rooms.Any())
            {
                context.Rooms.AddRange(
                    new Room { Name = "Programming" },
                    new Room { Name = "Music" },
                    new Room { Name = "Literature" },
                    new Room { Name = "Science" }
                );

                context.SaveChanges();
            }

            if (!context.Posts.Any())
            {
                Random rnd = new Random();

                for (int i = 1; i <= 100; i++)
                {
                    context.Posts.Add(new Post { Message = $"Message #{i}", RoomId = rnd.Next(1, 5), TimeStamp = DateTime.Now, UserName = "nestor.panu" });
                }

                context.SaveChanges();
            }
        }
    }
}
