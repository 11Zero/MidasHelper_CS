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
using System.Windows.Shapes;
using PcrCalcultor;

namespace MidasHelper_CS
{
    /// <summary>
    /// PcrForm.xaml 的交互逻辑
    /// </summary>
    public partial class PcrForm : Window
    {
        public PcrForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox_nx.Items.Add("<=3");
            comboBox_nx.Items.Add("4");
            comboBox_nx.Items.Add("5");
            comboBox_nx.Items.Add(">=6");
            
            comboBox_nx.SelectedIndex = 0;
            text_l.Text = "1.2";
            text_h.Text = "1.2";
            text_Q.Text = "60";
            text_EI.Text = "25.1114";
            text_sdg.Text = "0.2";
            text_xbg.Text = "0.2";
            text_betaJ.Text = "1.3";
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            comboBox_nx.SelectedIndex = 0;
            text_l.Text = "1.2";
            text_h.Text = "1.2";
            text_Q.Text = "60";
            text_EI.Text = "25.1114";
            text_sdg.Text = "0.2";
            text_xbg.Text = "0.2";
            text_betaJ.Text = "1.3";
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            int nx = 3;
            switch (comboBox_nx.SelectedIndex)
            {
                case 0:
                    {
                        nx = 3;
                    }break;
                case 1:
                    {
                        nx = 4;
                    } break;
                case 2:
                    {
                        nx = 5;
                    } break;
                case 3:
                    {
                        nx = 6;
                    } break;
                default: 
                    break;
            }
            double l = double.Parse(text_l.Text);
            double h = double.Parse(text_h.Text);
            double L = double.Parse(text_L.Text);
            double Q = double.Parse(text_Q.Text);
            double EI = double.Parse(text_EI.Text);
            double sdg = double.Parse(text_sdg.Text);
            double xbg = double.Parse(text_xbg.Text);
            double beta_J = double.Parse(text_betaJ.Text);

            double k = Calculator.Solvek(l,h,Q,EI);
            double n = Calculator.Solven(L, k, h, EI);
            double beta_alpha = Calculator.Solvealpha(sdg,xbg,h,nx);
            double beta_L = Calculator.SolvebetaL(L);

            double pi = 3.1415926;
            double Pcr = 2 * n * n * pi * pi * EI * beta_J / (beta_alpha * beta_alpha * beta_L * beta_L * L * L);
            text_Pcr.Text = string.Format("{0:0.00}",Pcr);
        }

    }
}
