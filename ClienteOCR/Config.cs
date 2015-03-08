using System.Net;

namespace ClienteSintegra
{
    public static class Config
    {
        private static readonly WebProxy Wp = WebProxy.GetDefaultProxy();
        public static readonly Gateway.Gateway Ws = new Gateway.Gateway();

        static Config()
        {
            Ws.Proxy = Wp;
            Wp.UseDefaultCredentials = true;    
        }
    }
}
