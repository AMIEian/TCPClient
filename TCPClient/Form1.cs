using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPClient
{
    public partial class Form1 : Form
    {
        Thread dataSender;
        bool startSend = false;
        bool start = true;
        public Form1()
        {
            InitializeComponent();
            dataSender = new Thread(SendData);
            dataSender.Start();
        }

        SimpleTcpClient client;
        string[] epcs = { "30361FAC682922C000000005", "30361FAC682922C000000009", "30361FAC682922C00000000D","30361FAC682922C000000016","30361FAC682922C00000004E","30361FAC6828B5C000000007", "30361FAC6828B5C000000008", "30361FAC6828B5C00000000B", "30361FAC6828B5C00000000D", "30361FAC6828B5C00000000E", "30361FAC6828B5C000000014", "30361FAC6828B5C000000015", "30361FAC6828B5C000000024", "30361FAC6828B5C000000030", "30361FAC6828B5C00000004C", "30361FAC6828B5C00000005E", "30361FAC6828B6000000001F", "30361FAC6828B60000000025", "30361FAC6828B60000000027", "30361FAC6828B60000000033", "30361FAC6828B6000000003B", "30361FAC6828B6000000003C", "30361FAC6828B6000000003D" }; //, "30361FAC6828B6400000000D", "30361FAC6828B6400000001A", "30361FAC6828B6400000001E", "30361FAC6828B64000000023" };//, "30361FAC6824CA400000004B", "30361FAC6824C30000000057", "30361FAC6824C2800000001C", "30361FAC681ADF4000000045", "30361FAC681AE9800000004C", "30361FAC681C7D800000007A", "30361FAC681C7C4000000078", "30361FAC6824C9C00000001D", "30361FAC681C7BC00000007C", "30361FAC681ADF4000000046", "30361FAC681ADE8000000057", "30361FAC681AE84000000064", "30361FAC681B18000000005B", "30361FAC681E5CC000000021", "30361FAC681C7C0000000063", "30361FAC6824CA4000000027", "30361FAC681AE5C000000014", "30361FAC681AE6800000006C", "30361FAC681C7C000000001C" };
        string ip;
        private void Form1_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;

            string localIp = "0.0.0.0";
            //Star server on local ip on port 500
            try
            {
                localIp = GetLocalIPAddress();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            txtHost.Text = localIp;
            ip = localIp;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            //Update message to txtStatus
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Text += e.MessageString;
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            //Connect to server
            client.Connect(txtHost.Text, Convert.ToInt32(txtPort.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (startSend == false)
            {
                startSend = true;
            }
            else
            {
                startSend = false;
            }
            System.Diagnostics.Debug.WriteLine(startSend.ToString());
        }

        void SendData()
        {
            while(start)
            {
                while (startSend)
                {
                    Random rnd = new Random();
                    string data = epcs[rnd.Next(0, epcs.Length)] + ",A0001," + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    txtMessage.Invoke((MethodInvoker)delegate ()
                    {
                        txtMessage.Text = data;
                    });
                    //client.Connect(ip, 500);
                    client.WriteLineAndGetReply(data, TimeSpan.FromSeconds(0));
                    //client.Disconnect();
                    Thread.Sleep(10);
                }
                Thread.Sleep(1000);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            start = false;
            Application.Exit();
        }
    }
}
