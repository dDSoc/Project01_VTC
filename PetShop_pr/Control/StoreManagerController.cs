using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using Spectre.Console;
using System.Text.RegularExpressions;

public static class StoreManagerController
{
    // Store management menu
    public static void StoreManagementMenu()
    {
        while (true)
        {
            Menu.StoreManagerMenu(); // Display store management menu
            AnsiConsole.Markup("[bold green]Enter your choice: [/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ProductController.ManagementProduct();  // Call product management function
                    break;
                case "2":
                    CategoryController.ManagementCategory(); // Call category management function
                    break;
                case "3":
                    OderController.ManagementOrder(); // Call order management function
                    break;
                case "4":
                    DashboardController.DashboardManagement(); // Call dashboard management function
                    break;
                case "0":
                    UserController.Logout();
                    if (UserController.user == null)
                    {
                        MenuController.DefaultMenuController(); // Call default menu controller function
                    }
                    break;
            }
        }
    }


}