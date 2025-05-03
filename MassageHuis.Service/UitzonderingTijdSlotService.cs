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
    public class UitzonderingTijdslotService : IService<UitzonderingTijdslot>
    {
        private IDAO<UitzonderingTijdslot> _uitzonderingTijdslotDAO;
        public UitzonderingTijdslotService(IDAO<UitzonderingTijdslot> uitzonderingTijdslotDAO)

        {
            _uitzonderingTijdslotDAO = uitzonderingTijdslotDAO;
        }
        public async Task AddAsync(UitzonderingTijdslot entity)
        {
            await _uitzonderingTijdslotDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(UitzonderingTijdslot entity)
        {
            await _uitzonderingTijdslotDAO.DeleteAsync(entity);
        }

        public async Task<UitzonderingTijdslot?> FindByIdAsync(UitzonderingTijdslot id)
        {
            return await _uitzonderingTijdslotDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<UitzonderingTijdslot>?> GetAllAsync()
        {
            return await _uitzonderingTijdslotDAO.GetAllAsync();
        }

        public Task UpdateAsync(UitzonderingTijdslot entity)
        {
            throw new NotImplementedException();
        }
    }
}
