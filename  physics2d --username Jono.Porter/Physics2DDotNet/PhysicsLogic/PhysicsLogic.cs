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
using System.Collections.Generic;

using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    /// <summary>
    /// A physics logic is a way for the engine to effect object within the Update call.
    /// Gravity is a Example of a PhysicsLogic.
    /// </summary>
    [Serializable]
    public abstract class PhysicsLogic : IPhysicsEntity
    {
        public event EventHandler LifetimeChanged;
        public event EventHandler Added;
        public event EventHandler Removed;

        Lifespan lifetime;
        PhysicsEngine engine;
        object tag;

        protected PhysicsLogic(Lifespan lifetime)
        {
            this.Lifetime = lifetime;
        }

        public PhysicsEngine Engine
        {
            get { return engine; }
        }
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public Lifespan Lifetime
        {
            get
            {
                return lifetime;
            }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                if (this.lifetime != value)
                {
                    lifetime = value;
                    if (LifetimeChanged != null) { LifetimeChanged(this, EventArgs.Empty); }
                }
            }
        }
        protected List<Body> Bodies
        {
            get
            {
                return engine.bodies;
            }
        }

        protected internal abstract void RunLogic(Scalar dt);
        internal void OnAddedInternal(PhysicsEngine engine)
        {
            if (this.engine != null) { throw new InvalidOperationException("The IPhysicsEntity cannot be added to more then one engine or added twice."); }
            this.engine = engine;
            OnAdded();
            if (Added != null) { Added(this, EventArgs.Empty); }
        }
        internal void OnRemovedInternal()
        {
            engine = null;
            OnRemoved();
            if (Removed != null) { Removed(this, EventArgs.Empty); }
        }
        protected virtual void OnAdded() { }
        protected virtual void OnRemoved() { }
        protected internal virtual void UpdateTime(Scalar dt)
        {
            this.lifetime.Update(dt);
        }
    }

}