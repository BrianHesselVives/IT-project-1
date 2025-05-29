using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;


namespace MassageHuis.Repositories
{
    public class KostPrijsDAO : IDAO<KostPrijs>
    {
        private readonly MassageHuisDbContext _dbContext;
        public KostPrijsDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(KostPrijs entity)
        {

            _dbContext.Entry(entity).State = EntityState.Added;
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

        public Task AddRangeAsync(IEnumerable<KostPrijs> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(KostPrijs entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<KostPrijs> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<KostPrijs?> FindByIdAsync(KostPrijs id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<KostPrijs>?> GetAllAsync()
        {
            try
            {
                return await _dbContext.KostPrijs.Include(r => r.IdTypeMassageNavigation).ToListAsync();
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

        public Task UpdateAsync(KostPrijs entity)
        {
            throw new NotImplementedException();
        }
    }
}
