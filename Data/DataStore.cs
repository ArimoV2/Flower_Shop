using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Flower_Shop.Models;

namespace Flower_Shop.Data;

public class DataStore
{
    private const string FileName = "data.json";

    public List<Flower> Flowers { get; set; } = new();
    public List<Supplier> Suppliers { get; set; } = new();
    public List<Seller> Sellers { get; set; } = new();

    public static DataStore Load()
    {
        if (!File.Exists(FileName))
            return new DataStore();

        var json = File.ReadAllText(FileName);
        return JsonSerializer.Deserialize<DataStore>(json)
               ?? new DataStore();
    }

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true,  Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
        File.WriteAllText(FileName, JsonSerializer.Serialize(this, options));
    }
}
