using System.Text;
using System.Net;
using System.Net.Sockets;
 
class Client
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
 
    public static void Auth()
    {
        Console.Write("Username: ");
        userName = Console.ReadLine();
        userName_byte = Encoding.ASCII.GetBytes(userName);
        Console.Write("\nPassword: ");
        passWord = Console.ReadLine();
        passWord_byte = Encoding.ASCII.GetBytes(passWord);
        Console.Write("\nServer (type 0 to connect to localhost): ");
        server_addr = Console.ReadLine();
        if (server_addr == "0")
        {
            server_addr = "127.0.0.1";
        }
        Console.Write("\nPort (type 0 to use default): ");
        server_port = Convert.ToInt32(Console.ReadLine());
        if (server_port == 0)
        {
            server_port = 2012;
        }
    }
        
    public static void StartClient()
    {
        Auth();
        IPHostEntry iphostinfo = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress = iphostinfo.AddressList[0];
        IPEndPoint EndPoint = new IPEndPoint(ipAddress, 2012);
        Socket sender_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            sender_socket.Connect(EndPoint);
            int bytesSentUsrName = sender_socket.Send(userName_byte);
            int bytesSentPassWrd = sender_socket.Send(passWord_byte);
            int bytesRecAuth = sender_socket.Receive(auth);
            if (Encoding.ASCII.GetString(auth,0,bytesRecAuth) != "OK")
            {
                Console.WriteLine("Auathorisation failed, closing programm");
                Disconnecting(sender_socket);
                return;
            }
            else
            {
                Console.WriteLine("{1} has successfully connected to {0}", server_addr, userName);
                while (true)
                {
                    string cons;
                    Console.Write("   You: "); cons = Console.ReadLine();
                    if (cons != "EXIT")
                    {
                        msgSent = Encoding.ASCII.GetBytes(cons);
                        int bytesSentMsg = sender_socket.Send(msgSent);
                        int bytesRecMsg = sender_socket.Receive(msgRec);
                        Console.Write("Server: {0}", Encoding.ASCII.GetString(msgRec,0,bytesRecMsg));
                    }
                    else
                    {
                        Disconnecting(sender_socket);
                        return;
                    }
                }
            }
        }
        catch (ArgumentNullException ane)
        {
            Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
        }
        catch (SocketException se)
        {
            Console.WriteLine("SocketException : {0}", se.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected exception : {0}", e.ToString());
        }
    }
    public static void Disconnecting(Socket socket)
    {
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}
