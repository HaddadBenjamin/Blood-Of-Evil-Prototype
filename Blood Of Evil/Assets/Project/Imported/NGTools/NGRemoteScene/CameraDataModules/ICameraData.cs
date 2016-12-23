using UnityEngine;

namespace NGTools
{
	public interface ICameraData
	{
		NGServerScene		ServerScene { get; }
		AbstractTcpListener	TCPListener { get; }
	}

	// Especially for ScreenshotModule, we need more data.
	public interface ICameraScreenshotData : ICameraData
	{
		Client				Sender { get; }
		int					TargetRefresh { get; }
		int					Width { get; }
		int					Height { get; }
		bool				Wireframe { get; }
		Camera				TargetCamera { get; }
		RenderTexture		RenderTexture { get; }
	}
}