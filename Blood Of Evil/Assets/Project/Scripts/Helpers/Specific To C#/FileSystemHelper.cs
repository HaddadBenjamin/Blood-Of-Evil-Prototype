using System;
using System.IO;
using System.Linq;

namespace BloodOfEvil.Helpers
{
    using Extensions;

    // Erreur :
    // Le chemmin n'est pas valide, trop long, contiend des caractères non voulues :
    // - solution : path.Replace("/", "\\");
    // Lorsque l'on travail avec des chemins de fichiers pointant sur le disque dur ":D" :
    //•	Il faut utiliser ":D\" plutôt que ":D/". 

    public static class FileSystemHelper
    {
        /// <summary>
        /// Renvoie le contenu d'un fichier de manière sécurisée.
        /// </summary>
        public static string SafeGetFileContent(string path)
        {
            return File.Exists(@path) ? 
                    File.ReadAllText(@path) :
                    "";
        }

        /// <summary>
        /// Affiche le contenu du fichier au chemin "path".
        /// </summary>
        public static void ShowFileContent(string path)
        {
            Console.WriteLine(SafeGetFileContent(@path));
        }

        /// <summary>
        /// Détermine si le "path" est un répertoire.
        /// </summary>
        public static bool IsDirectory(string path)
        {
            return (File.GetAttributes(@path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>
        /// Renvoit si le fichier ou le répertoire éxiste.
        /// </summary>
        public static bool DoesPathExists(string path)
        {
            return File.Exists(@path) ||
                   Directory.Exists(@path);
        }

        /// <summary>
        /// Renvoit si un chemin "path" est d'une des extensions "extensions".
        /// </summary>
        public static bool IsOfExtension(
            string path,
            string[] extensions)
        {
            return Array.Exists(extensions, extension => extension.Equals(Path.GetExtension(@path)));
        }

        /// <summary>
		/// Créé un fichier vide au chemin "path".
		/// </summary>
		public static void CreateEmptyFile(string path)
		{
			FileStream fileStream = File.Create(@path);

			fileStream.Close();
		}

		/// <summary>
		/// Permet de créer un fichier de manière sécurisée en créant le répertoire parent du fichier si il n'éxiste pas encore.
		/// Permet donc de s'assurer qu'un chemin éxiste.
		/// </summary>
		public static void SafeCreatePathIfDontExists(string path)
		{
			if (!string.IsNullOrEmpty(@path))
			{
				// Si on interprète le chemin comme un fichier.
				if (DoesPathIsAFileOrADirectory(@path))
				{
					string pathDirectory = Path.GetDirectoryName(@path);

					// Crée le répertoire parent de "path" si il n'éxiste pas.
					if (!string.IsNullOrEmpty(pathDirectory) &&
						!Directory.Exists(pathDirectory))
						Directory.CreateDirectory(pathDirectory);

					// Crée le fichier si il n'éxiste pas.
					if (!File.Exists(@path))
						CreateEmptyFile(path);
				}
				// Si on interprète le chemin comme un répertoire.
				else
				{
					// Crée le répertoire si il n'éxiste pas.
					if (!Directory.Exists(@path))
						Directory.CreateDirectory(@path);
				}
			}
		}

        /// <summary>
        /// Combine 2 chemins.
        /// </summary>
        public static string CombinePath(string path, string subPath)
        {
            ///Devrait utilise "params string subPath".
            return Path.Combine(@path, @subPath);
        }

        /// <summary>
        /// Supprime un répertoire de façon sécurisée et récursive.
        /// </summary>
        public static void SafeDeleteDirectoryRecursively(string directoryPath)
        {
            if (Directory.Exists(@directoryPath))
                Directory.Delete(@directoryPath, true);
        }

        /// <summary>
        /// Récupère tous les fichiers d'un répertoire ayant une des extensions notifié par "extensions" et 
        /// ne contenant pas un des filtres de "exclusiveIgnoreFilters".
        /// </summary>
        public static string[] GetAllDirectoryFilesRecursively(
            string directoryPath,
            string[] extensions,
            string[] exclusiveIgnoreFilters)
        {
            return extensions.SelectMany(extension => Directory.GetFiles(@directoryPath, "*" + extension, SearchOption.AllDirectories))
                             .Where(fileName => !fileName.ContainsAnArrayElement(exclusiveIgnoreFilters))
                             .ToArray();
        }

        /// <summary>
        /// Affiche tous les fichiers d'un répertoire "directoryPath" de manière récursive et d'une des extensions de "extensions"  et
        /// ne contenant pas un des filtres de "exclusiveIgnoreFilters".
        /// </summary>
        public static void ListAllFilesRecursivelyInADirectory(
            string directoryPath,
            string[] extensions,
            string[] exclusiveIgnoreFilters)
        {
            Array.ForEach(GetAllDirectoryFilesRecursively(directoryPath, extensions, exclusiveIgnoreFilters), Console.WriteLine);
        }

        /// <summary>
        /// Renvoie le nombre de sous répertoires du répertoire "rootDirectory".
        /// </summary>
        public static int GetTheNumberOfSubDirectories(string rootDirectory)
        {
            return Directory.GetDirectories(@rootDirectory).Length;
        }

        /// <summary>
        /// Renvoie si le fichier est vide.
        /// </summary>
        public static bool IsEmptyFile(string path)
        {
            return new FileInfo(@path).Length == 0;
        }

        /// <summary>
        /// Enlève l'extension d'un nom de fichier.
        /// </summary>
        public static string TakeOutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(@fileName);
        }

        /// <summary>
        /// Renvoit si le chemin est un fichier, c'est à dire qu'il contient une extension ou non et donc qu'il est un répértoire.
        /// </summary>
        public static bool DoesPathIsAFileOrADirectory(string path)
        {
            return !string.IsNullOrEmpty(Path.GetExtension(@path));
        }

        /// <summary>
        /// Renvoie le nom de tous les sous répertoires du répertoire "repositoryPath".
        /// </summary>
        public static string[] GetSubDirectories(string repositoryPath)
        {
            return Directory.GetDirectories(repositoryPath);
        }

        /// <summary>
        /// Renvoit le chemin pour accéder au bureau de l'utilisateur.
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        /// <summary>
        /// Renvoit la taille d'un répertoire en octets.
        /// </summary>
        public static long GetDirectorySize(string directoryPath)
        {
            return Directory.GetFiles(@directoryPath, "*", SearchOption.AllDirectories)
                            .Sum(subFile => (new FileInfo(@subFile).Length));
        }

        /// <summary>
        /// Exemple pour path = "Hello/Toto" -> créer le répertoire Hello si il n'éxiste pas et renvoit si il éxiste.
        /// </summary>
        public static void CreateFileDirectoryIfDontExists(string path)
        {
            SafeCreatePathIfDontExists(Path.GetDirectoryName(@path));
        }

        /// <summary>
        /// Détermine si un chemin est en lecture seule.
        /// </summary>
        public static bool DoesPathIsInReadOnly(string path)
        {
            return new FileInfo(path).IsReadOnly;
        }
    }
}
