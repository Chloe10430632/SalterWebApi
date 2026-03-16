namespace TripServiceHelper.Cloudinary;

public interface ICloudinaryTripService
{
    Task<bool> DeleteImageAsync(string publicId);
}