using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using magazynek.Data;

namespace Magazynek.Entities;


[Table("shipping_entry")]
public class ShippingEntry
{
    [Required] public Guid id { get; private set; }
    [Required] public Guid item { get; private set; }
    [Required] public uint quantity { get; private set; }
    [Required] public DateTime last_check { get; private set; }
    [Required] public uint stock { get; private set; }
    [Required] public float price { get; private set; }

    protected ShippingEntry() { }
    public ShippingEntry(Guid item, uint quantity, DateTime last_check, uint stock, float price)
    {
        this.id = Guid.NewGuid();
        this.item = item;
        this.quantity = quantity;
        this.last_check = last_check;
        this.stock = stock;
        this.price = price;
    }
    protected ShippingEntry(ShippingEntryViewModel model)
    {
        this.id = model.id;
        this.item = model.product.id!;
        this.quantity = (uint)model.quantity;
        this.last_check = model.last_check;
        this.stock = (uint)model.stock;
        this.price = model.price;
    }

    public void SetItem(Guid item) => this.item = item;
    public void SetQuantity(uint quantity) => this.quantity = quantity;
    public void SetLastCheck(DateTime last_check) => this.last_check = last_check;
    public void SetStock(uint stock) => this.stock = stock;
    public void SetPrice(float price) => this.price = price;
}

public class ShippingEntryViewModel
{
    [EditableField("Id", IsEditable: false)] public Guid id { get; set; }
    [EditableField("Id", IsEditable: false)] public Product product { get; set; }
    public int quantity { get; set; }
    public DateTime last_check { get; set; }
    public uint stock { get; set; }
    public float price { get; set; }
    public float stockValue => quantity * price;

    public ShippingEntryViewModel(ShippingEntry ShippingEntry, Product product)
    {
        this.id = ShippingEntry.id;
        this.product = product;
        this.quantity = (int)ShippingEntry.quantity;
        this.last_check = ShippingEntry.last_check;
        this.stock = ShippingEntry.stock;
        this.price = ShippingEntry.price;
    }
    
    public static ShippingEntryViewModel Empty() =>
        new ShippingEntryViewModel(
            new ShippingEntry(Guid.Empty, 0, DateTime.MinValue, 0, 0),
            Product.Temporary()
        );
}