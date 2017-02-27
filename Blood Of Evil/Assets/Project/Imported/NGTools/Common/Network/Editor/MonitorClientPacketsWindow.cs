using NGTools.Network;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.Network
{
	public class MonitorClientPacketsWindow : EditorWindow
	{
		public int	offsetSent = 0;
		public int	offsetReceived = 0;
		public int	maxPacketsDisplay = 100;

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
			Utility.LoadEditorPref(this);
			Utility.RegisterIntervalCallback(this.Repaint, 50);
		}

		protected virtual void	OnDisable()
		{
			Utility.SaveEditorPref(this);
			Utility.UnregisterIntervalCallback(this.Repaint);
		}

		protected virtual void	OnGUI()
		{
			if (this.client == null)
				return;

			EditorGUILayout.BeginHorizontal();
			{
				this.offsetSent = EditorGUILayout.IntField("Offset Sent", this.offsetSent);
				this.offsetReceived = EditorGUILayout.IntField("Offset Received", this.offsetReceived);
				this.maxPacketsDisplay = EditorGUILayout.IntField("Max Packets Display", this.maxPacketsDisplay);
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Label("Client " + this.client.tcpClient.Client.LocalEndPoint.ToString() + " " + DateTime.Now.ToString("HH:mm:ss"));
			GUILayout.Label(this.client.ToString());

			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Sent (" + client.sentPacketsHistoric.Count + ")");
					this.sendScrollPosition = GUILayout.BeginScrollView(this.sendScrollPosition);
					{
						for (int i = client.sentPacketsHistoric.Count - 1 - this.offsetSent, j = 0; i >= 0 && j < this.maxPacketsDisplay; --i, ++j)
							GUILayout.Label(this.client.sentPacketsHistoric[i].time + " " + this.client.sentPacketsHistoric[i].packet.ToString());
					}
					GUILayout.EndScrollView();
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					GUILayout.Label("Received (" + client.receivedPacketsHistoric.Count + ")");
					this.receiveScrollPosition = GUILayout.BeginScrollView(this.receiveScrollPosition);
					{
						for (int i = client.receivedPacketsHistoric.Count - 1 - this.offsetReceived, j = 0; i >= 0 && j < this.maxPacketsDisplay; --i, ++j)
							GUILayout.Label(client.receivedPacketsHistoric[i]);
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