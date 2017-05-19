using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Monitor_disk_capacity_For_i_Eco
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string password = "72103650";
        string folderpath;
        DriveInfo di;
        long totalfreespace, totalsize;
        int diskpercent;
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                folderpath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string psw = Interaction.InputBox("請輸入密碼以確認動作", "權限密碼", "", -1, -1);
            if (psw != "")
            {
                if (psw == password)
                {
                    checkdisk();
                    timer1.Enabled = !timer1.Enabled;
                    Synchronous();
                }
                else
                {
                    MessageBox.Show("密碼錯誤，無法進行動作!", "權限錯誤");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            folderpath = textBox1.Text;
            checkdisk();
            Synchronous();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@folderpath);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(textBox2.Text==""||int.Parse(textBox2.Text)<1||int.Parse(textBox2.Text)>86400)
            {
                textBox2.Text = "3";
            }
            timer1.Interval = int.Parse(textBox2.Text) * 1000;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || int.Parse(textBox3.Text) < 1 || int.Parse(textBox3.Text) > 99)
            {
                textBox3.Text = "10";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            totalfreespace = di.TotalFreeSpace;
            totalsize = di.TotalSize;
            diskpercent = (int)(((float)totalfreespace / (float)totalsize) * 100);
            progressBar1.Value = diskpercent;
            label2.Text = "還可使用空間:" + diskpercent + "%";
            if(diskpercent < int.Parse(textBox3.Text))
            {
                autodellogfile();
            }
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.SelectAll();
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            textBox3.SelectAll();
        }

        private void checkdisk()
        {
            foreach(DriveInfo fi in DriveInfo.GetDrives())
            {
                if(folderpath.Substring(0,3)==fi.Name)
                {
                    di = fi;
                    totalsize = di.TotalSize;
                    totalfreespace = di.TotalFreeSpace;
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string psw = Interaction.InputBox("請輸入密碼以確認刪除", "權限密碼", "", -1, -1);
            if (psw != "")
            {
                if (psw == password)
                {
                    dellogfile();
                }
                else
                {
                    MessageBox.Show("密碼錯誤，無法刪除!", "權限錯誤");
                }
            }
        }

        private void Synchronous()
        {
            button3.Text = timer1.Enabled ? "停止" : "執行";
            button3.BackColor = timer1.Enabled ? Color.Red : Color.Green;
            button1.Enabled = !timer1.Enabled;
            textBox2.ReadOnly = timer1.Enabled;
            textBox3.ReadOnly = timer1.Enabled;
            timer1.Interval = int.Parse(textBox2.Text) * 100;
            progressBar1.Value = timer1.Enabled ? diskpercent : 0;
            label2.Text = timer1.Enabled ? label2.Text : "容量";
        }
        
        string willdelkeyword = "Log**************.txt";
        private void dellogfile()
        {
            string reg = willdelkeyword.Substring(0, 3);
            int keywordlen = willdelkeyword.Length,i=0;
            string[] delfile = new string[0] { };
            long totalfilesize = 0;
            try
            {
                foreach(string filename in Directory.GetFileSystemEntries(folderpath,"*.txt"))
                {
                    string repl = filename.Replace(folderpath, "");
                    if (repl.Length== keywordlen & repl.Substring(0,3)==reg)
                    {
                        Array.Resize(ref delfile, delfile.Length + 1);
                        totalfilesize += new FileInfo(@filename).Length;
                        delfile[i++] = filename;
                    }
                }
                DialogResult myResult = MessageBox.Show("是否要刪除位於" + folderpath + "的"+ delfile.Length + "個記錄檔(大小佔:"+ ((float)totalfilesize / (float)totalsize)*100 + "%)?", "記錄檔", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (myResult == DialogResult.Yes)
                {
                    foreach (string fi in delfile)
                    {
                        File.Delete(@fi);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text += @"\";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show(" 確定退出嗎？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                Application.ExitThread();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void autodellogfile()
        {
            string reg = willdelkeyword.Substring(0, 3);
            int keywordlen = willdelkeyword.Length;
            string[] delfile = new string[0] { };
            try
            {
                foreach (string filename in Directory.GetFileSystemEntries(folderpath, "*.txt"))
                {
                    string repl = filename.Replace(folderpath, "");
                    if (repl.Length == keywordlen && repl.Substring(0, 3) == reg)
                    {
                        File.Delete(@filename);
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (NullReferenceException en)
            {
                MessageBox.Show(en.ToString());
            }
        }
    }
}
