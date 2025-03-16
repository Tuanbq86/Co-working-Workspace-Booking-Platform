using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IImageFeedbackRepository : IGenericRepository<ImageFeedback>
{
    public Task CreateImageFeedbackAsync(List<ImageFeedback> imageFeedbacks);


    public Task<List<ImageFeedback>> GetImageFeedbacksByFeedbackIdAsync(int feedbackId);

}
