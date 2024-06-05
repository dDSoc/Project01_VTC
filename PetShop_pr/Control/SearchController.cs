using System;
using Spectre.Console;
using System.Collections.Generic;

public static class SearchController
{
    public static void SearchProductByName()
    {
        Console.Clear();
        Console.WriteLine("Nhập tên sản phẩm cần tìm: ");
        string productName = Console.ReadLine();
        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Name.Contains(productName)).ToList();
        MenuController.products = products;
    }
    public static void SearchProductByCategory()
    {
        Console.WriteLine("Nhập tên danh mục cần tìm: ");
        string categoryName = Console.ReadLine().Trim(); // Xóa khoảng trắng ở đầu và cuối

        using var db = new ApplicationDbContext();

        // In ra giá trị của categoryName để kiểm tra
        Console.WriteLine($"Tìm kiếm danh mục với tên: {categoryName}");

        var category = db.Categories.FirstOrDefault(c => c.Name.ToLower().Contains(categoryName.ToLower()));

        // In ra tất cả các danh mục để kiểm tra
        var allCategories = db.Categories.ToList();
        Console.WriteLine("Danh sách tất cả các danh mục trong cơ sở dữ liệu:");
        foreach (var cat in allCategories)
        {
            Console.WriteLine($"- {cat.Name}");
        }

        if (category == null)
        {
            Console.WriteLine("Không tìm thấy danh mục");
            Console.ReadKey();
        }
        else
        {
            var products = db.Products.Where(p => p.CategoryId == category.Id).ToList();
            MenuController.products = products;

            // In ra danh sách sản phẩm để kiểm tra
            Console.WriteLine("Danh sách các sản phẩm trong danh mục tìm thấy:");
            foreach (var product in products)
            {
                Console.WriteLine($"- {product.Name}");
            }
            MenuController.products = products;
        }


    }
    public static void SearchProductByPriceRange()
    {
        Console.WriteLine("Nhập giá thấp nhất: ");
        decimal minPrice = decimal.Parse(Console.ReadLine());
        Console.WriteLine("Nhập giá cao nhất: ");
        decimal maxPrice = decimal.Parse(Console.ReadLine());
        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        MenuController.products = products;
    }
}