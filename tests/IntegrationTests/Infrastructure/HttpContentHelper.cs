using System.Text;
using Newtonsoft.Json;

namespace SmartFridgeManagerAPI.IntegrationTests.Infrastructure;

public static class HttpContentHelper
{
    public static HttpContent ToJsonHttpContent(this object obj)
    {
        string json = JsonConvert.SerializeObject(obj);

        StringContent httpContent = new(json, Encoding.UTF8, "application/json");

        return httpContent;
    }
}
