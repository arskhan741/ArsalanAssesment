namespace ArsalanAssesment.Web.Helper
{
    public static class ResponseMessages
    {
        // Sales Endpoints Messages
        public static string Successful { get; set; } = "Successful";
        public static string Added { get; set; } = "Added Successfully";
        public static string Modified { get; set; } = "Modified Successfully";
        public static string Deleted { get; set; } = "Deleted Successfully";
        public static string NotFound { get; set; } = "Record NotFound";
        public static string ExceptionMessage { get; set; } = "Unknown Error occured";
        public static string InvalidDates { get; set; } = "Starting Date Should be Greater then Ending Date";
        public static string InvalidData { get; set; } = "Please Enter correct Date range or Representative Id";

        // Auth Endpoints Messages
        public static string InvalidLoginDetails { get; set; } = "Invalid Credentials please enter correct credentials";
        public static string UserAlreadyExists { get; set; } = "User with current email already exists";
        public static string UserLoggedIn { get; set; } = "User Logged In";
        public static string UserRegistered { get; set; } = "User Registered";
        public static string UserRegistrationFailed { get; set; } = "User Registration Failed";
        public static string RoleAssignmentFailed { get; set; } = "Role Cannot be added to user";
        public static string UserUpdateFailed { get; set; } = "User Update Failed";
        public static string UserUpdated { get; set; } = "User Updated";
        public static string UserDeletionFailed { get; internal set; } = "User Deletion Failed";
        public static string UserDeleted { get; internal set; } = "User Deleted";
        public static string NoUsersFound { get; internal set; } = "No User registered Currently";
        public static string RoleUpdateFailed { get; internal set; } = "Role Update Failed";
        public static string RoleCreationFailed { get; internal set; } = "Failed to craete role";
        public static string RoleNotFound { get; internal set; } = "Roles does not exist";

        // Dash board Metrics
        public static string NoSalesFound { get; internal set; } = "No sales found, please make some sales";
    }
}
