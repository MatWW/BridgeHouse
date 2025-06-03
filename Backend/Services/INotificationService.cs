namespace Backend.Services;

public interface INotificationService
{
    Task SendJoinTableUpdate(string userId);
    Task SendInvitationUpdate(string userId);
    Task SendLeaveTableUpdate(string userId);
    Task SendTableUpdate(long tableId);
    Task SendDeleteTableUpdate(long tableId);
    Task SendStartOfGameUpdate(long tableId);
    Task SendBidUpdate(long tableId);
    Task SendCardPlayUpdate(long tableId);
    
    Task SendEndOfBiddingUpdate(long tableId);
    Task SendEndOfGameUpdate(long tableId);
}
