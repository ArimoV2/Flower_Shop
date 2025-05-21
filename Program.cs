using System.Text;
using Flower_Shop.Data;
using Flower_Shop.Models;

internal class Program {
    
    private static DataStore _store;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding  = Encoding.UTF8;
        Console.Title = "Консольная система: Магазин цветов";
        _store = DataStore.Load();
        MenuLoop();
    }

    private static void MenuLoop()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Магазин цветов ===");
            Console.WriteLine("1. Показать все цветы");
            Console.WriteLine("2. Показать всех поставщиков");
            Console.WriteLine("3. Показать всех продавцов");
            Console.WriteLine("4. Запросы");
            Console.WriteLine("5. Добавить/Удалить цветы у поставщика");
            Console.WriteLine("6. Добавить поставщика");
            Console.WriteLine("7. Добавить цветок");
            Console.WriteLine("8. Добавить продавца");
            Console.WriteLine("9. Редактировать поставщиков у продавца");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт: ");

            switch (Console.ReadLine())
            {
                case "1": ListFlowers(); break;
                case "2": ListSuppliers(); break;
                case "3": ListSellers(); break;
                case "4": QueriesMenu(); break;
                case "5": EditSupplierFlowers(); break;
                case "6": AddSupplier(); break;
                case "7": AddFlower(); break;
                case "8": AddSeller(); break;
                case "9": EditSellerSuppliers(); break; 
                case "0":
                    _store.Save();
                    return;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    private static void ListFlowers()
    {
        Console.WriteLine("-- Список цветов --");
        foreach (var f in _store.Flowers)
            Console.WriteLine($"{f.Id}. {f.Name} | {f.Type} | {f.Country} | {f.Season} | {f.Variety} | {f.Price:F2} ₽");
    }

    private static void ListSuppliers()
    {
        Console.WriteLine("-- Список поставщиков --");
        foreach (var s in _store.Suppliers)
            Console.WriteLine($"{s.Id}. {s.FullName} | {s.FarmType} | {s.Address}");
    }

    private static void ListSellers()
    {
        Console.WriteLine("-- Список продавцов --");
        foreach (var s in _store.Sellers)
        {
            var names = s.SupplierIds
                .Select(id => _store.Suppliers.FirstOrDefault(sp => sp.Id == id)?.FullName)
                .Where(n => n != null);
            Console.WriteLine($"{s.Id}. {s.FullName} | {s.Address} | Поставщики: {string.Join(", ", names)}");
        }
    }

    private static void QueriesMenu()
    {
        Console.Clear();
        Console.WriteLine("--- Запросы ---");
        Console.WriteLine("1. Цветы по поставщику");
        Console.WriteLine("2. Цветы по сезону");
        Console.WriteLine("3. Цветы по стране");
        Console.WriteLine("4. У кого можно купить сорт");
        Console.WriteLine("5. Продавцы самых дорогих цветов");
        Console.WriteLine("6. Совпадающие поставщики у продавцов");
        Console.Write("Выбор: ");

        switch (Console.ReadLine())
        {
            case "1": FlowersBySupplier(); break;
            case "2": FlowersBySeason(); break;
            case "3": FlowersByCountry(); break;
            case "4": WhoSellsVariety(); break;
            case "5": SellersOfMostExpensive(); break;
            case "6": MatchingSuppliers(); break;
            default:
                Console.WriteLine("Неверный выбор.");
                break;
        }
    }

    private static void FlowersBySupplier()
    {
        Console.Write("ID поставщика: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Некорректный ID.");
            return;
        }

        var sup = _store.Suppliers.FirstOrDefault(s => s.Id == id);
        if (sup == null)
        {
            Console.WriteLine("Поставщик не найден.");
            return;
        }

        Console.WriteLine($"Цветы от {sup.FullName}:");
        foreach (var fid in sup.FlowerIds)
        {
            var f = _store.Flowers.FirstOrDefault(x => x.Id == fid);
            if (f != null)
                Console.WriteLine($"- {f.Name} ({f.Variety})");
        }
    }

    private static void EditSellerSuppliers()
    {
        Console.Write("ID продавца: ");
        if (!int.TryParse(Console.ReadLine(), out var sid))
        {
            Console.WriteLine("Некорректный ID.");
            return;
        }

        var sel = _store.Sellers.FirstOrDefault(s => s.Id == sid);
        if (sel == null)
        {
            Console.WriteLine("Продавец не найден.");
            return;
        }

        Console.WriteLine("1. Добавить поставщика");
        Console.WriteLine("2. Удалить поставщика");
        Console.Write("Выбор: ");
        var choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("ID поставщика для добавления: ");
            if (int.TryParse(Console.ReadLine(), out var pid) &&
                _store.Suppliers.Any(sp => sp.Id == pid) &&
                !sel.SupplierIds.Contains(pid))
            {
                sel.SupplierIds.Add(pid);
                _store.Save();
                Console.WriteLine("Поставщик добавлен.");
            }
            else
            {
                Console.WriteLine("Ошибка: неверный ID или уже добавлен.");
            }
        }
        else if (choice == "2")
        {
            Console.Write("ID поставщика для удаления: ");
            if (int.TryParse(Console.ReadLine(), out var pid) &&
                sel.SupplierIds.Remove(pid))
            {
                _store.Save();
                Console.WriteLine("Поставщик удалён.");
            }
            else
            {
                Console.WriteLine("Ошибка: такого поставщика нет в списке.");
            }
        }
        else
        {
            Console.WriteLine("Неверный выбор.");
        }
    }

    
    private static void FlowersBySeason()
    {
        Console.Write("Сезон цветения: ");
        var season = Console.ReadLine() ?? "";
        var list = _store.Flowers
            .Where(f => f.Season.Equals(season, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine($"Цветы сезона '{season}':");
        foreach (var f in list)
            Console.WriteLine($"- {f.Name} ({f.Variety})");
    }

    private static void FlowersByCountry()
    {
        Console.Write("Страна выведения: ");
        var country = Console.ReadLine() ?? "";
        var list = _store.Flowers
            .Where(f => f.Country.Equals(country, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine($"Цветы из '{country}':");
        foreach (var f in list)
            Console.WriteLine($"- {f.Name} ({f.Variety})");
    }

    private static void WhoSellsVariety()
    {
        Console.Write("Название сорта: ");
        var variety = Console.ReadLine() ?? "";

        var suppliers = _store.Suppliers
            .Where(s => s.FlowerIds.Any(id =>
                _store.Flowers.FirstOrDefault(f => f.Id == id)?
                    .Variety.Equals(variety, StringComparison.OrdinalIgnoreCase) == true))
            .ToList();

        Console.WriteLine($"Поставщики сорта '{variety}':");
        foreach (var s in suppliers)
            Console.WriteLine($"- {s.FullName}");
    }

    private static void SellersOfMostExpensive()
    {
        if (!_store.Flowers.Any())
        {
            Console.WriteLine("Нет информации о цветах.");
            return;
        }

        var maxPrice = _store.Flowers.Max(f => f.Price);
        Console.WriteLine($"Самая высокая цена: {maxPrice:C}");
        Console.WriteLine("Поставщики, продающие эти цветы:");

        var flowers = _store.Flowers.Where(f => f.Price == maxPrice);
        foreach (var f in flowers)
        {
            var suppliers = _store.Suppliers.Where(s => s.FlowerIds.Contains(f.Id));
            foreach (var s in suppliers)
                Console.WriteLine($"- {s.FullName} ({f.Name})");
        }
    }

    private static void MatchingSuppliers()
    {
        var allSupplierIds = _store.Sellers
            .SelectMany(s => s.SupplierIds);

        var duplicateIds = allSupplierIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (!duplicateIds.Any())
        {
            Console.WriteLine("Нет поставщиков, которые работают более чем с одним продавцом.");
            return;
        }

        Console.WriteLine("Поставщики, совпадающие у нескольких продавцов:");
        foreach (var id in duplicateIds)
        {
            var sup = _store.Suppliers.FirstOrDefault(s => s.Id == id);
            if (sup != null)
                Console.WriteLine($"- {sup.FullName} (ID={sup.Id})");
        }
    }

    private static void EditSupplierFlowers()
    {
        Console.Write("ID поставщика: ");
        if (!int.TryParse(Console.ReadLine(), out var sid))
        {
            Console.WriteLine("Некорректный ID.");
            return;
        }

        var sup = _store.Suppliers.FirstOrDefault(s => s.Id == sid);
        if (sup == null)
        {
            Console.WriteLine("Поставщик не найден.");
            return;
        }

        Console.WriteLine("1. Добавить цветок");
        Console.WriteLine("2. Удалить цветок");
        Console.Write("Выбор: ");
        var choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("ID цветка для добавления: ");
            if (int.TryParse(Console.ReadLine(), out var fid) &&
                _store.Flowers.Any(f => f.Id == fid))
            {
                sup.FlowerIds.Add(fid);
                _store.Save();
                Console.WriteLine("Цветок добавлен.");
            }
            else Console.WriteLine("Ошибка: цветок не найден.");
        }
        else if (choice == "2")
        {
            Console.Write("ID цветка для удаления: ");
            if (int.TryParse(Console.ReadLine(), out var fid) &&
                sup.FlowerIds.Remove(fid))
            {
                _store.Save();
                Console.WriteLine("Цветок удалён.");
            }
            else Console.WriteLine("Ошибка: данный цветок не привязан.");
        }
        else
        {
            Console.WriteLine("Неверный выбор.");
        }
    }

    private static void AddSeller()
    {
        Console.WriteLine("--- Добавление нового продавца ---");
        var sel = new Seller
        {
            Id = _store.Sellers.Any() 
                ? _store.Sellers.Max(s => s.Id) + 1 
                : 1
        };

        Console.Write("ФИО: ");
        sel.FullName = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Адрес: ");
        sel.Address  = Console.ReadLine()?.Trim() ?? "";

        _store.Sellers.Add(sel);
        _store.Save();

        Console.WriteLine($"Продавец {sel.FullName} (ID={sel.Id}) добавлен.");
    }
    
    private static void AddSupplier()
    {
        Console.WriteLine("--- Добавление нового поставщика ---");
        var sup = new Supplier
        {
            Id = _store.Suppliers.Any() ? _store.Suppliers.Max(s => s.Id) + 1 : 1
        };

        Console.Write("ФИО: ");
        sup.FullName = Console.ReadLine() ?? "";
        Console.Write("Вид хозяйства: ");
        sup.FarmType = Console.ReadLine() ?? "";
        Console.Write("Адрес: ");
        sup.Address = Console.ReadLine() ?? "";

        _store.Suppliers.Add(sup);
        _store.Save();

        Console.WriteLine($"Поставщик {sup.FullName} (ID={sup.Id}) добавлен.");
    }

    private static void AddFlower()
    {
        Console.WriteLine("--- Добавление нового цветка ---");
        var f = new Flower
        {
            Id = _store.Flowers.Any() ? _store.Flowers.Max(x => x.Id) + 1 : 1
        };

        Console.Write("Название: ");
        f.Name = Console.ReadLine() ?? "";
        Console.Write("Тип (Садовый/Комнатный): ");
        f.Type = Console.ReadLine() ?? "";
        Console.Write("Страна: ");
        f.Country = Console.ReadLine() ?? "";
        Console.Write("Сезон цветения: ");
        f.Season = Console.ReadLine() ?? "";
        Console.Write("Сорт: ");
        f.Variety = Console.ReadLine() ?? "";

        Console.Write("Цена: ");
        if (!decimal.TryParse(Console.ReadLine(), out var price))
        {
            Console.WriteLine("Некорректная цена.");
            return;
        }
        f.Price = price;

        Console.Write("ID поставщика: ");
        if (!int.TryParse(Console.ReadLine(), out var supId) ||
            _store.Suppliers.All(s => s.Id != supId))
        {
            Console.WriteLine("Ошибка: такого поставщика нет. Сначала добавьте поставщика.");
            return;
        }

        _store.Flowers.Add(f);
        _store.Suppliers.First(s => s.Id == supId).FlowerIds.Add(f.Id);
        _store.Save();

        Console.WriteLine($"Цветок {f.Name} (ID={f.Id}) добавлен и привязан к поставщику ID={supId}.");
    }
}