using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magazynek.Services
{
    public interface IProjectService
    {
        Task<List<Project>> Get(User user);
        Task<Project?> GetByID(Guid ID);
        Task<Project> UpdateInfoOrInsertNew(User user, Project product, bool saveChangesAsync = true);
        Task<bool> Remove(Project product, bool saveChangesAsync = true);
    }

    public class ProjectService : IProjectService
    {
        readonly DatabaseContext database;

        public ProjectService(DatabaseContext database) => this.database = database;

        public async Task<List<Project>> Get(User user)
        {
            List<Project> projects = await database.Projects
                .Where(p => p.user_id == user.id)
                .ToListAsync();

            foreach (var proj in projects)
            {
                List<ProjectItem> items = await database.ProjectItems.Where(pi => pi.projectID == proj.id).ToListAsync();
                foreach (ProjectItem item in items) {
                    item.product = await database.Products.FirstOrDefaultAsync(p => p.id == item.itemID);
                    proj.items.Add(item);
                }
            }
            return projects;
        }

        public async Task<Project?> GetByID(Guid id)
        {
            Project? p = await database.Projects.FirstOrDefaultAsync(x => x.id == id);
            if (p == null) return null;

            List<ProjectItem> items = await database.ProjectItems.Where(pi => pi.projectID == p.id).ToListAsync();
            p = new Project(p, items);
            return p;
        }
        public async Task<Project> UpdateInfoOrInsertNew(User user, Project project, bool saveChangesAsync = true)
        {
            Console.WriteLine($"Updating or inserting project: {project}");
            Console.WriteLine($"With items:{Environment.NewLine}{string.Join(Environment.NewLine, project.items)}");

            Project? dbProject = await GetByID(project.id);
            if (dbProject != null)
            {
                dbProject = project;
                
                database.ProjectItems.RemoveRange(
                    database.ProjectItems.Where(pi => pi.projectID == dbProject.id)
                );
                foreach (var item in project.items) await database.ProjectItems.AddAsync(item);
            }
            else
            {
                dbProject = new Project(project);
                await database.Projects.AddAsync(dbProject);
                foreach (var item in project.items) await database.ProjectItems.AddAsync(item);
            }

            if (saveChangesAsync) await database.SaveChangesAsync();
            return dbProject;
        }

        public async Task<bool> Remove(Project project, bool saveChangesAsync = true)
        {
            database.Projects.Remove(project);
            if (saveChangesAsync) await database.SaveChangesAsync();
            return true;
        }
    }
}