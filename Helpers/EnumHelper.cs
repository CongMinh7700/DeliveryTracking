using System.ComponentModel;

public static class EnumHelper
{
    public static string GetDescription(Enum? value)
    {
        if (value == null)
            return string.Empty;

        var type = value.GetType();
        var name = Enum.GetName(type, value);

        if (name == null)
            return ((int)(object)value).ToString();

        var field = type.GetField(name);
        var attr = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attr != null ? attr.Description : name;
    }


    public static List<KeyValuePair<Enum, string>> GetEnumListWithDescriptions<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<Enum>()
            .Select(e => new KeyValuePair<Enum, string>(e, GetDescription(e)))
            .ToList();
    }
    public static List<KeyValuePair<int, string>> GetIntListWithDescriptions<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<Enum>()
            .Select(e => new KeyValuePair<int, string>(Convert.ToInt32(e), GetDescription(e)))
            .ToList();
    }
}