namespace Flower_Shop.Models;

public class Seller
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }

    public List<int> SupplierIds { get; set; } = new();
}