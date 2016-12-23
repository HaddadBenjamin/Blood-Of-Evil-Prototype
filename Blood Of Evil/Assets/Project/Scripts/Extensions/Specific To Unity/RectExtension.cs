using UnityEngine;

namespace BloodOfEvil.Extensions
{
    /// <summary>
    /// Cette classe vient de Manzalab.
    /// </summary>
    public static class RectExtension
	{
		[System.Flags]
		public enum OutCode
		{
			INSIDE = 0,
			LEFT = 1,
			RIGHT = 2,
			BOTTOM = 4,
			TOP = 8
		}

		public static OutCode ComputeOutCode(float x, float y, Rect rect)
		{
			OutCode code;

			code = OutCode.INSIDE;       // initialised as being inside of clip window

			if (x < rect.xMin)           // to the left of clip window
				code |= OutCode.LEFT;
			else if (x > rect.xMax)      // to the right of clip window
				code |= OutCode.RIGHT;
			if (y < rect.yMin)           // below the clip window
				code |= OutCode.BOTTOM;
			else if (y > rect.yMax)      // above the clip window
				code |= OutCode.TOP;
			return code;
		}

		public static Vector2 LineIntersect(this Rect rect, Vector2 outSidePoint, Vector2 insidePoint)
		{
			OutCode outcode0 = ComputeOutCode(outSidePoint.x, outSidePoint.y, rect);
			OutCode outcode1 = ComputeOutCode(insidePoint.x, insidePoint.y, rect);

			while (true)
			{
				if ((outcode0 | outcode1) == OutCode.INSIDE)
				{ // Bitwise OR is 0. Trivially accept and get out of loop
					break;
				}
				else if ((outcode0 & outcode1) != 0)
				{ // Bitwise AND is not 0. Trivially reject and get out of loop
					break;
				}
				else
				{
					// failed both tests, so calculate the line segment to clip
					// from an outside point to an intersection with clip edge
					float x = 0, y = 0;

					// At least one endpoint is outside the clip rectangle; pick it.
					OutCode outcodeOut = outcode0 != 0 ? outcode0 : outcode1;

					// Now find the intersection point;
					// use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
					if ((outcodeOut & OutCode.TOP) == OutCode.TOP)
					{           // point is above the clip rectangle
						x = outSidePoint.x + (insidePoint.x - outSidePoint.x) * (rect.yMax - outSidePoint.y) / (insidePoint.y - outSidePoint.y);
						y = rect.yMax;
					}
					else if ((outcodeOut & OutCode.BOTTOM) == OutCode.BOTTOM)
					{ // point is below the clip rectangle
						x = outSidePoint.x + (insidePoint.x - outSidePoint.x) * (rect.yMin - outSidePoint.y) / (insidePoint.y - outSidePoint.y);
						y = rect.yMin;
					}
					else if ((outcodeOut & OutCode.RIGHT) == OutCode.RIGHT)
					{  // point is to the right of clip rectangle
						y = outSidePoint.y + (insidePoint.y - outSidePoint.y) * (rect.xMax - outSidePoint.x) / (insidePoint.x - outSidePoint.x);
						x = rect.xMax;
					}
					else if ((outcodeOut & OutCode.LEFT) == OutCode.LEFT)
					{   // point is to the left of clip rectangle
						y = outSidePoint.y + (insidePoint.y - outSidePoint.y) * (rect.xMin - outSidePoint.x) / (insidePoint.x - outSidePoint.x);
						x = rect.xMin;
					}

					// Now we move outside point to intersection point to clip
					// and get ready for next pass.
					if (outcodeOut == outcode0)
					{
						outSidePoint.x = x;
						outSidePoint.y = y;
						outcode0 = ComputeOutCode(outSidePoint.x, outSidePoint.y, rect);
					}
					else
					{
						insidePoint.x = x;
						insidePoint.y = y;
						outcode1 = ComputeOutCode(insidePoint.x, insidePoint.y, rect);
					}
				}
			}
			return outSidePoint;
		}
	}
}
