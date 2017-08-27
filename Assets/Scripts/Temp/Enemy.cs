using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hp;
    public int maxHp;
    public float mana;
    public int maxMana;
    public float hpRegeneration;
    public float manaRegeneration;

    public float Hp
    {
        get { return hp; }
        set
        {
            if (value > maxHp) hp = maxHp;
            else if ((value < 0)) hp = 0;
            else hp = value;
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            if (value > maxMana) mana = maxMana;
            else if ((value < 0)) mana = 0;
            else mana = value;
        }
    }

    private void Start()
    {
        StartCoroutine(Regeneration());
    }

    public GameObject ParticleCaller(GameObject part, Vector3 pos)
    {
        var inst = Instantiate(part, pos, Quaternion.identity);
        Destroy(inst, 3);
        return inst;
    }

    public GameObject ParticleCaller(GameObject part, Transform parent)
    {
        var inst = Instantiate(part, parent);
        Destroy(inst, 3);
        return inst;
    }

    public ParticleSystem ParticleDestroyer(string name)
    {
        var p = transform.FindChild(name).GetComponent<ParticleSystem>();
        p.Stop();

        Destroy(p.gameObject, 3f);
        return p;
    }

    void ConsumeMana(ISpell spell)
    {
        Mana -= spell.GetManaCost();
        float fill = Mana / maxMana;
        EventManager.DispatchEvent("EnemyManaUpdate", new object[] { Mana, fill });
    }
    void RegainMana(float regained)
    {
        Mana += regained;
        float fill = Mana / maxMana;
        EventManager.DispatchEvent("EnemyManaUpdate", new object[] { Mana, fill });
    }
    void LoseHP(float damage)
    {
        Hp -= damage;
        float fill = Hp / maxHp;
        EventManager.DispatchEvent("EnemyLifeUpdate", new object[] { Hp, fill });
    }
    void RegainHp(float regained)
    {
        Hp += regained;
        float fill = Hp / maxHp;
        EventManager.DispatchEvent("EnemyLifeUpdate", new object[] { Hp, fill });
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        LoseHP(damage);
        if (PhotonNetwork.offlineMode)
        {
            EventManager.DispatchEvent("EnemyDamaged", new object[] { transform.position, this });
        }
        else EventManager.DispatchEvent("CharacterDamaged", new object[] { transform.position, this });
    }

    IEnumerator Regeneration()
    {
        float tick = 0.16f;
        float hpRegenByTick = hpRegeneration * tick;
        float manaRegenByTick = manaRegeneration * tick;
        while (true)
        {
            RegainHp(hpRegenByTick);
            RegainMana(manaRegenByTick);
            yield return new WaitForSeconds(tick);
        }
    }
}
