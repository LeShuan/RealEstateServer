using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealEstate.Models;
using RealEstate.Representation.Request;
using Microsoft.AspNetCore.Hosting;
using RealEstate.Services;
using Amazon.S3;
using RealEstate.Representation.Response;

namespace RealEstate.Controllers
{
    [Route("Images")]
    [ApiController]
    public class ImagesController : Controller
    {
      
        private readonly ILogger _logger;
        private readonly IImageService _imageService;

        public ImagesController(ILogger<ImagesController> logger, 
             IImageService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }





        /// <summary>
        ///  Create property method, it recieves all the relevan data to create a property,
        ///  it uploads the images to aws S3 and saves the data in a progresSQL database.
        ///  This method only accepts 60 mb
        /// </summary>
        /// <param name="propertyRequest"></param>
        /// <returns> Property object in a json string</returns>
        [HttpPost]        
        [RequestSizeLimit(62914560)]
        [Route("CreateProperty")]
        public async Task<ActionResult<string>> PostImagesNormal([FromForm]PropertyRequest propertyRequest)
        {
            try
            {
                using (var _context = new RealEstateContext())
                {
                    _context.Database.EnsureCreated();
                }
                Property response = await _imageService.UploadFileS3Async(propertyRequest);
                return StatusCode(201, response);
            }
            catch (AmazonS3Exception e)
            {
                
                return StatusCode(500, e.Message+ e.StackTrace);
            }
            catch (Exception e)
            {
                
                return StatusCode(500, e.Message+ e.StackTrace);
            }
        }
        [HttpPost]
        [Route("Search")]
        public  ActionResult<List<PropertyItemResponse>> SearchProperties([FromForm]SearchRequest search)
        {
            try
            {

                List<PropertyItemResponse> responses =  _imageService.GetProperties(search);
                return StatusCode(201, responses);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        [Route("Property/{id}")]
        public ActionResult<PropertyDetailsResponse> GetPropertyDetails([FromRoute] string id)
        {
            try
            {
                PropertyDetailsResponse response =  _imageService.GetPropertyDetails(id);
                return StatusCode(201, response);
            }
           
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }



        #region
        //Testing routes that do not add anything to the app
        [HttpGet]
        [Route("postNormal/{bucketName}")]
        public async Task<ActionResult<string>> CreateBucker([FromRoute] string bucketName)
        {
            try
            {
                string response = await _imageService.CreateBucketAsync(bucketName);
                return StatusCode(201,response);
            }
            catch (AmazonS3Exception e)
            {
                return StatusCode(500,e.Message);
            }
            catch (Exception e) {
                return StatusCode(500, e.Message);
            }
        }


        [HttpPost]
        [Route("postBase64")]
        public ActionResult<string> PostImagesBase64(Base64PropertyRequest propertyRequest)
        {

            _logger.LogInformation("data: " + propertyRequest.Name);
            _logger.LogInformation("data: " + propertyRequest.Images[1].Name);

            _imageService.CreateFiles(propertyRequest);

            return propertyRequest.Name;
        }


        [HttpPost]
        [Route("postFrom")]
        public ActionResult<string> PostImagesFrom(IFormCollection collection )
        {
            string name = collection["name"];
            string description = collection["description"];
            IFormFileCollection files = collection.Files;
            IFormFile file = files.GetFile("file");
            _logger.LogInformation("data:"+file.ContentType);
            _logger.LogInformation("data:" + name);
            _logger.LogInformation("data:" + description);

            _imageService.CreateFormFiles(files);

            return "data"+file.ContentType+name+description;
        }

        [HttpGet]
        [Route("get")]
        public ActionResult Index()
        {
            using (var _context = new RealEstateContext())
            {
                _context.Database.EnsureCreated();
            }

                return View();
        }
        [HttpPost]
        [Route("get")]
        public ActionResult<string> Index2([FromRoute] PropertyRequest bucketName)
        {


            _logger.LogInformation("dad" + bucketName.Name);
            return "shua";

        }
        #endregion
    }
}
