using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;

namespace MassageHuis.Repositories
{
    public class ReservatieDAO : IDAO<Reservatie>
    {
        private readonly MassageHuisDbContext _dbContext;
        public ReservatieDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddRangeAsync(IEnumerable<Reservatie> entities)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<Reservatie> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<Reservatie?> FindByIdAsync(Reservatie entity)
        {
            throw new NotImplementedException();
        }
        

        Task IDAO<Reservatie>.AddAsync(Reservatie entity)
        {
            throw new NotImplementedException();
        }

        Task IDAO<Reservatie>.DeleteAsync(Reservatie entity)
        {
            throw new NotImplementedException();
        }

        async Task<IEnumerable<Reservatie>?> IDAO<Reservatie>.GetAllAsync()
        {
            try
            {
                return await _dbContext.Reservaties.ToListAsync();
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

        Task IDAO<Reservatie>.UpdateAsync(Reservatie entity)
        {
            throw new NotImplementedException();
        }
    }
}
