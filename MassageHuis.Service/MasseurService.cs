using MassageHuis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassageHuis.Entities;
using MassageHuis.Repositories.Interfaces;

namespace MassageHuis.Services
{
    public class MasseurService : IService<Masseur>
    {
        private IDAO<Masseur> _masseurDAO;
        public MasseurService(IDAO<Masseur> masseurDAO)

        {
            _masseurDAO = masseurDAO;
        }
        public async Task AddAsync(Masseur entity)
        {
            await _masseurDAO.AddAsync(entity);
        }

        public Task AddRangeAsync(IEnumerable<Masseur> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Masseur entity)
        {
            await _masseurDAO.DeleteAsync(entity);
        }

        public Task DeleteRangeAsync(IEnumerable<Masseur> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<Masseur?> FindByIdAsync(Masseur id)
        {
            return await _masseurDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Masseur>?> GetAllAsync()
        {
            return await _masseurDAO.GetAllAsync();
        }

        public Task UpdateAsync(Masseur entity)
        {
            throw new NotImplementedException();
        }
    }
}
