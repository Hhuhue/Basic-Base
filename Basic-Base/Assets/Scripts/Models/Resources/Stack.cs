using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models.Resources
{
    /// <summary>
    /// Class representing a stack of resources
    /// </summary>
    public class Stack
    {
        private int _quantity;

        private readonly Resource _resourceInfo;

        /// <summary>
        /// Creates a new resource stack instance.
        /// </summary>
        /// <param name="resource">The item resource. </param>
        /// <param name="quantity">The resource quantity. </param>
        public Stack(Resource resource, int quantity)
        {
            _quantity = quantity;

            _resourceInfo = resource ?? Resource.Default();
        }

        /// <summary>
        /// Gives the name of the item's resource
        /// </summary>
        /// <returns>The name of the resource. </returns>
        public string GetResourceName()
        {
            return _resourceInfo.Name;
        }

        /// <summary>
        /// Gives the stack capacity of the item's resource
        /// </summary>
        /// <returns>The name of the resource. </returns>
        public int GetStackCap()
        {
            return _resourceInfo.StackCap;
        }

        /// <summary>
        /// Gives the quantity of the resource
        /// </summary>
        /// <returns></returns>
        public int GetQuantity()
        {
            return _quantity;
        }

        /// <summary>
        /// Sets the quantity of the resource.
        /// </summary>
        /// <param name="quantity">The new quantity.</param>
        /// <exception cref="ArgumentException">Thrown when the quantity is invalid. </exception>>
        public void SetQuantity(int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity must be positive");
            }

            if (quantity > _resourceInfo.StackCap)
            {
                throw new ArgumentException("Resource is overstacked");
            }

            _quantity = quantity;
        }

        /// <summary>
        /// Combines the resources of two stack.
        /// </summary>
        /// <param name="a">The first stack. </param>
        /// <param name="b">The second stack. </param>
        /// <returns>The combined stack. </returns>
        /// <exception cref="ArgumentException">Thrown when the stacks can't be combined. </exception>
        public static Stack operator+ (Stack a, Stack b)
        {
            if (a.GetResourceName() != b.GetResourceName())
            {
                throw new ArgumentException("Stacks must be of the same resource");
            }

            if (a.GetQuantity() + b.GetQuantity() > a.GetStackCap())
            {
                throw new ArgumentException("First stack is overstacked");
            }

            a.SetQuantity(a.GetQuantity() + b.GetQuantity());

            return a;
        }
    }
}
