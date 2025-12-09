using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.API.Utils;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLibraryController : ControllerBase
    {
        private readonly IUserLibraryService _userLibraryService;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;
        private readonly Validator _validator;

        public UserLibraryController(IMapper mapper, 
            IUserLibraryService userLibraryService, IUserContextService userContext, Validator validator)
        {
            _userLibraryService = userLibraryService;
            _mapper = mapper;
            _userContext = userContext;
            _validator = validator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLibraries()
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var libraries = await _userLibraryService.GetLibrariesByUserAsync(user.Id);
            return Ok(APIResponse<List<UserLibraryDTO>>.SuccessResponse(libraries, "Libraries fetched successfully"));
        }

        [HttpGet("minimal")]
        [Authorize]
        public async Task<IActionResult> GetLibrariesMinimal()
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var libraries = await _userLibraryService.GetMinimalLibrariesByUserAsync(user.Id);
            return Ok(APIResponse<List<UserLibraryDTO>>.SuccessResponse(libraries, "Libraries fetched successfully"));
        }

        [HttpGet("{collectionId}")]
        [Authorize]
        public async Task<IActionResult> GetLibrary(Guid collectionId)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var library = await _userLibraryService.GetLibraryCollectionAsync(collectionId, user);
            if (library == null) return NotFound(APIResponse<object>.ErrorResponse("Library not found"));

            return Ok(APIResponse<UserLibraryDTO>.SuccessResponse(library, "Library fetched successfully"));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateLibrary([FromBody] UserLibraryCreateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var result = await _userLibraryService.CreateLibraryAsync(request, user);
            return Ok(APIResponse<UserLibraryItemDTO>.SuccessResponse(result, "Library created successfully"));
        }

        [HttpPost("initialize")]
        [Authorize]
        public async Task<IActionResult> InitializeDefaultLibraries()
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var libraries = await _userLibraryService.CreateDefaultLibrariesAsync(user);
            var dtoList = _mapper.Map<List<UserLibraryDTO>>(libraries);
            return Ok(APIResponse<List<UserLibraryDTO>>.SuccessResponse(dtoList, "Default libraries created"));

        }

        [HttpPost("item")]
        [Authorize]
        public async Task<IActionResult> AddItemToLibrary([FromBody] UserLibraryItemCreateRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var item = await _userLibraryService.AddItemToLibraryAsync(request, user);
                return Ok(APIResponse<UserLibraryItemDTO>.SuccessResponse(item, "Item added to library successfully"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditLibrary([FromBody] UserLibraryEditRequest request)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            try
            {
                var result = await _userLibraryService.EditLibraryAsync(request, user);
                return Ok(APIResponse<UserLibraryDTO>.SuccessResponse(result, "Library updated"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteLibrary(Guid id)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try { 
                var success = await _userLibraryService.DeleteLibraryAsync(id, user);
                return Ok(APIResponse<object>.SuccessResponse(success, "Library deleted successfully"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("item/{itemId}")]
        [Authorize]
        public async Task<IActionResult> RemoveLibraryItem(Guid itemId)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {
                var res = await _userLibraryService.RemoveLibraryItemAsync(itemId, user);
                return Ok(APIResponse<object>.SuccessResponse(res, "Item removed from library successfully"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(APIResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(APIResponse<object>.ErrorResponse(ex.Message));
            }

        }
    }
}
