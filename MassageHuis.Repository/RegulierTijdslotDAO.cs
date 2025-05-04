using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;


namespace MassageHuis.Repositories
{
    public class RegulierTijdslotDAO : IDAO<RegulierTijdslot>
    {
        private readonly MassageHuisDbContext _dbContext;
        public RegulierTijdslotDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RegulierTijdslot entity)
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

        public Task AddRangeAsync(IEnumerable<RegulierTijdslot> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(RegulierTijdslot entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<RegulierTijdslot> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<RegulierTijdslot?> FindByIdAsync(RegulierTijdslot entity)
        {
            try
            {

                return await _dbContext.RegulierTijdslots.Where(b => b.Id == entity.Id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            { throw new Exception("error DAO Masseur"); }
        }

        public async Task<IEnumerable<RegulierTijdslot>?> GetAllAsync()
        {
            try
            {
                return await _dbContext.RegulierTijdslots.ToListAsync();
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

        public Task UpdateAsync(RegulierTijdslot entity)
        {
            throw new NotImplementedException();
        }
    }
}
