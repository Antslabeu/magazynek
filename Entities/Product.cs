using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magazynek.Entities;

[Table("product")]
public class Product
{
    [Key] public Guid id { get; protected set; }
    [Required] public string name { get; private set; }
    [Required] public string package { get; private set; }
    [Required] public string description { get; private set; }
    [Required] public bool active { get; private set; }
    [Column("farnellid")][Required] public string farnellID { get; private set; }
    [Column("tmeid")][Required] public string tmeID { get; private set; }
    [Required] public string type { get; private set; }

    public Product(string name, string package, string farnellID, string tmeID, string description, bool active, string type)
    {
        this.id = Guid.NewGuid();
        this.name = name;
        this.package = package;
        this.farnellID = farnellID;
        this.tmeID = tmeID;
        this.description = description;
        this.active = active;
        this.type = type;
    }
    protected Product(ProductViewModel model)
    {
        this.id = model.id;
        this.name = model.name;
        this.package = model.package;
        this.farnellID = model.farnellID;
        this.tmeID = model.tmeID;
        this.description = model.description;
        this.active = model.active;
        this.type = model.type;
    }

    protected Product()
    {
        this.id = Guid.Empty;
        this.name = "";
        this.package = "";
        this.farnellID = "";
        this.tmeID = "";
        this.description = "";
        this.active = true;
        this.type = "NO TYPE";
    }
    public static Product Temporary()
    {
        return new Product();
    }

    public void SetName(string name) => this.name = name;
    public void SetPackage(string package) => this.package = package;
    public void SetFarnellID(string farnellID) => this.farnellID = farnellID;
    public void SetTmeID(string tmeID) => this.tmeID = tmeID;
    public void SetDescription(string description) => this.description = description;
    public void SetActive(bool active) => this.active = active;
    public void SetType(string type) => this.type = type;

    public override string ToString() => $"PRODUCT: {name}: {description} ({package})";

    public static void AddToList(List<Product> list, ProductViewModel viewModel) => list.Add(new Product(viewModel));
    public static string GetSafeDefaultType(List<string> types)
    {
        if (types.Count == 0) return "NO TYPE";
        return types[0];
    }
}


public class ProductViewModel
{
    public Guid id { get; private set; }
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public string package { get; set; } = "";
    public string farnellID { get; set; } = "";
    public string tmeID { get; set; } = "";
    public bool active { get; set; }
    public string type { get; set; }

    public ProductViewModel(Product product)
    {
        this.id = product.id;
        this.name = product.name;
        this.package = product.package;
        this.farnellID = product.farnellID;
        this.tmeID = product.tmeID;
        this.description = product.description;
        this.active = product.active;
        this.type = product.type;
    }
}
