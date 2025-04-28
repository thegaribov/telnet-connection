using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TelnetConnectionUtility;

public class TelnetConnection : IDisposable
{
    private TcpClient tcpClient;

    //public int TimeOutMs = 100;

    private string _host;

    private int _port;

    public bool IsConnected
    {
        get
        {
            if (tcpClient != null)
            {
                return tcpClient.Connected;
            }
            return false;
        }
    }

    public TelnetConnection(string Hostname, int Port)
    {
        _host = Hostname;
        _port = Port;
        tcpClient = new TcpClient();
        Connect();
    }

    private bool InternalConnect(int timeoutSeconds)
    {
        IAsyncResult asyncResult = tcpClient.BeginConnect(_host, _port, null, null);
        asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeoutSeconds));
        if (!tcpClient.Connected)
        {
            return false;
        }

        tcpClient.EndConnect(asyncResult);
        return true;
    }

    public bool Connect()
    {
        bool result = true;
        try
        {
            if (tcpClient == null)
                tcpClient = new TcpClient();

            if (!tcpClient.Connected)
                return InternalConnect(timeoutSeconds: 3);

            return result;
        }
        catch (Exception)
        {
            tcpClient = null;
            return false;
        }
    }

    public void Disconnect()
    {
        if (tcpClient is null)
            return;

        Dispose();
    }

    //public string Login(string Username, string Password, int LoginTimeOutMs)
    //{
    //    int timeOutMs = TimeOutMs;
    //    TimeOutMs = LoginTimeOutMs;
    //    string text = Read();
    //    if (!text.TrimEnd().EndsWith(":"))
    //    {
    //        throw new Exception("Failed to connect : no login prompt");
    //    }
    //    WriteLine(Username);
    //    string text2 = text + Read();
    //    if (!text2.TrimEnd().EndsWith(":"))
    //    {
    //        throw new Exception("Failed to connect : no password prompt");
    //    }
    //    WriteLine(Password);
    //    string result = text2 + Read();
    //    TimeOutMs = timeOutMs;
    //    return result;
    //}

    //public void WriteLine(string cmd)
    //{
    //    Write(cmd + "\r\n");
    //}

    //public void Write(string cmd)
    //{
    //    if (IsConnected)
    //    {
    //        byte[] bytes = Encoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
    //        tcpClient.GetStream().Write(bytes, 0, bytes.Length);
    //    }
    //}

    //public string Read()
    //{
    //    if (!IsConnected)
    //    {
    //        return null;
    //    }

    //    StringBuilder stringBuilder = new StringBuilder();

    //    do
    //    {
    //        ParseTelnet(stringBuilder);
    //        Thread.Sleep(TimeOutMs);
    //    }
    //    while (tcpClient.Available > 0);

    //    return stringBuilder.ToString();
    //}

    //private void ParseTelnet(StringBuilder sb)
    //{
    //    while (tcpClient.Available > 0)
    //    {
    //        int num = tcpClient.GetStream().ReadByte();
    //        switch (num)
    //        {
    //            case 255:
    //                {
    //                    int num2 = tcpClient.GetStream().ReadByte();
    //                    switch (num2)
    //                    {
    //                        case 255:
    //                            sb.Append(num2);
    //                            break;
    //                        case 251:
    //                        case 252:
    //                        case 253:
    //                        case 254:
    //                            {
    //                                int num3 = tcpClient.GetStream().ReadByte();
    //                                if (num3 != -1)
    //                                {
    //                                    tcpClient.GetStream().WriteByte(byte.MaxValue);
    //                                    if (num3 == 3)
    //                                    {
    //                                        tcpClient.GetStream().WriteByte((byte)((num2 == 253) ? 251 : 253));
    //                                    }
    //                                    else
    //                                    {
    //                                        tcpClient.GetStream().WriteByte((byte)((num2 == 253) ? 252 : 254));
    //                                    }
    //                                    tcpClient.GetStream().WriteByte((byte)num3);
    //                                }
    //                                break;
    //                            }
    //                    }
    //                    break;
    //                }
    //            default:
    //                sb.Append((char)num);
    //                break;
    //            case -1:
    //                break;
    //        }
    //    }
    //}

    public void Dispose()
    {
        tcpClient.Dispose();
        tcpClient = null;
    }
}
