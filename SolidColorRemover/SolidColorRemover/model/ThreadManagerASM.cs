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

        public ThreadManagerASM(int numThreads, Bitmap bitmap, Color colorPassed, int offset)
        {
            ThreadPool.SetMaxThreads(numThreads, numThreads);
            ThreadPool.SetMinThreads(numThreads, numThreads);
            _splitSize = numThreads;

            int bitmapSize = bitmap.Width * bitmap.Height;
            int sizeOfPixelsArray = (((bitmapSize) / 4) * 4)+4;
            int chunkSize = ((sizeOfPixelsArray / _splitSize) / 4) * 4;

            pixels = new int[sizeOfPixelsArray];
            _colorToRemove = colorPassed;
            lowColorBoundry = (255 << 24) | (Math.Max(0,_colorToRemove.R - offset) << 16) | (Math.Max(0, _colorToRemove.G - offset) << 8) | (Math.Max(0, _colorToRemove.B - offset));
            highColorBoundry = (255 << 24) | (Math.Min(255, _colorToRemove.R + offset) << 16) | (Math.Min(255, _colorToRemove.G + offset) << 8) | (Math.Min(255, _colorToRemove.B + offset));
            
            

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);
            Marshal.Copy(bitmapData.Scan0, pixels, 0, bitmapSize);
            bitmap.UnlockBits(bitmapData);
           
            var tasks = new Task[_splitSize+1];

            DateTime beggingDateTime = DateTime.Now;



            for (int i = 0; i < _splitSize; i++)
            {
                int start = i * chunkSize;
                int end = (i + 1) * chunkSize;

                tasks[i] = Task.Factory.StartNew(() => doTask(start,chunkSize/4));

            }

            int donePixels = _splitSize * chunkSize;
            if (_splitSize * chunkSize < pixels.Length)
            {
                tasks[_splitSize] = Task.Factory.StartNew(() => doTask(donePixels - 1, (pixels.Length - donePixels) / 4));
            }
            else
            {
                tasks[_splitSize] = Task.Factory.StartNew(() => { return; });
            }

            Task.WaitAll(tasks);

            DateTime endDateTime = DateTime.Now;

            TimeElapsed = (endDateTime - beggingDateTime);

            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);
            Marshal.Copy(pixels, 0, bitmapData.Scan0, bitmapSize);
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
