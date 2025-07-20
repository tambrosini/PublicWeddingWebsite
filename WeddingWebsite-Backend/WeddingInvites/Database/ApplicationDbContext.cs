using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeddingInvites.Domain;

namespace WeddingInvites.Database;

public class ApplicationDbContext : IdentityDbContext 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public virtual DbSet<Guest> Guests { get; set; }
    public virtual DbSet<Invite> Invites { get; set; }
    public virtual DbSet<EventLog> EventLogs { get; set; }

    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("dbo");
        
        ConfigureBaseProperties<Guest>(builder);
        ConfigureBaseProperties<Invite>(builder);
        ConfigureBaseProperties<EventLog>(builder);
        
        ConfigureGuests(builder);
        
        base.OnModelCreating(builder);
    }

    public void ConfigureGuests(ModelBuilder builder)
    {
        var entity = builder.Entity<Guest>();
        entity.HasOne(g => g.Invite)
            .WithMany(i => i.Guests)
            .HasForeignKey(g => g.InviteId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }

    /// <summary>
    /// Generates the base values used in all the entities. These exist in <see cref="BaseEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">Domain entity that extends the <see cref="BaseEntity"/></typeparam>
    private void ConfigureBaseProperties<TEntity>(ModelBuilder builder) where TEntity : BaseEntity
    {
        var entity = builder.Entity<TEntity>();     

        entity.HasKey(x => x.Id);
        entity.ToTable(typeof(TEntity).Name);
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
    
}