using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;

namespace MassageHuis.Repositories
{
    public class SchemaDAO : IDAO<Schema>
    {
        private readonly MassageHuisDbContext _dbContext;
        public SchemaDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddRangeAsync(IEnumerable<Schema> entities)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<Schema> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<Schema?> FindByIdAsync(Schema entity)
        {
            try
            {
                return await _dbContext.Schemas.Where(b => b.IdMasseur == entity.IdMasseur).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            { 
                throw new Exception("error DAO Masseur"); 
            }
        }


        async Task IDAO<Schema>.AddAsync(Schema entity)
        {
            _dbContext.Schemas.Add(entity); 
            if (entity.RegulierTijdslots != null && entity.RegulierTijdslots.Any())
            {
                foreach (var tijdslot in entity.RegulierTijdslots)
                {
                    _dbContext.Entry(tijdslot).State = EntityState.Added;
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                
                throw;
            }
        }

        Task IDAO<Schema>.DeleteAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        async Task<IEnumerable<Schema>?> IDAO<Schema>.GetAllAsync()
        {
            try
            {
                return await _dbContext.Schemas.ToListAsync();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Debug.WriteLine("db error:", ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        async Task IDAO<Schema>.UpdateAsync(Schema entity)
        {
            _dbContext.Schemas.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
