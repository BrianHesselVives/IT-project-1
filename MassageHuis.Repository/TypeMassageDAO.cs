using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;


namespace MassageHuis.Repositories
{
    public class TypeMassageDAO : IDAO<TypeMassage>
    {
        private readonly MassageHuisDbContext _dbContext;
        public TypeMassageDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TypeMassage entity)
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

        public Task AddRangeAsync(IEnumerable<TypeMassage> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(TypeMassage entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<TypeMassage> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<TypeMassage?> FindByIdAsync(TypeMassage id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TypeMassage>?> GetAllAsync()
        {
            try
            {
                return await _dbContext.TypeMassages.Include(r => r.KostPrijs).ToListAsync();
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

        public Task UpdateAsync(TypeMassage entity)
        {
            throw new NotImplementedException();
        }
    }
}
