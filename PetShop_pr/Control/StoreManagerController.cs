using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using Spectre.Console;
using System.Text.RegularExpressions;

public static class StoreManagerController
{
    public static void StoreManagementMenu()
    {
        while (true)
        {
            Menu.StoreManagerMenu();
            AnsiConsole.MarkupLine("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ProductController.ManagementProduct();
                    break;
                case "2":
                    CategoryController.ManagementCategory();
                    break;
                case "3":
                    OderController.ManagementOrder();
                    break;
                case "4":
                    //DashboardController.ShowDashboard();
                    break;
                case "0":
                    UserController.Logout();
                    if (UserController.user == null)
                    {
                        MenuController.DefaultMenuController();
                    }
                    break;
            }
        }
    }


}