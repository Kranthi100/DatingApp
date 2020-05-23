using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/user/{userid}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private Cloudinary _cloudinary;
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;


        public PhotosController(IDatingRepository repo, IMapper mapper,
        IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;
      
      //  public Account(string cloud, string apiKey, string apiSecret);

            // Account acc = new Account(
            //     _cloudinaryConfig.Value.CloudName,
            //     _cloudinaryConfig.Value.ApiKey,
            //     _cloudinaryConfig.Value.ApiSecret
            // );
            Account acc = new Account("kumar1611","376372766126778","xnlW5MPFHVVjKARi1C78ziVhh6I");

            _cloudinary = new Cloudinary(acc);
        }


        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,
            [FromForm] PhotoForCrationDto photoForCrationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
  
            var userFromRepo = await _repo.GetUser(userId);
            var file = photoForCrationDto.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uplaodParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                        .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uplaodParams);
                }
            }

            photoForCrationDto.Url = uploadResult.Uri.ToString();
            photoForCrationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCrationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id },
                 photoToReturn);
            }
            return BadRequest("Could not add photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> setMainPhoto(int userId, int id) {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repo.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == id)) {
                return Unauthorized();            
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo.IsMain) {
                return BadRequest("This is already set to main photo!");
            }

            var currentMainphoto = await _repo.GetMainPhotoForUser(userId);
            currentMainphoto.IsMain = false;
            photoFromRepo.IsMain = true ;

            if(await _repo.SaveAll())
                return NoContent();

            return BadRequest("Cloud not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto (int userId, int id) {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repo.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == id)) {
                return Unauthorized();            
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo.IsMain) {
                return BadRequest("You can not delete your main photo!");          
            }

            if(photoFromRepo.PublicId != null) {

            var deleteParams = new DeletionParams(photoFromRepo.PublicId);
            var results = _cloudinary.Destroy(deleteParams);

                if(results.Result == "ok") {
                 _repo.Delete(photoFromRepo);
                }
            }
            
                if(photoFromRepo.PublicId == null) {
                     _repo.Delete(photoFromRepo);
                }
             
            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to delete photo!");         
        }
    }
}