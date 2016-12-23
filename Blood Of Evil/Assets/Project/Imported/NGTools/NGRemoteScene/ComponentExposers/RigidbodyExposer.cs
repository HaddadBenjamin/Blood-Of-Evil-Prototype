using System.Reflection;
using UnityEngine;

namespace NGTools
{
	public class RigidbodyExposer : ComponentExposer
	{
		public	RigidbodyExposer() : base(typeof(Rigidbody))
		{
		}

		public override PropertyInfo[]	GetPropertyInfos()
		{
			PropertyInfo[]	fields = new PropertyInfo[] {
				this.type.GetProperty("angularDrag"),
				this.type.GetProperty("angularVelocity"),
				this.type.GetProperty("centerOfMass"),
				this.type.GetProperty("collisionDetectionMode"),
				this.type.GetProperty("detectCollisions"),
				this.type.GetProperty("drag"),
				this.type.GetProperty("freezeRotation"),
				this.type.GetProperty("inertiaTensor"),
				this.type.GetProperty("inertiaTensorRotation"),
				this.type.GetProperty("interpolation"),
				this.type.GetProperty("isKinematic"),
				this.type.GetProperty("mass"),
				this.type.GetProperty("maxAngularVelocity"),
				this.type.GetProperty("maxDepenetrationVelocity"),
				this.type.GetProperty("position"),
				this.type.GetProperty("rotation"),
				this.type.GetProperty("sleepThreshold"),
				this.type.GetProperty("solverIterationCount"),
				this.type.GetProperty("useGravity"),
				this.type.GetProperty("velocity"),
			};

			return fields;
		}
	}
}