namespace Backend.Services;

public interface IPlayerStateService
{
    Task<long?> GetSignedInPlayerTableIdAsync();
    Task<long?> GetSignedInPlayerInviteTableIdAsync();

    Task<long?> GetSignedInPlayerGameIdAsync();
}
