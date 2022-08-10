using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wishmaster.DataAccess;
using Wishmaster.DataAccess.Models;

namespace Wishmaster.Backend.Services
{
    public interface ISpaceService
    {
        Task<IEnumerable<Space>> GetListAsync();
    }

    public class SpaceService : ISpaceService
    {
        private readonly Db _db;

        public SpaceService(Db db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Space>> GetListAsync()
        {
            return await _db.Spaces.AsNoTracking().ToArrayAsync();
        }
    }
}
