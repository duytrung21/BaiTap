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
            InitializeWebView2Async();
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
                //text += "<a href=\"https://s3-ap-northeast-1.amazonaws.com/fcdownload/PDF/R0704_menu.pdf\" target=\"_blank\" rel=\"noopener\">詳しくはこちら</a>"; //test link
                webView21.NavigateToString(text);
            }
            catch (Exception ex)
            {
                webView21.NavigateToString($"Lỗi khi tải RSS: {ex.Message}");
            }    
        }
    }
}

