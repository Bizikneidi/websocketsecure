namespace WebsocketSecure_Server.Entities
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

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