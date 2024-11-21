using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
namespace RealtimeApp.Models{
using MongoDB.Bson;

public class MessageModels
{
    public ObjectId Id { get; set; }
    public string User { get; set; }
    public string Message { get; set; }
    public string UserCode { get; set; }
    public DateTime Timestamp { get; set; }
}

}
