namespace WebsocketSecure_Server.Entities
{
    public class User
    {
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }
        private string Password { get; }

        public override bool Equals(object other)
        {
            if(other is User user)
                return string.Equals(Username.ToLower(), user.Username.ToLower()) && string.Equals(Password, user.Password);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Username != null ? Username.GetHashCode() : 0) * 397) ^ (Password != null ? Password.GetHashCode() : 0);
            }
        }
    }
}