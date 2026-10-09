namespace my_first_service
{
    public class Greeting
    {

        public long Id
        {
            get; set;
        }

        public required String Name
        {
            get; set;
        }

        public required String Message
        {
            get; set;
        }
    }

    public class GreetingReq
    {
        public required String Name
        {
            get; set;
        }
        public required String Message
        {
            get; set;
        }
    }
}