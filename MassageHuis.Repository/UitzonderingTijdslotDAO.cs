using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MassageHuis.Repositories
{
    public class UitzonderingTijdslotDAO : IDAO<UitzonderingTijdslot>
    {
        private readonly MassageHuisDbContext _dbContext;
        public UitzonderingTijdslotDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UitzonderingTijdslot>?> GetAllAsync()
        {
            return await _dbContext.UitzonderingTijdslots.ToListAsync();
        }

        public async Task<UitzonderingTijdslot?> FindByIdAsync(UitzonderingTijdslot entity)
        {
            return await _dbContext.UitzonderingTijdslots.FindAsync(entity.Id);
        }

        public async Task AddAsync(UitzonderingTijdslot entity)
        {
            _dbContext.UitzonderingTijdslots.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<UitzonderingTijdslot> entities)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    await _dbContext.UitzonderingTijdslots.AddRangeAsync(entities);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task DeleteAsync(UitzonderingTijdslot entity)
        {
            _dbContext.UitzonderingTijdslots.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<UitzonderingTijdslot> entities)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.UitzonderingTijdslots.RemoveRange(entities);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw; // Gooi de exception opnieuw om de service te informeren over de fout
                }
            }
        }

        public Task UpdateAsync(UitzonderingTijdslot entity)
        {
            throw new NotImplementedException();
        }
    }
}