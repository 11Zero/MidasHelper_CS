using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
//using System.Windows.Forms;

namespace MidasHelper_CS
{
    public class BackThreader
    {
        //*****************************************************************************************//
        private bool end = false;//结束线程标志
        private bool kill = false;//终结线程标志
        private bool stop = false;//暂停线程标志
        private Thread thread = null;//恢复线程标志
        private Queue<int> msgQueue = null;//存储消息队列
        MainWindow Parent = null;//用于传入其他线程句柄，一般通过线程刷新某个窗口UI,FrameWin是需要控制的窗口类，自行修改
        //*****************************************************************************************//
        double[] x_input = null;
        Log log = null;
        public BackThreader(MainWindow parent)
        {
            Parent = (MainWindow)parent;//强制转换
            
            msgQueue = new Queue<int>();
            thread = new Thread(new ThreadStart(Run));//真正定义线程
            thread.IsBackground = true;
            x_input = new double[50];
            log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
        }
        ~BackThreader()
        {
            this.End();//析构时结束线程
        }
        public void PostMessage(int id)//id为传入的消息标识
        {
            if (end || kill)//如果线程结束或终止，不执行任何动作
                return;
            if (id > 0)
                msgQueue.Enqueue(id);//将post来的消息添加到消息队列
            if (stop)
                return;//如果线程暂停，将只接受消息，暂不执行，一旦线程恢复，继续执行所接收消息
            if (!this.thread.IsAlive)//如果线程未开启，将启动线程
                this.thread.Start();
        }
        public void Start()
        {
            if (end || kill)//如果线程已被结束或终止，将不执行任何动作
                return;
            if (!this.thread.IsAlive)//如果线程未开启，将启动线程
                thread.Start();
        }
        public void End()
        {
            end = true;//如果线程结束，将结束标识设为真，线程将在消息队列中所有消息执行完后终止
            Console.WriteLine("结束线程");
        }
        public void Kill()
        {
            kill = true;//如果线程终止，将终止标识设为真，线程将不再执行消息队列中剩余消息
            Console.WriteLine("终止线程");
        }
        public void Stop()
        {
            stop = true;//如果线程暂停，将暂停标识设为真，线程将暂不执行消息队列中剩余消息，
                        //但是消息队列仍然在接收消息，一旦线程恢复，继续执行所接收消息
            Console.WriteLine("暂停线程");
        }
        public void Resume()
        {
            stop = false;//如果线程恢复，将恢复标识设为真，线程将继续执行消息队列中剩余消息
            Console.WriteLine("恢复线程");
        }
        private void Run()
        {
            while (true)
            {
                if (kill)//如果线程终止，线程函数将立即跳出，消息队列里剩余消息不再执行，此线程结束，无法再开启
                    break;
                if (!stop && msgQueue.Count != 0)//如果线程未被暂停且消息队列中有剩余消息，将顺序执行剩余消息
                {
                    switch (msgQueue.Peek())//获取当前消息队列中消息，并一一比对执行相应的动作
                    {
                        case 1:
                            {
                                msgFunction_1();//例如消息码为1是，执行msgFunction_1()函数
                            } break;
                        case 2:
                            {
                                msgFunction_2();//例如消息码为2是，执行msgFunction_2()函数
                            } break;
                    }
                    msgQueue.Dequeue();//比对完当前消息并执行相应动作后，消息队列扔掉当前消息
                }
                if (msgQueue.Count == 0 && end)//如果线程被结束时当前消息队列中没有消息，将结束此线程
                                                //如果当前消息队列中仍有未执行消息，线程将执行完所有消息后结束
                    break;
                System.Threading.Thread.Sleep(1);//每次循环间隔1ms，我还不知道到底有没有必要
            }
        }
        private void msgFunction_1()//对应消息码为1的时要执行的函数
        {
            //string x_str = Parent.text_x_input.Text;
            //if (x_str == "")
            //{
            //    Parent.status_bar_text.Text = "23";
            //    return;
            //}
            //x_str = x_str.Replace(","," ");
            //string[] str_splited = x_str.Split(' ');
            //x_input_count = 0;
            //for (int i = 0; i < str_splited.Length; i++)
            //{
            //    if (str_splited[i].Contains('@'))
            //    {
            //        double i_value = double.Parse(str_splited[i].Substring(str_splited[i].IndexOf('@')+1));
            //        int i_count = int.Parse(str_splited[i].Substring(0,str_splited[i].IndexOf('@')));
            //        for (int j = 0; j < i_count; j++)
            //        {
            //            x_input[x_input_count++] = i_value;
            //        }
            //    }
            //    else
            //        x_input[x_input_count++] = double.Parse(str_splited[i]);
            //}
            //for (int i = 0; i < x_input_count; i++)
            //{
            //    log.log(x_input[i].ToString());
            //}
            ////Parent.FrameStatusBar
            //Console.WriteLine("执行消息码1对应事件");
        }
        private void msgFunction_2()//对应消息码为2的时要执行的函数
        {
            Console.WriteLine("执行消息码2对应事件");
        }
    }
}