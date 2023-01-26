using Core.AI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.Tools
{
	[CustomEditor(typeof(AIStateMachine))]
	public class AIStateMachineEditor : Editor
	{
		bool isTaskFoldout = true, isPropertiesFoldout = false;

		public override void OnInspectorGUI()
		{
			AIStateMachine machine = target as AIStateMachine;

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.LabelField("Info");

			EditorGUILayout.TextField("Current State", machine.CurrentState != null ? machine.CurrentState.Name : "NULL");
			
			//  populate task
			AITask task = machine.CurrentState?.CurrentTask;
			if (task != null)
			{
				Type type = task.GetType();

				isTaskFoldout = EditorGUILayout.Foldout(isTaskFoldout, "Current Task");
				if (isTaskFoldout)
				{
					EditorGUILayout.LabelField(type.FullName);

					//  show every fields
					EditorGUI.indentLevel++;
					foreach (FieldInfo info in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					{
						EditorGUILayout.TextField(info.Name, info.GetValue(task)?.ToString());
					}
					EditorGUI.indentLevel--;
				}
			}

			//  populate properties
			isPropertiesFoldout = EditorGUILayout.Foldout(isPropertiesFoldout, "Properties");
			if (isPropertiesFoldout)
			{
				EditorGUI.indentLevel++;
				foreach (KeyValuePair<string, object> pair in machine.Properties)
				{
					EditorGUILayout.TextField(pair.Key, pair.Value == null ? "null" : pair.Value.ToString());
				}
				EditorGUI.indentLevel--;
			}

			EditorGUI.EndDisabledGroup();
		}
	}
}