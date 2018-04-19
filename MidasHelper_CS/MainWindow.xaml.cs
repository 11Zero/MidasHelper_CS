using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using Midas;
using XMLoperator;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
//using System.Windows.Forms;


namespace MidasHelper_CS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //private BackThreader threader = null;
        private Queue<int> msgQueue = null;
        //public TextBlock status_bar_text = null;
        private double wall_thickness = 0.0;//壁厚
        private double calibre = 0.0;//管径
        private BackgroundWorker backThreader = null;
        private double[] x_input = null;
        private int x_input_count = 0;
        private double[] y_input = null;
        private int y_input_count = 0;
        private double[] z_input = null;
        private int z_input_count = 0;
        private double[] x_points = null;
        private double[] y_points = null;
        private double[] z_points = null;
        private double x_length = 0.0;
        private double y_length = 0.0;
        private double z_length = 0.0;
        private double[] x_bridging_points = null;
        private double[] y_bridging_points = null;
        private double[] z_bridging_points = null;
        private int x_bridging_count = 0;
        private int y_bridging_count = 0;
        private int z_bridging_count = 0;
        private double tan_alpha = 1.0;
        private Log log = null;
        private bool buckle_analyse = false;
        private bool nolinear_analyse = false;
        //private Mctxt mctfile = null;
        private FileStream mctfile = null;
        public delegate object delegateGetTextCallBack(object text);
        public delegate void delegateSetTextCallBack(string str, object object_id);
        public delegate void delegateSetProcessCallBack(object value);
        private SectionForm section_form = null;
        private DefectForm defect_form = null;
        public bool section_form_closed = true;
        public bool defect_form_closed = true;
        public int selected_section = -1;
        private double h0, h1, h2, h3, l1, l2, G1, G2, G3, P1, P2, P3, Y1, Y2, Y3, J1, J2;
        public int H01 = 0, H02 = 0, H03 = 0;
        public int B01 = 0, B02 = 0, B03 = 0, B04 = 0, B05 = 0, B06 = 0;
        public int H11 = 0, H12 = 0, H21 = 0, H22 = 0, H31 = 0, H32 = 0, H41 = 0;
        public int h11 = 0, h12 = 0, h21 = 0, h22 = 0, h31 = 0, h32 = 0, h41 = 0, h42 = 0;
        public int b11 = 0, b12 = 0, b21 = 0, b22 = 0, b31 = 0, b32 = 0, b41 = 0, b42 = 0;
        public double bridge_length = 0.0;
        public string x_input_info;
        public bool section_selected_flag = false;
        public bool bridging_check = true;

        public MainWindow()
        {
            InitializeComponent();
            //threader = new BackThreader(this);
            backThreader = new BackgroundWorker();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //status_bar_text = new TextBlock();
            //BottomStatusBar.Items.Add(status_bar_text);
            //BottomStatusBar.Items.GetItemAt(0)
            x_input = new double[100];
            y_input = new double[100];
            z_input = new double[100];
            msgQueue = new Queue<int>();
            check_cut.IsChecked = true;
            //section_form = new SectionForm();
            log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
            InitializeBackgroundWorker();
            //getXmlValue();
            PostMessage(2);

        }


        private void InitializeBackgroundWorker()
        {
            backThreader.DoWork += new DoWorkEventHandler(backThreader_DoWork);
            backThreader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backThreader_RunWorkerCompleted);
            backThreader.ProgressChanged += new ProgressChangedEventHandler(backThreader_ProgressChanged);
        }

        private void backThreader_DoWork(object sender,
           DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                if (msgQueue.Count > 0)
                {
                    e.Result = RunMessage((int)msgQueue.Dequeue(), worker, e);
                }
            }
        }

        // This event handler deals with the results of the
        // background operation.
        private void backThreader_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void backThreader_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            //this.progressBar1.Value = e.ProgressPercentage;
        }

        private void PostMessage(int msg)
        {
            if (!backThreader.IsBusy)
                backThreader.RunWorkerAsync(0);
            if (msgQueue.Count > 200)
            {
                SetText("当前消息队列过于拥堵，暂停接收消息", this.status_bar_text);
                return;
            }
            msgQueue.Enqueue(msg);
        }

        private int RunMessage(int msg, BackgroundWorker worker, DoWorkEventArgs e)
        {
            switch (msg)//获取当前消息队列中消息，并一一比对执行相应的动作
            {
                case 1:
                    {
                        msgFunction_1();//例如消息码为1是，执行msgFunction_1()函数
                    } break;
                case 2:
                    {
                        msgFunction_2();//例如消息码为2是，执行msgFunction_2()函数
                    } break;
                //case 3:
                //    {
                //        msgFunction_3();//例如消息码为2是，执行msgFunction_2()函数
                //    } break;
                //case 4:
                //    {
                //        msgFunction_4();//例如消息码为2是，执行msgFunction_2()函数
                //    } break;
                default: break;
            }
            return 0;
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="object_id">text相关控件name</param>
        /// <returns>读取到的值</returns>
        public string GetText(object object_id)
        {
            string result = "";
            if (!Dispatcher.CheckAccess())
            {
                result = (string)Dispatcher.Invoke(DispatcherPriority.Normal, new delegateGetTextCallBack(GetTextCallBack), object_id);
            }
            return result;
        }
        /// <summary>
        /// delegate函数
        /// </summary>
        /// <param name="object_id">text相关控件name</param>
        /// <returns>读取到的值</returns>
        private object GetTextCallBack(object object_id)
        {
            string str = "";
            if (object_id is TextBox)
                str = ((TextBox)object_id).Text;
            if (object_id is TextBlock)
                str = ((TextBlock)object_id).Text;
            return str;
        }
        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="str">要设置的值</param>
        /// <param name="object_id">text相关控件name</param>
        public void SetText(string str, object object_id)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new delegateSetTextCallBack(SetTextCallBack), str, object_id);
                return;
            }
        }
        /// <summary>
        /// delegate函数
        /// </summary>
        /// <param name="str">要设置的值</param>
        /// <param name="object_id">text相关控件name</param>
        private void SetTextCallBack(string str, object object_id)
        {
            if (object_id is TextBox)
                ((TextBox)object_id).Text = str;
            if (object_id is TextBlock)
                ((TextBlock)object_id).Text = str;
        }

        private void SetProcessCallBack(object value)
        {
            status_bar_progress.Value = (int)value;
        }

        private void SetProcess(int value)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new delegateSetProcessCallBack(SetProcessCallBack), value);
                return;
            }
        }
        /// <summary>
        /// 获取x输入框数组
        /// </summary>
        private void get_x_info()
        {
            string x_str = "";
            x_input_count = 0;
            x_length = 0.0;
            x_bridging_count = 0;
            x_str = (string)GetText(text_x_input);
            x_str = x_str.Replace(",", " ");
            if (x_str == "")
            {
                SetText("x输入值无效", this.status_bar_text);
                return;
            }
            string[] str_splited = x_str.Split(' ');
            for (int i = 0; i < str_splited.Length; i++)
            {
                if (str_splited[i].Contains('@'))
                {
                    double i_value = double.Parse(str_splited[i].Substring(str_splited[i].IndexOf('@') + 1));
                    int i_count = int.Parse(str_splited[i].Substring(0, str_splited[i].IndexOf('@')));
                    for (int j = 0; j < i_count; j++)
                    {
                        x_input[x_input_count++] = i_value;
                    }
                }
                else
                    x_input[x_input_count++] = double.Parse(str_splited[i]);
            }
            x_points = new double[x_input_count + 1];
            x_points[0] = 0.0;
            for (int i = 1; i < x_input_count + 1; i++)
            {
                x_points[i] = x_points[i - 1] + x_input[i - 1];
            }
            x_length = x_points[x_points.Length - 1];

            x_bridging_points = new double[200];
            double temp_l1 = 0.0;
            x_bridging_points[x_bridging_count++] = 0.0;
            for (int i = 0; i < x_input_count; i++)
            {
                temp_l1 = temp_l1 + x_input[i];
                if (temp_l1 > 4.5)
                {
                    x_bridging_points[x_bridging_count++] = x_points[i];
                    temp_l1 = x_input[i];
                }
            }
            if (x_bridging_points[x_bridging_count - 1] < x_length)
            {
                x_bridging_points[x_bridging_count++] = x_length;
            }
            //log.log("**********************************************");
            //for (int i = 0; i < x_input_count; i++)
            //{
            //    log.log(x_input[i].ToString());
            //}
            //Parent.FrameStatusBar
        }
        /// <summary>
        /// 获取y输入框数组
        /// </summary>
        private void get_y_info()
        {
            string y_str = "";
            y_input_count = 0;
            y_length = 0.0;
            y_bridging_count = 0;
            y_str = (string)GetText(text_y_input);
            y_str = y_str.Replace(",", " ");
            if (y_str == "")
            {
                SetText("y输入值无效", this.status_bar_text);
                return;
            }
            string[] str_splited = y_str.Split(' ');
            for (int i = 0; i < str_splited.Length; i++)
            {
                if (str_splited[i].Contains('@'))
                {
                    double i_value = double.Parse(str_splited[i].Substring(str_splited[i].IndexOf('@') + 1));
                    int i_count = int.Parse(str_splited[i].Substring(0, str_splited[i].IndexOf('@')));
                    for (int j = 0; j < i_count; j++)
                    {
                        y_input[y_input_count++] = i_value;
                    }
                }
                else
                    y_input[y_input_count++] = double.Parse(str_splited[i]);
            }
            y_points = new double[y_input_count + 1];
            y_points[0] = 0.0;
            for (int i = 1; i < y_input_count + 1; i++)
            {
                y_points[i] = y_points[i - 1] + y_input[i - 1];
            }
            y_length = y_points[y_points.Length - 1];

            y_bridging_points = new double[200];
            double temp_l1 = 0.0;
            y_bridging_points[y_bridging_count++] = 0.0;
            for (int i = 0; i < y_input_count; i++)
            {
                temp_l1 = temp_l1 + y_input[i];
                if (temp_l1 > 4.5)
                {
                    y_bridging_points[y_bridging_count++] = y_points[i];
                    temp_l1 = y_input[i];
                }
            }
            if (y_bridging_points[y_bridging_count - 1] < y_length)
            {
                y_bridging_points[y_bridging_count++] = y_length;
            }
            //log.log("**********************************************");
            //for (int i = 0; i < y_input_count; i++)
            //{
            //    log.log(y_input[i].ToString());
            //}
        }

        /// <summary>
        /// 获取z输入框数组
        /// </summary>
        private void get_z_info()//忽略扫地杆这一间距，扫地杆只布置在底层外围一周
        {
            string z_str = "";
            z_input_count = 0;
            z_length = 0.0;
            z_bridging_count = 0;
            z_str = (string)GetText(text_z_input);
            z_str = z_str.Replace(",", " ");
            if (z_str == "")
            {
                SetText("z输入值无效", this.status_bar_text);
                return;
            }
            string[] str_splited = z_str.Split(' ');
            z_input[z_input_count++] = h3;
            z_input[z_input_count++] = 0.3;
            //z_input[z_input_count++] = h2 - h3;
            for (int i = 0; i < str_splited.Length; i++)
            {
                if (str_splited[i].Contains('@'))
                {
                    double i_value = double.Parse(str_splited[i].Substring(str_splited[i].IndexOf('@') + 1));
                    int i_count = int.Parse(str_splited[i].Substring(0, str_splited[i].IndexOf('@')));
                    for (int j = 0; j < i_count; j++)
                    {
                        z_input[z_input_count++] = i_value;
                    }
                }
                else
                    z_input[z_input_count++] = double.Parse(str_splited[i]);
            }
            z_input[z_input_count++] = h1;
            if (h0 > 0.005)
                z_input[z_input_count++] = h0;
            z_points = new double[z_input_count + 1];
            z_points[0] = 0.0;
            for (int i = 1; i < z_input_count + 1; i++)
            {
                z_points[i] = z_points[i - 1] + z_input[i - 1];
            }
            z_length = z_points[z_points.Length - 1];

            z_bridging_points = new double[200];
            double temp_l1 = z_points[2];
            z_bridging_points[z_bridging_count++] = temp_l1;
            for (int i = 2; i < z_input_count - 2; i++)
            {
                temp_l1 = temp_l1 + z_input[i];
                if (temp_l1 > 4.5)
                {
                    z_bridging_points[z_bridging_count++] = z_points[i];
                    temp_l1 = z_input[i];
                }
            }
            if (z_bridging_points[z_bridging_count - 1] < z_length - h0 - h1)
            {
                z_bridging_points[z_bridging_count++] = z_length - h0 - h1;
            }


            //log.log("**********************************************");
            //for (int i = 0; i < z_input_count; i++)
            //{
            //    log.log(z_input[i].ToString());
            //}
        }

        private MidasNode get_cross_point(MidasElement bridging_line, MidasElement normal_line)//生成正常节点，剪刀撑节点
        {
            MidasNode result = null;
            /////判断a线起点是否在b线上
            //bool judge1 = Math.Abs((bridging_line.fNode.x - normal_line.fNode.x) * (bridging_line.fNode.y - normal_line.bNode.y) - (bridging_line.fNode.x - normal_line.bNode.x) * (bridging_line.fNode.y - normal_line.fNode.y)) > 0.005;
            //bool judge2 = Math.Abs((bridging_line.fNode.x - normal_line.fNode.x) * (bridging_line.fNode.z - normal_line.bNode.z) - (bridging_line.fNode.x - normal_line.bNode.x) * (bridging_line.fNode.z - normal_line.fNode.z)) > 0.005;
            //bool judge3 = Math.Abs((bridging_line.fNode.y - normal_line.fNode.y) * (bridging_line.fNode.y - normal_line.bNode.y) - (bridging_line.fNode.y - normal_line.bNode.y) * (bridging_line.fNode.y - normal_line.fNode.y)) > 0.005;
            /////判断a线终点是否在b线上
            //bool judge4 = Math.Abs((bridging_line.bNode.x - normal_line.fNode.x) * (bridging_line.bNode.y - normal_line.bNode.y) - (bridging_line.bNode.x - normal_line.bNode.x) * (bridging_line.bNode.y - normal_line.fNode.y)) > 0.005;
            //bool judge5 = Math.Abs((bridging_line.bNode.x - normal_line.fNode.x) * (bridging_line.bNode.z - normal_line.bNode.z) - (bridging_line.bNode.x - normal_line.bNode.x) * (bridging_line.bNode.z - normal_line.fNode.z)) > 0.005;
            //bool judge6 = Math.Abs((bridging_line.bNode.y - normal_line.fNode.y) * (bridging_line.bNode.y - normal_line.bNode.y) - (bridging_line.bNode.y - normal_line.bNode.y) * (bridging_line.bNode.y - normal_line.fNode.y)) > 0.005;

            //if (judge1 && judge2 && judge3 && judge4 && judge5 && judge6)///如果a线起点终点都在b线上，则两线重合，无唯一交点
            //{
            //    return result;
            //}
            double x1 = -1.0, y1 = -1.0, x2 = -1.0, y2 = -1.0, a1 = -1.0, b1 = -1.0, a2 = -1.0, b2 = -1.0;
            bool judge7 = Math.Abs(bridging_line.fNode.x - bridging_line.bNode.x) < 0.005 && Math.Abs(normal_line.fNode.x - normal_line.bNode.x) < 0.005 && Math.Abs(bridging_line.fNode.x - normal_line.bNode.x) < 0.005;
            bool judge8 = Math.Abs(bridging_line.fNode.y - bridging_line.bNode.y) < 0.005 && Math.Abs(normal_line.fNode.y - normal_line.bNode.y) < 0.005 && Math.Abs(bridging_line.fNode.y - normal_line.bNode.y) < 0.005;
            bool judge9 = Math.Abs(bridging_line.fNode.z - bridging_line.bNode.z) < 0.005 && Math.Abs(normal_line.fNode.z - normal_line.bNode.z) < 0.005 && Math.Abs(bridging_line.fNode.z - normal_line.bNode.z) < 0.005;
            if (judge7)
            {
                x1 = normal_line.fNode.y;
                y1 = normal_line.fNode.z;
                x2 = normal_line.bNode.y;
                y2 = normal_line.bNode.z;

                a1 = bridging_line.fNode.y;
                b1 = bridging_line.fNode.z;
                a2 = bridging_line.bNode.y;
                b2 = bridging_line.bNode.z;
            }
            else if (judge8)
            {
                x1 = normal_line.fNode.x;
                y1 = normal_line.fNode.z;
                x2 = normal_line.bNode.x;
                y2 = normal_line.bNode.z;

                a1 = bridging_line.fNode.x;
                b1 = bridging_line.fNode.z;
                a2 = bridging_line.bNode.x;
                b2 = bridging_line.bNode.z;
            }
            else if (judge9)
            {
                x1 = normal_line.fNode.x;
                y1 = normal_line.fNode.y;
                x2 = normal_line.bNode.x;
                y2 = normal_line.bNode.y;

                a1 = bridging_line.fNode.x;
                b1 = bridging_line.fNode.y;
                a2 = bridging_line.bNode.x;
                b2 = bridging_line.bNode.y;
            }
            if (Math.Abs(x1 + 1.0) < 0.005)///判断变量未被赋值
            {
                return result;
            }
            double res1 = ((b1 - y1) * (x1 - x2) - (y1 - y2) * (a1 - x1)) * ((b2 - y1) * (x1 - x2) - (y1 - y2) * (a2 - x1));
            double res2 = ((y1 - b1) * (a1 - a2) - (b1 - b2) * (x1 - a1)) * ((y2 - b1) * (a1 - a2) - (b1 - b2) * (x2 - a1));
            if (res1 < 0.005 && res2 < 0.005)
            {
                result = new MidasNode();
                if (Math.Abs(x1 - x2) < 0.005)
                {
                    res1 = x1;
                    res2 = (b1 - b2) * (x1 - a1) / (a1 - a2) + b1;
                }
                else if (Math.Abs(y1 - y2) < 0.005)
                {
                    res1 = y1;
                    res2 = a1 + (y1 - b1) * (a1 - a2) / (b1 - b2);
                }
                else
                {
                    res1 = (x1 * (y1 - y2) * (a1 - a2) - a1 * (b1 - b2) * (x1 - x2) - y1 + b1) / ((y1 - y2) * (a1 - a2) - (b1 - b2) * (x1 - x2));
                    res2 = (b1 - b2) * (res1 - a1) / (a1 - a2) + b1;
                }
                if (judge7)
                {
                    result.num = 1;
                    result.y = res1;
                    result.z = res2;
                    result.x = normal_line.bNode.x;
                }
                else if (judge8)
                {
                    result.num = 1;
                    result.x = res1;
                    result.z = res2;
                    result.y = normal_line.bNode.y;
                }
                else if (judge9)
                {
                    result.num = 1;
                    result.x = res2;
                    result.y = res1;
                    result.z = normal_line.bNode.z;
                }
                else
                    result = null;
            }
            return result;
        }


        private bool judge_node_inline(MidasNode node, MidasElement line)
        {
            bool judge1 = Math.Abs((node.x - line.fNode.x) * (node.y - line.bNode.y) - (node.x - line.bNode.x) * (node.y - line.fNode.y)) < 0.005;
            bool judge2 = Math.Abs((node.z - line.fNode.z) * (node.y - line.bNode.y) - (node.z - line.bNode.z) * (node.y - line.fNode.y)) < 0.005;
            bool judge3 = Math.Abs((node.z - line.fNode.z) * (node.x - line.bNode.x) - (node.z - line.bNode.z) * (node.x - line.fNode.x)) < 0.005;
            if (judge1 && judge2 && judge3)
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 存储文件对话框
        /// </summary>
        private void msgFunction_1()//写入mct
        {
            #region 判断参数是否有效
            if (Math.Abs(h0) < 0.005)
            {
                MessageBox.Show("顶层可调托撑输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(h1) < 0.005)
            {
                MessageBox.Show("顶层可调托撑输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(h3) < 0.005)
            {
                MessageBox.Show("下部可调托撑输入格式不对或输入值过小");
                return;
            }
            if (h2 <= h3)
            {
                MessageBox.Show("扫地杆高度不能小于下部可调托撑高度\n前者包含后者");
                return;
            }
            if (Math.Abs(h2) < 0.005)
            {
                MessageBox.Show("扫地杆高度输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(l1) < 0.005)
            {
                MessageBox.Show("剪刀撑面间距输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(l2) < 0.005)
            {
                MessageBox.Show("剪刀撑线间距输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(G1) < 0.005)
            {
                MessageBox.Show("混凝土自重输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(G2) < 0.005)
            {
                MessageBox.Show("模板支撑梁自重输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(G3) < 0.005)
            {
                MessageBox.Show("附加构件荷载输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(G3) < 0.005)
            {
                MessageBox.Show("附加构件荷载输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(G3) < 0.005)
            {
                MessageBox.Show("附加构件荷载输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(G3) < 0.005)
            {
                MessageBox.Show("附加构件荷载输入格式不对或输入值过小");
                return;
            }

            if (Math.Abs(P1) < 0.005)
            {
                MessageBox.Show("施工人员材料设备荷载输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(P2) < 0.005)
            {
                MessageBox.Show("浇筑振捣混凝土荷载输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(P3) < 0.005)
            {
                MessageBox.Show("风荷载输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(Y1) < 0.005)
            {
                MessageBox.Show("第一次预压荷载系数输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(Y2) < 0.005)
            {
                MessageBox.Show("第二次预压荷载系数输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(Y3) < 0.005)
            {
                MessageBox.Show("第三次预压荷载系数输入格式不对或输入值过小");
                return;
            }
            if (J1 < 0.005)
            {
                MessageBox.Show("第一次浇筑荷载系数输入格式不对或输入值过小");
                return;
            }
            if (J1 > 1)
            {
                MessageBox.Show("第一次浇筑荷载系数输入不能大于1");
                return;
            }
            if (J2 < 0.005)
            {
                MessageBox.Show("第二次浇筑荷载系数输入格式不对或输入值过小");
                return;
            }
            if (J2 > 1)
            {
                MessageBox.Show("第二次浇筑荷载系数输入不能大于1");
                return;
            }
            if (J1 + J2 > 1.0)
            {
                MessageBox.Show("两次浇筑荷载系数和必须为1");
                return;
            }
            #endregion
            setXmlValue();

            SetProcess(0);
            get_x_info();//生成x间距数组和点坐标数组
            get_y_info();//生成y间距数组和点坐标数组
            get_z_info();//生成z间距数组和点坐标数组
            if (x_input_count == 0 || y_input_count == 0 || z_input_count == 0)
            {
                MessageBox.Show("支架参数获取不足");
                return;
            }

            //////////////////////////////////////////////写入文件部分////////////////////////////////
            //创建一个保存文件式的对话框
            SaveFileDialog sfd = new SaveFileDialog();
            //设置这个对话框的起始保存路径
            sfd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //设置保存的文件的类型，注意过滤器的语法
            sfd.Filter = "mct文件|*.mct";
            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮
            StreamWriter writer = null;
            System.IO.FileInfo fileInfo = null;

            string buffer_string = "";

            if (sfd.ShowDialog() == true)
            {
                if (sfd.SafeFileName.IndexOf(".mct") > 0)
                {
                    fileInfo = new System.IO.FileInfo(sfd.FileName);
                    if (!fileInfo.Exists)
                    {
                        mctfile = fileInfo.Create();
                        writer = new StreamWriter(mctfile, Encoding.Default);
                    }
                    else
                    {
                        mctfile = fileInfo.Open(FileMode.Create, FileAccess.Write);
                        writer = new StreamWriter(mctfile, Encoding.Default);
                    }
                }
                else
                {
                    fileInfo = new System.IO.FileInfo(sfd.FileName + ".mct");
                    if (!fileInfo.Exists)
                    {
                        mctfile = fileInfo.Create();
                        writer = new StreamWriter(mctfile, Encoding.Default);
                    }
                    else
                    {
                        mctfile = fileInfo.Open(FileMode.Create, FileAccess.Write);
                        writer = new StreamWriter(mctfile, Encoding.Default);
                    }
                }
                SetText("创建成功", this.status_bar_text);
            }
            else
            {
                SetText("取消创建", this.status_bar_text);
                return;
            }

            //MidasNode node = new MidasNode();
            MidasNode[] all_normal_nodes = new MidasNode[40000];//未计入底层扫地杆节点
            int regular_nodes_start = 1;
            for (int i = 0; i < all_normal_nodes.Length; i++)
            {
                all_normal_nodes[i] = new MidasNode();
            }
            int normal_nodes_count = 0;
            for (int i = 0; i < z_input_count + 1; i++)
            {
                for (int j = 0; j < x_input_count + 1; j++)
                {
                    for (int k = 0; k < y_input_count + 1; k++)
                    {
                        all_normal_nodes[normal_nodes_count].num = normal_nodes_count + 1;
                        all_normal_nodes[normal_nodes_count].x = x_points[j];
                        all_normal_nodes[normal_nodes_count].y = y_points[k];
                        all_normal_nodes[normal_nodes_count++].z = z_points[i];
                    }
                }
            }
            SetProcess(2);
            SetText("计算中...", this.status_bar_text);
            int regular_nodes_end = normal_nodes_count;

            MidasNode[] bottom_nodes = new MidasNode[5000];//扫地杆节点
            int bottom_nodes_start = normal_nodes_count + 1;
            for (int i = 0; i < bottom_nodes.Length; i++)
            {
                bottom_nodes[i] = new MidasNode();
            }
            int bottom_nodes_count = 0;
            for (int i = 0; i < x_points.Length; i++)//沿x方向y=0扫地杆节点
            {
                bottom_nodes[bottom_nodes_count].num = normal_nodes_count + 1;
                bottom_nodes[bottom_nodes_count].x = x_points[i];
                bottom_nodes[bottom_nodes_count].y = 0.0;
                bottom_nodes[bottom_nodes_count].z = h2;
                all_normal_nodes[normal_nodes_count++] = bottom_nodes[bottom_nodes_count].Copy();
                bottom_nodes_count++;
            }
            for (int i = 0; i < x_points.Length; i++)//沿x方向y=max扫地杆节点
            {
                bottom_nodes[bottom_nodes_count].num = normal_nodes_count + 1;
                bottom_nodes[bottom_nodes_count].x = x_points[i];
                bottom_nodes[bottom_nodes_count].y = y_length;
                bottom_nodes[bottom_nodes_count].z = h2;
                all_normal_nodes[normal_nodes_count++] = bottom_nodes[bottom_nodes_count].Copy();
                bottom_nodes_count++;
            }
            for (int i = 1; i < y_points.Length - 1; i++)//沿y方向x=0扫地杆节点
            {
                bottom_nodes[bottom_nodes_count].num = normal_nodes_count + 1;
                bottom_nodes[bottom_nodes_count].x = 0.0;
                bottom_nodes[bottom_nodes_count].y = y_points[i];
                bottom_nodes[bottom_nodes_count].z = h2;
                all_normal_nodes[normal_nodes_count++] = bottom_nodes[bottom_nodes_count].Copy();
                bottom_nodes_count++;
            }
            for (int i = 1; i < y_points.Length - 1; i++)//沿y方向x=max扫地杆节点
            {
                bottom_nodes[bottom_nodes_count].num = normal_nodes_count + 1;
                bottom_nodes[bottom_nodes_count].x = x_length;
                bottom_nodes[bottom_nodes_count].y = y_points[i];
                bottom_nodes[bottom_nodes_count].z = h2;
                all_normal_nodes[normal_nodes_count++] = bottom_nodes[bottom_nodes_count].Copy();
                bottom_nodes_count++;
            }
            bottom_nodes_start = bottom_nodes_start < normal_nodes_count ? bottom_nodes_start : normal_nodes_count;
            int bottom_nodes_end = normal_nodes_count;
            SetProcess(3);

            int all_elements_count = 0;//单元号计数器

            MidasElement[] y_elements = new MidasElement[40000];
            for (int i = 0; i < y_elements.Length; i++)
            {
                y_elements[i] = new MidasElement();
            }
            int y_elements_count = 0;

            MidasElement[] x_elements = new MidasElement[40000];
            for (int i = 0; i < x_elements.Length; i++)
            {
                x_elements[i] = new MidasElement();
            }
            int x_elements_count = 0;
            int z_top2_count = 2;
            if (h0 < 0.005)
                z_top2_count = 1;
            for (int i = 2; i < z_input_count + 1 - z_top2_count; i++)
            {
                for (int j = 0; j < y_input_count + 1; j++)
                {
                    for (int k = 0; k < x_input_count + 1; k++)
                    {
                        for (int l = 0; l < normal_nodes_count; l++)
                        {
                            if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005 && Math.Abs(all_normal_nodes[l].x - x_points[k]) < 0.005)
                            {
                                if (k == 0)
                                {
                                    x_elements[x_elements_count].fNode = all_normal_nodes[l].Copy();
                                }
                                else
                                {
                                    all_elements_count++;
                                    x_elements[x_elements_count].num = all_elements_count;
                                    x_elements[x_elements_count++].bNode = all_normal_nodes[l].Copy();
                                    x_elements[x_elements_count].fNode = all_normal_nodes[l].Copy();
                                }
                                break;
                            }
                        }
                    }
                }
            }
            SetProcess(4);

            MidasElement[] z_elements = new MidasElement[40000];
            for (int i = 0; i < z_elements.Length; i++)
            {
                z_elements[i] = new MidasElement();
            }
            int z_elements_count = 0;
            for (int i = 0; i < x_input_count + 1; i++)
            {
                for (int j = 0; j < y_input_count + 1; j++)
                {
                    for (int k = 0; k < z_input_count + 1; k++)
                    {
                        if (i == 0 || j == 0)
                        {
                            if (k == 2)//bottom_nodes
                            {
                                for (int l = 0; l < bottom_nodes_count; l++)
                                {
                                    if (Math.Abs(bottom_nodes[l].y - y_points[j]) < 0.005 && Math.Abs(bottom_nodes[l].x - x_points[i]) < 0.005)
                                    {
                                        //if (k == 0)
                                        //{
                                        //    z_elements[z_elements_count].fNode = all_normal_nodes[l];
                                        //}
                                        //else
                                        //{
                                        all_elements_count++;
                                        z_elements[z_elements_count].num = all_elements_count;
                                        z_elements[z_elements_count++].bNode = bottom_nodes[l].Copy();
                                        z_elements[z_elements_count].fNode = bottom_nodes[l].Copy();

                                        break;
                                    }
                                }
                            }
                        }
                        for (int l = 0; l < regular_nodes_end; l++)
                        {
                            if (Math.Abs(all_normal_nodes[l].z - z_points[k]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005 && Math.Abs(all_normal_nodes[l].x - x_points[i]) < 0.005)
                            {
                                if (k == 0)
                                {
                                    z_elements[z_elements_count].fNode = all_normal_nodes[l].Copy();
                                }
                                else
                                {
                                    all_elements_count++;
                                    z_elements[z_elements_count].num = all_elements_count;
                                    z_elements[z_elements_count++].bNode = all_normal_nodes[l].Copy();
                                    z_elements[z_elements_count].fNode = all_normal_nodes[l].Copy();
                                }
                                break;
                            }
                        }
                    }
                }
            }
            SetProcess(5);
            MidasElement[] bottom_elements = new MidasElement[5000];
            for (int i = 0; i < bottom_elements.Length; i++)
            {
                bottom_elements[i] = new MidasElement();
            }
            int bottom_elements_count = 0;

            for (int j = 0; j < x_input_count + 1; j++)
            {
                for (int k = 0; k < y_input_count + 1; k++)
                {
                    for (int l = 0; l < bottom_nodes_count; l++)
                    {
                        if (Math.Abs(bottom_nodes[l].z - h2) < 0.005 && Math.Abs(bottom_nodes[l].x - x_points[j]) < 0.005 && Math.Abs(bottom_nodes[l].y - y_points[k]) < 0.005)
                        {
                            if (k == 0)
                            {
                                bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
                            }
                            else
                            {
                                all_elements_count++;
                                bottom_elements[bottom_elements_count].num = all_elements_count;
                                bottom_elements[bottom_elements_count++].bNode = bottom_nodes[l].Copy();
                                bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
                            }
                            break;
                        }
                    }
                }
                j = j + x_input_count - 1;
            }
            for (int j = 0; j < y_input_count + 1; j++)
            {
                for (int k = 0; k < x_input_count + 1; k++)
                {
                    for (int l = 0; l < bottom_nodes_count; l++)
                    {
                        if (Math.Abs(bottom_nodes[l].z - h2) < 0.005 && Math.Abs(bottom_nodes[l].x - x_points[k]) < 0.005 && Math.Abs(bottom_nodes[l].y - y_points[j]) < 0.005)
                        {
                            if (k == 0)
                            {
                                bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
                            }
                            else
                            {
                                all_elements_count++;
                                bottom_elements[bottom_elements_count].num = all_elements_count;
                                bottom_elements[bottom_elements_count++].bNode = bottom_nodes[l].Copy();
                                bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
                            }
                            break;
                        }
                    }
                }
                j = j + y_input_count - 1;
            }
            SetProcess(10);

            all_elements_count = 0;//单元号计数器
            for (int i = 0; i < y_elements.Length; i++)
            {
                y_elements[i] = new MidasElement();
            }
            y_elements_count = 0;
            for (int i = 2; i < z_input_count - 1; i++)
            {
                for (int j = 0; j < x_input_count + 1; j++)
                {
                    MidasNode tempfnode = new MidasNode();
                    tempfnode.y = 1000;
                    MidasNode tempbnode = new MidasNode();
                    tempbnode.y = 1000;
                    for (int l = 0; l < normal_nodes_count; l++)
                    {
                        if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].x - x_points[j]) < 0.005)
                        {
                            if (tempfnode.y > all_normal_nodes[l].y)
                            {
                                tempfnode = all_normal_nodes[l].Copy();
                            }
                        }
                    }
                    while (true)
                    {
                        tempbnode.y = 1000;
                        for (int l = 0; l < normal_nodes_count; l++)
                        {
                            if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].x - x_points[j]) < 0.005 && all_normal_nodes[l].y - tempfnode.y > 0.005)
                            {
                                if (tempbnode.y > all_normal_nodes[l].y)
                                {
                                    tempbnode = all_normal_nodes[l].Copy();
                                }
                            }
                        }
                        if (tempbnode.y - 1000 > -0.005)
                        {
                            break;
                        }
                        all_elements_count++;
                        y_elements[y_elements_count].num = all_elements_count;
                        y_elements[y_elements_count].fNode = tempfnode.Copy();
                        y_elements[y_elements_count++].bNode = tempbnode.Copy();
                        tempfnode = tempbnode.Copy();
                    }
                }
            }
            SetProcess(20);

            //MidasElement[] x_elements = new MidasElement[20000];
            for (int i = 0; i < x_elements.Length; i++)
            {
                x_elements[i] = new MidasElement();
            }
            x_elements_count = 0;
            for (int i = 2; i < z_input_count - 1; i++)
            {
                for (int j = 0; j < y_input_count + 1; j++)
                {


                    MidasNode tempfnode = new MidasNode();
                    tempfnode.x = 1000;
                    MidasNode tempbnode = new MidasNode();
                    tempbnode.x = 1000;
                    for (int l = 0; l < normal_nodes_count; l++)
                    {
                        if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005)
                        {
                            if (tempfnode.x > all_normal_nodes[l].x)
                            {
                                tempfnode = all_normal_nodes[l].Copy();
                            }
                        }
                    }
                    while (true)
                    {
                        tempbnode.x = 1000;
                        for (int l = 0; l < normal_nodes_count; l++)
                        {
                            if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005 && all_normal_nodes[l].x - tempfnode.x > 0.005)
                            {
                                if (tempbnode.x > all_normal_nodes[l].x)
                                {
                                    tempbnode = all_normal_nodes[l].Copy();
                                }
                            }
                        }
                        if (tempbnode.x - 1000 > -0.005)
                        {
                            break;
                        }
                        all_elements_count++;
                        x_elements[x_elements_count].num = all_elements_count;
                        x_elements[x_elements_count].fNode = tempfnode.Copy();
                        x_elements[x_elements_count++].bNode = tempbnode.Copy();
                        tempfnode = tempbnode.Copy();
                    }
                }
            }
            SetProcess(30);

            //MidasElement[] z_elements = new MidasElement[20000];
            for (int i = 0; i < z_elements.Length; i++)
            {
                z_elements[i] = new MidasElement();
            }
            z_elements_count = 0;
            for (int i = 0; i < x_input_count + 1; i++)
            {
                for (int j = 0; j < y_input_count + 1; j++)
                {
                    MidasNode tempfnode = new MidasNode();
                    tempfnode.z = 1000;
                    MidasNode tempbnode = new MidasNode();
                    tempbnode.z = 1000;
                    for (int l = 0; l < normal_nodes_count; l++)
                    {
                        if (Math.Abs(all_normal_nodes[l].x - x_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005)
                        {
                            if (tempfnode.z > all_normal_nodes[l].z)
                            {
                                tempfnode = all_normal_nodes[l].Copy();
                            }
                        }
                    }
                    while (true)
                    {
                        tempbnode.z = 1000;
                        for (int l = 0; l < normal_nodes_count; l++)
                        {
                            if (Math.Abs(all_normal_nodes[l].x - x_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005 && all_normal_nodes[l].z - tempfnode.z > 0.005)
                            {
                                if (tempbnode.z > all_normal_nodes[l].z)
                                {
                                    tempbnode = all_normal_nodes[l].Copy();
                                }
                            }
                        }
                        if (tempbnode.z - 1000 > -0.005)
                        {
                            break;
                        }
                        all_elements_count++;
                        z_elements[z_elements_count].num = all_elements_count;
                        z_elements[z_elements_count].fNode = tempfnode.Copy();
                        z_elements[z_elements_count++].bNode = tempbnode.Copy();
                        tempfnode = tempbnode.Copy();
                    }
                }
            }
            SetProcess(40);


            //MidasElement[] bottom_elements = new MidasElement[1000];
            for (int i = 0; i < bottom_elements.Length; i++)
            {
                bottom_elements[i] = new MidasElement();
            }
            bottom_elements_count = 0;

            for (int j = 0; j < x_input_count + 1; j++)
            {
                for (int k = 0; k < y_input_count + 1; k++)
                {
                    for (int l = 0; l < bottom_nodes_count; l++)
                    {
                        if (Math.Abs(bottom_nodes[l].z - h2) < 0.005 && Math.Abs(bottom_nodes[l].x - x_points[j]) < 0.005 && Math.Abs(bottom_nodes[l].y - y_points[k]) < 0.005)
                        {
                            if (k == 0)
                            {
                                bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
                            }
                            else
                            {
                                all_elements_count++;
                                bottom_elements[bottom_elements_count].num = all_elements_count;
                                bottom_elements[bottom_elements_count++].bNode = bottom_nodes[l].Copy();
                                bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
                            }
                            break;
                        }
                    }
                }
                j = j + x_input_count - 1;
            }
            for (int j = 0; j < y_input_count + 1; j++)
            {
                for (int k = 0; k < x_input_count + 1; k++)
                {
                    for (int l = 0; l < bottom_nodes_count; l++)
                    {
                        if (Math.Abs(bottom_nodes[l].z - h2) < 0.005 && Math.Abs(bottom_nodes[l].x - x_points[k]) < 0.005 && Math.Abs(bottom_nodes[l].y - y_points[j]) < 0.005)
                        {
                            if (k == 0)
                            {
                                bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
                            }
                            else
                            {
                                all_elements_count++;
                                bottom_elements[bottom_elements_count].num = all_elements_count;
                                bottom_elements[bottom_elements_count++].bNode = bottom_nodes[l].Copy();
                                bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
                            }
                            break;
                        }
                    }
                }
                j = j + y_input_count - 1;
            }
            SetProcess(50);
            int bottom_elements_end = all_elements_count;


            # region 剪刀撑计算
            /////////////////////////////////////////////////////////////////////////
            int xy_bridging_lines_count = 0;
            int axis_xy_bridging_lines_num = 0;
            int xz_bridging_lines_count = 0;
            int axis_xz_bridging_lines_num = 0;
            int yz_bridging_lines_count = 0;
            int axis_yz_bridging_lines_num = 0;
            MidasElement[] xy_bridging_lines = null;
            MidasElement[] xz_bridging_lines = null;
            MidasElement[] yz_bridging_lines = null;
            int xy_bridging_nodes_end = 0;
            int yz_bridging_nodes_end = 0;
            int xz_bridging_nodes_end = 0;
            int[] xy_bridging_repeat_nodenum = null;
            int xy_bridging_nodes_start = 0;
            int xy_bridging_repeat_nodenum_count = 0;
            int[] xz_bridging_repeat_nodenum = null;
            int xz_bridging_nodes_start = 0;
            int xz_bridging_repeat_nodenum_count = 0;
            int[] yz_bridging_repeat_nodenum = null;
            int yz_bridging_nodes_start = 0;
            int yz_bridging_repeat_nodenum_count = 0;
            //MidasElement[] _xz_bridging_elements = new MidasElement[2000];
            //int _xz_bridging_elements_count = 0;


            MidasElement[] xy_bridging_elements = new MidasElement[20000];
            int xy_bridging_elements_count = 0;
            int xy_bridging_elements_start = 0;
            int xy_bridging_elements_end = 0;

            MidasElement[] xz_bridging_elements = new MidasElement[20000];
            int xz_bridging_elements_count = 0;
            int xz_bridging_elements_start = 0;
            int xz_bridging_elements_end = 0;

            MidasElement[] yz_bridging_elements = new MidasElement[20000];
            int yz_bridging_elements_count = 0;
            int yz_bridging_elements_start = 0;
            int yz_bridging_elements_end = 0;

            if (bridging_check)
            {
                xy_bridging_lines = new MidasElement[5000];//单层xy剪刀线
                for (int i = 0; i < xy_bridging_lines.Length; i++)
                {
                    xy_bridging_lines[i] = new MidasElement();
                }
                for (int i = 0; i <= (int)(x_length / l2 - 0.3); i++)//xy面正向中线下
                {
                    xy_bridging_lines[xy_bridging_lines_count].fNode.x = i * l2;
                    xy_bridging_lines[xy_bridging_lines_count].fNode.y = 0.0;
                    xy_bridging_lines[xy_bridging_lines_count].bNode.x = xy_bridging_lines[xy_bridging_lines_count].fNode.x + (y_length / tan_alpha < x_length - xy_bridging_lines[xy_bridging_lines_count].fNode.x ? y_length / tan_alpha : x_length - xy_bridging_lines[xy_bridging_lines_count].fNode.x);
                    xy_bridging_lines[xy_bridging_lines_count].bNode.y = y_length < (x_length - xy_bridging_lines[xy_bridging_lines_count].fNode.x) * tan_alpha ? y_length : (x_length - xy_bridging_lines[xy_bridging_lines_count].fNode.x) * tan_alpha;
                    xy_bridging_lines_count++;
                }
                for (int i = 1; i <= (int)(y_length / l2 - 0.3); i++)//xy面正向中线上
                {
                    xy_bridging_lines[xy_bridging_lines_count].fNode.x = 0.0;
                    xy_bridging_lines[xy_bridging_lines_count].fNode.y = i * l2;
                    xy_bridging_lines[xy_bridging_lines_count].bNode.x = (x_length < (y_length - xy_bridging_lines[xy_bridging_lines_count].fNode.y) / tan_alpha) ? x_length : (y_length - xy_bridging_lines[xy_bridging_lines_count].fNode.y) / tan_alpha;
                    xy_bridging_lines[xy_bridging_lines_count].bNode.y = xy_bridging_lines[xy_bridging_lines_count].fNode.y + ((y_length - xy_bridging_lines[xy_bridging_lines_count].fNode.y) < x_length * tan_alpha ? (y_length - xy_bridging_lines[xy_bridging_lines_count].fNode.y) : x_length * tan_alpha);
                    xy_bridging_lines_count++;
                }
                axis_xy_bridging_lines_num = xy_bridging_lines_count;
                for (int i = 0; i < xy_bridging_lines_count; i++)
                {
                    xy_bridging_lines[xy_bridging_lines_count + i] = xy_bridging_lines[i].Copy();
                    xy_bridging_lines[xy_bridging_lines_count + i].bNode.x = x_length - xy_bridging_lines[i].bNode.x;
                    xy_bridging_lines[xy_bridging_lines_count + i].fNode.x = x_length - xy_bridging_lines[i].fNode.x;
                }
                xy_bridging_lines_count = 2 * xy_bridging_lines_count;
                SetProcess(55);

                xz_bridging_lines = new MidasElement[5000];//单层xz剪刀线
                xz_bridging_lines_count = 0;
                for (int i = 0; i < xz_bridging_lines.Length; i++)
                {
                    xz_bridging_lines[i] = new MidasElement();
                }
                //double actual_start_point = z_length - 0.3 - h3 - h0 - h1;
                for (int i = 0; i <= (int)(x_length / l2 - 0.3); i++)//xy面正向中线下
                {
                    xz_bridging_lines[xz_bridging_lines_count].fNode.x = i * l2;
                    xz_bridging_lines[xz_bridging_lines_count].fNode.z = h3 + 0.3;
                    xz_bridging_lines[xz_bridging_lines_count].bNode.x = xz_bridging_lines[xz_bridging_lines_count].fNode.x + ((z_length - 0.3 - h3) / tan_alpha < x_length - xz_bridging_lines[xz_bridging_lines_count].fNode.x ? (z_length - 0.3 - h3) / tan_alpha : x_length - xz_bridging_lines[xz_bridging_lines_count].fNode.x);
                    xz_bridging_lines[xz_bridging_lines_count].bNode.z = h3 + 0.3 + ((z_length - 0.3 - h3) < (x_length - xz_bridging_lines[xz_bridging_lines_count].fNode.x) * tan_alpha ? (z_length - 0.3 - h3) : (x_length - xz_bridging_lines[xz_bridging_lines_count].fNode.x) * tan_alpha);
                    xz_bridging_lines_count++;
                }
                for (int i = 1; i <= (int)((z_length - 0.3 - h3) / l2 - 0.3); i++)//xy面正向中线上
                {
                    xz_bridging_lines[xz_bridging_lines_count].fNode.x = 0.0;
                    xz_bridging_lines[xz_bridging_lines_count].fNode.z = i * l2;
                    xz_bridging_lines[xz_bridging_lines_count].bNode.x = (x_length < ((z_length) - xz_bridging_lines[xz_bridging_lines_count].fNode.z) / tan_alpha) ? x_length : ((z_length) - xz_bridging_lines[xz_bridging_lines_count].fNode.z) / tan_alpha;
                    xz_bridging_lines[xz_bridging_lines_count].bNode.z = xz_bridging_lines[xz_bridging_lines_count].fNode.z + (((z_length) - xz_bridging_lines[xz_bridging_lines_count].fNode.z) < x_length * tan_alpha ? ((z_length) - xz_bridging_lines[xz_bridging_lines_count].fNode.z) : x_length * tan_alpha);
                    xz_bridging_lines_count++;
                }
                axis_xz_bridging_lines_num = xz_bridging_lines_count;
                for (int i = 0; i < xz_bridging_lines_count; i++)
                {
                    xz_bridging_lines[xz_bridging_lines_count + i] = xz_bridging_lines[i].Copy();
                    xz_bridging_lines[xz_bridging_lines_count + i].bNode.x = x_length - xz_bridging_lines[i].bNode.x;
                    xz_bridging_lines[xz_bridging_lines_count + i].fNode.x = x_length - xz_bridging_lines[i].fNode.x;
                }
                xz_bridging_lines_count = 2 * xz_bridging_lines_count;
                SetProcess(60);

                yz_bridging_lines = new MidasElement[5000];//单层yz剪刀线
                yz_bridging_lines_count = 0;
                for (int i = 0; i < yz_bridging_lines.Length; i++)
                {
                    yz_bridging_lines[i] = new MidasElement();
                }
                //double actual_start_point = z_length - 0.3 - h3 - h0 - h1;
                for (int i = 0; i <= (int)(y_length / l2 - 0.3); i++)//xy面正向中线下
                {
                    yz_bridging_lines[yz_bridging_lines_count].fNode.y = i * l2;
                    yz_bridging_lines[yz_bridging_lines_count].fNode.z = h3 + 0.3;
                    yz_bridging_lines[yz_bridging_lines_count].bNode.y = yz_bridging_lines[yz_bridging_lines_count].fNode.y + (((z_length - 0.3 - h3) / tan_alpha) < y_length - yz_bridging_lines[yz_bridging_lines_count].fNode.y ? ((z_length - 0.3 - h3) / tan_alpha) : y_length - yz_bridging_lines[yz_bridging_lines_count].fNode.y);
                    yz_bridging_lines[yz_bridging_lines_count].bNode.z = h3 + 0.3 + ((z_length - 0.3 - h3) < (y_length - yz_bridging_lines[yz_bridging_lines_count].fNode.y) * tan_alpha ? (z_length - 0.3 - h3) : (y_length - yz_bridging_lines[yz_bridging_lines_count].fNode.y) * tan_alpha);
                    yz_bridging_lines_count++;
                }
                for (int i = 1; i <= (int)((z_length - 0.3 - h3) / l2 - 0.3); i++)//xy面正向中线上
                {
                    yz_bridging_lines[yz_bridging_lines_count].fNode.y = 0.0;
                    yz_bridging_lines[yz_bridging_lines_count].fNode.z = i * l2;
                    yz_bridging_lines[yz_bridging_lines_count].bNode.y = (y_length < ((z_length) - yz_bridging_lines[yz_bridging_lines_count].fNode.z) / tan_alpha) ? y_length : ((z_length) - yz_bridging_lines[yz_bridging_lines_count].fNode.z) / tan_alpha;
                    yz_bridging_lines[yz_bridging_lines_count].bNode.z = yz_bridging_lines[yz_bridging_lines_count].fNode.z + (((z_length) - yz_bridging_lines[yz_bridging_lines_count].fNode.z) < y_length * tan_alpha ? ((z_length) - yz_bridging_lines[yz_bridging_lines_count].fNode.z) : y_length * tan_alpha);
                    yz_bridging_lines_count++;
                }
                axis_yz_bridging_lines_num = yz_bridging_lines_count;
                for (int i = 0; i < yz_bridging_lines_count; i++)
                {
                    yz_bridging_lines[yz_bridging_lines_count + i] = yz_bridging_lines[i].Copy();
                    yz_bridging_lines[yz_bridging_lines_count + i].bNode.y = y_length - yz_bridging_lines[i].bNode.y;
                    yz_bridging_lines[yz_bridging_lines_count + i].fNode.y = y_length - yz_bridging_lines[i].fNode.y;
                }
                yz_bridging_lines_count = 2 * yz_bridging_lines_count;
                SetProcess(65);

                //MidasNode[] xy_bridging_nodes = new MidasNode[5000];//xy面剪刀撑节点，包含与前部分节点重复的节点
                xy_bridging_elements_start = all_elements_count + 1;
                xy_bridging_elements_end = all_elements_count;
                for (int i = 0; i < xy_bridging_elements.Length; i++)
                {
                    xy_bridging_elements[i] = new MidasElement();
                }
                xy_bridging_repeat_nodenum = new int[20000];//xy面剪刀撑节点号与前部分节点重复的节点号码
                xy_bridging_nodes_start = normal_nodes_count + 1;
                xy_bridging_nodes_end = normal_nodes_count;
                xy_bridging_repeat_nodenum_count = 0;
                //for (int i = 0; i < xy_bridging_nodes.Length; i++)
                //{
                //    xy_bridging_nodes[i] = new MidasNode();
                //}
                for (int k = 0; k < z_bridging_count; k++)
                {
                    for (int i = 0; i < xy_bridging_lines_count; i++)
                    {
                        MidasNode[] singleLineNodes = new MidasNode[1000];
                        int singleLineNodes_count = 0;
                        for (int j = 0; j < singleLineNodes.Length; j++)
                        {
                            singleLineNodes[j] = new MidasNode();
                        }
                        xy_bridging_lines[i].fNode.z = z_bridging_points[k];
                        xy_bridging_lines[i].bNode.z = z_bridging_points[k];
                        for (int j = 0; j < x_elements_count; j++)
                        {
                            //if (z_elements[j].fNode.z - z_points[2] < -0.005 && z_elements[j].bNode.z - z_points[2] < -0.005)
                            //    continue;
                            MidasNode temp_point = get_cross_point(xy_bridging_lines[i], x_elements[j]);
                            if (null != temp_point)
                            {
                                bool flag = false;
                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - temp_point.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - temp_point.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - temp_point.z) < 0.005)
                                    {
                                        bool inner_flag = false;
                                        for (int m = 0; m < xy_bridging_repeat_nodenum_count; m++)
                                        {
                                            if (xy_bridging_repeat_nodenum[m] == all_normal_nodes[l].num)
                                            {
                                                inner_flag = true;
                                                break;
                                            }
                                        }
                                        if (inner_flag == false)
                                        {
                                            //xy_bridging_repeat_nodenum[xy_bridging_repeat_nodenum_count++] = all_normal_nodes[l].num;
                                            singleLineNodes[singleLineNodes_count++] = all_normal_nodes[l].Copy();
                                        }
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag == false)
                                {
                                    temp_point.num = normal_nodes_count + 1;
                                    //all_normal_nodes[normal_nodes_count++] = temp_point.Copy();
                                    singleLineNodes[singleLineNodes_count++] = temp_point.Copy();
                                }
                            }
                        }
                        //至此已找到该线上所有交点，设计算法来找到每间隔H处的点的最近正规节点

                        for (int j = 0; j < singleLineNodes_count; j++)//此处应当先行排序，以便于后续对比z方向间距集合，睡觉啦先。。
                        {
                            MidasNode midval = new MidasNode();
                            for (int l = j; l < singleLineNodes_count; l++)
                            {
                                if (singleLineNodes[l].x < singleLineNodes[j].x)
                                {
                                    midval = singleLineNodes[j].Copy();
                                    singleLineNodes[j] = singleLineNodes[l].Copy();
                                    singleLineNodes[l] = midval.Copy();
                                }
                            }

                        }
                        MidasNode fNode = new MidasNode();
                        MidasNode bNode = new MidasNode();
                        double H_step = 4;
                        double line_length = Math.Abs(singleLineNodes[0].x - singleLineNodes[singleLineNodes_count - 1].x);
                        //Console.WriteLine(String.Format("count = {0}", singleLineNodes_count));
                        int step_count = (int)Math.Round(line_length / H_step);
                        if (step_count == 0)
                            H_step = line_length;
                        else
                            H_step = line_length / step_count;
                        int single_elements_count = 0;
                        for (int j = 0; j < singleLineNodes_count; j++)
                        {

                            if (j == 0)
                            {
                                fNode = singleLineNodes[0].Copy();

                                for (int l = 0; l < x_input_count; l++)
                                {
                                    if (fNode.x >= x_points[l] && fNode.x <= x_points[l + 1])
                                    {
                                        fNode.x = Math.Abs(fNode.x - x_points[l]) < Math.Abs(fNode.x - x_points[l + 1]) ? x_points[l] : x_points[l + 1];
                                        //if (l < 2) 
                                        //    fNode.z = z_points[2];
                                        if (l == 0)
                                            fNode.x = x_points[0];
                                        break;
                                    }
                                }

                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - fNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - fNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - fNode.z) < 0.005)
                                    {
                                        fNode.num = all_normal_nodes[l].num;
                                        break;
                                    }
                                }
                                //all_normal_nodes[normal_nodes_count++] = fNode.Copy();
                            }
                            if (Math.Abs(singleLineNodes[j].x - fNode.x) - H_step > -0.005)
                            {
                                //all_normal_nodes[normal_nodes_count++] = singleLineNodes[j].Copy();
                                bNode = singleLineNodes[j].Copy();
                                for (int l = 0; l < x_input_count; l++)
                                {
                                    if (bNode.x >= x_points[l] && bNode.x <= x_points[l + 1])
                                    {
                                        bNode.x = Math.Abs(bNode.x - x_points[l]) < Math.Abs(bNode.x - x_points[l + 1]) ? x_points[l] : x_points[l + 1];
                                        if (l == x_input_count - 1)
                                            bNode.x = x_points[x_input_count];
                                        if (l == 1)
                                            bNode.x = x_points[0];
                                        break;
                                    }
                                }
                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                    {
                                        bNode.num = all_normal_nodes[l].num;
                                        break;
                                    }
                                }
                                if (Math.Abs(bNode.x - fNode.x) > 2.5)
                                {
                                    //Console.WriteLine(String.Format("fnode.x = {0},bnode.x = {1},gap = {2}", fNode.x, bNode.x, bNode.x - fNode.x));
                                    xy_bridging_elements[xy_bridging_elements_count].fNode = fNode.Copy();
                                    xy_bridging_elements[xy_bridging_elements_count].num = ++all_elements_count;
                                    xy_bridging_elements[xy_bridging_elements_count++].bNode = bNode.Copy();
                                    single_elements_count++;

                                    fNode = bNode.Copy();
                                }
                            }

                            if (j == singleLineNodes_count - 1 && singleLineNodes_count != 1)
                            {
                                if (single_elements_count >= 1)
                                {
                                    if (Math.Abs(xy_bridging_elements[xy_bridging_elements_count - 1].bNode.x - singleLineNodes[singleLineNodes_count - 1].x) > 0.005)
                                    {
                                        if (bNode.num != 0)
                                            fNode = bNode.Copy();

                                        bNode = singleLineNodes[singleLineNodes_count - 1].Copy();
                                        for (int l = 0; l < x_input_count; l++)
                                        {
                                            if (bNode.x >= x_points[l] && bNode.x <= x_points[l + 1])
                                            {
                                                bNode.x = Math.Abs(bNode.x - x_points[l]) < Math.Abs(bNode.x - x_points[l + 1]) ? x_points[l] : x_points[l + 1];
                                                //if (l < 2) 
                                                //    bNode.z = z_points[2];
                                                if (l == x_input_count - 1)
                                                    bNode.x = x_points[x_input_count];

                                                break;
                                            }
                                        }

                                        for (int l = 0; l < normal_nodes_count; l++)
                                        {
                                            if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                            {
                                                bNode.num = all_normal_nodes[l].num;
                                                break;
                                            }
                                        }
                                        if (Math.Abs(bNode.x - fNode.x) > 2.5)
                                        {
                                            xy_bridging_elements[xy_bridging_elements_count].fNode = fNode.Copy();
                                            xy_bridging_elements[xy_bridging_elements_count].num = ++all_elements_count;
                                            xy_bridging_elements[xy_bridging_elements_count++].bNode = bNode.Copy();
                                            single_elements_count++;
                                            fNode = bNode.Copy();
                                        }
                                        else
                                        {
                                            xy_bridging_elements[xy_bridging_elements_count - 1].bNode = bNode.Copy();
                                        }
                                    }

                                }
                                else
                                {
                                    bNode = singleLineNodes[singleLineNodes_count - 1].Copy();
                                    for (int l = 0; l < x_input_count; l++)
                                    {
                                        if (bNode.x >= x_points[l] && bNode.x <= x_points[l + 1])
                                        {
                                            bNode.x = Math.Abs(bNode.x - x_points[l]) < Math.Abs(bNode.x - x_points[l + 1]) ? x_points[l] : x_points[l + 1];
                                            //if (l < 2) 
                                            //    bNode.z = z_points[2];
                                            if (l == x_input_count - 1)
                                                bNode.x = x_points[x_input_count];

                                            break;
                                        }
                                    }

                                    for (int l = 0; l < normal_nodes_count; l++)
                                    {
                                        if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                        {
                                            bNode.num = all_normal_nodes[l].num;
                                            break;
                                        }
                                    }
                                    if (Math.Abs(bNode.x - fNode.x) > 2.5)
                                    {
                                        xy_bridging_elements[xy_bridging_elements_count].fNode = fNode.Copy();
                                        xy_bridging_elements[xy_bridging_elements_count].num = ++all_elements_count;
                                        xy_bridging_elements[xy_bridging_elements_count++].bNode = bNode.Copy();
                                        single_elements_count++;
                                        fNode = bNode.Copy();
                                    }
                                    //xy_bridging_elements[xy_bridging_elements_count - 1].bNode = bNode.Copy();
                                }
                            }
                        }
                    }
                }
                xy_bridging_nodes_start = xy_bridging_nodes_start < normal_nodes_count ? xy_bridging_nodes_start : normal_nodes_count;
                xy_bridging_nodes_end = normal_nodes_count;

                xy_bridging_elements_end = all_elements_count;
                xy_bridging_elements_start = xy_bridging_elements_start < xy_bridging_elements_end ? xy_bridging_elements_start : xy_bridging_elements_end;
                SetProcess(70);

                //MidasNode[] xz_bridging_nodes = new MidasNode[5000];//xz面剪刀撑节点
                //int xz_bridging_nodes_count = 0;
                for (int i = 0; i < xz_bridging_elements.Length; i++)
                {
                    xz_bridging_elements[i] = new MidasElement();
                }
                xz_bridging_elements_start = all_elements_count + 1;
                xz_bridging_elements_end = all_elements_count;

                xz_bridging_repeat_nodenum = new int[20000];//xz面剪刀撑节点号与前部分节点重复的节点号码
                xz_bridging_nodes_start = normal_nodes_count + 1;
                xz_bridging_nodes_end = normal_nodes_count;
                xz_bridging_repeat_nodenum_count = 0;
                //for (int i = 0; i < xz_bridging_nodes.Length; i++)
                //{
                //    xz_bridging_nodes[i] = new MidasNode();
                //}
                for (int k = 0; k < y_bridging_count; k++)
                {
                    for (int i = 0; i < xz_bridging_lines_count; i++)
                    {
                        MidasNode[] singleLineNodes = new MidasNode[1000];
                        int singleLineNodes_count = 0;
                        for (int j = 0; j < singleLineNodes.Length; j++)
                        {
                            singleLineNodes[j] = new MidasNode();
                        }
                        xz_bridging_lines[i].fNode.y = y_bridging_points[k];
                        xz_bridging_lines[i].bNode.y = y_bridging_points[k];
                        for (int j = 0; j < z_elements_count; j++)
                        {
                            if (z_elements[j].fNode.z - z_points[2] < -0.005 && z_elements[j].bNode.z - z_points[2] < -0.005)
                                continue;
                            MidasNode temp_point = get_cross_point(xz_bridging_lines[i], z_elements[j]);
                            if (null != temp_point)
                            {
                                bool flag = false;
                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - temp_point.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - temp_point.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - temp_point.z) < 0.005)
                                    {

                                        bool inner_flag = false;
                                        for (int m = 0; m < xz_bridging_repeat_nodenum_count; m++)
                                        {
                                            if (xz_bridging_repeat_nodenum[m] == all_normal_nodes[l].num)
                                            {
                                                inner_flag = true;
                                                break;
                                            }
                                        }
                                        if (inner_flag == false)
                                        {
                                            //xz_bridging_repeat_nodenum[xz_bridging_repeat_nodenum_count++] = all_normal_nodes[l].num;
                                            singleLineNodes[singleLineNodes_count++] = all_normal_nodes[l].Copy();
                                        }
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag == false)
                                {
                                    temp_point.num = normal_nodes_count + 1;
                                    //all_normal_nodes[normal_nodes_count++] = temp_point.Copy();
                                    singleLineNodes[singleLineNodes_count++] = temp_point.Copy();
                                }
                            }
                        }
                        //至此已找到该线上所有交点，设计算法来找到每间隔H处的点的最近正规节点

                        for (int j = 0; j < singleLineNodes_count; j++)//此处应当先行排序，以便于后续对比z方向间距集合，睡觉啦先。。
                        {
                            MidasNode midval = new MidasNode();
                            for (int l = j; l < singleLineNodes_count; l++)
                            {
                                if (singleLineNodes[l].z < singleLineNodes[j].z)
                                {
                                    midval = singleLineNodes[j].Copy();
                                    singleLineNodes[j] = singleLineNodes[l].Copy();
                                    singleLineNodes[l] = midval.Copy();
                                }
                            }

                        }
                        MidasNode fNode = new MidasNode();
                        MidasNode bNode = new MidasNode();
                        double H_step = 4;
                        double line_length = Math.Abs(singleLineNodes[0].z - singleLineNodes[singleLineNodes_count - 1].z);
                        //Console.WriteLine(String.Format("count = {0}", singleLineNodes_count));
                        int step_count = (int)Math.Round(line_length / H_step);
                        if (step_count == 0)
                            H_step = line_length;
                        else
                            H_step = line_length / step_count;
                        int single_elements_count = 0;
                        for (int j = 0; j < singleLineNodes_count; j++)
                        {

                            if (j == 0)
                            {
                                fNode = singleLineNodes[0].Copy();

                                for (int l = 0; l < z_input_count; l++)
                                {
                                    if (fNode.z >= z_points[l] && fNode.z <= z_points[l + 1])
                                    {
                                        fNode.z = Math.Abs(fNode.z - z_points[l]) < Math.Abs(fNode.z - z_points[l + 1]) ? z_points[l] : z_points[l + 1];
                                        //if (l < 2)
                                        //    fNode.z = z_points[2];
                                        if (l <= 3)
                                            fNode.z = z_points[2];
                                        break;
                                    }
                                }

                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - fNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - fNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - fNode.z) < 0.005)
                                    {
                                        fNode.num = all_normal_nodes[l].num;
                                        break;
                                    }
                                }
                                //all_normal_nodes[normal_nodes_count++] = fNode.Copy();
                            }
                            if (Math.Abs(singleLineNodes[j].z - fNode.z) - H_step > -0.005)
                            {
                                //all_normal_nodes[normal_nodes_count++] = singleLineNodes[j].Copy();
                                bNode = singleLineNodes[j].Copy();
                                for (int l = 0; l < z_input_count; l++)
                                {
                                    if (bNode.z >= z_points[l] && bNode.z <= z_points[l + 1])
                                    {
                                        bNode.z = Math.Abs(bNode.z - z_points[l]) < Math.Abs(bNode.z - z_points[l + 1]) ? z_points[l] : z_points[l + 1];
                                        if (h0 > 0.005)
                                        {
                                            if (l >= z_input_count - 3)
                                                bNode.z = z_points[z_input_count - 2];
                                        }
                                        else
                                        {
                                            if (l >= z_input_count - 2)
                                                bNode.z = z_points[z_input_count - 1];
                                        }
                                        break;
                                    }
                                }
                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                    {
                                        bNode.num = all_normal_nodes[l].num;
                                        break;
                                    }
                                }
                                if (Math.Abs(bNode.z - fNode.z) > 2.5)
                                {
                                    //Console.WriteLine(String.Format("fnode.z = {0},bnode.z = {1},gap = {2}", fNode.z, bNode.z, bNode.z - fNode.z));
                                    xz_bridging_elements[xz_bridging_elements_count].fNode = fNode.Copy();
                                    xz_bridging_elements[xz_bridging_elements_count].num = ++all_elements_count;
                                    xz_bridging_elements[xz_bridging_elements_count++].bNode = bNode.Copy();
                                    single_elements_count++;
                                    fNode = bNode.Copy();
                                }
                            }

                            if (j == singleLineNodes_count - 1 && singleLineNodes_count != 1)
                            {
                                if (single_elements_count >= 1)
                                {

                                    if (Math.Abs(xz_bridging_elements[xz_bridging_elements_count - 1].bNode.z - singleLineNodes[singleLineNodes_count - 1].z) > 0.005)
                                    {

                                        if (bNode.num != 0)
                                            fNode = bNode.Copy();
                                        //fNode = bNode.Copy();
                                        bNode = singleLineNodes[singleLineNodes_count - 1].Copy();
                                        for (int l = 0; l < z_input_count; l++)
                                        {
                                            if (bNode.z >= z_points[l] && bNode.z <= z_points[l + 1])
                                            {
                                                bNode.z = Math.Abs(bNode.z - z_points[l]) < Math.Abs(bNode.z - z_points[l + 1]) ? z_points[l] : z_points[l + 1];
                                                if (h0 > 0.005)
                                                {
                                                    //if (l > z_input_count - 2)
                                                    //    bNode.z = z_points[z_input_count - 2];
                                                    if (l >= z_input_count - 3)
                                                        bNode.z = z_points[z_input_count - 2];
                                                }
                                                else
                                                {
                                                    //if (l > z_input_count - 1)
                                                    //    bNode.z = z_points[z_input_count - 1];
                                                    if (l >= z_input_count - 2)
                                                        bNode.z = z_points[z_input_count - 1];
                                                }
                                                break;
                                            }
                                        }
                                        for (int l = 0; l < normal_nodes_count; l++)
                                        {
                                            if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                            {
                                                bNode.num = all_normal_nodes[l].num;
                                                break;
                                            }
                                        }
                                        if (Math.Abs(fNode.z - bNode.z) > 2.5)
                                        {
                                            //Console.WriteLine(String.Format("fnode.z = {0},bnode.z = {1},gap = {2}", fNode.z, bNode.z, bNode.z - fNode.z));
                                            xz_bridging_elements[xz_bridging_elements_count].fNode = fNode.Copy();
                                            xz_bridging_elements[xz_bridging_elements_count].num = ++all_elements_count;
                                            xz_bridging_elements[xz_bridging_elements_count++].bNode = bNode.Copy();
                                            single_elements_count++;
                                        }
                                        else
                                        {
                                            xz_bridging_elements[xz_bridging_elements_count - 1].bNode = bNode.Copy();
                                        }
                                    }

                                }
                                else
                                {
                                    bNode = singleLineNodes[singleLineNodes_count - 1].Copy();

                                    for (int l = 0; l < z_input_count; l++)
                                    {
                                        if (bNode.z >= z_points[l] && bNode.z <= z_points[l + 1])
                                        {
                                            bNode.z = Math.Abs(bNode.z - z_points[l]) < Math.Abs(bNode.z - z_points[l + 1]) ? z_points[l] : z_points[l + 1];
                                            if (h0 > 0.005)
                                            {
                                                //if (l > z_input_count - 2)
                                                //    bNode.z = z_points[z_input_count - 2];
                                                if (l >= z_input_count - 3)
                                                    bNode.z = z_points[z_input_count - 2];
                                            }
                                            else
                                            {
                                                //if (l > z_input_count - 1)
                                                //    bNode.z = z_points[z_input_count - 1];
                                                if (l >= z_input_count - 2)
                                                    bNode.z = z_points[z_input_count - 1];
                                            }
                                            break;
                                        }
                                    }

                                    for (int l = 0; l < normal_nodes_count; l++)
                                    {
                                        if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                        {
                                            bNode.num = all_normal_nodes[l].num;
                                            break;
                                        }
                                    }
                                    if (Math.Abs(fNode.z - bNode.z) > 2.5)
                                    {
                                        //Console.WriteLine(String.Format("fnode.z = {0},bnode.z = {1},gap = {2}", fNode.z, bNode.z, bNode.z - fNode.z));
                                        xz_bridging_elements[xz_bridging_elements_count].fNode = fNode.Copy();
                                        xz_bridging_elements[xz_bridging_elements_count].num = ++all_elements_count;
                                        xz_bridging_elements[xz_bridging_elements_count++].bNode = bNode.Copy();
                                        single_elements_count++;
                                    }
                                }
                            }
                        }
                    }
                }
                xz_bridging_nodes_start = xz_bridging_nodes_start < normal_nodes_count ? xz_bridging_nodes_start : normal_nodes_count;
                xz_bridging_nodes_end = normal_nodes_count;

                xz_bridging_elements_end = all_elements_count;
                xz_bridging_elements_start = xz_bridging_elements_start < xz_bridging_elements_end ? xz_bridging_elements_start : xz_bridging_elements_end;

                SetProcess(75);

                //MidasNode[] yz_bridging_nodes = new MidasNode[5000];//yz面剪刀撑节点
                for (int i = 0; i < yz_bridging_elements.Length; i++)
                {
                    yz_bridging_elements[i] = new MidasElement();
                }

                yz_bridging_elements_start = all_elements_count + 1;
                yz_bridging_elements_end = all_elements_count;

                //int yz_bridging_nodes_count = 0;
                yz_bridging_repeat_nodenum = new int[20000];//yz面剪刀撑节点号与前部分节点重复的节点号码
                yz_bridging_nodes_start = normal_nodes_count + 1;
                yz_bridging_nodes_end = normal_nodes_count;
                yz_bridging_repeat_nodenum_count = 0;
                //for (int i = 0; i < yz_bridging_nodes.Length; i++)
                //{
                //    yz_bridging_nodes[i] = new MidasNode();
                //}
                for (int k = 0; k < x_bridging_count; k++)
                {
                    for (int i = 0; i < yz_bridging_lines_count; i++)
                    {
                        MidasNode[] singleLineNodes = new MidasNode[1000];
                        int singleLineNodes_count = 0;
                        for (int j = 0; j < singleLineNodes.Length; j++)
                        {
                            singleLineNodes[j] = new MidasNode();
                        }
                        yz_bridging_lines[i].fNode.x = x_bridging_points[k];
                        yz_bridging_lines[i].bNode.x = x_bridging_points[k];
                        for (int j = 0; j < z_elements_count; j++)
                        {
                            if (z_elements[j].fNode.z - z_points[2] < -0.005 && z_elements[j].bNode.z - z_points[2] < -0.005)
                                continue;
                            MidasNode temp_point = get_cross_point(yz_bridging_lines[i], z_elements[j]);
                            if (null != temp_point)
                            {
                                bool flag = false;
                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - temp_point.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - temp_point.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - temp_point.z) < 0.005)
                                    {
                                        bool inner_flag = false;
                                        for (int m = 0; m < yz_bridging_repeat_nodenum_count; m++)
                                        {
                                            if (yz_bridging_repeat_nodenum[m] == all_normal_nodes[l].num)
                                            {
                                                inner_flag = true;
                                                break;
                                            }
                                        }
                                        if (inner_flag == false)
                                        {
                                            //yz_bridging_repeat_nodenum[yz_bridging_repeat_nodenum_count++] = all_normal_nodes[l].num;
                                            singleLineNodes[singleLineNodes_count++] = all_normal_nodes[l].Copy();
                                        }
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag == false)
                                {
                                    temp_point.num = normal_nodes_count + 1;
                                    //all_normal_nodes[normal_nodes_count++] = temp_point.Copy();
                                    singleLineNodes[singleLineNodes_count++] = temp_point.Copy();
                                }
                            }
                        }
                        //至此已找到该线上所有交点，设计算法来找到每间隔H处的点的最近正规节点

                        for (int j = 0; j < singleLineNodes_count; j++)//此处应当先行排序，以便于后续对比z方向间距集合，睡觉啦先。。
                        {
                            MidasNode midval = new MidasNode();
                            for (int l = j; l < singleLineNodes_count; l++)
                            {
                                if (singleLineNodes[l].z < singleLineNodes[j].z)
                                {
                                    midval = singleLineNodes[j].Copy();
                                    singleLineNodes[j] = singleLineNodes[l].Copy();
                                    singleLineNodes[l] = midval.Copy();
                                }
                            }

                        }
                        MidasNode fNode = new MidasNode();
                        MidasNode bNode = new MidasNode();
                        double H_step = 4;
                        double line_length = Math.Abs(singleLineNodes[0].z - singleLineNodes[singleLineNodes_count - 1].z);
                        //Console.WriteLine(String.Format("count = {0}", singleLineNodes_count));
                        int step_count = (int)Math.Round(line_length / H_step);
                        if (step_count == 0)
                            H_step = line_length;
                        else
                            H_step = line_length / step_count;
                        int single_elements_count = 0;
                        for (int j = 0; j < singleLineNodes_count; j++)
                        {

                            if (j == 0)
                            {
                                fNode = singleLineNodes[0].Copy();

                                for (int l = 0; l < z_input_count; l++)
                                {
                                    if (fNode.z >= z_points[l] && fNode.z <= z_points[l + 1])
                                    {
                                        fNode.z = Math.Abs(fNode.z - z_points[l]) < Math.Abs(fNode.z - z_points[l + 1]) ? z_points[l] : z_points[l + 1];
                                        //if (l < 2) 
                                        //    fNode.z = z_points[2];
                                        if (l <= 3)
                                            fNode.z = z_points[2];
                                        break;
                                    }
                                }

                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - fNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - fNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - fNode.z) < 0.005)
                                    {
                                        fNode.num = all_normal_nodes[l].num;
                                        break;
                                    }
                                }
                                //all_normal_nodes[normal_nodes_count++] = fNode.Copy();
                            }
                            if (Math.Abs(singleLineNodes[j].z - fNode.z) - H_step > -0.005)
                            {
                                //all_normal_nodes[normal_nodes_count++] = singleLineNodes[j].Copy();
                                bNode = singleLineNodes[j].Copy();
                                for (int l = 0; l < z_input_count; l++)
                                {
                                    if (bNode.z >= z_points[l] && bNode.z <= z_points[l + 1])
                                    {
                                        bNode.z = Math.Abs(bNode.z - z_points[l]) < Math.Abs(bNode.z - z_points[l + 1]) ? z_points[l] : z_points[l + 1];
                                        if (h0 > 0.005)
                                        {
                                            if (l >= z_input_count - 3)
                                                bNode.z = z_points[z_input_count - 2];
                                        }
                                        else
                                        {
                                            if (l >= z_input_count - 2)
                                                bNode.z = z_points[z_input_count - 1];
                                        }
                                        break;
                                    }
                                }
                                for (int l = 0; l < normal_nodes_count; l++)
                                {
                                    if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                    {
                                        bNode.num = all_normal_nodes[l].num;
                                        break;
                                    }
                                }
                                if (Math.Abs(bNode.z - fNode.z) > 2.5)
                                {

                                    //Console.WriteLine(String.Format("fnode.z = {0},bnode.z = {1},gap = {2}", fNode.z, bNode.z, bNode.z - fNode.z));
                                    yz_bridging_elements[yz_bridging_elements_count].fNode = fNode.Copy();
                                    yz_bridging_elements[yz_bridging_elements_count].num = ++all_elements_count;
                                    yz_bridging_elements[yz_bridging_elements_count++].bNode = bNode.Copy();
                                    single_elements_count++;
                                    fNode = bNode.Copy();
                                }
                            }

                            if (j == singleLineNodes_count - 1 && singleLineNodes_count != 1)
                            {
                                if (single_elements_count >= 1)
                                {

                                    if (Math.Abs(yz_bridging_elements[yz_bridging_elements_count - 1].bNode.z - singleLineNodes[singleLineNodes_count - 1].z) > 0.005)
                                    {

                                        if (bNode.num != 0)
                                            fNode = bNode.Copy();
                                        //fNode = bNode.Copy();
                                        bNode = singleLineNodes[singleLineNodes_count - 1].Copy();
                                        for (int l = 0; l < z_input_count; l++)
                                        {
                                            if (bNode.z >= z_points[l] && bNode.z <= z_points[l + 1])
                                            {
                                                bNode.z = Math.Abs(bNode.z - z_points[l]) < Math.Abs(bNode.z - z_points[l + 1]) ? z_points[l] : z_points[l + 1];
                                                if (h0 > 0.005)
                                                {
                                                    //if (l > z_input_count - 2)
                                                    //    bNode.z = z_points[z_input_count - 2];
                                                    if (l >= z_input_count - 3)
                                                        bNode.z = z_points[z_input_count - 2];
                                                }
                                                else
                                                {
                                                    //if (l > z_input_count - 1)
                                                    //    bNode.z = z_points[z_input_count - 1];
                                                    if (l >= z_input_count - 2)
                                                        bNode.z = z_points[z_input_count - 1];
                                                }
                                                break;
                                            }
                                        }
                                        for (int l = 0; l < normal_nodes_count; l++)
                                        {
                                            if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                            {
                                                bNode.num = all_normal_nodes[l].num;
                                                break;
                                            }
                                        }
                                        if (Math.Abs(fNode.z - bNode.z) > 2.5)
                                        {
                                            //Console.WriteLine(String.Format("fnode.z = {0},bnode.z = {1},gap = {2}", fNode.z, bNode.z, bNode.z - fNode.z));
                                            yz_bridging_elements[yz_bridging_elements_count].fNode = fNode.Copy();
                                            yz_bridging_elements[yz_bridging_elements_count].num = ++all_elements_count;
                                            yz_bridging_elements[yz_bridging_elements_count++].bNode = bNode.Copy();
                                            single_elements_count++;
                                        }
                                        else
                                        {
                                            yz_bridging_elements[yz_bridging_elements_count - 1].bNode = bNode.Copy();
                                        }
                                    }

                                }
                                else
                                {
                                    bNode = singleLineNodes[singleLineNodes_count - 1].Copy();

                                    for (int l = 0; l < z_input_count; l++)
                                    {
                                        if (bNode.z >= z_points[l] && bNode.z <= z_points[l + 1])
                                        {
                                            bNode.z = Math.Abs(bNode.z - z_points[l]) < Math.Abs(bNode.z - z_points[l + 1]) ? z_points[l] : z_points[l + 1];
                                            if (h0 > 0.005)
                                            {
                                                //if (l > z_input_count - 2)
                                                //    bNode.z = z_points[z_input_count - 2];
                                                if (l >= z_input_count - 3)
                                                    bNode.z = z_points[z_input_count - 2];
                                            }
                                            else
                                            {
                                                //if (l > z_input_count - 1)
                                                //    bNode.z = z_points[z_input_count - 1];
                                                if (l >= z_input_count - 2)
                                                    bNode.z = z_points[z_input_count - 1];
                                            }
                                            break;
                                        }
                                    }

                                    for (int l = 0; l < normal_nodes_count; l++)
                                    {
                                        if (Math.Abs(all_normal_nodes[l].x - bNode.x) < 0.005 && Math.Abs(all_normal_nodes[l].y - bNode.y) < 0.005 && Math.Abs(all_normal_nodes[l].z - bNode.z) < 0.005)
                                        {
                                            bNode.num = all_normal_nodes[l].num;
                                            break;
                                        }
                                    }
                                    if (Math.Abs(fNode.z - bNode.z) > 2.5)
                                    {
                                        //Console.WriteLine(String.Format("fnode.z = {0},bnode.z = {1},gap = {2}", fNode.z, bNode.z, bNode.z - fNode.z));
                                        yz_bridging_elements[yz_bridging_elements_count].fNode = fNode.Copy();
                                        yz_bridging_elements[yz_bridging_elements_count].num = ++all_elements_count;
                                        yz_bridging_elements[yz_bridging_elements_count++].bNode = bNode.Copy();
                                        single_elements_count++;
                                    }
                                }
                            }
                        }
                    }
                }
                yz_bridging_nodes_start = yz_bridging_nodes_start < normal_nodes_count ? yz_bridging_nodes_start : normal_nodes_count;
                yz_bridging_nodes_end = normal_nodes_count;

                yz_bridging_elements_end = all_elements_count;
                yz_bridging_elements_start = yz_bridging_elements_start < yz_bridging_elements_end ? yz_bridging_elements_start : yz_bridging_elements_end;

            }
            /////////////////////////////////////////////////////////////////////////
            # endregion

            SetProcess(80);
            //all_elements_count = 0;//单元号计数器
            //for (int i = 0; i < y_elements.Length;i++)
            //{
            //    y_elements[i] = new MidasElement();
            //}
            //y_elements_count = 0;
            //for (int i = 2; i < z_input_count - 1; i++)
            //{
            //    for (int j = 0; j < x_input_count + 1; j++)
            //    {
            //        MidasNode tempfnode = new MidasNode();
            //        tempfnode.y = 1000;
            //        MidasNode tempbnode = new MidasNode();
            //        tempbnode.y = 1000;
            //        for (int l = 0; l < normal_nodes_count; l++)
            //        {
            //            if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].x - x_points[j]) < 0.005)
            //            {
            //                if (tempfnode.y > all_normal_nodes[l].y)
            //                {
            //                    tempfnode = all_normal_nodes[l].Copy();
            //                }
            //            }
            //        }
            //        while (true)
            //        {
            //            tempbnode.y = 1000;
            //            for (int l = 0; l < normal_nodes_count; l++)
            //            {
            //                if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].x - x_points[j]) < 0.005 && all_normal_nodes[l].y - tempfnode.y > 0.005)
            //                {
            //                    if (tempbnode.y > all_normal_nodes[l].y)
            //                    {
            //                        tempbnode = all_normal_nodes[l].Copy();
            //                    }
            //                }
            //            }
            //            if (tempbnode.y - 1000 > -0.005)
            //            {
            //                break;
            //            }
            //            all_elements_count++;
            //            y_elements[y_elements_count].num = all_elements_count;
            //            y_elements[y_elements_count].fNode = tempfnode.Copy();
            //            y_elements[y_elements_count++].bNode = tempbnode.Copy();
            //            tempfnode = tempbnode.Copy();
            //        }
            //    }
            //}
            //SetProcess(60);

            ////MidasElement[] x_elements = new MidasElement[20000];
            //for (int i = 0; i < x_elements.Length; i++)
            //{
            //    x_elements[i] = new MidasElement();
            //}
            //x_elements_count = 0;
            //for (int i = 2; i < z_input_count - 1; i++)
            //{
            //    for (int j = 0; j < y_input_count + 1; j++)
            //    {


            //        MidasNode tempfnode = new MidasNode();
            //        tempfnode.x = 1000;
            //        MidasNode tempbnode = new MidasNode();
            //        tempbnode.x = 1000;
            //        for (int l = 0; l < normal_nodes_count; l++)
            //        {
            //            if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005)
            //            {
            //                if (tempfnode.x > all_normal_nodes[l].x)
            //                {
            //                    tempfnode = all_normal_nodes[l].Copy();
            //                }
            //            }
            //        }
            //        while (true)
            //        {
            //            tempbnode.x = 1000;
            //            for (int l = 0; l < normal_nodes_count; l++)
            //            {
            //                if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005 && all_normal_nodes[l].x - tempfnode.x > 0.005)
            //                {
            //                    if (tempbnode.x > all_normal_nodes[l].x)
            //                    {
            //                        tempbnode = all_normal_nodes[l].Copy();
            //                    }
            //                }
            //            }
            //            if (tempbnode.x - 1000 > -0.005)
            //            {
            //                break;
            //            }
            //            all_elements_count++;
            //            x_elements[x_elements_count].num = all_elements_count;
            //            x_elements[x_elements_count].fNode = tempfnode.Copy();
            //            x_elements[x_elements_count++].bNode = tempbnode.Copy();
            //            tempfnode = tempbnode.Copy();
            //        }
            //    }
            //}
            //SetProcess(70);

            ////MidasElement[] z_elements = new MidasElement[20000];
            //for (int i = 0; i < z_elements.Length; i++)
            //{
            //    z_elements[i] = new MidasElement();
            //}
            //z_elements_count = 0;
            //for (int i = 0; i < x_input_count + 1; i++)
            //{
            //    for (int j = 0; j < y_input_count + 1; j++)
            //    {
            //        MidasNode tempfnode = new MidasNode();
            //        tempfnode.z = 1000;
            //        MidasNode tempbnode = new MidasNode();
            //        tempbnode.z = 1000;
            //        for (int l = 0; l < normal_nodes_count; l++)
            //        {
            //            if (Math.Abs(all_normal_nodes[l].x - x_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005)
            //            {
            //                if (tempfnode.z > all_normal_nodes[l].z)
            //                {
            //                    tempfnode = all_normal_nodes[l].Copy();
            //                }
            //            }
            //        }
            //        while (true)
            //        {
            //            tempbnode.z = 1000;
            //            for (int l = 0; l < normal_nodes_count; l++)
            //            {
            //                if (Math.Abs(all_normal_nodes[l].x - x_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[j]) < 0.005 && all_normal_nodes[l].z - tempfnode.z > 0.005)
            //                {
            //                    if (tempbnode.z > all_normal_nodes[l].z)
            //                    {
            //                        tempbnode = all_normal_nodes[l].Copy();
            //                    }
            //                }
            //            }
            //            if (tempbnode.z - 1000 > -0.005)
            //            {
            //                break;
            //            }
            //            all_elements_count++;
            //            z_elements[z_elements_count].num = all_elements_count;
            //            z_elements[z_elements_count].fNode = tempfnode.Copy();
            //            z_elements[z_elements_count++].bNode = tempbnode.Copy();
            //            tempfnode = tempbnode.Copy();
            //        }
            //    }
            //}
            //SetProcess(80);


            ////MidasElement[] bottom_elements = new MidasElement[1000];
            //for (int i = 0; i < bottom_elements.Length; i++)
            //{
            //    bottom_elements[i] = new MidasElement();
            //}
            //bottom_elements_count = 0;

            //for (int j = 0; j < x_input_count + 1; j++)
            //{
            //    for (int k = 0; k < y_input_count + 1; k++)
            //    {
            //        for (int l = 0; l < bottom_nodes_count; l++)
            //        {
            //            if (Math.Abs(bottom_nodes[l].z - h2) < 0.005 && Math.Abs(bottom_nodes[l].x - x_points[j]) < 0.005 && Math.Abs(bottom_nodes[l].y - y_points[k]) < 0.005)
            //            {
            //                if (k == 0)
            //                {
            //                    bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
            //                }
            //                else
            //                {
            //                    all_elements_count++;
            //                    bottom_elements[bottom_elements_count].num = all_elements_count;
            //                    bottom_elements[bottom_elements_count++].bNode = bottom_nodes[l].Copy();
            //                    bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
            //                }
            //                break;
            //            }
            //        }
            //    }
            //    j = j + x_input_count - 1;
            //}
            //for (int j = 0; j < y_input_count + 1; j++)
            //{
            //    for (int k = 0; k < x_input_count + 1; k++)
            //    {
            //        for (int l = 0; l < bottom_nodes_count; l++)
            //        {
            //            if (Math.Abs(bottom_nodes[l].z - h2) < 0.005 && Math.Abs(bottom_nodes[l].x - x_points[k]) < 0.005 && Math.Abs(bottom_nodes[l].y - y_points[j]) < 0.005)
            //            {
            //                if (k == 0)
            //                {
            //                    bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
            //                }
            //                else
            //                {
            //                    all_elements_count++;
            //                    bottom_elements[bottom_elements_count].num = all_elements_count;
            //                    bottom_elements[bottom_elements_count++].bNode = bottom_nodes[l].Copy();
            //                    bottom_elements[bottom_elements_count].fNode = bottom_nodes[l].Copy();
            //                }
            //                break;
            //            }
            //        }
            //    }
            //    j = j + y_input_count - 1;
            //}
            //SetProcess(81);
            //int bottom_elements_end = all_elements_count;

            //MidasElement[] xy_bridging_elements = null;
            //int xy_bridging_elements_count = 0;
            //int xy_bridging_elements_start = 0;
            //int xy_bridging_elements_end = 0;

            //MidasElement[] xz_bridging_elements = null;
            //int xz_bridging_elements_count = 0;
            //int xz_bridging_elements_start = 0;
            //int xz_bridging_elements_end = 0;

            //MidasElement[] yz_bridging_elements = null;
            //int yz_bridging_elements_count = 0;
            //int yz_bridging_elements_start = 0;
            //int yz_bridging_elements_end = 0;

            # region 可以去除的部分，该部分为以前的版本中计算剪刀撑单元算法
            if (false)
            {
                xy_bridging_elements = new MidasElement[50000];
                xy_bridging_elements_count = 0;
                xy_bridging_elements_start = all_elements_count + 1;
                for (int i = 0; i < xy_bridging_elements.Length; i++)
                {
                    xy_bridging_elements[i] = new MidasElement();
                }
                for (int k = 0; k < z_bridging_count; k++)
                {
                    for (int i = 0; i < xy_bridging_lines_count; i++)
                    {
                        xy_bridging_lines[i].fNode.z = z_bridging_points[k];
                        xy_bridging_lines[i].bNode.z = z_bridging_points[k];
                        //算法从xy剪刀节点中找出在这条线上的点，并按先后生成单元
                        MidasNode[] xy_bridgingline_nodes = new MidasNode[1000];
                        int xy_bridgingline_nodes_count = 0;
                        for (int j = 0; j < xy_bridgingline_nodes.Length; j++)
                        {
                            xy_bridgingline_nodes[j] = new MidasNode();
                            xy_bridgingline_nodes[j].y = 1000;
                        }
                        //xy_bridgingline_nodes[xy_bridgingline_nodes_count] = xy_bridging_lines[i].fNode.Copy();
                        for (int j = xy_bridging_nodes_start - 1; j <= xy_bridging_nodes_end - 1; j++)
                        {
                            if (true == judge_node_inline(all_normal_nodes[j], xy_bridging_lines[i]))
                            {
                                if (xy_bridgingline_nodes[xy_bridgingline_nodes_count].y - all_normal_nodes[j].y > 0.005)
                                    xy_bridgingline_nodes[xy_bridgingline_nodes_count] = all_normal_nodes[j].Copy();
                            }
                        }
                        for (int j = 0; j < xy_bridging_repeat_nodenum_count; j++)
                        {
                            if (true == judge_node_inline(all_normal_nodes[xy_bridging_repeat_nodenum[j] - 1], xy_bridging_lines[i]))
                            {
                                if (xy_bridgingline_nodes[xy_bridgingline_nodes_count].y - all_normal_nodes[xy_bridging_repeat_nodenum[j] - 1].y > 0.005)
                                    xy_bridgingline_nodes[xy_bridgingline_nodes_count] = all_normal_nodes[xy_bridging_repeat_nodenum[j] - 1].Copy();
                            }
                        }
                        xy_bridgingline_nodes_count++;
                        while (true)
                        {
                            for (int j = xy_bridging_nodes_start - 1; j <= xy_bridging_nodes_end - 1; j++)
                            {
                                if (true == judge_node_inline(all_normal_nodes[j], xy_bridging_lines[i]))
                                {
                                    if (all_normal_nodes[j].y - xy_bridgingline_nodes[xy_bridgingline_nodes_count - 1].y > 0.005)
                                    {
                                        if (all_normal_nodes[j].y - xy_bridgingline_nodes[xy_bridgingline_nodes_count].y < -0.005)
                                        {
                                            xy_bridgingline_nodes[xy_bridgingline_nodes_count] = all_normal_nodes[j].Copy();
                                        }
                                    }

                                }
                            }
                            for (int j = 0; j < xy_bridging_repeat_nodenum_count; j++)
                            {
                                if (true == judge_node_inline(all_normal_nodes[xy_bridging_repeat_nodenum[j] - 1], xy_bridging_lines[i]))
                                {
                                    if (all_normal_nodes[xy_bridging_repeat_nodenum[j] - 1].y - xy_bridgingline_nodes[xy_bridgingline_nodes_count - 1].y > 0.005)
                                    {
                                        if (all_normal_nodes[xy_bridging_repeat_nodenum[j] - 1].y - xy_bridgingline_nodes[xy_bridgingline_nodes_count].y < -0.005)
                                        {
                                            xy_bridgingline_nodes[xy_bridgingline_nodes_count] = all_normal_nodes[xy_bridging_repeat_nodenum[j] - 1].Copy();
                                        }
                                    }
                                }
                            }
                            if (xy_bridgingline_nodes[xy_bridgingline_nodes_count].y - 1000 > -0.005)
                            {
                                break;
                            }
                            xy_bridgingline_nodes_count++;
                        }
                        for (int j = 1; j < xy_bridgingline_nodes_count; j++)
                        {
                            all_elements_count++;
                            xy_bridging_elements[xy_bridging_elements_count].num = all_elements_count;
                            xy_bridging_elements[xy_bridging_elements_count].fNode = xy_bridgingline_nodes[j - 1].Copy();
                            xy_bridging_elements[xy_bridging_elements_count++].bNode = xy_bridgingline_nodes[j].Copy();
                        }
                    }
                }
                xy_bridging_elements_start = xy_bridging_elements_start < all_elements_count ? xy_bridging_elements_start : all_elements_count;
                xy_bridging_elements_end = all_elements_count;
                SetProcess(85);



                xz_bridging_elements = new MidasElement[50000];
                xz_bridging_elements_count = 0;
                xz_bridging_elements_start = all_elements_count + 1;
                for (int i = 0; i < xz_bridging_elements.Length; i++)
                {
                    xz_bridging_elements[i] = new MidasElement();
                }
                for (int k = 0; k < y_bridging_count; k++)
                {
                    for (int i = 0; i < xz_bridging_lines_count; i++)
                    {
                        xz_bridging_lines[i].fNode.y = y_bridging_points[k];
                        xz_bridging_lines[i].bNode.y = y_bridging_points[k];
                        //算法从xy剪刀节点中找出在这条线上的点，并按先后生成单元
                        MidasNode[] xz_bridgingline_nodes = new MidasNode[1000];
                        int xz_bridgingline_nodes_count = 0;
                        for (int j = 0; j < xz_bridgingline_nodes.Length; j++)
                        {
                            xz_bridgingline_nodes[j] = new MidasNode();
                            xz_bridgingline_nodes[j].z = 1000;
                        }
                        //xz_bridgingline_nodes[xz_bridgingline_nodes_count] = xz_bridging_lines[i].fNode.Copy();
                        for (int j = xz_bridging_nodes_start - 1; j <= xz_bridging_nodes_end - 1; j++)
                        {
                            if (true == judge_node_inline(all_normal_nodes[j], xz_bridging_lines[i]))
                            {
                                if (xz_bridgingline_nodes[xz_bridgingline_nodes_count].z - all_normal_nodes[j].z > 0.005)
                                    xz_bridgingline_nodes[xz_bridgingline_nodes_count] = all_normal_nodes[j].Copy();
                            }
                        }
                        for (int j = 0; j < xz_bridging_repeat_nodenum_count; j++)
                        {
                            if (true == judge_node_inline(all_normal_nodes[xz_bridging_repeat_nodenum[j] - 1], xz_bridging_lines[i]))
                            {
                                if (xz_bridgingline_nodes[xz_bridgingline_nodes_count].z - all_normal_nodes[xz_bridging_repeat_nodenum[j] - 1].z > 0.005)
                                    xz_bridgingline_nodes[xz_bridgingline_nodes_count] = all_normal_nodes[xz_bridging_repeat_nodenum[j] - 1].Copy();
                            }
                        }
                        xz_bridgingline_nodes_count++;
                        while (true)
                        {
                            for (int j = xz_bridging_nodes_start - 1; j <= xz_bridging_nodes_end - 1; j++)
                            {
                                if (true == judge_node_inline(all_normal_nodes[j], xz_bridging_lines[i]))
                                {
                                    if (all_normal_nodes[j].z - xz_bridgingline_nodes[xz_bridgingline_nodes_count - 1].z > 0.005)
                                    {
                                        if (all_normal_nodes[j].z - xz_bridgingline_nodes[xz_bridgingline_nodes_count].z < -0.005)
                                        {
                                            xz_bridgingline_nodes[xz_bridgingline_nodes_count] = all_normal_nodes[j].Copy();
                                        }
                                    }

                                }
                            }
                            for (int j = 0; j < xz_bridging_repeat_nodenum_count; j++)
                            {
                                if (true == judge_node_inline(all_normal_nodes[xz_bridging_repeat_nodenum[j] - 1], xz_bridging_lines[i]))
                                {
                                    if (all_normal_nodes[xz_bridging_repeat_nodenum[j] - 1].z - xz_bridgingline_nodes[xz_bridgingline_nodes_count - 1].z > 0.005)
                                    {
                                        if (all_normal_nodes[xz_bridging_repeat_nodenum[j] - 1].z - xz_bridgingline_nodes[xz_bridgingline_nodes_count].z < -0.005)
                                        {
                                            xz_bridgingline_nodes[xz_bridgingline_nodes_count] = all_normal_nodes[xz_bridging_repeat_nodenum[j] - 1].Copy();
                                        }
                                    }
                                }
                            }
                            if (xz_bridgingline_nodes[xz_bridgingline_nodes_count].z - 1000 > -0.005)
                            {
                                break;
                            }
                            xz_bridgingline_nodes_count++;
                        }
                        for (int j = 1; j < xz_bridgingline_nodes_count; j++)
                        {
                            all_elements_count++;
                            xz_bridging_elements[xz_bridging_elements_count].num = all_elements_count;
                            xz_bridging_elements[xz_bridging_elements_count].fNode = xz_bridgingline_nodes[j - 1].Copy();
                            xz_bridging_elements[xz_bridging_elements_count++].bNode = xz_bridgingline_nodes[j].Copy();
                        }
                    }
                }
                xz_bridging_elements_start = xz_bridging_elements_start < all_elements_count ? xz_bridging_elements_start : all_elements_count;
                xz_bridging_elements_end = all_elements_count;
                SetProcess(90);

                yz_bridging_elements = new MidasElement[50000];
                yz_bridging_elements_count = 0;
                yz_bridging_elements_start = all_elements_count + 1;
                for (int i = 0; i < yz_bridging_elements.Length; i++)
                {
                    yz_bridging_elements[i] = new MidasElement();
                }
                for (int k = 0; k < x_bridging_count; k++)
                {
                    for (int i = 0; i < yz_bridging_lines_count; i++)
                    {
                        yz_bridging_lines[i].fNode.x = x_bridging_points[k];
                        yz_bridging_lines[i].bNode.x = x_bridging_points[k];
                        //算法从xy剪刀节点中找出在这条线上的点，并按先后生成单元
                        MidasNode[] yz_bridgingline_nodes = new MidasNode[1000];
                        int yz_bridgingline_nodes_count = 0;
                        for (int j = 0; j < yz_bridgingline_nodes.Length; j++)
                        {
                            yz_bridgingline_nodes[j] = new MidasNode();
                            yz_bridgingline_nodes[j].z = 1000;
                        }
                        //yz_bridgingline_nodes[yz_bridgingline_nodes_count] = yz_bridging_lines[i].fNode.Copy();
                        for (int j = yz_bridging_nodes_start - 1; j <= yz_bridging_nodes_end - 1; j++)
                        {
                            if (true == judge_node_inline(all_normal_nodes[j], yz_bridging_lines[i]))
                            {
                                if (yz_bridgingline_nodes[yz_bridgingline_nodes_count].z - all_normal_nodes[j].z > 0.005)
                                    yz_bridgingline_nodes[yz_bridgingline_nodes_count] = all_normal_nodes[j].Copy();
                            }
                        }
                        for (int j = 0; j < yz_bridging_repeat_nodenum_count; j++)
                        {
                            if (true == judge_node_inline(all_normal_nodes[yz_bridging_repeat_nodenum[j] - 1], yz_bridging_lines[i]))
                            {
                                if (yz_bridgingline_nodes[yz_bridgingline_nodes_count].z - all_normal_nodes[yz_bridging_repeat_nodenum[j] - 1].z > 0.005)
                                    yz_bridgingline_nodes[yz_bridgingline_nodes_count] = all_normal_nodes[yz_bridging_repeat_nodenum[j] - 1].Copy();
                            }
                        }
                        yz_bridgingline_nodes_count++;
                        while (true)
                        {
                            for (int j = yz_bridging_nodes_start - 1; j <= yz_bridging_nodes_end - 1; j++)
                            {
                                if (true == judge_node_inline(all_normal_nodes[j], yz_bridging_lines[i]))
                                {
                                    if (all_normal_nodes[j].z - yz_bridgingline_nodes[yz_bridgingline_nodes_count - 1].z > 0.005)
                                    {
                                        if (all_normal_nodes[j].z - yz_bridgingline_nodes[yz_bridgingline_nodes_count].z < -0.005)
                                        {
                                            yz_bridgingline_nodes[yz_bridgingline_nodes_count] = all_normal_nodes[j].Copy();
                                        }
                                    }

                                }
                            }
                            for (int j = 0; j < yz_bridging_repeat_nodenum_count; j++)
                            {
                                if (true == judge_node_inline(all_normal_nodes[yz_bridging_repeat_nodenum[j] - 1], yz_bridging_lines[i]))
                                {
                                    if (all_normal_nodes[yz_bridging_repeat_nodenum[j] - 1].z - yz_bridgingline_nodes[yz_bridgingline_nodes_count - 1].z > 0.005)
                                    {
                                        if (all_normal_nodes[yz_bridging_repeat_nodenum[j] - 1].z - yz_bridgingline_nodes[yz_bridgingline_nodes_count].z < -0.005)
                                        {
                                            yz_bridgingline_nodes[yz_bridgingline_nodes_count] = all_normal_nodes[yz_bridging_repeat_nodenum[j] - 1].Copy();
                                        }
                                    }
                                }
                            }
                            if (yz_bridgingline_nodes[yz_bridgingline_nodes_count].z - 1000 > -0.005)
                            {
                                break;
                            }
                            yz_bridgingline_nodes_count++;
                        }
                        for (int j = 1; j < yz_bridgingline_nodes_count; j++)
                        {
                            all_elements_count++;
                            yz_bridging_elements[yz_bridging_elements_count].num = all_elements_count;
                            yz_bridging_elements[yz_bridging_elements_count].fNode = yz_bridgingline_nodes[j - 1].Copy();
                            yz_bridging_elements[yz_bridging_elements_count++].bNode = yz_bridgingline_nodes[j].Copy();
                        }
                    }
                }
                yz_bridging_elements_start = yz_bridging_elements_start < all_elements_count ? yz_bridging_elements_start : all_elements_count;
                yz_bridging_elements_end = all_elements_count;
                SetProcess(95);
            }
            # endregion



            SetText("写入中...", this.status_bar_text);
            buffer_string = String.Format(";---------------------------------------------------------------------------");
            writer.WriteLine(buffer_string);
            //        str.Format();
            //File1.WriteString(str);
            buffer_string = String.Format(";  MIDAS/Civil Text(MCT) File.");
            writer.WriteLine(buffer_string);
            //str.Format(";  MIDAS/Civil Text(MCT) File.\n");
            //File1.WriteString(str);
            //CTime t = CTime::GetCurrentTime();
            buffer_string = String.Format(";  Date : {0}", DateTime.Now.ToShortDateString());
            writer.WriteLine(buffer_string);
            //str.Format(";  Date : %d/%d/%d\n",t.GetYear(),t.GetMonth(),t.GetDay());
            //File1.WriteString(str);
            buffer_string = String.Format(";---------------------------------------------------------------------------\r\n");
            writer.WriteLine(buffer_string);
            //str.Format(";---------------------------------------------------------------------------\n\n");
            //File1.WriteString(str);
            buffer_string = String.Format("*VERSION\r\n   8.0.5\r\n");
            writer.WriteLine(buffer_string);
            //str.Format("*VERSION\n   8.0.5\n\n");
            //File1.WriteString(str);
            buffer_string = String.Format(";助手版本 1.7.0\r\n");
            writer.WriteLine(buffer_string);
            //str.Format(";助手版本 1.7.0\n\n");
            //File1.WriteString(str);
            buffer_string = String.Format("*UNIT    ; Unit System\r\n; FORCE, LENGTH, HEAT, TEMPER\r\n   KN   , M, KJ, C\r\n");
            writer.WriteLine(buffer_string);
            //str.Format("*UNIT    ; Unit System\n; FORCE, LENGTH, HEAT, TEMPER\n   KN   , M, KJ, C\n\n");
            //File1.WriteString(str);



            buffer_string = String.Format("*NODE    ; Nodes");
            writer.WriteLine(buffer_string);
            buffer_string = String.Format("; iNO, X, Y, Z");
            writer.WriteLine(buffer_string);
            buffer_string = String.Format(";Normal节点");
            writer.WriteLine(buffer_string);
            for (int i = regular_nodes_start - 1; i <= regular_nodes_end - 1; i++)
            {
                buffer_string = string.Format("{0,-5},{1:0.00},{2:0.00},{3:0.00}", all_normal_nodes[i].num,
                    all_normal_nodes[i].x, all_normal_nodes[i].y, all_normal_nodes[i].z);
                writer.WriteLine(buffer_string);
            }

            buffer_string = String.Format("\r\n;扫地杆节点");
            writer.WriteLine(buffer_string);
            for (int i = bottom_nodes_start - 1; i <= bottom_nodes_end - 1; i++)
            {
                buffer_string = string.Format("{0,-5},{1:0.00},{2:0.00},{3:0.00}", all_normal_nodes[i].num,
                    all_normal_nodes[i].x, all_normal_nodes[i].y, all_normal_nodes[i].z);
                writer.WriteLine(buffer_string);
            }

            if (false)
            {
                buffer_string = String.Format("\r\n;XY剪刀撑节点");
                writer.WriteLine(buffer_string);
                for (int i = xy_bridging_nodes_start - 1; i <= xy_bridging_nodes_end - 1; i++)
                {
                    buffer_string = string.Format("{0,-5},{1:0.00},{2:0.00},{3:0.00}", all_normal_nodes[i].num,
                        all_normal_nodes[i].x, all_normal_nodes[i].y, all_normal_nodes[i].z);
                    writer.WriteLine(buffer_string);
                }

                buffer_string = String.Format("\r\n;XZ剪刀撑节点");
                writer.WriteLine(buffer_string);
                for (int i = xz_bridging_nodes_start - 1; i <= xz_bridging_nodes_end - 1; i++)
                {
                    buffer_string = string.Format("{0,-5},{1:0.00},{2:0.00},{3:0.00}", all_normal_nodes[i].num,
                        all_normal_nodes[i].x, all_normal_nodes[i].y, all_normal_nodes[i].z);
                    writer.WriteLine(buffer_string);
                }

                buffer_string = String.Format("\r\n;YZ剪刀撑节点");
                writer.WriteLine(buffer_string);
                for (int i = yz_bridging_nodes_start - 1; i <= yz_bridging_nodes_end - 1; i++)
                {
                    buffer_string = string.Format("{0,-5},{1:0.00},{2:0.00},{3:0.00}", all_normal_nodes[i].num,
                        all_normal_nodes[i].x, all_normal_nodes[i].y, all_normal_nodes[i].z);
                    writer.WriteLine(buffer_string);
                }
            }


            buffer_string = String.Format("\r\n*ELEMENT ");
            writer.WriteLine(buffer_string);
            buffer_string = String.Format("\r\n;Y方向单元 ");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < y_elements_count; i++)
            {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                buffer_string = string.Format("{0,-5},BEAM,{1},{2},{3},{4},{5}", y_elements[i].num, 1, 1, y_elements[i].fNode.num, y_elements[i].bNode.num, 0);
                writer.WriteLine(buffer_string);
            }
            buffer_string = String.Format("\r\n;X方向单元 ");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < x_elements_count; i++)
            {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                buffer_string = string.Format("{0,-5},BEAM,{1},{2},{3},{4},{5}", x_elements[i].num, 1, 1, x_elements[i].fNode.num, x_elements[i].bNode.num, 0);
                writer.WriteLine(buffer_string);
            }
            buffer_string = String.Format("\r\n;Z方向单元 ");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < z_elements_count; i++)
            {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                buffer_string = string.Format("{0,-5},BEAM,{1},{2},{3},{4},{5}", z_elements[i].num, 1, 1, z_elements[i].fNode.num, z_elements[i].bNode.num, 0);
                writer.WriteLine(buffer_string);
            }
            buffer_string = String.Format("\r\n;扫地杆单元 ");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < bottom_elements_count; i++)
            {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                buffer_string = string.Format("{0,-5},BEAM,{1},{2},{3},{4},{5}", bottom_elements[i].num, 1, 1, bottom_elements[i].fNode.num, bottom_elements[i].bNode.num, 0);
                writer.WriteLine(buffer_string);
            }

            if (bridging_check)
            {
                buffer_string = String.Format("\r\n;XY剪刀单元 ");
                writer.WriteLine(buffer_string);
                for (int i = 0; i < xy_bridging_elements_count; i++)
                {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                    buffer_string = string.Format("{0,-5},BEAM,{1},{2},{3},{4},{5}", xy_bridging_elements[i].num, 1, 1, xy_bridging_elements[i].fNode.num, xy_bridging_elements[i].bNode.num, 0);
                    writer.WriteLine(buffer_string);
                }

                buffer_string = String.Format("\r\n;XZ剪刀单元 ");
                writer.WriteLine(buffer_string);
                for (int i = 0; i < xz_bridging_elements_count; i++)
                {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                    buffer_string = string.Format("{0,-5},BEAM,{1},{2},{3},{4},{5}", xz_bridging_elements[i].num, 1, 1, xz_bridging_elements[i].fNode.num, xz_bridging_elements[i].bNode.num, 0);
                    writer.WriteLine(buffer_string);
                }

                buffer_string = String.Format("\r\n;YZ剪刀单元 ");
                writer.WriteLine(buffer_string);
                for (int i = 0; i < yz_bridging_elements_count; i++)
                {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                    buffer_string = string.Format("{0,-5},BEAM,{1},{2},{3},{4},{5}", yz_bridging_elements[i].num, 1, 1, yz_bridging_elements[i].fNode.num, yz_bridging_elements[i].bNode.num, 0);
                    writer.WriteLine(buffer_string);
                }
            }

            buffer_string = String.Format("\r\n;分组信息 ");
            writer.WriteLine(buffer_string);
            buffer_string = String.Format("\r\n*GROUP    ; Group\n; NAME, NODE_LIST, ELEM_LIST, PLANE_TYPE");
            writer.WriteLine(buffer_string);

            buffer_string = String.Format("规则节点,{0}to{1},,0\r\n", regular_nodes_start, regular_nodes_end);
            writer.WriteLine(buffer_string);
            buffer_string = String.Format("扫地杆节点,{0}to{1},,0\r\n", bottom_nodes_start, bottom_nodes_end);
            writer.WriteLine(buffer_string);

            if (false)
            {
                string bridging_nodesnum_repeat = "";
                for (int i = 0; i < xy_bridging_repeat_nodenum_count; i++)
                {
                    bridging_nodesnum_repeat = bridging_nodesnum_repeat + " " + xy_bridging_repeat_nodenum[i].ToString();
                }
                buffer_string = String.Format("XY剪刀撑节点,{0}to{1}{2},,0\r\n", xy_bridging_nodes_start, xy_bridging_nodes_end, bridging_nodesnum_repeat);
                writer.WriteLine(buffer_string);
                bridging_nodesnum_repeat = "";
                for (int i = 0; i < xz_bridging_repeat_nodenum_count; i++)
                {
                    bridging_nodesnum_repeat = bridging_nodesnum_repeat + " " + xz_bridging_repeat_nodenum[i].ToString();
                }
                buffer_string = String.Format("XZ剪刀撑节点,{0}to{1}{2},,0\r\n", xz_bridging_nodes_start, xz_bridging_nodes_end, bridging_nodesnum_repeat);
                writer.WriteLine(buffer_string);
                bridging_nodesnum_repeat = "";
                for (int i = 0; i < yz_bridging_repeat_nodenum_count; i++)
                {
                    bridging_nodesnum_repeat = bridging_nodesnum_repeat + " " + yz_bridging_repeat_nodenum[i].ToString();
                }
                buffer_string = String.Format("YZ剪刀撑节点,{0}to{1}{2},,0\r\n", yz_bridging_nodes_start, yz_bridging_nodes_end, bridging_nodesnum_repeat);
                writer.WriteLine(buffer_string);
            }

            buffer_string = String.Format("X方向单元,,{0}to{1},0\r\n", x_elements[0].num, x_elements[x_elements_count - 1].num);
            writer.WriteLine(buffer_string);
            buffer_string = String.Format("Y方向单元,,{0}to{1},0\r\n", y_elements[0].num, y_elements[y_elements_count - 1].num);
            writer.WriteLine(buffer_string);
            buffer_string = String.Format("Z方向单元,,{0}to{1},0\r\n", z_elements[0].num, z_elements[z_elements_count - 1].num);
            writer.WriteLine(buffer_string);
            buffer_string = String.Format("扫地杆单元,,{0}to{1},0\r\n", bottom_elements[0].num, bottom_elements[bottom_elements_count - 1].num);
            writer.WriteLine(buffer_string);
            if (bridging_check)
            {
                buffer_string = String.Format("XY剪刀单元,,{0}to{1},0\r\n", xy_bridging_elements_start, xy_bridging_elements_end);
                writer.WriteLine(buffer_string);
                buffer_string = String.Format("XZ剪刀单元,,{0}to{1},0\r\n", xz_bridging_elements_start, xz_bridging_elements_end);
                writer.WriteLine(buffer_string);
                buffer_string = String.Format("YZ剪刀单元,,{0}to{1},0\r\n", yz_bridging_elements_start, yz_bridging_elements_end);
                writer.WriteLine(buffer_string);
            }

            //'±ß½ç×é¡¢ºÉÔØ×é¶¨Òå
            //************* Lines = Lines & readFile("d:\²ÎÊý»¯½¨Ä£\Ö§¼ÜÎÄ¼þ\BNDR-GROUP.mct") ************

            buffer_string = "\r\n*BNDR-GROUP    ; Boundary Group";
            writer.WriteLine(buffer_string);
            buffer_string = "; NAME";
            writer.WriteLine(buffer_string);
            buffer_string = "地基支撑\r\n横杆-立杆\r\n剪刀撑-立杆";
            writer.WriteLine(buffer_string);
            buffer_string = "\r\n*LOAD-GROUP    ; Load Group";
            writer.WriteLine(buffer_string);
            buffer_string = "; NAME";
            writer.WriteLine(buffer_string);
            buffer_string = "预压第一次";
            writer.WriteLine(buffer_string);
            buffer_string = "预压第二次";
            writer.WriteLine(buffer_string);
            buffer_string = "预压第三次";
            writer.WriteLine(buffer_string);
            buffer_string = "浇筑第一次-腹板";
            writer.WriteLine(buffer_string);
            buffer_string = "浇筑第一次-底板";
            writer.WriteLine(buffer_string);
            buffer_string = "浇筑第二次";
            writer.WriteLine(buffer_string);
            buffer_string = "风荷载";
            writer.WriteLine(buffer_string);
            buffer_string = "支架自重";
            writer.WriteLine(buffer_string);
            buffer_string = "模板方楞等";
            writer.WriteLine(buffer_string);
            buffer_string = "附加构件";
            writer.WriteLine(buffer_string);
            buffer_string = "施工人员机械";
            writer.WriteLine(buffer_string);
            buffer_string = "浇筑及振捣";
            writer.WriteLine(buffer_string);


            buffer_string = @"
*MATERIAL    ; Material
; iMAT, TYPE, MNAME, SPHEAT, HEATCO, PLAST, TUNIT, bMASS, DAMPRATIO, [DATA1]          ; STEEL, CONC, USER
; iMAT, TYPE, MNAME, SPHEAT, HEATCO, PLAST, TUNIT, bMASS, DAMPRATIO, [DATA2], [DATA2] ; SRC
; [DATA1] : 1, DB, NAME, CODE 
; [DATA1] : 2, ELAST, POISN, THERMAL, DEN, MASS
; [DATA1] : 3, Ex, Ey, Ez, Tx, Ty, Tz, Sxy, Sxz, Syz, Pxy, Pxz, Pyz, DEN, MASS   ; Orthotropic
; [DATA2] : 1, DB, NAME, CODE or 2, ELAST, POISN, THERMAL, DEN, MASS
    1, STEEL, Q235              , 0, 0, , C, NO, 0.02, 1, GB03(S)    ,            , Q235  

*MATL-COLOR
; iMAT, W_R, W_G, W_B, HF_R, HF_G, HF_B, HE_R, HE_G, HE_B, bBLEND, FACT
    1, 255,   0,   0,    0, 255,   0,    0,   0, 255,  NO, 0.5


*SECTION    ; Section
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, [DATA1], [DATA2]                    ; 1st line - DB/USER
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, BLT, D1, ..., D8, iCEL              ; 1st line - VALUE
;       AREA, ASy, ASz, Ixx, Iyy, Izz                                          ; 2nd line
;       CyP, CyM, CzP, CzM, QyB, QzB, PERI_OUT, PERI_IN, Cy, Cz                ; 3rd line
;       Y1, Y2, Y3, Y4, Z1, Z2, Z3, Z4, Zyy, Zzz                               ; 4th line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, ELAST, DEN, POIS, POIC, SF          ; 1st line - SRC
;       D1, D2, [SRC]                                                          ; 2nd line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, 1, DB, NAME1, NAME2, D1, D2         ; 1st line - COMBINED
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, 2, D11, D12, D13, D14, D15, D21, D22, D23, D24
; iSEC, TYPE, SNAME, [OFFSET2], bSD, SHAPE, iyVAR, izVAR, STYPE                ; 1st line - TAPERED
;       DB, NAME1, NAME2                                                       ; 2nd line(STYPE=DB)
;       [DIM1], [DIM2]                                                         ; 2nd line(STYPE=USER)
;       D11, D12, D13, D14, D15, D16, D17, D18                                 ; 2nd line(STYPE=VALUE)
;       AREA1, ASy1, ASz1, Ixx1, Iyy1, Izz1                                    ; 3rd line(STYPE=VALUE)
;       CyP1, CyM1, CzP1, CzM1, QyB1, QzB1, PERI_OUT1, PERI_IN1, Cy1, Cz1      ; 4th line(STYPE=VALUE)
;       Y11, Y12, Y13, Y14, Z11, Z12, Z13, Z14, Zyy1, Zyy2                     ; 5th line(STYPE=VALUE)
;       D21, D22, D23, D24, D25, D26, D27, D28                                 ; 6th line(STYPE=VALUE)
;       AREA2, ASy2, ASz2, Ixx2, Iyy2, Izz2                                    ; 7th line(STYPE=VALUE)
;       CyP2, CyM2, CzP2, CzM2, QyB2, QzB2, PERI_OUT2, PERI_IN2, Cy2, Cz2      ; 8th line(STYPE=VALUE)
;       Y21, Y22, Y23, Y24, Z21, Z22, Z23, Z24, Zyy2, Zzz2                     ; 9th line(STYPE=VALUE)
;       OPT1, OPT2, [JOINT]                                                    ; 2nd line(STYPE=PSC)
;       ELAST, DEN, POIS, POIC                                                 ; 2nd line(STYPE=PSC-CMPW)
;       bSHEARCHK, [SCHK-I], [SCHK-J], [WT-I], [WT-J], WI, WJ, bSYM, bSIDEHOLE ; 3rd line(STYPE=PSC)
;       bSHEARCHK, bSYM, bHUNCH, [CMPWEB-I], [CMPWEB-J]                        ; 3rd line(STYPE=PSC-CMPW)
;       bUSERDEFMESHSIZE, MESHSIZE, bUSERINPSTIFF, [STIFF-I], [STIFF-J]        ; 4th line(STYPE=PSC)
;       [SIZE-A]-i                                                             ; 5th line(STYPE=PSC)
;       [SIZE-B]-i                                                             ; 6th line(STYPE=PSC)
;       [SIZE-C]-i                                                             ; 7th line(STYPE=PSC)
;       [SIZE-D]-i                                                             ; 8th line(STYPE=PSC)
;       [SIZE-A]-j                                                             ; 9th line(STYPE=PSC)
;       [SIZE-B]-j                                                             ; 10th line(STYPE=PSC)
;       [SIZE-C]-j                                                             ; 11th line(STYPE=PSC)
;       [SIZE-D]-j                                                             ; 12th line(STYPE=PSC)
;       GN, CTC, Bc, Tc, Hh, EsEc, DsDc, Ps, Pc, bMULTI, EsEc-L, EsEc-S        ; 2nd line(STYPE=CMP-B/I)
;       SW_i, Hw_i, tw_i, B_i, Bf1_i, tf1_i, B2_i, Bf2_i, tf2_i                ; 3rd line(STYPE=CMP-B/I)
;       SW_j, Hw_j, tw_j, B_j, Bf1_j, tf1_j, B2_j, Bf2_j, tf2_j                ; 4th line(STYPE=CMP-B/I)
;       N1, N2, Hr, Hr2, tr1, tr2                                              ; 5th line(STYPE=CMP-B)
;       GN, CTC, Bc, Tc, Hh, EgdEsb, DgdDsb, Pgd, Psb, bSYM, SW_i, SW_j        ; 2nd line(STYPE=CMP-CI/CT)
;       OPT1, OPT2, [JOINT]                                                    ; 3rd line(STYPE=CMP-CI/CT)
;       [SIZE-A]-i                                                             ; 4th line(STYPE=CMP-CI/CT)
;       [SIZE-B]-i                                                             ; 5th line(STYPE=CMP-CI/CT)
;       [SIZE-C]-i                                                             ; 6th line(STYPE=CMP-CI/CT)
;       [SIZE-D]-i                                                             ; 7th line(STYPE=CMP-CI/CT)
;       [SIZE-A]-j                                                             ; 8th line(STYPE=CMP-CI/CT)
;       [SIZE-B]-j                                                             ; 9th line(STYPE=CMP-CI/CT)
;       [SIZE-C]-j                                                             ; 10th line(STYPE=CMP-CI/CT)
;       [SIZE-D]-j                                                             ; 11th line(STYPE=CMP-CI/CT)
; iSEC, TYPE, SNAME, [OFFSET], bSD, STYPE1, STYPE2                             ; 1st line - CONSTRUCT
;       SHAPE, ...(same with other type data from shape)                       ; Before (STYPE1)
;       SHAPE, ...(same with other type data from shape)                       ; After  (STYPE2)
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE                                      ; 1st line - COMPOSITE-SB
;       Hw, tw, B, Bf1, tf1, B2, Bf2, tf2                                      ; 2nd line
;       N1, N2, Hr, Hr2, tr1, tr2                                              ; 3rd line
;       SW, GN, CTC, Bc, Tc, Hh, EsEc, DsDc, Ps, Pc, bMulti, Elong, Esh        ; 4th line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE                                      ; 1st line - COMPOSITE-SI
;       Hw, tw, B, tf1, B2, tf2                                                ; 2nd line
;       SW, GN, CTC, Bc, Tc, Hh, EsEc, DsDc, Ps, Pc, bMulti, Elong, Esh        ; 3rd line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE                                      ; 1st line - COMPOSITE-CI/CT
;       OPT1, OPT2, [JOINT]                                                    ; 2nd line
;       [SIZE-A]                                                               ; 3rd line
;       [SIZE-B]                                                               ; 4th line
;       [SIZE-C]                                                               ; 5th line
;       [SIZE-D]                                                               ; 6th line
;       SW, GN, CTC, Bc, Tc, Hh, EgdEsb, DgdDsb, Pgd, Psb                      ; 7th line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE                                      ; 1st line - PSC
;       OPT1, OPT2, [JOINT]                                                    ; 2nd line
;       bSHEARCHK, [SCHK], [WT], WIDTH, bSYM, bSIDEHOLE                        ; 3rd line
;       bUSERDEFMESHSIZE, MESHSIZE, bUSERINPSTIFF, [STIFF]                     ; 4th line
;       [SIZE-A]                                                               ; 5th line
;       [SIZE-B]                                                               ; 6th line
;       [SIZE-C]                                                               ; 7th line
;       [SIZE-D]                                                               ; 8th line
; [DATA1] : 1, DB, NAME or 2, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10
; [DATA2] : CCSHAPE or iCEL or iN1, iN2
; [SRC]  : 1, DB, NAME1, NAME2 or 2, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, iN1, iN2
; [DIM1], [DIM2] : D1, D2, D3, D4, D5, D6, D7, D8
; [OFFSET] : OFFSET, iCENT, iREF, iHORZ, HUSER, iVERT, VUSER
; [OFFSET2]: OFFSET, iCENT, iREF, iHORZ, HUSERI, HUSERJ, iVERT, VUSERI, VUSERJ
; [JOINT]  :  8(1CELL, 2CELL), 13(3CELL),  9(PSCM),  8(PSCH), 9(PSCT),  2(PSCB), 0(nCELL),  2(nCEL2)
; [SIZE-A] :  6(1CELL, 2CELL), 10(3CELL), 10(PSCM),  6(PSCH), 8(PSCT), 10(PSCB), 5(nCELL), 11(nCEL2)
; [SIZE-B] :  6(1CELL, 2CELL), 12(3CELL),  6(PSCM),  6(PSCH), 8(PSCT),  6(PSCB), 8(nCELL), 18(nCEL2)
; [SIZE-C] : 10(1CELL, 2CELL), 13(3CELL),  9(PSCM), 10(PSCH), 7(PSCT),  8(PSCB), 0(nCELL), 11(nCEL2)
; [SIZE-D] :  8(1CELL, 2CELL), 13(3CELL),  6(PSCM),  7(PSCH), 8(PSCT),  5(PSCB), 0(nCELL), 18(nCEL2)
; [STIFF]  : AREA, ASy, ASz, Ixx, Iyy, Izz
; [SCHK]   : bAUTO_Z1, Z1, bAUTO_Z3, Z3
; [WT]     : bAUTO_TOR, TOR, bAUTO_SHR1, SHR1, bAUTO_SHR2, SHR2, bAUTO_SHR3, SHR3
; [CMPWEB] : EFD, LRF, A, B, H, T";
            writer.WriteLine(buffer_string);
            buffer_string = "    1, DBUSER    , 钢管截面          , CC, 0, 0, 0, 0, 0, 0, YES, P  , 2, 0.048, 0.0035, 0, 0, 0, 0, 0, 0, 0, 0";
            writer.WriteLine(buffer_string);
            buffer_string = @"
*SECT-COLOR
; iSEC, W_R, W_G, W_B, HF_R, HF_G, HF_B, HE_R, HE_G, HE_B, bBLEND, FACT
    1, 255,   0,   0,    0, 255,   0,    0,   0, 255,  NO, 0.5

*DGN-SECT
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, [DATA1], [DATA2]                    ; 1st line - DB/USER
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, BLT, D1, ..., D8, iCEL              ; 1st line - VALUE
;       AREA, ASy, ASz, Ixx, Iyy, Izz                                          ; 2nd line
;       CyP, CyM, CzP, CzM, QyB, QzB, PERI_OUT, PERI_IN, Cy, Cz                ; 3rd line
;       Y1, Y2, Y3, Y4, Z1, Z2, Z3, Z4, Zyy, Zzz                               ; 4th line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, ELAST, DEN, POIS, POIC, SF          ; 1st line - SRC
;       D1, D2, [SRC]                                                          ; 2nd line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, 1, DB, NAME1, NAME2, D1, D2         ; 1st line - COMBINED
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE, 2, D11, D12, D13, D14, D15, D21, D22, D23, D24
; iSEC, TYPE, SNAME, [OFFSET2], bSD, SHAPE, iyVAR, izVAR, STYPE                ; 1st line - TAPERED
;       DB, NAME1, NAME2                                                       ; 2nd line(STYPE=DB)
;       [DIM1], [DIM2]                                                         ; 2nd line(STYPE=USER)
;       D11, D12, D13, D14, D15, D16, D17, D18                                 ; 2nd line(STYPE=VALUE)
;       AREA1, ASy1, ASz1, Ixx1, Iyy1, Izz1                                    ; 3rd line(STYPE=VALUE)
;       CyP1, CyM1, CzP1, CzM1, QyB1, QzB1, PERI_OUT1, PERI_IN1, Cy1, Cz1      ; 4th line(STYPE=VALUE)
;       Y11, Y12, Y13, Y14, Z11, Z12, Z13, Z14, Zyy1, Zyy2                     ; 5th line(STYPE=VALUE)
;       D21, D22, D23, D24, D25, D26, D27, D28                                 ; 6th line(STYPE=VALUE)
;       AREA2, ASy2, ASz2, Ixx2, Iyy2, Izz2                                    ; 7th line(STYPE=VALUE)
;       CyP2, CyM2, CzP2, CzM2, QyB2, QzB2, PERI_OUT2, PERI_IN2, Cy2, Cz2      ; 8th line(STYPE=VALUE)
;       Y21, Y22, Y23, Y24, Z21, Z22, Z23, Z24, Zyy2, Zzz2                     ; 9th line(STYPE=VALUE)
;       OPT1, OPT2, [JOINT]                                                    ; 2nd line(STYPE=PSC)
;       ELAST, DEN, POIS, POIC                                                 ; 2nd line(STYPE=PSC-CMPW)
;       bSHEARCHK, [SCHK-I], [SCHK-J], [WT-I], [WT-J], WI, WJ, bSYM, bSIDEHOLE ; 3rd line(STYPE=PSC)
;       bSHEARCHK, bSYM, bHUNCH, [CMPWEB-I], [CMPWEB-J]                        ; 3rd line(STYPE=PSC-CMPW)
;       bUSERDEFMESHSIZE, MESHSIZE, bUSERINPSTIFF, [STIFF-I], [STIFF-J]        ; 4th line(STYPE=PSC)
;       [SIZE-A]-i                                                             ; 5th line(STYPE=PSC)
;       [SIZE-B]-i                                                             ; 6th line(STYPE=PSC)
;       [SIZE-C]-i                                                             ; 7th line(STYPE=PSC)
;       [SIZE-D]-i                                                             ; 8th line(STYPE=PSC)
;       [SIZE-A]-j                                                             ; 9th line(STYPE=PSC)
;       [SIZE-B]-j                                                             ; 10th line(STYPE=PSC)
;       [SIZE-C]-j                                                             ; 11th line(STYPE=PSC)
;       [SIZE-D]-j                                                             ; 12th line(STYPE=PSC)
;       GN, CTC, Bc, Tc, Hh, EsEc, DsDc, Ps, Pc, bMULTI, EsEc-L, EsEc-S        ; 2nd line(STYPE=CMP-B/I)
;       SW_i, Hw_i, tw_i, B_i, Bf1_i, tf1_i, B2_i, Bf2_i, tf2_i                ; 3rd line(STYPE=CMP-B/I)
;       SW_j, Hw_j, tw_j, B_j, Bf1_j, tf1_j, B2_j, Bf2_j, tf2_j                ; 4th line(STYPE=CMP-B/I)
;       N1, N2, Hr, Hr2, tr1, tr2                                              ; 5th line(STYPE=CMP-B)
;       GN, CTC, Bc, Tc, Hh, EgdEsb, DgdDsb, Pgd, Psb, bSYM, SW_i, SW_j        ; 2nd line(STYPE=CMP-CI/CT)
;       OPT1, OPT2, [JOINT]                                                    ; 3rd line(STYPE=CMP-CI/CT)
;       [SIZE-A]-i                                                             ; 4th line(STYPE=CMP-CI/CT)
;       [SIZE-B]-i                                                             ; 5th line(STYPE=CMP-CI/CT)
;       [SIZE-C]-i                                                             ; 6th line(STYPE=CMP-CI/CT)
;       [SIZE-D]-i                                                             ; 7th line(STYPE=CMP-CI/CT)
;       [SIZE-A]-j                                                             ; 8th line(STYPE=CMP-CI/CT)
;       [SIZE-B]-j                                                             ; 9th line(STYPE=CMP-CI/CT)
;       [SIZE-C]-j                                                             ; 10th line(STYPE=CMP-CI/CT)
;       [SIZE-D]-j                                                             ; 11th line(STYPE=CMP-CI/CT)
; iSEC, TYPE, SNAME, [OFFSET], bSD, STYPE1, STYPE2                             ; 1st line - CONSTRUCT
;       SHAPE, ...(same with other type data from shape)                       ; Before (STYPE1)
;       SHAPE, ...(same with other type data from shape)                       ; After  (STYPE2)
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE                                      ; 1st line - COMPOSITE-SB
;       Hw, tw, B, Bf1, tf1, B2, Bf2, tf2                                      ; 2nd line
;       N1, N2, Hr, Hr2, tr1, tr2                                              ; 3rd line
;       SW, GN, CTC, Bc, Tc, Hh, EsEc, DsDc, Ps, Pc, bMulti, Elong, Esh        ; 4th line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE                                      ; 1st line - COMPOSITE-SI
;       Hw, tw, B, tf1, B2, tf2                                                ; 2nd line
;       SW, GN, CTC, Bc, Tc, Hh, EsEc, DsDc, Ps, Pc, bMulti, Elong, Esh        ; 3rd line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE                                      ; 1st line - COMPOSITE-CI/CT
;       OPT1, OPT2, [JOINT]                                                    ; 2nd line
;       [SIZE-A]                                                               ; 3rd line
;       [SIZE-B]                                                               ; 4th line
;       [SIZE-C]                                                               ; 5th line
;       [SIZE-D]                                                               ; 6th line
;       SW, GN, CTC, Bc, Tc, Hh, EgdEsb, DgdDsb, Pgd, Psb                      ; 7th line
; iSEC, TYPE, SNAME, [OFFSET], bSD, SHAPE                                      ; 1st line - PSC
;       OPT1, OPT2, [JOINT]                                                    ; 2nd line
;       bSHEARCHK, [SCHK], [WT], WIDTH, bSYM, bSIDEHOLE                        ; 3rd line
;       bUSERDEFMESHSIZE, MESHSIZE, bUSERINPSTIFF, [STIFF]                     ; 4th line
;       [SIZE-A]                                                               ; 5th line
;       [SIZE-B]                                                               ; 6th line
;       [SIZE-C]                                                               ; 7th line
;       [SIZE-D]                                                               ; 8th line
; [DATA1] : 1, DB, NAME or 2, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10
; [DATA2] : CCSHAPE or iCEL or iN1, iN2
; [SRC]  : 1, DB, NAME1, NAME2 or 2, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, iN1, iN2
; [DIM1], [DIM2] : D1, D2, D3, D4, D5, D6, D7, D8
; [OFFSET] : OFFSET, iCENT, iREF, iHORZ, HUSER, iVERT, VUSER
; [OFFSET2]: OFFSET, iCENT, iREF, iHORZ, HUSERI, HUSERJ, iVERT, VUSERI, VUSERJ
; [JOINT]  :  8(1CELL, 2CELL), 13(3CELL),  9(PSCM),  8(PSCH), 9(PSCT),  2(PSCB), 0(nCELL),  2(nCEL2)
; [SIZE-A] :  6(1CELL, 2CELL), 10(3CELL), 10(PSCM),  6(PSCH), 8(PSCT), 10(PSCB), 5(nCELL), 11(nCEL2)
; [SIZE-B] :  6(1CELL, 2CELL), 12(3CELL),  6(PSCM),  6(PSCH), 8(PSCT),  6(PSCB), 8(nCELL), 18(nCEL2)
; [SIZE-C] : 10(1CELL, 2CELL), 13(3CELL),  9(PSCM), 10(PSCH), 7(PSCT),  8(PSCB), 0(nCELL), 11(nCEL2)
; [SIZE-D] :  8(1CELL, 2CELL), 13(3CELL),  6(PSCM),  7(PSCH), 8(PSCT),  5(PSCB), 0(nCELL), 18(nCEL2)
; [STIFF]  : AREA, ASy, ASz, Ixx, Iyy, Izz
; [SCHK]   : bAUTO_Z1, Z1, bAUTO_Z3, Z3
; [WT]     : bAUTO_TOR, TOR, bAUTO_SHR1, SHR1, bAUTO_SHR2, SHR2, bAUTO_SHR3, SHR3
; [CMPWEB] : EFD, LRF, A, B, H, T";
            writer.WriteLine(buffer_string);
            buffer_string = "\r\n    1, DBUSER    , 钢管截面          , CC, 0, 0, 0, 0, 0, 0, YES, P  , 2, 0.048, 0.0035, 0, 0, 0, 0, 0, 0, 0, 0";
            writer.WriteLine(buffer_string);
            buffer_string = @"


*STLDCASE    ; Static Load Cases
; LCNAME, LCTYPE, DESC
   杆系自重, D , 
   浇筑和振捣混凝土, D , 
   施工人员、材料、设备, D , 
   预压荷载1, D , 
   预压荷载2, D , 
   预压荷载3, D , 
   第一次浇筑, D , 
   第二次浇筑, D , 
   模板、支撑梁, D , 
   防护设施、附加构件, D , 
   风荷载, D , 


*CONSTRAINT    ; Supports
; NODE_LIST, CONSt(Dx,Dy,Dz,Rx,Ry,Rz),GROUP";
            writer.WriteLine(buffer_string);
            buffer_string = string.Format("{0} to  {1}  , 111000,地基支撑", 1, (x_input_count + 1) * (y_input_count + 1));
            writer.WriteLine(buffer_string);
            buffer_string = @"
*FRAME-RLS    ; Beam End Release
; ELEM_LIST, bVALUE, FLAG-i, Fxi, Fyi, Fzi, Mxi, Myi, Mzi        ; 1st line
;                    FLAG-j, Fxj, Fyj, Fzj, Mxj, Myj, Mzj, GROUP ; 2nd line";
            writer.WriteLine(buffer_string);
            string fnode_rel = "";
            string bnode_rel = "";
            double xy_My_i = double.Parse((string)GetText(text_My_i_normal));
            double xy_My_j = double.Parse((string)GetText(text_My_j_normal));
            double xy_Mz_i = double.Parse((string)GetText(text_Mz_i_normal));
            double xy_Mz_j = double.Parse((string)GetText(text_Mz_j_normal));
            if (Math.Abs(xy_My_i) < 0.005)
                xy_My_i = 40.0;
            if (Math.Abs(xy_My_j) < 0.005)
                xy_My_j = 40.0;
            if (Math.Abs(xy_Mz_i) < 0.005)
                xy_Mz_i = 40.0;
            if (Math.Abs(xy_Mz_j) < 0.005)
                xy_Mz_j = 40.0;
            for (int i = 0; i < y_elements_count; i++)
            {
                //if (y_elements[i].fNode.num > bottom_nodes_end)
                //    fnode_rel = "000000, 0, 0, 0, 0, 0, 0";
                //else
                //    fnode_rel = string.Format("000110, 0, 0, 0, {0}, {1}, 0", 40.0, 40.0);
                //if (y_elements[i].bNode.num > bottom_nodes_end)
                //    bnode_rel = "           000000, 0, 0, 0, 0, 0, 0";
                //else
                //    bnode_rel = string.Format("         000110, 0, 0, 0, {0}, {1}, 0", 40.0, 40.0);
                fnode_rel = string.Format("000011, 0, 0, 0, 0, {0}, {1}", xy_My_i, xy_Mz_i);
                bnode_rel = string.Format("         000011, 0, 0, 0, 0, {0}, {1}", xy_My_j, xy_Mz_j);
                buffer_string = string.Format("{0,-5},  YES,{1}\n{2},{3}", y_elements[i].num, fnode_rel, bnode_rel, "横杆-立杆");
                writer.WriteLine(buffer_string);
            }
            for (int i = 0; i < x_elements_count; i++)
            {
                //if (x_elements[i].fNode.num > bottom_nodes_end)
                //    fnode_rel = "000000, 0, 0, 0, 0, 0, 0";
                //else
                //    fnode_rel = string.Format("000110, 0, 0, 0, {0}, {1}, 0", 40.0, 40.0);
                //if (x_elements[i].bNode.num > bottom_nodes_end)
                //    bnode_rel = "           000000, 0, 0, 0, 0, 0, 0";
                //else
                //    bnode_rel = string.Format("         000110, 0, 0, 0, {0}, {1}, 0", 40.0, 40.0);
                fnode_rel = string.Format("000011, 0, 0, 0, 0, {0}, {1}", xy_My_i, xy_Mz_i);
                bnode_rel = string.Format("         000011, 0, 0, 0, 0, {0}, {1}", xy_My_j, xy_Mz_j);
                buffer_string = string.Format("{0,-5},  YES,{1}\n{2},{3}", x_elements[i].num, fnode_rel, bnode_rel, "横杆-立杆");
                writer.WriteLine(buffer_string);
            }
            //for (int i = 0; i < z_elements_count; i++)
            //{
            //    if (z_elements[i].fNode.num > bottom_nodes_end)
            //        fnode_rel = "000000, 0, 0, 0, 0, 0, 0";
            //    else
            //        fnode_rel = string.Format("000110, 0, 0, 0, {0}, {1}, 0", 40.0, 40.0);
            //    if (z_elements[i].bNode.num > bottom_nodes_end)
            //        bnode_rel = "           000000, 0, 0, 0, 0, 0, 0";
            //    else
            //        bnode_rel = string.Format("         000110, 0, 0, 0, {0}, {1}, 0", 40.0, 40.0);
            //    buffer_string = string.Format("{0,-5},  YES,{1}\n{2},{3}", z_elements[i].num, fnode_rel, bnode_rel, "横杆-立杆");
            //    writer.WriteLine(buffer_string);
            //}


            //for (int i = 0; i < bottom_elements_count; i++)
            //{
            //    if (bottom_elements[i].fNode.num > bottom_nodes_end)
            //        fnode_rel = "000000, 0, 0, 0, 0, 0, 0";
            //    else
            //        fnode_rel = string.Format("000110, 0, 0, 0, {0}, {1}, 0", 40.0, 40.0);
            //    if (bottom_elements[i].bNode.num > bottom_nodes_end)
            //        bnode_rel = "           000000, 0, 0, 0, 0, 0, 0";
            //    else
            //        bnode_rel = string.Format("         000110, 0, 0, 0, {0}, {1}, 0", 40.0, 40.0);
            //    buffer_string = string.Format("{0,-5},  YES,{1}\n{2},{3}", bottom_elements[i].num, fnode_rel, bnode_rel, "横杆-立杆");
            //    writer.WriteLine(buffer_string);
            //}

            //double My_i = 50.0;
            //double My_j = 50.0;
            //double Mz_i = 50.0;
            //double Mz_j = 50.0;
            double My_i = double.Parse((string)GetText(text_My_i_cut));
            double My_j = double.Parse((string)GetText(text_My_j_cut));
            double Mz_i = double.Parse((string)GetText(text_Mz_i_cut));
            double Mz_j = double.Parse((string)GetText(text_Mz_j_cut));
            if (Math.Abs(My_i) < 0.005)
                My_i = 50.0;
            if (Math.Abs(My_j) < 0.005)
                My_j = 50.0;
            if (Math.Abs(Mz_i) < 0.005)
                Mz_i = 50.0;
            if (Math.Abs(Mz_j) < 0.005)
                Mz_j = 50.0;
            if (bridging_check)
            {
                for (int i = 0; i < xy_bridging_elements_count; i++)
                {
                    //fnode_rel = string.Format("000000, 0, 0, 0, {0}, {1}, 0", 0, 0);
                    //bnode_rel = string.Format("         000000, 0, 0, 0, {0}, {1}, 0", 0, 0);
                    fnode_rel = string.Format("000011, 0, 0, 0, 0, {0}, {1}", My_i, Mz_i);
                    bnode_rel = string.Format("         000011, 0, 0, 0, 0, {0}, {1}", My_j, Mz_j);
                    buffer_string = string.Format("{0,-5},  YES,{1}\n{2},{3}", xy_bridging_elements[i].num, fnode_rel, bnode_rel, "剪刀撑-立杆");
                    writer.WriteLine(buffer_string);
                }

                for (int i = 0; i < xz_bridging_elements_count; i++)
                {
                    //fnode_rel = string.Format("000000, 0, 0, 0, {0}, {1}, 0", 0, 0);
                    //bnode_rel = string.Format("         000000, 0, 0, 0, {0}, {1}, 0", 0, 0);
                    fnode_rel = string.Format("000011, 0, 0, 0, 0, {0}, {1}", My_i, Mz_i);
                    bnode_rel = string.Format("         000011, 0, 0, 0, 0, {0}, {1}", My_j, Mz_j);
                    buffer_string = string.Format("{0,-5},  YES,{1}\n{2},{3}", xz_bridging_elements[i].num, fnode_rel, bnode_rel, "剪刀撑-立杆");
                    writer.WriteLine(buffer_string);
                }

                for (int i = 0; i < yz_bridging_elements_count; i++)
                {
                    //fnode_rel = string.Format("000000, 0, 0, 0, {0}, {1}, 0", 0, 0);
                    //bnode_rel = string.Format("         000000, 0, 0, 0, {0}, {1}, 0", 0, 0);
                    fnode_rel = string.Format("000011, 0, 0, 0, 0, {0}, {1}", My_i, Mz_i);
                    bnode_rel = string.Format("         000011, 0, 0, 0, 0, {0}, {1}", My_j, Mz_j);
                    buffer_string = string.Format("{0,-5},  YES,{1}\n{2},{3}", yz_bridging_elements[i].num, fnode_rel, bnode_rel, "剪刀撑-立杆");
                    writer.WriteLine(buffer_string);
                }
            }

            buffer_string = @"
*USE-STLD, 杆系自重
; *SELFWEIGHT, X, Y, Z, GROUP
*SELFWEIGHT, 0, 0, -1, 支架自重
; End of data for load case [杆系自重] -------------------------
*USE-STLD, 浇筑和振捣混凝土
";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            int P2_nodes_count = 0;
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005)
                {
                    P2_nodes_count++;
                }
            }

            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005)
                {
                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, P2 / P2_nodes_count, "浇筑及振捣");
                    writer.WriteLine(buffer_string);
                }
            }
            buffer_string = @"
; End of data for load case [浇筑和振捣混凝土] -------------------------
";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*USE-STLD, 施工人员、材料、设备

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP
";
            writer.WriteLine(buffer_string);
            int P1_nodes_count = P2_nodes_count;
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005)
                {
                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, P1 / P1_nodes_count, "施工人员机械");
                    writer.WriteLine(buffer_string);
                }
            }
            buffer_string = @"
; End of data for load case [施工人员、材料、设备] -------------------------
";
            writer.WriteLine(buffer_string);

            int section_width = 0;
            switch (selected_section)
            {
                case 0:
                    {
                        section_width = 2 * B01 + 2 * B02 + B03;
                    } break;
                case 1:
                    {
                        section_width = 2 * B01 + 2 * B02 + 2 * B03 + B04;
                    } break;
                case 2:
                    {
                        section_width = 2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + B05;
                    } break;
                case 3:
                    {
                        section_width = 2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06;
                    } break;
                default:
                    break;
            }
            double scaffold_width = x_points[x_input_count];
            double offset_scaffold = (scaffold_width - section_width / 100.0) / 2;


            buffer_string = @"
*USE-STLD,预压荷载1

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            string first_step_info = x_input_info.Substring(0, x_input_info.IndexOf(' '));
            string last_step_info = x_input_info.Substring(x_input_info.LastIndexOf(' ') + 1, x_input_info.Length - 1 - x_input_info.LastIndexOf(' '));
            //MessageBox.Show(first_step_info);
            //MessageBox.Show(last_step_info);
            int Y1_nodes_count = 0;
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 && (all_normal_nodes[i].x - offset_scaffold - B01 / 100.0) > -0.005 && (all_normal_nodes[i].x - (x_length - offset_scaffold - B01 / 100.0)) < 0.005)
                {
                    Y1_nodes_count++;
                }
            }
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 && (all_normal_nodes[i].x - offset_scaffold - B01 / 100.0) > -0.005 && (all_normal_nodes[i].x - (x_length - offset_scaffold - B01 / 100.0)) < 0.005)
                {
                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, Y1 * 1.1 * (G1 + G2) / Y1_nodes_count, "预压第一次");
                    writer.WriteLine(buffer_string);
                }
            }
            buffer_string = @"
; End of data for load case [预压荷载1] -------------------------
";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*USE-STLD,预压荷载2

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            int Y2_nodes_count = Y1_nodes_count;
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 && (all_normal_nodes[i].x - offset_scaffold - B01 / 100.0) > -0.005 && (all_normal_nodes[i].x - (x_length - offset_scaffold - B01 / 100.0)) < 0.005)
                {
                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, Y2 * 1.1 * (G1 + G2) / Y2_nodes_count, "预压第二次");
                    writer.WriteLine(buffer_string);
                }
            }
            buffer_string = @"
; End of data for load case [预压荷载2] -------------------------
";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*USE-STLD,预压荷载3

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            int Y3_nodes_count = Y1_nodes_count;
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 && (all_normal_nodes[i].x - offset_scaffold - B01 / 100.0) > -0.005 && (all_normal_nodes[i].x - (x_length - offset_scaffold - B01 / 100.0)) < 0.005)
                {
                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, Y3 * 1.1 * (G1 + G2) / Y3_nodes_count, "预压第三次");
                    writer.WriteLine(buffer_string);
                }
            }
            buffer_string = @"
; End of data for load case [预压荷载3] -------------------------
";
            writer.WriteLine(buffer_string);


            buffer_string = @"
*USE-STLD,第一次浇筑

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            int J1_web_nodes_count = 0;
            int J1_bottom_nodes_count = 0;
            switch (selected_section)
            {
                case 0:
                    {
                        for (int i = 0; i < bottom_nodes_end; i++)
                        {
                            if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 &&
                                all_normal_nodes[i].x > x_length / 2 - B03 / 200.0 - B02 / 100.0 - 0.005 &&
                                all_normal_nodes[i].x < x_length / 2 + B03 / 200.0 + B02 / 100.0 + 0.005)
                            {
                                if ((all_normal_nodes[i].x > x_length / 2 - B03 / 200.0 - B02 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B03 / 200.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B03 / 200.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B03 / 200.0 + B02 / 100.0 + 0.005))
                                {
                                    J1_web_nodes_count++;
                                }
                                else
                                {
                                    J1_bottom_nodes_count++;
                                }
                            }
                        }
                    } break;
                case 1:
                    {
                        for (int i = 0; i < bottom_nodes_end; i++)
                        {
                            if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 &&
                                all_normal_nodes[i].x > x_length / 2 - B04 / 200.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                all_normal_nodes[i].x < x_length / 2 + B04 / 200.0 + B03 / 100.0 + B02 / 100.0 + 0.005)
                            {
                                if ((all_normal_nodes[i].x > x_length / 2 - B04 / 200.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B04 / 200.0 - B03 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 - B04 / 200.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B04 / 200.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B04 / 200.0 + B03 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B04 / 200.0 + B03 / 100.0 + B02 / 100.0 + 0.005))
                                {
                                    J1_web_nodes_count++;
                                }
                                else
                                {
                                    J1_bottom_nodes_count++;
                                }
                            }
                        }
                    } break;
                case 2:
                    {
                        for (int i = 0; i < bottom_nodes_end; i++)
                        {
                            if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 &&
                                all_normal_nodes[i].x > x_length / 2 - B05 / 200.0 - B04 / 100.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                all_normal_nodes[i].x < x_length / 2 + B05 / 200.0 + B04 / 100.0 + B03 / 100.0 + B02 / 100.0 + 0.005)
                            {
                                if ((all_normal_nodes[i].x > x_length / 2 - B05 / 200.0 - B04 / 100.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B05 / 200.0 - B04 / 100.0 - B03 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 - B05 / 200.0 - B04 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B05 / 200.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B05 / 200.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B05 / 200.0 + B04 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B05 / 200.0 + B04 / 100.0 + B03 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B05 / 200.0 + B04 / 100.0 + B03 / 100.0 + B02 / 100.0 + 0.005))
                                {
                                    J1_web_nodes_count++;
                                }
                                else
                                {
                                    J1_bottom_nodes_count++;
                                }
                            }
                        }
                    } break;
                case 3:
                    {
                        for (int i = 0; i < bottom_nodes_end; i++)
                        {
                            if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 &&
                                all_normal_nodes[i].x > x_length / 2 - B06 / 200.0 - B05 / 100.0 - B04 / 100.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                all_normal_nodes[i].x < x_length / 2 + B06 / 200.0 + B05 / 100.0 + B04 / 100.0 + B03 / 100.0 + B02 / 100.0 + 0.005)
                            {
                                if ((all_normal_nodes[i].x > x_length / 2 - B06 / 200.0 - B05 / 100.0 - B04 / 100.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B06 / 200.0 - B05 / 100.0 - B04 / 100.0 - B03 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 - B06 / 200.0 - B05 / 100.0 - B04 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B06 / 200.0 - B05 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 - B06 / 200.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B06 / 200.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B06 / 200.0 + B05 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B06 / 200.0 + B05 / 100.0 + B04 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B06 / 200.0 + B05 / 100.0 + B04 / 100.0 + B03 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B06 / 200.0 + B05 / 100.0 + B04 / 100.0 + B03 / 100.0 + B02 / 100.0 + 0.005))
                                {
                                    J1_web_nodes_count++;
                                }
                                else
                                {
                                    J1_bottom_nodes_count++;
                                }
                            }
                        }
                    } break;
                default: break;
            }

            switch (selected_section)
            {
                case 0:
                    {
                        for (int i = 0; i < bottom_nodes_end; i++)
                        {
                            if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 &&
                               all_normal_nodes[i].x > x_length / 2 - B03 / 200.0 - B02 / 100.0 - 0.005 &&
                               all_normal_nodes[i].x < x_length / 2 + B03 / 200.0 + B02 / 100.0 + 0.005)
                            {
                                if ((all_normal_nodes[i].x > x_length / 2 - B03 / 200.0 - B02 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B03 / 200.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B03 / 200.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B03 / 200.0 + B02 / 100.0 + 0.005))
                                {
                                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J1 * G1 / (J1_bottom_nodes_count + J1_web_nodes_count), "浇筑第一次-腹板");
                                    writer.WriteLine(buffer_string);
                                }
                                else
                                {
                                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J1 * G1 / (J1_bottom_nodes_count + J1_web_nodes_count), "浇筑第一次-底板");
                                    writer.WriteLine(buffer_string);
                                }
                            }
                        }
                    } break;
                case 1:
                    {
                        for (int i = 0; i < bottom_nodes_end; i++)
                        {
                            if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 &&
                                 all_normal_nodes[i].x > x_length / 2 - B04 / 200.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                 all_normal_nodes[i].x < x_length / 2 + B04 / 200.0 + B03 / 100.0 + B02 / 100.0 + 0.005)
                            {
                                if ((all_normal_nodes[i].x > x_length / 2 - B04 / 200.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B04 / 200.0 - B03 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 - B04 / 200.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B04 / 200.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B04 / 200.0 + B03 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B04 / 200.0 + B03 / 100.0 + B02 / 100.0 + 0.005))
                                {
                                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J1 * G1 / (J1_bottom_nodes_count + J1_web_nodes_count), "浇筑第一次-腹板");
                                    writer.WriteLine(buffer_string);
                                }
                                else
                                {
                                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J1 * G1 / (J1_bottom_nodes_count + J1_web_nodes_count), "浇筑第一次-底板");
                                    writer.WriteLine(buffer_string);
                                }
                            }
                        }
                    } break;
                case 2:
                    {
                        for (int i = 0; i < bottom_nodes_end; i++)
                        {
                            if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 &&
                                all_normal_nodes[i].x > x_length / 2 - B05 / 200.0 - B04 / 100.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                all_normal_nodes[i].x < x_length / 2 + B05 / 200.0 + B04 / 100.0 + B03 / 100.0 + B02 / 100.0 + 0.005)
                            {
                                if ((all_normal_nodes[i].x > x_length / 2 - B05 / 200.0 - B04 / 100.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B05 / 200.0 - B04 / 100.0 - B03 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 - B05 / 200.0 - B04 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B05 / 200.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B05 / 200.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B05 / 200.0 + B04 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B05 / 200.0 + B04 / 100.0 + B03 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B05 / 200.0 + B04 / 100.0 + B03 / 100.0 + B02 / 100.0 + 0.005))
                                {
                                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J1 * G1 / (J1_bottom_nodes_count + J1_web_nodes_count), "浇筑第一次-腹板");
                                    writer.WriteLine(buffer_string);
                                }
                                else
                                {
                                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J1 * G1 / (J1_bottom_nodes_count + J1_web_nodes_count), "浇筑第一次-底板");
                                    writer.WriteLine(buffer_string);
                                }
                            }
                        }
                    } break;
                case 3:
                    {
                        for (int i = 0; i < bottom_nodes_end; i++)
                        {
                            if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005 &&
                                all_normal_nodes[i].x > x_length / 2 - B06 / 200.0 - B05 / 100.0 - B04 / 100.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                all_normal_nodes[i].x < x_length / 2 + B06 / 200.0 + B05 / 100.0 + B04 / 100.0 + B03 / 100.0 + B02 / 100.0 + 0.005)
                            {
                                if ((all_normal_nodes[i].x > x_length / 2 - B06 / 200.0 - B05 / 100.0 - B04 / 100.0 - B03 / 100.0 - B02 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B06 / 200.0 - B05 / 100.0 - B04 / 100.0 - B03 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 - B06 / 200.0 - B05 / 100.0 - B04 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 - B06 / 200.0 - B05 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 - B06 / 200.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B06 / 200.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B06 / 200.0 + B05 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B06 / 200.0 + B05 / 100.0 + B04 / 100.0 + 0.005) ||
                                    (all_normal_nodes[i].x > x_length / 2 + B06 / 200.0 + B05 / 100.0 + B04 / 100.0 + B03 / 100.0 - 0.005 &&
                                    all_normal_nodes[i].x < x_length / 2 + B06 / 200.0 + B05 / 100.0 + B04 / 100.0 + B03 / 100.0 + B02 / 100.0 + 0.005))
                                {
                                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J1 * G1 / (J1_bottom_nodes_count + J1_web_nodes_count), "浇筑第一次-腹板");
                                    writer.WriteLine(buffer_string);
                                }
                                else
                                {
                                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J1 * G1 / (J1_bottom_nodes_count + J1_web_nodes_count), "浇筑第一次-底板");
                                    writer.WriteLine(buffer_string);
                                }
                            }
                        }
                    } break;
                default: break;
            }
            buffer_string = @"
; End of data for load case [第一次浇筑] -------------------------";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*USE-STLD,第二次浇筑

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            int J2_nodes_count = 0;
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005)
                {
                    J2_nodes_count++;
                }
            }
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005)
                {
                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, J2 * G1 / J2_nodes_count, "浇筑第二次");
                    writer.WriteLine(buffer_string);
                }
            }

            buffer_string = @"
; End of data for load case [第二次浇筑] -------------------------";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*USE-STLD,模板、支撑梁

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            int G2_nodes_count = J2_nodes_count;
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005)
                {
                    buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, G2 / G2_nodes_count, "模板方楞等");
                    writer.WriteLine(buffer_string);
                }
            }

            buffer_string = @"
; End of data for load case [模板、支撑梁] -------------------------";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*USE-STLD,防护设施、附加构件

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            int G3_nodes_count = 0;
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005)
                {
                    if (all_normal_nodes[i].x <= x_points[0] + 0.005 || all_normal_nodes[i].x >= x_points[x_input_count] - 0.005
                        || all_normal_nodes[i].y <= y_points[0] + 0.005 || all_normal_nodes[i].y >= y_points[y_input_count] - 0.005)
                        G3_nodes_count++;
                    //buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.00} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, G3 / G3_nodes_count, "附加构件");
                    //writer.WriteLine(buffer_string);
                }
            }
            for (int i = 0; i < bottom_nodes_end; i++)
            {
                if (Math.Abs(G3 / G3_nodes_count) < 0.0001)
                    break;
                if (Math.Abs(all_normal_nodes[i].z - z_length) < 0.005)
                {
                    if (all_normal_nodes[i].x <= x_points[0] + 0.005 || all_normal_nodes[i].x >= x_points[x_input_count] - 0.005
                        || all_normal_nodes[i].y <= y_points[0] + 0.005 || all_normal_nodes[i].y >= y_points[y_input_count] - 0.005)
                    {
                        buffer_string = string.Format("{0,-5},  0.00 , 0.00 , -{1:0.0000} , 0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, G3 / G3_nodes_count, "附加构件");
                        writer.WriteLine(buffer_string);
                    }
                }
            }

            buffer_string = @"
; End of data for load case [防护设施、附加构件] -------------------------";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*USE-STLD,风荷载

*CONLOAD    ; Nodal Loads
; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP";
            writer.WriteLine(buffer_string);
            int P3_nodes_count = 0;
            if (h0 < 0.005)
            {
                for (int i = 0; i < bottom_nodes_end; i++)
                {
                    if (Math.Abs(all_normal_nodes[i].x - x_length) < 0.005)
                    {
                        if (all_normal_nodes[i].z <= z_points[z_input_count - 1] + 0.005 && all_normal_nodes[i].z >= z_points[2] - 0.005)
                        {
                            P3_nodes_count++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < bottom_nodes_end; i++)
                {
                    if (Math.Abs(all_normal_nodes[i].x - x_length) < 0.005)
                    {
                        if (all_normal_nodes[i].z <= z_points[z_input_count - 2] + 0.005 && all_normal_nodes[i].z >= z_points[2] - 0.005)
                        {
                            P3_nodes_count++;
                        }
                    }
                }
            }
            if (Math.Abs(P3 / P3_nodes_count) >= 0.0001)
            {
                if (h0 < 0.005)
                {
                    for (int i = 0; i < bottom_nodes_end; i++)
                    {
                        if (Math.Abs(all_normal_nodes[i].x - x_length) < 0.005)
                        {
                            if (all_normal_nodes[i].z <= z_points[z_input_count - 1] + 0.005 && all_normal_nodes[i].z >= z_points[2] - 0.005)
                            {
                                buffer_string = string.Format("{0,-5},  -{1:0.0000} ,0.00, 0.00 ,  0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, P3 / P3_nodes_count, "风荷载");
                                writer.WriteLine(buffer_string);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < bottom_nodes_end; i++)
                    {
                        if (Math.Abs(all_normal_nodes[i].x - x_length) < 0.005)
                        {
                            if (all_normal_nodes[i].z <= z_points[z_input_count - 2] + 0.005 && all_normal_nodes[i].z >= z_points[2] - 0.005)
                            {
                                buffer_string = string.Format("{0,-5},  -{1:0.00} ,0.00, 0.00 ,  0.00 , 0.00 , 0.00 , {2}", all_normal_nodes[i].num, P3 / P3_nodes_count, "风荷载");
                                writer.WriteLine(buffer_string);
                            }
                        }
                    }
                }
            }


            buffer_string = @"
; End of data for load case [风荷载] -------------------------";
            writer.WriteLine(buffer_string);

            buffer_string = @"
*LOADCOMB    ; Combinations
; NAME=NAME, KIND, ACTIVE, bES, iTYPE, DESC, iSERV-TYPE, nLCOMTYPE   ; line 1
;      ANAL1, LCNAME1, FACT1, ...                                    ; from line 2
   NAME=第一次预压, GEN, ACTIVE, 0, 0, , 0, 0
        ST,杆系自重, 1, ST, 预压荷载1, 1, ST, 模板、支撑梁, 1
        ST, 施工人员、材料、设备, 1, ST, 风荷载, 1
   NAME=第二次预压, GEN, ACTIVE, 0, 0, , 0, 0
        ST,杆系自重, 1, ST, 预压荷载2, 1, ST, 模板、支撑梁, 1
        ST, 施工人员、材料、设备, 1, ST, 风荷载, 1
   NAME=第三次预压, GEN, ACTIVE, 0, 0, , 0, 0
        ST,杆系自重, 1, ST, 预压荷载3, 1, ST, 模板、支撑梁, 1
        ST, 施工人员、材料、设备, 1, ST, 风荷载, 1
   NAME=浇筑前, GEN, ACTIVE, 0, 0, , 0, 0
        ST, 杆系自重, 1, ST, 施工人员、材料、设备, 1, ST, 模板、支撑梁, 1
        ST, 防护设施、附加构件, 1, ST, 风荷载, 1
   NAME=第一次浇筑, GEN, ACTIVE, 0, 0, , 0, 0
        ST, 杆系自重, 1, ST, 第一次浇筑, 1, ST, 浇筑和振捣混凝土, 1
        ST, 施工人员、材料、设备, 1, ST, 模板、支撑梁, 1
        ST, 防护设施、附加构件, 1, ST, 风荷载, 1
   NAME=第二次浇筑, GEN, ACTIVE, 0, 0, , 0, 0
        ST, 杆系自重, 1, ST,第一次浇筑, 1,ST,第二次浇筑, 1
        ST, 浇筑和振捣混凝土, 1, ST, 施工人员、材料、设备, 1
        ST, 模板、支撑梁, 1, ST, 防护设施、附加构件, 1, ST, 风荷载, 1

*LC-COLOR    ; Diagram Color for Load Case
; ANAL, LCNAME, iR1(ALL), iG1(ALL), iB1(ALL), iR2(MIN), iG2(MIN), iB2(MIN), iR3(MAX), iG2(MAX), iB2(MAX)
 ST, 预压荷载1, 0, 192, 192, 0, 128, 57, 255, 255, 87
 ST, 预压荷载2, 163, 255, 160, 210, 210, 210, 0, 128, 192
 ST, 预压荷载3, 93, 255, 87, 0, 128, 192, 0, 192, 192
 ST, 杆系自重, 160, 192, 255, 148, 87, 255, 0, 192, 128
 ST, 模板、支撑梁, 255, 160, 255, 146, 0, 255, 163, 255, 160
 ST, 浇筑和振捣混凝土, 0, 192, 128, 0, 128, 192, 0, 192, 128
 ST, 施工人员、材料、设备, 148, 87, 255, 192, 128, 0, 192, 128, 0
 CB, 第一次预压, 192, 192, 0, 0, 192, 192, 192, 192, 192
 CB, 第二次预压, 78, 0, 255, 0, 157, 192, 160, 255, 255
 CB, 第三次预压, 192, 0, 128, 192, 0, 128, 93, 255, 87
 CB, 浇筑前, 160, 160, 255, 93, 255, 87, 192, 192, 192
 CB, 第一次浇筑, 255, 160, 255, 210, 210, 210, 255, 0, 128
 ST, 第一次浇筑, 163, 160, 255, 93, 255, 87, 192, 192, 192
 CB, 第二次浇筑, 192, 72, 0, 148, 87, 255, 192, 0, 192
 ST, 第二次浇筑, 192, 0, 192, 0, 128, 255, 212, 160, 255
 ST, 防护设施、附加构件, 0, 128, 57, 0, 192, 128, 0, 192, 128
 ST, 风荷载, 255, 128, 0, 192, 0, 192, 85, 0, 192

*DGN-MATL    ; Modify Steel(Concrete) Material
; iMAT, TYPE, MNAME, [DATA1]                                    ; STEEL
; iMAT, TYPE, MNAME, [DATA2], [R-DATA], FCI, bSERV, SHORT, LONG ; CONC
; iMAT, TYPE, MNAME, [DATA3], [DATA2], [R-DATA]                 ; SRC
; iMAT, TYPE, MNAME, [DATA5]                                    ; STEEL(None) & KSCE-ASD05
; [DATA1] : 1, DB, CODE, NAME or 2, ELAST, POISN, FU, FY1, FY2, FY3, FY4
;           FY5, FY6, AFT, AFT2, AFT3, FY, AFV, AFV2, AFV3
; [DATA2] : 1, DB, CODE, NAME or 2, FC
; [DATA3] : 1, DB, CODE, NAME or 2, ELAST, FU, FY1, FY2, FY3, FY4
;              FY5, FY6, AFT, AFT2, AFT3, FY, AFV, AFV2, AFV3
; [DATA4] : 1, DB, CODE, NAME or 2, FC
; [DATA5] : 3, ELAST, POISN, AL1, AL2, AL3, AL4, AL5, AL6, AL7, AL8, AL9, AL10
;              MIN1, MIN2, MIN3
; [R-DATA]: RBCODE, RBMAIN, RBSUB, FY(R), FYS
    1, STEEL, Q235              , 1, GB03(S)    ,            ,Q235
";
            writer.WriteLine(buffer_string);

            if (buckle_analyse)
            {
                buffer_string = "*BUCK-CTRL    ; Buckling Analysis Control";
                writer.WriteLine(buffer_string);
                buffer_string = "; iMODENUM, bPOSITIVE, bAXIALONLY, RNG1, RNG2, bSTURM    ; line 1";
                writer.WriteLine(buffer_string);
                buffer_string = "; LCNAME1, FACT1, iLTYPE1, LCNAME2, FACT2, iLTYPE2, ...  ; from line 2";
                writer.WriteLine(buffer_string);
                buffer_string = "5, NO, NO, 0, 0, NO";
                writer.WriteLine(buffer_string);
                buffer_string = "杆系自重, 1.0, 1, 第二次浇筑, 1.0, 0";
                writer.WriteLine(buffer_string);
            }
            if (nolinear_analyse)
            {
                buffer_string = "*NONL-CTRL    ; Non-linear Analysis Control";
                writer.WriteLine(buffer_string);
                buffer_string = "; TYPE, ITER, LSTEP, MAX, [CONV_CRITERIA]                      ; ITER=NEWTON";
                writer.WriteLine(buffer_string);
                buffer_string = "; STLD, LSTEP, MITER, FACT1, FACT2, ...                        ; from line 2";
                writer.WriteLine(buffer_string);
                buffer_string = "; TYPE, ITER, IFR, NINC, MITER, MDISP, [CONV_CRITERIA]         ; ITER=ARC";
                writer.WriteLine(buffer_string);
                buffer_string = "; LCNAME, NINC, MITER, MDISP                                   ; from line 2";
                writer.WriteLine(buffer_string);
                buffer_string = "; TYPE, ITER, DSTEP, MITER, MNODE, DIR, MDISP, [CONV_CRITERIA] ; ITER=DISP";
                writer.WriteLine(buffer_string);
                buffer_string = "; LCNAME, DSTEP, MITER, MNODE, DIR, MDISP                      ; from line 2";
                writer.WriteLine(buffer_string);
                buffer_string = "; [CONV_CRITERIA] : bENGR, EV, bDISP, DV, bFORC, FV";
                writer.WriteLine(buffer_string);
                buffer_string = "   GEOM, NEWTON, 1, 30, NO, 0.001, YES, 0.001, NO, 0.001";
                writer.WriteLine(buffer_string);
                buffer_string = "     杆系自重, 1, 30, 1";
                writer.WriteLine(buffer_string);
                //buffer_string = "     预压荷载1, 1, 30, 1";
                //writer.WriteLine(buffer_string);
                //buffer_string = "     预压荷载2, 1, 30, 1";
                //writer.WriteLine(buffer_string);
                //buffer_string = "     预压荷载3, 1, 30, 1";
                //writer.WriteLine(buffer_string);
                buffer_string = "     第一次浇筑, 1, 30, 1";
                writer.WriteLine(buffer_string);
                //buffer_string = "     第二次浇筑, 1, 30, 1";
                //writer.WriteLine(buffer_string);
                //buffer_string = "     模板、支撑梁, 1, 30, 1";
                //writer.WriteLine(buffer_string);
                //buffer_string = "     浇筑和振捣混凝土, 1, 30, 1";
                //writer.WriteLine(buffer_string);
                //buffer_string = "     施工人员、材料、设备, 1, 30, 1";
                //writer.WriteLine(buffer_string);
                //buffer_string = "     风荷载, 1, 30, 1";
                //writer.WriteLine(buffer_string);

            }
            //buffer_string = String.Format("\r\n;XY剪刀线 ");
            //writer.WriteLine(buffer_string);
            //for (int i = 0; i < xy_bridging_lines_count; i++)
            //{//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
            //    buffer_string = string.Format("{0},{1},{2},{3}", xy_bridging_lines[i].fNode.x, xy_bridging_lines[i].fNode.y, xy_bridging_lines[i].bNode.x, xy_bridging_lines[i].bNode.y);
            //    writer.WriteLine(buffer_string);
            //}
            //buffer_string = String.Format("\r\n;XZ剪刀线 ");
            //writer.WriteLine(buffer_string);
            //for (int i = 0; i < xz_bridging_lines_count; i++)
            //{//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
            //    buffer_string = string.Format("{0},{1},{2},{3}", xz_bridging_lines[i].fNode.x, xz_bridging_lines[i].fNode.z, xz_bridging_lines[i].bNode.x, xz_bridging_lines[i].bNode.z);
            //    writer.WriteLine(buffer_string);
            //}
            //buffer_string = String.Format("\r\n;YZ剪刀线 ");
            //writer.WriteLine(buffer_string);
            //for (int i = 0; i < yz_bridging_lines_count; i++)
            //{//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
            //    buffer_string = string.Format("{0},{1},{2},{3}", yz_bridging_lines[i].fNode.y, yz_bridging_lines[i].fNode.z, yz_bridging_lines[i].bNode.y, yz_bridging_lines[i].bNode.z);
            //    writer.WriteLine(buffer_string);
            //}
            //buffer_string = String.Format("\r\n;剪刀线生成开始");
            //writer.WriteLine(buffer_string);
            //for (int i = 0; i < xy_bridging_lines_count; i++)
            //{
            //    buffer_string = string.Format("{0,-5},{1:0.00},{2:0.00},{3:0.00}", bottom_nodes[i].num,
            //        bottom_nodes[i].x, bottom_nodes[i].y, bottom_nodes[i].z);
            //    writer.WriteLine(buffer_string);
            //}
            //xy_bridging_lines[xy_bridging_lines_count]
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
                mctfile.Close();
                mctfile.Dispose();
            }
            SetProcess(100);
            if (Math.Abs(P3 / P3_nodes_count) < 0.0001)
                SetText("写入完成,风荷载不足0.01,未添加", this.status_bar_text);
            else
                SetText("写入完成", this.status_bar_text);
        }
        private void btn_section_Click(object sender, RoutedEventArgs e)
        {
            //this.status_bar_text.Text = "23";
            //PostMessage(3);
            //PostMessage(2);
            //PostMessage(1);
            //MessageBox.Show("123");
            if (!section_form_closed)
            {
                section_form.Show();
            }
            else
            {
                section_form = new SectionForm(this);
                section_form.Show();
                this.section_form_closed = false;
            }

        }
        private void msgFunction_2()
        {
            getXmlValue();
        }
        private void setXmlValue()
        {
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "mainconfig.xml";
            //MessageBox.Show(XmlHelper.getXmlElementValue(xmlpath, "内部轮廓", "h11"));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "纵距x", GetText(text_x_input));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "横距y", GetText(text_y_input));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "步距z", GetText(text_z_input));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "h0", GetText(text_h0));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "h1", GetText(text_h1));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "h2", GetText(text_h2));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "h3", GetText(text_h3));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "l1", GetText(text_l1));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "l2", GetText(text_l2));

            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "G1", GetText(text_G1));
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "G2", GetText(text_G2));
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "G3", GetText(text_G3));
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "P1", GetText(text_P1));
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "P2", GetText(text_P2));
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "P3", GetText(text_P3));

            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "Y1", GetText(text_Y1));
            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "Y2", GetText(text_Y2));
            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "Y3", GetText(text_Y3));
            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "J1", GetText(text_J1));
            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "J2", GetText(text_J2));
            XmlHelper.setXmlElementValue(xmlpath, "缺陷参数", "标准壁厚", wall_thickness.ToString());
            XmlHelper.setXmlElementValue(xmlpath, "缺陷参数", "标准管径", calibre.ToString());

            XmlHelper.setXmlElementValue(xmlpath, "刚度设置", "My_i_cut", GetText(text_My_i_cut));
            XmlHelper.setXmlElementValue(xmlpath, "刚度设置", "My_j_cut", GetText(text_My_j_cut));
            XmlHelper.setXmlElementValue(xmlpath, "刚度设置", "Mz_i_cut", GetText(text_Mz_i_cut));
            XmlHelper.setXmlElementValue(xmlpath, "刚度设置", "Mz_j_cut", GetText(text_Mz_j_cut));

            XmlHelper.setXmlElementValue(xmlpath, "刚度设置", "My_i_normal", GetText(text_My_i_normal));
            XmlHelper.setXmlElementValue(xmlpath, "刚度设置", "My_j_normal", GetText(text_My_j_normal));
            XmlHelper.setXmlElementValue(xmlpath, "刚度设置", "Mz_i_normal", GetText(text_Mz_i_normal));
            XmlHelper.setXmlElementValue(xmlpath, "刚度设置", "Mz_j_normal", GetText(text_Mz_j_normal));
        }

        private void getXmlValue()
        {
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "mainconfig.xml";
            //MessageBox.Show(XmlHelper.getXmlElementValue(xmlpath, "内部轮廓", "h11"));
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "纵距x"), text_x_input);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "横距y"), text_y_input);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "步距z"), text_z_input);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "h0"), text_h0);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "h1"), text_h1);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "h2"), text_h2);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "h3"), text_h3);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "l1"), text_l1);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "支架参数", "l2"), text_l2);

            SetText(XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "G1"), text_G1);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "G2"), text_G2);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "G3"), text_G3);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "P1"), text_P1);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "P2"), text_P2);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "P3"), text_P3);

            SetText(XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "Y1"), text_Y1);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "Y2"), text_Y2);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "Y3"), text_Y3);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "J1"), text_J1);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "J2"), text_J2);

            SetText(XmlHelper.getXmlElementValue(xmlpath, "刚度设置", "My_i_cut"), text_My_i_cut);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "刚度设置", "My_j_cut"), text_My_j_cut);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "刚度设置", "Mz_i_cut"), text_Mz_i_cut);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "刚度设置", "Mz_j_cut"), text_Mz_j_cut);

            SetText(XmlHelper.getXmlElementValue(xmlpath, "刚度设置", "My_i_normal"), text_My_i_normal);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "刚度设置", "My_j_normal"), text_My_j_normal);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "刚度设置", "Mz_i_normal"), text_Mz_i_normal);
            SetText(XmlHelper.getXmlElementValue(xmlpath, "刚度设置", "Mz_j_normal"), text_Mz_j_normal);

            try
            {
                wall_thickness = double.Parse(XmlHelper.getXmlElementValue(xmlpath, "缺陷参数", "标准壁厚"));
                calibre = double.Parse(XmlHelper.getXmlElementValue(xmlpath, "缺陷参数", "标准管径"));
            }
            catch (Exception)
            {
                if (wall_thickness <= 0.0)
                {
                    wall_thickness = 0.0035;
                }
                if (calibre <= 0.0)
                {
                    calibre = 0.048;
                }

                throw;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Random ran = new Random();
            Normal nor = new Normal(0, 1);
            double[] sample = new double[1000];
            nor.Samples(sample);
            double sum = 0.0;
            for (int i = 0; i < sample.Length; i++)
            {
                sum = sum + sample[i];
            }
        }
        private void btn_export_mct_Click(object sender, RoutedEventArgs e)
        {
            //PostMessage(1);//获取x输入框数组
            //PostMessage(2);//获取y输入框数组
            //PostMessage(3);//获取z输入框数组
            if (!section_selected_flag)
            {
                this.status_bar_text.Text = "未确认截面参数，无法建模，请确认界面参数";
                return;
            }
            this.status_bar_text.Text = "模型创建中...";
            x_input_info = this.text_x_input.Text;
            PostMessage(1);//存储文件对话框
        }

        private void text_h0_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                h0 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                h0 = 0.0;
            }
        }

        private void text_h2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                h2 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                h2 = 0.0;
            }
            //h2 = double.Parse(((TextBox)sender).Text);
        }

        private void text_h1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                h1 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                h1 = 0.0;
            }
            //h1 = double.Parse(((TextBox)sender).Text);
        }

        private void text_h3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                h3 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                h3 = 0.0;
            }
            //h3 = double.Parse(((TextBox)sender).Text);
        }

        private void text_l1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                l1 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                l1 = 0.0;
            }
            //l1 = double.Parse(((TextBox)sender).Text);
        }

        private void text_l2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                l2 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                l2 = 0.0;
            }
            //l2 = double.Parse(((TextBox)sender).Text);
        }

        private void text_G1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                G1 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                G1 = 0.0;
            }
            //G1 = double.Parse(((TextBox)sender).Text);
        }

        private void text_G2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                G2 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                G2 = 0.0;
            }
            //G2 = double.Parse(((TextBox)sender).Text);
        }

        private void text_G3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                G3 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                G3 = 0.0;
            }
            //G3 = double.Parse(((TextBox)sender).Text);
        }

        private void text_P1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                P1 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                P1 = 0.0;
            }
            //P1 = double.Parse(((TextBox)sender).Text);
        }

        private void text_P2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                P2 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                P2 = 0.0;
            }
            //P2 = double.Parse(((TextBox)sender).Text);
        }

        private void text_P3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                P3 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                P3 = 0.0;
            }
            //P3 = double.Parse(((TextBox)sender).Text);
        }

        private void text_Y1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Y1 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                Y1 = 0.0;
            }
            //Y1 = double.Parse(((TextBox)sender).Text);
        }

        private void text_Y2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Y2 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                Y2 = 0.0;
            }
            //Y2 = double.Parse(((TextBox)sender).Text);
        }

        private void text_Y3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Y3 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                Y3 = 0.0;
            }
            //Y3 = double.Parse(((TextBox)sender).Text);
        }

        private void text_J1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                J1 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                J1 = 0.0;
            }
            finally
            {
                text_J2.Text = (1 - J1).ToString();
            }
            //J1 = double.Parse(((TextBox)sender).Text);
        }

        private void text_J2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                J2 = double.Parse(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                J2 = 0.0;
            }
        }

        private void check_buckle_Checked(object sender, RoutedEventArgs e)
        {
            buckle_analyse = (bool)check_buckle.IsChecked;
            if (buckle_analyse && nolinear_analyse)
            {
                check_nolinear.IsChecked = false;
                nolinear_analyse = false;
            }
        }

        private void check_nolinear_Checked(object sender, RoutedEventArgs e)
        {
            nolinear_analyse = (bool)check_nolinear.IsChecked;
            if (nolinear_analyse && buckle_analyse)
            {
                check_buckle.IsChecked = false;
                buckle_analyse = false;
            }
        }

        private void btn_defect_Click(object sender, RoutedEventArgs e)
        {
            if (!defect_form_closed)
            {
                defect_form.Show();
            }
            else
            {
                defect_form = new DefectForm(this);
                defect_form.Show();
                this.defect_form_closed = false;
            }
        }

        private void btn_cacul_Click(object sender, RoutedEventArgs e)
        {
            PcrForm Cacul_Form = new PcrForm();
            Cacul_Form.Show();
        }

        private void check_cut_Checked(object sender, RoutedEventArgs e)
        {


        }

        private void check_cut_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)check_cut.IsChecked)
            {
                this.bridging_check = false;
                text_l1.IsEnabled = false;
                text_l2.IsEnabled = false;
            }
            else
            {
                this.bridging_check = true;
                text_l1.IsEnabled = true;
                text_l2.IsEnabled = true;
            }
        }

    }
}
