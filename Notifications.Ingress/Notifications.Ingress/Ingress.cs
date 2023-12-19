using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Notifications.Ingress;

public static class Ingress
{
    [FunctionName("Ingress")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var notifications = JsonConvert.DeserializeObject<BodyData>(requestBody);

        var highQueueClient = new QueueClient(
            "",
            QueueName.High.ToString());
        var mediumQueueClient = new QueueClient(
            "",
            QueueName.Medium.ToString());
        var lowQueueClient = new QueueClient(
            "",
            QueueName.Low.ToString());

        foreach (var notif in notifications.Data)
        {
            notif.Errored = notif.Notification.Title == null || notif.Notification.Content == null ||
                            notif.Notification.Category == null || notif.Notification.ActionUrl == null ||
                            notif.Notification.Recipients.Count == 0;

            if (notif.Errored) continue;

            using var httpClient = new HttpClient();
            notif.Queue = (await httpClient.GetFromJsonAsync<NotificationGroupTemplate>(
                $"http://localhost:4888/NotificationPriority?Category={notif.Notification.Category}")).Queue);

            switch (notif.Queue)
            {
                case QueueName.High:
                    await highQueueClient.SendMessageAsync(JsonConvert.SerializeObject(notif.Notification));
                    break;
                case QueueName.Medium:
                    await mediumQueueClient.SendMessageAsync(JsonConvert.SerializeObject(notif.Notification));
                    break;
                case QueueName.Low:
                    await lowQueueClient.SendMessageAsync(JsonConvert.SerializeObject(notif.Notification));
                    break;
                default:
                    var exception = new ArgumentOutOfRangeException
                    {
                        HelpLink = null,
                        HResult = 0,
                        Source = null
                    };
                    throw exception;
            }
        }

        return (ActionResult)new AcceptedResult();
    }
}