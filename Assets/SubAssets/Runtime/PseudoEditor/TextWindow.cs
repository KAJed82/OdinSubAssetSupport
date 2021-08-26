#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SubAssets.Editor
{
	public class TextWindow : EditorWindow
	{
		public static void Open( string title, string startText, System.Action<string> callback )
		{
			var w = CreateWindow<TextWindow>();
			w.titleContent = new GUIContent( title );
			w.text = startText;
			w.callback = callback;
			w.minSize = new Vector2( 300, 20 );
			w.maxSize = w.minSize;

			var textField = new TextField();
			textField.value = w.text;
			textField.RegisterCallback<AttachToPanelEvent>( evt => textField.Focus() );
			textField.RegisterValueChangedCallback(
				changedEvent =>
				{
					w.text = changedEvent.newValue;
				}
			);
			textField.RegisterCallback<KeyDownEvent>( w.OnKeyDown );

			w.rootVisualElement.Add( textField );

			w.ShowModalUtility();
		}

		private System.Action<string> callback;
		[SerializeField] private string text;

		private void OnKeyDown( KeyDownEvent evt )
		{
			if ( evt == null )
				return;

			if ( evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter )
			{
				evt.StopImmediatePropagation();

				if ( callback != null )
					callback( text );
				Close();
			}

			if ( evt.keyCode == KeyCode.Escape )
			{
				evt.StopImmediatePropagation();
				Close();
			}
		}
	}
}
#endif