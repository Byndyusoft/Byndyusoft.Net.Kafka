namespace Byndyusoft.Net.Kafka
{
    public class KafkaSettings
    {
        public string[] Hosts { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        
        public string Prefix { get; set; }
        
        //TODO Дать более понятное название
        public string ServiceName { get; set; }
    }
}