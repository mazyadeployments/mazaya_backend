namespace MMA.WebApi.Bootstrap
{
    //public static class AzureBlobStorageContainerExtensions
    //{
    //    public static void AddAzureBlobStorageContainer(this IServiceCollection services, IConfiguration config)
    //    {
    //        services.AddTransient<BlobContainerClient>(p => GetBlobClient(p.GetRequiredService<IConfiguration>()));
    //    }

    //    private static BlobContainerClient GetBlobClient(IConfiguration config)
    //    {
    //        var connectionString = config.GetConnectionString("AzureBlobStorage");

    //        var containerName = config.GetSection("AzureBlobStorage").GetValue<string>("ContainerName");

    //        if (string.IsNullOrWhiteSpace(connectionString))
    //        {
    //            throw new Exception("Azure blob storage connection string is not set");
    //        }

    //        if (string.IsNullOrWhiteSpace(containerName))
    //        {
    //            throw new Exception("Azure blob storage container name is not set");
    //        }

    //        var blobServiceClient = new BlobServiceClient(connectionString);

    //        return blobServiceClient.GetBlobContainerClient(containerName);
    //    }
    //}
}
