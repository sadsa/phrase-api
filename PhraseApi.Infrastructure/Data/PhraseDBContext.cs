using Microsoft.EntityFrameworkCore;
using PhraseApi.Core.Entities;

namespace PhraseApi.Infrastructure.Data
{
    public class PhrasesDbContext : DbContext
    {
        public PhrasesDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Phrase> Phrases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Phrase>()
                .Property(p => p.Text)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}