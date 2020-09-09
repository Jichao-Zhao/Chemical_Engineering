using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;


namespace huagong
{
    public partial class Form1 : Form
    {
        string Process_Time;
        int now_time;
        int show_time;
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                comboBox1.Items.Add(i);
                comboBox1.Text = "请选择速度";
            }
            for (int j = 1; j <= 20; j++)
            {
                comboBox2.Items.Add("COM" + j.ToString());          //统一添加"0x"
                comboBox2.Text = "COM4";
            }
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);//必须手动添加事件处理

            Series series = chart1.Series[0];                       //设置图线的样式
            series.ChartType = SeriesChartType.FastLine;            //画样条曲线（Spliine）
            series.BorderWidth = 2;                                 //线宽 2 个像素
            series.Color = System.Drawing.Color.Red;                //线的颜色 红色
            series.LegendText = "光线强度";                         //图示上的文字

            //设置显示范围包括横纵轴坐标的最大最小值
            ChartArea chartArea = Chart1.ChartAreas[0];
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = 1000;
            chartArea.AxisY.Minimum = 0d;
            chartArea.AxisY.Maximum = 100d;

            //滚动条位于图表区内还是图表区外 是否使能滑动条
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;

            chart1.ChartAreas[0].AxisX.ScaleView.Position = 0;      //指当前（最后边）显示的是第几个
            
            //视野范围内共有多少个数据点，动态折线图的关键就是根据量的不同增加这个变量
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 100;

        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)//串口数据接收事件
        {
            try
            {
                if(serialPort1.ReadBufferSize > 0)                  //判断缓存区存在数据
                {
                    string str = serialPort1.ReadExisting();        //字符串方式读取串口缓存区数据
                    textBox7.AppendText(str);
                    
                    string[] data = new string[2000];         //声明一个int类型数组并初始化长度为2000
                    for(int x = 0; x < 2000; x++)
                    {
                        data[x] = str;
                    }



                    Series series = chart1.Series[0];
                    int j = 0;
                    j = j + 1;
                    int str_data;
                    int.TryParse(str, out str_data);
                    if (str_data < 10)
                    {
                        //return 0;
                    }
                    else
                    {
                        series.Points.AddXY(j.ToString(), str);                  //添加一个点
                    }


                    //chart1.ChartAreas[0].AxisX.ScaleView.Size = ;   //


                    now_time = Convert.ToInt16(DateTime.Now.Second.ToString());
                    Process_Time = DateTime.Now.Second.ToString();  //读取当前时间的 秒 数
                    //show_time = Convert.ToInt16(Process_Time) - now_time;
                    //string time = show_time.ToString();
                    if (Process_Time.Length == 1)
                    {
                        textBox8.AppendText("0" + DateTime.Now.Second.ToString());
                        if (now_time == 0)
                        {
                            int i = 0;
                            i++;
                            textBox5.AppendText(i.ToString());
                        }
                    }
                    else
                    {
                        textBox8.AppendText(DateTime.Now.Second.ToString());

                    }



                    //timer1.Start();
                    //timer1.Interval = 10000;                         //设置计时器时间间隔
                    //Process_Time++;                                 //程序运行时间Process_Time自加1
                    //textBox8.AppendText(Process_Time.ToString() + "\r\n");

                }
            }
            catch
            {

            }
        }

        private void button1_Click_1(object sender, EventArgs e)    //开始测量
        {
            try                                                     //尝试判断串口选择正确
            {
                serialPort1.PortName = comboBox2.Text;
                serialPort1.Open();
                chart1.Series[0].Points.Clear();                    //清除图标chart
                button1.Enabled = false;                            //打开串口按钮不可用
                button3.Enabled = true;                             //停止测量按钮可用
                try                                                 //尝试判断采集速率已经选择
                {
                    string a = comboBox1.Text;                      //判断速度框的字符串是否为数字
                    int b = int.Parse(a);                           //判断速度框的字符串是否为数字
                    serialPort1.WriteLine(Convert.ToString(0));     //发送字符串0，表示开始
                }
                catch
                {
                    serialPort1.Close();
                    button1.Enabled = true;                         //打开串口按钮不可用
                    button3.Enabled = false;                        //停止测量按钮可用
                    MessageBox.Show("未选择测量速度", "速度错误");
                }
            }
            catch
            {
                MessageBox.Show("端口错误，请检查串口", "端口错误");
            }
        }

        private void button3_Click(object sender, EventArgs e)      //停止测量事件
        {
            serialPort1.Close();                                    //关闭串口
            button1.Enabled = true;                                 //打开串口按钮可用
            button3.Enabled = false;                                //关闭串口按钮不可用
            //chart1.Series[0].Points.Clear();                        //清除图标chart
        }

        private void button2_Click(object sender, EventArgs e)      //数据清除事件
        {
            textBox8.Clear();                                       //清除时间数据
            textBox7.Clear();                                       //清除光线强度数据
        }

        private void chart1_Click(object sender, EventArgs e)       //双击显示滚动条
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 5;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        }

        private void InitChart()
        {
            DateTime time = DateTime.Now;
            int chart1Timer = 1000;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
