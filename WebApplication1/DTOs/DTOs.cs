namespace WebApplication1.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public int ConversationId { get; set; }
    }

    public class ConversationDto
    {
        public int Id { get; set; }
        public string ConversationName { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string RecieverId { get; set; } = string.Empty;
        public DateTime LastMessageDate { get; set; }
    }
}