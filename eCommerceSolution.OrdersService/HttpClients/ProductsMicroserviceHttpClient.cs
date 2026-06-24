using eCommerceSolution.OrdersService.Models.DTOs.HttpClient.Formats.ProductsMicroservice;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace eCommerceSolution.OrdersService.HttpClients;

public class ProductsMicroserviceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;

    public ProductsMicroserviceHttpClient(HttpClient httpClient, IDistributedCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }


    public async Task<bool> ProductExistsAsync(Guid productId)
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Products/search/check/{productId}");
        if (httpResponse.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }

    public async Task<GetProductByIdResponse> GetProductByIdAsync(Guid productId)
    {
        string cacheKey = $"Product_{productId}";

        var cachedProduct = await _cache.GetStringAsync(cacheKey);

        if (cachedProduct == null)
        {

            HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Products/search/product-id/{productId}");
            if (httpResponse.IsSuccessStatusCode)
            {
                GetProductByIdResponse? response = await httpResponse.Content.ReadFromJsonAsync<GetProductByIdResponse>();
                if (response != null)
                {
                    string serializedData = JsonSerializer.Serialize(response);
                    DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5), // NO MATTER WHAT, delete this data 5 minutes after it was first created.
                        SlidingExpiration = TimeSpan.FromMinutes(1) // If no one requests this data for 1 minute, delete it.
                    };
                    await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
                    return response;
                }
            }
            throw new Exception($"Product with id {productId} not found.");
        }
        else
        {
            GetProductByIdResponse? responseFromCache = JsonSerializer.Deserialize<GetProductByIdResponse>(cachedProduct);
            return responseFromCache;
        }
    }
}
