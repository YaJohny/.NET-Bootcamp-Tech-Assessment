using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DataAccess.Data
{
    public class BookDB : IdentityDbContext<User> 
    {
        public BookDB(DbContextOptions<BookDB> options) : base(options) { }

        public DbSet<User> Users {  get; set; }
        public DbSet<Book> Books { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Book>().HasIndex(b => b.Title).IsUnique();
		}
	}
}
