using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidColorRemover.model
{
    internal class Job
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public Job(int start, int end)
        {
            StartIndex = start;
            EndIndex = end;
        }
    }
}
