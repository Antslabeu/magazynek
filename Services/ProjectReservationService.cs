using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magazynek.Services
{
    public interface IProjectReservationService
    {
        Task<List<ProjectRealizationViewModel>> Get();
        Task<ProjectRealizationViewModel> UpdateInfoOrInsertNew(ProjectRealizationViewModel product, bool saveChangesAsync = true);
    }

    public class ProjectReservationService : IProjectReservationService
    {
        readonly DatabaseContext database;

        public ProjectReservationService(DatabaseContext database) => this.database = database;
        

        public async Task<List<ProjectRealizationViewModel>> Get()
        {
            List<ProjectRealizationViewModel> realizations = new();
            
            await Task.Delay(0); // Simulate async operation

            return realizations;
        }
        public async Task<ProjectRealizationViewModel> UpdateInfoOrInsertNew(ProjectRealizationViewModel model, bool saveChangesAsync = true)
        {
            await Task.Delay(0);
            return model;
        }
    }
}