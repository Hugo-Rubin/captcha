using System.Net;

namespace ClienteSintegra
{
    public static class Config
    {
        private static readonly WebProxy Wp = WebProxy.GetDefaultProxy();
        public static readonly RemoteGateway.Gateway Ws = new RemoteGateway.Gateway();

        static Config()
        {
            Ws.Proxy = Wp;
            Wp.UseDefaultCredentials = true;    
        }
    }
}
