using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;

namespace Violetum.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<PostVote> PostVotes { get; set; }
        public DbSet<CommentVote> CommentVotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Community>(entity => { entity.HasIndex(x => x.Name).IsUnique(); });
            modelBuilder.Entity<Community>().Property(entity => entity.Image)
                .HasDefaultValue($"{nameof(Community)}/no-image.jpg");
            modelBuilder.Entity<User>().Property(entity => entity.Image)
                .HasDefaultValue($"{nameof(Community)}/no-image.jpg");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                IEnumerable<EntityEntry> entries = ChangeTracker
                    .Entries()
                    .Where(e => e.Entity is BaseEntity && (
                        (e.State == EntityState.Added)
                        || (e.State == EntityState.Modified)));

                foreach (EntityEntry entityEntry in entries)
                {
                    ((BaseEntity) entityEntry.Entity).UpdatedAt = DateTime.Now;

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((BaseEntity) entityEntry.Entity).CreatedAt = DateTime.Now;
                    }
                }

                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}