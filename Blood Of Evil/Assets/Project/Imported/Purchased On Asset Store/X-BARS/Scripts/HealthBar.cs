using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BloodOfEvil.Scene;

//NOTE! You should hava a Camera with "MainCamera" tag and a canvas with a Screen Space - Overlay mode to script works properly;

public class HealthBar : MonoBehaviour
{
    private float minmumLife;
    private float maximumLife;

    public RectTransform HealthbarPrefab;			//Our health bar prefab;
	public float YOffset;							//Horizontal position offset;
	public bool keepSize = true;	                //keep distance independed size;
	public float scale = 1;							//Scale of the healthbar;
	public Vector2 sizeOffsets;						//Use this to overwright healthbar width and height values;
	public bool DrawOFFDistance;					//Disable health bar if it out of drawDistance;
	public float drawDistance = 10;
	public bool showHealthInfo;						//Show the health info on top of the health bar or not;
	public enum HealthInfoAlignment {top, center, bottom};
	public HealthInfoAlignment healthInfoAlignment = HealthInfoAlignment.center;
	public float healthInfoSize = 10;
    public AlphaSettings alphaSettings;
	private Image healthVolume, backGround;			//Health bar images, should be named as "Health" and "Background";
	private Text healthInfo;						//Health info, a healthbar's child Text object(should be named as HealthInfo);
	private CanvasGroup canvasGroup;
	private Vector2 healthbarPosition, healthbarSize, healthInfoPosition;
	private Transform thisT;
	private float lastHealth, camDistance, dist, pos, rate;
	private Camera cam;
    private GameObject healthbarRoot;
	[HideInInspector]public Canvas canvas;

    public void SetMaximumLife(float maximumLife)
    {
        this.maximumLife = maximumLife;
    }

    public void SetMinimumLife(float minmumLife)
    {
        this.minmumLife = (int)minmumLife;
    }

    public void DisableHealthBar()
    {
        this.HealthbarPrefab.gameObject.SetActive(false);
    }

    public void EnableHealthBar()
    {
        this.HealthbarPrefab.gameObject.SetActive(true);
    }

    public void Initialize(float minLife, float maxLife)
	{
        this.canvas = SceneServicesContainer.Instance.GameObjectInSceneReferencesService.Get("[UI] Enemies Health Bars").transform.parent.GetComponent<Canvas>();

        this.SetMaximumLife(maxLife);
        lastHealth = maxLife;
        this.SetMinimumLife(minLife);

        GetComponent<BloodOfEvil.Enemies.EnemyServicesAndModulesContainer>().Instance.AttributesModule.EnemySizeListener += delegate(float enemySize)
        {
            this.YOffset *= enemySize;
            this.scale *= enemySize;
        };

        #region Default Initialization
        if (!HealthbarPrefab)
        {
            Debug.LogWarning("HealthbarPrefab is empty, please assign your healthbar prefab in inspector");
            return;
        }

        thisT = this.transform;
        if (canvas.transform.FindChild("HealthbarRoot") != null)
            healthbarRoot = canvas.transform.FindChild("HealthbarRoot").gameObject;
        else
            healthbarRoot = new GameObject("HealthbarRoot", typeof(RectTransform), typeof(HealthbarRoot));
        healthbarRoot.transform.SetParent(canvas.transform, false);

        //HealthbarPrefab = SceneServicesContainer.Instance.ObjectsPoolService.AddObjectInPool("HealthBarExample").GetComponent<RectTransform>();
        HealthbarPrefab = SceneServicesContainer.Instance.PrefabReferencesService.Instantiate("HealthBarExample").GetComponent<RectTransform>();
        HealthbarPrefab.position = new Vector2(-1000, -1000);
        HealthbarPrefab.rotation = Quaternion.identity;

        HealthbarPrefab.name = "HealthBar";
        HealthbarPrefab.SetParent(healthbarRoot.transform, false);
        canvasGroup = HealthbarPrefab.GetComponent<CanvasGroup>();

        healthVolume = HealthbarPrefab.transform.Find("Health").GetComponent<Image>();
        backGround = HealthbarPrefab.transform.Find("Background").GetComponent<Image>();
        healthInfo = HealthbarPrefab.transform.Find("HealthInfo").GetComponent<Text>();
        healthInfo.resizeTextForBestFit = true;
        healthInfo.rectTransform.anchoredPosition = Vector2.zero;
        healthInfoPosition = healthInfo.rectTransform.anchoredPosition;
        healthInfo.resizeTextMinSize = 1;
        healthInfo.resizeTextMaxSize = 500;

        healthbarSize = HealthbarPrefab.sizeDelta;
        canvasGroup.alpha = alphaSettings.fullAplpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        cam = Camera.main;
        #endregion
    }


    // Update is called once per frame
    void FixedUpdate () {
		if(!HealthbarPrefab)
			return;
		
		HealthbarPrefab.transform.position = cam.WorldToScreenPoint(new Vector3(thisT.position.x, thisT.position.y + YOffset, thisT.position.z));
        healthVolume.fillAmount = this.minmumLife / this.maximumLife;

		float maxDifference = 0.1F;


		if(backGround.fillAmount > healthVolume.fillAmount + maxDifference)
			backGround.fillAmount = healthVolume.fillAmount + maxDifference;
        if (backGround.fillAmount > healthVolume.fillAmount)
            backGround.fillAmount -= (1 / (maximumLife / 100)) * Time.deltaTime;
        else
            backGround.fillAmount = healthVolume.fillAmount;
	}
	
	
	void Update()
	{
		if(!HealthbarPrefab)
			return;
		
		camDistance = Vector3.Dot(thisT.position - cam.transform.position, cam.transform.forward);
		
		if(showHealthInfo)
			healthInfo.text = this.minmumLife +" / "+maximumLife;
		else
			healthInfo.text = "";

        if(lastHealth != this.minmumLife)
        {
            rate = Time.time + alphaSettings.onHit.duration;
            lastHealth = this.minmumLife;
        }

        if (!OutDistance() && IsVisible())
        {
            if (this.minmumLife <= 0)
            {
                if (alphaSettings.nullFadeSpeed > 0)
                {
                    //Destroy(gameObject);
                    if (backGround.fillAmount <= 0)
                        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alphaSettings.nullAlpha, alphaSettings.nullFadeSpeed);
                }
                else
                    canvasGroup.alpha = alphaSettings.nullAlpha;
            }
            else if (this.minmumLife == maximumLife)
                canvasGroup.alpha = alphaSettings.fullFadeSpeed > 0 ? Mathf.MoveTowards(canvasGroup.alpha, alphaSettings.fullAplpha, alphaSettings.fullFadeSpeed) : alphaSettings.fullAplpha;
            else
            {
                if (rate > Time.time)
                    canvasGroup.alpha = alphaSettings.onHit.onHitAlpha;
                else
                    canvasGroup.alpha = alphaSettings.onHit.fadeSpeed > 0 ? Mathf.MoveTowards(canvasGroup.alpha, alphaSettings.defaultAlpha, alphaSettings.onHit.fadeSpeed) : alphaSettings.defaultAlpha;
            }
        }
        else
            canvasGroup.alpha = alphaSettings.defaultFadeSpeed > 0 ? Mathf.MoveTowards(canvasGroup.alpha, 0, alphaSettings.defaultFadeSpeed) : 0;

		
		if(this.minmumLife <= 0)
			this.minmumLife = 0;

		dist = keepSize ? camDistance / scale :  scale;

		HealthbarPrefab.sizeDelta = new Vector2 (healthbarSize.x/(dist-sizeOffsets.x/100), healthbarSize.y/(dist-sizeOffsets.y/100));
		
		healthInfo.rectTransform.sizeDelta = new Vector2 (HealthbarPrefab.sizeDelta.x * healthInfoSize/10, 
		                                                  HealthbarPrefab.sizeDelta.y * healthInfoSize/10);
		
		healthInfoPosition.y = HealthbarPrefab.sizeDelta.y + (healthInfo.rectTransform.sizeDelta.y - HealthbarPrefab.sizeDelta.y) / 2;
		
		if(healthInfoAlignment == HealthInfoAlignment.top)
			healthInfo.rectTransform.anchoredPosition = healthInfoPosition;
		else if (healthInfoAlignment == HealthInfoAlignment.center)
			healthInfo.rectTransform.anchoredPosition = Vector2.zero;
		else
			healthInfo.rectTransform.anchoredPosition = -healthInfoPosition;

        if(this.minmumLife > maximumLife)
            maximumLife = this.minmumLife;
	}

	bool IsVisible()
	{
		return canvas.pixelRect.Contains (HealthbarPrefab.position);
	}

    bool OutDistance()
    {
        return DrawOFFDistance == true && camDistance > drawDistance;
    }

    public float GetDefaultHealth()
    {
        return maximumLife;
    }

    public float GetCurrentHealth()
    {
        return this.minmumLife;
    }
}

[System.Serializable]
public class AlphaSettings
{
    
    public float defaultAlpha = 0.7F;           //Default healthbar alpha (health is bigger then zero and not full);
    public float defaultFadeSpeed = 0.1F;
    public float fullAplpha = 1.0F;             //Healthbar alpha when health is full;
    public float fullFadeSpeed = 0.1F;
    public float nullAlpha = 0.0F;              //Healthbar alpha when health is zero or less;
    public float nullFadeSpeed = 0.1F;
    public OnHit onHit;                         //On hit settings
}

[System.Serializable]
public class OnHit
{
    public float fadeSpeed = 0.1F;              //Alpha state fade speed;
    public float onHitAlpha = 1.0F;             //On hit alpha;
    public float duration = 1.0F;               //Duration of alpha state;
}
