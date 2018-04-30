﻿// The MIT License (MIT)
// 
// Copyright (c) 2015-2017 Rasmus Mikkelsen
// Copyright (c) 2015-2017 eBay Software Foundation
// https://github.com/eventflow/EventFlow
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventFlow.Extensions;

namespace EventFlow.Core.ObjectStreams
{
    public class InMemoryObjectStream<T> : ObjectStream<T>
    {
        private readonly IEnumerable<T> _stream;
        private readonly int _batchSize;

        public InMemoryObjectStream(
            IEnumerable<T> stream,
            int batchSize = -1)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _batchSize = batchSize;
        }

        protected override IEnumerable<Task<IReadOnlyCollection<T>>> Iterate()
        {
            if (_batchSize > 0)
            {
                foreach (var partition in _stream.Partition(_batchSize))
                {
                    yield return Task.FromResult(partition);
                }
            }
            else
            {
                yield return Task.FromResult<IReadOnlyCollection<T>>(_stream.ToList());
            }
        }
    }
}