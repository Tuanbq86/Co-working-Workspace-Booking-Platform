using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IFeedbackRepository : IGenericRepository<Feedback>
{
    public Task<Feedback?> GetFeedbackById(int Id);
    public Task<List<Feedback>> GetAllFeedbacks();
    public Task<List<Feedback>> GetFeedbacksByUserId(int userId);
    public Task<List<Feedback>> GetFeedbacksByOwnerId(int ownerId);

}
