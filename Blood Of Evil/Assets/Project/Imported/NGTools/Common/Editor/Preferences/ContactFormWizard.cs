using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class ContactFormWizard : ScriptableWizard
	{
		public enum Subject
		{
			Contact,
			BugReport,
			Translation,
			Suggestion,
			Congratulation,
		}

		public Subject	subject = Subject.Contact;
		[DefaultValueEditorPref("Anonymous")]
		public string	contactName;
		public string	contactEMail = string.Empty;
		[DefaultValueEditorPref(true)]
		public bool		unityInformation;
		[DefaultValueEditorPref(true)]
		public bool		osInformation;
		[DefaultValueEditorPref(true)]
		public bool		hardwareInformation;

		protected virtual void	OnEnable()
		{
			Utility.LoadEditorPref(this);
		}

		protected virtual void	OnDestroy()
		{
			Utility.SaveEditorPref(this);
		}

		private void	OnGUI()
		{
			GUILayout.Label(LC.G("ContactFormWizard_Title"), GeneralStyles.MainTitle);

			this.contactName = EditorGUILayout.TextField(LC.G("ContactFormWizard_ContactName"), this.contactName);
			this.contactEMail = EditorGUILayout.TextField(LC.G("ContactFormWizard_ContactEMail"), this.contactEMail);
			this.unityInformation = EditorGUILayout.Toggle(LC.G("ContactFormWizard_UnityInformation"), this.unityInformation);
			this.osInformation = EditorGUILayout.Toggle(LC.G("ContactFormWizard_OSInformation"), this.osInformation);
			this.hardwareInformation = EditorGUILayout.Toggle(LC.G("ContactFormWizard_HardwareInformation"), this.hardwareInformation);

			GUILayout.FlexibleSpace();

			if (string.IsNullOrEmpty(this.contactName) == true)
				GUILayout.Label(LC.G("ContactFormWizard_NameRequired"), "CN EntryError");

			if (Regex.IsMatch(this.contactEMail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
				GUILayout.Label(LC.G("ContactFormWizard_ValidEMailRequired"), "CN EntryError");

			if (this.subject == Subject.BugReport)
			{
				if (this.unityInformation == false ||
					this.osInformation == false ||
					this.hardwareInformation == false)
				{
					GUILayout.Label(LC.G("ContactFormWizard_BugReportRecommendation"), "CN EntryWarn");
				}
			}

			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button(LC.G("ContactFormWizard_PrepareTheEMail"), GUILayout.ExpandWidth(false)) == true)
				{
					this.OnWizardCreate();
				}
			}
			GUILayout.EndHorizontal();
		}

		private void	OnWizardCreate()
		{
			EditorUtility.DisplayDialog("", LC.G("ContactFormWizard_SupportLanguagesWarning"), LC.G("Ok"));

			StringBuilder	body = Utility.GetBuffer();
			StringBuilder	appendixData = Utility.GetBuffer();

			body.AppendLine("Nickname: " + this.contactName);
			body.AppendLine("E-Mail: " + this.contactEMail);
			body.AppendLine();
			body.AppendLine();
			body.AppendLine(Constants.PackageTitle + " Version: " + Constants.Version);

			if (this.unityInformation == true)
			{
				appendixData.AppendLine();
				appendixData.AppendLine();
				appendixData.AppendLine("ActiveBuildTarget: " + EditorUserBuildSettings.activeBuildTarget);
				appendixData.AppendLine("Platform: " + Application.platform);
				appendixData.AppendLine("RunInBackground: " + Application.runInBackground);
				appendixData.AppendLine("SystemLanguage: " + Application.systemLanguage);
				appendixData.AppendLine("UnityVersion: " + Application.unityVersion);
#if UNITY_5
				appendixData.Append("Version: " + Application.version);
#endif
			}

			if (this.osInformation == true)
			{
				appendixData.AppendLine();
				appendixData.AppendLine();
				appendixData.AppendLine("OperatingSystem: " + SystemInfo.operatingSystem);
				appendixData.AppendLine("ProcessorCount: " + SystemInfo.processorCount);
				appendixData.Append("ProcessorType: " + SystemInfo.processorType);
			}

			if (this.hardwareInformation == true)
			{
				appendixData.AppendLine();
				appendixData.AppendLine();
				appendixData.AppendLine("DeviceModel: " + SystemInfo.deviceModel);
				appendixData.AppendLine("DeviceName: " + SystemInfo.deviceName);
				appendixData.AppendLine("DeviceType: " + SystemInfo.deviceType);
				appendixData.AppendLine("GraphicsDeviceName: " + SystemInfo.graphicsDeviceName);
				appendixData.AppendLine("GraphicsDeviceVendor: " + SystemInfo.graphicsDeviceVendor);
				appendixData.AppendLine("GraphicsDeviceVendorID: " + SystemInfo.graphicsDeviceVendorID);
				appendixData.AppendLine("GraphicsDeviceVersion: " + SystemInfo.graphicsDeviceVersion);
				appendixData.AppendLine("GraphicsMemorySize: " + SystemInfo.graphicsMemorySize);
#if UNITY_5
				appendixData.AppendLine("GraphicsMultiThreaded: " + SystemInfo.graphicsMultiThreaded);
#endif
				appendixData.AppendLine("GraphicsShaderLevel: " + SystemInfo.graphicsShaderLevel);
				appendixData.Append("SystemMemorySize: " + SystemInfo.systemMemorySize);
			}

			Application.OpenURL("mailto:" + Constants.SupportEmail + "?subject=[NGConsole]%20" + Utility.NicifyVariableName(this.subject.ToString()) + "%20from%20" + this.contactName + "&body=" + Uri.EscapeUriString(Utility.ReturnBuffer(body)) + Uri.EscapeUriString(Utility.ReturnBuffer(appendixData)));
		}
	}
}