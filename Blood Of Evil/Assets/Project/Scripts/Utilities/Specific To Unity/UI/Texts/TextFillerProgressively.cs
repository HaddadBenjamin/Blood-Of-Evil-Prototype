using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using BloodOfEvil.Extensions;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Cette classe permet de remplir un texte de façon progressive via différents mode et à une certaine vitesse configurable.
    /// </summary>
    public class TextFillerProgressively : MonoBehaviour
    {
        #region Fields
        public enum ETextFillerWritingMode
        {
            TimeToWriteOneCharacter,
            TimeToWriteAllTheText,
        };

        /// <summary>
        /// C'est le texte que l'on souhaite écrire dans notre UnityEngine.UI.Text.
        /// </summary>
        private string textToWrite;
        [SerializeField, Tooltip("C'est la zoneque l'on va remplir avec textToWrite.")]
        private Text textToFill;
        [SerializeField, Tooltip("C'est le mode d'écriture.")]
        private ETextFillerWritingMode writingMode;
        [SerializeField, Tooltip("C'est la vitesse d'écriture soit par caractère, soit pour écrire toute la chaîne de caractères, elle dépend du mode d'écriture")]
        private float writingTime;

        /// <summary>
        /// Détermine si une coroutine est entrain de se lancer sur cette objet.
        /// </summary>
        private bool isACoroutineRunning = false;

        [Tooltip("Cet évênement est appelé lorsque la chaîne de caractères s'est complété.")]
        public Action OnTextHaveBeenWrited;

        /// <summary>
        /// Lorsque cette option est activé, alors on peut arrêter toutes les couroutines de l'objet.
        /// </summary>
        private bool canStopAllCoroutines;
        #endregion

        #region Unity Behaviour
        /// <summary>
        /// Permet de stopper la coroutine actuel et de la relancer avec les nouveaux paramètres spécifiés.
        /// </summary>
        IEnumerator TextFillerDiffered()
        {
            yield return new WaitForEndOfFrame();

            canStopAllCoroutines = false;

            StartCoroutine(this.FillText(this.textToWrite, this.writingMode, this.writingTime));
        }

        public void FillText(string TheTextToWrite)
        {
            StartCoroutine(FillTextCoroutine(TheTextToWrite));
        }

        public IEnumerator FillTextCoroutine(string TheTextToWrite)
        {
            StartCoroutine(FillText(TheTextToWrite, this.writingMode, this.writingTime));
            yield return null;
        }


        /// <summary>
        /// Coroutine qui remplie un UnityEngine.UI.Text, ou on peut rentrer du Text avec le text en parametre en
        ///  fonction du mode souhaité et du time l'Area se fillera character par
        ///  character.
        ///  Si jamais une autre Coroutine TextFiller tourne arrete toute les coroutine
        ///  Et appel TextFillerDiffered.
        /// </summary>
        public IEnumerator FillText(string TheTextToWrite, ETextFillerWritingMode TheWritingMode, float TheWritingTime)
        {
            this.textToWrite = TheTextToWrite;
            this.writingMode = TheWritingMode;
            this.writingTime = TheWritingTime;

            textToFill.text = "";

            if (isACoroutineRunning)
            {
                textToFill.text = "";

                canStopAllCoroutines = true;
                isACoroutineRunning = false;

                StopAllCoroutines();
            }

            isACoroutineRunning = !canStopAllCoroutines;

            float timeToWaitForAChar =
                TheWritingMode == ETextFillerWritingMode.TimeToWriteAllTheText ?
                TheWritingTime / TheTextToWrite.Length :
                TheWritingTime;

            int stringLength = TheTextToWrite.Length;
            for (int i = 0; i < stringLength && !canStopAllCoroutines; i++)
            {
                textToFill.text += TheTextToWrite[i];
                yield return new WaitForSeconds(timeToWaitForAChar);
            }

            isACoroutineRunning = false;

            this.OnTextHaveBeenWrited.SafeCall();

            if (canStopAllCoroutines)
                StartCoroutine("TextFillerDiffered");
        }
        #endregion
    }
}