using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Notifications.Ingress;

public static class NotificationIngress
{
    [FunctionName("NotificationIngress")]
    [return: Queue("notifcationapp-high",  Connection = "QueueHigh")]
    public static async Task<BodyData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var notifications = JsonConvert.DeserializeObject<BodyData>(requestBody);
        bool erroredNotification = false;
        
        foreach (var notif in notifications.Data)
        erroredNotification = notif.Notification.Title == null || notif.Notification.Content == null || notif.Notification.Category == null || notif.Notification.ActionUrl == null || notif.Notification.Recipients.Count == 0;

        return notifications;
    }
}