using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public static class EnumExt<TEnum>
        where TEnum : struct
{
    private static readonly Dictionary<string, TEnum> read_dict_Enums;
    private static readonly Dictionary<string, string> read_dict_Enums_desc;

    static EnumExt()
    {
        read_dict_Enums = new Dictionary<string, TEnum>();
        read_dict_Enums_desc = new Dictionary<string, string>();
        Type enumType = typeof(TEnum);
        foreach (var val in Enum.GetValues(enumType))
        {
            string valstr = val.ToString();
            read_dict_Enums.Add(valstr, (TEnum)val);

            var descAttr = enumType.GetField(valstr).GetCustomAttributes(typeof(DescriptionAttribute), false);
            string desc = valstr;
            if (descAttr.Length > 0)
            {
                desc = (descAttr.FirstOrDefault() as DescriptionAttribute).Description;
            }
            read_dict_Enums_desc.Add(val.ToString(), desc);
        }
    }

    public static Dictionary<string, TEnum> All()
    {
        return new Dictionary<string, TEnum>(read_dict_Enums);
    }

    public static string ToDescriptionString(TEnum @this)
    {
        return read_dict_Enums_desc[@this.ToString()];
    }

    public static int ToInt(TEnum @this)
    {
        return Convert.ToInt32(@this);
    }
}