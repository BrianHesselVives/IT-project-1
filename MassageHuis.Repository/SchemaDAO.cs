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
        

        Task IDAO<Schema>.AddAsync(Schema entity)
        {
            throw new NotImplementedException();
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

        Task IDAO<Schema>.UpdateAsync(Schema entity)
        {
            throw new NotImplementedException();
        }
    }
}
