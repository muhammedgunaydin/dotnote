using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dot_note.Models
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt{ get; set; }
        public DateTime ModifiedAt{ get; set; }
        public string UserId{ get; set;}
    }
}
