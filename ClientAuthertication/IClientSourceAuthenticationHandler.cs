namespace ClientAuthertication
{
    public interface IClientSourceAuthenticationHandler
    {
        bool Validate(string clientSource);

    }
}
