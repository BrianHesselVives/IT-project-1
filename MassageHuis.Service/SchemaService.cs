using MassageHuis.Entities;
using MassageHuis.Repositories;
using MassageHuis.Repositories.Interfaces;
using MassageHuis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _schemaDAO.AddAsync(entity);

                scope.Complete(); 
            }
        }

        public Task AddRangeAsync(IEnumerable<Schema> entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Schema entity)
        {
            await _schemaDAO.DeleteAsync(entity);
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

        async public Task UpdateAsync(Schema entity)
        {
            await _schemaDAO.UpdateAsync(entity); 
        }
    }
}
