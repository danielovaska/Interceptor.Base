namespace Mogul.Interceptor.Cache
{
    public interface ICachedResponse
    {
        bool GotItemFromCache { get; set; }
    }
}