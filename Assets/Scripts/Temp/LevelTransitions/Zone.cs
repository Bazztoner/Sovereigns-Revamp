using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public List<LevelTransition> transitions;

    void Start()
    {
        transitions = GetComponentsInChildren<LevelTransition>().Where(x => x.transform.parent.name == "Transitions").ToList();
    }
}
