using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace ThucHanh
{
    public partial class Form1 : Form
    {       
        public Form1()
        {
            InitializeComponent();

            
            //webView21.CoreWebView2InitializationCompleted += async (s, e) =>
            //{
            //    if (e.IsSuccess)
            //    {
            //        webView21.NavigationStarting += CoreWebView2_NavigationStarting;
            //        await LoadAndDisplayRssAsync();
            //    }

            //    await LoadAndDisplayRssAsync();
            //};

            InitializeWebView2Async();

            //task.r await LoadAndDisplayRssAsync();
            webView21.EnsureCoreWebView2Async(); //đảm bảo WebView2 được khởi tạo đúng cách
        }

        private async void InitializeWebView2Async()
        {
            await webView21.EnsureCoreWebView2Async(null);

            webView21.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting2;

            // Gọi hàm lấy dữ liệu RSS và hiển thị
            await LoadAndDisplayRssAsync();
        }

        // Xử lý sự kiện khi click vào liên kết
        private void CoreWebView2_NavigationStarting2(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // Kiểm tra nếu URL là liên kết bên ngoài (bắt đầu bằng http hoặc https)
            if (e.Uri.StartsWith("http://") || e.Uri.StartsWith("https://") || e.Uri.EndsWith(".pdf"))
            {
                // Hủy điều hướng trong WebView2
                e.Cancel = true;

                // Mở liên kết trong trình duyệt mặc định
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = e.Uri,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở liên kết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            string url = e.Uri;

            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                e.Cancel = true;

                try
                {
                    string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; //link dẫn đến app Chrome 
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = chromePath,
                        Arguments = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể mở Chrome: " + ex.Message);
                }
            }
            else
            {
                e.Cancel = false;
            }
        }
        private async Task LoadAndDisplayRssAsync()
        {
            string rssUrl = "https://www.fc-soft.jp/dcms-rss/news_k/";
            try
            {
                List<RssItem> rssItems = await BeeLibrary.GetRssFeedAsync(rssUrl); //gọi thư viện
                Cursor.Current = Cursors.WaitCursor; 
                if (rssItems == null || rssItems.Count == 0)
                {
                    webView21.NavigateToString("データ取得に失敗しました。"); //báo không lấy được dữ liệu nếu không có dữ liệu
                    return;
                }

                //tạo HTML để hiển thị
                var sortedList = rssItems.OrderByDescending(a => a.PubDate).Take(10).ToList(); //sắp xếp theo ngày mới nhất
                string text = "<table style='width: 100%'>"; 
                foreach (RssItem serviceInfo in sortedList)
                {
                    string desc = serviceInfo.Description.Replace("\n", "<br>");
                    text += $"<tr valign='top'><td style='width:350px;'>{serviceInfo.PubDate}</td><td style='width: 20px;'>&nbsp;</td><td><h3>{serviceInfo.Title}</h3>{desc}<hr/></td></tr>"; 
                }
                text += "</table></body></html>";
                webView21.NavigateToString(text);
            }
            catch (Exception ex)
            {
                webView21.NavigateToString($"Lỗi khi tải RSS: {ex.Message}");
            }    
        }
    }
}

