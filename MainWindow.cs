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
    public enum MsgType
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
    
    private System.Timers.Timer m_loginTimer = null;
    private System.Timers.Timer m_onlineTimer = null;
    private UdpClient client = null;
    public string UniqueKey = "";
    private IPEndPoint m_socket; 
    private int PORT = 0x17a2;
    private string HOST = "77.252.88.4";
    private UTF8Encoding encoding = new UTF8Encoding();
    private Process wowProc;
    
    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();
        m_socket = new IPEndPoint(IPAddress.Parse(HOST), PORT);
    }
    
    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {
        if (client != null)
        {
            SendLogoutRequest();
            client.Close();
        }

        if (m_loginTimer != null)
            m_loginTimer.Dispose();

        if (m_onlineTimer != null)
            m_onlineTimer.Dispose();

        if (wowProc != null)
        {
            if (wowProc.HasExited == false)
                wowProc.Kill();
            wowProc.Dispose();
        }

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
    
        UniqueKey = GetUniqueKey();

        SendUniqueKey();
        
        string arg = "";

        arg = "Wow.exe";

        if (chopengl.Active)
            arg += " -opengl";

        wowProc = Process.Start("wine", arg);
    }
    
    private void SendUniqueKey()
    {
        byte[] bytes = BitConverter.GetBytes((short)MsgType.UNIQUE_KEY);
        byte[] sourceArray = StrToByteArray(UniqueKey);
        byte[] destinationArray = new byte[(bytes.Length + sourceArray.Length) - 1];
        Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
        Array.Copy(sourceArray, 0, destinationArray, 1, sourceArray.Length);
        client.Send(destinationArray, destinationArray.Length, m_socket);
    }
    
    private void SendLoginRequest(object sender, ElapsedEventArgs e)
    {
        byte[] bytes = BitConverter.GetBytes((short)MsgType.LOGIN_REQUEST);
        byte[] sourceArray = StrToByteArray(txtLogin.Text);
        byte[] destinationArray = new byte[(bytes.Length + sourceArray.Length) - 1];
        Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
        Array.Copy(sourceArray, 0, destinationArray, 1, sourceArray.Length);
        client.Send(destinationArray, destinationArray.Length, m_socket);
    }

    private void SendOnlineRequest(object sender, ElapsedEventArgs e)
    {
        byte[] bytes = BitConverter.GetBytes((short)MsgType.ONLINE_REQUEST);
        client.Send(bytes, bytes.Length, m_socket);
    }
    
    public void SendLogoutRequest()
    {
        byte[] bytes = BitConverter.GetBytes((short)MsgType.LOGOUT_REQUEST);
        byte[] sourceArray = StrToByteArray(txtLogin.Text);
        byte[] destinationArray = new byte[(bytes.Length + sourceArray.Length) - 1];
        Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
        Array.Copy(sourceArray, 0, destinationArray, 1, sourceArray.Length);
        client.Send(destinationArray, destinationArray.Length, m_socket);
    }

    private void SendPing()
    {
        byte[] bytes = BitConverter.GetBytes((short)MsgType.PING);
        client.Send(bytes, bytes.Length, m_socket);
    }
    
    private string GetHardDiskId()
    {
        // test implementation HDD ID for unix system :P
        Process proc = new Process();

        proc.EnableRaisingEvents = false;
        //proc.StartInfo.FileName = "ls";
        //proc.StartInfo.Arguments = "-l /dev/disk/by-id/ | grep scsi- | grep -v part | awk '{print $(NF-2)}' | sed 's|../../||g' | sed 's/scsi-...._.*_//g'";
        proc.StartInfo.FileName = "ls";
        proc.StartInfo.Arguments = "-1 /dev/disk/by-id/";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();

        string line = "";
        string hddid = "";

        // c# wersja: | grep scsi- | grep -v part | awk '{print $(NF-2)}' | sed 's|../../||g' | sed 's/scsi-...._.*_//g'
        while ((line = proc.StandardOutput.ReadLine()) != null)
        {
            if (line.Length == 0)
                continue;

            if (!line.Contains("scsi-"))
                continue;

            if (line.Contains("part"))
                continue;

            int ind1 = line.LastIndexOf("_");

            if (ind1 >= 0)
                hddid = line.Substring(ind1+1);
        }

        proc.Dispose();

        if (hddid == "")
            return null;

        return hddid;
    }

    private string GetProcessorId()
    {
        // temp implementation for CPU ID for unix system :P
        Process proc = new Process();

        proc.EnableRaisingEvents = false;
        proc.StartInfo.FileName = "uname";
        proc.StartInfo.Arguments = "-i";
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.UseShellExecute = false;
        proc.Start();

        string cpuid = proc.StandardOutput.ReadLine();
        proc.WaitForExit();
        proc.Dispose();

        if (cpuid == null || cpuid == "")
            return null;

        return cpuid;
    }

    public byte[] StrToByteArray(string str)
    {
        return encoding.GetBytes(str);
    }

    private string GetUniqueKey()
    {
        /*
        // zbędne na linuxie
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
        */

        string hardDiskId = GetHardDiskId();
        string processorId = GetProcessorId();

        string tmpUniq = processorId.Substring(processorId.Length - 4);
        tmpUniq += processorId.Substring(1, 4);
        tmpUniq += hardDiskId;

        if (processorId.Length >= 8)
            tmpUniq += processorId.Substring(4, 4);
        else if (processorId.Length > 4)
            tmpUniq += processorId.Substring(4);

        return tmpUniq;
    }
}
