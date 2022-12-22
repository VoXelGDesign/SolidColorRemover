using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidColorRemover.model
{
    internal class ThreadManagerASM : ThreadManager
    {
        ThreadManagerASM(int numberOfThreads)
        {
            ThreadPool.SetMinThreads(numberOfThreads, numberOfThreads);
            ThreadPool.SetMaxThreads(numberOfThreads, numberOfThreads);

           // jobsQueue = JobsQueueFactory.CreateJobQueue();

            foreach (var job in jobsQueue)
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
