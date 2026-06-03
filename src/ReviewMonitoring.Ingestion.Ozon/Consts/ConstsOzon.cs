namespace ReviewMonitoring.Ingestion.Ozon.Consts;
internal static class ConstsOzon
{
    public static readonly Dictionary<string, string> DefualtHeaders = new()
        {
            ["x-o3-app-name"] = "dweb_client",
            ["x-o3-app-version"] = "release_27-4-2026_86f40e87",
            ["Referer"] = "https://www.ozon.ru/sw/bx/3.0.9.js",
            ["sec-fetch-dest"] = "empty",
            ["sec-fetch-mode"] = "cors",
            ["sec-fetch-site"] = "same-origin"
        };
}
