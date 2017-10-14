using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingLifeBar : MonoBehaviour
{
    private Transform _cam;
    private Image _fill;
    private string _name;

    void Start()
    {
        Initialize();
        EventManager.AddEventListener("LifeUpdate", OnLifeUpdate);
    }
    
    void LateUpdate()
    {
        this.GetComponent<RectTransform>().LookAt(_cam);
    }

    private void OnLifeUpdate(params object[] paramsContainer)
    {
        if (_name == (string)paramsContainer[0])
            _fill.fillAmount = (float)paramsContainer[2];
    }

    private void Initialize()
    {
        if(GameManager.screenDivided)
            _name = this.GetComponentInParent<Player1Input>().gameObject.name;
        else
            _name = this.GetComponentInParent<Enemy>().gameObject.name;

        _fill = this.transform.Find("Fill").GetComponent<Image>();

        if (_name == "Player1")
            _cam = GameObject.Find("CameraPlayer2").GetComponentInChildren<Camera>().transform;
        else if (_name == "Player2")
            _cam = GameObject.Find("CameraPlayer1").GetComponentInChildren<Camera>().transform;
        else
            _cam = GameObject.Find("CameraContainer").GetComponentInChildren<Camera>().transform;
    }
}
