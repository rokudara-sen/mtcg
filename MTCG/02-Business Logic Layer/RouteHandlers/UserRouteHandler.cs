using MTCG._02_Business_Logic_Layer.Interfaces;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;

namespace MTCG._02_Business_Logic_Layer.RouteHandlers
{
    public class UserRouteHandler : IRouteHandler
    {
        private readonly IUserRepository _userRepository;

        // Constructor Injection
        public UserRouteHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public OperationResult RegisterUser(UserCredentials? credentials)
        {
            var validation = ValidateCredentials(credentials);
            if (!validation.Success)
            {
                return validation;
            }

            if (GetUserByUsername(credentials.Username) != null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Username is already taken." };
            }

            credentials.Password = HashPassword(credentials.Password);

            var newUser = new User(credentials.Username, credentials.Password);
            _userRepository.AddUser(newUser);

            return new OperationResult { Success = true };
        }

        public OperationResult LoginUser(UserCredentials? credentials)
        {
            var validation = ValidateCredentials(credentials);
            if (!validation.Success)
            {
                return validation;
            }
            var user = GetUserByUsername(credentials.Username);
            if (user == null || !VerifyPassword(credentials.Password, user.Password))
            {
                return new OperationResult { Success = false, ErrorMessage = "Invalid username or password." };
            }

            if (!IsValidUser(user))
            {
                return new OperationResult { Success = false, ErrorMessage = "User data is invalid." };
            }

            user.Authorization = GenerateAuthToken(user.Username);
            _userRepository.UpdateUser(user);

            return new OperationResult { Success = true, Data = user.Authorization };
        }

        public User? GetUserByAuthToken(string? authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                return null;
            }
            return _userRepository.GetUserByAuthToken(authToken);
        }

        public OperationResult UpdateUserByAuthToken(string authToken, User updatedUser)
        {
            var existingUser = GetUserByAuthToken(authToken);
            if (existingUser == null)
                return new OperationResult { Success = false, ErrorMessage = "Invalid auth token." };

            UpdateUserDetails(existingUser, updatedUser);
            _userRepository.UpdateUser(existingUser);

            return new OperationResult { Success = true };
        }

        private User? GetUserByUsername(string? username)
        {
            return string.IsNullOrWhiteSpace(username) ? null : _userRepository.GetUserByUsername(username);
        }

        private static void UpdateUserDetails(User existingUser, User updatedUser)
        {
            existingUser.Elo = updatedUser.Elo;
            existingUser.Gold = updatedUser.Gold;
            existingUser.Wins = updatedUser.Wins;
            existingUser.Losses = updatedUser.Losses;
        }

        private string GenerateAuthToken(string username)
        {
            return $"{username}-mtcgToken";
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public bool IsValidUser(User? user)
        {
            return user != null &&
                   !string.IsNullOrWhiteSpace(user.Username) &&
                   !string.IsNullOrWhiteSpace(user.Password) &&
                   user is { Elo: >= 0, Gold: >= 0, Wins: >= 0, Losses: >= 0 };
        }

        private static OperationResult ValidateCredentials(UserCredentials? credentials)
        {
            if (credentials == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Invalid credentials." };
            }
            if (string.IsNullOrWhiteSpace(credentials.Username))
            {
                return new OperationResult { Success = false, ErrorMessage = "Username cannot be empty." };
            }
            if (string.IsNullOrWhiteSpace(credentials.Password))
            {
                return new OperationResult { Success = false, ErrorMessage = "Password cannot be empty." };
            }
            return new OperationResult { Success = true };
        }
    }
}
