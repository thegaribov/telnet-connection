using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TelnetConnectionUtility;

public class TelnetConnection
{
    private TcpClient tcpSocket;

    public int TimeOutMs = 100;

    private string _host;

    private int _port;

    public bool IsConnected
    {
        get
        {
            if (tcpSocket != null)
            {
                return tcpSocket.Connected;
            }
            return false;
        }
    }

    public TelnetConnection(string Hostname, int Port)
    {
        _host = Hostname;
        _port = Port;
        tcpSocket = new TcpClient();
        Connect();
    }

    private bool ConnectTimeOut(int s)
    {
        IAsyncResult asyncResult = tcpSocket.BeginConnect(_host, _port, null, null);
        asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(s));
        if (!tcpSocket.Connected)
        {
            return false;
        }

        tcpSocket.EndConnect(asyncResult);
        return true;
    }

    public bool Connect()
    {
        bool result = true;
        try
        {
            if (tcpSocket == null)
            {
                tcpSocket = new TcpClient();
            }
            if (!tcpSocket.Connected)
            {
                return ConnectTimeOut(3);
            }
            return result;
        }
        catch (Exception)
        {
            tcpSocket = null;
            return false;
        }
    }

    public void DisConnect()
    {
        tcpSocket.GetStream().Close();
        tcpSocket = null;
    }

    public string Login(string Username, string Password, int LoginTimeOutMs)
    {
        int timeOutMs = TimeOutMs;
        TimeOutMs = LoginTimeOutMs;
        string text = Read();
        if (!text.TrimEnd().EndsWith(":"))
        {
            throw new Exception("Failed to connect : no login prompt");
        }
        WriteLine(Username);
        string text2 = text + Read();
        if (!text2.TrimEnd().EndsWith(":"))
        {
            throw new Exception("Failed to connect : no password prompt");
        }
        WriteLine(Password);
        string result = text2 + Read();
        TimeOutMs = timeOutMs;
        return result;
    }

    public void WriteLine(string cmd)
    {
        Write(cmd + "\r\n");
    }

    public void Write(string cmd)
    {
        if (IsConnected)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(bytes, 0, bytes.Length);
        }
    }

    public string Read()
    {
        if (!IsConnected)
        {
            return null;
        }

        StringBuilder stringBuilder = new StringBuilder();

        do
        {
            ParseTelnet(stringBuilder);
            Thread.Sleep(TimeOutMs);
        }
        while (tcpSocket.Available > 0);

        return stringBuilder.ToString();
    }

    private void ParseTelnet(StringBuilder sb)
    {
        while (tcpSocket.Available > 0)
        {
            int num = tcpSocket.GetStream().ReadByte();
            switch (num)
            {
                case 255:
                    {
                        int num2 = tcpSocket.GetStream().ReadByte();
                        switch (num2)
                        {
                            case 255:
                                sb.Append(num2);
                                break;
                            case 251:
                            case 252:
                            case 253:
                            case 254:
                                {
                                    int num3 = tcpSocket.GetStream().ReadByte();
                                    if (num3 != -1)
                                    {
                                        tcpSocket.GetStream().WriteByte(byte.MaxValue);
                                        if (num3 == 3)
                                        {
                                            tcpSocket.GetStream().WriteByte((byte)((num2 == 253) ? 251 : 253));
                                        }
                                        else
                                        {
                                            tcpSocket.GetStream().WriteByte((byte)((num2 == 253) ? 252 : 254));
                                        }
                                        tcpSocket.GetStream().WriteByte((byte)num3);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    sb.Append((char)num);
                    break;
                case -1:
                    break;
            }
        }
    }
}
