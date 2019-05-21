using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    public class ListUtil<T>
    {
        public ListNode<T> list;
        private int count;

        public ListUtil()
        {
            count = 0;
        }
        public ListUtil(params ListNode<T>[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                Add(nodes[i]);
                count++;
            }
        }
        public ListUtil(params T[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Add(data[i]);
                count++;
            }
        }


        public void Add(ListNode<T> node)
        {
            if (list != null)
            {
                list.last = node;
                node.next = list;
                list = node;
                count++;
            }
            else
            {
                list = node;
                count++;
            }
        }

        public void Add(T dat)
        {
            Add(new ListNode<T>(dat));
            count++;
        }

        public void Remove(ListNode<T> node)
        {
            node.Remove();
            count--;
            if (count <= 1)
            {
             //   list = null;
            }
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
