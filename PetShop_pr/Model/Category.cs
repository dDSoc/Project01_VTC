using System;
using System.Collections.Generic;
public enum Name
{
    Cat,
    Dog
}
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Product> Products { get; set; }
}
