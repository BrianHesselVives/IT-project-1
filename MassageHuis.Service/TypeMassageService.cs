using MassageHuis.Entities;
using MassageHuis.Repositories;
using MassageHuis.Repositories.Interfaces;
using MassageHuis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassageHuis.Services
{
    public class TypeMassageService : IService<TypeMassage>
    {
        private IDAO<TypeMassage> _TypeMassageDAO;
        public TypeMassageService(IDAO<TypeMassage> TypeMassageDAO)
        {
            _TypeMassageDAO = TypeMassageDAO;
        }
        public async Task AddAsync(TypeMassage entity)
        {
            await _TypeMassageDAO.AddAsync(entity);
        }

        public Task AddRangeAsync(IEnumerable<TypeMassage> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(TypeMassage entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<TypeMassage> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<TypeMassage?> FindByIdAsync(TypeMassage id)
        {
            return await _TypeMassageDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<TypeMassage>?> GetAllAsync()
        {
            return await _TypeMassageDAO.GetAllAsync();
        }

        public Task UpdateAsync(TypeMassage entity)
        {
            throw new NotImplementedException();
        }
    }
}
