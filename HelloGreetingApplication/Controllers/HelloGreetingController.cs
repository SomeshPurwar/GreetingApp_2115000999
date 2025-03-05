using BusinessLayer.Interface;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.DTO;

namespace HelloGreetingApplication.Controllers
{
    /// <summary>
    /// Class providing API for HelloGreeting
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloGreetingController : ControllerBase
    {
        private static Dictionary<string, string> greetings = new Dictionary<string, string>();
        private readonly IGreetingBL _greetingBL;

        public HelloGreetingController(IGreetingBL greetingBL)
        {
            _greetingBL = greetingBL;
        }

        /// <summary>
        /// Get method to get the greeting message
        /// </summary>
        /// <returns> "Hello World" </returns>
        [HttpGet]
        public IActionResult GetMethod() 
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Hello to Greeting App Api endpoint.";
            responseModel.Data = "Hello World";
            return Ok(responseModel);
        }

        /// <summary>
        /// Post method to get the greeting message
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns> response model</returns>
        [HttpPost]
        public IActionResult Post(RequestModel requestModel) {
            if (requestModel == null || string.IsNullOrWhiteSpace(requestModel.Key) || string.IsNullOrWhiteSpace(requestModel.Value))
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Key and Value are required.",
                    Data = null
                });
            }

            if (greetings.ContainsKey(requestModel.Key))
            {
                return Conflict(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Key already exists. Use a different key.",
                    Data = null
                });
            }

            greetings[requestModel.Key] = requestModel.Value;
            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting added successfully.",
                Data = $"Key: {requestModel.Key}, Value: {requestModel.Value}"
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns>Updated greeting message</returns>
        [HttpPut("{key}")]
        public IActionResult Put(string key, string newValue) 
        {
            if (!greetings.ContainsKey(key))
            {
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Greeting not found.",
                    Data = null
                });
            }

            greetings[key] = newValue;
            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting updated successfully.",
                Data = $"Key: {key}, Value: {newValue}"
            });
        
        }

        /// <summary>
        /// Delete method to remove a greeting message
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Deleted greeting message</returns>
        [HttpDelete]
        public IActionResult Delete(string key) 
        {
            if (!greetings.ContainsKey(key))
            {
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Greeting not found.",
                    Data = null
                });
            }

            string removedGreeting = greetings[key];
            greetings.Remove(key);
            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting deleted successfully.",
                Data = $"Key: {key}, Value: {removedGreeting}"
            });
        }

        [HttpPatch("{key}")]
        public IActionResult Patch(string key, string partialUpdate) 
        {
            if (!greetings.ContainsKey(key))
            {
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Greeting not found.",
                    Data = null
                });
            }

            greetings[key] += " " + partialUpdate;
            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting modified successfully.",
                Data = $"Key: {key}, Value: {greetings[key]}"
            });
        }

        /// <summary>
        /// Get a greeting message.
        /// </summary>
        [HttpGet("Greet")]
        public IActionResult GetGreeting()
        {
            var greetingMessage = _greetingBL.GetGreetingMessage(null,null);
            var response = new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting retrieved successfully.",
                Data = greetingMessage
            };

            return Ok(response);
        }

        /// <summary>
        /// Get a greeting message based on user name.
        /// </summary>
        //[HttpGet("personalizedgreet")]
        //public IActionResult GetGreeting([FromQuery] string? firstName=null, [FromQuery] string lastName)
        //{
        //    var greetingMessage = _greetingBL.GetGreetingMessage(firstName, lastName);
        //    var response = new ResponseModel<string>
        //    {
        //        Success = true,
        //        Message = "Greeting retrieved successfully.",
        //        Data = greetingMessage
        //    };

        //    return Ok(response);
        //}

        /// <summary>
        /// Post method to get a greeting using a request body.
        /// </summary>
        [HttpPost("personalizedgreet")]
        public IActionResult PostGreeting([FromBody] GreetingRequestModel requestModel)
        {
            var greetingMessage = _greetingBL.GetPersonalizedGreeting(requestModel);
            var response = new ResponseModel<string>
            {
                Success = true,
                Message = "Personalized greeting created successfully.",
                Data = greetingMessage
            };

            return Ok(response);
        }

        /// <summary>
        /// Post methdd to add greetings
        /// </summary>
        /// <param name="greetingDTO"></param>
        /// <returns>ResponseModel</returns>
        [HttpPost("addgreet")]
        public IActionResult Post([FromBody] GreetingDTO greetingDTO)
        {
            if (greetingDTO == null || string.IsNullOrWhiteSpace(greetingDTO.Key) || string.IsNullOrWhiteSpace(greetingDTO.Value))
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Key and Value are required.",
                    Data = null
                });
            }

            if (!_greetingBL.AddGreeting(greetingDTO))
            {
                return Conflict(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Greeting with this key already exists.",
                    Data = null
                });
            }

            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting added successfully.",
                Data = $"{greetingDTO.Key}:{greetingDTO.Value}"
            });
        }

        [HttpGet("getallgreeting")]
        public IActionResult GetAllGreetings()
        {
            List<GreetingDTO> greetings = _greetingBL.GetAllGreetings();

            return Ok(new ResponseModel<List<GreetingDTO>>
            {
                Success = true,
                Message = "Greetings retrieved successfully.",
                Data = greetings
            });
        }




    }
}
