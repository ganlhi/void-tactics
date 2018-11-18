using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


namespace Vdev {

/// <summary>
/// 
/// </summary>
public static class PropertiesHelper
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="array"></param>
	/// <param name="f"></param>
	/// <returns></returns>
	public static int FindIndexOf( SerializedProperty array, Predicate<SerializedProperty> f )
	{
		for( int i = 0; i < array.arraySize; ++i )
		{
			var element = array.GetArrayElementAtIndex(i);
			if( f(element) )
				return i;
		}

		return -1;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="array"></param>
	/// <param name="f"></param>
	/// <returns></returns>
	public static SerializedProperty FindIf( SerializedProperty array, Predicate<SerializedProperty> f )
	{
		for( int i = 0; i < array.arraySize; ++i )
		{
			var element = array.GetArrayElementAtIndex(i);
			if( f(element) )
				return element;
		}

		return null;
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="arrayProperty"></param>
	/// <param name="atIndex"></param>
	/// <param name="offset"></param>
	public static void RShiftArrayElements( SerializedProperty arrayProperty, int atIndex, int offset )
	{
		if( !arrayProperty.isArray )
			throw new ArgumentException( "RShiftArrayElements - property isn't an array" );
		
		if( offset == 0 )
			return;

		if( offset < 0 )
			throw new ArgumentException( string.Format( "Incorrect offset value {0} - can't be negative ", offset ) );

		int oldArraySize = arrayProperty.arraySize;

		if( oldArraySize == 0 || atIndex > oldArraySize - 1)
			throw new ArgumentException( string.Format( "Incorrect array index {0}, because array size is {1}", atIndex, oldArraySize ) );

		arrayProperty.arraySize = arrayProperty.arraySize + offset;

		for( int srcIndex = oldArraySize - 1, dstIndex = arrayProperty.arraySize - 1; srcIndex >= atIndex; --srcIndex, --dstIndex )
			arrayProperty.MoveArrayElement( srcIndex, dstIndex );
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="arrayProperty"></param>
	/// <param name="atIndex"></param>
	/// <returns></returns>
	public static SerializedProperty InsertEmptyAt( SerializedProperty arrayProperty, int atIndex )
	{
		if (!arrayProperty.isArray)
		{
			throw new ArgumentException( "Attempt to Call InsertEmptyAt for non-array property" );
		}

		int currentSize = arrayProperty.arraySize;

		if (atIndex < 0 || currentSize < atIndex)
		{
			throw new ArgumentException(string.Format( "Can't insert element at index {0}, because array size is {1}", atIndex, currentSize ) );
		}


		if (atIndex == currentSize)
		{
			arrayProperty.arraySize = arrayProperty.arraySize + 1;
		}
		else
		{
			RShiftArrayElements(arrayProperty, atIndex, 1);
		}

		return arrayProperty.GetArrayElementAtIndex( atIndex );
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="arrayProperty"></param>
	/// <returns></returns>
	public static SerializedProperty InsertEmptyAtEnd( SerializedProperty arrayProperty )
	{
		return InsertEmptyAt( arrayProperty, arrayProperty.arraySize );
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="arrayProperty"></param>
	/// <param name="elementsIndices"></param>
	public static void DeleteArrayElements( SerializedProperty arrayProperty, IEnumerable<int> elementsIndices )
	{
		if (!arrayProperty.isArray)
		{
			throw new ArgumentException( "Attempt to Call DeleteElements for non-array property" );
		}

		List<int> removedIndices = new List<int>(elementsIndices);

		if (removedIndices.Count == 0)
		{
			return;
		}

		int arraySize = arrayProperty.arraySize;
		removedIndices.Sort();

		foreach (var id in removedIndices)
		{
			if (id < 0 || id >= arraySize)
			{
				throw new ArgumentException( string.Format( "Bad index value {0}, array size is {1}", id, arraySize) );
			}
		}

		var indicesRemapOffsets = new int[arraySize];
		for (int i = 0, offset = 0; i < arraySize; ++i)
		{
			if( offset < removedIndices.Count && i == removedIndices[offset] )
			{
				indicesRemapOffsets[i] = 0;
				++offset;
			}
			else
			{
				indicesRemapOffsets[i] = -offset;
			}
		}

		for (int i = 0; i < arraySize; ++i)
		{
			int offset = indicesRemapOffsets[i];
			if (offset != 0)
			{
				arrayProperty.MoveArrayElement(i, i + offset);
			}
		}

		arrayProperty.arraySize = arrayProperty.arraySize - removedIndices.Count;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="array"></param>
	/// <param name="index"></param>
	public static void DeleteArrayElementAt( SerializedProperty array, int index )
	{
		DeleteArrayElements( array, new int[]{ index } );
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="array"></param>
	/// <param name="f"></param>
	/// <returns></returns>
	public static int DeleteArrayElementsIf( SerializedProperty array, Predicate<SerializedProperty> f )
	{
		List<int> indices = null;

		for (int i = 0; i < array.arraySize; ++i)
		{
			var element = array.GetArrayElementAtIndex(i);
			if (f(element))
			{
				if (indices == null)
				{
					indices = new List<int>();
				}
				
				indices.Add( i );
			}
		}

		if (indices != null)
		{
			DeleteArrayElements( array, indices );
		}

		return (indices == null ? 0 : indices.Count);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="prop"></param>
	/// <param name="enumValue"></param>
	public static void SetEnumValue<T>( SerializedProperty prop, T enumValue )
	{
		int index = Array.IndexOf( Enum.GetValues(typeof(T)), enumValue );
		if (index >= 0)
		{
			prop.enumValueIndex = index;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="prop"></param>
	/// <returns></returns>
	public static T GetEnumValue<T>( SerializedProperty prop )
	{
		var enumValues = (T[])Enum.GetValues(typeof(T));

		return enumValues[prop.enumValueIndex];
	}

	//---------------------------------------------------------------------------------------------
	#if DISABLED_BLOCK
	public static void ArrayElementEditor( string label, int arraySize, Func<int,string> getDisplayString, System.Func<string> getValue, System.Action<string,int> setNewValue )
	{
		var currentValue = getValue();

		if( arraySize > 0 )
		{
			var values = new string[arraySize+1];
			values[0] = "[none]";
			
			for( int i = 0; i < arraySize; ++i )
				values[i+1] = getDisplayString(i);

			var index = Array.IndexOf( values, currentValue, 1 );
			if( index < 0 && !string.IsNullOrEmpty( currentValue ) )
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label( label );

				if( GUILayout.Button( "Unlock: {0}".Fmt(currentValue) ))
					setNewValue(string.Empty,-1);
				
				GUILayout.EndHorizontal();
			}
			else
			{
				var newIndex = EditorGUILayout.Popup( label, index, values );
				if( newIndex != index )
				{
					var newValue = newIndex == 0 ? string.Empty : values[newIndex];
					setNewValue( newValue, (newIndex - 1));
				}
			}
		}
		else
			GUILayout.Label( "{0}: {1}".Fmt( label, currentValue ) );
	}
#endif
}

}


#endif