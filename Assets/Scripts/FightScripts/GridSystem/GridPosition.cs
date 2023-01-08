using System;
using Unity.VisualScripting;
using UnityEngine;

namespace GridSystems
{
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int x;
        public int z;
        
        public bool Equals(GridPosition other)
        {
            return x == other.x && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, z);
        }

        

        public GridPosition(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public override string ToString()
        {
            return "x: " + x + "; " + "z: " + z;
        }

        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.x == b.x && a.z == b.z;
        }
        
        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return !(a ==b);
        }
        
        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.x + b.x, a.z + b.z);
        }
        
        public static GridPosition operator -(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.x - b.x, a.z - b.z);
        }

        public static float Distance(GridPosition gridPosition1, GridPosition gridPosition2)
        {
            return Vector2.Distance(new Vector2(gridPosition1.x, gridPosition1.z),
                new Vector2(gridPosition2.x, gridPosition2.z));
        }
    }
}