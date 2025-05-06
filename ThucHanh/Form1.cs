using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace ThucHanh
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(webView);
            webView.Source = new Uri("https://www.fc-soft.jp/campaign/y_naming2025/");
        }
		async void LoadListViewData()
		{
			string rssUrl = "https://www.fc-soft.jp/dcms-rss/news_k/";
		}
	}
}
