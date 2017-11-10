using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(InputManager))]
public class InputEditor : Editor
{
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
            _selections[1] = EditorGUILayout.Popup("Backwards", _selections[1], _keyCodes);
            _selections[2] = EditorGUILayout.Popup("Left", _selections[2], _keyCodes);
            _selections[3] = EditorGUILayout.Popup("Right", _selections[3], _keyCodes);
            _selections[4] = EditorGUILayout.Popup("Horizontal Attack", _selections[4], _keyCodes);
            _selections[5] = EditorGUILayout.Popup("Vertical Attack", _selections[5], _keyCodes);
            _selections[6] = EditorGUILayout.Popup("Block", _selections[6], _keyCodes);
            _selections[7] = EditorGUILayout.Popup("Dodge / Run", _selections[7], _keyCodes);
            _selections[8] = EditorGUILayout.Popup("Movement Skill", _selections[8], _keyCodes);
            _selections[9] = EditorGUILayout.Popup("Class Skill", _selections[9], _keyCodes);
            _selections[10] = EditorGUILayout.Popup("Environmental Skill", _selections[10], _keyCodes);
            _selections[11] = EditorGUILayout.Popup("Universal Skill", _selections[11], _keyCodes);
            _selections[12] = EditorGUILayout.Popup("Lock On", _selections[12], _keyCodes);
            _selections[13] = EditorGUILayout.Popup("Use Skill", _selections[13], _keyCodes);
        }
        else
        {
            _selections[0] = EditorGUILayout.Popup("Horizontal Attack", _selections[0], _keyCodes);
            _selections[1] = EditorGUILayout.Popup("Vertical Attack", _selections[1], _keyCodes);
            _selections[2] = EditorGUILayout.Popup("Dodge / Run", _selections[2], _keyCodes);
            _selections[3] = EditorGUILayout.Popup("Movement Skill", _selections[3], _keyCodes);
            _selections[4] = EditorGUILayout.Popup("Lock On", _selections[4], _keyCodes);
            _selections[5] = EditorGUILayout.Popup("Use Skill", _selections[5], _keyCodes);
        }
        GUILayout.EndVertical();

        if (GUILayout.Button("Apply")) UpdateValues(ref IM, _selected);
    }

    private void SetSelections(ref InputManager im, int ind)
    {
        if (ind == 0)
        {
            _selections[0] = _keybindings[im.forward.ToString()];
            _selections[1] = _keybindings[im.backwards.ToString()];
            _selections[2] = _keybindings[im.left.ToString()];
            _selections[3] = _keybindings[im.right.ToString()];
            _selections[4] = _keybindings[im.horizontalAttack.ToString()];
            _selections[5] = _keybindings[im.verticalAttack.ToString()];
            _selections[6] = _keybindings[im.block.ToString()];
            _selections[7] = _keybindings[im.dodge_run.ToString()];
            _selections[8] = _keybindings[im.movementSkill.ToString()];
            _selections[9] = _keybindings[im.classSkill.ToString()];
            _selections[10] = _keybindings[im.enviromentalSkill.ToString()];
            _selections[11] = _keybindings[im.universalSkill.ToString()];
            _selections[12] = _keybindings[im.lockOn.ToString()];
            _selections[13] = _keybindings[im.useSkill.ToString()];
        }
        else
        {
            _selections[0] = _keybindings[im.horizontalAttackGamepad.ToString()];
            _selections[1] = _keybindings[im.verticalAttackGamepad.ToString()];
            _selections[2] = _keybindings[im.dodge_runGamepad.ToString()];
            _selections[3] = _keybindings[im.movementSkillGamepad.ToString()];
            _selections[4] = _keybindings[im.lockOnGamepad.ToString()];
            _selections[5] = _keybindings[im.useSkillGamepad.ToString()];
        }
    }

    private void UpdateValues(ref InputManager im, int ind)
    {
        if (ind == 0)
        {
            if (im.forward.ToString() != _keyCodes[_selections[0]]) im.forward = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[0]]);
            if (im.backwards.ToString() != _keyCodes[_selections[1]]) im.backwards = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[1]]);
            if (im.left.ToString() != _keyCodes[_selections[2]]) im.left = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[2]]);
            if (im.right.ToString() != _keyCodes[_selections[3]]) im.right = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[3]]);
            if (im.horizontalAttack.ToString() != _keyCodes[_selections[4]]) im.horizontalAttack = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[4]]);
            if (im.verticalAttack.ToString() != _keyCodes[_selections[5]]) im.verticalAttack = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[5]]);
            if (im.block.ToString() != _keyCodes[_selections[6]]) im.block = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[6]]);
            if (im.dodge_run.ToString() != _keyCodes[_selections[7]]) im.dodge_run = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[7]]);
            if (im.movementSkill.ToString() != _keyCodes[_selections[8]]) im.movementSkill = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[8]]);
            if (im.classSkill.ToString() != _keyCodes[_selections[9]]) im.classSkill = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[9]]);
            if (im.enviromentalSkill.ToString() != _keyCodes[_selections[10]]) im.enviromentalSkill = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[10]]);
            if (im.universalSkill.ToString() != _keyCodes[_selections[11]]) im.universalSkill = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[11]]);
            if (im.lockOn.ToString() != _keyCodes[_selections[12]]) im.lockOn = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[12]]);
            if (im.useSkill.ToString() != _keyCodes[_selections[13]]) im.useSkill = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[13]]);
        }
        else
        {
            if (im.horizontalAttackGamepad.ToString() != _keyCodes[_selections[0]]) im.horizontalAttackGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[0]]);
            if (im.verticalAttackGamepad.ToString() != _keyCodes[_selections[1]]) im.verticalAttackGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[1]]);
            if (im.dodge_runGamepad.ToString() != _keyCodes[_selections[2]]) im.dodge_runGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[2]]);
            if (im.movementSkillGamepad.ToString() != _keyCodes[_selections[3]]) im.movementSkillGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[3]]);
            if (im.lockOnGamepad.ToString() != _keyCodes[_selections[4]]) im.lockOnGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[4]]);
            if (im.useSkillGamepad.ToString() != _keyCodes[_selections[5]]) im.useSkillGamepad = (KeyCode)Enum.Parse(typeof(KeyCode), _keyCodes[_selections[5]]);
        }
    }
}
