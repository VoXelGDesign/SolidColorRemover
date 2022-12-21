using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidColorRemover
{
    
    abstract class ThreadManager
    {
        protected List<Job> jobsQueue = new List<Job>();
        float timeElapsed;
        int numberOfThreads;

        abstract protected void doTask(Job task);
    }
}
