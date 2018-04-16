using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImageQuantization
{
     public class node
     {
          public node left;
          public node right;
          public int freq;
          public int value;
          public bool isparent; //every frequeny node is a parent to 2 other nodes

     }

    public  class huffman: PriorityQueue<node>
     {
  
          public string code;
          public node root;
          public node parent;
          public PriorityQueue<node> pq = new PriorityQueue<node>();
          public Dictionary<int, string> d;
          //----------------------
          //class default constructor
          //-----------------------
          public huffman()
          {
               root = null;
               parent = null;
               code = null;
               d = new Dictionary<int, string>();
          }


         //---------------------------------
         // gets the nodes and sorting it according to its frequency
         //----------------------------------
          public PriorityQueue<node> sortnodes(int[,] val)
          {

               for (int i = 0; i < val.GetLength(0); ) // val.getlength(0) gets number of rows in the 2D array 
               {
                    node nwnode = new node();

                    nwnode.freq = val[i, 0];
                    nwnode.value = val[i, 1];
                    nwnode.left = null;
                    nwnode.right = null;
                    pq.Push(nwnode);
                    i++;

               }

               return pq;
          }

       
          public void printtree(node root) // used for debugging purposes
          {

               if (root == null)
               {
                    return;
               }
               Console.Write(root.freq + " ");
               Console.WriteLine();

               printtree(root.left);
               printtree(root.right);


          }






        //prints the bits for each value 
        public void printcode(node root, string code, Dictionary<int, string> d, Dictionary<string, int> dd, Dictionary<int, int> ddd)
        {

            if (root == null)
            {

                return;
            }
            if (root.isparent == false)
            {
                //        Console.WriteLine(root.value + " " + code);
                d.Add(root.value, code); // adds color value and huffman bits  
                dd.Add(code, root.value);// adds huffman bits and color value to write
                ddd.Add(root.value, root.freq);// adds color value and frequency 
            }

            printcode(root.left, code + '0', d, dd, ddd);
            printcode(root.right, code + '1', d, dd, ddd);

            return;

        }
          public node createtree()
          {
               node firstnode = new node();
               node secondnode = new node();
               node newnode = new node();
               node tempnode = new node();


               if (pq.size() == 1) // if only one node remains, set it as root
               {
                    root = parent; 
                    root.left = parent.left;
                    root.right = parent.right;
                    root.freq = parent.freq;
                    pq.pop(); // complexity log N 


               }

               else if (root == null) // takes the value of both child nodes, adds them together 
               {                      // and pushes them out, then pushes the new value into the tree

                    firstnode = pq.top();// O(1)
                    firstnode.freq = pq.top().freq;
                    firstnode.value = pq.top().value;
                    firstnode.left = pq.top().left;
                    firstnode.right = pq.top().right;
                    firstnode.isparent = pq.top().isparent;
                    pq.pop();

                    secondnode = pq.top();
                    secondnode.freq = pq.top().freq;
                    secondnode.value = pq.top().value;
                    secondnode.left = pq.top().left;
                    secondnode.right = pq.top().right;
                    secondnode.isparent = pq.top().isparent;
                    pq.pop();
                    if (secondnode.isparent == true && firstnode.isparent == false) // Switches any parent to the right side
                    {
                         newnode.right = secondnode;
                         newnode.left = firstnode;
                    }
                    else
                    {
                         newnode.right = firstnode;
                         newnode.left = secondnode;
                    }
                         newnode.freq = firstnode.freq + secondnode.freq;
                         parent = newnode;
                         parent.isparent = true;
                         pq.Push(parent);
                         createtree();

                    }
               
               return root;

          }
     }
}
     

