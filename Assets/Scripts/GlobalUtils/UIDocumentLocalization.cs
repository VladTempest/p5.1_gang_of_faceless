﻿/// void* src = https://gist.github.com/andrew-raphael-lukasik/72a4d3d14dd547a1d61ae9dc4c4513da
///
/// Copyright (C) 2022 Andrzej Rafał Łukasik (also known as: Andrew Raphael Lukasik)
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, version 3 of the License.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for details https://www.gnu.org/licenses/
///

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

// NOTE: this class assumes that you designate StringTable keys in label fields (as seen in Label, Button, etc)
// and start them all with '#' char (so other labels will be left be)
// example: https://i.imgur.com/H5RUIej.gif
namespace Editor.Scripts.GlobalUtils
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UIDocument))]
	public class UIDocumentLocalization : MonoBehaviour
	{
		private Dictionary<VisualElement,string> _originalTexts = new Dictionary<VisualElement, string>();

		[SerializeField] LocalizedStringTable _table = null;
		UIDocument _uiDocument;

		/// <summary> Executed after hierarchy is cloned fresh and translated. </summary>
		public event System.Action onCompleted = () =>
		{
			ConvenientLogger.Log(nameof(UIDocumentLocalization),GlobalLogConstant.IsLocalizeLogEnabled,$"{nameof(UIDocument)} translated");
		};


		void OnEnable()
		{
			if (_uiDocument == null)
				_uiDocument = GetComponent<UIDocument>();
			_table.TableChanged += OnTableChanged;
		}

		void OnDisable()
		{
			_table.TableChanged -= OnTableChanged;
		}


		void OnTableChanged(StringTable table)
		{
			ConvenientLogger.Log(nameof(UIDocumentLocalization),GlobalLogConstant.IsLocalizeLogEnabled,$"{nameof(StringTable)} changed, {nameof(VisualTreeAsset)} has been cloned anew",
				_uiDocument);

			var op = _table.GetTableAsync();
			if (op.IsDone)
			{
				OnTableLoaded(op);
			}
			else
			{
				op.Completed -= OnTableLoaded;
				op.Completed += OnTableLoaded;
			}
		}

		void OnTableLoaded(AsyncOperationHandle<StringTable> op)
		{
			StringTable table = op.Result;
			LocalizeChildrenRecursively(_uiDocument.rootVisualElement, table);
			_uiDocument.rootVisualElement.MarkDirtyRepaint();
			onCompleted();
		}

		void LocalizeChildrenRecursively(VisualElement element, StringTable table)
		{
			VisualElement.Hierarchy elementHierarchy = element.hierarchy;
			int numChildren = elementHierarchy.childCount;
			for (int i = 0; i < numChildren; i++)
			{
				VisualElement child = elementHierarchy.ElementAt(i);
				Localize(child, table);
			}

			for (int i = 0; i < numChildren; i++)
			{
				VisualElement child = elementHierarchy.ElementAt(i);
				VisualElement.Hierarchy childHierarchy = child.hierarchy;
				int numGrandChildren = childHierarchy.childCount;
				if (numGrandChildren != 0)
					LocalizeChildrenRecursively(child, table);
			}
		}

		void Localize(VisualElement next, StringTable table)
		{
			if (_originalTexts.TryGetValue(next, out var text))
			{
				((TextElement) next).text = text;
			}

			if (typeof(TextElement).IsInstanceOfType(next))
			{
				TextElement textElement = (TextElement) next;
				string key = textElement.text;
				if (!string.IsNullOrEmpty(key) && key[0] == '#')
				{
					_originalTexts.TryAdd(textElement, textElement.text);
					
					key = key.TrimStart('#');
					StringTableEntry entry = table[key];
					if (entry != null)
						textElement.text = entry.LocalizedValue;
					else
						ConvenientLogger.LogWarning(nameof(UIDocumentLocalization),GlobalLogConstant.IsLocalizeLogEnabled,$"No {table.LocaleIdentifier.Code} translation for key: '{key}'");
				}
			}
		}

	}
}