using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;

namespace MassageHuis.Repositories
{
    public class UitzonderingTijdslotDAO : IDAO<UitzonderingTijdslot>
    {
        private readonly MassageHuisDbContext _dbContext;
        public UitzonderingTijdslotDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        async public Task<UitzonderingTijdslot?> FindByIdAsync(UitzonderingTijdslot entity)
        {
            throw new NotImplementedException();
        }
        

        Task IDAO<UitzonderingTijdslot>.AddAsync(UitzonderingTijdslot entity)
        {
            throw new NotImplementedException();
        }

        Task IDAO<UitzonderingTijdslot>.DeleteAsync(UitzonderingTijdslot entity)
        {
            throw new NotImplementedException();
        }

        async Task<IEnumerable<UitzonderingTijdslot>?> IDAO<UitzonderingTijdslot>.GetAllAsync()
        {
            try
            {
                return await _dbContext.UitzonderingTijdslots.ToListAsync();
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

        Task IDAO<UitzonderingTijdslot>.UpdateAsync(UitzonderingTijdslot entity)
        {
            throw new NotImplementedException();
        }
    }
}
