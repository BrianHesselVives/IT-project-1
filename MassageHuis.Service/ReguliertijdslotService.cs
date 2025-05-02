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
    public class RegulierTijdslotService : IService<RegulierTijdslot>
    {
        private IDAO<RegulierTijdslot> _regulierTijdslotDAO;
        public RegulierTijdslotService(IDAO<RegulierTijdslot> regulierTijdslotDAO)

        {
            _regulierTijdslotDAO = regulierTijdslotDAO;
        }
        public async Task AddAsync(RegulierTijdslot entity)
        {
            await _regulierTijdslotDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(RegulierTijdslot entity)
        {
            await _regulierTijdslotDAO.DeleteAsync(entity);
        }

        public async Task<RegulierTijdslot?> FindByIdAsync(RegulierTijdslot id)
        {
            return await _regulierTijdslotDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<RegulierTijdslot>?> GetAllAsync()
        {
            return await _regulierTijdslotDAO.GetAllAsync();
        }

        public Task UpdateAsync(RegulierTijdslot entity)
        {
            throw new NotImplementedException();
        }
    }
}
