using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magazynek.Entities;

[Table("project_item")]
public class ProjectItem
{
    [Key] public Guid id { get; private set; }

    [Column("itemid")] [Required] public Guid itemID { get; set; }

    [Required] public int quantity { get; set; }

    [Column("projectid")] [Required] public Guid projectID { get; private set; }


    protected ProjectItem() {
        this.id = Guid.Empty;
        this.itemID = Guid.Empty;
        this.quantity = 0;
        this.projectID = Guid.Empty;
    }
    public ProjectItem(Product? product, int quantity, Project project)
    {
        this.id = Guid.NewGuid();
        if(product == null) this.itemID = Guid.Empty;
        else this.itemID = product.id;
        this.quantity = quantity;
        projectID = project.id;
    }

    public override string ToString()
    {
        return $"ProjectItem: {itemID} (Quantity: {quantity}) in Project: {projectID}";
    }
}

[Table("project")]
public class Project
{
    [Required] public Guid id { get; private set; }
    [Required] public string name { get; set; } = string.Empty;

    [NotMapped] public List<ProjectItem> items { get; private set; } = new List<ProjectItem>();

    protected Project() { }
    public Project(string name)
    {
        this.id = Guid.NewGuid();
        this.items = new List<ProjectItem>();
        this.name = name;
    }
    public Project(Project model, List<ProjectItem>? items = null)
    {
        this.id = model.id;
        this.name = model.name;
        if (items == null) this.items = new(model.items);
        else this.items = new(items);
    }
    public void AddItem(Product? product, int quantity) => this.items.Add(new ProjectItem(product, quantity, this));
    public void Removeitem(ProjectItem item) => this.items.Remove(item);
    public void SetItems(List<ProjectItem> items) => this.items = items;

    public override string ToString()
    {
        return $"Project: {name} (ID: {id})";
    }

    public void PrintMe()
    {
        Console.WriteLine(this.ToString());
        foreach (var item in items)
        {
            Console.WriteLine($"  - {item}");
        }
    }
}