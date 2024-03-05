using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NavicatCrypto;
using System.Xml.Linq;
using System.Xml;

namespace navicatc
{
    public partial class Form1 : Form
    {
        Navicat11Cipher navicatCipher11 = new Navicat11Cipher();
        Navicat12Cipher navicatCipher12 = new Navicat12Cipher();
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // 获取ComboBox1和ComboBox2中选择的数据
            string selectedDataComboBox1 = comboBox1.SelectedItem?.ToString();
            string selectedDataComboBox2 = comboBox2.SelectedItem?.ToString();
            string textBox1Value = textBox1.Text;

            // 检查是否有选择的数据
            if (string.IsNullOrEmpty(selectedDataComboBox1) || string.IsNullOrEmpty(selectedDataComboBox2) || string.IsNullOrEmpty(textBox1Value) || textBox1Value == "请输入密码字符串……")
            {
                MessageBox.Show("请选择版本和操作选项，并输入密码字符串！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (selectedDataComboBox1=="11" && selectedDataComboBox2=="加密")
            {
                string encrypted11Result = navicatCipher11.EncryptString(textBox1Value);
                textBox1.Text = "加密的密码为："+encrypted11Result;
            }
            else if (selectedDataComboBox1 == "11" && selectedDataComboBox2 == "解密")
            {
                string decrypted11Result = navicatCipher11.DecryptString(textBox1Value);
                textBox1.Text = "解密后的密码为："+decrypted11Result;
            }
            if (selectedDataComboBox1 == "12" && selectedDataComboBox2 == "加密")
            {
                string encrypted12Result = navicatCipher12.EncryptStringForNCX(textBox1Value);
                textBox1.Text = "加密的密码为："+encrypted12Result;
            }
            else if (selectedDataComboBox1 == "12" && selectedDataComboBox2 == "解密")
            {
                string decrypted12Result = navicatCipher12.DecryptStringForNCX(textBox1Value);
                textBox1.Text = "解密后的密码为：" + decrypted12Result;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "请输入密码字符串……";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void TextBox1_LostFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "请输入密码字符串……";
                textBox1.ForeColor = Color.Gray;
            }
            else
            {
                textBox2.ForeColor = Color.Black;
            }
        }
        private void TextBox1_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == "请输入密码字符串……")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        public bool textIsValue = true;
        private void button3_Click(object sender, EventArgs e)
        {
            if (textIsValue)
            {
                textBox2.Text = "加密密码字符串查找方法：\r\n一、注册表查找密码，加密版本为11：\r\n1、打开注册表并找到HCU\\SOFTWARE\\PremiumSoft\\Navicat\\Servers\r\n2、点击相应的服务器，在右边窗口中找到字符串键Pwd，双击此键\r\n3、在出现的窗口中，“数值数据”下方的框中即为使用11版本加密的密码。\r\n二、从软件导出密码，加密版本为12：\r\n1、点击软件的“文件”——“导出连接”\r\n2、勾选下方的“导出密码”，导出.ncx连接文件\r\n3、用文本编辑器打开导出的.ncx文件，找到服务器对应的Password，等号后面的字符串即为使用12版本加密的密码。\r\n作者：飘风剑，qq：12315557，邮箱：whhlcj@163.com";
            }
            else
            {
                textBox2.Text = string.Empty;
            }
            textIsValue = !textIsValue;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.Text = string.Empty;
            comboBox2.Text = string.Empty;
            textBox1.Text = null;
            if (textBox1.Text == "")
            {
                textBox1.Text = "请输入密码字符串……";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ncxDialog = new OpenFileDialog();
            ncxDialog.Filter = "NCX Files (*.ncx)|*.ncx";

            if (ncxDialog.ShowDialog() == DialogResult.OK)
            {
                string ncxFilePath = ncxDialog.FileName;
                string xmlContent = System.IO.File.ReadAllText(ncxFilePath);
                XDocument xDoc = XDocument.Parse(xmlContent);
                var connections = xDoc.Descendants("Connection");
                foreach (var connection in connections)
                {
                    string host = connection.Attribute("Host")?.Value ?? "N/A";
                    string de_password = connection.Attribute("Password")?.Value ?? "N/A";
                    string password = navicatCipher12.DecryptStringForNCX(de_password);

                    textBox2.AppendText($"Host: {host}, Password: {password}" + Environment.NewLine);
                }

            }

        }
    }
}