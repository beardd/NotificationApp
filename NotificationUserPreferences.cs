using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NotificationApp
{
    public class NotificationUserPreferences
    {
        private readonly ILogger<NotificationUserPreferences> _logger;

        public NotificationUserPreferences(ILogger<NotificationUserPreferences> logger)
        {
            _logger = logger;
        }

        [Function(nameof(NotificationUserPreferences))]
        public void Run([QueueTrigger("highqueue", Connection = "samjjnotifapp_STORAGE")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
