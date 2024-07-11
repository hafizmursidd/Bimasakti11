﻿namespace PMT01700SERVICE
{
    public class PMT01700Utilities
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async IAsyncEnumerable<T> PMT01700GetListStream<T>(List<T> poParameter)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            foreach (T item in poParameter)
            {
                yield return item;
            }
        }
    }
}
