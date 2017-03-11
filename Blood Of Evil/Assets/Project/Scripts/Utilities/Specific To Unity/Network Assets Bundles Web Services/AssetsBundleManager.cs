using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Utilities
{
    static public class AssetBundleManager
    {
        // A dictionary to hold the AssetBundle references
        static private Dictionary<string, AssetBundleRef> dictAssetBundleRefs;

        static AssetBundleManager()
        {
            //prefere l'utilisation d'une hash map (complexite O(1) au lieu d'un dictionnary qui est potentiellement un red black tree : complexite o(log n)
            //car dans notre cas on a pas besoin que nos donnees soit tries, on vu juste y acceder rapidement car pour chaque get bundle d'image je compte faire :

            dictAssetBundleRefs = new Dictionary<string, AssetBundleRef>();
        }
        // Class with the AssetBundle reference, url and version
        private class AssetBundleRef
        {
            public AssetBundle assetBundle = null;
            public int version;
            public string url;
            public AssetBundleRef(string strUrlIn, int intVersionIn)
            {
                url = strUrlIn;
                version = intVersionIn;
            }
        };
        // Get an AssetBundle
        public static AssetBundle getAssetBundle(string url, int version = 0)
        {
            /* petite methode pour recuperer la key name ? */
            string keyName = url + version.ToString();
            AssetBundleRef abRef;
            //petit ternaire ? =)
            if (dictAssetBundleRefs.TryGetValue(keyName, out abRef))
                return abRef.assetBundle;
            else
                return null;
        }
        // Download an AssetBundle

        public static IEnumerator downloadAssetBundle(string url, int version = 0, System.Action<float> DownloadProgress = null)
        {
            /* petite methode pour recuperer la key name ? */
            string keyName = url + version.ToString();
            if (dictAssetBundleRefs.ContainsKey(keyName))
                yield return null;
            else
            {
                Debug.Log("DL: " + url);

                using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
                {
                    while (!www.isDone)
                    {
                        if (null != DownloadProgress)
                            DownloadProgress(www.progress);
                        yield return null;
                    }
                    if (www.error != null)
                    {
                        Debug.LogError(string.Format("Bundle {0} have not been found, error : {1}", url, www.error));
                        throw new Exception("WWW download:" + www.error);
                    }

                    //pourquoi ne pas faire un constructeur qui prend 3 parametres ?
                    AssetBundleRef abRef = new AssetBundleRef(url, version);
                    abRef.assetBundle = www.assetBundle;
                    //que se passe t'il si la clef est deja dans le dictionnaire ? crash..
                    dictAssetBundleRefs.Add(keyName, abRef);
                }
            }
        }

        // Unload an AssetBundle
        public static void Unload(string url, int version, bool allObjects)
        {
            //une petite methode pour recuperer ta keyname ca serait cool, vu que tu l'utilise a 3 endroits differents =)
            string keyName = url + version.ToString();
            AssetBundleRef abRef;

            if (dictAssetBundleRefs.TryGetValue(keyName, out abRef))
            {
                abRef.assetBundle.Unload(allObjects);
                abRef.assetBundle = null;
                dictAssetBundleRefs.Remove(keyName);
            }
        }

        //public IEnumerator GetAssetBundleOrDownloadIt(string url, AssetBundle bundle)
        //{
        //    bundle = AssetBundleManager.getAssetBundle(url, 0);

        //    if (bundle == null)
        //    {
        //        yield return StartCoroutine(AssetBundleManager.downloadAssetBundle(url, 0));
        //        bundle = AssetBundleManager.getAssetBundle(url, 0);
        //    }
        //}
    }
}