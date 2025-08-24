using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magazynek.Services
{
    public interface IProjectService
    {
        Task<List<Project>> Get();
        Task<Project?> GetByID(Guid ID);
        Task<Project> UpdateInfoOrInsertNew(Project product);
        Task<bool> Remove(Project product);
    }

    public class ProjectService : IProjectService
    {
        readonly DatabaseContext database;

        public ProjectService(DatabaseContext database) => this.database = database;

        public Task<List<Project>> Get()
        {
            return database.Projects
                .Include(p => p.items)
                .ToListAsync();
        }

        public Task<Project?> GetByID(Guid id)
        { 
            return database.Projects
                .Include(p => p.items)
                .FirstOrDefaultAsync(x => x.id == id);
        }
        public async Task<Project> UpdateInfoOrInsertNew(Project project)
        {
            Project? dbProject = await database.Projects.FirstOrDefaultAsync(x => x.id == project.id);
            if (dbProject != null) {
                dbProject.name = project.name;
                dbProject.SetItems(project.items);
            }
            else {
                dbProject = new Project(project);
                await database.Projects.AddAsync(dbProject);
            } 

            await database.SaveChangesAsync();
            return dbProject;
        }

        public async Task<bool> Remove(Project project)
        {
            database.Projects.Remove(project);
            await database.SaveChangesAsync();
            return true;
        }
    }
}