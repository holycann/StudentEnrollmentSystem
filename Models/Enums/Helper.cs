using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class EnumHelper
{
    public static List<SelectListItem> GetEnumSelectList<T>()
        where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
            .ToList();
    }
}