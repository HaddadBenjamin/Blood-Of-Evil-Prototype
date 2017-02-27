using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	public interface ICameraData
	{
		NGServerScene		ServerScene { get; }
		AbstractTcpListener	TCPListener { get; }
	}

	// Especially for ScreenshotModule, we need more data.
	internal interface ICameraScreenshotData : ICameraData
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