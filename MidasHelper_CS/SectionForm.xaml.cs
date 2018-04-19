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
using SharpGL.SceneGraph;
using SharpGL;
using XMLoperator;
namespace MidasHelper_CS
{
    /// <summary>
    /// SectionForm.xaml 的交互逻辑
    /// </summary>
    public partial class SectionForm : Window
    {
        /// <summary>
        /// The current rotation.
        /// </summary>
        bool charFlag = true;
        private bool xmlLoaded = false;
        private bool openflag = false;
        private float rotation = 0.0f;
        private int xLength = 0;///x方向实际总长
        private int yLength = 0;///y方向实际总长
        private double xTimes = 1.0;///x方向缩放倍数
        private double yTimes = 1.0;///y方向缩放倍数
        private double xRange = 0;///x方向图形尺寸
        private double yRange = 0;///y方向图形尺寸
        private double ScralSize = 1.0;//鼠标缩放因数
        public double bridgeLength = 0.0;
        public double sectionArea = 0.0;
        public int big_step = 90;
        public int little_step = 60;


        public int H01 = 0, H02 = 0, H03 = 0;
        public int B01 = 0, B02 = 0, B03 = 0, B04 = 0, B05 = 0, B06 = 0;
        public int H11 = 0, H12 = 0, H21 = 0, H22 = 0, H31 = 0, H32 = 0, H41 = 0;
        public int h11 = 0, h12 = 0, h21 = 0, h22 = 0, h31 = 0, h32 = 0, h41 = 0, h42 = 0;
        public int b11 = 0, b12 = 0, b21 = 0, b22 = 0, b31 = 0, b32 = 0, b41 = 0, b42 = 0;
        //public int h11 = 0, h12 = 0, h21 = 0, h22 = 0, h31 = 0, h32 = 0, h41 = 0, h42 = 0;
        //public int b11 = 0, b12 = 0, b21 = 0, b22 = 0, b31 = 0, b32 = 0, b41 = 0, b42 = 0;
        MainWindow parentWin = null;
        //private OpenGL gl = null;
        public SectionForm(Window parent)
        {
            InitializeComponent();
            parentWin = (MainWindow)parent;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            combo_select.Items.Add("单箱单室");
            combo_select.Items.Add("单箱双室");
            combo_select.Items.Add("单箱三室");
            combo_select.Items.Add("单箱多室");
            combo_select.SelectedIndex = 0;
            getXmlValue();
            check_textview.IsChecked = true;
            text_big.Text = big_step.ToString();
            text_little.Text = little_step.ToString();
            openflag = true;
            drawSection(0);
            //check_textview.
            //gl = openGLControl.OpenGL;
            //gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            //gl.LoadIdentity();
        }
        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL object.
            return;
            OpenGL gl = openGLControl.OpenGL;
            gl = openGLControl.OpenGL;
            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();

            //  Rotate around the Y axis.
            gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

            //  Draw a coloured pyramid.
            gl.Begin(OpenGL.GL_TRIANGLES);
            gl.Color(1.1f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.End();

            //  Nudge the rotation.
            rotation += 3.0f;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="nX"></param>
        ///// <param name="nY"></param>
        ///// <param name="length">线长度</param>
        ///// <param name="col"></param>
        ///// <param name="isArrow"></param>
        ///// <param name="withBorder"></param>
        /// <summary>
        /// 画直线
        /// </summary>
        /// <param name="sX">起点x坐标</param>
        /// <param name="sY">起点y坐标</param>
        /// <param name="eX">终点x坐标</param>
        /// <param name="eY">终点y坐标</param>
        /// <param name="isArrow">是否带箭头</param>
        /// <param name="withBorder">是否带边界线</param>
        private void drawLine(int sX, int sY, int eX, int eY, bool isArrow = false, bool withBorder = true)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Translate(-1, -1, 0);
            gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
            gl.Vertex(sX * xRange + 1 - xTimes, sY * yRange + 1 - yTimes);
            gl.Vertex(eX * xRange + 1 - xTimes, eY * yRange + 1 - yTimes);
            gl.End();
            //gl.Color(0.0f, 1.0f, 0.0f);

        }
        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;
            //xTimes = 0.8;
            //yTimes = 0.8;
            //xLength = 2;
            //yLength = 2;
            //xRange = xTimes / xLength;///opengl整个空间大小为-1，-1到1,1，所以画线时需根据实际尺寸及绘图空间进行缩放
            //yRange = yTimes / yLength;
            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);
            //gl.Translate(-xTimes / 5, yTimes / 5, 0);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            return;
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void drawOneSection()
        {
            OpenGL gl = openGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
            gl.Vertex(1.0f, 2.0f);
            gl.Vertex(2.0f, 1.0f);
            gl.End();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            setXmlValue();
            getXmlValue();
            drawSection(combo_select.SelectedIndex);
            this.parentWin.text_G1.Text = (26 * sectionArea * bridgeLength).ToString();
            this.parentWin.selected_section = combo_select.SelectedIndex;
            int conserve_length = 40;
            switch (combo_select.SelectedIndex)
            {
                case 0:
                    {
                        int nStep_B03 = (int)Math.Floor(((double)B03 - conserve_length) / big_step);
                        int nStep_B02 = (int)Math.Ceiling((double)((B03 - nStep_B03 * big_step) / 2.0 + B02 + conserve_length) / little_step);
                        int nStep_B01 = (int)Math.Ceiling((double)(B01 * 2 + B02 * 2 + B03 - 2 * nStep_B02 * little_step - nStep_B03 * big_step) / 2.0 / big_step);
                        this.parentWin.text_x_input.Text = string.Format("{0}@{4:0.0} {1}@{3:0.0} {2}@{4:0.0} {1}@{3:0.0} {0}@{4:0.0}", nStep_B01, nStep_B02, nStep_B03, little_step / 100.0, big_step / 100.0);
                    } break;
                case 1:
                    {
                        int nStep_B04 = (int)Math.Ceiling(((double)B04 + conserve_length) / little_step);
                        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + B04 - nStep_B04 * little_step) / 2.0 -conserve_length) / big_step);
                        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + B04 + 2 * B03 - nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 + conserve_length) / little_step);
                        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + B04 - nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                        this.parentWin.text_x_input.Text = string.Format("{0}@{5:0.0} {1}@{4:0.0} {2}@{5:0.0} {3}@{4:0.0} {2}@{5:0.0} {1}@{4:0.0} {0}@{5:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, little_step / 100.0, big_step / 100.0);
                    } break;
                case 2:
                    {
                        int nStep_B05 = (int)Math.Floor(((double)B05 - conserve_length )/ big_step);
                        int nStep_B04 = (int)Math.Ceiling(((double)(2 * B04 + B05 - nStep_B05 * big_step) / 2.0 + conserve_length) / little_step);
                        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step) / 2.0 - conserve_length) / big_step);
                        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + 2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 + conserve_length )/ little_step);
                        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                        this.parentWin.text_x_input.Text = string.Format("{0}@{6:0.0} {1}@{5:0.0} {2}@{6:0.0} {3}@{5:0.0} {4}@{6:0.0} {3}@{5:0.0} {2}@{6:0.0} {1}@{5:0.0} {0}@{6:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, nStep_B05, little_step / 100.0, big_step / 100.0);
                    } break;
                case 3:
                    {
                        int nStep_B06 = (int)Math.Ceiling(((double)B06 + conserve_length )/ little_step);
                        int nStep_B05 = (int)Math.Floor(((double)(2 * B05 + B06 - nStep_B06 * little_step) / 2.0 - conserve_length )/ big_step);
                        int nStep_B04 = (int)Math.Ceiling(((double)(2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step) / 2.0+conserve_length) / little_step);
                        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step) / 2.0 - conserve_length) / big_step);
                        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 +conserve_length)/ little_step);
                        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                        this.parentWin.text_x_input.Text = string.Format("{0}@{7:0.0} {1}@{6:0.0} {2}@{7:0.0} {3}@{6:0.0} {4}@{7:0.0} {5}@{6:0.0} {4}@{7:0.0} {3}@{6:0.0} {2}@{7:0.0} {1}@{6:0.0} {0}@{7:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, nStep_B05, nStep_B05, little_step / 100.0, big_step / 100.0);

                    } break;
                default:
                    break;
            }
            this.parentWin.section_selected_flag = true;
            //////刷新视图
            big_step = int.Parse(text_big.Text);
            little_step = int.Parse(text_little.Text);
            if (openflag == false)
                return;
            drawSection(combo_select.SelectedIndex);
            //this.Close();
        }

        private void text_h01_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void setXmlValue()
        {
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "sectionconfig.xml";
            //MessageBox.Show(XmlHelper.getXmlElementValue(xmlpath, "内部轮廓", "h11"));
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "H01", text_H01.Text);
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "H02", text_H02.Text);
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "H03", text_H03.Text);

            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B01", text_B01.Text);
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B02", text_B02.Text);
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B03", text_B03.Text);
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B04", text_B04.Text);
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B05", text_B05.Text);
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B06", text_B06.Text);

            XmlHelper.setXmlElementValue(xmlpath, "上下板厚", "H11", text_H11.Text);
            XmlHelper.setXmlElementValue(xmlpath, "上下板厚", "H12", text_H12.Text);
            XmlHelper.setXmlElementValue(xmlpath, "上下板厚", "H21", text_H21.Text);
            XmlHelper.setXmlElementValue(xmlpath, "上下板厚", "H22", text_H22.Text);

            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h11", text_h11.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b11", text_b11.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h12", text_h12.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b12", text_b12.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h21", text_h21.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b21", text_b21.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h22", text_h22.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b22", text_b22.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h31", text_h31.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b31", text_b31.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h32", text_h32.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b32", text_b32.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h41", text_h41.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b41", text_b41.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h42", text_h42.Text);
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b42", text_b42.Text);

            XmlHelper.setXmlElementValue(xmlpath, "顺桥向桥长", "bridge_length", text_bridge_length.Text);

        }

        private void getXmlValue()
        {
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "sectionconfig.xml";
            //MessageBox.Show(XmlHelper.getXmlElementValue(xmlpath, "内部轮廓", "h11"));
            text_H01.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "H01");
            H01 = int.Parse(text_H01.Text); this.parentWin.H01 = H01;
            text_H02.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "H02");
            H02 = int.Parse(text_H02.Text); this.parentWin.H02 = H02;
            text_H03.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "H03");
            H03 = int.Parse(text_H03.Text); this.parentWin.H03 = H03;

            text_B01.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B01");
            B01 = int.Parse(text_B01.Text); this.parentWin.B01 = B01;
            text_B02.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B02");
            B02 = int.Parse(text_B02.Text); this.parentWin.B02 = B02;
            text_B03.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B03");
            B03 = int.Parse(text_B03.Text); this.parentWin.B03 = B03;
            text_B04.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B04");
            B04 = int.Parse(text_B04.Text); this.parentWin.B04 = B04;
            text_B05.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B05");
            B05 = int.Parse(text_B05.Text); this.parentWin.B05 = B05;
            text_B06.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B06");
            B06 = int.Parse(text_B06.Text); this.parentWin.B06 = B06;

            text_H11.Text = XmlHelper.getXmlElementValue(xmlpath, "上下板厚", "H11");
            H11 = int.Parse(text_H11.Text); this.parentWin.H11 = H11;
            text_H12.Text = XmlHelper.getXmlElementValue(xmlpath, "上下板厚", "H12");
            H12 = int.Parse(text_H12.Text); this.parentWin.H12 = H12;
            text_H21.Text = XmlHelper.getXmlElementValue(xmlpath, "上下板厚", "H21");
            H21 = int.Parse(text_H21.Text); this.parentWin.H21 = H21;
            text_H22.Text = XmlHelper.getXmlElementValue(xmlpath, "上下板厚", "H22");
            H22 = int.Parse(text_H22.Text); this.parentWin.H22 = H22;

            text_h11.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h11");
            h11 = int.Parse(text_h11.Text); this.parentWin.h11 = h11;
            text_b11.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b11");
            b11 = int.Parse(text_b11.Text); this.parentWin.b11 = b11;
            text_h12.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h12");
            h12 = int.Parse(text_h12.Text); this.parentWin.h12 = h12;
            text_b12.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b12");
            b12 = int.Parse(text_b12.Text); this.parentWin.b12 = b12;
            text_h21.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h21");
            h21 = int.Parse(text_h21.Text); this.parentWin.h21 = h21;
            text_b21.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b21");
            b21 = int.Parse(text_b21.Text); this.parentWin.b21 = b21;
            text_h22.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h22");
            h22 = int.Parse(text_h22.Text); this.parentWin.h22 = h22;
            text_b22.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b22");
            b22 = int.Parse(text_b22.Text); this.parentWin.b22 = b22;
            text_h31.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h31");
            h31 = int.Parse(text_h31.Text); this.parentWin.h31 = h31;
            text_b31.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b31");
            b31 = int.Parse(text_b31.Text); this.parentWin.b31 = b31;
            text_h32.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h32");
            h32 = int.Parse(text_h32.Text); this.parentWin.h32 = h32;
            text_b32.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b32");
            b32 = int.Parse(text_b32.Text); this.parentWin.b32 = b32;
            text_h41.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h41");
            h41 = int.Parse(text_h41.Text); this.parentWin.h41 = h41;
            text_b41.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b41");
            b41 = int.Parse(text_b41.Text); this.parentWin.b41 = b41;
            text_h42.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h42");
            h42 = int.Parse(text_h42.Text); this.parentWin.h42 = h42;
            text_b42.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b42");
            b42 = int.Parse(text_b42.Text); this.parentWin.b42 = b42;

            text_bridge_length.Text = XmlHelper.getXmlElementValue(xmlpath, "顺桥向桥长", "bridge_length");
            bridgeLength = double.Parse(text_bridge_length.Text);
            this.parentWin.bridge_length = bridgeLength;
            xmlLoaded = true;



        }

        private void combo_select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xmlLoaded == false)
                getXmlValue();
            drawSection(combo_select.SelectedIndex);
        }


        private void drawLine(double nX, double nY, double Length, bool Col, bool IsArrow = true, bool WithBorder = true, int color = 0)
        {
            OpenGL gl = openGLControl.OpenGL;
            if (color == 0)
                gl.Color(1.0f, 0.0f, 0.0f);
            if (color == 1)
                gl.Color(0.0f, 1.0f, 0.0f);
            double BorderLen = 5.0 / ScralSize;
            gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
            gl.Vertex((nX) * xRange, (nY) * yRange);
            gl.Vertex((nX + Length * Convert.ToInt16(!Col)) * xRange, (nY - Length * Convert.ToInt16(Col)) * yRange);
            gl.End();
            gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
            gl.Vertex((nX - BorderLen * yRange / xRange * Convert.ToInt16(Col)) * xRange, (nY - BorderLen * Convert.ToInt16(!Col)) * yRange);
            gl.Vertex((nX + BorderLen * yRange / xRange * Convert.ToInt16(Col)) * xRange, (nY + BorderLen * Convert.ToInt16(!Col)) * yRange);
            gl.End();
            gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
            gl.Vertex((nX + Length * Convert.ToInt16(!Col) - BorderLen * yRange / xRange * Convert.ToInt16(Col)) * xRange, (nY - Length * Convert.ToInt16(Col) - BorderLen * Convert.ToInt16(!Col)) * yRange);
            gl.Vertex((nX + Length * Convert.ToInt16(!Col) + BorderLen * yRange / xRange * Convert.ToInt16(Col)) * xRange, (nY - Length * Convert.ToInt16(Col) + BorderLen * Convert.ToInt16(!Col)) * yRange);
            gl.End();
            gl.Color(0.0f, 0.0f, 0.0f);
        }

        private void drawString(string info, double x, double y)
        {
            if (!charFlag)
                return;
            OpenGL gl = openGLControl.OpenGL;
            double ww = gl_grid.Width;
            double hh = gl_grid.Height;
            int w = Convert.ToInt32(ww * (x * xRange + 1 - xTimes / 2) / 2);//)
            int h = Convert.ToInt32(hh * (y * yRange + 1 + yTimes / 2) / 2);//) 
            gl.DrawText(w, h, 0, 255, 0, "Calibri", 15, info);
            //gl.Translate(-xTimes / 2, yTimes / 2, 0);

        }
        private void drawSection(int SelectFlag)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            xTimes = 1.2;
            yTimes = 1.2;
            switch (SelectFlag)
            {
                case 0:
                    {
                        xLength = 2 * B01 + 2 * B02 + B03;
                        text_h21.IsEnabled = false;
                        text_b21.IsEnabled = false;
                        text_h22.IsEnabled = false;
                        text_b22.IsEnabled = false;
                        text_h31.IsEnabled = false;
                        text_b31.IsEnabled = false;
                        text_h32.IsEnabled = false;
                        text_b32.IsEnabled = false;
                        text_h41.IsEnabled = false;
                        text_b41.IsEnabled = false;
                        text_h42.IsEnabled = false;
                        text_b42.IsEnabled = false;

                        text_B04.IsEnabled = false;
                        text_B05.IsEnabled = false;
                        text_B06.IsEnabled = false;
                        text_H21.IsEnabled = false;
                        text_H22.IsEnabled = false;

                    } break;
                case 1:
                    {
                        xLength = 2 * B01 + 2 * B02 + 2 * B03 + B04;
                        text_h21.IsEnabled = true;
                        text_b21.IsEnabled = true;
                        text_h22.IsEnabled = true;
                        text_b22.IsEnabled = true;
                        text_h31.IsEnabled = false;
                        text_b31.IsEnabled = false;
                        text_h32.IsEnabled = false;
                        text_b32.IsEnabled = false;
                        text_h41.IsEnabled = false;
                        text_b41.IsEnabled = false;
                        text_h42.IsEnabled = false;
                        text_b42.IsEnabled = false;

                        text_B04.IsEnabled = true;
                        text_B05.IsEnabled = false;
                        text_B06.IsEnabled = false;
                        text_H21.IsEnabled = false;
                        text_H22.IsEnabled = false;
                    } break;
                case 2:
                    {
                        xLength = 2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + B05;
                        text_h21.IsEnabled = true;
                        text_b21.IsEnabled = true;
                        text_h22.IsEnabled = true;
                        text_b22.IsEnabled = true;
                        text_h31.IsEnabled = true;
                        text_b31.IsEnabled = true;
                        text_h32.IsEnabled = true;
                        text_b32.IsEnabled = true;
                        text_h41.IsEnabled = false;
                        text_b41.IsEnabled = false;
                        text_h42.IsEnabled = false;
                        text_b42.IsEnabled = false;

                        text_B04.IsEnabled = true;
                        text_B05.IsEnabled = true;
                        text_B06.IsEnabled = false;
                        text_H21.IsEnabled = true;
                        text_H22.IsEnabled = true;
                    } break;
                case 3:
                    {
                        xLength = 2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06;
                        text_h21.IsEnabled = true;
                        text_b21.IsEnabled = true;
                        text_h22.IsEnabled = true;
                        text_b22.IsEnabled = true;
                        text_h31.IsEnabled = true;
                        text_b31.IsEnabled = true;
                        text_h32.IsEnabled = true;
                        text_b32.IsEnabled = true;
                        text_h41.IsEnabled = true;
                        text_b41.IsEnabled = true;
                        text_h42.IsEnabled = true;
                        text_b42.IsEnabled = true;

                        text_B04.IsEnabled = true;
                        text_B05.IsEnabled = true;
                        text_B06.IsEnabled = true;
                        text_H21.IsEnabled = true;
                        text_H22.IsEnabled = true;
                    } break;
            }

            yLength = H01 + H02 + H03;
            xRange = xTimes / xLength;
            yRange = yTimes / yLength;
            //gl.Translate(-1, 1, 0);


            gl.Translate(-xTimes / 2, yTimes / 2, 0);
            gl.Color(1.0f, 1.0f, 1.0f);//设置当前色为白色
            double tempX = 0.0, tempY = 0.0;
            int conserve_length = 40;
            switch (SelectFlag)
            {
                case 0:
                    {
                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = 0.0; tempY = 0.0;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - H01;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B01;
                        tempY = tempY - H02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - H03;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B02 + B03 + B02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + H03;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B01; tempY = tempY + H02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + H01;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = 0.0; tempY = 0.0;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();//此后添加截面中部轮廓


                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02; tempY = -h11 - H11;
                        //gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - h12 - h11 - H11 - H12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b12; tempY = tempY - h12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B03 - b12 - b12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b12; tempY = tempY + h12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H11 - H12 - h11 - h12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b11; tempY = tempY + h11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B03 - b11 - b11);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02; tempY = -h11 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        tempX = 0 - 50 * xTimes / ScralSize; tempY = 0;
                        drawLine(tempX, tempY, H01, true);
                        drawString("H01", tempX - 50 * xTimes / yTimes / ScralSize, tempY - H01 * 1 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempY = tempY - H01;
                        drawLine(tempX, tempY, H02, true);
                        drawString("H02", tempX - 50 * xTimes / yTimes / ScralSize, tempY - H02 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempY = tempY - H02;
                        drawLine(tempX, tempY, H03, true);
                        drawString("H03", tempX - 50 * xTimes / yTimes / ScralSize, tempY - H03 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempX = 0; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B01, false);
                        drawString("B01", tempX + B01 / 2 - 20 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B01; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B02, false);
                        drawString("B02", tempX + B02 / 2 - 20 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B02; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B03, false);
                        drawString("B03", tempX + B03 / 2 - 32 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);


                        tempX = B01 + B02 + b11 + (B03 - b11 - b21) / 2; tempY = 0;
                        drawLine(tempX, tempY, H11, true);
                        drawString("H11", tempX - 50 * xTimes / yTimes / ScralSize, tempY - H11 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempX = B01 + B02 + b12 + (B03 - b12 - b22) / 2; tempY = -(H01 + H02 + H03 - H12);
                        drawLine(tempX, tempY, H12, true);
                        drawString("H12", tempX - 50 * xTimes / yTimes / ScralSize, tempY - H12 / 2 - 4 * yTimes / xTimes / ScralSize);

                        tempX = B01 + B02 + b11 * 2 / 3; tempY = -(H11 + h11 / 2);
                        drawString("(1,1)", tempX, tempY);
                        tempX = B01 + B02 + b12 * 2 / 3; tempY = -(H01 + H02 + H03 - H12 - h12 / 2);
                        drawString("(1,2)", tempX, tempY);

                        
                        drawString(string.Format("big step = {0}cm,little step = {1}cm",big_step,little_step), -300, 30);
                //        int nStep_B03 = (int)Math.Floor(((double)B03 - conserve_length) / big_step);
                //        int nStep_B02 = (int)Math.Ceiling((double)((B03 - nStep_B03 * big_step) / 2.0 + B02 + conserve_length) / little_step);
                //        int nStep_B01 = (int)Math.Ceiling((double)(B01 * 2 + B02 * 2 + B03 - 2 * nStep_B02 * little_step - nStep_B03 * big_step) / 2.0 / big_step);
                //        this.parentWin.text_x_input.Text = string.Format("{0}@{4:0.0} {1}@{3:0.0} {2}@{4:0.0} {1}@{3:0.0} {0}@{4:0.0}", nStep_B01, nStep_B02, nStep_B03, little_step / 100.0, big_step / 100.0);
                //    } break;
                //case 1:
                //    {
                //        int nStep_B04 = (int)Math.Ceiling(((double)B04 + conserve_length) / little_step);
                //        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + B04 - nStep_B04 * little_step) / 2.0 - conserve_length) / big_step);
                //        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + B04 + 2 * B03 - nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 + conserve_length) / little_step);
                //        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + B04 - nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                //        this.parentWin.text_x_input.Text = string.Format("{0}@{5:0.0} {1}@{4:0.0} {2}@{5:0.0} {3}@{4:0.0} {2}@{5:0.0} {1}@{4:0.0} {0}@{5:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, little_step / 100.0, big_step / 100.0);
                //    } break;
                //case 2:
                //    {
                //        int nStep_B05 = (int)Math.Floor(((double)B05 - conserve_length) / big_step);
                //        int nStep_B04 = (int)Math.Ceiling(((double)(2 * B04 + B05 - nStep_B05 * big_step) / 2.0 + conserve_length) / little_step);
                //        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step) / 2.0 - conserve_length) / big_step);
                //        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + 2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 + conserve_length) / little_step);
                //        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                //        this.parentWin.text_x_input.Text = string.Format("{0}@{6:0.0} {1}@{5:0.0} {2}@{6:0.0} {3}@{5:0.0} {4}@{6:0.0} {3}@{5:0.0} {2}@{6:0.0} {1}@{5:0.0} {0}@{6:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, nStep_B05, little_step / 100.0, big_step / 100.0);
                //    } break;
                //case 3:
                //    {
                //        int nStep_B06 = (int)Math.Ceiling(((double)B06 + conserve_length) / little_step);
                //        int nStep_B05 = (int)Math.Floor(((double)(2 * B05 + B06 - nStep_B06 * little_step) / 2.0 - conserve_length) / big_step);
                //        int nStep_B04 = (int)Math.Ceiling(((double)(2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step) / 2.0 + conserve_length) / little_step);
                //        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step) / 2.0 - conserve_length) / big_step);
                //        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 + conserve_length) / little_step);
                //        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                //        this.parentWin.text_x_input.Text = string.Format("{0}@{7:0.0} {1}@{6:0.0} {2}@{7:0.0} {3}@{6:0.0} {4}@{7:0.0} {5}@{6:0.0} {4}@{7:0.0} {3}@{6:0.0} {2}@{7:0.0} {1}@{6:0.0} {0}@{7:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, nStep_B05, nStep_B05, little_step / 100.0, big_step / 100.0);


                        int nStep_B03 = (int)Math.Floor(((double)B03 - conserve_length) / big_step);
                        int nStep_B02 = (int)Math.Ceiling((double)((B03 - nStep_B03 * big_step) / 2.0 + B02 + conserve_length) / little_step);
                        int nStep_B01 = (int)Math.Ceiling((double)(B01 * 2 + B02 * 2 + B03 - 2 * nStep_B02 * little_step - nStep_B03 * big_step) / 2.0 / big_step);
                        int scalffold_width = 2 * nStep_B01 * big_step + 2 * nStep_B02 * little_step + nStep_B03 * big_step;
                        drawString(string.Format("width of bridge = {0:0.00}m,width of scaffold = {1:0.00}m", xLength/100.0, scalffold_width/100.0), -300, 10);
                        //this.parentWin.text_x_input.Text = string.Format("{0}@{4:0.0} {1}@{3:0.0} {2}@{4:0.0} {1}@{3:0.0} {0}@{4:0.0}", nStep_B01, nStep_B02, nStep_B03, little_step / 100.0, big_step / 100.0);

                        int startpoint = (2 * B01 + 2 * B02 + B03 - nStep_B03 * big_step - 2 * nStep_B02 * little_step - 2 * nStep_B01 * big_step) / 2;
                        for (int i = 0; i < nStep_B01; i++)
                        {
                            startpoint += 0;
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B01 * big_step;
                        for (int i = 0; i < nStep_B02; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B02 * little_step;
                        for (int i = 0; i < nStep_B03; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B03 * big_step;
                        for (int i = 0; i < nStep_B02; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B02 * little_step;
                        for (int i = 0; i < nStep_B01; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }

                        sectionArea = (2 * B01 + 2 * B02 + B03) * (H01 + H02 + H03) - H02 * B01 - 2 * H03 * B01 - (H01 + H02 + H03 - H11 - H12) * B03 + h11 * b11 + h12 * b12;
                        sectionArea = sectionArea / 10000.0;
                    } break;
                case 1:
                    {
                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = 0.0; tempY = 0.0;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - H01;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B01; tempY = tempY - H02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - H03;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + 2 * B02 + 2 * B03 + B04;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + H03;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B01; tempY = tempY + H02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + H01;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = 0.0; tempY = 0.0;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();//此后添加截面中部轮廓


                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02; tempY = -h11 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - h12 - h11 - H11 - H12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b12; tempY = tempY - h12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B03 - b12 - b22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b22; tempY = tempY + h22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H11 - H12 - h21 - h22);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b21; tempY = tempY + h21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B03 - b11 - b21);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02; tempY = -h11 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02 + B03 + B04; tempY = -h21 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - H12 - H11 - h21 - h22);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b22; tempY = tempY - h22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B03 - b22 - b12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b12; tempY = tempY + h12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H11 - H12 - h11 - h12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b11; tempY = tempY + h11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B03 - b21 - b11);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02 + B03 + B04; tempY = -h21 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();


                        tempX = 0 - 50 * xTimes / ScralSize; tempY = 0;
                        drawLine(tempX, tempY, H01, true);
                        drawString("H01", tempX - 70 * xTimes / yTimes / ScralSize, tempY - H01 * 1 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempY = tempY - H01;
                        drawLine(tempX, tempY, H02, true);
                        drawString("H02", tempX - 70 * xTimes / yTimes / ScralSize, tempY - H02 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempY = tempY - H02;
                        drawLine(tempX, tempY, H03, true);
                        drawString("H03", tempX - 70 * xTimes / yTimes / ScralSize, tempY - H03 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempX = 0; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B01, false);
                        drawString("B01", tempX + B01 / 2 - 32 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B01; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B02, false);
                        drawString("B02", tempX + B02 / 2 - 32 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B02; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B03, false);
                        drawString("B03", tempX + B03 / 2 - 32 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B03; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B04, false);
                        drawString("B04", tempX + B04 / 2 - 32 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);


                        tempX = B01 + B02 + b11 + (B03 - b11 - b21) / 2; tempY = 0;
                        drawLine(tempX, tempY, H11, true);
                        drawString("H11", tempX - 70 * xTimes / yTimes / ScralSize, tempY - H11 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempX = B01 + B02 + b12 + (B03 - b12 - b22) / 2; tempY = -(H01 + H02 + H03 - H12);
                        drawLine(tempX, tempY, H12, true);
                        drawString("H12", tempX - 70 * xTimes / yTimes / ScralSize, tempY - H12 / 2 - 4 * yTimes / xTimes / ScralSize);

                        tempX = B01 + B02 + b11 * 2 / 3; tempY = -(H11 + h11 / 2);
                        drawString("(1,1)", tempX, tempY);
                        tempX = B01 + B02 + b12 * 2 / 3; tempY = -(H01 + H02 + H03 - H12 - h12 / 2);
                        drawString("(1,2)", tempX, tempY);
                        tempX = B01 + B02 + B03 + b21 * 2 / 3 - 80 * xTimes; tempY = -(H11 + h21 / 2);
                        drawString("(2,1)", tempX, tempY);
                        tempX = B01 + B02 + B03 + b22 * 2 / 3 - 80 * xTimes; tempY = -(H01 + H02 + H03 - H12 - h22 / 2);
                        drawString("(2,2)", tempX, tempY);

                        drawString(string.Format("big step={0}cm,little step={1}cm", big_step, little_step), -350, 50);


                        int nStep_B04 = (int)Math.Ceiling(((double)B04 + conserve_length) / little_step);
                        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + B04 - nStep_B04 * little_step) / 2.0 - conserve_length) / big_step);
                        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + B04 + 2 * B03 - nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 + conserve_length) / little_step);
                        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + B04 - nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                        int scalffold_width = 2 * nStep_B01 * big_step + 2 * nStep_B02 * little_step + 2 * nStep_B03 * big_step + nStep_B04 * little_step;
                        drawString(string.Format("width of bridge = {0:0.00}m,width of scaffold = {1:0.00}m", xLength / 100.0, scalffold_width / 100.0), -350, 30);
                        //this.parentWin.text_x_input.Text = string.Format("{0}@{5:0.0} {1}@{4:0.0} {2}@{5:0.0} {3}@{4:0.0} {2}@{5:0.0} {1}@{4:0.0} {0}@{5:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, little_step / 100.0, big_step / 100.0);
                        int startpoint = (2 * B01 + 2 * B02 + 2 * B03 + B04 - nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step - 2 * nStep_B01 * big_step) / 2;
                        for (int i = 0; i < nStep_B01; i++)
                        {
                            startpoint += 0;
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B01 * big_step;
                        for (int i = 0; i < nStep_B02; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B02 * little_step;
                        for (int i = 0; i < nStep_B03; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B03 * big_step;
                        for (int i = 0; i < nStep_B04; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B04 * little_step;
                        for (int i = 0; i < nStep_B03; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B03 * big_step;
                        for (int i = 0; i < nStep_B02; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B02 * little_step;
                        for (int i = 0; i < nStep_B01; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        //drawString("B01", tempX + B01 / 2 - 32 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);


                        sectionArea = (2 * B01 + 2 * B02 + 2 * B03 + B04) * (H01 + H02 + H03) - H02 * B01 - 2 * H03 * B01 - 2 * (H01 + H02 + H03 - H11 - H12) * B03 + h11 * b11 + h12 * b12 + h21 * b21 + h22 * b22;
                        sectionArea = sectionArea / 10000.0;
                    } break;
                case 2:
                    {
                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = 0.0; tempY = 0.0;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - H01;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B01; tempY = tempY - H02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - H03;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + 2 * B02 + 2 * B03 + 2 * B04 + B05;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + H03;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B01; tempY = tempY + H02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + H01;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = 0.0; tempY = 0.0;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();//此后添加截面中部轮廓


                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02; tempY = -h11 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - h12 - h11 - H11 - H12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b12; tempY = tempY - h12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B03 - b12 - b22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b22; tempY = tempY + h22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H11 - H12 - h21 - h22);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b21; tempY = tempY + h21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B03 - b11 - b21);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02; tempY = -h11 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02 + B03 + B04; tempY = -h31 - H21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - h32 - h31 - H21 - H22);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b32; tempY = tempY - h32;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B05 - b32 - b32;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b32; tempY = tempY + h32;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H21 - H22 - h31 - h32);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b31; tempY = tempY + h31;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B05 - b31 - b31);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02 + B03 + B04; tempY = -h31 - H21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02 + B03 + 2 * B04 + B05; tempY = -h21 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - H12 - H11 - h21 - h22);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b22; tempY = tempY - h22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B03 - b22 - b12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b12; tempY = tempY + h12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H11 - H12 - h11 - h12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b11; tempY = tempY + h11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B03 - b21 - b11);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02 + B03 + 2 * B04 + B05; tempY = -h21 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        tempX = 0 - 50 * xTimes / ScralSize; tempY = 0;
                        drawLine(tempX, tempY, H01, true);
                        drawString("H01", tempX - 80 * xTimes / yTimes / ScralSize, tempY - H01 * 1 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempY = tempY - H01;
                        drawLine(tempX, tempY, H02, true);
                        drawString("H02", tempX - 80 * xTimes / yTimes / ScralSize, tempY - H02 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempY = tempY - H02;
                        drawLine(tempX, tempY, H03, true);
                        drawString("H03", tempX - 80 * xTimes / yTimes / ScralSize, tempY - H03 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempX = 0; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B01, false);
                        drawString("B01", tempX + B01 / 2 - 40 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B01; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B02, false);
                        drawString("B02", tempX + B02 / 2 - 40 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B02; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B03, false);
                        drawString("B03", tempX + B03 / 2 - 40 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B03; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B04, false);
                        drawString("B04", tempX + B04 / 2 - 40 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B04; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B05, false);
                        drawString("B05", tempX + B05 / 2 - 40 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);


                        tempX = B01 + B02 + b11 + (B03 - b11 - b21) / 2; tempY = 0;
                        drawLine(tempX, tempY, H11, true);
                        drawString("H11", tempX - 80 * xTimes / yTimes / ScralSize, tempY - H11 / 2 - 6 * yTimes / xTimes / ScralSize);
                        tempX = B01 + B02 + b12 + (B03 - b12 - b22) / 2; tempY = -(H01 + H02 + H03 - H12);
                        drawLine(tempX, tempY, H12, true);
                        drawString("H12", tempX - 80 * xTimes / yTimes / ScralSize, tempY - H12 / 2 - 6 * yTimes / xTimes / ScralSize);
                        tempX = B01 + B02 + B03 + B04 + b31 + (B03 - b31 - b21) / 2; tempY = 0;
                        drawLine(tempX, tempY, H21, true);
                        drawString("H21", tempX - 80 * xTimes / yTimes / ScralSize, tempY - H21 / 2 - 6 * yTimes / xTimes / ScralSize);
                        tempX = B01 + B02 + B03 + B04 + b32 + (B03 - b32 - b22) / 2; tempY = -(H01 + H02 + H03 - H22);
                        drawLine(tempX, tempY, H22, true);
                        drawString("H22", tempX - 80 * xTimes / yTimes / ScralSize, tempY - H22 / 2 - 6 * yTimes / xTimes / ScralSize);


                        tempX = B01 + B02 + b11 * 2 / 3; tempY = -(H11 + h11 / 2);
                        drawString("(1,1)", tempX, tempY);
                        tempX = B01 + B02 + b12 * 2 / 3; tempY = -(H01 + H02 + H03 - H12 - h12 / 2);
                        drawString("(1,2)", tempX, tempY);
                        tempX = B01 + B02 + B03 + b21 * 2 / 3 - 80 * xTimes; tempY = -(H11 + h21 / 2);
                        drawString("(2,1)", tempX, tempY);
                        tempX = B01 + B02 + B03 + b22 * 2 / 3 - 80 * xTimes; tempY = -(H01 + H02 + H03 - H12 - h22 / 2);
                        drawString("(2,2)", tempX, tempY);
                        tempX = B01 + B02 + B03 + B04 + b31 * 2 / 3; tempY = -(H21 + h31 / 2);
                        drawString("(3,1)", tempX, tempY);
                        tempX = B01 + B02 + B03 + B04 + b32 * 2 / 3; tempY = -(H01 + H02 + H03 - H22 - h32 / 2);
                        drawString("(3,2)", tempX, tempY);

                        drawString(string.Format("big step={0}cm,little step={1}cm", big_step, little_step), -400, 50);


                        int nStep_B05 = (int)Math.Floor(((double)B05 - conserve_length) / big_step);
                        int nStep_B04 = (int)Math.Ceiling(((double)(2 * B04 + B05 - nStep_B05 * big_step) / 2.0 + conserve_length) / little_step);
                        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step) / 2.0 - conserve_length) / big_step);
                        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + 2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 + conserve_length) / little_step);
                        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                        int scalffold_width = 2 * nStep_B01 * big_step + 2 * nStep_B02 * little_step + 2 * nStep_B03 * big_step + 2 * nStep_B04 * little_step + nStep_B05 * big_step;
                        drawString(string.Format("width of bridge = {0:0.00}m,width of scaffold = {1:0.00}m", xLength / 100.0, scalffold_width / 100.0), -400, 30);
                        //this.parentWin.text_x_input.Text = string.Format("{0}@{6:0.0} {1}@{5:0.0} {2}@{6:0.0} {3}@{5:0.0} {4}@{6:0.0} {3}@{5:0.0} {2}@{6:0.0} {1}@{5:0.0} {0}@{6:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, nStep_B05, little_step / 100.0, big_step / 100.0);


                        int startpoint = (2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + B05 - nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step - 2 * nStep_B01 * big_step) / 2;
                        for (int i = 0; i < nStep_B01; i++)
                        {
                            startpoint += 0;
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B01 * big_step;
                        for (int i = 0; i < nStep_B02; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B02 * little_step;
                        for (int i = 0; i < nStep_B03; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B03 * big_step;
                        for (int i = 0; i < nStep_B04; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B04 * little_step;
                        for (int i = 0; i < nStep_B05; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B05 * big_step;
                        for (int i = 0; i < nStep_B04; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B04 * little_step;
                        for (int i = 0; i < nStep_B03; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B03 * big_step;
                        for (int i = 0; i < nStep_B02; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B02 * little_step;
                        for (int i = 0; i < nStep_B01; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }


                        sectionArea = (2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + B05) * (H01 + H02 + H03) - H02 * B01 - 2 * H03 * B01 - 2 * (H01 + H02 + H03 - H11 - H12) * B03 + h11 * b11 + h12 * b12 + h21 * b21 + h22 * b22 - (H01 + H02 + H03 - H21 - H22) * B05 + h31 * b31 + h32 * b32;
                        sectionArea = sectionArea / 10000.0;

                    } break;
                case 3:
                    {
                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = 0.0; tempY = 0.0;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - H01;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B01; tempY = tempY - H02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - H03;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + H03;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B01; tempY = tempY + H02;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + H01;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = 0.0; tempY = 0.0;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();//此后添加截面中部轮廓


                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02; tempY = -h11 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - h12 - h11 - H11 - H12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b12; tempY = tempY - h12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B03 - b12 - b22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b22; tempY = tempY + h22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H11 - H12 - h21 - h22);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b21; tempY = tempY + h21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B03 - b11 - b21);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02; tempY = -h11 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02 + B03 + B04; tempY = -h31 - H21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - h32 - h31 - H21 - H22);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b32; tempY = tempY - h32;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B05 - b32 - b42;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b42; tempY = tempY + h42;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H21 - H22 - h41 - h42);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b41; tempY = tempY + h41;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B05 - b31 - b41);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02 + B03 + B04; tempY = -h31 - H21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02 + B03 + B04 + B05 + B06; tempY = -h41 - H21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - h42 - h41 - H21 - H22);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b42; tempY = tempY - h42;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B05 - b42 - b32;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b32; tempY = tempY + h32;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H21 - H22 - h31 - h32);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b31; tempY = tempY + h31;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B05 - b41 - b31);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02 + B03 + B04 + B05 + B06; tempY = -h41 - H21;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
                        tempX = B01 + B02 + B03 + 2 * B04 + 2 * B05 + B06; tempY = -h21 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY - (H01 + H02 + H03 - h22 - h21 - H11 - H12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b22; tempY = tempY - h22;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + B03 - b22 - b12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX + b12; tempY = tempY + h12;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempY = tempY + (H01 + H02 + H03 - H11 - H12 - h11 - h12);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - b11; tempY = tempY + h11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = tempX - (B03 - b21 - b11);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        tempX = B01 + B02 + B03 + 2 * B04 + 2 * B05 + B06; tempY = -h21 - H11;
                        gl.Vertex(tempX * xRange, tempY * yRange);
                        gl.End();

                        tempX = 0 - 50 * xTimes / ScralSize; tempY = 0;
                        drawLine(tempX, tempY, H01, true);
                        drawString("H01", tempX - 100 * xTimes / yTimes / ScralSize, tempY - H01 * 1 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempY = tempY - H01;
                        drawLine(tempX, tempY, H02, true);
                        drawString("H02", tempX - 100 * xTimes / yTimes / ScralSize, tempY - H02 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempY = tempY - H02;
                        drawLine(tempX, tempY, H03, true);
                        drawString("H03", tempX - 100 * xTimes / yTimes / ScralSize, tempY - H03 / 2 - 4 * yTimes / xTimes / ScralSize);
                        tempX = 0; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B01, false);
                        drawString("B01", tempX + B01 / 2 - 50 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B01; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B02, false);
                        drawString("B02", tempX + B02 / 2 - 50 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B02; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B03, false);
                        drawString("B03", tempX + B03 / 2 - 50 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B03; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B04, false);
                        drawString("B04", tempX + B04 / 2 - 50 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B04; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B05, false);
                        drawString("B05", tempX + B05 / 2 - 50 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);
                        tempX = tempX + B05; tempY = -(H01 + H02 + H03) - 20 * yTimes / xTimes / ScralSize;
                        drawLine(tempX, tempY, B06, false);
                        drawString("B06", tempX + B06 / 2 - 50 * xTimes / yTimes / ScralSize, tempY - 20 * yTimes / xTimes / ScralSize);


                        tempX = B01 + B02 + b11 + (B03 - b11 - b21) / 2; tempY = 0;
                        drawLine(tempX, tempY, H11, true);
                        drawString("H11", tempX - 100 * xTimes / yTimes / ScralSize, tempY - H11 / 2 - 6 * yTimes / xTimes / ScralSize);
                        tempX = B01 + B02 + b12 + (B03 - b12 - b22) / 2; tempY = -(H01 + H02 + H03 - H12);
                        drawLine(tempX, tempY, H12, true);
                        drawString("H12", tempX - 100 * xTimes / yTimes / ScralSize, tempY - H12 / 2 - 6 * yTimes / xTimes / ScralSize);
                        tempX = B01 + B02 + B03 + B04 + b31 + (B03 - b31 - b21) / 2; tempY = 0;
                        drawLine(tempX, tempY, H21, true);
                        drawString("H21", tempX - 100 * xTimes / yTimes / ScralSize, tempY - H21 / 2 - 6 * yTimes / xTimes / ScralSize);
                        tempX = B01 + B02 + B03 + B04 + b32 + (B03 - b32 - b22) / 2; tempY = -(H01 + H02 + H03 - H22);
                        drawLine(tempX, tempY, H22, true);
                        drawString("H22", tempX - 100 * xTimes / yTimes / ScralSize, tempY - H22 / 2 - 6 * yTimes / xTimes / ScralSize);

                        tempX = B01 + B02 + b11 * 2 / 3; tempY = -(H11 + h11 / 2);
                        drawString("(1,1)", tempX, tempY);
                        tempX = B01 + B02 + b12 * 2 / 3; tempY = -(H01 + H02 + H03 - H12 - h12 / 2);
                        drawString("(1,2)", tempX, tempY);
                        tempX = B01 + B02 + B03 + b21 * 2 / 3 - 80 * xTimes; tempY = -(H11 + h21 / 2);
                        drawString("(2,1)", tempX, tempY);
                        tempX = B01 + B02 + B03 + b22 * 2 / 3 - 80 * xTimes; tempY = -(H01 + H02 + H03 - H12 - h22 / 2);
                        drawString("(2,2)", tempX, tempY);
                        tempX = B01 + B02 + B03 + B04 + b31 * 2 / 3; tempY = -(H21 + h31 / 2);
                        drawString("(3,1)", tempX, tempY);
                        tempX = B01 + B02 + B03 + B04 + b32 * 2 / 3; tempY = -(H01 + H02 + H03 - H22 - h32 / 2);
                        drawString("(3,2)", tempX, tempY);
                        tempX = B01 + B02 + B03 + B04 + B05 + b41 * 2 / 3 - 80 * xTimes; tempY = -(H21 + h41 / 2);
                        drawString("(4,1)", tempX, tempY);
                        tempX = B01 + B02 + B03 + B04 + B05 + b42 * 2 / 3 - 80 * xTimes; tempY = -(H01 + H02 + H03 - H22 - h42 / 2);
                        drawString("(4,2)", tempX, tempY);

                        drawString(string.Format("big step={0}cm,little step={1}cm", big_step, little_step), -450, 50);

                        int nStep_B06 = (int)Math.Ceiling(((double)B06 + conserve_length) / little_step);
                        int nStep_B05 = (int)Math.Floor(((double)(2 * B05 + B06 - nStep_B06 * little_step) / 2.0 - conserve_length) / big_step);
                        int nStep_B04 = (int)Math.Ceiling(((double)(2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step) / 2.0 + conserve_length) / little_step);
                        int nStep_B03 = (int)Math.Floor(((double)(2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step) / 2.0 - conserve_length) / big_step);
                        int nStep_B02 = (int)Math.Ceiling(((double)(2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step) / 2.0 + conserve_length) / little_step);
                        int nStep_B01 = (int)Math.Ceiling((double)(2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step) / 2.0 / big_step);
                        int scalffold_width = 2 * nStep_B01 * big_step + 2 * nStep_B02 * little_step + 2 * nStep_B03 * big_step + 2 * nStep_B04 * little_step + 2 * nStep_B05 * big_step + nStep_B06 * little_step;
                        drawString(string.Format("width of bridge = {0:0.00}m,width of scaffold = {1:0.00}m", xLength / 100.0, scalffold_width / 100.0), -450, 30);
                        //this.parentWin.text_x_input.Text = string.Format("{0}@{7:0.0} {1}@{6:0.0} {2}@{7:0.0} {3}@{6:0.0} {4}@{7:0.0} {5}@{6:0.0} {4}@{7:0.0} {3}@{6:0.0} {2}@{7:0.0} {1}@{6:0.0} {0}@{7:0.0}", nStep_B01, nStep_B02, nStep_B03, nStep_B04, nStep_B05, nStep_B05, little_step / 100.0, big_step / 100.0);
                        int startpoint = (2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06 - nStep_B06 * little_step - 2 * nStep_B05 * big_step - 2 * nStep_B04 * little_step - 2 * nStep_B03 * big_step - 2 * nStep_B02 * little_step - 2 * nStep_B01 * big_step) / 2;
                        for (int i = 0; i < nStep_B01; i++)
                        {
                            startpoint += 0;
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B01 * big_step;
                        for (int i = 0; i < nStep_B02; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B02 * little_step;
                        for (int i = 0; i < nStep_B03; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B03 * big_step;
                        for (int i = 0; i < nStep_B04; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B04 * little_step;
                        for (int i = 0; i < nStep_B05; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B05 * big_step;
                        for (int i = 0; i < nStep_B06; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B06 * little_step;
                        for (int i = 0; i < nStep_B05; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B05 * big_step;
                        for (int i = 0; i < nStep_B04; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B04 * little_step;
                        for (int i = 0; i < nStep_B03; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }
                        startpoint += nStep_B03 * big_step;
                        for (int i = 0; i < nStep_B02; i++)
                        {
                            tempX = startpoint + i * little_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, little_step, false, true, true, 1);
                        }
                        startpoint += nStep_B02 * little_step;
                        for (int i = 0; i < nStep_B01; i++)
                        {
                            tempX = startpoint + i * big_step; tempY = -(H01 + H02 + H03) - 10 * yTimes / xTimes / ScralSize;
                            drawLine(tempX, tempY, big_step, false, true, true, 1);
                        }



                        sectionArea = (2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06) * (H01 + H02 + H03) - H02 * B01 - 2 * H03 * B01 - 2 * (H01 + H02 + H03 - H11 - H12) * B03 + h11 * b11 + h12 * b12 + h21 * b21 + h22 * b22 - 2 * (H01 + H02 + H03 - H21 - H22) * B05 + h31 * b31 + h32 * b32 + h41 * b41 + h42 * b42;
                        sectionArea = sectionArea / 10000.0;

                    } break;
            }

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            openflag = false;
            this.parentWin.section_form_closed = true;

        }

        private void check_textview_Clicked(object sender, RoutedEventArgs e)
        {
            charFlag = !charFlag;
            drawSection(combo_select.SelectedIndex);
        }

        private void text_H01_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_H01.Text == "")
                H01 = 1;
            else
                H01 = int.Parse(text_H01.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_H02_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_H02.Text == "")
                H02 = 1;
            else
                H02 = int.Parse(text_H02.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_H03_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_H03.Text == "")
                H03 = 1;
            else
                H03 = int.Parse(text_H03.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_B01_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_B01.Text == "")
                B01 = 1;
            else
                B01 = int.Parse(text_B01.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_B02_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_B02.Text == "")
                B02 = 1;
            else
                B02 = int.Parse(text_B02.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_B03_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_B03.Text == "")
                B03 = 1;
            else
                B03 = int.Parse(text_B03.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_B04_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_B04.Text == "")
                B04 = 1;
            else
                B04 = int.Parse(text_B04.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_B05_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_B05.Text == "")
                B05 = 1;
            else
                B05 = int.Parse(text_B05.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_B06_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_B06.Text == "")
                B06 = 1;
            else
                B06 = int.Parse(text_B06.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_h11_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_h11.Text == "")
               h11 = 1;
            else
                h11 = int.Parse(text_h11.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_b11_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_b11.Text == "")
                b11 = 1;
            else
                b11 = int.Parse(text_b11.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_h12_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_h12.Text == "")
                h12 = 1;
            else
                h12 = int.Parse(text_h12.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_b12_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_b12.Text == "")
                b12 = 1;
            else
                b12 = int.Parse(text_b12.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_h21_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_h21.Text == "")
                h21 = 1;
            else
                h21 = int.Parse(text_h21.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_b21_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_b21.Text == "")
                b21 = 1;
            else
                b21 = int.Parse(text_b21.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_h22_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_h22.Text == "")
                h22 = 1;
            else
                h22 = int.Parse(text_h22.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_b22_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_b22.Text == "")
                b22 = 1;
            else
                b22 = int.Parse(text_b22.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_h31_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_h31.Text == "")
                h31 = 1;
            else
                h31 = int.Parse(text_h31.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_b31_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_b31.Text == "")
                b31 = 1;
            else
                b31 = int.Parse(text_b31.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_h32_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_h32.Text == "")
                h32 = 1;
            else
                h32 = int.Parse(text_h32.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_b32_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_b32.Text == "")
                b32 = 1;
            else
                b32 = int.Parse(text_b32.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_h41_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_h41.Text == "")
                h41 = 1;
            else
                h41 = int.Parse(text_h41.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_b41_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_b41.Text == "")
                b41 = 1;
            else
                b41 = int.Parse(text_b41.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_h42_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_h42.Text == "")
                h42 = 1;
            else
                h42 = int.Parse(text_h42.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_b42_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_b42.Text == "")
                b42 = 1;
            else
                b42 = int.Parse(text_b42.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_H11_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_H11.Text == "")
                H11 = 1;
            else
                H11 = int.Parse(text_H11.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_H12_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_H12.Text == "")
                H12 = 1;
            else
                H12 = int.Parse(text_H12.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_H21_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_H21.Text == "")
                H21 = 1;
            else
                H21 = int.Parse(text_H21.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void text_H22_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (openflag == false)
                return;
            if (text_H22.Text == "")
                H22 = 1;
            else
                H22 = int.Parse(text_H22.Text);
            drawSection(combo_select.SelectedIndex);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            big_step = int.Parse(text_big.Text);
            little_step = int.Parse(text_little.Text);
            if (openflag == false)
                return;
            drawSection(combo_select.SelectedIndex);
            
        }



    }
}

