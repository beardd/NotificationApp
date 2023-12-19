using System.Collections.Generic;

namespace Notifications.Ingress;


public class BodyData
{
    public List<NotificationGroupTemplate> Data { get; set; }
}
public class NotificationGroupTemplate
{
    public NotificationTemplate Notification { get; set; }
    
    public bool Errored { get; set; }
    public QueueName Queue { get; set; }
}
public class NotificationTemplate
{
   /*
    *
    * notification": {
          "title": "Task assigned to you: Upgrade to Startup plan",
          "content": "Hello, can you upgrade us to the Startup plan. Thank you.",
          "category": "new_message",
          "action_url": "https://magicbell.com/pricing",
          "recipients": [
            { "external_id": "u001" },
            { "external_id": "u002" },  
         ]
      }
    */

   public string Title { get; set; }
   public string Content { get; set; }
   public string Category { get; set; }
   public string ActionUrl { get; set; }
   public string Priority { get; set; }
   public List<NotificationRecipientTemplate> Recipients { get; set; }
   
}

public class NotificationRecipientTemplate
{
    public string external_id { get; set; }
}

public enum QueueName
{
    High,
    Medium,
    Low
}