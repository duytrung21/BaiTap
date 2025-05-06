using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            webView.EnsureCoreWebView2Async();
        }

        private async void WebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            await LoadAndDisplayRssAsync();
        }
        private async Task LoadAndDisplayRssAsync()
        {
            string rssUrl = "https://www.fc-soft.jp/dcms-rss/news_k/";
            try
            {
                List<RssItem> rssItems = await BeeLibrary.GetRssFeedAsync(rssUrl);
                Cursor.Current = Cursors.WaitCursor;
                if (rssItems == null || rssItems.Count == 0)
                {
                    webView.NavigateToString("データ取得に失敗しました。");
                    return;
                }

                var sortedList = rssItems.OrderByDescending(a => a.PubDate).Take(10).ToList();

                string html = "<html><head><meta charset='UTF-8'></head><body style='font-family: Meiryo UI; font-size: 16px;'><table style='width: 100%'>";
                foreach (var item in sortedList)
                {
                    string desc = item.Description.Replace("\n", "<br>");
                    html += $"<tr valign='top'><td style='width:350px;'>{item.PubDate}</td><td style='width: 50px;'>&nbsp;</td><td><h3>{item.Title}</h3>{desc}<hr/></td></tr>";
                }
                html += "</table></body></html>";

                webView.NavigateToString(html);
            }
            catch (Exception ex)
            {
                webView.NavigateToString($"<p>Lỗi khi tải RSS: {ex.Message}</p>");
            }    
        }
    }
}

