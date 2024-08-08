using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public int ItemId { get; set; } // Foreign Key
        public string ConversationName { get; set; }
        public string SenderId { get; set; }
        public string RecieverId { get; set; }
        public DateTime LastMessageDate { get; set; }
        public ICollection<Message> Messages { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }
    }
}
