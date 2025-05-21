namespace Flower_Shop.Models;

public class Supplier
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string FarmType { get; set; }
    public string Address { get; set; }
    public List<int> FlowerIds { get; set; } = new();
}