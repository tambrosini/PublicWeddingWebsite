using Microsoft.EntityFrameworkCore;
using WeddingInvites.Database;
using WeddingInvites.Domain;

namespace WeddingInvites.Test;

public class TestDataManager<TEntity> : IDisposable
    where TEntity : BaseEntity
{
    private ApplicationDbContext DbContext;
    private DbSet<TEntity> DbSet;
    public string ConnectionString { get; set; } = null!;

    private List<int> IdsToDelete { get; } = new List<int>();

    public TestDataManager(string connectionString)
    {
        ConnectionString = connectionString;
        RefreshDbContext();
    }
    
    private void RefreshDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(ConnectionString);
        DbContext = new ApplicationDbContext(optionsBuilder.Options);
        DbSet = DbContext.Set<TEntity>();
    }
    
    public async Task<TEntity> CreateEntityAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        
        await DbContext.SaveChangesAsync();
        
        IdsToDelete.Add(entity.Id);
        
        return entity;
    }

    public async Task<ICollection<TEntity>> CreateManyAsync(string stringPropertyName, int count)
    {
        List<TEntity> entitiesToCreate = new List<TEntity>();
        for (int i = 0; i < count; i++)
        {
            TEntity entity = Activator.CreateInstance<TEntity>()!;
            entity.GetType().GetProperty(stringPropertyName)!.SetValue(entity, Guid.NewGuid().ToString());
            entitiesToCreate.Add(entity);
        }

        await DbSet.AddRangeAsync(entitiesToCreate);
        
        await DbContext.SaveChangesAsync();
        
        IdsToDelete.AddRange(entitiesToCreate.Select(x => x.Id));
        
        return entitiesToCreate;
    }

    public async Task<List<TEntity>> CreateManyAsync(List<TEntity> entities)
    {
        await DbSet.AddRangeAsync(entities);

        await DbContext.SaveChangesAsync();
        
        IdsToDelete.AddRange(entities.Select(x=> x.Id));

        return entities;
    }

    /// <summary>
    /// Cleans up any entities that were created during the setup portion of a test
    /// </summary>
    public async Task Cleanup()
    {
        // Make sure we have a fresh DbContext before cleanup
        RefreshDbContext();
        
        await DbSet.Where(x => IdsToDelete.Contains(x.Id)).ForEachAsync(x => DbSet.Remove(x));
        
        await DbContext.SaveChangesAsync();
        
        // Clear the IDs list after cleanup
        IdsToDelete.Clear();
    }

    /// <summary>
    /// Manually cleans up any entities that were created during the test
    /// </summary>
    public async Task Cleanup(List<int> ids)
    {
        // Make sure we have a fresh DbContext before cleanup
        RefreshDbContext();
        
        await DbSet.Where(x => ids.Contains(x.Id)).ForEachAsync(x => DbSet.Remove(x));
        
        await DbSet.Where(x => IdsToDelete.Contains(x.Id)).ForEachAsync(x => DbSet.Remove(x));
        
        await DbContext.SaveChangesAsync();
        
        // Clear the IDs list after cleanup
        IdsToDelete.Clear();
    }

    public void Dispose()
    {
        DbContext?.Dispose();
    }

    public async Task GenerateNewDbContext()
    {
        await DbContext.DisposeAsync();
        RefreshDbContext();
    }
}