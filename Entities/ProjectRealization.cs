using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Magazynek.Entities
{
    [Table("project_realization")]
    public class ProjectRealization
    {
        [Key] public Guid id { get; private set; }
        [Column("project_id")] public Guid projectID { get; private set; }
        [Column("name") ] public string name { get; private set; } = string.Empty;
        [Column("start_date")] public DateTime startDate { get; private set; }
        [Column("end_date")] public DateTime? endDate { get; private set; }
        [Column("quantity")] public int quantity { get; private set; }
        [Column("notes")] public string? notes { get; private set; }


        private ProjectRealization() { }

        public ProjectRealization(ProjectRealizationViewModel model) 
        {
            id = model.id;
            if(model.project != null) projectID = model.project.id;
            name = model.name;
            startDate = model.startDate;
            endDate = model.endDate;
            quantity = model.quantity;
            notes = model.notes;
        }
    }

    public class ProjectRealizationViewModel 
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public Project? project { get; set; }
        public Guid projectID { get; set; }
        public List<ProjectItem> items { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int quantity { get; set; } = 1;
        public string? notes { get; set; }

        public ProjectRealizationViewModel(string name)
        {
            this.id = Guid.NewGuid();
            this.name = name;
            this.project = new Project("");
            this.projectID = Guid.Empty;
            this.items = new List<ProjectItem>();
            this.startDate = DateTime.Now;
            this.endDate = null;
            this.quantity = 0;
            this.notes = null;
        }
        public ProjectRealizationViewModel(ProjectRealization realization, Project project, List<ProjectItem> items)
        {
            this.id = realization.id;
            this.project = project;
            this.projectID = project.id;
            this.name = realization.name;
            this.items = items;
            this.startDate = realization.startDate;
            this.endDate = realization.endDate;
            this.quantity = realization.quantity;
            this.notes = realization.notes;
        }
    }
}