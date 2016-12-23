using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

namespace BloodOfEvil.Helpers
{
    // A savoir :
    // - Lorsque l'on drag & drop un fichier ou un répertoire sur un éxécutable, 
    // l'éxécutable en question récupère se lance avec pour argument le chemin de ce fichier ou de ce répertoire.
    public static class InterProcessComunicationHelper
    {
        /// <summary>
	/// Lance l'éxécutable spécifié au chemin de "processPath" et lui envoie les arguments spécifié dans "processArguments".
	/// Example : LaunchProcess(@"C:\Users\Ben\Desktop\executable.exe", new string[] { "firstParam", "secondParam" });
	/// </summary>
	public static Process LaunchProcess(
		string processPath,
		string[] processArguments)
	{
		string processArgument = "";

		// On a besoin d'entouré chaque argument de quote pour qu'il soit reconnu comme argument unique si il contiend des espaces.
		for (int i = 0; i < processArguments.Length; i++)
			processArgument += "\"" + processArguments[i] + "\" ";

		Process process = new Process();

		process.StartInfo.Arguments = processArgument;
		process.StartInfo.FileName = processPath;
		// Permet de cacher la console du processu avec qui l'on communique.
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		
		process.Start();

		return process;
	}
	    
	/// <summary>
	/// Ce que ça fait : Lance une commande SSH en C#.
	/// Comment le faire fonctionner  : 
	/// - Pour que cette commande fonctionne, il faut rajouter le binaire ssh "C:/program files/git/usr/bin/ssh" dans les variables d'environnement "PATH".
	/// - Puis redémarrer le PC.
	/// À quoi ça sert ?
	/// - Appeler une commande (exemple ls -l) ou un éxécutable (processus) sur un ordinateur à distance.
	/// Comment l'utiliser ?
	/// - ssh nomDeCompteDuSystemDexploitationDeLordinateurDistant@IPDeLordinateurDistant commande.
	/// - ssh vincentberlioz@192.168.1.220 ls -l
	/// </summary>
	public static Process LaunchSSHCommand(
		string command = "echo t",
		string computerIP = "192.168.1.220",
		string computerUsername = "vincentberlioz",
		string computerPassword = "0000")
	{
		Process process = new Process();

		process.StartInfo.FileName = "ssh";
		process.StartInfo.Arguments = string.Format(
			"{0}@{1} {2}",
			computerUsername,
			computerIP,
			command);

		// Affiche le résultat de la console ssh dans cette console.
		process.StartInfo.UseShellExecute = false;

		process.Start();
		process.WaitForExit();

		return process;
	}
    }
}
