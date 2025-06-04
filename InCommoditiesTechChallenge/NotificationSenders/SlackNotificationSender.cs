
using InCommoditiesTechChallenge.NotificationSenders;

namespace InCommoditiesTechChallenge.Notifications
{
    public class SlackNotificationSender: INotificationSender
    {
        public SlackNotificationSender() { 

        }

        public async Task<bool> Send()
        {
            throw new NotImplementedException();
        }
    }
}
