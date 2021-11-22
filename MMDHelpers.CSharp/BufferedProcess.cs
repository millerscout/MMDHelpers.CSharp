using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMDHelpers.CSharp
{
    public class BufferedProcess<T> where T : class, new()
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

        private event EventHandler<int> OnBufferCompleted;

        ConcurrentQueue<int> bufferQueue;
        uint maxPerBuffer = 1000;
        private int currentIndexBuffer = 0;
        private int CurrentItemInBuffer = 0;
        object queueSelectLock = new object();

        private T[][] bufferedList;


        /// <summary>
        /// The returns the position of the buffer.
        /// </summary>
        /// <returns></returns>
        private (int currentIndexBuffer, int CurrentItemInBuffer) NextPosition()
        {
            lock (queueSelectLock)
            {
                if (CurrentItemInBuffer == maxPerBuffer - 1)
                {
                    onBufferReached(currentIndexBuffer);

                    var done = false;
                    while (!done)
                    {
                        if (bufferQueue.TryDequeue(out var item))
                        {
                            currentIndexBuffer = item;

                            CurrentItemInBuffer = 0;
                            if (bufferedList[item] == null)
                            {
                                bufferedList[item] = new T[maxPerBuffer];
                            }

                            return (currentIndexBuffer, CurrentItemInBuffer);
                        }
                        else
                        {
                            Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(1).TotalMilliseconds)).Wait();
                        }
                    }
                }

                if (bufferedList[currentIndexBuffer] == null)
                {
                    bufferedList[currentIndexBuffer] = new T[maxPerBuffer];
                }
                (int, int) result = (currentIndexBuffer, CurrentItemInBuffer);
                CurrentItemInBuffer++;
                return result;
            }
        }
        public ref T Next()
        {
            var t = NextPosition();
            if (bufferedList[t.currentIndexBuffer][t.CurrentItemInBuffer] == null)
            {
                bufferedList[t.currentIndexBuffer][t.CurrentItemInBuffer] = new T();
            }
            return ref bufferedList[t.currentIndexBuffer][t.CurrentItemInBuffer];
        }
        public IEnumerable<T> GetRemainingItemsInBuffer()
        {
            var bufferTag = GetLatestBufferIndexItem();
            return bufferedList[bufferTag.currentIndexBuffer].Take(bufferTag.CurrentItemInBufer - 1);
        }
        public ref T[] GetCollectionInBuffer(int BufferIndex)
        {
            return ref bufferedList[BufferIndex];
        }


        /// <summary>
        /// The returns the position of the buffer without changing.
        /// </summary>
        /// <returns></returns>
        public (int currentIndexBuffer, int CurrentItemInBufer) GetLatestBufferIndexItem()
        {
            lock (queueSelectLock)
            {
                return (currentIndexBuffer, CurrentItemInBuffer);
            }
        }

        public void CompleteBuffer(int bufferIndex)
        {
            OnBufferCompleted?.Invoke(this, bufferIndex);
        }
    }
}
