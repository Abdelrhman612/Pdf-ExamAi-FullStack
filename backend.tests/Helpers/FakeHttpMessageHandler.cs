using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;

public static class FakeHttpMessageHandler
{
    public static HttpClient Create(HttpResponseMessage response)
    {
        var handler = A.Fake<HttpMessageHandler>();

        A.CallTo(handler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(response);

        return new HttpClient(handler);
    }
}
