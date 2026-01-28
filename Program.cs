// See https://aka.ms/new-console-template for more information
using ConsoleApp1.Features;
using System.Net;
using System.Text;

Console.WriteLine("Hello, World!");

Test();

async void Test()
{
    try
    {
        string encryptedRequest =
    "9d845434dea7f3399ab6e33e45b5e09b725084a174f8903559b7be279949d71ab3aaffbbe3b5d880cc3fe7895e78b4ba66ab897845744b924545e0df5b594ca9";

        string response = await FetchPlansRawQAsync(
            "AVRH84FJ81KR61WZSC",
            "ES325632112345678998745632aa2986678",
            "1.0",
            "ES32",
            encryptedRequest
        );

        string _encrptnKey = "E53464862026EC4BCD081D0024EB0037";
        // 🔐 Decrypt response
        string decrypted = EncryptionHelper.Decrypt(response.Trim(), _encrptnKey);
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}



async Task<string> FetchPlansRawAsync(
    string accessCode,
    string requestId,
    string version,
    string instituteId,
    string encryptedRequestHex
)
{
   try
    {
        using var client = new HttpClient();

        // 1️⃣ Build URL exactly like PHP
        string url =
            "https://stgapi.billavenue.com/billpay/extFetchPlans/fetchPlansRequest/json" +
            $"?accessCode={accessCode}" +
            $"&requestId={requestId}" +
            $"&ver={version}" +
            $"&instituteId={instituteId}" +
            $"&encRequest={encryptedRequestHex}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);

        // 2️⃣ Headers (Content-Type: text/plain)
        request.Headers.Accept.Clear();
        request.Content = new StringContent(
            encryptedRequestHex,
            Encoding.UTF8,
            "text/plain"
        );

        // 3️⃣ Send request
        using HttpResponseMessage response = await client.SendAsync(request);

        // 4️⃣ Handle response
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Unexpected HTTP status: {(int)response.StatusCode} {response.ReasonPhrase}\n{responseBody}"
            );
        }

        return responseBody;
    }
    catch(Exception ex)
    {
        Console.WriteLine( ex.ToString() );
    }
    return string.Empty;
}

async Task<string> FetchPlansRawQAsync(
    string accessCode,
    string requestId,
    string version,
    string instituteId,
    string encryptedRequestHex
)
{
    try
    {
        // 1️⃣ Create HttpClient (do not dispose immediately)
        var client = new HttpClient();

        // 2️⃣ Build URL with all parameters including encryptedRequestHex
        var query = new Dictionary<string, string>
        {
            ["accessCode"] = accessCode,
            ["requestId"] = requestId,
            ["ver"] = version,
            ["instituteId"] = instituteId,
            ["encryptedRequestHex"] = encryptedRequestHex
        };

        var url = "https://stgapi.billavenue.com/billpay/extFetchPlans/fetchPlansRequest/json";

        // Build the query string safely
        var uriBuilder = new UriBuilder(url)
        {
            Query = string.Join("&", query.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"))
        };

        // 3️⃣ Create request manually
        var request = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri);

        // 4️⃣ Headers (optional if no body)
        request.Headers.Accept.Clear();

        // 5️⃣ Send request
        HttpResponseMessage response = await client.SendAsync(request);

        // 6️⃣ Read response
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Unexpected HTTP status: {(int)response.StatusCode} {response.ReasonPhrase}\n{responseBody}"
            );
        }

        return responseBody;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }

    return string.Empty;
}
