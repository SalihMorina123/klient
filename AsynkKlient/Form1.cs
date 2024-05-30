using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsynkKlient
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5000);
                stream = client.GetStream();
                textBoxLog.AppendText("Connected to server...\r\n");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Invoke((Action)(() => textBoxLog.AppendText($"Received: {message}\r\n")));
                        }
                    }
                    catch (Exception ex)
                    {
                        Invoke((Action)(() => textBoxLog.AppendText($"Error: {ex.Message}\r\n")));
                    }
                });
            }
            catch (Exception ex)
            {
                textBoxLog.AppendText($"Error: {ex.Message}\r\n");
            }
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                string message = textBoxMessage.Text;
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
                textBoxLog.AppendText($"Sent: {message}\r\n");
                textBoxMessage.Clear();
            }
            catch (Exception ex)
            {
                textBoxLog.AppendText($"Error: {ex.Message}\r\n");
            }
        }

    }
}
