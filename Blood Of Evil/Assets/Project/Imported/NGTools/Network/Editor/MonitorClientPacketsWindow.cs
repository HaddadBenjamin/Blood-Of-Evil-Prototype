using NGTools;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class MonitorClientPacketsWindow : EditorWindow
	{
		private Client	client;
		private Vector2	sendScrollPosition;
		private Vector2	receiveScrollPosition;

		public static void	Open(Client client)
		{
			MonitorClientPacketsWindow	window = EditorWindow.CreateInstance<MonitorClientPacketsWindow>();

			window.client = client;
			window.SetTitle("Monitor Client");
			window.Show();
		}

		protected virtual void	OnEnable()
		{
			Utility.RegisterIntervalCallback(this.Repaint, 50);
		}

		protected virtual void	OnDisable()
		{
			Utility.UnregisterIntervalCallback(this.Repaint);
		}

		protected virtual void	OnGUI()
		{
			if (this.client == null)
				return;

			GUILayout.Label("Client " + this.client.tcpClient.Client.LocalEndPoint.ToString() + " " + DateTime.Now.ToString("HH:mm:ss"));
			GUILayout.Label(this.client.ToString());

			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Sent (" + client.sentPacketsHistoric.Count + ")");
					this.sendScrollPosition = GUILayout.BeginScrollView(this.sendScrollPosition);
					{
						for (int i = this.client.sentPacketsHistoric.Count - 1; i >= 0; --i)
						{
							GUILayout.Label(this.client.sentPacketsHistoric[i].time + " " + this.client.sentPacketsHistoric[i].packet.ToString());
						}
					}
					GUILayout.EndScrollView();
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					GUILayout.Label("Received (" + client.receivedPacketsHistoric.Count + ")");
					this.receiveScrollPosition = GUILayout.BeginScrollView(this.receiveScrollPosition);
					{
						for (int i = client.receivedPacketsHistoric.Count - 1; i >= 0; --i)
						{
							GUILayout.Label(client.receivedPacketsHistoric[i]);
						}
					}
					GUILayout.EndScrollView();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true || this.client == null || this.client.tcpClient.Client.Connected == false)
			{
				this.Close();
				return;
			}
		}
	}
}