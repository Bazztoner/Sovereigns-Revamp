using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public Zone currentZone;
    public Image blackScreen;
    Tuple<LevelTransition, GameObject, GameObject> transitionElements;
    Tuple<GameObject, GameObject> dummies;

	void Start ()
    {
        EventManager.AddEventListener("TransitionActivated", OnTransitionActivation);
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
        EventManager.AddEventListener("DummyDamaged", OnDummyDamaged);
    }

    void OnCharacterDamaged(object[] paramsContainer)
    {
        var damagedName = (string)paramsContainer[0];

        var tuple = GetIfInTrigger(damagedName);

        if (tuple != null)
        {
            if (tuple.Item1 != default(LevelTransition) && tuple.Item2 != default(GameObject))
            {
                var transition = tuple.Item1;
                var victim = tuple.Item2;
                var attacker = tuple.Item2.GetComponent<PlayerMovement>().Enemy.gameObject;

                var tempAngle = Vector3.Angle(attacker.transform.forward, transition.transform.forward);
                if (tempAngle < 30f)
                {
                    EventManager.DispatchEvent("TransitionActivated", new object[] { transition, attacker, victim } );
                }
            }
        }
    }

    void OnDummyDamaged(object[] paramsContainer)
    {
        var damagedName = (string)paramsContainer[0];

        var tuple = GetIfInTrigger(damagedName);

        if (tuple != null)
        {
            if (tuple.Item1 != default(LevelTransition) && tuple.Item2 != default(GameObject))
            {
                var transition = tuple.Item1;
                var victim = tuple.Item2;
                var attacker = tuple.Item2.GetComponent<Enemy>().GetEnemy().gameObject;

                var tempAngle = Vector3.Angle(attacker.transform.forward, transition.transform.forward);
                if (tempAngle < 30f)
                {
                    EventManager.DispatchEvent("TransitionActivated", new object[] { transition, attacker, victim });
                }
            }
        }
    }

    Tuple<LevelTransition, GameObject> GetIfInTrigger(string playerName)
    {
        var posibleTransitions = currentZone.transitions;

        var transition = posibleTransitions
                        .Where(x => x.canBeUsed)
                        .Where(x => x.playersInTrigger.Any(g => g.name == playerName))
                        .FirstOrDefault();

        if (transition == default(LevelTransition)) return null;

        var player = transition.playersInTrigger.Where(g => g.name == playerName).FirstOrDefault();

        return new Tuple<LevelTransition, GameObject>(transition, player);
    }

    void OnTransitionActivation(object[] paramsContainer)
    {
        var transition = (LevelTransition)paramsContainer[0];
        var attacker = (GameObject)paramsContainer[1];
        var victim = (GameObject)paramsContainer[2];

        transitionElements = new Tuple<LevelTransition, GameObject, GameObject>(transition, attacker, victim);

        //StartCoroutine(TransitionWithLerping(transition, attacker, victim));
        StartCoroutine(TransitionWithAnimation(transition, attacker, victim));
    }

    #region Testing 3/9
    //Voy a tratar de ordenar las funciones en base a los pasos
    IEnumerator TransitionWithAnimation(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { false });
        var maxTimeForLerping = 1f;
        var timeQuota = .2f;

        StartCoroutine(
                       MoveObject(attacker.transform, attacker.transform.position, transition.attackerTransitionOrigin.position, timeQuota, maxTimeForLerping)
                      );
        StartCoroutine(
                       MoveObject(victim.transform, victim.transform.position, transition.victimTransitionOrigin.position, timeQuota, maxTimeForLerping)
                      );

        yield return new WaitForSeconds(maxTimeForLerping);

        EventManager.AddEventListener("ActivateTransitionDummies", OnTransitionDummiesActivated);

        victim.transform.position = transition.victimTransitionOrigin.position;
        attacker.transform.position = transition.attackerTransitionOrigin.position;

        EventManager.DispatchEvent("ActivateTransitionDummies", new object[] { transition, attacker, victim });
    }

    void OnTransitionDummiesActivated(object[] paramsContainer)
    {
        EventManager.RemoveEventListener("ActivateTransitionDummies", OnTransitionDummiesActivated);
        var transition = (LevelTransition)paramsContainer[0];
        var attacker = (GameObject)paramsContainer[1];
        var victim = (GameObject)paramsContainer[2];

        var dummyAttacker = GameObject.Instantiate(Resources.Load("Transitions/Dummies/TransitionPlayerDummy") as GameObject, attacker.transform.position, Quaternion.identity);
        dummyAttacker.transform.forward = attacker.transform.forward;
        attacker.SetActive(false);

        var dummyVictim = GameObject.Instantiate(Resources.Load("Transitions/Dummies/TransitionPlayerDummy") as GameObject, victim.transform.position, Quaternion.identity);
        dummyVictim.transform.forward = victim.transform.forward;
        victim.SetActive(false);

        dummies = new Tuple<GameObject, GameObject>(dummyAttacker, dummyVictim);

        transitionElements.Item1.camerasForTransition[0].gameObject.SetActive(true);
        //dummyAttacker.MakeAttack();
        float waitTime = .3f;
        StartCoroutine(AttackWaitTime(transition, dummyAttacker, dummyVictim, waitTime));
    }

    IEnumerator AttackWaitTime(LevelTransition transition, GameObject attacker, GameObject victim, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        float launchForce = 166f;
        OnTransitionLaunchDummy(transition, attacker, victim, launchForce);
    }

    void OnTransitionLaunchDummy(LevelTransition transition, GameObject attacker, GameObject victim, float launchForce)
    {
        var victimDummy = victim.GetComponent<TransitionDummy>();
        var victimRb = victim.GetComponent<Rigidbody>();
        victimRb.AddForce(attacker.transform.forward * launchForce, ForceMode.Impulse);
        victimDummy.isLaunched = true;
        EventManager.AddEventListener("DummyCollidedWithDestructible", OnDummyCollided);
    }

    void OnDummyCollided(object[] paramsContainer)
    {
        EventManager.RemoveEventListener("DummyCollidedWithDestructible", OnDummyCollided);
        var destructible = (DestructibleObject)paramsContainer[0];
        destructible.DestroyObject();
        float attackerRunDelay = .1f;
        float cameraDelay = .2f;
        StartCoroutine(DummyCollisionWaitTime(attackerRunDelay, cameraDelay));
        //TODO: Agregar partícula de sangre a victim

    }

    IEnumerator DummyCollisionWaitTime(float attackerRunDelay, float cameraDelay)
    {
        yield return new WaitForSeconds(attackerRunDelay);
        var maxTimeForLerping = 1f;
        var timeQuota = 1f;
        StartCoroutine(
                       MoveObject(transitionElements.Item2.transform,
                                  transitionElements.Item2.transform.position, 
                                  transitionElements.Item1.attackerRelocatePoint.position, 
                                  timeQuota, maxTimeForLerping)
                      );
        yield return new WaitForSeconds(cameraDelay);

        transitionElements.Item1.camerasForTransition[1].gameObject.SetActive(true);
        transitionElements.Item1.camerasForTransition[0].gameObject.SetActive(false);
        yield return new WaitForSeconds(maxTimeForLerping);

        OnEndOfTransition(transitionElements.Item1, transitionElements.Item2, transitionElements.Item3);
    }

    void OnEndOfTransition(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        victim.transform.position = transition.victimRelocatePoint.position;
        attacker.transform.position = transition.attackerRelocatePoint.position;
        dummies.Item1.SetActive(false);
        dummies.Item2.SetActive(false);
        transitionElements.Item2.SetActive(true);
        transitionElements.Item3.SetActive(true);
        transitionElements.Item1.camerasForTransition[1].gameObject.SetActive(false);
        transitionElements.Item1.camerasForTransition[0].gameObject.SetActive(false);
        //FIXME: no es lo ideal
        //if (GameManager.screenDivided) victim.GetComponentInParent<PlayerStats>().TakeDamage(transition.damage);

        currentZone = transition.to;
        transition.canBeUsed = false;
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { true });
    }
    #endregion


    IEnumerator TransitionWithLerping(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { false });
        var maxTimeForLerping = 1f;
        var timeQuota = .2f;

        StartCoroutine(
                       MoveObject(attacker.transform, attacker.transform.position, transition.attackerTransitionOrigin.position, timeQuota, maxTimeForLerping)
                      );
        StartCoroutine(
                       MoveObject(victim.transform, victim.transform.position, transition.victimTransitionOrigin.position, timeQuota, maxTimeForLerping)
                      );

        yield return new WaitForSeconds(maxTimeForLerping);

        OnLerpingTransition(transition, attacker, victim);

        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { true });
    }

    void OnLerpingTransition(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        victim.transform.position = transition.victimRelocatePoint.position;
        //FIXME: no es lo ideal
        if (GameManager.screenDivided) victim.GetComponentInParent<PlayerStats>().TakeDamage(transition.damage);

        attacker.transform.position = transition.attackerRelocatePoint.position;

        currentZone = transition.to;
        transition.canBeUsed = false;
    }

    IEnumerator MoveObject(Transform objToMove, Vector3 startPos,Vector3 endPos, float timeQuota, float maxTime)
    {
        var i = 0f;
        float rate = 1f / timeQuota;
        while (i < maxTime)
        {
            i += Time.deltaTime * rate;
            objToMove.position = Vector3.Lerp(startPos, endPos, i);
            yield return new WaitForEndOfFrame(); 
        }
    }


    #region Deprecated
    void OnBlackScreenTransition(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        //Attacked Player
        victim.transform.position = transition.victimRelocatePoint.position;
        //FIXME: no es lo ideal
        if (GameManager.screenDivided) victim.GetComponentInParent<PlayerStats>().TakeDamage(transition.damage);

        //Attacker Player
        attacker.transform.position = transition.attackerRelocatePoint.position;

        currentZone = transition.to;
        transition.canBeUsed = false;
    }

    IEnumerator TransitionWithBlackScreen(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        ActivateBlackScreen(true);
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { false });
        yield return new WaitForSeconds(0.5f);

        OnBlackScreenTransition(transition, attacker, victim);

        yield return new WaitForSeconds(0.5f);
        ActivateBlackScreen(false);
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { true });
    }

    void ActivateBlackScreen(GameObject attacker, GameObject victim, bool activation)
    {
        var victimCamera = victim.GetComponent<Player1Input>().GetCamera.GetCamera;

        var attackerCamera = attacker.GetComponent<Player1Input>().GetCamera.GetCamera;

        victimCamera.enabled = activation;
        attackerCamera.enabled = activation;
    }

    void ActivateBlackScreen(bool activation)
    {
        blackScreen.enabled = activation;
    }
    #endregion
}
