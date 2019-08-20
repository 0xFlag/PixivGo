using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixivGo
{
    public partial class Form1 : Form
    {
        private int zpid;
        private string apiurl;
        private string poriginalurl;
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.textBox1.Text = null;
        }

        private void textBox1_MouseLeave(object sender, EventArgs e)
        {
            this.textBox1.Text = "请在此输入Pixiv作品id（例：id=63678321），然后按回车_(:з」∠)_";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                if (this.textBox1.Text == "")
                {
                    MessageBox.Show("请输入Pixiv作品id");
                }
                else if (this.textBox1.Text.IndexOf("id=") < 0)
                {
                    MessageBox.Show("清在作品id前加上 id=");
                }
                else
                {
                    e.Handled = true;
                    this.tabControl1.SelectedIndex = 1;
                    pgo();
                }
            }
        }

        private void pgo()
        {
            this.listBox1.Items.Clear();
            string url = "https://api.imjad.cn/pixiv/v2/?type=illust&" + this.textBox1.Text;
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
            string load = client.DownloadString(url);
            string getJson = load;

            Root rt = JsonConvert.DeserializeObject<Root>(getJson);
            //作品信息
            zpid = rt.illust.id;                                    //作品id
            string zpbt = rt.illust.title;                          //作品标题
            int hsid = rt.illust.user.id;                           //画师id
            string hsnc = rt.illust.user.name;                      //画师昵称
            string hszh = rt.illust.user.account;                   //画师帐号
            string zpsm = rt.illust.caption;                        //作品说明
            string zpck = rt.illust.width + "x" + rt.illust.height; //作品长宽
            string zpsj = rt.illust.create_date;                    //作品上传时间

            //pixv图片地址
            string purl = "https://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + zpid;
            Uri u = new Uri("https://i.pximg.net/c/540x540_10_webp/img-master/img/2017/07/04/01/01/45/63694804_p0_square1200.jpg");
            string temp = u.LocalPath.Remove(0, 19);
            string purl64 = "https://i.pximg.net/c/64x64/" + temp;
            string purl128 = "https://i.pximg.net/c/128x128/" + temp;
            poriginalurl = rt.illust.meta_single_page.original_image_url;
            string pgeturl = "https://img.pixivic.com:23334/get/" + poriginalurl;
            apiurl = "https://api.imjad.cn/interface/img/PixivProxy.php?url=" + rt.illust.image_urls.medium;
            string qturl = "\r\n" + rt.illust.image_urls.square_medium + "\r\n" + rt.illust.image_urls.large + "\r\n" + rt.illust.user.profile_image_urls.medium;
            
            //pixv图片信息
            this.textBox2.Text = "作品  id：" + zpid + "\r\n作品标题：" + zpbt + "\r\n画师  id：" + hsid + "\r\n画师昵称：" + hsnc + "\r\n画师帐号：" + hszh + "\r\n作品说明：" + zpsm + "\r\n作品长宽：" + zpck + "\r\n作品上传时间：" + zpsj + "\r\n\r\nPixivUrl：" + purl + "\r\nPixiv64x64：" + purl64 + "\r\nPixiv128x128：" + purl128 + "\r\n原始PixivUrl：" + poriginalurl + "\r\nPixivGetUrl：" + pgeturl + "\r\n\r\nimjadApiUrl：" + apiurl + "\r\n\r\n其他大小Url：" + qturl;

            //图片预览
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(poriginalurl);
            httpWebRequest.Referer = purl;
            httpWebRequest.Method = "GET";
            HttpWebResponse webResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = webResponse.GetResponseStream();
            this.pictureBox1.Image = Image.FromStream(stream);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
            {
                MessageBox.Show("预览失败");
            }
            else
            {
                string url = apiurl;
                System.Diagnostics.Process.Start(url);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
            {
                MessageBox.Show("保存失败");
            }
            else
            {
                download();
            }
        }

        private void download()
        {
            if (false == System.IO.Directory.Exists(zpid.ToString())) //判断文件夹是否存在
            {
                Directory.CreateDirectory(zpid.ToString()); //不存在自动创建

                string Link = poriginalurl;
                string[] Links = Link.Split(new String[] { "https" }, StringSplitOptions.RemoveEmptyEntries);
                string path = zpid.ToString(); //用于存放path
                string pathing = path;

                WebClient myWebClient = new WebClient();//创建WebClient并开始下载
                myWebClient.Proxy = null;
                ServicePointManager.DefaultConnectionLimit = 500;
                
                //循环中需要用到的变量
                string link = "";
                string pathadd;
                string referer;
                foreach (string l in Links)
                {
                    Application.DoEvents();
                    try
                    {
                        link = "https" + l;
                        link = link.Trim();
                        pathadd = l.Replace("://i.pximg.net/img-original/img/", "").Replace('/', '.').Trim();
                        //设置referer
                        referer = l.Remove(0, 52);
                        referer = referer.Remove(referer.IndexOf("_"));
                        myWebClient.Headers.Add("Referer", "https://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + referer);
                        path = path + "/" + pathadd;
                        //开始下载
                        myWebClient.DownloadFile(link, path);
                        Application.DoEvents();
                    }
                    catch
                    {
                        //数据还原
                        path = pathing;
                        myWebClient.Headers.Clear();
                        continue;
                    }
                    path = pathing;
                    myWebClient.Headers.Clear();
                    this.textBox3.AppendText(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "----" + zpid + "----下载完成！(*^▽^*)\r\n"); //下载日志记录
                }
            }
            else
            {
                MessageBox.Show("请先删除历史文件");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文档|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.FileName != "")
                {
                    StreamReader streamReader = new StreamReader(ofd.FileName, Encoding.Default);
                    this.textBox4.Text = streamReader.ReadToEnd(); ;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowDialog();
            this.textBox5.Text = "" + folderDlg.SelectedPath;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //获取下载链接和下载路径并传参
            string link = this.textBox4.Text;
            string path = this.textBox5.Text;
            if (link.Equals("") || path.Equals(""))
            {
                MessageBox.Show("输入值无效");
            }
            this.label10.Text = "PixivGo 正在下载图片";
            downloading(link, path);
            this.label10.Text = "PixivGo 正在吃瓜等待命令";
        }

        private void downloading(string link, string path)
        {
            string[] links = link.Split(new String[] { "https" }, StringSplitOptions.RemoveEmptyEntries);
            //用于存放Path
            string pathing = path;
            //总任务数量
            int total = links.Length;
            this.label4.Text = total.ToString();
            //失败和成功的任务数量
            int falsein = 0;
            int truein = 0;
            //创建WebClient并开始下载
            WebClient myWebClient = new WebClient();
            myWebClient.Proxy = null;
            ServicePointManager.DefaultConnectionLimit = 500;
            //循环中需要用到的变量
            string lin = "";
            string pathadd;
            string referer;
            progressBar1.Value = 0;
            foreach (string l in links)
            {
                Application.DoEvents();
                try
                {
                    lin = "https" + l;
                    lin = link.Trim();
                    pathadd = l.Replace("://i.pximg.net/img-original/img/", "").Replace('/', '.').Trim();
                    //设置referer
                    referer = l.Remove(0, 52);
                    referer = referer.Remove(referer.IndexOf("_"));
                    myWebClient.Headers.Add("Referer", "https://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + referer);
                    path = path + "/" + pathadd;
                    //开始下载
                    myWebClient.DownloadFile(link, path);
                    Application.DoEvents();
                }
                catch
                {
                    falsein = falsein + 1;
                    this.label8.Text = falsein.ToString();
                    //数据还原
                    path = pathing;
                    myWebClient.Headers.Clear();
                    continue;
                }
                path = pathing;
                myWebClient.Headers.Clear();
                truein = truein + 1;
                this.label6.Text = truein.ToString();
                this.progressBar1.Value = this.progressBar1.Value + (1 / total) * 100;
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox6_MouseDown(object sender, MouseEventArgs e)
        {
            this.textBox6.Text = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.textBox6.Text == "请在此输入搜索关键词_(:з」∠)_" || this.textBox6.Text == "" || this.textBox7.Text == null)
            {
                MessageBox.Show("搜索失败");
            }
            else
            {
                searchword();
            }
        }

        private void searchword()
        {
            this.listBox1.Items.Clear();
            this.pictureBox1.Image = null;
            this.textBox2.Clear();
            this.textBox3.Clear();
            string word = this.textBox6.Text;
            string pageid = this.textBox7.Text;
            string keywordurl = "https://api.imjad.cn/pixiv/v2/?type=search&word=" + word + "&page=" + pageid;
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
            string load = client.DownloadString(keywordurl);

            string getJson = load;
            Root rt = JsonConvert.DeserializeObject<Root>(getJson);
            for (int i = 0; i < rt.illusts.Count; i++)
            {
                listBox1.Items.Add("Pixiv id=" + rt.illusts[i].id.ToString() + " 总浏览数：" + rt.illusts[i].total_view);
            }
            this.tabControl1.SelectedIndex = 1;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem != null)
            {
                Showid();
            }
        }

        private void Showid()
        {
            string id = this.listBox1.SelectedItem.ToString().Substring(9, 8);
            string url = "https://api.imjad.cn/pixiv/v2/?type=illust&id=" + id;
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
            string load = client.DownloadString(url);
            string getJson = load;

            Root rt = JsonConvert.DeserializeObject<Root>(getJson);
            //作品信息
            zpid = rt.illust.id;                                    //作品id
            string zpbt = rt.illust.title;                          //作品标题
            int hsid = rt.illust.user.id;                           //画师id
            string hsnc = rt.illust.user.name;                      //画师昵称
            string hszh = rt.illust.user.account;                   //画师帐号
            string zpsm = rt.illust.caption;                        //作品说明
            string zpck = rt.illust.width + "x" + rt.illust.height; //作品长宽
            string zpsj = rt.illust.create_date;                    //作品上传时间

            //pixv图片地址
            string purl = "https://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + zpid;
            Uri u = new Uri("https://i.pximg.net/c/540x540_10_webp/img-master/img/2017/07/04/01/01/45/63694804_p0_square1200.jpg");
            string temp = u.LocalPath.Remove(0, 19);
            string purl64 = "https://i.pximg.net/c/64x64/" + temp;
            string purl128 = "https://i.pximg.net/c/128x128/" + temp;
            poriginalurl = rt.illust.meta_single_page.original_image_url;
            string pgeturl = "https://img.pixivic.com:23334/get/" + poriginalurl;
            apiurl = "https://api.imjad.cn/interface/img/PixivProxy.php?url=" + rt.illust.image_urls.medium;
            string qturl = "\r\n" + rt.illust.image_urls.square_medium + "\r\n" + rt.illust.image_urls.large + "\r\n" + rt.illust.user.profile_image_urls.medium;

            //pixv图片信息
            this.textBox2.Text = "作品  id：" + zpid + "\r\n作品标题：" + zpbt + "\r\n画师  id：" + hsid + "\r\n画师昵称：" + hsnc + "\r\n画师帐号：" + hszh + "\r\n作品说明：" + zpsm + "\r\n作品长宽：" + zpck + "\r\n作品上传时间：" + zpsj + "\r\n\r\nPixivUrl：" + purl + "\r\nPixiv64x64：" + purl64 + "\r\nPixiv128x128：" + purl128 + "\r\n原始PixivUrl：" + poriginalurl + "\r\nPixivGetUrl：" + pgeturl + "\r\n\r\nimjadApiUrl：" + apiurl + "\r\n\r\n其他大小Url：" + qturl;
            
            //图片预览
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(poriginalurl);
            httpWebRequest.Referer = purl;
            httpWebRequest.Method = "GET";
            HttpWebResponse webResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = webResponse.GetResponseStream();
            this.pictureBox1.Image = Image.FromStream(stream);
        }
    }
}
