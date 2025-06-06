using MassageHuis.Entities;
using MassageHuis.Repositories.Interfaces;
using MassageHuis.Services.Interfaces;

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

        public Task AddRangeAsync(IEnumerable<Reservatie> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Reservatie entity)
        {
            await _reservatieDAO.DeleteAsync(entity);
        }

        public Task DeleteRangeAsync(IEnumerable<Reservatie> entities)
        {
            throw new NotImplementedException();
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
