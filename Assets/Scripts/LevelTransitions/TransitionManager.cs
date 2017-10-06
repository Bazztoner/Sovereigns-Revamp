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
    /// <summary>
    /// 1 - LevelTransition - Transición ||
    /// 2 - GameObject - Atacante ||
    /// 3 - GameObject - Víctima
    /// </summary>
    Tuple<LevelTransition, GameObject, GameObject> transitionElements;
    /// <summary>
    /// 1 - GameObject - Atacante ||
    /// 2 - GameObject - Víctima
    /// </summary>
    Tuple<GameObject, GameObject> dummies;

    public static TransitionManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    /// <summary>
    /// IUS FOR INISIALISEISHON
    /// </summary>
	void Start ()
    {
        EventManager.AddEventListener("TransitionActivated", OnTransitionActivation);
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
        EventManager.AddEventListener("DummyDamaged", OnDummyDamaged);
    }

    /// <summary>
    /// Handler se activa cuando se daña un jugador
    /// </summary>
    /// <param name="paramsContainer">
    /// 0 - Name ||
    /// 1 - Transform ||
    /// 2 - PlayerParticles ||
    /// 3 - AttackType (string)
    /// </param>
    void OnCharacterDamaged(object[] paramsContainer)
    {
        var attackType = (string)paramsContainer[3];
        var attackTypeExists = attackType != null && attackType != default(string);
        if (attackTypeExists && (attackType == "Melee" || attackType == "MeleeHorizontal" || attackType == "MeleeVertical"))
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
                        EventManager.DispatchEvent("TransitionActivated", new object[] { transition, attacker, victim });
                        EventManager.RemoveEventListener("CharacterDamaged", OnCharacterDamaged);
                        EventManager.RemoveEventListener("DummyDamaged", OnDummyDamaged);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handler cuando se daña un dummy
    /// </summary>
    /// <param name="paramsContainer">
    /// 0 - Name ||
    /// 1 - Transform ||
    /// 2 - AttackType (string)
    /// </param>
    void OnDummyDamaged(object[] paramsContainer)
    {
        var attackType = (string)paramsContainer[2];
        var attackTypeExists = attackType != null && attackType != default(string);
        if (attackTypeExists && (attackType == "Melee" || attackType == "MeleeHorizontal" || attackType == "MeleeVertical"))
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
                        EventManager.RemoveEventListener("CharacterDamaged", OnCharacterDamaged);
                        EventManager.RemoveEventListener("DummyDamaged", OnDummyDamaged);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Retorna una tupla con la Transición en la que está el jugador con el nombre parametrizado y el gameobject del mismo
    /// </summary>
    /// <param name="playerName"> Nombre del gameobject del jugador </param>
    Tuple<LevelTransition, GameObject> GetIfInTrigger(string playerName)
    {
        var posibleTransitions = currentZone.transitions;

        //Retorna LevelTransition si encuentra un gameObject con el nombre del parámetro, sino default
        var transition = posibleTransitions
                        .Where(x => x.canBeUsed)
                        .Where(x => x.playersInTrigger.Any(g => g.name == playerName))
                        .FirstOrDefault();

        if (transition == default(LevelTransition)) return null;

        var player = transition.playersInTrigger.Where(g => g.name == playerName).FirstOrDefault();

        return new Tuple<LevelTransition, GameObject>(transition, player);
    }

    //Ordenadas por pasos

    /// <summary>
    /// Handler cuando se dispara la transición
    /// </summary>
    /// <param name="paramsContainer">
    /// 0 - LevelTransition - Transición ||
    /// 1 - GameObject - Atacante ||
    /// 2 - GameObject - Víctima
    /// </param>
    void OnTransitionActivation(object[] paramsContainer)
    {
        var transition = (LevelTransition)paramsContainer[0];
        var attacker = (GameObject)paramsContainer[1];
        var victim = (GameObject)paramsContainer[2];

        transitionElements = new Tuple<LevelTransition, GameObject, GameObject>(transition, attacker, victim);

        StartCoroutine(TransitionWithAnimation(transition, attacker, victim));
    }

    /// <summary>
    /// Corrutina para aplicar el Lerp hacia los puntos de origen, además de quitar el input a los jugadores
    /// </summary>
    /// <param name="transition"></param>
    /// <param name="attacker"></param>
    /// <param name="victim"></param>
    /// <returns></returns>
    IEnumerator TransitionWithAnimation(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { false });
        var maxTimeForLerping = 1f;

        StartCoroutine(
                       LerpPosition(attacker.transform, attacker.transform.position, transition.attackerTransitionOrigin.position, maxTimeForLerping)
                      );
        StartCoroutine(
                       LerpPosition(victim.transform, victim.transform.position, transition.victimTransitionOrigin.position, maxTimeForLerping)
                      );
        
        yield return new WaitForSeconds(0);
        
        victim.transform.position = transition.victimTransitionOrigin.position;
        attacker.transform.position = transition.attackerTransitionOrigin.position;

        OnTransitionDummiesActivated(transition, attacker, victim);
    }

    /// <summary>
    /// Activa los dummies, desactiva a los jugadores
    /// </summary>
    /// <param name="paramsContainer"></param>
    void OnTransitionDummiesActivated(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        EventManager.DispatchEvent("TransitionSmoothCameraUpdate", new object[] { false });
        EventManager.DispatchEvent("TransitionCameraUpdate", true);

        var dummyAttacker = GameObject.Instantiate(Resources.Load("Transitions/Dummies/TransitionPlayerDummy") as GameObject, attacker.transform.position, Quaternion.identity);
        dummyAttacker.transform.forward = attacker.transform.forward;
        attacker.SetActive(false);

        var dummyVictim = GameObject.Instantiate(Resources.Load("Transitions/Dummies/TransitionPlayerDummy") as GameObject, victim.transform.position, Quaternion.identity);
        dummyVictim.transform.forward = victim.transform.forward;
        victim.SetActive(false);

        dummies = new Tuple<GameObject, GameObject>(dummyAttacker, dummyVictim);

        //Activamos la cámara que apunta al atacante
        transitionElements.Item1.camerasForTransition[0].gameObject.SetActive(true);
        dummyVictim.GetComponent<TransitionDummy>().Animate("transitionDamage");
        OnTransitionLaunchDummy(transition, dummyAttacker, dummyVictim, 0.5f);
    }

    /// <summary>
    /// Mueve con Lerp hacia la posicion final. Además, agrega un evento para cuando este colisione contra el destructible.
    /// </summary>
    /// <param name="transition"></param>
    /// <param name="attacker"></param>
    /// <param name="victim"></param>
    /// <param name="maxTime"></param>
    void OnTransitionLaunchDummy(LevelTransition transition, GameObject attacker, GameObject victim, float maxTime)
    {
        var victimDummy = victim.GetComponent<TransitionDummy>();
        StartCoroutine(
                       LerpPosition(victim.transform, victim.transform.position, transition.victimRelocatePoint.position, maxTime)
                      );
        victimDummy.isLaunched = true;
        EventManager.AddEventListener("DummyCollidedWithDestructible", OnDummyCollided);
    }

    /// <summary>
    /// Handler para cuando colisiona el dummy víctima con el destructible
    /// </summary>
    /// <param name="paramsContainer"></param>
    void OnDummyCollided(object[] paramsContainer)
    {
        EventManager.RemoveEventListener("DummyCollidedWithDestructible", OnDummyCollided);
        var destructible = (DestructibleObject)paramsContainer[0];
        destructible.DestroyObject(transitionElements.Item1.transform.forward, 90f);
        float attackerRunDelay = .1f;
        float cameraDelay = .2f;
        StartCoroutine(DummyCollisionWaitTime(attackerRunDelay, cameraDelay));
    }

    /// <summary>
    /// Corrutina para que el atacante espere un poco luego de que el dummy víctima atraviese el destructible. El atacante correrá y la cámara se cambiará
    /// </summary>
    /// <param name="attackerRunDelay"></param>
    /// <param name="cameraDelay"></param>
    /// <returns></returns>
    IEnumerator DummyCollisionWaitTime(float attackerRunDelay, float cameraDelay)
    {
        yield return new WaitForSeconds(attackerRunDelay);
        var maxTimeForLerpingPosition = .6f;
        var maxTimeForLerpingForward = .2f;

        StartCoroutine(
                       LerpPosition(transitionElements.Item2.transform,
                                   transitionElements.Item2.transform.position, 
                                   transitionElements.Item1.attackerRelocatePoint.position, 
                                   maxTimeForLerpingPosition)
                      );

        StartCoroutine(
                       LerpForward(transitionElements.Item3.transform,
                                   transitionElements.Item3.transform.forward,
                                   -transitionElements.Item1.transform.forward,
                                   maxTimeForLerpingForward)
                      );

        yield return new WaitForSeconds(cameraDelay);

        //Cambiamos las cámaras
        StartCoroutine(
                        LerpPosition(transitionElements.Item1.camerasForTransition[0].transform,
                                    transitionElements.Item1.camerasForTransition[0].transform.position,
                                    transitionElements.Item1.camerasForTransition[1].transform.position,
                                    maxTimeForLerpingPosition)
                       );

        yield return new WaitForSeconds(maxTimeForLerpingPosition/2);

        StartCoroutine(
                       LerpForward(transitionElements.Item1.camerasForTransition[0].transform,
                                   transitionElements.Item1.camerasForTransition[0].transform.forward,
                                   transitionElements.Item1.camerasForTransition[1].transform.forward,
                                   maxTimeForLerpingForward)
                      );

        yield return new WaitForSeconds(maxTimeForLerpingPosition/2);

        transitionElements.Item1.camerasForTransition[1].gameObject.SetActive(true);

        transitionElements.Item1.camerasForTransition[0].gameObject.SetActive(false);

        OnEndOfTransition(transitionElements.Item1, transitionElements.Item2, transitionElements.Item3);
    }

    /// <summary>
    /// Finalizamos la transición: apagamos dummies, prendemos jugadores, devolvemos el input a los jugadores y apagamos las cámaras.
    /// También cancelamos la transición que usamos y la que va para el lado contrario
    /// </summary>
    /// <param name="transition"></param>
    /// <param name="attacker"></param>
    /// <param name="victim"></param>
    void OnEndOfTransition(LevelTransition transition, GameObject attacker, GameObject victim)
    {
        victim.transform.position = transition.victimRelocatePoint.position;
        victim.transform.forward = -transition.transform.forward;

        attacker.transform.position = transition.attackerRelocatePoint.position;
        attacker.transform.forward = transition.transform.forward;

        dummies.Item1.SetActive(false);
        dummies.Item2.SetActive(false);

        transitionElements.Item2.SetActive(true);
        transitionElements.Item3.SetActive(true);
        var camAttacker = transitionElements.Item2.GetComponentInParent<Player1Input>().GetCamera;
        camAttacker.transform.forward = transitionElements.Item1.transform.forward;

        if (GameManager.screenDivided)
        {
            var camVictim = transitionElements.Item3.GetComponentInParent<Player1Input>().GetCamera;
            camVictim.transform.forward = -transitionElements.Item1.transform.forward;
        }

        EventManager.DispatchEvent("TransitionCameraUpdate", false);

        transitionElements.Item1.camerasForTransition[1].gameObject.SetActive(false);
        transitionElements.Item1.camerasForTransition[0].gameObject.SetActive(false);
        //FIXME: no es lo ideal - Hacer que haga daño sin partícula?
        //if (GameManager.screenDivided) victim.GetComponentInParent<PlayerStats>().TakeDamage(transition.damage, "Transition");

        var previousZone = currentZone;
        currentZone = transition.to;
        transition.canBeUsed = false;
        transition.otherSide.canBeUsed = false;
       
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
        EventManager.AddEventListener("DummyDamaged", OnDummyDamaged);

        //StartCoroutine(BlackScreenUpdate(.5f));
        Invoke("CameraSmoothChange", .2f);
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { true });
    }

    IEnumerator BlackScreenUpdate(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);

        var color = blackScreen.color;
        var colorTransparent = blackScreen.color;
        var fadeTime = .3f;
        colorTransparent.a = 0;

        StartCoroutine(LerpBlackScreen(blackScreen.color, blackScreen.color, colorTransparent, fadeTime));
        yield return new WaitForSeconds(fadeTime);
        blackScreen.enabled = false;
        Invoke("CameraSmoothChange", .2f);
        EventManager.DispatchEvent("TransitionBlockInputs", new object[] { true });
    }

    void CameraSmoothChange()
    {
        EventManager.DispatchEvent("TransitionSmoothCameraUpdate", new object[] { true });
    }

    IEnumerator LerpPosition(Transform objToMove, Vector3 startPos, Vector3 endPos, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            objToMove.position = Vector3.Lerp(startPos, endPos, i);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LerpForward(Transform objToMove, Vector3 startPos, Vector3 endPos, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            objToMove.forward = Vector3.Lerp(startPos, endPos, i);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LerpFloat(float value, float startValue, float endValue, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            value = Mathf.Lerp(startValue, endValue, i);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LerpBlackScreen(Color color, Color startValue, Color endValue, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            color = Color.Lerp(startValue, endValue, i);
            blackScreen.color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
