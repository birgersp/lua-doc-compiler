using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDC
{
    public class Node<T>
    {
        public List<Node<T>> Children = new List<Node<T>>();
        public T Data;

        public Node(T data)
        {
            Data = data;
        }
    }
}
