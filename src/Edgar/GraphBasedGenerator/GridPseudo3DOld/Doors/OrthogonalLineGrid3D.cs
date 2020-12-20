using System;
using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.GridPseudo3DOld.Doors
{
    public struct OrthogonalLineGrid3D : IEquatable<OrthogonalLineGrid3D>
    {
        public readonly OrthogonalLineGrid2D Line;

        public readonly int Z;

        public OrthogonalLineGrid3D(OrthogonalLineGrid2D line, int z)
        {
            Line = line;
            Z = z;
        }

        public bool Equals(OrthogonalLineGrid3D other)
        {
            return Line.Equals(other.Line) && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is OrthogonalLineGrid3D other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Line.GetHashCode() * 397) ^ Z;
            }
        }
    }
}