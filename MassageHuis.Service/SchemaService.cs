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
    public class SchemaService : IService<Schema>
    {
        private IDAO<Schema> _schemaDAO;
        public SchemaService(IDAO<Schema> schemaDAO)

        {
            _schemaDAO = schemaDAO;
        }

        Task IService<Schema>.AddAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        Task IService<Schema>.DeleteAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        Task<Schema?> IService<Schema>.FindByIdAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Schema>?> IService<Schema>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task IService<Schema>.UpdateAsync(Schema entity)
        {
            throw new NotImplementedException();
        }
    }
}
