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
        private float rotation = 0.0f;
        private int xLength = 0;///x方向实际总长
        private int yLength = 0;///y方向实际总长
        private double xTimes = 1.0;///x方向缩放倍数
        private double yTimes = 1.0;///y方向缩放倍数
        private double xRange = 0;///x方向图形尺寸
        private double yRange = 0;///y方向图形尺寸
        private double ScralSize = 1.0;//鼠标缩放因数
        public double bridgeLength = 0.0;
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
            drawSection(0);
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

        private void btn_test_Click(object sender, RoutedEventArgs e)
        {
            //setXmlValue();
            //getXmlValue();
            getXmlValue();
            //drawSection(0, 0);
            //OpenGL gl = openGLControl.OpenGL;
            //gl.Translate(-xTimes / 2, yTimes / 2, 0);
            
            //xLength = 1000;
            //yLength = 500;
            //xTimes = 0.8;
            //yTimes = 0.8;
            //xRange = 2 * xTimes / xLength;
            //yRange = 2 * yTimes / yLength;
            //drawLine(0, 0, 1000, 500);
            //drawString("11", 0, 0);
            //drawLine(0, 0, -2, 2);
            //drawOneSection();
        }

        private void text_h01_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void setXmlValue()
        {
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "sectionconfig.xml";
            //MessageBox.Show(XmlHelper.getXmlElementValue(xmlpath, "内部轮廓", "h11"));
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "H01", "20");
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "H02", "20");
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "H03", "110");

            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B01", "200");
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B02", "50");
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B03", "250");
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B04", "50");
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B05", "250");
            XmlHelper.setXmlElementValue(xmlpath, "外部轮廓", "B06", "50");

            XmlHelper.setXmlElementValue(xmlpath, "上下板厚", "H11", "30");
            XmlHelper.setXmlElementValue(xmlpath, "上下板厚", "H12", "30");
            XmlHelper.setXmlElementValue(xmlpath, "上下板厚", "H21", "30");
            XmlHelper.setXmlElementValue(xmlpath, "上下板厚", "H22", "30");

            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h11", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b11", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h12", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b12", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h21", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b21", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h22", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b22", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h31", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b31", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h32", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b32", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h41", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b41", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "h42", "30");
            XmlHelper.setXmlElementValue(xmlpath, "斜角尺寸", "b42", "30");

        }

        private void getXmlValue()
        {
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "sectionconfig.xml";
            //MessageBox.Show(XmlHelper.getXmlElementValue(xmlpath, "内部轮廓", "h11"));
            text_H01.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "H01");
            H01 = int.Parse(text_H01.Text);
            text_H02.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "H02");
            H02 = int.Parse(text_H02.Text);
            text_H03.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "H03");
            H03 = int.Parse(text_H03.Text);

            text_B01.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B01");
            B01 = int.Parse(text_B01.Text);
            text_B02.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B02");
            B02 = int.Parse(text_B02.Text);
            text_B03.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B03");
            B03 = int.Parse(text_B03.Text);
            text_B04.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B04");
            B04 = int.Parse(text_B04.Text);
            text_B05.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B05");
            B05 = int.Parse(text_B05.Text);
            text_B06.Text = XmlHelper.getXmlElementValue(xmlpath, "外部轮廓", "B06");
            B06 = int.Parse(text_B06.Text);

            text_H11.Text = XmlHelper.getXmlElementValue(xmlpath, "上下板厚", "H11");
            H11 = int.Parse(text_H11.Text);
            text_H12.Text = XmlHelper.getXmlElementValue(xmlpath, "上下板厚", "H12");
            H12 = int.Parse(text_H12.Text);
            text_H21.Text = XmlHelper.getXmlElementValue(xmlpath, "上下板厚", "H21");
            H21 = int.Parse(text_H21.Text);
            text_H22.Text = XmlHelper.getXmlElementValue(xmlpath, "上下板厚", "H22");
            H22 = int.Parse(text_H22.Text);

            text_h11.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h11");
            h11 = int.Parse(text_h11.Text);
            text_b11.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b11");
            b11 = int.Parse(text_b11.Text);
            text_h12.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h12");
            h12 = int.Parse(text_h12.Text);
            text_b12.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b12");
            b12 = int.Parse(text_b12.Text);
            text_h21.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h21");
            h21 = int.Parse(text_h21.Text);
            text_b21.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b21");
            b21 = int.Parse(text_b21.Text);
            text_h22.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h22");
            h22 = int.Parse(text_h22.Text);
            text_b22.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b22");
            b22 = int.Parse(text_b22.Text);
            text_h31.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h31");
            h31 = int.Parse(text_h31.Text);
            text_b31.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b31");
            b31 = int.Parse(text_b31.Text);
            text_h32.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h32");
            h32 = int.Parse(text_h32.Text);
            text_b32.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b32");
            b32= int.Parse(text_b32.Text);
            text_h41.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h41");
            h41 = int.Parse(text_h41.Text);
            text_b41.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b41");
            b41 = int.Parse(text_b41.Text);
            text_h42.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "h42");
            h42 = int.Parse(text_h42.Text);
            text_b42.Text = XmlHelper.getXmlElementValue(xmlpath, "斜角尺寸", "b42"); 
            b42 = int.Parse(text_b42.Text);
            xmlLoaded = true;

            

        }

        private void combo_select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xmlLoaded == false)
                getXmlValue();
            drawSection(combo_select.SelectedIndex);
        }


        private void drawLine(double nX, double nY, double Length, bool Col, bool IsArrow = true, bool WithBorder = true)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Color(1.0f, 0.0f, 0.0f);
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
            int w = Convert.ToInt32(ww * (x *xRange+1-xTimes/2)/2);//)
            int h = Convert.ToInt32(hh * (y * yRange + 1+yTimes/2)/2);//) 
            gl.DrawText(w, h, 0, 255, 0, "宋体", 10, info);
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
                    } break;
                case 1:
                    {
                        xLength = 2 * B01 + 2 * B02 + 2 * B03 + B04;
                    } break;
                case 2:
                    {
                        xLength = 2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + B05;
                    } break;
                case 3:
                    {
                        xLength = 2 * B01 + 2 * B02 + 2 * B03 + 2 * B04 + 2 * B05 + B06;
                    } break;
            }

            yLength = H01 + H02 + H03;
            xRange = xTimes / xLength;
            yRange = yTimes / yLength;
            //gl.Translate(-1, 1, 0);


            gl.Translate(-xTimes / 2, yTimes / 2, 0);
            gl.Color(1.0f, 1.0f, 1.0f);//设置当前色为白色
            double tempX = 0.0, tempY = 0.0;
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
                    } break;
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.parentWin.section_from_closed = true;
        }

    }
}

