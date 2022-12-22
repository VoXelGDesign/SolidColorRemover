using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace SolidColorRemover.model
{
    internal class ThreadManagerCS : ThreadManager
    {

        
        private int[] pixels;
        private Color _colorToRemove = new Color();
        private int _splitSize = 200;


        public TimeSpan TimeElapsed { get; set; } 
        public Bitmap Bitmap { get; set; }
        public int Offset { get; set; }

        public ThreadManagerCS(int numThreads, Bitmap bitmap, Color colorPassed, int offset)
        {
            ThreadPool.SetMaxThreads(numThreads, 1);
            ThreadPool.SetMinThreads(numThreads, 1);

            var width = bitmap.Width;
            var height = bitmap.Height;
            pixels = new int[width * height];
            _colorToRemove = colorPassed;
            Offset = offset;
            
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);

            bitmap.UnlockBits(bitmapData);

            int chunkSize = pixels.Length / _splitSize;
            var tasks = new Task[_splitSize];
            
            DateTime beggingDateTime = DateTime.Now;
            
            for (int i = 0; i < _splitSize; i++)
            {
                int start = i * chunkSize;
                int end = (i + 1) * chunkSize;

                tasks[i] = Task.Factory.StartNew(() => doTask(new Job(start, end)));
                
            }

            Task.WaitAll(tasks);

            DateTime endDateTime = DateTime.Now;
            TimeElapsed = (endDateTime - beggingDateTime);

            bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            bitmap.UnlockBits(bitmapData);
            Bitmap = bitmap;


        }
        protected override void doTask(Job task)
        {
            
            for (int i = task.StartIndex; i < task.EndIndex; i++)
            {
                
                int alpha = (pixels[i] >> 24) & 0xff;
                int red = (pixels[i] >> 16) & 0xff;
                int green= (pixels[i] >> 8) & 0xff;
                int blue = pixels[i] & 0xff;


                if ((green > _colorToRemove.G - Offset && green < _colorToRemove.G + Offset) && 
                    (red > _colorToRemove.R - Offset && red < _colorToRemove.R + Offset) && 
                    (blue > _colorToRemove.B - Offset && blue < _colorToRemove.B + Offset)) 
                {
                    pixels[i] = (pixels[i] & 0x00FFFFFF) | 0x00000000;
                }
                
            }
        }
        
    }
}
