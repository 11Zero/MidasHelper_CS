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
        private float rotation = 0.0f;
        private int xLength = 0;///x方向实际总长
        private int yLength = 0;///y方向实际总长
        private double xTimes = 1.0;///x方向缩放倍数
        private double yTimes = 1.0;///y方向缩放倍数
        private double xRange = 0;///x方向图形尺寸
        private double yRange = 0;///y方向图形尺寸
        //private OpenGL gl = null;
        public SectionForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
        private void drawLine(int sX, int sY, int eX, int eY,bool isArrow=false, bool withBorder=true)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Begin(SharpGL.Enumerations.BeginMode.Lines);
            gl.Vertex(sX * xRange, sY * yRange);
            gl.Vertex(eX * xRange, eY * yRange);
            gl.End();
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
            xTimes = 0.8;
            yTimes = 0.8;
            xLength = 2;
            yLength = 2;
            xRange = xTimes/xLength;///opengl整个空间大小为-1，-1到1,1，所以画线时需根据实际尺寸及绘图空间进行缩放
            yRange = yTimes/yLength;
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
            gl.Vertex(1.0f,2.0f);
            gl.Vertex(2.0f, 1.0f);
            gl.End();
        }

        private void btn_test_Click(object sender, RoutedEventArgs e)
        {
            drawLine(0,0,1,1);
            //drawLine(0, 0, -2, 2);
            //drawOneSection();
        }

        private void text_h01_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
