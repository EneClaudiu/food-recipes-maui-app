using Amazon.S3;
using Amazon.S3.Transfer;
using System.Diagnostics;

namespace RecipeCabinetAPI.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = string.Empty;

        public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"];
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                var transferUtility = new TransferUtility(_s3Client);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = fileStream,
                    Key = fileName,
                    BucketName = _bucketName,
                };

                await transferUtility.UploadAsync(uploadRequest);
                return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading to S3: " + ex.Message);
                throw;
            }
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, fileName);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ResponseStream;
            }
            else
            {
                throw new Exception("Failed to retrieve file from S3.");
            }
        }
    }
}
