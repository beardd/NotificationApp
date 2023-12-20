using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Notifications.Ingress;

public static class NotificationHandler
{
    [FunctionName("NotificationHandler")]
    public static async Task RunAsync([QueueTrigger("notifcationapp-high", Connection = "QueueHigh")] BodyData myQueueItem, ILogger log)
    {
        using var httpClient = new HttpClient();
        var textMessageQueueClient = new QueueClient(
             config["samjjnotifapp_STORAGE"],
             $"{NotificationType.TextMessage}_queue");
        var emailQueueClient = new QueueClient(
            config["samjjnotifapp_STORAGE"],
            $"{NotificationType.Email}_queue");
        var fileShareQueueClient = new QueueClient(
            config["samjjnotifapp_STORAGE"],
            $"{NotificationType.FileShare}_queue");


        foreach (var notification in myQueueItem.Data)
        {
            var notificationsToSend =
                await httpClient.GetFromJsonAsync<List<UserNotificationPreferences>>(
                    $"http://localhost:4888/NotificationPriority?Category={notification.Notification.Category}");

            foreach (var notification in notificationsToSend)
            {
                switch (notification.NotificationTypes)
                {
                    case NotificationType.Email:
                        await emailQueueClient.SendMessageAsync(JsonConvert.SerializeObject(notif.Notification));
                        break;
                    case NotificationType.TextMessage:
                        await textMessageQueueClient.SendMessageAsync(JsonConvert.SerializeObject(notif.Notification));
                        break;
                    case NotificationType.FileShare:
                        await fileShareQueueClient.SendMessageAsync(JsonConvert.SerializeObject(notif.Notification));
                        break;
                }
            }
        }
    }
}

/*
 * {
 *      data: [{
 *              "UserKey": "User",
 *              "NotificationType":"TextMessage",
 *              "UserOptions": "{
                                    "FirstName":"Jake",
                                    "LastName":"Beard",
                                    "PhoneNumber":"4198675309",
                                    "EmailAddress":"beardd15@gmail.com",
                                    "FileShareLocation":"",
                                    "FilesShareKey":""
                                }"
 *          }]
 *
 * }
 */
