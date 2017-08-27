using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(InputManager))]
public class InputEditor : Editor {

    private string[] _options = new string[] { "Keyboard and Mouse", "Joystick" };
    private string[] _keyCodes = Enum.GetNames(typeof(KeyCode));
    private Dictionary<string, int> _keybindings = new Dictionary<string, int>();
    private int[] _selections = new int[18];
    private int _selected = 0;
    private int _lastSelected = 1;

    private void OnEnable()
    {
        int count = 0;

        foreach (var key in _keyCodes)
        {
            _keybindings[key] = count;
            count++;
        }
    }

    public override void OnInspectorGUI()
    {
        InputManager IM = (InputManager)target;

        _selected = EditorGUILayout.Popup("Device", _selected, _options);

        if (_selected != _lastSelected)
        {
            SetSelections(ref IM, _selected);
            _lastSelected = _selected;
        }
        GUILayout.BeginVertical();
        if (_selected == 0)
        {   
            _selections[0] = EditorGUILayout.Popup("Forward", _selections[0], _keyCodes);
            _selections[1] = EditorGUILayout.Popup("Backward", _selections[1], _keyCodes);
            _selections[2] = EditorGUILayout.Popup("Left", _selections[2], _keyCodes);
            _selections[3] = EditorGUILayout.Popup("Right", _selections[3], _keyCodes);
            _selections[4] = EditorGUILayout.Popup("Light Attack", _selections[4], _keyCodes);
            _selections[5] = EditorGUILayout.Popup("Heavy Attack", _selections[5], _keyCodes);
            _selections[6] = EditorGUILayout.Popup("Blocking", _selections[6], _keyCodes);
            _selections[7] = EditorGUILayout.Popup("Dodge / Run", _selections[7], _keyCodes);
            _selections[8] = EditorGUILayout.Popup("Use Item", _selections[8], _keyCodes);
            _selections[9] = EditorGUILayout.Popup("Class Skill", _selections[9], _keyCodes);
            _selections[10] = EditorGUILayout.Popup("Enviroment Skill", _selections[10], _keyCodes);
            _selections[11] = EditorGUILayout.Popup("Skill", _selections[11], _keyCodes);
            _selections[12] = EditorGUILayout.Popup("Lock On", _selections[12], _keyCodes);
            _selections[13] = EditorGUILayout.Popup("Enviroment Action", _selections[13], _keyCodes);
            _selections[14] = EditorGUILayout.Popup("Item 1", _selections[14], _keyCodes);
            _selections[15] = EditorGUILayout.Popup("Item 2", _selections[15], _keyCodes);
            _selections[16] = EditorGUILayout.Popup("Item 3", _selections[16], _keyCodes);
            _selections[17] = EditorGUILayout.Popup("Item 4", _selections[17], _keyCodes);
        }
        else
        {
            _selections[0] = EditorGUILayout.Popup("Light Attack", _selections[0], _keyCodes);
            _selections[1] = EditorGUILayout.Popup("Heavy Attack", _selections[1], _keyCodes);
            _selections[2] = EditorGUILayout.Popup("Dodge / Run", _selections[2], _keyCodes);
            _selections[3] = EditorGUILayout.Popup("Use Item", _selections[3], _keyCodes);
            _selections[4] = EditorGUILayout.Popup("Class Skill", _selections[4], _keyCodes);
            _selections[5] = EditorGUILayout.Popup("Enviroment Skill", _selections[5], _keyCodes);
            _selections[6] = EditorGUILayout.Popup("Skill", _selections[6], _keyCodes);
            _selections[7] = EditorGUILayout.Popup("Lock On", _selections[7], _keyCodes);
            _selections[8] = EditorGUILayout.Popup("Enviroment Action", _selections[8], _keyCodes);
        }
        GUILayout.EndVertical();

        if (GUILayout.Button("Apply"))UpdateValues(ref IM, _selected);
    }

    private void SetSelections(ref InputManager im, int ind)
    {
        if (ind == 0)
        {
            _selections[0] = _keybindings[im.forward.ToString()];
            _selections[1] = _keybindings[im.backward.ToString()];
            _selections[2] = _keybindings[im.left.ToString()];
            _selections[3] = _keybindings[im.right.ToString()];
            _selections[4] = _keybindings[im.lightAttack.ToString()];
            _selections[5] = _keybindings[im.heavyAttack.ToString()];
            _selections[6] = _keybindings[im.blocking.ToString()];
            _selections[7] = _keybindings[im.dodge_run.ToString()];
            _selections[8] = _keybindings[im.useObject.ToString()];
            _selections[9] = _keybindings[im.classSkill.ToString()];
            _selections[10] = _keybindings[im.enviromentSkill.ToString()];
            _selections[11] = _keybindings[im.skill.ToString()];
            _selections[12] = _keybindings[im.lockOn.ToString()];
            _selections[13] = _keybindings[im.enviromentAction.ToString()];
            _selections[14] = _keybindings[im.item1.ToString()];
            _selections[15] = _keybindings[im.item2.ToString()];
            _selections[16] = _keybindings[im.item3.ToString()];
            _selections[17] = _keybindings[im.item4.ToString()];
        }
        else
        {
            _selections[0] = _keybindings[im.lightAttackGamepad.ToString()];
            _selections[1] = _keybindings[im.heavyAttackGamepad.ToString()];
            _selections[2] = _keybindings[im.dodge_runGamepad.ToString()];
            _selections[3] = _keybindings[im.useObjectGamepad.ToString()];
            _selections[4] = _keybindings[im.classSkillGamepad.ToString()];
            _selections[5] = _keybindings[im.enviromentSkillGamepad.ToString()];
            _selections[6] = _keybindings[im.skillGamepad.ToString()];
            _selections[7] = _keybindings[im.lockOnGamepad.ToString()];
            _selections[8] = _keybindings[im.enviromentActionGamepad.ToString()];
        }
    }

    private void UpdateValues(ref InputManager im, int ind)
    {
        if (ind == 0)
        {
            if (im.forward.ToString() != _keyCodes[_selections[0]]) im.forward = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[0]]);
            if (im.backward.ToString() != _keyCodes[_selections[1]]) im.backward = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[1]]);
            if (im.left.ToString() != _keyCodes[_selections[2]]) im.left = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[2]]);
            if (im.right.ToString() != _keyCodes[_selections[3]]) im.right = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[3]]);
            if (im.lightAttack.ToString() != _keyCodes[_selections[4]]) im.lightAttack = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[4]]);
            if (im.heavyAttack.ToString() != _keyCodes[_selections[5]]) im.heavyAttack = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[5]]);
            if (im.blocking.ToString() != _keyCodes[_selections[6]]) im.blocking = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[6]]);
            if (im.dodge_run.ToString() != _keyCodes[_selections[7]]) im.dodge_run = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[7]]);
            if (im.useObject.ToString() != _keyCodes[_selections[8]]) im.useObject = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[8]]);
            if (im.classSkill.ToString() != _keyCodes[_selections[9]]) im.classSkill = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[9]]);
            if (im.enviromentSkill.ToString() != _keyCodes[_selections[10]]) im.enviromentSkill = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[10]]);
            if (im.skill.ToString() != _keyCodes[_selections[11]]) im.skill = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[11]]);
            if (im.lockOn.ToString() != _keyCodes[_selections[12]]) im.lockOn = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[12]]);
            if (im.enviromentAction.ToString() != _keyCodes[_selections[13]]) im.enviromentAction = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[13]]);
            if (im.item1.ToString() != _keyCodes[_selections[14]]) im.item1 = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[14]]);
            if (im.item2.ToString() != _keyCodes[_selections[15]]) im.item2 = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[15]]);
            if (im.item3.ToString() != _keyCodes[_selections[16]]) im.item3 = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[16]]);
            if (im.item4.ToString() != _keyCodes[_selections[17]]) im.item4 = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[17]]);
        }
        else
        {
            if (im.lightAttackGamepad.ToString() != _keyCodes[_selections[0]]) im.lightAttackGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[0]]);
            if (im.heavyAttackGamepad.ToString() != _keyCodes[_selections[1]]) im.heavyAttackGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[1]]);
            if (im.dodge_runGamepad.ToString() != _keyCodes[_selections[2]]) im.dodge_runGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[2]]);
            if (im.useObjectGamepad.ToString() != _keyCodes[_selections[3]]) im.useObjectGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[3]]);
            if (im.classSkillGamepad.ToString() != _keyCodes[_selections[4]]) im.classSkillGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[4]]);
            if (im.enviromentSkillGamepad.ToString() != _keyCodes[_selections[5]]) im.enviromentSkillGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[5]]);
            if (im.skillGamepad.ToString() != _keyCodes[_selections[6]]) im.skillGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[6]]);
            if (im.lockOnGamepad.ToString() != _keyCodes[_selections[7]]) im.lockOnGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[7]]);
            if (im.enviromentActionGamepad.ToString() != _keyCodes[_selections[8]]) im.enviromentActionGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[8]]);
        }
    }
}
