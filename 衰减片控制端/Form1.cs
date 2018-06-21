using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 衰减片控制端
{
    public partial class Form1 : Form
    {
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。（可变的字符序列）
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string ComNum;                      //生成COM口名称
            for (int i = 0; i < 20; i++)
            {
                ComNum = i.ToString();
                ComNum = "COM" + ComNum;
                comboBox1.Items.Add(ComNum);
            }

            comboBox2.Items.Add(43000);         //生成波特率
            comboBox2.Items.Add(56000);
            comboBox2.Items.Add(57600);
            comboBox2.Items.Add(115200);

            comboBox3.Items.Add("None");        //生成校验位
            comboBox3.Items.Add("Odd");
            comboBox3.Items.Add("Event");
            comboBox3.Items.Add("Mark");
            comboBox3.Items.Add("Space");

            comboBox4.Items.Add(8);             //生成数据位
            comboBox4.Items.Add(7);
            comboBox4.Items.Add(6);

            comboBox5.Items.Add("None");        //生成停止位
            comboBox5.Items.Add("One");
            comboBox5.Items.Add("OnePointFive");
            comboBox5.Items.Add("Two");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)                   //打开串口
        {
            if (!serialPort1.IsOpen)
            {
                try
                {
                    //boxwrite();
                    serialPort1.Open();
                    button1.Text = "关闭串口";
                    serialPort1.DataReceived += serialPort1_DataReceived;
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                }
            }
            else if (serialPort1.IsOpen)
            {
                button1.Text = "打开串口";
                serialPort1.Close();
            }
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        }

        private void serialPort1_DataReceived(object sender, EventArgs e)           //串口接收函数（数据接收的事件）
        { 
            while (serialPort1.BytesToRead != 0)
            {
                System.Threading.Thread.Sleep(100);
                /*延时为了让所有数据到达,不延时会有数据丢失
                如果数据太长延时100ms也是不够的，最好把接
                收的数据存入缓存区中,校检数据完整后再发出*/
                int count = serialPort1.BytesToRead;
                byte[] data = new byte[count];
                serialPort1.Read(data, 0, data.Length);
                builder.Clear();//清除字符串构造器的内容
                //因为要访问ui资源，所以需要使用invoke方式同步ui。
                this.Invoke((EventHandler)(delegate
                {
                    //判断是否是显示为16进制
                    if (false)
                    {
                        //依次的拼接出16进制字符串
                        foreach (byte b in data)
                        {
                            builder.Append(b.ToString("X2") + " ");
                        }
                    }
                    else if (true)
                    {
                        //直接按UTF8规则转换成字符串，ASCII码转中文乱码，UTF8不会
                        builder.Append(Encoding.UTF8.GetString(data));
                    }
                    //追加的形式添加到文本框末端，并滚动到最后。
                    //textBox2.AppendText(System.DateTime.Now.ToString() + ": " + builder.ToString() +  "\n") ;
                    textBox1.AppendText(count.ToString() + "个字符: " + builder.ToString() + "\n");
                }));
            }
        }
    }
}
