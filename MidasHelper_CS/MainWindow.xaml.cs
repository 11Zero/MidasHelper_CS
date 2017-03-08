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
        private Log log = null;
        public delegate object delegateGetTextCallBack(object text);
        public delegate void delegateSetTextCallBack(string str, object object_id);
        private SectionForm section_form = null;
        public bool section_from_closed = true;
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
               if (msgQueue.Count>0)
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
           if (msgQueue.Count>200)
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
               case 3:
                   {
                       msgFunction_3();//例如消息码为2是，执行msgFunction_2()函数
                   } break;
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
        private void msgFunction_1()
        {
            string x_str = "";
            x_str = (string)GetText(text_x_input);
            x_str = x_str.Replace(",", " ");
            if (x_str == "")
            {
                SetText("x输入值无效", this.status_bar_text);
                return;
            }
            string[] str_splited = x_str.Split(' ');
            x_input_count = 0;
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
            log.log("**********************************************");
            for (int i = 0; i < x_input_count; i++)
            {
                log.log(x_input[i].ToString());
            }
            //Parent.FrameStatusBar
        }
        /// <summary>
        /// 获取y输入框数组
        /// </summary>
        private void msgFunction_2()
        {
            string y_str = "";
            y_str = (string)GetText(text_y_input);
            y_str = y_str.Replace(",", " ");
            if (y_str == "")
            {
                SetText("y输入值无效", this.status_bar_text);
                return;
            }
            string[] str_splited = y_str.Split(' ');
            y_input_count = 0;
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
            log.log("**********************************************");
            for (int i = 0; i < y_input_count; i++)
            {
                log.log(y_input[i].ToString());
            }
        }

        /// <summary>
        /// 获取z输入框数组
        /// </summary>
        private void msgFunction_3()
        {
            string z_str = "";
            z_str = (string)GetText(text_z_input);
            z_str = z_str.Replace(",", " ");
            if (z_str == "")
            {
                SetText("z输入值无效", this.status_bar_text);
                return;
            }
            string[] str_splited = z_str.Split(' ');
            z_input_count = 0;
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
            log.log("**********************************************");
            for (int i = 0; i < z_input_count; i++)
            {
                log.log(z_input[i].ToString());
            }
        }

        private void btn_test_Click(object sender, RoutedEventArgs e)
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

        private void btn_export_mct_Click(object sender, RoutedEventArgs e)
        {
            PostMessage(1);
            PostMessage(2);
            PostMessage(3);
        }


    }
}
