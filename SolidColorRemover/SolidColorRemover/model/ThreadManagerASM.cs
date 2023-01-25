using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;





namespace SolidColorRemover.model
{
    internal unsafe class ThreadManagerASM
    {
        private int[] pixels;
        private Color _colorToRemove = new Color();
        private int _splitSize;

        private int lowColorBoundry;
        private int highColorBoundry;

        [DllImport(@"C:\Users\tryne\Documents\VoXelGDesign\SolidColorRemover\SolidColorRemover\SolidColorRemover\resources\ASMLibrary.dll")]
        private static extern int RemoveColor(int* pointer,int lowboundry, int highboundry);

        public TimeSpan TimeElapsed { get; set; }
        public Bitmap Bitmap { get; set; }
        public int Offset { get; set; }

        private int chunkSize;

        public ThreadManagerASM(int numThreads, Bitmap bitmap, Color colorPassed, int offset)
        {
            ThreadPool.SetMaxThreads(numThreads, numThreads);
            ThreadPool.SetMinThreads(numThreads, numThreads);
     
            pixels = new int[bitmap.Width * bitmap.Height];
            _colorToRemove = colorPassed;
            lowColorBoundry = (255 << 24) | (Math.Max(0,_colorToRemove.R - offset) << 16) | (Math.Max(0, _colorToRemove.G - offset) << 8) | (Math.Max(0, _colorToRemove.B - offset));
            highColorBoundry = (255 << 24) | (Math.Min(255, _colorToRemove.R + offset) << 16) | (Math.Min(255, _colorToRemove.G + offset) << 8) | (Math.Min(255, _colorToRemove.B + offset));
            _splitSize = numThreads;
            chunkSize = ((pixels.Length / _splitSize) / 4) * 4;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);
            bitmap.UnlockBits(bitmapData);
           
            var tasks = new Task[_splitSize];

            DateTime beggingDateTime = DateTime.Now;



            for (int i = 0; i < _splitSize; i++)
            {
                int start = i * chunkSize;
                int end = (i + 1) * chunkSize;

                tasks[i] = Task.Factory.StartNew(() => doTask(start,chunkSize/4));

            }

            Task.WaitAll(tasks);

            DateTime endDateTime = DateTime.Now;

            TimeElapsed = (endDateTime - beggingDateTime);

            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            bitmap.UnlockBits(bitmapData);

            Bitmap = bitmap;
            
        }

        private void doTask(int start, int end)
        {            
            fixed (int* ptr = &pixels[start])
            {
               for (int i = 0; i < end; i++)
               {
                        RemoveColor(ptr+(i*4), lowColorBoundry, highColorBoundry);
               }
            }

        }
    }
}
