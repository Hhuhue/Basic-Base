using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Models.Resources
{
    /// <summary>
    /// Container of items
    /// </summary>
    public class Inventory
    {
        public int Capacity { get; private set; }

        private readonly List<Stack> _stacks;

        /// <summary>
        /// Creates a new instance of inventory.
        /// </summary>
        /// <param name="capacity">The item capacity of the inentory. </param>
        public Inventory(int capacity)
        {
            Capacity = capacity;
            _stacks = new List<Stack>();
        }

        /// <summary>
        /// Adds resources to the inventory
        /// </summary>
        /// <param name="newStack">The items to add. </param>
        /// <param name="leftoverStack">The resources that couldn't fit. </param>
        public void Add(Stack newStack, out Stack leftoverStack)
        {
            //Check if the item is valid
            if (newStack == null)
            {
                throw new ArgumentNullException();
            }

            //Search for a stack of the same resource that isn't full
            Stack existingStack = _stacks
                .Where(stack => stack.GetResourceName() == newStack.GetResourceName())
                .FirstOrDefault(stack => stack.GetQuantity() != stack.GetStackCap());

            if (existingStack == null)
            {
                //Check if the inventory is full
                if (_stacks.Count() + 1 > Capacity)
                {
                    leftoverStack = newStack;
                }
                else
                {
                    _stacks.Add(newStack);
                    leftoverStack = null;
                }
            }
            //Check if the new stack can fit in the existing stack 
            else if (existingStack.GetQuantity() + newStack.GetQuantity() > existingStack.GetStackCap())
            {
                //Fill the existing stack with the new stack
                newStack.SetQuantity(newStack.GetQuantity() - (existingStack.GetStackCap() - existingStack.GetQuantity()));

                existingStack.SetQuantity(existingStack.GetStackCap());

                //Check if the inventory is full
                if (_stacks.Count() + 1 > Capacity)
                {
                    leftoverStack = newStack;
                }
                else
                {
                    _stacks.Add(newStack);
                    leftoverStack = null;
                }
            }
            else
            {
                //Combine the new stack with the existing stack
                existingStack += newStack;
                leftoverStack = null;
            }
        }

        /// <summary>
        /// Draws resources from the inventory.
        /// </summary>
        /// <param name="resouceName">The type of resource to draw. </param>
        /// <param name="quantity">The quantity to draw. </param>
        /// <returns>A stack of resource. </returns>
        /// <exception cref="ArgumentException">Thrown when the quantity is invalid. </exception>
        public Stack Draw(string resouceName, int quantity)
        {
            if (quantity < 0)
            {
                throw  new ArgumentException("The quantity drawn from an inventory must be positive.");
            }

            Stack stack = _stacks.FirstOrDefault(s => s.GetResourceName() == resouceName);

            return stack;
        }

        /// <summary>
        /// Checksif the inventory contains a certain resource.
        /// </summary>
        /// <param name="resourceName">The name of the resource to search for. </param>
        /// <returns>Wheither or not the inventory contains the resource. </returns>
        public bool Contains(string resourceName)
        {
            return true;
        }
    }
}
