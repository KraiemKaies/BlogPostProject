using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
                var blog = new Blog(".Net Blog");
                var tags = new[]
                {
                    new Tag
                    {
                        Text = ".Net"
                    },
                    new Tag
                    {
                        Text = "EF"
                    },
                };
                blog.AddPost(new Post
                {
                    Title = "Announcing the Release of EF Core 5.0",
                    Content = "Announcing the Release of EF Core 5.0, a full feature cross-platform... ",
                    Tags = { tags[0], tags[1] }
                });
                blog.AddPost(new Post
                {
                    Title = "Announcing F# 5",
                    Content = "F# 5 is the lastest version of F# ",
                    Tags = { tags[1] }
                });
                blog.AddPost(new Post
                {
                    Title = "Announcing .Net 5",
                    Content = ".Net 5 includes many enhacements, including single file applications, more ... ",
                    Tags = { tags[0] }
                });
                context.Add(blog);
                context.SaveChanges();
            }
            using (var context = new BlogsContext())
            {
                var queryable = context.Blogs.Include(e => e.Posts).ThenInclude(e => e.Tags);
                var blogs = queryable.ToList();
                Console.WriteLine();
                Console.WriteLine();
                foreach (var blog in blogs)
                {
                    Console.WriteLine($"Blog: {blog.Name}");
                    foreach (var post in blog.Posts)
                    {
                        Console.WriteLine($"           Post: {post.Title}");

                        foreach (var tag in post.Tags)
                        {
                            Console.WriteLine($"                          Tag : {tag.Text}");
                        }
                    }
                }
            }
        }
    }
    public class Blog
    {
        private readonly int _id;
        private readonly List<Post> _posts = new List<Post>();

        public Blog(string name)
        {
            Name = name;
        }

        public Blog(int id, string name)
        {
            _id = id;
            Name = name;
        }
        public string Name { get; }
        public void AddPost(Post post) => ((List<Post>)_posts).Add(post);
        public IReadOnlyList<Post> Posts => _posts.ToList();
    }
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Blog Blog { get; set; }
        public ICollection<Tag> Tags { get; } = new List<Tag>();
    }
    public class Tag
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<Post> Posts { get; } = new List<Post>();
    }
    public class BlogsContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogs");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(b =>
            {
                b.Property<int>("_id").HasColumnName("Id");
                b.HasKey("_id");
                b.Property(e => e.Name);
            });
        }
    }
}
