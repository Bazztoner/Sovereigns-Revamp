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
                print("Transition: " + tuple.Item1 + " || " + "Attacked: " + tuple.Item2.name);
                var trans = tuple.Item1;
                var attacked = tuple.Item2;
                var attacker = tuple.Item2.GetComponent<PlayerMovement>().Enemy.gameObject;

                var tempAngle = Vector3.Angle(attacker.transform.forward, trans.transform.forward);
                if (tempAngle < 30f)
                {
                    EventManager.DispatchEvent("TransitionActivated", new object[] { trans, attacker, attacked } );
                }
            }
        }
    }

    void OnDummyDamaged(object[] paramsContainer)
    {
        print("Enter OnDummyDamaged");
        var damagedName = (string)paramsContainer[0];

        var tuple = GetIfInTrigger(damagedName);

        if (tuple != null)
        {
            print("Entered tuple != null");
            if (tuple.Item1 != default(LevelTransition) && tuple.Item2 != default(GameObject))
            {
                print("Transition: " + tuple.Item1 + " || " + "Attacked: " + tuple.Item2.name);
                var trans = tuple.Item1;
                var attacked = tuple.Item2;
                var attacker = tuple.Item2.GetComponent<Enemy>().GetEnemy().gameObject;

                var tempAngle = Vector3.Angle(attacker.transform.forward, trans.transform.forward);
                if (tempAngle < 30f)
                {
                    EventManager.DispatchEvent("TransitionActivated", new object[] { trans, attacker, attacked });
                }
            }
        }
    }


    Tuple<LevelTransition, GameObject> GetIfInTrigger(string playerName)
    {
        var posibleTransitions = currentZone.transitions;

        #region Deprecated
        /*var transition = posibleTransitions
                        .Where(x => x.playersInTrigger.Any())
                        .TakeWhile(x => x.playersInTrigger.Any(g => g.name == playerName))
                        .FirstOrDefault();*/
        #endregion

        var transition = posibleTransitions
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
        var attacked = (GameObject)paramsContainer[2];

        StartCoroutine(Transition(transition, attacker, attacked));
    }

    void OnTransition(LevelTransition transition, GameObject attacker, GameObject attacked)
    {
        //Attacked Player
        attacked.transform.position = transition.attackedRelocatePoint.position;
        //FIXME: no es lo ideal
        if (GameManager.screenDivided) attacked.GetComponentInParent<PlayerStats>().TakeDamage(transition.damage);

        //Attacker Player
        attacker.transform.position = transition.attackerRelocatePoint.position;

        currentZone = transition.to;
    }

    IEnumerator Transition(LevelTransition transition, GameObject attacker, GameObject attacked)
    {
        ActivateBlackScreen(true);
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { false } );
        yield return new WaitForSeconds(0.5f);

        OnTransition(transition, attacker, attacked);

        yield return new WaitForSeconds(0.5f);
        ActivateBlackScreen(false);
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { true });
    }

    void ActivateBlackScreen(GameObject attacker, GameObject attacked, bool activation)
    {
        var attackedCamera = attacked.GetComponent<Player1Input>().GetCamera.GetCamera;

        var attackerCamera = attacker.GetComponent<Player1Input>().GetCamera.GetCamera;

        attackedCamera.enabled = activation;
        attackerCamera.enabled = activation;
    }

    void ActivateBlackScreen(bool activation)
    {
        blackScreen.enabled = activation;
    }
}
