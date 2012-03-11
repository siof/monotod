using System;
using System.Timers;
using System.Net;
using System.Net.Sockets;
using System.Management;
using System.Text;
using System.IO;
using System.Diagnostics;
using Gtk;

public partial class MainWindow: Gtk.Window
{
    public enum MessageType
    {
        ACK = 1,
        LOGIN_REQUEST = 5,
        LOGOUT_REQUEST = 6,
        ONLINE_RELEASE = 0x53,
        ONLINE_REQUEST = 3,
        PING = 2,
        PONG = 0x52,
        UNIQUE_KEY = 4
    }
    
    private bool wine = true;    // bo kij wie jak zrobić standardowe #define ... #ifdef
    
    private System.Timers.Timer m_loginTimer = null;
    private System.Timers.Timer m_onlineTimer = null;
    private UdpClient client = null;
    public string UniqueKey = "";
    private IPEndPoint m_socket; 
    private int PORT = 0x17a2;
    private string HOST = "77.252.88.4";
    private UTF8Encoding encoding = new UTF8Encoding();
    
    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();
        m_socket = new IPEndPoint(IPAddress.Parse(HOST), PORT);
    }
    
    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {
        if (client != null)
            SendLogoutRequest();
        
        Application.Quit ();
        a.RetVal = true;
    }

    protected void OnBtnPlayClicked (object sender, System.EventArgs e)
    {
        if (client == null)
            client = new UdpClient(PORT);
        m_onlineTimer = new System.Timers.Timer(15000.0);
        m_onlineTimer.Elapsed += new ElapsedEventHandler(this.SendLoginRequest);
        m_onlineTimer.Start();
        m_loginTimer = new System.Timers.Timer(5000.0);
        m_loginTimer.Elapsed += new ElapsedEventHandler(this.SendOnlineRequest);
        m_loginTimer.Start();
    
        UniqueKey = GetUniqueKey("");
        SendUniqueKey();
        
        string arg = "";
        string app = "";

        if (wine == true)
        { 
            app = "wine";
            arg = "Wow.exe";
        }
        else
            app = "Wow.exe";
        
        if (chopengl.Active)
            arg += " -opengl";
        Process.Start(app, arg);
    }
    
    private void SendUniqueKey()
    {
        byte[] bytes = BitConverter.GetBytes((short)MessageType.UNIQUE_KEY);
        byte[] sourceArray = StrToByteArray(UniqueKey);
        byte[] destinationArray = new byte[(bytes.Length + sourceArray.Length) - 1];
        Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
        Array.Copy(sourceArray, 0, destinationArray, 1, sourceArray.Length);
        client.Send(destinationArray, destinationArray.Length, m_socket);
    }
    
    private void SendLoginRequest(object sender, ElapsedEventArgs e)
    {
        byte[] bytes = BitConverter.GetBytes((short)MessageType.LOGIN_REQUEST);
        byte[] sourceArray = StrToByteArray(txtLogin.Text);
        byte[] destinationArray = new byte[(bytes.Length + sourceArray.Length) - 1];
        Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
        Array.Copy(sourceArray, 0, destinationArray, 1, sourceArray.Length);
        client.Send(destinationArray, destinationArray.Length, m_socket);
    }

    private void SendOnlineRequest(object sender, ElapsedEventArgs e)
    {
        byte[] bytes = BitConverter.GetBytes((short)MessageType.ONLINE_REQUEST);
        client.Send(bytes, bytes.Length, m_socket);
    }
    
    public void SendLogoutRequest()
    {
        byte[] bytes = BitConverter.GetBytes((short)MessageType.LOGOUT_REQUEST);
        byte[] sourceArray = StrToByteArray(txtLogin.Text);
        byte[] destinationArray = new byte[(bytes.Length + sourceArray.Length) - 1];
        Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
        Array.Copy(sourceArray, 0, destinationArray, 1, sourceArray.Length);
        client.Send(destinationArray, destinationArray.Length, m_socket);
    }

    private void SendPing()
    {
        byte[] bytes = BitConverter.GetBytes((short)MessageType.PING);
        client.Send(bytes, bytes.Length, m_socket);
    }
    
    private static string GetHardDiskId(string drive)
    {
        // test implementation HDD ID for unix system :P based on mono doc
        Process proc = new Process();

        proc.EnableRaisingEvents = false;

        // nie jest standardem w systemie :p 
        proc.StartInfo.FileName = "hdparm";
        proc.StartInfo.Arguments = "-i" + drive + " | grep -i serial";
        proc.Start();
        
        // ReadToEnd jest SynchronizationContext, potrzebujemy czekac ?  // raczej zbędne, pozatym zależnie od implementacji może spieprzyć sie coś przy czytaniu
        //proc.WaitForExit();

        return proc.StandardOutput.ReadToEnd();

    }

    private string GetProcessorId()
    {
        string str = string.Empty;
        ManagementObjectCollection instances = new ManagementClass("win32_processor").GetInstances();
        foreach (ManagementObject obj2 in instances)
        {
            if (str == "")
            {
                return obj2.Properties["processorID"].Value.ToString();
            }
        }
        return str;
    }

    public byte[] StrToByteArray(string str)
    {
        return encoding.GetBytes(str);
    }

    private string GetUniqueKey(string drive)
    {
        try
        {
            // przerobic to, aby znajodowalo unix type sciezke do HDD /dev/hda /dev/sda or smth
            if (drive == string.Empty)
            {
                foreach (DriveInfo info in DriveInfo.GetDrives())
                {
                    if (info.IsReady)
                    {
                        drive = info.RootDirectory.ToString();
                        break;
                    }
                }
            }
            if (drive.EndsWith(@":\"))
            {
                drive = drive.Substring(0, drive.Length - 2);
            }
            string hardDiskId = GetHardDiskId(drive);
            string processorId = GetProcessorId();
            return (processorId.Substring(13) + processorId.Substring(1, 4) + hardDiskId + processorId.Substring(4, 4));
        }
        catch (Exception)
        {
            return null;
        }
    }
}
