using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DestructAnimCont : MonoBehaviour
{
    Animation _an;
    bool hasFinishedAnimation;

    void Start()
    {
        _an = GetComponent<Animation>();
        var activateObjects = transform.GetComponentsInChildren<TelekineticObject>();

        foreach (var obj in activateObjects)
        {
            obj.ActivateFromDestruction();
        }
    }

    void Update()
    {
        if (!_an.isPlaying && !hasFinishedAnimation) OnDestructionEnd();
    }

    void OnDestructionEnd()
    {
        var boundingBox = GetComponentInChildren<DestructibleBoundingBox>();
        if (boundingBox != null)
        {
            boundingBox.gameObject.SetActive(false);
        }
        
        var activateObjects = transform.GetComponentsInChildren<TelekineticObject>();

        foreach (var obj in activateObjects)
        {
            var objTrns = obj.transform.position;
            obj.transform.SetParent(GameObject.Find("TelekinesisObjects").transform);
            obj.transform.localPosition = transform.InverseTransformPoint(objTrns);
            obj.transform.position = objTrns;
            obj.DeactivateFromDestruction();
        }

        hasFinishedAnimation = true;
        StartCoroutine(CleanDebris());
    }

    IEnumerator CleanDebris()
    {
        var debrisList = transform.GetComponentsInChildren<Transform>()
                         .Where(x => x.gameObject != this.gameObject)
                         .Where(x => x.GetComponent<TelekineticObject>() == null)
                         .Where(x => x.GetComponent<Debris>() == null);

        if (!debrisList.Any()) yield break;

        var cnt = Mathf.FloorToInt(debrisList.Count());

        while (debrisList.Any())
        {
            yield return new WaitForSeconds(10f);
            var rnd = Random.Range(1, cnt/3);
            var cleanNow = debrisList.Take(rnd);

            foreach (Transform t in cleanNow)
            {
                t.gameObject.SetActive(false);
            }

            debrisList = debrisList.Where(x => x.gameObject.activeInHierarchy);
        }
        
    }
}
