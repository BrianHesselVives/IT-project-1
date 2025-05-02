using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;


namespace MassageHuis.Repositories
{
    public class MasseurDAO : IDAO<Masseur>
    {
        private readonly MassageHuisDbContext _dbContext;
        public MasseurDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Masseur entity)
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

        public async Task DeleteAsync(Masseur entity)
        {
            try
            {
                var masseurToDelete = await _dbContext.Masseurs.Where(b => b.IdAspNetUsers == entity.IdAspNetUsers && b.Actief == true).FirstOrDefaultAsync();
                if (masseurToDelete != null)
                {
                    masseurToDelete.Actief = false;
                    masseurToDelete.Einddienstverband = DateOnly.FromDateTime(DateTime.Now);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("error DAO Masseur Delete");
            }
        }

        public async Task<Masseur?> FindByIdAsync(Masseur id)
        {
            try
            {

                return await _dbContext.Masseurs.Where(b => b.IdAspNetUsers == id.IdAspNetUsers).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            { throw new Exception("error DAO Masseur"); }
        }

        public async Task<IEnumerable<Masseur>?> GetAllAsync()
        {
            try
            {
                return await _dbContext.Masseurs.Where(b => b.Actief == true).ToListAsync();
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

        public Task UpdateAsync(Masseur entity)
        {
            throw new NotImplementedException();
        }
    }
}
