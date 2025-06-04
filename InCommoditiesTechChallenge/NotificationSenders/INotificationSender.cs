namespace InCommoditiesTechChallenge.NotificationSenders
{
    public interface INotificationSender
    {
        Task<bool> Send();
    }
}
