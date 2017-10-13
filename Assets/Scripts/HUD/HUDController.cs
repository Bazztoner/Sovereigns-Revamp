using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class HUDController : Photon.MonoBehaviour
{
    #region Variables
    public GameObject gui;
    public GameObject crosshair;
    Image[] spells = new Image[5];
    Image hp;
    Image mana;
    public Image youWin;
    public Image youLose;
    public Image timeOut;
    public Image victory;
    public Image defeat;
    public Image tie;
    public Image enemyLifebar;
    public Image lockOnImage;
    public Image[] roundTexts;
    public Text damageText;
    public Animator hudAnim;
    public RectTransform enemyBar;

    private float _imgTime = 3f;
    private float _smooth = 0.1f;
    private bool _gameInCourse = false;
    private Vector3 _v3Lock;
    private Vector2 _localPos;
    private RectTransform _rect;
    private Transform _lockTarget;
    private Camera _cam;

    List<Text[]> allCooldowns = new List<Text[]>();

    public enum Spells { Environmental, Class, Picked, Mobility, Passive, Count };
    #endregion

    void Start()
    {
        GetEverything();
        AddEvents();
        DeactivateImages();
    }

    void LateUpdate()
    {
        FollowTarget();
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("LifeUpdate", ApplyHPChanges);
        EventManager.AddEventListener("ManaUpdate", ApplyManaChanges);
        EventManager.AddEventListener("SpellCooldown", OnSpellCooldown);
        EventManager.AddEventListener("EnemyLifeUpdate", ApplyEnemyHPChanges);
        EventManager.AddEventListener("EnemyDamaged", OnEnemyDamaged);
        EventManager.AddEventListener("GameFinished", OnGameFinished);
        EventManager.AddEventListener("RestartRound", OnRestartRound);
        EventManager.AddEventListener("LockOnActivated", OnLockOnActivated);
        EventManager.AddEventListener("SetRoundText", OnSetRoundText);
        EventManager.AddEventListener("EndOfMatch", OnEndOfMatch);
        EventManager.AddEventListener("PlayerDeath", OnPlayerDeath);
    }

    private void DeactivateImages()
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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, _cam.WorldToScreenPoint(_lockTarget.position), _cam, out _localPos);
        else if (lockOnImage.gameObject.activeInHierarchy && !GameManager.screenDivided)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, _cam.WorldToScreenPoint(_lockTarget.position), null, out _localPos);

        _v3Lock = new Vector3(_localPos.x, _localPos.y, 0f);

        if (lockOnImage.gameObject.activeInHierarchy && _localPos != null && lockOnImage.rectTransform.localPosition != _v3Lock)
            lockOnImage.rectTransform.localPosition = Vector3.Lerp(lockOnImage.rectTransform.localPosition, _v3Lock, _smooth);
    }

    private void OnPlayerDeath(params object[] paramsContainer)
    {
        lockOnImage.gameObject.SetActive(false);
        lockOnImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
    }
    #endregion

    public void OnOnlineMode()
    {
        EventManager.DispatchEvent("DoConnect", new object[] { true });
    }

    public void OnOfflineMode()
    {
        EventManager.DispatchEvent("DoNotConnect", new object[] { true });
    }

    public void OnDummyTestMode()
    {
        EventManager.DispatchEvent("DoDummyTest", new object[] { true });
    }

    public void OnDividedScreen()
    {
        EventManager.DispatchEvent("DividedScreen", new object[] { true });
        GameManager.screenDivided = true;
    }

    void GetEverything()
    {
        var tnf = gui.transform.GetComponentsInChildren<Transform>(false);
        _rect = this.GetComponent<RectTransform>();

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
        }

        FillArrays();
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
            if (this.gameObject.name == "HUD1")
            {
                if ((string)paramsContainer[0] == "Player1")
                    hp.fillAmount = (float)paramsContainer[2];
                else if ((string)paramsContainer[0] == "Player2" && enemyLifebar != null)
                    enemyLifebar.fillAmount = (float)paramsContainer[2];
            }
            else if (this.gameObject.name == "HUD2")
            {
                if ((string)paramsContainer[0] == "Player1" && enemyLifebar != null)
                    enemyLifebar.fillAmount = (float)paramsContainer[2];
                else if ((string)paramsContainer[0] == "Player2")
                    hp.fillAmount = (float)paramsContainer[2];
            }
        }
        else
        {
            hp.fillAmount = (float)paramsContainer[2];
            if (!PhotonNetwork.offlineMode) photonView.RPC("NotifyLife", PhotonTargets.All, hp.fillAmount, PhotonNetwork.player.NickName);
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

    [PunRPC]
    public void NotifyLife(float fillAmount, string nickName)
    {
        ApplyEnemyHPChanges(new object[] { fillAmount, nickName });
    }

    void ApplyEnemyHPChanges(params object[] paramsContainer)
    {
        //Puse esto para que deje de tirar el error al inicio del juego de que no encuentra la barra de vida.
        if (enemyLifebar != null)
        {
            if (PhotonNetwork.offlineMode)
                enemyLifebar.fillAmount = (float)paramsContainer[1];
            else if ((string)paramsContainer[1] != PhotonNetwork.player.NickName)
                enemyLifebar.fillAmount = (float)paramsContainer[0];
        }
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
            if (!PhotonNetwork.offlineMode)
            {
                if ((string)paramsContainer[0] == PhotonNetwork.player.NickName) youLose.enabled = true;
                else youWin.enabled = true;
            }
            else if (GameManager.screenDivided)
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
            }
        }
        if (this.gameObject.name != "HUD")
        {
            if (youWin.enabled) StartCoroutine(RemoveImage(youWin, _imgTime));
            else if (youLose.enabled) StartCoroutine(RemoveImage(youLose, _imgTime));
            else if (timeOut.enabled) StartCoroutine(RemoveImage(timeOut, _imgTime));
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
            EventManager.RemoveEventListener("LifeUpdate", ApplyHPChanges);
            EventManager.RemoveEventListener("ManaUpdate", ApplyManaChanges);
            EventManager.RemoveEventListener("SpellCooldown", OnSpellCooldown);
            EventManager.RemoveEventListener("EnemyLifeUpdate", ApplyEnemyHPChanges);
            EventManager.RemoveEventListener("EnemyDamaged", OnEnemyDamaged);
            EventManager.RemoveEventListener("GameFinished", OnGameFinished);
            EventManager.RemoveEventListener("RestartRound", OnRestartRound);
            EventManager.RemoveEventListener("LockOnActivated", OnLockOnActivated);
            EventManager.RemoveEventListener("SetRoundText", OnSetRoundText);
            EventManager.RemoveEventListener("EndOfMatch", OnEndOfMatch);
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
