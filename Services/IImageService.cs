using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RealEstate.Models;
using RealEstate.Representation.Request;
using RealEstate.Representation.Response;

namespace RealEstate.Services
{
    public interface IImageService
    {
        Task CreateFiles(Base64PropertyRequest propertyRequest);
        void CreateFormFiles(IFormFileCollection files);
        Task<string> CreateBucketAsync(string bucket);
        Task<Property> UploadFileS3Async(PropertyRequest propertyRequest);
        List<PropertyItemResponse> GetProperties(SearchRequest search);
        PropertyDetailsResponse GetPropertyDetails(string id);
    }
}
