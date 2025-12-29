using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.Persistence;
public static class SeedProductCategoriesFromFile
{
    private const string FilePath = "seed-data/taxonomy-with-ids.en-US.txt";

    public static async Task TrySeedAsync(ApplicationDbContext dbContext)
    {
        Console.WriteLine("Seeding product categories from taxonomy file...");

        if (!File.Exists(FilePath))
        {
            Console.WriteLine($"Taxonomy file not found at path: {FilePath}");
            return;
        }

        var lines = await File.ReadAllLinesAsync(FilePath);
        var categories = new Dictionary<string, ProductCategory>(); // full path => category

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains(">") && !line.Contains("\t"))
                continue;

            // Example line: 2271 - Home & Garden > Kitchen & Dining > Kitchen Appliances > Refrigerators
            var parts = line.Split('-');
            if (parts.Length != 2) continue;

            var rawPath = parts[1].Trim(); // Home & Garden > Kitchen & Dining > ...
            var segments = rawPath.Split('>').Select(s => s.Trim()).ToList();

            string? parentPath = null;
            for (int level = 0; level < segments.Count; level++)
            {
                var currentName = segments[level];
                var currentPath = parentPath == null ? currentName : $"{parentPath} > {currentName}";

                if (!categories.ContainsKey(currentPath))
                {
                    var category = new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        ParentId = parentPath != null && categories.ContainsKey(parentPath)
                            ? categories[parentPath].Id
                            : null,
                        Slug = GenerateSlug(currentName),
                        OrderIndex = level,
                        IsActive = true,
                        Translations = new List<ProductCategoryTranslation>
                        {
                            new ProductCategoryTranslation() { Culture = "en-US", Title = currentName }
                        }
                    };
                    categories[currentPath] = category;
                }

                parentPath = currentPath;
            }
        }

        // جلوگیری از درج دوباره:
        var existingSlugs = dbContext.ProductCategories
            .Select(c => c.Slug)
            .IgnoreQueryFilters()
            .ToHashSet();
        var newCategories = categories.Values
            .Where(c => !existingSlugs.Contains(c.Slug))
            .ToList();

        Console.WriteLine($"Total new product categories to insert: {newCategories.Count}");

        if (newCategories.Any())
        {
            await dbContext.ProductCategories.AddRangeAsync(newCategories);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Product categories seeded successfully.");
        }
        else
        {
            Console.WriteLine("No new product categories found to seed.");
        }
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace("&", "and")
            .Replace(" ", "-")
            .Replace(",", "")
            .Replace("/", "-")
            .Replace("--", "-");
    }
}

