using System;

namespace Plato.ModelProvisioning
{
    public struct Vector : IEquatable<Vector>
    {
        #region Constructors

        public Vector(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Properties

        public double X { get; set; }

        public double Y { get; set; }

        #endregion

        #region Methods

        #region IEquatable<Vector> members

        public bool Equals(Vector other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        #endregion

        public static bool operator ==(Vector a, Vector b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector && Equals((Vector) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        #endregion
    }
}
