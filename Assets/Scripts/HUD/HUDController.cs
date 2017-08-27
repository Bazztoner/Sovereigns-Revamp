using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class HUDController : Photon.MonoBehaviour
{
    public GameObject gui;
    public GameObject crosshair;
    Image[] spells = new Image[5];
    Image hp;
    Image mana;
    public Image enemyLifebar;
    public Text damageText;
    public Animator hudAnim;

    List<Text[]> allCooldowns = new List<Text[]>();

    public enum Spells { Environmental, Class, Picked, Mobility, Passive, Count };

    void Start()
    {
        GetEverything();
        EventManager.AddEventListener("LifeUpdate", ApplyHPChanges);
        EventManager.AddEventListener("ManaUpdate", ApplyManaChanges);
        EventManager.AddEventListener("SpellCooldown", OnSpellCooldown);
        EventManager.AddEventListener("EnemyLifeUpdate", ApplyEnemyHPChanges);
        EventManager.AddEventListener("EnemyDamaged", OnEnemyDamaged);
        EventManager.AddEventListener("GameFinished", OnGameFinished);
        //EventManager.AddEventListener("DamageMade", OnDamageMade);
    }

    public void OnOnlineMode()
    {
        EventManager.DispatchEvent("DoConnect");
    }

    public void OnOfflineMode()
    {
        EventManager.DispatchEvent("DoNotConnect");
    }

    public void OnDividedScree()
    {
        EventManager.DispatchEvent("DividedScreen");
        GameManager.screenDivided = true;
    }

    void GetEverything()
    {
        var tnf = gui.transform.GetComponentsInChildren<Transform>(false);

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

    [PunRPC]
    public void NotifyLife(float fillAmount, string nickName)
    {
        ApplyEnemyHPChanges(new object[] { fillAmount, nickName });
    }

    private void ApplyManaChanges(params object[] paramsContainer)
    {
        mana.fillAmount = (float)paramsContainer[1];
    }

    void ApplyEnemyHPChanges(params object[] paramsContainer)
    {
        //Puse esto para que deje de tirar el error al inicio del juego de que no encuentra la barra de vida.
        if (enemyLifebar != null)
        {
            if (PhotonNetwork.offlineMode)
                enemyLifebar.fillAmount = (float)paramsContainer[1];
            else if((string)paramsContainer[1] != PhotonNetwork.player.NickName)
                enemyLifebar.fillAmount = (float)paramsContainer[0];
        } 
    }

    void OnEnemyDamaged(object[] paramsContainer)
    {
        hudAnim.Play(0);
    }

    private void OnDamageMade(params object[] paramsContainer)
    {
        var damage = (float)paramsContainer[0];
        damageText.text += damage.ToString() + " Damage Made.\n";
        StartCoroutine(CleanText());
    }

    private void OnGameFinished(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == "") damageText.text = "Time Out";
        else
        {
            if (!PhotonNetwork.offlineMode)
            {
                if ((string)paramsContainer[0] == PhotonNetwork.player.NickName) damageText.text = "You Lose";
                else damageText.text = "You Win";
            }
            else if (GameManager.screenDivided)
            {
                if (this.gameObject.name == "HUD1")
                {
                    if ((string)paramsContainer[0] == "Player1") damageText.text = "You Lose";
                    else damageText.text = "You Win";
                }
                else if (this.gameObject.name == "HUD2")
                {
                    if ((string)paramsContainer[0] == "Player2") damageText.text = "You Lose";
                    else damageText.text = "You Win";
                }
            }
        }
    }

    #region Cooldown
    public void OnSpellCooldown(params object[] paramsContainer)
    {
        StartCoroutine(ApplyCooldown((float)paramsContainer[0], (Spells)paramsContainer[1]));
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
    #endregion

    IEnumerator CleanText()
    {
        var w = new WaitForSeconds(1.5f);
        yield return w;
        damageText.text = "";
    }
}
