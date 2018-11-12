// =============================================================================
// MIT License
// 
// Copyright (c) 2018 Valeriya Pudova (hww.github.io)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// =============================================================================

using System.Collections.Generic;

namespace VARP.delegates
{
    /// <summary>Collection of Action organized as Linked List & Table</summary>
    public class FastAction
    {
        private readonly LinkedList<System.Action> delegates = new LinkedList<System.Action>();
        private readonly Dictionary<System.Action, LinkedListNode<System.Action>> lookup = new Dictionary<System.Action, LinkedListNode<System.Action>>();

        public void Add(System.Action function)
        {
            if (lookup.ContainsKey(function)) return;
            lookup[function] = delegates.AddLast(function);
        }
        public void Remove(System.Action function)
        {
            LinkedListNode<System.Action> node;
            if (lookup.TryGetValue(function, out node))
            {
                lookup.Remove(function);
                delegates.Remove(node);
            }
        }
        public void Call()
        {
            var node = delegates.First;
            while (node != null)
            {
                node.Value();
                node = node.Next;
            }
        }
        public void Clear()
        {
            lookup.Clear();
            delegates.Clear();
        }
    }

    /// <summary>Collection of Action<A> organized as Linked List & Table</summary>
    public class FastAction<A>
    {
        private readonly LinkedList<System.Action<A>> delegates = new LinkedList<System.Action<A>>();
        private readonly Dictionary<System.Action<A>, LinkedListNode<System.Action<A>>> lookup = new Dictionary<System.Action<A>, LinkedListNode<System.Action<A>>>();

        public void Add(System.Action<A> function)
        {
            if (lookup.ContainsKey(function)) return;
            lookup[function] = delegates.AddLast(function);
        }
        public void Remove(System.Action<A> function)
        {
            LinkedListNode<System.Action<A>> node;
            if (lookup.TryGetValue(function, out node))
            {
                lookup.Remove(function);
                delegates.Remove(node);
            }
        }
        public void Call(A a)
        {
            var node = delegates.First;
            while (node != null)
            {
                node.Value(a);
                node = node.Next;
            }
        }
        public void Clear()
        {
            lookup.Clear();
            delegates.Clear();
        }
    }

    /// <summary>Collection of Action<A,B> organized as Linked List & Table</summary>
    public class FastAction<A, B>
    {
        private readonly LinkedList<System.Action<A, B>> delegates = new LinkedList<System.Action<A, B>>();
        private readonly Dictionary<System.Action<A, B>, LinkedListNode<System.Action<A, B>>> lookup = new Dictionary<System.Action<A, B>, LinkedListNode<System.Action<A, B>>>();

        public void Add(System.Action<A, B> function)
        {
            if (lookup.ContainsKey(function)) return;
            lookup[function] = delegates.AddLast(function);
        }
        public void Remove(System.Action<A, B> function)
        {
            LinkedListNode<System.Action<A, B>> node;
            if (lookup.TryGetValue(function, out node))
            {
                lookup.Remove(function);
                delegates.Remove(node);
            }
        }
        public void Call(A a, B b)
        {
            var node = delegates.First;
            while (node != null)
            {
                node.Value(a, b);
                node = node.Next;
            }
        }
        public void Clear()
        {
            lookup.Clear();
            delegates.Clear();
        }
    }
    
    /// <summary>Collection of Action<A,B,C> organized as Linked List & Table</summary>
    public class FastAction<A, B, C>
    {
        private readonly LinkedList<System.Action<A, B, C>> delegates = new LinkedList<System.Action<A, B, C>>();
        private readonly Dictionary<System.Action<A, B, C>, LinkedListNode<System.Action<A, B, C>>> lookup = new Dictionary<System.Action<A, B, C>, LinkedListNode<System.Action<A, B, C>>>();

        public void Add(System.Action<A, B, C> function)
        {
            if (lookup.ContainsKey(function)) return;
            lookup[function] = delegates.AddLast(function);
        }
        public void Remove(System.Action<A, B, C> function)
        {
            LinkedListNode<System.Action<A, B, C>> node;
            if (lookup.TryGetValue(function, out node))
            {
                lookup.Remove(function);
                delegates.Remove(node);
            }
        }
        public void Call(A a, B b, C c)
        {
            var node = delegates.First;
            while (node != null)
            {
                node.Value(a, b, c);
                node = node.Next;
            }
        }
        public void Clear()
        {
            lookup.Clear();
            delegates.Clear();
        }
    }
}


