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

        foreach (var notification in myQueueItem.Data)
        {
            var notificationsToSend =
                await httpClient.GetAsync(
                    $"http://localhost:4888/NotificationPriority?Category={notification.Notification.Category}");


            
            Json.Decode
            
            var obj = JsonConvert.DeserializeObject(await notificationsToSend.Content.ReadAsStringAsync());
            
            (INotificationType)obj.NotificationType
            
        }




        log.LogInformation($"C# Queue trigger function processed: {myQueueItem.Data.First().Notification.Category}");
        //{myQueueItem.Data.First().Notification.Category}");
        
    }
}

/*
 * {
 *      data: [{
 *              "UserKey": "User",
 *              "NotificationType":"TextMessage",
 *              "":""
 *          }]
 *
 * }
 */
public interface INotificationType
{
    public string UserKey { get; set; }
    public string NotificationType { get; set; }
}

public class EmailNotification : INotificationType
{
    public string UserKey { get; set; }
    public string NotificationType { get; set; }
    public string EmailAddress { get; set; }
}

public class SmsNotification : INotificationType
{
    public string UserKey { get; set; }
    public string NotificationType { get; set; }
    public string PhoneNumber { get; set; }
}

public class FileShareNotification : INotificationType
{
    public string UserKey { get; set; }
    public string NotificationType { get; set; }
    public string FileShareLocation { get; set; }
    public string FileShareKey { get; set; }
}

