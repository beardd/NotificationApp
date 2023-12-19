using System.Net.Http;
using System.Net.Http.Json;

namespace Notifications.Ingress;

public class Notification
{
    public void getNotificationUsersAndPreferences()
    {
        using var httpClient = new HttpClient();
        (await httpClient.GetFromJsonAsync<NotificationGroupTemplate>(
            $"http://localhost:4888/NotificationPriority?Category={notif.Notification.Category}")).Queue);

    }
}

public class User