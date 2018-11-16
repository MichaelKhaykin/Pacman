using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class MinHeapTree
    {
        Vertex[] heapArray = new Vertex[0];

        public int Count => heapArray.Length;

        public bool Contains(Vertex thingy)
        {
            for (int i = 0; i < heapArray.Length; i++)
            {
                if (heapArray[i].Equals(thingy))
                {
                    return true;
                }
            }
            return false;
        }
        public void HeapifyUp(int index)
        {
            if (index == 0)
            {
                return;
            }
            int parentIndex = (index - 1) / 2;

            Vertex parentValue = heapArray[parentIndex];
            Vertex indexValue = heapArray[index];

            if (indexValue.CompareTo(parentValue) < 0)
            {
                Vertex temp = heapArray[index];
                heapArray[index] = heapArray[parentIndex];
                heapArray[parentIndex] = temp;
                HeapifyUp(parentIndex);
            }
        }
        public void Add(Vertex value)
        {
            Vertex[] temp = new Vertex[heapArray.Length + 1];
            for (int i = 0; i < heapArray.Length; i++)
            {
                temp[i] = heapArray[i];
            }
            heapArray = temp;

            heapArray[heapArray.Length - 1] = value;
            HeapifyUp(heapArray.Length - 1);
        }
        public void HeapifyDown(int index)
        {
            int leftChildIndex = (index * 2) + 1;
            int rightChildIndex = (index * 2) + 2;
            if (leftChildIndex >= heapArray.Length && rightChildIndex >= heapArray.Length)
            {
                return;
            }
            //This is because otherwise i would get an error that my left and right child value is unassigned 
            //but it doesnt matter we know it will go into one of the 4 if statements and that will set them
            Vertex leftChildValue = heapArray[index];
            Vertex rightChildValue = heapArray[index];

            Vertex biggerChildValue = leftChildValue;
            int biggerChildIndex = leftChildIndex;

            if (rightChildIndex >= heapArray.Length)
            {
                leftChildValue = heapArray[leftChildIndex];
                biggerChildValue = leftChildValue;
                biggerChildIndex = leftChildIndex;
            }
            else// if (leftChildIndex < heapArray.Length && rightChildIndex < heapArray.Length)
            {
                leftChildValue = heapArray[leftChildIndex];
                rightChildValue = heapArray[rightChildIndex];

                if (leftChildValue.CompareTo(rightChildValue) < 0)
                {
                    //LeftChild is bigger
                    biggerChildValue = leftChildValue;
                    biggerChildIndex = leftChildIndex;
                }
                else// if (rightChildValue.CompareTo(leftChildValue) > 0)
                {
                    //RightChild is bigger
                    biggerChildValue = rightChildValue;
                    biggerChildIndex = rightChildIndex;
                }
            }
            if (biggerChildValue.CompareTo(heapArray[index]) < 0)
            {
                Vertex temp = heapArray[biggerChildIndex];
                heapArray[biggerChildIndex] = heapArray[index];
                heapArray[index] = temp;
                HeapifyDown(biggerChildIndex);
            }
        }
        public Vertex Pop()
        {
            Vertex temp = heapArray[0];
            heapArray[0] = heapArray[heapArray.Length - 1];

            Vertex[] tempArray = new Vertex[heapArray.Length - 1];
            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = heapArray[i];
            }
            heapArray = tempArray;
            HeapifyDown(0);

            return temp;
        }
    }
}
