using ArsalanAssesment.Web.DTOs;
using ArsalanAssesment.Web.DTOs.UserDTOs;
using ArsalanAssesment.Web.Helper;
using ArsalanAssesment.Web.Repository.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ArsalanAssesment.Web.Repository
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserAuthRepository> _logger;
        private readonly IMapper _mapper;

        public UserAuthRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<UserAuthRepository> logger, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseDTO> Login(LoginUserDTO loginUserDTO)
        {
            try
            {
                // Check if email or password is null
                if (string.IsNullOrEmpty(loginUserDTO?.Email) || string.IsNullOrEmpty(loginUserDTO?.Password))
                {
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.InvalidLoginDetails);
                }

                // Check User by email
                var enteredUser = await _userManager.FindByEmailAsync(loginUserDTO.Email);

                if (enteredUser == null)
                {
                    //if User is not found, We return a reponse with Message.
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.InvalidLoginDetails);
                }

                if (!await _userManager.CheckPasswordAsync(enteredUser, loginUserDTO.Password))
                {
                    //if Password is invalid, We return a reponse with Message.
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.InvalidLoginDetails);
                }

                // Get User Guid
                Guid userId = Guid.Parse(enteredUser.Id);

                // Fetch the roles for the user
                List<string> rolesList = (await _userManager.GetRolesAsync(enteredUser)).ToList();

                string token = GenerateTokenString(loginUserDTO.Email, userId, rolesList);

                return ResponseHelper.CreateResponse(true, false, ResponseMessages.UserLoggedIn, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Exception occured");

                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }

        }

        public async Task<ResponseDTO> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            try
            {
                // Split the roles by commas and trim whitespace
                var roles = registerUserDTO.Role.Split(',').Select(r => r.Trim().ToLower()).ToList();

                // Ensure each role exists in the database
                foreach (var role in roles)
                {
                    IdentityRole? roleFromDB = await _roleManager.FindByNameAsync(role);

                    if (roleFromDB is null)
                    {
                        // Create new role if it does not exist
                        var newRole = new IdentityRole(role);
                        var roleResult = await _roleManager.CreateAsync(newRole);

                        if (!roleResult.Succeeded)
                        {
                            return ResponseHelper.CreateResponse(true, false, ResponseMessages.RoleCreationFailed, roleResult.Errors);
                        }
                    }
                }

                // Check if the user by email already exists
                var user = await _userManager.FindByEmailAsync(registerUserDTO.Email);

                if (user is not null)
                {
                    // If the user is found, return a response indicating the user already exists
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.UserAlreadyExists);
                }

                // If the user is not found, create a new one
                IdentityUser newUser = new IdentityUser()
                {
                    Email = registerUserDTO.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registerUserDTO.Username,
                    NormalizedUserName = registerUserDTO.Name
                };

                var createUserResult = await _userManager.CreateAsync(newUser, registerUserDTO.Password);

                if (!createUserResult.Succeeded)
                {
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.UserRegistrationFailed, createUserResult.Errors);
                }

                // Assign all roles to the new user
                foreach (var role in roles)
                {
                    var roleAssignResult = await _userManager.AddToRoleAsync(newUser, role);

                    if (!roleAssignResult.Succeeded)
                    {
                        return ResponseHelper.CreateResponse(true, false, ResponseMessages.RoleAssignmentFailed, roleAssignResult.Errors);
                    }
                }

                // Map the new user to UserDetailsDTO
                UserDetailsDTO userDetailsDTO = _mapper.Map<UserDetailsDTO>(newUser);
                userDetailsDTO.Roles = string.Join(", ", roles);

                return ResponseHelper.CreateResponse(true, false, ResponseMessages.UserRegistered, userDetailsDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Exception occurred");

                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }


        public async Task<ResponseDTO> UpdateUser(Guid id, UpdateUserDTO updateUserDTO)
        {
            try
            {
                // Convert Guid to string 
                string userId = id.ToString();

                // Find the user by Id
                IdentityUser? user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    // Handle case when user is not found
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.NoUsersFound);
                }

                // Split the roles by commas and trim whitespace
                var rolesToUpdate = updateUserDTO.Roles.Split(',').Select(r => r.Trim().ToLower()).ToList();

                // Fetch the current roles of the user
                var currentRoles = await _userManager.GetRolesAsync(user);

                foreach (var role in rolesToUpdate)
                {
                    // Check if the role exists in the database
                    var roleInDb = await _roleManager.FindByNameAsync(role);

                    if (roleInDb == null)
                    {
                        // Return a response if the role does not exist in the database
                        return ResponseHelper.CreateResponse(true, false, ResponseMessages.RoleNotFound, role);
                    }

                    /*  If the role is not already assigned to the user, add it
                        If the role is already assigned, do nothing */

                    if (!currentRoles.Contains(role))
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(user, role);

                        if (!addRoleResult.Succeeded)
                        {
                            return ResponseHelper.CreateResponse(true, false, ResponseMessages.RoleAssignmentFailed, addRoleResult.Errors);
                        }
                    }
                }

                // Update user details as per the updateUserDTO
                user.UserName = updateUserDTO.Username;
                user.Email = updateUserDTO.Email;

                // Update the user in the database
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    // Handle case when updating the user fails
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.UserUpdateFailed, updateResult.Errors);
                }

                // Map the updated user to UserDetailsDTO
                UserDetailsDTO userDetailsDTO = _mapper.Map<UserDetailsDTO>(user);
                userDetailsDTO.Roles = string.Join(", ", rolesToUpdate);

                // Return a success response with the updated user details
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.UserUpdated, userDetailsDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Exception occurred");

                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }


        public async Task<ResponseDTO> DeleteUser(Guid id)
        {
            try
            {
                // Convert Guid to string and find the user by Id
                string userId = id.ToString();
                IdentityUser? user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    // Return response if user not found
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.InvalidLoginDetails);
                }

                // Delete the user
                var deleteResult = await _userManager.DeleteAsync(user);

                if (!deleteResult.Succeeded)
                {
                    // Handle case when deleting the user fails
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.UserDeletionFailed, deleteResult.Errors);
                }

                // Return success response
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.UserDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Exception occurred");

                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }

        public async Task<ResponseDTO> GetAllUser()
        {

            try
            {
                // Retrieve all users
                var users = await _userManager.Users.ToListAsync();

                if (!users.Any())
                {
                    // Return response if no users found
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.NoUsersFound);
                }

                // Return success response with the list of users
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Successful, users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Exception occurred");

                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }

        public async Task<ResponseDTO> GetUser(Guid id)
        {
            try
            {
                // Convert Guid to string and find the user by Id
                string userId = id.ToString();
                IdentityUser? user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    // Return response if user not found
                    return ResponseHelper.CreateResponse(true, false, ResponseMessages.InvalidLoginDetails);
                }

                UserDetailsDTO userDetailsDTO = _mapper.Map<UserDetailsDTO>(user);

                // Fetch the roles for the user
                var roles = await _userManager.GetRolesAsync(user);

                // If the user has roles, assign the first role or concatenate all roles
                userDetailsDTO.Roles = roles.Any() ? string.Join(", ", roles) : "No roles assigned";

                // Return success response with the user information
                return ResponseHelper.CreateResponse(true, false, ResponseMessages.Successful, userDetailsDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Exception occurred");

                return ResponseHelper.CreateResponse(false, true, ResponseMessages.ExceptionMessage);
            }
        }


        /// <summary>
        /// This function generates the respective JWT Token for user email and role.
        /// </summary>
        /// <param name="userEmail"> The email of logged in user</param>
        /// <param name="roles"> The roles of logged in user</param>
        /// <returns></returns>
        public string GenerateTokenString(string userEmail, Guid userGuid, List<string> roles)
        {
            // Create claims for the user's email and roles
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userEmail),
                new Claim("UserGuid", userGuid.ToString()) // Adding the GUID as a custom claim
            };

            // Add a separate claim for each role
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Key").Value!));
            SigningCredentials signingcreds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value,
                signingCredentials: signingcreds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return tokenString;
        }


    }
}
