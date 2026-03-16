using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace TripServiceHelper.Cloudinary;

public class CloudinaryTripService : ICloudinaryTripService
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;

    public CloudinaryTripService(IConfiguration config)
    {
        var account = new Account(
            config["CloudinaryTrip:CloudName"],
            config["CloudinaryTrip:ApiKey"],
            config["CloudinaryTrip:ApiSecret"]
        );
        _cloudinary = new CloudinaryDotNet.Cloudinary(account);
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }
}