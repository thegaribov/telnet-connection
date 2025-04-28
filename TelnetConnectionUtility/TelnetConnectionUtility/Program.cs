
namespace TelnetConnectionUtility
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TelnetConnection telnetConnection = new TelnetConnection("learn.microsoft.com", 445);
            await telnetConnection.ConnectAsync();
            Console.WriteLine(telnetConnection.IsConnected);

        }
    }
}
