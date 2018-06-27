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
            comboBox2.Items.Add(1000000);

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

            button2.Enabled = false;                             //发送键
            button3.Enabled = false;                             //
            button4.Enabled = false;                             //
            button5.Enabled = false;                             //
            button6.Enabled = false;                             //

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
                    boxwrite();
                    serialPort1.Open();
                    button1.Text = "关闭串口";
                    serialPort1.DataReceived += serialPort1_DataReceived;
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                }

                button2.Enabled = true;                             //发送键
                button3.Enabled = true;                             //
                button4.Enabled = true;                             //
                button5.Enabled = true;                             //
                button6.Enabled = true;                             //
            }
            else if (serialPort1.IsOpen)
            {
                button1.Text = "打开串口";
                button2.Enabled = false;                             //发送键
                button3.Enabled = false;                             //
                button4.Enabled = false;                             //
                button5.Enabled = false;                             //
                button6.Enabled = false;                             //

                serialPort1.Close();

            }
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);


           

        }

        private void button2_Click(object sender, EventArgs e)                    //发送按键
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("发送数据为空！");
                return;
            }
            if (false)//ASCII码直接发送
            {
                string serialStringTemp = this.textBox2.Text;
                this.serialPort1.WriteLine(serialStringTemp);
            }
            else if (true)
            {
                //byte[] BSendTemp = System.Text.Encoding.UTF8.GetBytes(textBox2.Text); //string转字节存入数组
                //serialPort1.Write(BSendTemp, 0, BSendTemp.Length);//发送数据    
                string textData = textBox2.Text.Replace("\r\n", ""); ;
                //string textData = "FF FF 01 09 03 2A 00 88 00 00 E8 03 55";
                //string textData = "FF FF 01 0A 03 29 01 00 00 00 00 5F 00 68";
                string[] grp = textData.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                List<byte> list = new List<byte>();

                foreach (var item in grp)
                {
                    list.Add(Convert.ToByte(item, 16));
                }

                serialPort1.Write(list.ToArray(), 0, list.Count);
            }

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
                    if (true)
                    {
                        //依次的拼接出16进制字符串
                        foreach (byte b in data)
                        {
                            builder.Append(b.ToString("X2") + " ");
                        }
                    }
                    else if (false)
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

        private void boxwrite()
        {
            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = int.Parse(comboBox2.Text);

            //serialPort1.Parity = (System.IO.Ports)comboBox3.Text;//校验位
            if (comboBox3.Text == "Even")
                serialPort1.Parity = Parity.Even;
            if (comboBox3.Text == "Mark")
                serialPort1.Parity = Parity.Mark;
            if (comboBox3.Text == "None")
                serialPort1.Parity = Parity.None;
            if (comboBox3.Text == "Odd")
                serialPort1.Parity = Parity.Odd;
            if (comboBox3.Text == "Space")
                serialPort1.Parity = Parity.Space;

            if (comboBox4.Text != "")
                serialPort1.DataBits = int.Parse(comboBox4.Text);//数据位

            //serialPort1.StopBits = StopBits.None;//停止位
            if (comboBox5.Text == "None")
                serialPort1.StopBits = StopBits.None;
            if (comboBox5.Text == "One")
                serialPort1.StopBits = StopBits.One;
            if (comboBox5.Text == "OnePointFive")
                serialPort1.StopBits = StopBits.OnePointFive;
            if (comboBox5.Text == "Two")
                serialPort1.StopBits = StopBits.Two;
        }
       
        private void button3_Click(object sender, EventArgs e)   //舵机1：0° 舵机2：0°
         {
            CommandSend(1, 7, 42, 2000, 0);
            CommandSend(2, 7, 42, 2000, 0);

            button3.Enabled = false;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button4_Click(object sender, EventArgs e)   //舵机1：90° 舵机2：0°
        {
            CommandSend(1, 7, 42, 2000, 341);
            CommandSend(2, 7, 42, 2000, 0);

            button3.Enabled = true;                            //
            button4.Enabled = false;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                             //
        }

        private void button5_Click(object sender, EventArgs e)    //舵机1：180° 舵机2：0°
        {
            CommandSend(1, 7, 42, 2000, 682);
            CommandSend(2, 7, 42, 2000, 0);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = false;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button6_Click(object sender, EventArgs e)             //舵机1：270° 舵机2：0°
        {
            CommandSend(1, 7, 42, 2000, 1023);
            CommandSend(2, 7, 42, 2000, 0);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = false;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 0);
            CommandSend(2, 7, 42, 2000, 341);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = false;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 341);
            CommandSend(2, 7, 42, 2000, 341);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = false;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button9_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 682);
            CommandSend(2, 7, 42, 2000, 341);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = false;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button10_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 1023);
            CommandSend(2, 7, 42, 2000, 341);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = false;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button11_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 0);
            CommandSend(2, 7, 42, 2000, 682);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = false;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button12_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 341);
            CommandSend(2, 7, 42, 2000, 682);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = false;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button13_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 682);
            CommandSend(2, 7, 42, 2000, 682);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = false;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button14_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 1023);
            CommandSend(2, 7, 42, 2000, 682);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = false;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button15_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 0);
            CommandSend(2, 7, 42, 2000, 1023);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = false;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button16_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 341);
            CommandSend(2, 7, 42, 2000, 1023);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = false;                            //
            button17.Enabled = true;                            //
            button18.Enabled = true;                            //
        }

        private void button17_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 682);
            CommandSend(2, 7, 42, 2000, 1023);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = false;                            //
            button18.Enabled = true;                            //
        }

        private void button18_Click(object sender, EventArgs e)
        {
            CommandSend(1, 7, 42, 2000, 1032);
            CommandSend(2, 7, 42, 2000, 1032);

            button3.Enabled = true;                            //
            button4.Enabled = true;                             //
            button5.Enabled = true;                             //
            button6.Enabled = true;                             //
            button7.Enabled = true;                             //
            button8.Enabled = true;                             //
            button9.Enabled = true;                             //
            button10.Enabled = true;                            //
            button11.Enabled = true;                            //
            button12.Enabled = true;                            //
            button13.Enabled = true;                            //
            button14.Enabled = true;                            //
            button15.Enabled = true;                            //
            button16.Enabled = true;                            //
            button17.Enabled = true;                            //
            button18.Enabled = false;                            //
        }


        private void CommandSend(byte ID, int DateLen, byte MemAddr, ushort Speed, short Pos)  //写指令函数
        {
            int msgLen = DateLen + 2;           //DateLen  7 or 8
            byte[] bBuf = new byte[5 + msgLen];
            byte CheckSum = 0;
            bBuf[0] = 0xff;            //数据头
            bBuf[1] = 0xff;
            bBuf[2] = ID;              //ID
            bBuf[3] = (byte)msgLen;    //参数长度
            bBuf[4] = 03;              //写命令
            bBuf[5] = MemAddr;         //写地址       42 or 41

            int PosH = Pos >> 8;
            int PosL = Pos & 255;
        

            int SpeedH = Speed >> 8;
            int SpeedL = Speed & 255;

            if (msgLen ==8)                             //SC20XX
            {
                bBuf[6] = 0;              //加速度
                bBuf[7] = (byte)PosH;     //位置高
                bBuf[8] = (byte)PosL;     //位置低
                bBuf[9] = 0;              //时间高
                bBuf[10] = 0;             //时间低
                bBuf[11] = (byte)SpeedH;  //速度高
                bBuf[12] = (byte)SpeedL;  //速度低

                CheckSum = (byte)(bBuf[2] + bBuf[3] + bBuf[4] + bBuf[5] + bBuf[6] + bBuf[7] + bBuf[8] + bBuf[9] + bBuf[10] + bBuf[11] + bBuf[12]);
                bBuf[13] = (byte)(~CheckSum);
            }
            //SCS2332
            bBuf[6] = (byte)PosH;     //位置高
            bBuf[7] = (byte)PosL;     //位置低
            bBuf[8] = 0;              //时间高
            bBuf[9] = 0;              //时间低
            bBuf[10] = (byte)SpeedH;  //速度高
            bBuf[11] = (byte)SpeedL;  //速度低

            CheckSum = (byte)(bBuf[2] + bBuf[3] + bBuf[4] + bBuf[5] + bBuf[6] + bBuf[7] + bBuf[8] + bBuf[9] + bBuf[10] + bBuf[11]);
            bBuf[12] = (byte)(~CheckSum);
            serialPort1.Write(bBuf, 0, 5 + msgLen);
        }


        string HexFromInt(int I)
        {
            return I.ToString("X");
        }

        int IntFromHex(string HexI)
        {
            return int.Parse(HexI, System.Globalization.NumberStyles.HexNumber);
        }


    }
}
