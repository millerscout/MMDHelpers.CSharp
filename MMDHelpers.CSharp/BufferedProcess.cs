using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMDHelpers.CSharp
{
    public class BufferedProcess<T> where T : class
    {
        public BufferedProcess(uint MaxItemPerBuffer, uint MaxCollectionBuffer)
        {
            if (MaxCollectionBuffer < 2) MaxCollectionBuffer = 2;

            bufferQueue = new ConcurrentQueue<int>(new List<int>(Convert.ToInt32(MaxCollectionBuffer)));

            
            for (int i = 1; i < MaxCollectionBuffer; i++)
                bufferQueue.Enqueue(i);

            maxPerBuffer = MaxItemPerBuffer;
            bufferedList = new T[MaxCollectionBuffer][];

            this.OnBufferCompleted += BufferedProcess_onBufferCompleted;
        }

        private void BufferedProcess_onBufferCompleted(object sender, int bufferIndex)
        {
            bufferQueue.Enqueue(bufferIndex);
        }

        public delegate void OnBufferReached(int bufferIndex);
        public event OnBufferReached onBufferReached;

        public event EventHandler<int> OnBufferCompleted;

        ConcurrentQueue<int> bufferQueue;
        uint maxPerBuffer = 1000;
        private int currentIndexBuffer = 0;
        private int CurrentItemInBufer = -1;
        object queueSelectLock = new object();

        public T[][] bufferedList;
        /// <summary>
        /// The return value should be completely used before 
        /// </summary>
        /// <returns></returns>
        public (int currentIndexBuffer, int CurrentItemInBufer) SelectBufferReturnsIndexItem()
        {
            lock (queueSelectLock)
            {
                if (CurrentItemInBufer == maxPerBuffer - 1)
                {
                    onBufferReached(currentIndexBuffer);

                    var done = false;
                    while (!done)
                    {
                        if (bufferQueue.TryDequeue(out var item))
                        {
                            currentIndexBuffer = item;

                            CurrentItemInBufer = 0;
                            if (bufferedList[item] == null)
                            {
                                bufferedList[item] = new T[maxPerBuffer];
                            }

                            return (currentIndexBuffer, CurrentItemInBufer);
                        }
                        else
                        {
                            Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(1).TotalMilliseconds));
                        }
                    }
                }

                if (bufferedList[currentIndexBuffer] == null)
                {
                    bufferedList[currentIndexBuffer] = new T[maxPerBuffer];
                }
                (int, int) result = (currentIndexBuffer, CurrentItemInBufer);
                CurrentItemInBufer++;
                return result;
            }
        }

        public void CompleteBuffer(int bufferIndex)
        {
            OnBufferCompleted?.Invoke(this, bufferIndex);
        }
    }
}
