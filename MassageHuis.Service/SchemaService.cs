using MassageHuis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassageHuis.Entities;
using MassageHuis.Repositories.Interfaces;
using MassageHuis.Repositories;

namespace MassageHuis.Services
{
    public class SchemaService : IService<Schema>
    {
        private IDAO<Schema> _schemaDAO;
        public SchemaService(IDAO<Schema> schemaDAO)

        {
            _schemaDAO = schemaDAO;
        }
        public async Task AddAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<Schema> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<Schema> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<Schema?> FindByIdAsync(Schema id)
        {
            return await _schemaDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Schema>?> GetAllAsync()
        {
            return await _schemaDAO.GetAllAsync();
        }

        public Task UpdateAsync(Schema entity)
        {
            throw new NotImplementedException();
        }
    }
}
