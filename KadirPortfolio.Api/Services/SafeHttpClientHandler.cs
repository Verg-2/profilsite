using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace KadirPortfolio.Api.Services
{
    public static class SafeHttpClientHandler
    {
        public static SocketsHttpHandler Create()
        {
            return new SocketsHttpHandler
            {
                ConnectCallback = async (context, cancellationToken) =>
                {
                    var hostEntry = await Dns.GetHostEntryAsync(context.DnsEndPoint.Host, cancellationToken);
                    var ipAddress = hostEntry.AddressList.FirstOrDefault();

                    if (ipAddress == null || IsPrivateIP(ipAddress) || IPAddress.IsLoopback(ipAddress))
                    {
                        throw new WebException($"Güvenlik İhlali: Özel/Yerel IP adreslerine bağlantı engellendi ({ipAddress})");
                    }

                    var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    await socket.ConnectAsync(new IPEndPoint(ipAddress, context.DnsEndPoint.Port), cancellationToken);
                    return new NetworkStream(socket, ownsSocket: true);
                }
            };
        }

        private static bool IsPrivateIP(IPAddress ip)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] bytes = ip.GetAddressBytes();
                if (bytes[0] == 10) return true; // 10.0.0.0/8
                if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true; // 172.16.0.0/12
                if (bytes[0] == 192 && bytes[1] == 168) return true; // 192.168.0.0/16
                if (bytes[0] == 169 && bytes[1] == 254) return true; // Link-Local
            }
            else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                if (ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal) return true;
                byte[] bytes = ip.GetAddressBytes();
                if ((bytes[0] & 0xFE) == 0xFC) return true; // Unique Local Address
            }
            return false;
        }
    }
}
