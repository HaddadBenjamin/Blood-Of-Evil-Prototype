using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BloodOfEvil.Utilities;

public class ImagesService : ASingletonMonoBehaviour<ImagesService>
{
    #region Public Behaviour
    /// <summary>
    /// Fait clignotter une image pendant ("timerPerBlink" * numberOfBlinks * 2).
    /// </summary>
    public IEnumerator Blink(object[] parameters)
    {
        Image image = (Image)parameters[0];
        Sprite blinkSprite = (Sprite)parameters[1];
        float timePerBlink = (float)parameters[2];
        int numberOfBlinks = (int)parameters[3];

        Sprite defaultSprite = image.sprite;

        yield return null;

        for (int i = 0; i < numberOfBlinks; i++)
        {
            image.sprite = blinkSprite;

            yield return new WaitForSeconds(timePerBlink);

            image.sprite = defaultSprite;
        }
    }
    #endregion
}
