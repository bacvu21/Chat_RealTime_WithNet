using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RealtimeApp.Hubs;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB
string connectionString = "mongodb://localhost:27017";
string databaseName = "MessageStorage";
var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase(databaseName);
var messageCollection = database.GetCollection<BsonDocument>("Message");

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton(messageCollection); // Register the collection as a singleton

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

// Mapping the SignalR hub
app.MapHub<ChatHub>("/ChatHub");

app.Run();
