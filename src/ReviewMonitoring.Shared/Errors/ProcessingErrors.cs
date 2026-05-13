namespace ReviewMonitoring.Shared.Errors;
public static class ProcessingErrors
{
    static readonly Dictionary<int, string> DictErrors = [];

    /// <summary>
    /// Получает строку ошибки по коду ошибки
    /// </summary>
    /// <param name="errorVal">Значение ошибки</param>
    /// <returns>Строка ошибки</returns>
    public static string GetErrorString(int errorVal)
    {
        if (DictErrors.Count <= 0) FillDict();

        if (DictErrors.TryGetValue(errorVal, out string? value))
        {
            return value;
        }
        else
        {
            return string.Format("Ошибка #{0}", errorVal < 0 ? errorVal * -1 : errorVal);
        }
    }

    /// <summary>
    /// Заполнить словарь
    /// </summary>
    private static void FillDict()
    {
        DictErrors.Add(Err100, Err100Str);
    }

    /// <summary>
    /// Ошибка #100 - Передан Null
    /// </summary>
    public const int Err100 = -100;


    /// <summary>
    /// Ошибка #100 - Передан Null
    /// </summary>
    public const string Err100Str = "Ошибка #100 - Передан Null";
}
