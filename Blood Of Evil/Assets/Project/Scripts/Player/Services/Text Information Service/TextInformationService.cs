using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Services.TextInformation
{
    using Helpers;
    using Extensions;
    using ObjectInScene;

    public class TextInformationService : AInitializableComponent
    {
        #region Fields
        private Queue<Text> textInformations;
        private List<RectTransform> textInformationsRectTranform;
        private Transform parentTransform;

        [SerializeField]
        private Color informationColor = ColorHelper.Blue;
        [SerializeField]
        private Color warningColor = ColorHelper.Yellow;
        [SerializeField]
        private Color rareEventColor = ColorHelper.Purple;
        [SerializeField]
        private Color youCantDoAnActionColor = ColorHelper.Red;

        private readonly float UpdateTime = 0.2f;
        #endregion

        #region Abstract Initializer
        public override void Initialize()
        {
            this.parentTransform = PlayerServicesAndModulesContainer.Instance.GameObjectInSceneReferencesService.Get("Screen Center").transform;

            this.textInformations = new Queue<Text>();
            this.textInformationsRectTranform = new List<RectTransform>();

            StartCoroutine(this.UpdatePoolElementsEveryNSeconds());
        }
        #endregion

        #region Unity Methods
        void Update()
        {
            this.UpdatePositionPoolElements();
        }
        #endregion

        #region Behaviour Methods
        public void AddTextInformation(string content, ETextInformation type = ETextInformation.Information)
        {
            GameObject textInformationGameObject = PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.AddObjectInPool("Text Information");
            Text text = textInformationGameObject.GetComponent<Text>();

            text.text = content;
            text.color =
                ETextInformation.Information == type ? this.informationColor :
                ETextInformation.RareEvent == type ? this.rareEventColor :
                ETextInformation.Warning == type ? this.warningColor :
                                                        this.youCantDoAnActionColor;

            textInformationGameObject.transform.SetPositionAndParent(Vector3.zero, this.parentTransform);

            // Enqueue : Add
            // Dequeue : Remove
            // Peek : Get First Element
            this.textInformations.Enqueue(text);
            this.textInformationsRectTranform.Add(textInformationGameObject.GetComponent<RectTransform>());
        }

        /// <summary>
        /// Permet de lancer une fonction tous les n temps de façon propre.
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdatePoolElementsEveryNSeconds()
        {
            while (true)
            {
                this.UpdatePoolElements();

                yield return new WaitForSeconds(this.UpdateTime);
            }
        }

        private void UpdatePoolElements()
        {
            this.UpdateDeletePoolElements();
        }

        private void UpdatePositionPoolElements()
        {
            for (int rectTransformIndex = 0; rectTransformIndex < this.textInformationsRectTranform.Count; rectTransformIndex++)
                this.textInformationsRectTranform[rectTransformIndex].SetPosition
                    (Vector3.Lerp(this.textInformationsRectTranform[rectTransformIndex].position, new Vector3(0.0f, rectTransformIndex * 50.0f, 0.0f), 1f));
        }

        private void UpdateDeletePoolElements()
        {
            if (this.textInformations.Count > 0)
            {
                Text text = this.textInformations.Peek();

                if (text.color.a <= 0.0f)
                {
                    this.textInformations.Dequeue();
                    this.textInformationsRectTranform.Remove(text.gameObject.GetComponent<RectTransform>());

                    // Pour la pool puisse le récupérer.
                    text.gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
}