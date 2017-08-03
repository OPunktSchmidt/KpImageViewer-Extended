using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Timers;

namespace KaiwaProjects
{
    public class GifImage : IDisposable
    {
        private KpImageViewer KpViewer;
        private Image gif;
        private FrameDimension dimension;
        private int frameCount;
        private int rotation = 0;
        private int currentFrame = 0;
        private Bitmap currentFrameBmp = null;
        private Size currentFrameSize = new Size();
        private bool updating = false;
        private Timer timer = null;
        private double framesPerSecond = 0;
        private bool animationEnabled = true;

        public Size CurrentFrameSize
        {
            get
            {
                return currentFrameSize;
            }
        }

        public void Dispose()
        {
            Lock();
            timer.Enabled = false;
            gif.Dispose();
            gif = null;
            Unlock();

            timer.Dispose();
        }

        public double FPS
        {
            get { return (1000.0 / framesPerSecond); }
            set
            {
                if (value <= 30.0 && value > 0.0)
                {
                    framesPerSecond = 1000.0 / value;

                    if (timer != null)
                    {
                        timer.Interval = framesPerSecond;
                    }
                }
            }
        }

        public bool AnimationEnabled
        {
            get { return animationEnabled; }
            set
            {
                animationEnabled = value;

                if (timer != null)
                {
                    timer.Enabled = animationEnabled;
                }
            }
        }

        public GifImage(KpImageViewer KpViewer, Image img, bool animation, double fps)
        {
            this.updating = true;
            this.KpViewer = KpViewer;
            this.gif = img;
            this.dimension = new FrameDimension(gif.FrameDimensionsList[0]);
            this.frameCount = gif.GetFrameCount(dimension);
            this.gif.SelectActiveFrame(dimension, 0);
            this.currentFrame = 0;
            this.animationEnabled = animation;

            this.timer = new Timer();

            this.updating = false;

            framesPerSecond = 1000.0 / fps; // 15 FPS
            this.timer.Enabled = this.animationEnabled;
            this.timer.Interval = framesPerSecond;
            this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            this.currentFrameBmp = (Bitmap)gif;
            this.currentFrameSize = new Size(currentFrameBmp.Size.Width, currentFrameBmp.Size.Height);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NextFrame();
        }

        public bool Lock()
        {
            if (updating == false)
            {
                while (updating)
                {
                    // Wait
                }

                return true;
            }

            return false;
        }

        public void Unlock()
        {
            updating = false;
        }

        public void NextFrame()
        {
            try
            {
                if (gif != null)
                {
                    if (Lock())
                    {
                        lock (gif)
                        {
                            gif.SelectActiveFrame(this.dimension, this.currentFrame);
                            currentFrame++;

                            if (currentFrame >= this.frameCount)
                            {
                                currentFrame = 0;
                            }

                            OnFrameChanged();
                        }
                    }

                    Unlock();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public int Rotation
        {
            get { return rotation; }
        }

        public void Rotate(int rotation)
        {
            this.rotation = (this.rotation + rotation) % 360;
        }

        private void OnFrameChanged()
        {
            this.currentFrameBmp = (Bitmap)gif;
            this.currentFrameSize = new Size(currentFrameBmp.Size.Width, currentFrameBmp.Size.Height);

            this.KpViewer.InvalidatePanel();
        }

        public Bitmap CurrentFrame
        {
            get
            {
                return currentFrameBmp;
            }
        }

        public int FrameCount
        {
            get { return frameCount; }
        }
    }
}
