using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    public class ListUtil<T>
    {
        public ListNode<T> list;

        public ListUtil()
        {

        }
        public ListUtil(params ListNode<T>[] nodes)
        {
            for (int i=0; i<nodes.Length; i++)
            {
                Add(nodes[i]);
            }
        }
        public ListUtil(params T[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Add(data[i]);
            }
        }


        public void Add(ListNode<T> node)
        {
            if (list != null)
            {
                list.last = node;
                node.next = list;
                list = node;
            }
            else
            {
                list = node;
            }
        }

        public void Add(T dat)
        {
            Add(new ListNode<T>(dat));    
        }

        public void Remove(ListNode<T> node)
        {
            node.Remove();
        }

    }



    public class ListNode<T>
    {
        public T dat;
        public ListNode<T> next;
        public ListNode<T> last;

        public ListNode(T dat)
        {
            this.dat = dat;
        }

        public void Remove()
        {
            if (last != null)
                last.next = next;
            
            if (next != null)
                next.last = last;
        }

    }
}
