using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CreateNotification
{
    public static class CreateNotification
    {
        [FunctionName("CreateNotification")]
        public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req, ILogger log)
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var notifications = JsonConvert.DeserializeObject<BodyData>(requestBody);

            var highQueueClient = new QueueClient(
                config["samjjnotifapp_STORAGE"],
                QueueName.High.ToString());
            var mediumQueueClient = new QueueClient(
                config["samjjnotifapp_STORAGE"],
                QueueName.Medium.ToString());
            var lowQueueClient = new QueueClient(
                config["samjjnotifapp_STORAGE"],
                QueueName.Low.ToString());

            foreach (var notif in notifications.Data)
            {
                notif.Errored = notif.Notification.Title == null || notif.Notification.Content == null ||
                                notif.Notification.Category == null || notif.Notification.ActionUrl == null ||
                                notif.Notification.Recipients.Count == 0;

                if (notif.Errored) continue;

                using var httpClient = new HttpClient();
                notif.Queue = (await httpClient.GetFromJsonAsync<NotificationGroupTemplate>(
                    $"{config["BaseApiUri"]}NotificationPriority?Category={notif.Notification.Category}")).Queue;

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
}
