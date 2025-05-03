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
    public class ReservatieService : IService<Reservatie>
    {
        private IDAO<Reservatie> _reservatieDAO;
        public ReservatieService(IDAO<Reservatie> reservatieDAO)

        {
            _reservatieDAO = reservatieDAO;
        }
        public async Task AddAsync(Reservatie entity)
        {
            await _reservatieDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(Reservatie entity)
        {
            await _reservatieDAO.DeleteAsync(entity);
        }

        public async Task<Reservatie?> FindByIdAsync(Reservatie id)
        {
            return await _reservatieDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Reservatie>?> GetAllAsync()
        {
            return await _reservatieDAO.GetAllAsync();
        }

        public Task UpdateAsync(Reservatie entity)
        {
            throw new NotImplementedException();
        }
    }
}
