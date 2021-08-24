using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

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
        }

        public delegate void OnBufferReached(int bufferIndex);
        public event OnBufferReached onBufferReached;

        ConcurrentQueue<int> bufferQueue;
        uint maxPerBuffer = 1000;
        public int currentIndexBuffer = 0;
        public int CurrentItemInBufer = 0;
        object queueSelectLock = new object();

        public T[][] bufferedList;
        public (int currentIndexBuffer, int CurrentItemInBufer) SelectBufferReturnsIndexItem()
        {
            bool retry = false;
            lock (queueSelectLock)
            {
                if (CurrentItemInBufer == maxPerBuffer)
                {
                    onBufferReached(currentIndexBuffer);
                    if (bufferQueue.TryDequeue(out var item))
                    {
                        currentIndexBuffer = item;
                    }
                    CurrentItemInBufer = 0;
                    if (bufferedList[item] == null)
                    {
                        bufferedList[item] = new T[maxPerBuffer];
                    }
                    return (currentIndexBuffer, CurrentItemInBufer);
                }
                if (!retry)
                {
                    if (bufferedList[currentIndexBuffer] == null)
                    {
                        bufferedList[currentIndexBuffer] = new T[maxPerBuffer];
                    }
                    (int, int) result = (currentIndexBuffer, CurrentItemInBufer);
                    CurrentItemInBufer++;
                    return result;
                }
            }
            return SelectBufferReturnsIndexItem();
        }
    }
}
