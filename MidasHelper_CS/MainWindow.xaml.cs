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
//using System.Windows.Forms;


namespace MidasHelper_CS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackThreader threader = null;
        private Queue<int> msgQueue = null;
        //public TextBlock status_bar_text = null;
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
        //private Mctxt mctfile = null;
        private FileStream mctfile = null;
        public delegate object delegateGetTextCallBack(object text);
        public delegate void delegateSetTextCallBack(string str, object object_id);
        private SectionForm section_form = null;
        public bool section_from_closed = true;
        private double h0, h1, h2, h3, l1, l2, G1, G2, G3, P1, P2, P3, Y1, Y2, Y3, J1, J2;
        public MainWindow()
        {
            InitializeComponent();
            threader = new BackThreader(this);
            backThreader = new BackgroundWorker();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //status_bar_text = new TextBlock();
            //BottomStatusBar.Items.Add(status_bar_text);
            //BottomStatusBar.Items.GetItemAt(0)
            x_input = new double[50];
            y_input = new double[50];
            z_input = new double[50];
            msgQueue = new Queue<int>();
            //section_form = new SectionForm();
            log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
            InitializeBackgroundWorker();
            getXmlValue();
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
                //case 2:
                //    {
                //        msgFunction_2();//例如消息码为2是，执行msgFunction_2()函数
                //    } break;
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
        /// <summary>
        /// 获取x输入框数组
        /// </summary>
        private void get_x_info()
        {
            string x_str = "";
            x_input_count = 0;
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

            x_bridging_points = new double[100];
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

            y_bridging_points = new double[100];
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
            z_input[z_input_count++] = h0;
            z_points = new double[z_input_count + 1];
            z_points[0] = 0.0;
            for (int i = 1; i < z_input_count + 1; i++)
            {
                z_points[i] = z_points[i - 1] + z_input[i - 1];
            }
            z_length = z_points[z_points.Length - 1];

            z_bridging_points = new double[100];
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
            bool judge7 = Math.Abs(bridging_line.fNode.x - bridging_line.fNode.x) < 0.005;
            bool judge8 = Math.Abs(bridging_line.fNode.y - bridging_line.fNode.y) < 0.005;
            bool judge9 = Math.Abs(bridging_line.fNode.z - bridging_line.fNode.z) < 0.005;
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
            result = new MidasNode();
            double res1 = ((b1 - y1) * (x1 - x2) - (y1 - y2) * (a1 - x1)) * ((b2 - y1) * (x1 - x2) - (y1 - y2) * (a2 - x1));
            double res2 = ((y1 - b1) * (a1 - a2) - (b1 - b2) * (x1 - a1)) * ((y2 - b1) * (a1 - a2) - (b1 - b2) * (x2 - a1));
            if (res1 < 0.005 && res2 < 0.005)
            {
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
            }
            if (judge7)
            {
                result.num = 1;
                result.y = res1;
                result.z = res2;
            }
            else if (judge8)
            {
                result.num = 1;
                result.x = res1;
                result.z = res2;
            }
            else if (judge9)
            {
                result.num = 1;
                result.x = res1;
                result.y = res2;
            }
            else
                return null;
            return result;
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
            if (Math.Abs(J1) < 0.005)
            {
                MessageBox.Show("第一次浇筑荷载系数输入格式不对或输入值过小");
                return;
            }
            if (Math.Abs(J2) < 0.005)
            {
                MessageBox.Show("第二次浇筑荷载系数输入格式不对或输入值过小");
                return;
            }
            if (J1 + J2 > 1.0)
            {
                MessageBox.Show("两次浇筑荷载系数和必须为1");
                return;
            }
            #endregion

            get_x_info();//生成x间距数组和点坐标数组
            get_y_info();//生成y间距数组和点坐标数组
            get_z_info();//生成z间距数组和点坐标数组
            if (x_input_count == 0 || y_input_count == 0 || z_input_count == 0)
            {
                MessageBox.Show("支架参数获取不足");
                return;
            }
            //MidasNode node = new MidasNode();
            MidasNode[] all_normal_nodes = new MidasNode[10000];//未计入底层扫地杆节点
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
            MidasNode[] bottom_nodes = new MidasNode[500];//扫地杆节点
            for (int i = 0; i < bottom_nodes.Length; i++)
            {
                bottom_nodes[i] = new MidasNode();
            }
            int bottom_nodes_count = 0;
            for (int i = 0; i < x_points.Length; i++)//沿x方向y=0扫地杆节点
            {
                bottom_nodes[bottom_nodes_count].num = normal_nodes_count + bottom_nodes_count + 1;
                bottom_nodes[bottom_nodes_count].x = x_points[i];
                bottom_nodes[bottom_nodes_count].y = 0.0;
                bottom_nodes[bottom_nodes_count].z = h2;
                //all_normal_nodes[normal_nodes_count++] = bottom_nodes[bottom_nodes_count];
                bottom_nodes_count++;
            }
            for (int i = 0; i < x_points.Length; i++)//沿x方向y=max扫地杆节点
            {
                bottom_nodes[bottom_nodes_count].num = normal_nodes_count + bottom_nodes_count + 1;
                bottom_nodes[bottom_nodes_count].x = x_points[i];
                bottom_nodes[bottom_nodes_count].y = y_length;
                bottom_nodes[bottom_nodes_count].z = h2;
                //all_normal_nodes[normal_nodes_count++] = bottom_nodes[bottom_nodes_count];
                bottom_nodes_count++;
            }
            for (int i = 0; i < y_points.Length; i++)//沿y方向x=0扫地杆节点
            {
                bottom_nodes[bottom_nodes_count].num = normal_nodes_count + bottom_nodes_count + 1;
                bottom_nodes[bottom_nodes_count].x = 0.0;
                bottom_nodes[bottom_nodes_count].y = y_points[i];
                bottom_nodes[bottom_nodes_count].z = h2;
                //all_normal_nodes[normal_nodes_count++] = bottom_nodes[bottom_nodes_count];
                bottom_nodes_count++;
            }
            for (int i = 0; i < y_points.Length; i++)//沿y方向x=max扫地杆节点
            {
                bottom_nodes[bottom_nodes_count].num = normal_nodes_count + bottom_nodes_count + 1;
                bottom_nodes[bottom_nodes_count].x = x_length;
                bottom_nodes[bottom_nodes_count].y = y_points[i];
                bottom_nodes[bottom_nodes_count].z = h2;
                //all_normal_nodes[normal_nodes_count++] = bottom_nodes[bottom_nodes_count];
                bottom_nodes_count++;
            }

            int all_elements_count = 0;//单元号计数器

            MidasElement[] y_elements = new MidasElement[10000];
            for (int i = 0; i < y_elements.Length; i++)
            {
                y_elements[i] = new MidasElement();
            }
            int y_elements_count = 0;
            for (int i = 2; i < z_input_count - 1; i++)
            {
                for (int j = 0; j < x_input_count + 1; j++)
                {
                    for (int k = 0; k < y_input_count + 1; k++)
                    {
                        for (int l = 0; l < normal_nodes_count; l++)
                        {
                            if (Math.Abs(all_normal_nodes[l].z - z_points[i]) < 0.005 && Math.Abs(all_normal_nodes[l].x - x_points[j]) < 0.005 && Math.Abs(all_normal_nodes[l].y - y_points[k]) < 0.005)
                            {
                                if (k == 0)
                                {
                                    y_elements[y_elements_count].fNode = all_normal_nodes[l].Copy();
                                }
                                else
                                {
                                    all_elements_count++;
                                    y_elements[y_elements_count].num = all_elements_count;
                                    y_elements[y_elements_count++].bNode = all_normal_nodes[l].Copy();
                                    y_elements[y_elements_count].fNode = all_normal_nodes[l].Copy();
                                }
                                break;
                            }
                        }
                    }
                }
            }

            MidasElement[] x_elements = new MidasElement[10000];
            for (int i = 0; i < x_elements.Length; i++)
            {
                x_elements[i] = new MidasElement();
            }
            int x_elements_count = 0;
            for (int i = 2; i < z_input_count - 1; i++)
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


            MidasElement[] z_elements = new MidasElement[10000];
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
                        for (int l = 0; l < normal_nodes_count; l++)
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

            MidasElement[] bottom_elements = new MidasElement[300];
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
                j = j+x_input_count - 1;
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
                j = j+y_input_count - 1;
            }

            MidasElement[] xy_bridging_lines = new MidasElement[300];//单层xy剪刀线
            int xy_bridging_lines_count = 0;
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
            int axis_xy_bridging_lines_num = xy_bridging_lines_count;
            for (int i = 0; i < xy_bridging_lines_count; i++)
            {
                xy_bridging_lines[xy_bridging_lines_count + i] = xy_bridging_lines[i].Copy();
                xy_bridging_lines[xy_bridging_lines_count + i].bNode.x = x_length - xy_bridging_lines[i].bNode.x;
                xy_bridging_lines[xy_bridging_lines_count + i].fNode.x = x_length - xy_bridging_lines[i].fNode.x;
            }
            xy_bridging_lines_count = 2*xy_bridging_lines_count;

            MidasElement[] xz_bridging_lines = new MidasElement[300];//单层xz剪刀线
            int xz_bridging_lines_count = 0;
            for (int i = 0; i < xz_bridging_lines.Length; i++)
            {
                xz_bridging_lines[i] = new MidasElement();
            }
            //double actual_start_point = z_length - 0.3 - h3 - h0 - h1;
            for (int i = 0; i <= (int)(x_length / l2 - 0.3); i++)//xy面正向中线下
            {
                xz_bridging_lines[xz_bridging_lines_count].fNode.x = i * l2;
                xz_bridging_lines[xz_bridging_lines_count].fNode.z = h3+0.3;
                xz_bridging_lines[xz_bridging_lines_count].bNode.x = xz_bridging_lines[xz_bridging_lines_count].fNode.x + ((z_length - 0.3 - h3 - h0 - h1) / tan_alpha < x_length - xz_bridging_lines[xz_bridging_lines_count].fNode.x ? (z_length - 0.3 - h3 - h0 - h1) / tan_alpha : x_length - xz_bridging_lines[xz_bridging_lines_count].fNode.x);
                xz_bridging_lines[xz_bridging_lines_count].bNode.z = h3 + 0.3 + ((z_length - 0.3 - h3 - h0 - h1) < (x_length - xz_bridging_lines[xz_bridging_lines_count].fNode.x) * tan_alpha ? (z_length - 0.3 - h3 - h0 - h1) : (x_length - xz_bridging_lines[xz_bridging_lines_count].fNode.x) * tan_alpha);
                xz_bridging_lines_count++;
            }
            for (int i = 1; i <= (int)((z_length-0.3-h3-h0-h1) / l2 - 0.3); i++)//xy面正向中线上
            {
                xz_bridging_lines[xz_bridging_lines_count].fNode.x = 0.0;
                xz_bridging_lines[xz_bridging_lines_count].fNode.z = i * l2;
                xz_bridging_lines[xz_bridging_lines_count].bNode.x = (x_length < ((z_length - h0 - h1) - xz_bridging_lines[xz_bridging_lines_count].fNode.z) / tan_alpha) ? x_length : ((z_length - h0 - h1) - xz_bridging_lines[xz_bridging_lines_count].fNode.z) / tan_alpha;
                xz_bridging_lines[xz_bridging_lines_count].bNode.z = xz_bridging_lines[xz_bridging_lines_count].fNode.z + (((z_length - h0 - h1) - xz_bridging_lines[xz_bridging_lines_count].fNode.z) < x_length * tan_alpha ? ((z_length - h0 - h1) - xz_bridging_lines[xz_bridging_lines_count].fNode.z) : x_length * tan_alpha);
                xz_bridging_lines_count++;
            }
            int axis_xz_bridging_lines_num = xz_bridging_lines_count;
            for (int i = 0; i < xz_bridging_lines_count; i++)
            {
                xz_bridging_lines[xz_bridging_lines_count + i] = xz_bridging_lines[i].Copy();
                xz_bridging_lines[xz_bridging_lines_count + i].bNode.x = x_length - xz_bridging_lines[i].bNode.x;
                xz_bridging_lines[xz_bridging_lines_count + i].fNode.x = x_length - xz_bridging_lines[i].fNode.x;
            }
            xz_bridging_lines_count = 2*xz_bridging_lines_count;

            MidasElement[] yz_bridging_lines = new MidasElement[300];//单层yz剪刀线
            int yz_bridging_lines_count = 0;
            for (int i = 0; i < yz_bridging_lines.Length; i++)
            {
                yz_bridging_lines[i] = new MidasElement();
            }
            //double actual_start_point = z_length - 0.3 - h3 - h0 - h1;
            for (int i = 0; i <= (int)(y_length / l2 - 0.3); i++)//xy面正向中线下
            {
                yz_bridging_lines[yz_bridging_lines_count].fNode.y = i * l2;
                yz_bridging_lines[yz_bridging_lines_count].fNode.z = h3 + 0.3;
                yz_bridging_lines[yz_bridging_lines_count].bNode.y = yz_bridging_lines[yz_bridging_lines_count].fNode.y + (((z_length - 0.3 - h3 - h0 - h1) / tan_alpha) < y_length - yz_bridging_lines[yz_bridging_lines_count].fNode.y ? ((z_length - 0.3 - h3 - h0 - h1) / tan_alpha) : y_length - yz_bridging_lines[yz_bridging_lines_count].fNode.y);
                yz_bridging_lines[yz_bridging_lines_count].bNode.z = h3 + 0.3 + ((z_length - 0.3 - h3 - h0 - h1) < (y_length - yz_bridging_lines[yz_bridging_lines_count].fNode.y) * tan_alpha ? (z_length - 0.3 - h3 - h0 - h1) : (y_length - yz_bridging_lines[yz_bridging_lines_count].fNode.y) * tan_alpha);
                yz_bridging_lines_count++;
            }
            for (int i = 1; i <= (int)((z_length - 0.3 - h3 - h0 - h1) / l2 - 0.3); i++)//xy面正向中线上
            {
                yz_bridging_lines[yz_bridging_lines_count].fNode.y = 0.0;
                yz_bridging_lines[yz_bridging_lines_count].fNode.z = i * l2;
                yz_bridging_lines[yz_bridging_lines_count].bNode.y = (y_length < ((z_length - h0 - h1) - yz_bridging_lines[yz_bridging_lines_count].fNode.z) / tan_alpha) ? y_length : ((z_length - h0 - h1) - yz_bridging_lines[yz_bridging_lines_count].fNode.z) / tan_alpha;
                yz_bridging_lines[yz_bridging_lines_count].bNode.z = yz_bridging_lines[yz_bridging_lines_count].fNode.z + (((z_length - h0 - h1) - yz_bridging_lines[yz_bridging_lines_count].fNode.z) < y_length * tan_alpha ? ((z_length - h0 - h1) - yz_bridging_lines[yz_bridging_lines_count].fNode.z) : y_length * tan_alpha);
                yz_bridging_lines_count++;
            }
            int axis_yz_bridging_lines_num = yz_bridging_lines_count;
            for (int i = 0; i < yz_bridging_lines_count; i++)
            {
                yz_bridging_lines[yz_bridging_lines_count + i] = yz_bridging_lines[i].Copy();
                yz_bridging_lines[yz_bridging_lines_count + i].bNode.y = y_length - yz_bridging_lines[i].bNode.y;
                yz_bridging_lines[yz_bridging_lines_count + i].fNode.y = y_length - yz_bridging_lines[i].fNode.y;
            }
            yz_bridging_lines_count = 2*yz_bridging_lines_count;
            // for (int i = 0; i <= (int)(x_length / l2 - 0.3); i++)//xy面反向中线下
            //{
            //    xy_bridging_lines[xy_bridging_lines_count].fNode.x = x_length - i * l2;
            //    xy_bridging_lines[xy_bridging_lines_count].fNode.y = 0.0;
            //    xy_bridging_lines[xy_bridging_lines_count].bNode.x = x_length - (xy_bridging_lines[xy_bridging_lines_count].fNode.x + (y_length / tan_alpha < x_length - xy_bridging_lines[xy_bridging_lines_count].fNode.x ? y_length / tan_alpha : x_length - xy_bridging_lines[xy_bridging_lines_count].fNode.x));
            //    xy_bridging_lines[xy_bridging_lines_count].bNode.y = y_length < (x_length - xy_bridging_lines[xy_bridging_lines_count].fNode.x) * tan_alpha ? y_length : (x_length - xy_bridging_lines[xy_bridging_lines_count].fNode.x) * tan_alpha;
            //    xy_bridging_lines_count++;
            //}
            //for (int i = 1; i <= (int)(y_length / l2 - 0.3); i++)//xy面反向中线上
            //{
            //    xy_bridging_lines[xy_bridging_lines_count].fNode.x = x_length - 0.0;
            //    xy_bridging_lines[xy_bridging_lines_count].fNode.y = i * l2;
            //    xy_bridging_lines[xy_bridging_lines_count].bNode.x = x_length - ((x_length < (y_length - xy_bridging_lines[xy_bridging_lines_count].fNode.y) / tan_alpha) ? x_length : (y_length - xy_bridging_lines[xy_bridging_lines_count].fNode.y) / tan_alpha);
            //    xy_bridging_lines[xy_bridging_lines_count].bNode.y = xy_bridging_lines[xy_bridging_lines_count].fNode.y+((y_length - xy_bridging_lines[xy_bridging_lines_count].fNode.y) < x_length * tan_alpha ? (y_length - xy_bridging_lines[xy_bridging_lines_count].fNode.y) : x_length * tan_alpha);
            //    xy_bridging_lines_count++;
            //}
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
            
            SetText("正在写入", this.status_bar_text);
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
            for (int i = 0; i < normal_nodes_count; i++)
            {
                buffer_string = string.Format("{0,-5},{1:0.00},{2:0.00},{3:0.00}", all_normal_nodes[i].num,
                    all_normal_nodes[i].x, all_normal_nodes[i].y, all_normal_nodes[i].z);
                writer.WriteLine(buffer_string);
            }

            buffer_string = String.Format("\r\n;扫地杆节点");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < bottom_nodes_count; i++)
            {
                buffer_string = string.Format("{0,-5},{1:0.00},{2:0.00},{3:0.00}", bottom_nodes[i].num,
                    bottom_nodes[i].x, bottom_nodes[i].y, bottom_nodes[i].z);
                writer.WriteLine(buffer_string);
            }

            buffer_string = String.Format("\r\n*ELEMENT ");
            writer.WriteLine(buffer_string);
            buffer_string = String.Format("\r\n;Y方向单元 ");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < y_elements_count; i++)
            {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                buffer_string = string.Format("{0,-5},BEAM,{1},{2},{3},{4},{5}", y_elements[i].num, 1,1,y_elements[i].fNode.num, y_elements[i].bNode.num, 0);
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

            buffer_string = String.Format("\r\n;XY剪刀线 ");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < xy_bridging_lines_count; i++)
            {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                buffer_string = string.Format("{0},{1},{2},{3}", xy_bridging_lines[i].fNode.x, xy_bridging_lines[i].fNode.y, xy_bridging_lines[i].bNode.x, xy_bridging_lines[i].bNode.y);
                writer.WriteLine(buffer_string);
            }
            buffer_string = String.Format("\r\n;XZ剪刀线 ");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < xz_bridging_lines_count; i++)
            {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                buffer_string = string.Format("{0},{1},{2},{3}", xz_bridging_lines[i].fNode.x, xz_bridging_lines[i].fNode.z, xz_bridging_lines[i].bNode.x, xz_bridging_lines[i].bNode.z);
                writer.WriteLine(buffer_string);
            }
            buffer_string = String.Format("\r\n;YZ剪刀线 ");
            writer.WriteLine(buffer_string);
            for (int i = 0; i < yz_bridging_lines_count; i++)
            {//%d , %s ,    %d,    %d,    %d,    %d,    %d\n",dy+tempcount,"BEAM",1,1,Yelement[i].qd,Yelement[i].zd,0
                buffer_string = string.Format("{0},{1},{2},{3}", yz_bridging_lines[i].fNode.y, yz_bridging_lines[i].fNode.z, yz_bridging_lines[i].bNode.y, yz_bridging_lines[i].bNode.z);
                writer.WriteLine(buffer_string);
            }
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
            SetText("写入完成", this.status_bar_text);
        }
        private void btn_section_Click(object sender, RoutedEventArgs e)
        {
            //this.status_bar_text.Text = "23";
            //PostMessage(3);
            //PostMessage(2);
            //PostMessage(1);
            //MessageBox.Show("123");
            if (!section_from_closed)
            {
                section_form.Show();
            }
            else
            {
                section_form = new SectionForm(this);
                section_form.Show();
                this.section_from_closed = false;
            }

        }
        private void setXmlValue()
        {
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "mainconfig.xml";
            //MessageBox.Show(XmlHelper.getXmlElementValue(xmlpath, "内部轮廓", "h11"));
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "纵距x", "2@0.9 3@0.6 4@0.9 3@0.6 2@0.9");
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "横距y", "30@0.9");
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "步距z", "8@1.2");
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "h0", "0.3");
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "h1", "0.15");
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "h2", "0.2");
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "h3", "0.15");
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "l1", "4.5");
            XmlHelper.setXmlElementValue(xmlpath, "支架参数", "l2", "4.5");

            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "G1", "100");
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "G2", "20");
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "G3", "15");
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "P1", "20");
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "P2", "30");
            XmlHelper.setXmlElementValue(xmlpath, "荷载参数", "P3", "0.15");

            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "Y1", "0.2");
            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "Y2", "0.3");
            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "Y3", "0.4");
            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "J1", "0.3");
            XmlHelper.setXmlElementValue(xmlpath, "阶段设置", "J2", "0.7");
        }

        private void getXmlValue()
        {
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "mainconfig.xml";
            //MessageBox.Show(XmlHelper.getXmlElementValue(xmlpath, "内部轮廓", "h11"));
            text_x_input.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "纵距x");
            text_y_input.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "横距y");
            text_z_input.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "步距z");
            text_h0.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "h0");
            text_h1.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "h1");
            text_h2.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "h2");
            text_h3.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "h3");
            text_l1.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "l1");
            text_l2.Text = XmlHelper.getXmlElementValue(xmlpath, "支架参数", "l2");

            text_G1.Text = XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "G1");
            text_G2.Text = XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "G2");
            text_G3.Text = XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "G3");
            text_P1.Text = XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "P1");
            text_P2.Text = XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "P2");
            text_P3.Text = XmlHelper.getXmlElementValue(xmlpath, "荷载参数", "P3");

            text_Y1.Text = XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "Y1");
            text_Y2.Text = XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "Y2");
            text_Y3.Text = XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "Y3");
            text_J1.Text = XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "J1");
            text_J2.Text = XmlHelper.getXmlElementValue(xmlpath, "阶段设置", "J2");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {


        }
        private void btn_export_mct_Click(object sender, RoutedEventArgs e)
        {
            //PostMessage(1);//获取x输入框数组
            //PostMessage(2);//获取y输入框数组
            //PostMessage(3);//获取z输入框数组
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

    }
}
