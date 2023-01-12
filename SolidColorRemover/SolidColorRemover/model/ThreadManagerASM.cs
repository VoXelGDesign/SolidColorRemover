﻿using System;
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
    internal class ThreadManagerASM : ThreadManager
    {
        private int[] pixels;
        private Color _colorToRemove = new Color();

        private int a;
        private int b;

        [DllImport(@"C:\Users\tryne\Documents\VoXelGDesign\SolidColorRemover\SolidColorRemover\SolidColorRemover\resources\ASMLibrary.dll")]
        private static extern unsafe int RemoveColor(int* pointer,int lowboundry, int highboundry);

        public TimeSpan TimeElapsed { get; set; }
        public Bitmap Bitmap { get; set; }
        public int Offset { get; set; }

        public ThreadManagerASM(int numThreads, Bitmap bitmap, Color colorPassed, int offset)
        {
            ThreadPool.SetMaxThreads(numThreads, 1);
            ThreadPool.SetMinThreads(numThreads, 1);

            var width = bitmap.Width;
            var height = bitmap.Height;
            pixels = new int[width * height];
            _colorToRemove = colorPassed;
            int offset1 = (int)(offset*0.3);
            a = (255 << 24) | (Math.Max(0,_colorToRemove.R - offset) << 16) | (Math.Max(0, _colorToRemove.G - offset) << 8) | (Math.Max(0, _colorToRemove.B - offset));
            b = (255 << 24) | (Math.Min(255, _colorToRemove.R + offset1) << 16) | (Math.Min(255, _colorToRemove.G + offset1) << 8) | (Math.Min(255, _colorToRemove.B + offset1));

            
            int splitSize = pixels.Length / 4;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);

            bitmap.UnlockBits(bitmapData);

            int chunkSize = 4;
            var tasks = new Task[splitSize];


            DateTime beggingDateTime = DateTime.Now;



            for (int i = 0; i < splitSize; i++)
            {
                int start = i * chunkSize;
                int end = (i + 1) * chunkSize;

                tasks[i] = Task.Factory.StartNew(() => doTask(new Job(start, end)));

            }


            Task.WaitAll(tasks);

            DateTime endDateTime = DateTime.Now;
            TimeElapsed = (endDateTime - beggingDateTime);

            bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            bitmap.UnlockBits(bitmapData);
            Bitmap = bitmap;
            


        }

        protected override void doTask(Job jobToDo)
        {
            
            unsafe
            {
                fixed (int* ptr = &pixels[jobToDo.StartIndex])
                {
                   RemoveColor(ptr,  a, b);
                   
                }
            }
        }
    }
}
