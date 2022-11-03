
using BasicBilling.Service.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicBilling.Service.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DatabaseContext databaseContext;
        
        public Repository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public IQueryable<T> Query => databaseContext.Set<T>().AsNoTracking().AsQueryable();

        public async Task<T> Create(T model)
        {
            var entry = databaseContext.Entry(model);
            entry.State = EntityState.Added;
            await SaveChangesAsync();
            entry.State = EntityState.Detached;
            return model;
        }

        public async Task<T> Update(T model)
        {
            var entry = databaseContext.Entry(model);
            entry.State = EntityState.Modified;

            var currentValues = await entry.GetDatabaseValuesAsync();

            if (currentValues != null)
                entry.OriginalValues.SetValues(currentValues);

            foreach (var property in entry.Properties)
            {
                if (property?.OriginalValue == null && property?.CurrentValue != null)
                    property.IsModified = true;

                if (property.IsModified && property?.CurrentValue == null && property?.OriginalValue != null)
                    property.IsModified = true;

                if (property.IsModified && property?.CurrentValue == null && property?.OriginalValue == null)
                    property.IsModified = false;

                if (property.IsModified && property?.OriginalValue == property?.CurrentValue)
                    property.IsModified = false;

                if (property.IsModified && property.OriginalValue != null && property.CurrentValue != null && property.OriginalValue.Equals(property.CurrentValue))
                    property.IsModified = false;

                if (property.Metadata.PropertyInfo == null)
                    continue;
            }

            await SaveChangesAsync();
            entry.State = EntityState.Detached;
            return model;
        }

        public async Task<T> Delete(T model)
        {
            var entry = databaseContext.Entry(model);
            entry.State = EntityState.Deleted;
            await SaveChangesAsync();
            entry.State = EntityState.Detached;
            return model;
        }


        private Task<int> SaveChangesAsync()
        {
            return databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> Create(IEnumerable<T> models)
        {
            var response = new List<EntityEntry<T>>();
            foreach (var model in models)
            {
                var entry = databaseContext.Entry(model);
                entry.State = EntityState.Added;
                response.Add(entry);
            }

            await SaveChangesAsync();

            foreach (var entry in response)
            {
                entry.State = EntityState.Detached;
            }

            return models;
        }
    }
}
