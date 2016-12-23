using UnityEngine;

namespace BloodOfEvil.Extensions
{
    using UnityEngine;

    /// <summary>
    /// Cette classe vient de Manzalab.
    /// </summary>
    public static class CameraExtension
	{
		public static void ZoomInOrthographic(this Camera cam, float zoomSpeed, bool stickToMouse = true)
		{
			float orthoDelta = cam.orthographicSize * zoomSpeed;

			cam.orthographicSize += orthoDelta;
			if (stickToMouse)
			{
				Vector3 viewportMouse = cam.ScreenToViewportPoint(Input.mousePosition);

				cam.transform.position -= new Vector3(orthoDelta * cam.aspect * Mathf.Lerp(-1, 1, viewportMouse.x), orthoDelta * Mathf.Lerp(-1, 1, viewportMouse.y), 0);
			}
		}
		public static void ZoomOutOrthographic(this Camera cam, float zoomSpeed, bool stickToMouse = true)
		{
			float orthoDelta = cam.orthographicSize * zoomSpeed;

			cam.orthographicSize -= orthoDelta;
			if (stickToMouse)
			{
				Vector3 viewportMouse = cam.ScreenToViewportPoint(Input.mousePosition);

				cam.transform.position += new Vector3(orthoDelta * cam.aspect * Mathf.Lerp(-1, 1, viewportMouse.x), orthoDelta * Mathf.Lerp(-1, 1, viewportMouse.y), 0);
			}
		}

		/// <summary>
		/// Give the point image to another camera
		/// </summary>
		/// <param name="from">The camera that sees the point</param>
		/// <param name="to">The camera that want to sees the point like the first one</param>
		/// <param name="point">The point to convert</param>
		/// <returns>The point converted.</returns>
		/// <example>
		/// Vector3 guiPoint = ChangeCamera(Camera.main, guiCamera, unit.transform.position);
		/// </example>
		/// <seealso cref="WorldToNormalizedViewportPoint"/>
		/// <seealso cref="NormalizedViewportToWorldPoint"/>
		public static Vector3 ChangeCamera(this Camera from, Camera to, Vector3 point)
		{
			return NormalizedViewportToWorldPoint(
				to, WorldToNormalizedViewportPoint(from, point)
			);
		}

		/// <summary>
		/// Converts a world point to our normalized viewport point (related to a camera)
		/// </summary>
		/// <param name="camera">The camera that "sees" the point</param>
		/// <param name="point">The point to convert</param>
		/// <returns>The converted point</returns>
		/// <example>
		/// Vector3 normPoint = WorldToNormalizedViewportPoint(projCamera, unit);
		/// Vector3 unitGUIpos = NormalizedViewportToWorldPoint(orthoCamera, normPoint);
		/// </example>
		/// <seealso cref="NormalizedViewportToWorldPoint"/>
		/// <seealso cref="ChangeLayerRecursively"/>
		public static Vector3 WorldToNormalizedViewportPoint(this Camera camera, Vector3 point)
		{
			point = camera.WorldToViewportPoint(point);

			if (camera.orthographic)
			{
				point.z = (2 * (point.z - camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) - 1f;
			}
			else
			{
				point.z = ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane))
				+ (1 / point.z) * (-2 * camera.farClipPlane * camera.nearClipPlane / (camera.farClipPlane - camera.nearClipPlane));
			}

			return point;
		}

		/// <summary>
		/// Converts the point back from our viewport to the world
		/// </summary>
		/// <param name="camera">The camera that will see the point</param>
		/// <param name="point">The point to convert</param>
		/// <returns>The converted point</returns>
		/// <example>
		/// Vector3 normPoint = WorldToNormalizedViewportPoint(projCamera, unit);
		/// Vector3 unitGUIpos = NormalizedViewportToWorldPoint(orthoCamera, normPoint);
		/// </example>
		/// <seealso cref="WorldToNormalizedViewportPoint"/>
		/// <seealso cref="ChangeLayerRecursively"/>
		public static Vector3 NormalizedViewportToWorldPoint(this Camera camera, Vector3 point)
		{
			if (camera.orthographic)
			{
				point.z = (point.z + 1f) * (camera.farClipPlane - camera.nearClipPlane) * 0.5f + camera.nearClipPlane;
			}
			else
			{
				point.z = ((-2 * camera.farClipPlane * camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) /
				(point.z - ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)));
			}

			return camera.ViewportToWorldPoint(point);
		}
	}
}
