using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Magazynek.Services;
public record FlashMessage(string Text, FlashMessageType Type);
public enum FlashMessageType
{
    info, error, success
}
public interface IFlashService
{
    
    Task SetAsync(FlashMessage msg);
    Task<FlashMessage?> ConsumeAsync();
}

public sealed class FlashService : IFlashService
{
    private const string Key = "flashMessage";


    private readonly ProtectedSessionStorage _storage;
    public FlashService(ProtectedSessionStorage storage) => _storage = storage;

    public async Task SetAsync(FlashMessage msg) => await _storage.SetAsync(Key, msg);

    public async Task<FlashMessage?> ConsumeAsync()
    {
        var r = await _storage.GetAsync<FlashMessage>(Key);
        if (!r.Success) return null;
        await _storage.DeleteAsync(Key);
        return r.Value;
    }
}