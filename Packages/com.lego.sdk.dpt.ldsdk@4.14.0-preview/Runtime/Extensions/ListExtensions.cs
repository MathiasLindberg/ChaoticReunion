using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class ListExtensions
{

    public static List<DerivedType> OfType<DerivedType>( this IEnumerable list ) where DerivedType : class
    {
        List<DerivedType> derivedItems = new List<DerivedType>();
        foreach (var item in list)
        {
            if( item is DerivedType )
                derivedItems.Add( item as DerivedType );
        }

        return derivedItems;
    }

#if NETFX_CORE

	public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
	{
		foreach (T item in list)
		{
			action(item);
		}
	}

#endif

}
