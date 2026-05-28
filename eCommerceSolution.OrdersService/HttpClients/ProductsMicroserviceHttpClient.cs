using eCommerceSolution.OrdersService.Models.DTOs.HttpClient.Formats.ProductsMicroservice;

namespace eCommerceSolution.OrdersService.HttpClients;

public class ProductsMicroserviceHttpClient
{
    private readonly HttpClient _httpClient;

    public ProductsMicroserviceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

    public async Task<GetProductByIdResponse> GetProductById(Guid productId)
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Products/search/product-id/{productId}");
        if (httpResponse.IsSuccessStatusCode)
        {
            GetProductByIdResponse? response = await httpResponse.Content.ReadFromJsonAsync<GetProductByIdResponse>();
            if (response != null)
            {
                return response;
            }
        }
        throw new Exception($"Product with id {productId} not found."); 
    }

}
