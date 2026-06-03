namespace ReviewMonitoring.Shared.Helpers;

public static class UrlHelper
{
    public static string NormalizeUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;
        return url.Split('?')[0].TrimEnd('/');
    }
}
