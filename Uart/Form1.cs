using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            
        }
        private void Form1_Leave(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }



        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(400);
            string i = serialPort1.ReadExisting();
            int a = analyze_date(i);
            if (a == 1)
            {
                MessageBox.Show("123");
            //打开LED1
            };
            if (a == 2) 
            { 
            //打开LED2
            };
            if (a == 3)
            { 
            //打开LED3
            };
            if (a == 4)
            { 
            //打开LED4
            };
            if (a == 5)
            { 
            //返回温度信息
            };

        }


        /* send_msg(string phone_number, string message, int temp1,int temp2,int temp3,int temp4)
         * 函数功能：通过AT指令发送信息到指定的手机
         * 函数参数：phone_number接收信息的电话号码；
         *           message为要发送的提示信息；
         *           temp1~4为4个节点的温度
         */
        private void send_msg(string phone_number, string message, int temp1,int temp2,int temp3,int temp4)
        {
            message = message + "\r\n site 1 : " + temp1 + "degree\r\n site 2 :" + temp2 + "degree\r\n site 3 :" + temp3 + "degree\r\n site 4 :" + temp4 + "degree\r\n";
            byte[] b;
            b = Encoding.Default.GetBytes("AT+CMGS=\"" + phone_number + "\"\r\n" + message);
            byte[] c = new byte[b.Length + 1];
            b.CopyTo(c, 0);
            c[b.Length] = 26;
            serialPort1.Write(c, 0, c.Length);
        }


        /* ======================init_gprs()==============================
         * 函数功能：通过AT指令初始化GSM设备
         */
        private void init_gprs()
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            send_command("ATI\r\n" + "AT+CMGF=1\r\n" + "AT+CMGD=1\r\n");
        }


        /* ===============send_command(string command)=====================
         * 
         * 函数功能：通过串口发送控制指令
         * 函数参数：string类型的指令
         */
        private void send_command(string command)
        {
            byte[] a;
            a = Encoding.Default.GetBytes(command);
            serialPort1.Write(a,0,a.Length);
        }


        /* =============analyze_date(string i)=======================
         * 
         * 函数功能：分析接连接GSM模块的串口收到的数据 
         * 函数参数：i为通过串口接收到的数据。
         * 函数返回值：如果是数据表示的是控制信息则返回控制信号：“1、2、3、4、5”
         * 如果是数据表示接收信息则通过串口发出读取信息的命令，返回0。
         * 如果以上的都不是则不做任何处理，返回0。
         */
        private int analyze_date(string i)
        {
            //当串口收到的信息长度为17时，表示GSM模块有接收到信息
            int a = 0;
            string b="0";
            if (i.Length == 17) send_command("AT+CMGR=1\r\n");

            //当串口收到的信息长度为85或83时，表示通过串口读取到了信息
            if (i.Length == 85) b = i.Substring(76, 1);
            if (i.Length == 83) b = i.Substring(74, 1);
            a = Convert.ToInt32(b);
            if (a > 0 && a < 10) send_command("AT+CMGD=1\r\n");
            return a;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "关闭短信控制")
            {
                button3.Text = "打开短信控制";

                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
            }
            else
            { 
            try
            {
                if(serialPort1.IsOpen)
                    {
                        serialPort1.Close();
                    }
            
                    //打开combo1选择的串口
                    serialPort1.PortName =comboBox1.SelectedItem.ToString();
                    serialPort1.Open();
                    button3.Text = "关闭短信控制";
                    init_gprs();        
                }
                catch
                {
                    MessageBox.Show("没发现次串口或串口已经在使用");
                    button3.Text = "打开短信控制";
                }            
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                init_gprs();
            }
            catch
            {
                MessageBox.Show("没发现次串口或串口已经在使用");
            }
            
        }


    }
}
