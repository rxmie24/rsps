namespace RS2.Server.player
{
    internal class LoginDetails
    {
        private string username;
        private long longName;
        private string password;

        public long getLongName()
        {
            return longName;
        }

        public void setLongName(long longName)
        {
            this.longName = longName;
        }

        public string getUsername()
        {
            return username;
        }

        public void setUsername(string username)
        {
            this.username = username;
        }

        public string getPassword()
        {
            return password;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }
    }
}