using MassageHuis.Entities;
using MassageHuis.Repositories.Interfaces;
using MassageHuis.Services.Interfaces;

namespace MassageHuis.Services
{
    public class KostPrijsService : IService<KostPrijs>
    {
        private IDAO<KostPrijs> _KostPrijsDAO;
        public KostPrijsService(IDAO<KostPrijs> KostPrijsDAO)
        {
            _KostPrijsDAO = KostPrijsDAO;
        }
        public async Task AddAsync(KostPrijs entity)
        {
            await _KostPrijsDAO.AddAsync(entity);
        }

        public Task AddRangeAsync(IEnumerable<KostPrijs> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(KostPrijs entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<KostPrijs> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<KostPrijs?> FindByIdAsync(KostPrijs id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<KostPrijs>?> GetAllAsync()
        {
            return await _KostPrijsDAO.GetAllAsync();
        }

        public Task UpdateAsync(KostPrijs entity)
        {
            throw new NotImplementedException();
        }
    }
}
