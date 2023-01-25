using Core.Characters.AI;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.Tools
{
	[CustomEditor(typeof(AIStateMachine))]
	public class AIStateMachineEditor : Editor
	{
		bool is_foldout = true;

		public override void OnInspectorGUI()
		{
			AIStateMachine machine = target as AIStateMachine;

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.LabelField("Info");

			EditorGUILayout.TextField("Current State", machine.CurrentState != null ? machine.CurrentState.Name : "NULL");
			
			AITask task = machine.CurrentState?.CurrentTask;
			if (task != null)
			{
				Type type = task.GetType();

				is_foldout = EditorGUILayout.Foldout(is_foldout, "Current Task");
				if (is_foldout)
				{
					EditorGUILayout.LabelField(type.FullName);

					int old_level = EditorGUI.indentLevel;
					EditorGUI.indentLevel++;

					//  show every fields
					foreach (FieldInfo info in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					{
						EditorGUILayout.TextField(info.Name, info.GetValue(task)?.ToString());
					}

					EditorGUI.indentLevel = old_level;
				}

			}

			EditorGUI.EndDisabledGroup();
		}
	}
}