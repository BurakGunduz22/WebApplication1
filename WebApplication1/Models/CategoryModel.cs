﻿namespace WebApplication1.Models
{
    public class CategoryModel
    {
        public int Id { get; set; } 
        public int? ParentId { get; set; }
        public string Name { get; set; }
    }
}
