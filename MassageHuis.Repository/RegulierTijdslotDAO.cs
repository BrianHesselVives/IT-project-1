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

        async public Task<RegulierTijdslot?> FindByIdAsync(RegulierTijdslot entity)
        {
            throw new NotImplementedException();
        }
        

        Task IDAO<RegulierTijdslot>.AddAsync(RegulierTijdslot entity)
        {
            throw new NotImplementedException();
        }

        Task IDAO<RegulierTijdslot>.DeleteAsync(RegulierTijdslot entity)
        {
            throw new NotImplementedException();
        }

        async Task<IEnumerable<RegulierTijdslot>?> IDAO<RegulierTijdslot>.GetAllAsync()
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

        Task IDAO<RegulierTijdslot>.UpdateAsync(RegulierTijdslot entity)
        {
            throw new NotImplementedException();
        }
    }
}
