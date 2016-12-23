//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Game miniMap can have icons on it -- this class takes care of animating them when needed.
/// </summary>

namespace NJG
{
	public class MapArrow : MonoBehaviour
	{
		[SerializeField]
		public MapItem item;

#if NJG_NGUI

        [HideInInspector]
        public UISprite sprite;

#else

        [HideInInspector]
        public Image sprite;

#endif

        public Map map;

		public Transform child;

		//public bool isValid;

		protected float rotationOffset = 0.0f;

		Vector3 mRot = Vector3.zero;
		Vector3 mArrowRot = Vector3.zero;
		Vector3 mFrom = Vector3.zero;

		/// <summary>
		/// Triggered when the icon is visible on the miniMap.
		/// </summary>

		public void UpdateRotation(Vector3 fromTarget)
		{
			mFrom = fromTarget - item.transform.position;

			float angle = 0;

			if (NJGMap.instance.orientation == NJGMap.Orientation.XZDefault)
			{
				mFrom.y = 0;
				angle = Vector3.Angle(Vector3.forward, mFrom);
			}
			else
			{
				mFrom.z = 0;
				angle = Vector3.Angle(Vector3.up, mFrom);
			}

			if (Vector3.Dot(Vector3.right, mFrom) < 0)
				angle = 360 - angle;

			angle += 180;

			mRot = Vector3.zero;

			if (NJGMap.instance.orientation == NJGMap.Orientation.XZDefault)
			{
				mRot.z = angle;
				mRot.y = 180;
			}
			else
			{
				mRot.z = -angle;
				mRot.y = mRot.x = 0;
			}

            if (!transform.localEulerAngles.Equals(mRot)) transform.localEulerAngles = mRot;

			if (!item.arrowRotate)
			{
				mArrowRot.x = 0;
				mArrowRot.y = 180;
                mArrowRot.z = map.rotateWithPlayer ? (map.iconRoot.localEulerAngles.z - transform.localEulerAngles.z) : -transform.localEulerAngles.z;
				if (child.localEulerAngles != mArrowRot) child.localEulerAngles = mArrowRot;
			}
		}
	}
}