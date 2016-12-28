using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models.Structures
{
    public class Border
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Border(int top, int bottom, int left, int right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public bool IsPositionWithinBorder(int x, int y)
        {
            return x >= Left && x <= Right && y >= Bottom && y <= Top;
        }

        public bool IsPositionOnBorder(int x, int y)
        {
            return IsPositionWithinBorder(x, y) && (x == Left || x == Right || y == Bottom || y == Top);
        }

        public static bool operator ==(Border border1, Border border2)
        {
            if (border1 == null || border2 == null) return false;

            return border1.Top == border2.Top &&
                   border1.Bottom == border2.Bottom &&
                   border1.Left == border2.Left &&
                   border1.Right == border2.Right;
        }

        public static bool operator !=(Border border1, Border border2)
        {
            return !(border1 == border2);
        }

        protected bool Equals(Border other)
        {
            return Top == other.Top && Bottom == other.Bottom && Left == other.Left && Right == other.Right;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Border)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Top;
                hashCode = (hashCode * 397) ^ Bottom;
                hashCode = (hashCode * 397) ^ Left;
                hashCode = (hashCode * 397) ^ Right;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return "Top: " + Top + " Bottom: " + Bottom + " Left: " + Left + " Right: " + Right;
        }
    }
}
