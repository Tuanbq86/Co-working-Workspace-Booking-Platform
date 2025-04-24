using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories
{
    public interface IOwnerWithdrawalRequestRepository : IGenericRepository<OwnerWithdrawalRequest>
    {
        Task<List<OwnerWithdrawalRequest>> GetByOwnerIdAsync(int ownerId);

        Task<OwnerWithdrawalRequest?> GetWithdrawalRequestByIdAsync(int id);
    }
}
