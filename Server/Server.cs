using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
 
class Server
{
    public static string userName;
    public static byte[] userName_byte;
    public static string passWord;
    public static byte[] passWord_byte;
    public static string server_addr;
    public static int server_port;
    public static byte[] auth;
    public static byte[] msgSent;
    public static byte[] msgRec;
    public static string msgRec_str;
 
    public static void ServerListener()
    {
        IPHostEntry iphostinfo = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress = iphostinfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 2012);
        Socket listener_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            listener_socket.Bind(localEndPoint);
            listener_socket.Listen(10);
            while (true)
            {
                Console.WriteLine("Waiting for Connections...");
                Socket handler = listener_socket.Accept();
                while (true)
                {
                    int UsrnameBytesRec = handler.Receive(userName_byte);
                    userName = Encoding.ASCII.GetString(userName_byte,0,UsrnameBytesRec);

                    int PassBytesRec = handler.Receive(passWord_byte);
                    passWord = Encoding.ASCII.GetString(passWord_byte,0,PassBytesRec);

                    if (userName == "admin" && passWord == "admin")
                    {
                        handler.Send(Encoding.ASCII.GetBytes("OK"));
                        Console.WriteLine("{0} has successfully logged in", userName);
                    }
                    else
                    {
                        handler.Send(Encoding.ASCII.GetBytes("not OK"));
                        Console.WriteLine("User {0} has tried to log in with password {1}",userName,passWord);
                        break;
                    }    
                    while (true)
                    {
                        int MsgRbytes = handler.Receive(msgRec); msgRec_str = Encoding.ASCII.GetString(msgRec,0,MsgRbytes);
                        Console.WriteLine("{0} says: {1}", userName, msgRec_str);
                        msgSent = Encoding.ASCII.GetBytes("You've said: \"" + msgRec_str + "\"");
                        handler.Send(msgSent);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
