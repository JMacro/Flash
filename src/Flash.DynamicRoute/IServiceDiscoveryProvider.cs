namespace Flash.DynamicRoute
{
    public interface IServiceDiscoveryProvider
    {
        void Register();

        void Deregister();

        string ServiceId { get; }
    }
}
