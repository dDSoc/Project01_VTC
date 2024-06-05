using System;
using Spectre.Console;
using System.Collections.Generic;

public static class SearchController
{
    // public static void SearchMenu()
    // {
    //     while (true)
    //     {
    //         Menu.CustomerMenu_Search();
    //         string choice = Console.ReadLine();
    //         switch (choice)
    //         {
    //             case "1":
    //                 SearchProductByName();
    //                 break;
    //             // case "2":
    //             //     SearchCategory();
    //             //     break;
    //             case "0":
    //                 return;
    //             default:
    //                 Console.WriteLine("Chức năng không tồn tại ở đây");
    //                 Console.ReadKey();
    //                 break;
    //         }
    //     }
    // }
    public static void SearchProductByName()
    {
        Console.Clear();
        Console.WriteLine("Nhập tên sản phẩm cần tìm: ");
        string productName = Console.ReadLine();
        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Name.Contains(productName)).ToList();
        MenuController.products = products;
        // if (products.Count == 0)
        // {
        //     Console.WriteLine("Not Found");
        // }
        // else
        // {
        //     Console.WriteLine("Found " + products.Count + " products");
        //     var table = new Table();
        //     table.AddColumn("ID");
        //     table.AddColumn("Tên sản phẩm");
        //     table.AddColumn("Mô tả");
        //     table.AddColumn("Giá");
        //     table.AddColumn("Số lượng");

        //     int pageSize = 5;
        //     int currentPage = 1;
        //     int totalPages = (int)Math.Ceiling((double)products.Count / pageSize);

        //     while (true)
        //     {
        //         Console.Clear();
        //         Console.WriteLine($"Page {currentPage}/{totalPages}");
        //         Console.WriteLine();

        //         for (int i = (currentPage - 1) * pageSize; i < currentPage * pageSize && i < products.Count; i++)
        //         {
        //             var product = products[i];
        //             table.AddRow(
        //                 product.Id.ToString(),
        //                 product.Name,
        //                 product.Description,
        //                 product.Price.ToString(),
        //                 product.Stock.ToString()
        //             );
        //         }

        //         AnsiConsole.Render(table);

        //         Console.WriteLine();
        //         Console.WriteLine("Press 'L' for previous page, 'R' for next page, or any other key to exit.");
        //         var key = Console.ReadKey(true).Key;

        //         if (key == ConsoleKey.LeftArrow && currentPage > 1)
        //         {
        //             currentPage--;
        //             table.Rows.Clear();
        //         }
        //         else if (key == ConsoleKey.RightArrow && currentPage < totalPages)
        //         {
        //             currentPage++;
        //             table.Rows.Clear();
        //         }
        //         else
        //         {
        //             break;
        //         }
        //     }
        // }
        // Console.ReadKey();
        // return products;
    }
    public static void SearchProductByCategory()
    {
        Console.WriteLine("Nhập tên danh mục cần tìm: ");
        string categoryName = Console.ReadLine();
        using var db = new ApplicationDbContext();
        var category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
        if (category == null)
        {
            Console.WriteLine("Không tìm thấy danh mục");
            Console.ReadKey();
        }
        var products = db.Products.Where(p => p.CategoryId == category.Id).ToList();
        MenuController.products = products;
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