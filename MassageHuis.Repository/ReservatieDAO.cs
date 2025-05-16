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
            try
            {

                return await _dbContext.Reservaties.Where(b => b.Id == entity.Id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            { throw new Exception("error DAO Reservatie"); }
        }
        

        async Task IDAO<Reservatie>.AddAsync(Reservatie entity)
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

        async Task IDAO<Reservatie>.DeleteAsync(Reservatie entity)
        {
            // voor het plaatsen van reservering op status geannuleerd
            try
            {
                var annulatieReservatie = await _dbContext.Reservaties.Where(b => b.Id == entity.Id).FirstOrDefaultAsync();
                if (annulatieReservatie != null)
                {
                    annulatieReservatie.Status = "Geannuleerd"; // Status wijzigen naar "Geannuleerd"
                    _dbContext.Update(annulatieReservatie);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("error DAO Masseur Delete");
            }
        }

        async Task<IEnumerable<Reservatie>?> IDAO<Reservatie>.GetAllAsync()
        {
            try
            {
                return await _dbContext.Reservaties
                    .Include(r => r.IdMasseurNavigation) 
                    .ThenInclude(m => m.IdAspNetUsersNavigation)
                    .Include(n=>n.IdTypeMassageNavigation)
                    .Include(o=> o.IdAspNetUsersNavigation)
                    .ToListAsync();
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
