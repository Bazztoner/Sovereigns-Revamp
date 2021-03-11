using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class HUDController : MonoBehaviour
{
    #region Variables
    public GameObject gui;
    public GameObject crosshair;
    Image[] spells = new Image[5];
    Image[] icons = new Image[5];
    Image[] frames = new Image[5];
    Image hp;
    Image mana;
    public Image youWin;
    public Image youLose;
    public Image timeOut;
    public Image victory;
    public Image defeat;
    public Image tie;
    public Image lockOnImage;
    public Image enemyLifeBarFill;
    public Image[] roundTexts;
    public Text damageText;
    public Animator hudAnim;
    public RectTransform enemyBar;
    public float comboLifeTime = 1f;

    private float _imgTime = 3f;
    private bool _gameInCourse = false;
    private Vector3 _v3Lock;
    private Vector2 _barLocalPos;
    private Vector2 _localPos;
    private RectTransform _rect;
    private Transform _lockTarget;
    private Transform _enemyBarPos;
    private Camera _cam;

    Text comboText;
    byte comboMeter;

    public int Combo
    {
        get { return (int)comboMeter; }
    }

    List<Text[]> allCooldowns = new List<Text[]>();

    public enum Spells { Environmental, Class, Universal, Mobility, Passive, Count };
    #endregion

    void Start()
    {
        GetEverything();
        AddEvents();
        DeactivateImages();
        Invoke("OnDividedScreen", 2f);
    }

    void LateUpdate()
    {
        FollowTarget();
        RelocateEnemyLifeBar();
    }

    void AddEvents()
    {
        EventManager.AddEventListener(CharacterEvents.LifeUpdate, ApplyHPChanges);
        EventManager.AddEventListener(CharacterEvents.ManaUpdate, ApplyManaChanges);
        EventManager.AddEventListener(SkillEvents.SpellCooldown, OnSpellCooldown);
        EventManager.AddEventListener(GameEvents.GameFinished, OnGameFinished);
        EventManager.AddEventListener(GameEvents.RestartRound, OnRestartRound);
        EventManager.AddEventListener(CameraEvents.LockOnActivated, OnLockOnActivated);
        EventManager.AddEventListener(UIEvents.SetRoundText, OnSetRoundText);
        EventManager.AddEventListener(GameEvents.EndOfMatch, OnEndOfMatch);
        EventManager.AddEventListener(CharacterEvents.PlayerDeath, OnPlayerDeath);
        EventManager.AddEventListener(UIEvents.UpdateComboMeter, OnCombo);
        EventManager.AddEventListener(UIEvents.SpellChanged, OnSpellChanged);
        EventManager.AddEventListener(UIEvents.UpdateSkillState, OnUpdateSkillState);
    }

    void OnUpdateSkillState(object[] paramsContainer)
    {
        var hasMana = (bool)paramsContainer[0];
        var i = (int)paramsContainer[1];
        var sender = (string)paramsContainer[2];

        if (this.gameObject.name == "HUD1" && sender == "Player1" || this.gameObject.name == "HUD2" && sender == "Player2")
        {
            var newColor = icons[i].color;
            newColor = hasMana ? Color.white : Color.cyan;
            icons[i].color = newColor;
        }
    }

    /// <summary>
    /// 0 - Skill Type Enum
    /// 1 - Player Name
    /// </summary>
    /// <param name="paramsContainer"></param>
    void OnSpellChanged(object[] paramsContainer)
    {
        var type = (Spells)paramsContainer[0];
        var pos = spells[(int)type].rectTransform;

        if (this.gameObject.name == "HUD1" && (string)paramsContainer[1] == "Player1" || this.gameObject.name == "HUD2" && (string)paramsContainer[1] == "Player2")
        {
            for (int i = 0; i < frames.Length - 1; i++)
            {
                if (i != (int)type && frames[i] != null)
                {
                    var newColor = frames[i].color;
                    newColor = Color.red;
                    frames[i].color = newColor;
                }
                else
                {
                    var newColor = frames[i].color;
                    newColor = Color.white;
                    frames[i].color = newColor;
                }
            }
        }
    }

    void OnCombo(object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.gameObject.name == "HUD1" && (string)paramsContainer[0] == "Player2")
            {
                UpdateCombo(true);
            }
            else if (this.gameObject.name == "HUD1" && (string)paramsContainer[0] == "Player1")
            {
                ResetCombo();
            }
            else if (this.gameObject.name == "HUD2" && (string)paramsContainer[0] == "Player1")
            {
                UpdateCombo(true);
            }
            else if (this.gameObject.name == "HUD2" && (string)paramsContainer[0] == "Player2")
            {
                ResetCombo();
            }
        }
    }

    void UpdateCombo(bool cancelInvoke)
    {
        if (cancelInvoke)
        {
            CancelInvoke("ResetCombo");
        }

        comboMeter++;
        comboText.text = "x" + comboMeter.ToString();
        Invoke("ResetCombo", comboLifeTime);
    }

    void ResetCombo()
    {
        comboMeter = 0;
        comboText.text = "";

        CancelInvoke("StartComboLifeTime");
    }

    void DeactivateImages()
    {
        if (this.gameObject.name != "HUD")
        {
            foreach (var img in roundTexts)
            {
                img.enabled = false;
            }

            youWin.enabled = false;
            youLose.enabled = false;
            timeOut.enabled = false;
            victory.enabled = false;
            defeat.enabled = false;
            tie.enabled = false;

            OnSetRoundText(new object[] { 0 });
        }
    }

    #region LockOn
    private void OnLockOnActivated(object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if ((this.gameObject.name == "HUD1" && (string)paramsContainer[0] == "Player1") ||
                (this.gameObject.name == "HUD2" && (string)paramsContainer[0] == "Player2"))
            {
                if ((bool)paramsContainer[1])
                {
                    crosshair.SetActive(false);
                    lockOnImage.gameObject.SetActive(true);
                    _lockTarget = (Transform)paramsContainer[2];
                    _cam = (Camera)paramsContainer[3];
                    _cam = _cam.GetComponentInChildren<Camera>();
                }
                else
                {
                    crosshair.SetActive(true);
                    lockOnImage.gameObject.SetActive(false);
                    lockOnImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
                }
            }
        }
        else
        {
            if ((bool)paramsContainer[1])
            {
                crosshair.SetActive(false);
                lockOnImage.gameObject.SetActive(true);
                _lockTarget = (Transform)paramsContainer[2];
                _cam = (Camera)paramsContainer[3];
                _cam = _cam.GetComponentInChildren<Camera>();
            }
            else
            {
                crosshair.SetActive(true);
                lockOnImage.gameObject.SetActive(false);
                lockOnImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
            }
        }
    }

    private void FollowTarget()
    {
        if (lockOnImage.gameObject.activeInHierarchy && GameManager.screenDivided)
        {
            var cam = this.gameObject.name == "HUD1" ? GameObject.Find("Cam1").GetComponent<Camera>() : GameObject.Find("Cam2").GetComponent<Camera>();
            var screenPoint = RectTransformUtility.WorldToScreenPoint(cam, _lockTarget.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(),
                                                                    screenPoint,
                                                                    this.GetComponent<Canvas>().worldCamera,
                                                                    out _localPos);
        }
        else if (lockOnImage.gameObject.activeInHierarchy && !GameManager.screenDivided)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, _cam.WorldToScreenPoint(_lockTarget.position), null, out _localPos);

        _v3Lock = _localPos;

        if (lockOnImage.gameObject.activeInHierarchy && _localPos != null && lockOnImage.rectTransform.localPosition != _v3Lock)
            lockOnImage.rectTransform.localPosition = Vector3.Lerp(lockOnImage.rectTransform.localPosition, _v3Lock, this.GetComponent<Canvas>().worldCamera.GetComponentInParent<CamRotationController>().smoothPercentage);
    }

    private void OnPlayerDeath(params object[] paramsContainer)
    {
        lockOnImage.gameObject.SetActive(false);
        lockOnImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
    }
    #endregion

    public void OnOnlineMode()
    {
        EventManager.DispatchEvent(GameEvents.DoConnect, new object[] { true });
    }

    public void OnOfflineMode()
    {
        EventManager.DispatchEvent(GameEvents.DoNotConnect, new object[] { true });
    }

    public void OnDummyTestMode()
    {
        EventManager.DispatchEvent(GameEvents.DoDummyTest, new object[] { true });
    }

    public void OnDividedScreen()
    {
        EventManager.DispatchEvent(GameEvents.DividedScreen, new object[] { true });
        GameManager.screenDivided = true;
    }

    void GetEverything()
    {
        var tnf = gui.transform.GetComponentsInChildren<Transform>(true);
        _rect = GetComponent<RectTransform>();

        foreach (var child in tnf)
        {
            if (child.gameObject.name == "HP") hp = child.GetComponent<Image>();
            else if (child.gameObject.name == "Mana") mana = child.GetComponent<Image>();
            else if (child.gameObject.name == "EnvSkillCD")
            {
                spells[0] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "ClassSkillCD")
            {
                spells[1] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "PickedSkillCD")
            {
                spells[2] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "BlinkSkillCD")
            {
                spells[3] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "PassiveSkillCD")
            {
                spells[4] = child.GetComponent<Image>();
            }

            else if (child.gameObject.name == "EnvSkillIcon")
            {
                icons[0] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "ClassSkillIcon")
            {
                icons[1] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "PickedSkillIcon")
            {
                icons[2] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "BlinkSkillIcon")
            {
                icons[3] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "PassiveSkillIcon")
            {
                icons[4] = child.GetComponent<Image>();
            }

            else if (child.gameObject.name == "EnvSkillFrame")
            {
                frames[0] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "ClassSkillFrame")
            {
                frames[1] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "PickedSkillFrame")
            {
                frames[2] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "BlinkSkillFrame")
            {
                frames[3] = child.GetComponent<Image>();
            }
            else if (child.gameObject.name == "PassiveSkillFrame")
            {
                frames[4] = child.GetComponent<Image>();
            }
        }

        comboText = transform.Find("Combometer").GetComponent<Text>();

        if (this.gameObject.name == "HUD1")
            _enemyBarPos = GameObject.Find("Player2").transform.Find("LifeBarPos");
        else if (this.gameObject.name == "HUD2")
            _enemyBarPos = GameObject.Find("Player1").transform.Find("LifeBarPos");

        FillArrays();
    }

    private void RelocateEnemyLifeBar()
    {
        if (_enemyBarPos != null)
        {
            var camPos = this.GetComponent<Canvas>().worldCamera.transform.position;
            var enemyDir = (_enemyBarPos.position - camPos).normalized;
            var enemyDir2 = new Vector2(enemyDir.x, enemyDir.z);
            var camForward = new Vector2(this.GetComponent<Canvas>().worldCamera.transform.forward.x, this.GetComponent<Canvas>().worldCamera.transform.forward.z);
            var angle = Vector3.Angle(enemyDir2, camForward);

            if (angle <= this.GetComponent<Canvas>().worldCamera.fieldOfView)
            {
                enemyBar.gameObject.SetActive(true);
                var cam = this.gameObject.name == "HUD1" ? GameObject.Find("Cam1").GetComponent<Camera>() : GameObject.Find("Cam2").GetComponent<Camera>();

                RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(),
                                      RectTransformUtility.WorldToScreenPoint(cam, _enemyBarPos.position),
                                      this.GetComponent<Canvas>().worldCamera,
                                      out _barLocalPos);

                enemyBar.localPosition = _barLocalPos;
            }
            else
            {
                enemyBar.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Fill cooldown arrays and lists
    /// </summary>
    void FillArrays()
    {
        for (int i = 0; i < (int)Spells.Count; i++)
        {
            allCooldowns.Add(spells[i].GetComponentsInChildren<Text>());

            for (int j = 0; j < allCooldowns[i].Length; j++)
            {
                allCooldowns[i][j].text = "";
            }

            spells[i].fillAmount = 0;
        }
    }

    private void ApplyHPChanges(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.gameObject.name == "HUD1" && (string)paramsContainer[0] == "Player1")
                hp.fillAmount = (float)paramsContainer[2];
            else if (this.gameObject.name == "HUD2" && (string)paramsContainer[0] == "Player2")
                hp.fillAmount = (float)paramsContainer[2];
            else if ((this.gameObject.name == "HUD1" && (string)paramsContainer[0] == "Player2") || (this.gameObject.name == "HUD2" && (string)paramsContainer[0] == "Player1"))
                enemyLifeBarFill.fillAmount = (float)paramsContainer[2];
        }
        else
        {
            hp.fillAmount = (float)paramsContainer[2];
        }
    }

    private void ApplyManaChanges(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.gameObject.name == "HUD1")
            {
                if ((string)paramsContainer[2] == "Player1")
                    mana.fillAmount = (float)paramsContainer[1];
            }
            else if (this.gameObject.name == "HUD2")
            {
                if ((string)paramsContainer[2] == "Player2")
                    mana.fillAmount = (float)paramsContainer[1];
            }
        }
        else mana.fillAmount = (float)paramsContainer[1];
    }

    void OnEnemyDamaged(object[] paramsContainer)
    {
        hudAnim.Play(0);
    }

    private void OnGameFinished(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == "") timeOut.enabled = true;
        else
        {
            if (this.gameObject.name == "HUD1")
            {
                if ((string)paramsContainer[0] == "Player1") youLose.enabled = true;
                else youWin.enabled = true;
            }
            else if (this.gameObject.name == "HUD2")
            {
                if ((string)paramsContainer[0] == "Player2") youLose.enabled = true;
                else youWin.enabled = true;
            }
            if (this.gameObject.name != "HUD")
            {
                if (youWin.enabled) StartCoroutine(RemoveImage(youWin, _imgTime));
                else if (youLose.enabled) StartCoroutine(RemoveImage(youLose, _imgTime));
                else if (timeOut.enabled) StartCoroutine(RemoveImage(timeOut, _imgTime));
            }
        }
    }

        private void OnEndOfMatch(params object[] paramsContainer)
        {
            if (this.gameObject.name != "HUD")
            {
                var p1 = (int)paramsContainer[0];
                var p2 = (int)paramsContainer[1];

                if (p1 == p2) tie.enabled = true;
                else if (p1 > p2)
                {
                    if (this.gameObject.name == "HUD1") victory.enabled = true;
                    else if (this.gameObject.name == "HUD2") defeat.enabled = true;
                }
                else
                {
                    if (this.gameObject.name == "HUD2") victory.enabled = true;
                    else if (this.gameObject.name == "HUD1") defeat.enabled = true;
                }
            }
        }

        private void OnRestartRound(params object[] paramsContainer)
        {
            damageText.text = "";

            if ((bool)paramsContainer[0])
            {
                EventManager.RemoveEventListener(CharacterEvents.LifeUpdate, ApplyHPChanges);
                EventManager.RemoveEventListener(CharacterEvents.ManaUpdate, ApplyManaChanges);
                EventManager.RemoveEventListener(SkillEvents.SpellCooldown, OnSpellCooldown);
                EventManager.RemoveEventListener(GameEvents.GameFinished, OnGameFinished);
                EventManager.RemoveEventListener(GameEvents.RestartRound, OnRestartRound);
                EventManager.RemoveEventListener(CameraEvents.LockOnActivated, OnLockOnActivated);
                EventManager.RemoveEventListener(UIEvents.SetRoundText, OnSetRoundText);
                EventManager.RemoveEventListener(GameEvents.EndOfMatch, OnEndOfMatch);
            }
        }

        private void OnSetRoundText(params object[] paramsContainer)
        {
            if (this.gameObject.name != "HUD")
            {
                roundTexts[(int)paramsContainer[0]].enabled = true;
                StartCoroutine(RemoveImage(roundTexts[(int)paramsContainer[0]], _imgTime));
            }
        }

        #region Cooldown
        public void OnSpellCooldown(params object[] paramsContainer)
        {
            if (GameManager.screenDivided)
            {
                if (this.gameObject.name == "HUD1")
                {
                    if ((string)paramsContainer[2] == "Player1")
                    {
                        StartCoroutine(ApplyCooldown((float)paramsContainer[0], (Spells)paramsContainer[1], this.gameObject.name));
                    }
                }
                else if (this.gameObject.name == "HUD2")
                {
                    if ((string)paramsContainer[2] == "Player2")
                    {
                        StartCoroutine(ApplyCooldown((float)paramsContainer[0], (Spells)paramsContainer[1], this.gameObject.name));
                    }
                }
            }
            else StartCoroutine(ApplyCooldown((float)paramsContainer[0], (Spells)paramsContainer[1]));
        }

        IEnumerator ApplyCooldown(float cooldown, Spells spell)
        {
            for (int j = 0; j < allCooldowns[(int)spell].Length; j++)
            {
                allCooldowns[(int)spell][j].text = cooldown < 1 ? "1" : Mathf.Floor(cooldown).ToString();
            }

            StartCoroutine(FillImageCooldown(cooldown, spells[(int)spell]));

            float cd = cooldown;

            while (true)
            {
                yield return new WaitForSeconds(1);
                cd--;

                if (cd > 0)
                {
                    for (int j = 0; j < allCooldowns[(int)spell].Length; j++)
                    {
                        allCooldowns[(int)spell][j].text = cd < 1 ? "1" : Mathf.Floor(cd).ToString();
                    }
                }
                else
                {
                    spells[(int)spell].fillAmount = 0;

                    for (int j = 0; j < allCooldowns[(int)spell].Length; j++)
                    {
                        allCooldowns[(int)spell][j].text = "";
                    }

                    yield break;
                }
            }
        }

        IEnumerator FillImageCooldown(float cooldown, Image image)
        {
            var cd = cooldown;
            while (cd > 0)
            {
                cd -= Time.deltaTime;
                image.fillAmount = cd / cooldown;
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator RemoveImage(Image img, float time)
        {
            yield return new WaitForSeconds(time);
            img.enabled = false;
        }

        #region SplitScreen KORRUPTINES
        IEnumerator ApplyCooldown(float cooldown, Spells spell, string hudName)
        {
            if (this.gameObject.name == hudName)
            {
                for (int j = 0; j < allCooldowns[(int)spell].Length; j++)
                {
                    allCooldowns[(int)spell][j].text = cooldown < 1 ? "1" : Mathf.Floor(cooldown).ToString();
                }

                StartCoroutine(FillImageCooldown(cooldown, spells[(int)spell]));

                float cd = cooldown;

                while (true)
                {
                    yield return new WaitForSeconds(1);
                    cd--;

                    if (cd > 0)
                    {
                        for (int j = 0; j < allCooldowns[(int)spell].Length; j++)
                        {
                            allCooldowns[(int)spell][j].text = cd < 1 ? "1" : Mathf.Floor(cd).ToString();
                        }
                    }
                    else
                    {
                        spells[(int)spell].fillAmount = 0;

                        for (int j = 0; j < allCooldowns[(int)spell].Length; j++)
                        {
                            allCooldowns[(int)spell][j].text = "";
                        }

                        yield break;
                    }
                }
            }
        }

        IEnumerator FillImageCooldown(float cooldown, Image image, string hudName)
        {
            if (this.gameObject.name == hudName)
            {
                var cd = cooldown;
                while (cd > 0)
                {
                    cd -= Time.deltaTime;
                    image.fillAmount = cd / cooldown;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        #endregion
        #endregion
    }
