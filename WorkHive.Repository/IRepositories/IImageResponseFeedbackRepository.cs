using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories
{
    public interface IImageResponseFeedbackRepository : IGenericRepository<ImageResponseFeedback>
    {
        public Task CreateImageResponseFeedbackAsync(List<ImageResponseFeedback> imageResponseFeedbacks );


        public Task<List<ImageResponseFeedback>> GetImageResponseFeedbacksByFeedbackIdAsync(int responseId);
    }
}
