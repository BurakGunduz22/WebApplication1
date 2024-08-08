namespace WebApplication1.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public String Brand { get; set; }
        public String Description { get; set; }
        public int Category { get; set; }
        public int? SubCategory { get; set; }
        public int Condition { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? Status { get; set; }
        public int? ViewCount { get; set; }
        public int? LikeCount { get; set; }
        public string? UserId { get; set; }
        public byte[]? ItemImage { get; set; }
        public DateTime Created { get; set; }
    }
}
