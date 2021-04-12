using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BlogPostProject
{
    class Program
    {
        static void Main()
        {
            using (var context = new BlogsContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add(new Blog
                {
                    Name = ".Net Blog",
                    Posts =
                    {
                        new Post
                        {
                            Title = "Announcing the Release of EF Core 5",
                            Content= "Announcing the Release of EF Core 5, a full feature cross-platform"
                        },
                        new Post
                        {
                            Title = "Announcing F#5",
                            Content= "Announcing the F# 5, the fonctional programming language ..."
                        },
                        new Post
                        {
                            Title = "Announcing .Net 5.0",
                            Content= "Announcing the .net 5.0, including single file application, more ...."
                        },
                    }
                });
                context.SaveChanges();

            }
        }
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public Blog Blog { get; set; }
    }

    public class BlogsContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogs");
    }
}
