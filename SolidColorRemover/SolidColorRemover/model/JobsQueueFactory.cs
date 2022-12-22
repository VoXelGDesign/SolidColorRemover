using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidColorRemover.model
{
    internal class JobsQueueFactory
    {
        public static List<Job> CreateJobQueue(Bitmap bitmap)
        {
            List<Job> jobs = new List<Job>();
            
            //int rest = 4 - bitmap.Width%4;

            //for (int y = 0; y < bitmap.Height; y++)
            //{
            //    for (int x = 0; x < bitmap.Width - rest; x += 4)
            //    {
            //        jobs.Add(new Job(x, x + 3, y));
            //    }

            //}

            //if (bitmap.Width % 4 > 0)
            //{
            //    for (int y = 0; y < bitmap.Height; y++)
            //        jobs.Add(new Job(bitmap.Width - (bitmap.Width % 4), bitmap.Width - 1, y));
            //}

            return jobs;
        }
    }
}
