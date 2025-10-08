using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magazynek.Services
{
    public interface IProjectService
    {
        Task<List<Project>> Get();
        Task<Project?> GetByID(Guid ID);
        Task<Project> UpdateInfoOrInsertNew(Project product, bool saveChangesAsync = true);
        Task<bool> Remove(Project product, bool saveChangesAsync = true);
    }

    public class ProjectService : IProjectService
    {
        readonly DatabaseContext database;

        public ProjectService(DatabaseContext database) => this.database = database;

        public async Task<List<Project>> Get()
        {
            List<Project> projects = new List<Project>();
            foreach (var proj in await database.Projects.ToListAsync())
            {
                List<ProjectItem> items = await database.ProjectItems.Where(pi => pi.projectID == proj.id).ToListAsync();
                foreach (ProjectItem item in items)
                {
                    item.product = await database.Products.FirstOrDefaultAsync(p => p.id == item.itemID);
                }
                projects.Add(new Project(proj, items));
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
        public async Task<Project> UpdateInfoOrInsertNew(Project project, bool saveChangesAsync = true)
        {
            Console.WriteLine($"Updating or inserting project: {project}");
            Console.WriteLine($"With items:{Environment.NewLine}{string.Join(Environment.NewLine, project.items)}");

            Project? dbProject = await GetByID(project.id);
            if (dbProject != null)
            {
                dbProject.name = project.name;

                // Remove items that are no longer present
                for (int i = 0; i < dbProject.items.Count; i++)
                {
                    if (project.items.Any(x => x.id == dbProject.items[i].id)) continue;
                    database.ProjectItems.Remove(dbProject.items[i]);
                    dbProject.items.Remove(dbProject.items[i]);
                    i--;
                }

                // Update existing items or add new ones
                foreach (var item in project.items)
                {
                    var existing = dbProject.items.FirstOrDefault(i => i.id == item.id);

                    if (existing != null)
                    {
                        existing.itemID = item.itemID;
                        existing.quantity = item.quantity;
                    }
                    else
                    {
                        dbProject.items.Add(item);
                        await database.ProjectItems.AddAsync(item);
                    }
                }
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