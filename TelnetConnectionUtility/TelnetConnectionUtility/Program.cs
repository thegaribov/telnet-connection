namespace TelnetConnectionUtility
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TelnetConnection telnetConnection = new TelnetConnection("learn.microsoft.com", 445);
            Console.WriteLine(telnetConnection.IsConnected);

            telnetConnection.Disconnect();
        }
    }
}
