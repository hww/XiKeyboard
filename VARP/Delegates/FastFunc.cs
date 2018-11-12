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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VARP.Delegates
{
    /// <summary>Collection of Function<TRes> organized as Linked List & Table</summary>
    public class FastFunc<TResult>
    {
        LinkedList<System.Func<TResult>> Delegates = new LinkedList<System.Func<TResult>>();
        Dictionary<System.Func<TResult>, LinkedListNode<System.Func<TResult>>> Lookup = new Dictionary<System.Func<TResult>, LinkedListNode<System.Func<TResult>>>();

        public void Add(System.Func<TResult> function)
        {
            if (Lookup.ContainsKey(function)) return;
            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Func<TResult> function)
        {
            LinkedListNode<System.Func<TResult>> node;
            if (Lookup.TryGetValue(function, out node))
            {
                Lookup.Remove(function);
                Delegates.Remove(node);
            }
        }

        public void Call()
        {
            var node = Delegates.First;
            while (node != null)
            {
                node.Value();
                node = node.Next;
            }
        }

        public void Clear()
        {
            Lookup.Clear();
            Delegates.Clear();
        }

        public LinkedList<System.Func<TResult>> GetDelegates()
        {
            return Delegates;
        }

    }

    /// <summary>Collection of Function<A,TRes> organized as Linked List & Table</summary>
    public class FastFunc<A, TResult>
    {
        LinkedList<System.Func<A, TResult>> Delegates = new LinkedList<System.Func<A, TResult>>();
        Dictionary<System.Func<A, TResult>, LinkedListNode<System.Func<A, TResult>>> Lookup = new Dictionary<System.Func<A, TResult>, LinkedListNode<System.Func<A, TResult>>>();

        public void Add(System.Func<A, TResult> function)
        {
            if (Lookup.ContainsKey(function)) return;
            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Func<A, TResult> function)
        {
            LinkedListNode<System.Func<A, TResult>> node;
            if (Lookup.TryGetValue(function, out node))
            {
                Lookup.Remove(function);
                Delegates.Remove(node);
            }
        }

        public void Call(A a)
        {
            var node = Delegates.First;
            while (node != null)
            {
                node.Value(a);
                node = node.Next;
            }
        }

        public void Clear()
        {
            Lookup.Clear();
            Delegates.Clear();
        }

        public LinkedList<System.Func<A, TResult>> GetDelegates()
        {
            return Delegates;
        }
    }

    /// <summary>Collection of Function<A,B,TRes> organized as Linked List & Table</summary>
    public class FastFunc<A, B, TResult>
    {
        LinkedList<System.Func<A, B, TResult>> Delegates = new LinkedList<System.Func<A, B, TResult>>();
        Dictionary<System.Func<A, B, TResult>, LinkedListNode<System.Func<A, B, TResult>>> Lookup = new Dictionary<System.Func<A, B, TResult>, LinkedListNode<System.Func<A, B, TResult>>>();

        public void Add(System.Func<A, B, TResult> function)
        {
            if (Lookup.ContainsKey(function)) return;
            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Func<A, B, TResult> function)
        {
            LinkedListNode<System.Func<A, B, TResult>> node;
            if (Lookup.TryGetValue(function, out node))
            {
                Lookup.Remove(function);
                Delegates.Remove(node);
            }
        }

        public void Call(A a, B b)
        {
            var node = Delegates.First;
            while (node != null)
            {
                node.Value(a, b);
                node = node.Next;
            }
        }

        public void Clear()
        {
            Lookup.Clear();
            Delegates.Clear();
        }

        public LinkedList<System.Func<A, B, TResult>> GetDelegates()
        {
            return Delegates;
        }
    }

    /// <summary>Collection of Function<A,B,C,TRes> organized as Linked List & Table</summary>
    public class FastFunc<A, B, C, TResult>
    {
        LinkedList<System.Func<A, B, C, TResult>> Delegates = new LinkedList<System.Func<A, B, C, TResult>>();
        Dictionary<System.Func<A, B, C, TResult>, LinkedListNode<System.Func<A, B, C, TResult>>> Lookup = new Dictionary<System.Func<A, B, C, TResult>, LinkedListNode<System.Func<A, B, C, TResult>>>();

        public void Add(System.Func<A, B, C, TResult> function)
        {
            if (Lookup.ContainsKey(function)) return;
            Lookup[function] = Delegates.AddLast(function);
        }

        public void Remove(System.Func<A, B, C, TResult> function)
        {
            LinkedListNode<System.Func<A, B, C, TResult>> node;
            if (Lookup.TryGetValue(function, out node))
            {
                Lookup.Remove(function);
                Delegates.Remove(node);
            }
        }

        public void Call(A a, B b, C c)
        {
            var node = Delegates.First;
            while (node != null)
            {
                node.Value(a, b, c);
                node = node.Next;
            }
        }

        public void Clear()
        {
            Lookup.Clear();
            Delegates.Clear();
        }

        public LinkedList<System.Func<A, B, C, TResult>> GetDelegates()
        {
            return Delegates;
        }
    }
}
