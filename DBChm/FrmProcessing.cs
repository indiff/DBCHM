using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBCHM
{
    public partial class FrmProcessing : Form
    {
        private static Image m_Image = null;

        private EventHandler evtHandler = null;

        private Action<object> workAction = null;
        private object workActionArg = null;

        private Thread workThread = null;

        public string Message
        {
            get
            {
                return lbMessage.Text;
            }
            set
            {
                lbMessage.Text = value;
            }
        }

        public bool WorkCompleted = false;

        public Exception WorkException
        { get; private set; }

        public void SetWorkAction(Action<object> workAction, object arg)
        {
            this.workAction = workAction;
            this.workActionArg = arg;
        }

        public FrmProcessing(string msg)
        {
            InitializeComponent();
            this.Message = msg;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_Image != null)
            {
                //获得当前gif动画下一步要渲染的帧。
                UpdateImage();

                //将获得的当前gif动画需要渲染的帧显示在界面上的某个位置。
                int x = (int)(panImage.ClientRectangle.Width - m_Image.Width) / 2;
                int y = 0;
                //e.Graphics.DrawImage(m_Image, new Rectangle(x, y, m_Image.Width, m_Image.Height));
                panImage.CreateGraphics().DrawImage(m_Image, new Rectangle(x, y, m_Image.Width, m_Image.Height));
            }
            if (this.WorkCompleted)
            {
                this.Close();
            }
        }

        private void FrmProcessing_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(this.Owner.Left, this.Owner.Top);
                //MessageBox.Show(string.Format("X={0},Y={1}", this.Owner.Left, this.Owner.Top));
                this.Width = this.Owner.Width;
                this.Height = this.Owner.Height;
            }
            else
            {
                Rectangle screenRect = Screen.PrimaryScreen.WorkingArea;
                this.Location = new Point((screenRect.Width - this.Width) / 2, (screenRect.Height - this.Height) / 2);
            }

            //为委托关联一个处理方法
            evtHandler = new EventHandler(OnImageAnimate);

            if (m_Image == null)
            {
                Assembly assy = Assembly.GetExecutingAssembly();
                //获取要加载的gif动画文件
                m_Image = Image.FromStream(assy.GetManifestResourceStream(assy.GetName().Name + ".Resources.loading.gif"));
            }
            //调用开始动画方法
            BeginAnimate();
        }

        //开始动画方法

        private void BeginAnimate()
        {
            if (m_Image != null)
            {
                //当gif动画每隔一定时间后，都会变换一帧，那么就会触发一事件，该方法就是将当前image每变换一帧时，都会调用当前这个委托所关联的方法。
                ImageAnimator.Animate(m_Image, evtHandler);
            }
        }

        //委托所关联的方法

        private void OnImageAnimate(Object sender, EventArgs e)
        {
            //该方法中，只是使得当前这个winform重绘，然后去调用该winform的OnPaint（）方法进行重绘)
            this.Invalidate();
        }

        //获得当前gif动画的下一步需要渲染的帧，当下一步任何对当前gif动画的操作都是对该帧进行操作)

        private void UpdateImage()
        {
            ImageAnimator.UpdateFrames(m_Image);
        }

        //关闭显示动画，该方法可以在winform关闭时，或者某个按钮的触发事件中进行调用，以停止渲染当前gif动画。

        private void StopAnimate()
        {
            m_Image = null;
            ImageAnimator.StopAnimate(m_Image, evtHandler);
        }

        private void FrmProcessing_Shown(object sender, EventArgs e)
        {
            if (this.workAction != null)
            {
                workThread = new Thread(ExecWorkAction);
                workThread.IsBackground = true;
                workThread.Start();
            }
        }

        private void ExecWorkAction()
        {
            try
            {
                var workTask = new Task((arg) =>
                {
                    this.workAction(arg);
                },
                this.workActionArg);

                workTask.Start();
                Task.WaitAll(workTask);
            }
            catch (Exception ex)
            {
                this.WorkException = ex;
            }
            finally
            {
                this.WorkCompleted = true;
            }
        }
    }
}