using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using System.IO;

namespace token_briefing_client_win_updater
{
    public partial class updater : Form

    {
        string tmpSetupPath;
        public updater()
        {
            InitializeComponent();
        }
        [STAThread]
        private void updater_Load(object sender, EventArgs e)
        {
            try
            {
                label1.Text = "기존 프로그램 종료 중";

                Process[] processesList = Process.GetProcessesByName("token-briefing-client");

                if (processesList.Length > 0)
                {
                    int time = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
                    int endTime = time + 3;
                    for (time = Convert.ToInt32(DateTime.Now.ToString("HHmmss")); time < endTime; time = Convert.ToInt32(DateTime.Now.ToString("HHmmss")))
                    {
                        Process[] processList = Process.GetProcessesByName("token-briefing-client");
                        try
                        {
                            if (processList.Length > 0)
                            {
                                processList[0].Kill();
                            }

                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    MessageBox.Show("잘못된 접근입니다.", "실행 오류");
                    Application.Exit();
                }

                using (WebClient fileDownloader = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    label1.Text = "파일 다운로드를 시작";
                   
                    string filePath = @"C:\Users\" + ((System.Security.Principal.WindowsIdentity.GetCurrent().Name).Split('\\')[1]) + @"\AppData\Local\token-briefing-client\";
                    string dataLink = "";

                    try
                    {
                        var lines = File.ReadLines(filePath + "serverip.txt", Encoding.UTF8);
                        foreach (var line in lines)
                        {
                            dataLink = "http://" + line + "/api/v1/client/download/win/setup";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("업데이트 파일을 받는 중 오류가 발생했습니다.\n" + ex + "\nserverIp:" + dataLink);
                        Application.Exit();
                    }

                    tmpSetupPath = Path.Combine(Application.StartupPath, "setup.exe");
                    fileDownloader.DownloadFileCompleted += fileDownloader_DownloadFileCompleted;
                    fileDownloader.DownloadFileAsync(new Uri(dataLink), tmpSetupPath, tmpSetupPath);

                    label1.Text = "완료";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("업데이트 파일을 받는 중 오류가 발생했습니다.\n" + ex);
                Application.Exit();
            }

        }

        void fileDownloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Process.Start(tmpSetupPath);
            Application.Exit();
        }
    }
}
