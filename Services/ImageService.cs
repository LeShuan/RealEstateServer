using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Representation.Request;
using RealEstate.Representation.Response;

using RealEstate.Utils;

namespace RealEstate.Services
{
    public class ImageService : IImageService
    {


        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IAmazonS3 _client;

        public ImageService(IHostingEnvironment hostingEnvironment, IAmazonS3 client)
        {

            _client = client;
            _hostingEnvironment = hostingEnvironment;
          
        }


        public async Task<string> CreateBucketAsync(string bucket) {
            
                if (await AmazonS3Util.DoesS3BucketExistAsync(_client, bucket) == false) {

                    var putBucketRequest = new PutBucketRequest {
                        BucketName = bucket,
                        UseClientRegion = true
                    };

                    var response = await _client.PutBucketAsync(putBucketRequest);

                    return "RequestID:"+response.ResponseMetadata.RequestId;
                }
            return "The Bucket already exists";
           

        }

        public async Task<Property> UploadFileS3Async(PropertyRequest propertyRequest) {
            var fileTransferUtility = new TransferUtility(_client);

            var bucket = "shua-shua-shua";
            Property property = new Property();
            PropertyDetails propertyDetails = new PropertyDetails();
            List<Image> images = new List<Image>();

            foreach (var formFile in propertyRequest.Images)
            {
                //file validations
                if (formFile.Length <= 0) { continue; }
                if (formFile.Length >= 5242880) { throw new FileLoadException("The file you are trying to upload is to big."); }
                string[] fileContent = formFile.FileName.Split('.');
                if (!fileContent[1].Equals("jpg") && !fileContent[1].Equals("jpeg") && !fileContent[1].Equals("gif") && !fileContent[1].Equals("png")) { throw new FileLoadException("The file you are trying to upload must be an Image with a jpg jpeg gif or png extension."); }                  

                string key = GeneralUtils.DateText(8) + formFile.FileName;

                using (var stream = formFile.OpenReadStream())
                {

                   
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucket,
                        InputStream = stream,
                        Key = key,
                        StorageClass = S3StorageClass.Standard,
                        PartSize = 6291456, //mb
                        CannedACL = S3CannedACL.PublicRead
                    };

                    await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                    images.Add(new Image { Url = "http://s3.amazonaws.com/"+ bucket + "/"+key });

                }

            }

            property.Name = propertyRequest.Name;
            property.MainImageURL = images[0].Url;
            property.Longitud = propertyRequest.Longitud;
            property.Latitud = propertyRequest.Latitud;
            property.Price = propertyRequest.Price;
            property.Size = propertyRequest.Size;
            property.Address = propertyRequest.Address;
            propertyDetails.Bathrooms = propertyRequest.Bathrooms;
            propertyDetails.Rooms = propertyRequest.Rooms;
            propertyDetails.ParkingSlots = propertyRequest.ParkingSlots;
            propertyDetails.Description = propertyRequest.Description;
            propertyDetails.Images = images;

            property.Details = propertyDetails;

            using (var _context = new RealEstateContext()) {

                _context.Properties.Add(property);

                _context.SaveChanges();

            }

           return property;

        }


        public async Task CreateFiles(Base64PropertyRequest propertyRequest)
        {
            string SavePath = _hostingEnvironment.ContentRootPath + "/Images/"+GeneralUtils.DateText(8);

            foreach(var file in propertyRequest.Images) {
                byte[] bytes = file.getBase64ByteArray();
                using (FileStream SourceStream = File.Open(SavePath+file.Name, FileMode.Create))
                {
                    SourceStream.Seek(0, SeekOrigin.End);
                    await SourceStream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
        }

        public void CreateFormFiles(IFormFileCollection files) {

            string SavePath = _hostingEnvironment.ContentRootPath + "/Images/" + GeneralUtils.DateText(8);

            foreach (var formFile in files) {

                if (formFile.Length <= 0) { continue; }
               
                using (var stream = new FileStream(SavePath + formFile.FileName, FileMode.Create))
                {
                    formFile.CopyToAsync(stream);
                }
            }
        }

        public async Task DeletingAnObject(string key)
        {
            try
            {
                
                await _client.DeleteAsync("shua-shua-shua",key, new Dictionary<string,object>());

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when deleting an object", amazonS3Exception.Message);
                }
            }
        }

        public List<PropertyItemResponse> GetProperties(SearchRequest search)
        {
            List<PropertyItemResponse> propertyItemsResponses = new List<PropertyItemResponse>();
            List<Property> properties;
            if (search.Distance == 0) { search.Distance = 10; }

            double MinLat = search.Latitud - (search.Distance * 0.01);
            double MaxLat = search.Latitud + (search.Distance * 0.01);
            double MinLon = search.Longitud - (search.Distance * 0.01);
            double MaxLon = search.Longitud + (search.Distance * 0.01);

            using (var _context = new RealEstateContext()) {
                if (search.PriceMin != 0 && search.PriceMax != 0)
                {
                    properties = _context.Properties.Where(s => (s.Latitud > MinLat && s.Latitud < MaxLat &&
                                                     s.Longitud > MinLon && s.Longitud < MaxLon &&
                                                     s.Price > search.PriceMin && s.Price < search.PriceMax)).ToList<Property>();
                }
                else if ( search.PriceMax == 0)
                {
                    properties = _context.Properties.Where(s => (s.Latitud > MinLat && s.Latitud < MaxLat &&
                                                    s.Longitud > MinLon && s.Longitud < MaxLon &&
                                                    s.Price > search.PriceMin)).ToList<Property>();
                }
                else {
                    properties = new List<Property>();
                }
               

            }

            foreach (var pro in properties) {
                propertyItemsResponses.Add(new PropertyItemResponse {
                    id = pro.id,
                    Name = pro.Name,
                    MainImageURL = pro.MainImageURL,
                    Longitud = pro.Longitud,
                    Latitud = pro.Latitud,
                    Price = pro.Price,
                    Size = pro.Size,
                    Address = pro.Address
                });
            }



            return propertyItemsResponses;

        }

        public PropertyDetailsResponse GetPropertyDetails(string id)
        {
            PropertyDetailsResponse propertyDetailsResponse = new PropertyDetailsResponse();

            Property daProperty;

            using (var _context = new RealEstateContext())
            {
                daProperty = _context.Properties.Include("Details.Images").Where(s => s.id == Int32.Parse(id) ).FirstOrDefault<Property>();
            }
            

            propertyDetailsResponse.Name = daProperty.Name;
            propertyDetailsResponse.MainImageURL = daProperty.MainImageURL;
            propertyDetailsResponse.Address = daProperty.Address;
            propertyDetailsResponse.Latitud = daProperty.Latitud;
            propertyDetailsResponse.Longitud = daProperty.Longitud;
            propertyDetailsResponse.id = daProperty.id;
            propertyDetailsResponse.Price = daProperty.Price;
            propertyDetailsResponse.Size = daProperty.Size;
            propertyDetailsResponse.Rooms = daProperty.Details.Rooms;
            propertyDetailsResponse.ParkingSlots = daProperty.Details.ParkingSlots;
            propertyDetailsResponse.Bathrooms = daProperty.Details.Bathrooms;
            propertyDetailsResponse.Description = daProperty.Details.Description;
            propertyDetailsResponse.Images = new List<ImagesResponse>();

            foreach (Image im in daProperty.Details.Images) {
                propertyDetailsResponse.Images.Add(new ImagesResponse {Id=im.Id, Url=im.Url });
            }


            return propertyDetailsResponse;
        }
    }
}
