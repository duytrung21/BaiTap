using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace ThucHanh
{
    public partial class Form1 : Form
    {
        private WebView2 webView;
        public Form1()
        {
            InitializeComponent();
            webView = new WebView2
            {
                Dock = DockStyle.Fill 
            };
            this.Controls.Add(webView);
            webView.CoreWebView2InitializationCompleted += async (s, e) =>
            {
                if (e.IsSuccess)
                {
                    webView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                    await LoadAndDisplayRssAsync();
                }
            };
            webView.EnsureCoreWebView2Async(); //đảm bảo WebView2 được khởi tạo đúng cách
        }

        private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            e.Cancel = true;
            string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            string url = e.Uri;
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = chromePath,
                    Arguments = url,
                    UseShellExecute = false
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở Chrome: " + ex.Message);
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
                    webView.NavigateToString("データ取得に失敗しました。"); //báo không lấy được dữ liệu nếu không có dữ liệu
                    return;
                }

                //tạo HTML để hiển thị
                var sortedList = rssItems.OrderByDescending(a => a.PubDate).Take(10).ToList(); //sắp xếp theo ngày mới nhất
                string text = "<table style='width: 100%'>";
                foreach (RssItem serviceInfo in sortedList)
                {
                    string desc = serviceInfo.Description.Replace("\n", "<br>");
                    text += $"<tr valign='top'><td style='width:350px;'>{serviceInfo.PubDate}</td><td style='width: 50px;'>&nbsp;</td><td><h3>{serviceInfo.Title}</h3>{desc}<hr/></td></tr>"; 
                }
                text += "</table></body></html>";

                webView.NavigateToString(text);
            }
            catch (Exception ex)
            {
                webView.NavigateToString($"Lỗi khi tải RSS: {ex.Message}");
            }    
        }
    }
}

