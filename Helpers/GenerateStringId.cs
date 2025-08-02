namespace DeliveryTrackingApp.Helpers;

public static class GenerateStringId
{
    public static string GenerateCode<T>(
        IQueryable<T> query,
        Func<T, string> idSelector,
        string prefix,
        int numberLength = 2)
    {
        var lastCode = query
            .AsEnumerable() // chuyển từ IQueryable -> IEnumerable
            .Where(x => idSelector(x).StartsWith(prefix))
            .OrderByDescending(x => idSelector(x))
            .Select(x => idSelector(x))
            .FirstOrDefault();

        int nextNumber = 1;

        if (!string.IsNullOrEmpty(lastCode) && lastCode.StartsWith(prefix))
        {
            var numberPart = lastCode.Substring(prefix.Length);
            if (int.TryParse(numberPart, out int current))
            {
                nextNumber = current + 1;
            }
        }

        return $"{prefix}{nextNumber.ToString().PadLeft(numberLength, '0')}";
    }
}
