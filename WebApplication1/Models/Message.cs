using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string SenderId { get; set; } // Foreign Key
        public int ConversationId { get; set; }

        [ForeignKey("ConversationId")]
        public Conversation Conversation { get; set; }
    }
}