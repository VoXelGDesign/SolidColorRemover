using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidColorRemover
{
    internal class ThreadManagerASM : ThreadManager
    {
        ThreadManagerASM(int numberOfThreads)
        {
            ThreadPool.SetMinThreads(numberOfThreads, numberOfThreads);
            ThreadPool.SetMaxThreads(numberOfThreads, numberOfThreads);

            this.jobsQueue = JobsQueueFactory.CreateJobQueue();

            foreach(var job in this.jobsQueue)
            {
                var task = Task.Factory.StartNew(() => doTask(job));
            }
        }
        protected override void doTask(Job jobToDo)
        {
            throw new NotImplementedException();
        }
    }
}
