using MassageHuis.Entities;
using MassageHuis.Repositories.Interfaces;
using MassageHuis.Services.Interfaces;

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

        public Task AddRangeAsync(IEnumerable<RegulierTijdslot> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(RegulierTijdslot entity)
        {
            await _regulierTijdslotDAO.DeleteAsync(entity);
        }

        public Task DeleteRangeAsync(IEnumerable<RegulierTijdslot> entities)
        {
            throw new NotImplementedException();
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
