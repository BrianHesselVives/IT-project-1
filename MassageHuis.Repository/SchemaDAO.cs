using MassageHuis.Repositories.Interfaces;
using System.Diagnostics;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;

namespace MassageHuis.Repositories
{
    public class SchemaDAO : IDAO<Schema>
    {
        private readonly MassageHuisDbContext _dbContext;
        public SchemaDAO(MassageHuisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        Task IDAO<Schema>.AddAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        Task IDAO<Schema>.DeleteAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        Task<Schema?> IDAO<Schema>.FindByIdAsync(Schema entity)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Schema>?> IDAO<Schema>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task IDAO<Schema>.UpdateAsync(Schema entity)
        {
            throw new NotImplementedException();
        }
    }
}
