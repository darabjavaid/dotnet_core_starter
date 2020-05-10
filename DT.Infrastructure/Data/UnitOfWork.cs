using SD.BuildingBlocks.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DT.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDBContext _dbContext;

        public UnitOfWork(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int AffectedRows {get; private set;}

        public int Commit()
        {
            AffectedRows = _dbContext.SaveChanges();
            return AffectedRows;
        }

        public async Task<int> CommitAsync()
        {
            AffectedRows = await _dbContext.SaveChangesAsync();
            return AffectedRows;
        }
    }
}
