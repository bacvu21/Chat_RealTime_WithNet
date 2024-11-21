using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using RealtimeApp.Models;
namespace RealtimeApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<BsonDocument> _messageCollection;

        public ChatHub(IMongoCollection<BsonDocument> messageCollection)
        {
            _messageCollection = messageCollection;
        }

        public async Task SendMessage(string user, string message, string codeID)
        {
            // Create a new BSON document
            var document = new BsonDocument
            {
                { "User", user },
                { "Message", message },
                { "userCode", codeID },
                { "Timestamp", DateTime.UtcNow }
            };

            // Insert the message into the MongoDB collection
            await _messageCollection.InsertOneAsync(document);

            // Notify all clients
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.UtcNow, codeID);
        }

        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
        public async Task<List<BsonDocument>> GetMessages(string searchString)
        {
            var filter = Builders<BsonDocument>.Filter.Regex("Message", new BsonRegularExpression(searchString, "i"));
            var messages = await _messageCollection.Find(filter).ToListAsync();
            return messages;
        }

        public async Task<List<MessageModels>> GetMessagesByCodeID(string codeID)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("userCode", codeID);
                var bsonMessages = await _messageCollection.Find(filter).ToListAsync();

                // Convert BsonDocuments to Models
                var messages = bsonMessages.Select(doc => new MessageModels
                {
                    User = doc.GetValue("User").AsString,
                    Message = doc.GetValue("Message").AsString,
                    UserCode = doc.GetValue("userCode").AsString,
                    Timestamp = doc.GetValue("Timestamp").ToUniversalTime()  // Ensure it's DateTime
                }).ToList();

                // Log the number of messages found
                Console.WriteLine($"Found {messages.Count} messages for codeID: {codeID}");

                return messages;
            }
            catch (Exception ex)
            {
                // Log the exception message
                Console.WriteLine($"Error retrieving messages for codeID {codeID}: {ex.Message}");
                // Optionally, you can rethrow the exception or return an empty list
                return new List<MessageModels>();
            }
        }

    }
}
