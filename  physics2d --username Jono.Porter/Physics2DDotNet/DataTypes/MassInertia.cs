#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion




#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Runtime.Serialization;
using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    public static class PhysicsConstants
    {
        public const Scalar GravitationalConstant = 6.67e-11f;
    }


    /// <summary>
    /// This class Stores mass information and Moment of Inertia Together since they are very closly related.
    /// </summary>
    [Serializable]
    public sealed class MassInfo : IDeserializationCallback
    {
        #region static methods
        public static MassInfo FromCylindricalShell(Scalar mass, Scalar radius)
        {
            return new MassInfo(mass, mass * radius * radius);
        }
        public static MassInfo FromHollowCylinder(Scalar mass, Scalar innerRadius, Scalar outerRadius)
        {
            return new MassInfo(mass, (Scalar).5 * mass * (innerRadius * innerRadius + outerRadius * outerRadius));
        }
        public static MassInfo FromSolidCylinder(Scalar mass, Scalar radius)
        {
            return new MassInfo(mass, (Scalar).5 * mass * (radius * radius));
        }
        public static MassInfo FromRectangle(Scalar mass, Scalar length, Scalar width)
        {
            return new MassInfo(mass, (1f / 12f) * mass * (length * length + width * width));
        }
        public static MassInfo FromSquare(Scalar mass, Scalar sideLength)
        {
            return new MassInfo(mass, (1f / 6f) * mass * (sideLength * sideLength));
        }
        public static MassInfo FromPolygon(Vector2D[] vertexes, Scalar mass)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length == 0) { throw new ArgumentOutOfRangeException("vertexes"); }

            if (vertexes.Length == 1) { return new MassInfo(mass, 0); }

            Scalar denom = 0.0f;
            Scalar numer = 0.0f;

            for (int j = vertexes.Length - 1, i = 0; i < vertexes.Length; j = i, i++)
            {
                Scalar a, b, c;
                Vector2D P0 = vertexes[j];
                Vector2D P1 = vertexes[i];
                Vector2D.Dot(ref P1, ref P1, out a);
                Vector2D.Dot(ref P1, ref P0, out b);
                Vector2D.Dot(ref P0, ref P0, out c);
                a += b + c;
                Vector2D.ZCross(ref P0, ref P1, out b);
                b = MathHelper.Abs(b);
                denom += (b * a);
                numer += b;
            }
            return new MassInfo(mass, (mass * denom) / (numer * 6));
        }

        #endregion
        #region fields
        private Scalar mass;
        private Scalar momentofInertia;
        [NonSerialized]
        private Scalar massInv;
        [NonSerialized]
        private Scalar momentofInertiaInv;
        [NonSerialized]
        private Scalar accelerationDueToGravity;
        #endregion
        #region constructors
        public MassInfo() { }
        public MassInfo(Scalar mass, Scalar momentOfInertia)
        {
            this.MomentofInertia = momentOfInertia;
            this.Mass = mass;
        }
        #endregion
        #region properties
        public Scalar Mass
        {
            get
            {
                return mass;
            }
            set
            {
                this.mass = value;
                this.massInv = 1 / value;
                this.accelerationDueToGravity = value * PhysicsConstants.GravitationalConstant;
            }
        }

        public Scalar MomentofInertia
        {
            get
            {
                return momentofInertia;
            }
            set
            {
                this.momentofInertia = value;
                this.momentofInertiaInv = 1 / value;
            }
        }
        [System.ComponentModel.Browsable(false)]
        public Scalar MassInv
        {
            get
            {
                return massInv;
            }
        }
        [System.ComponentModel.Browsable(false)]
        public Scalar MomentofInertiaInv
        {
            get
            {
                return momentofInertiaInv;
            }
        }
        [System.ComponentModel.Browsable(false)]
        public Scalar AccelerationDueToGravity
        {
            get
            {
                return accelerationDueToGravity;
            }
        }
        #endregion
        #region IDeserializationCallback Members
        public void OnDeserialization(object sender)
        {
            this.massInv = 1 / this.mass;
            this.momentofInertiaInv = 1 / this.momentofInertia;
            this.accelerationDueToGravity = this.mass * PhysicsConstants.GravitationalConstant;
        }
        #endregion
    }
}
