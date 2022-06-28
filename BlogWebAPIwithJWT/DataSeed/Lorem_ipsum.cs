namespace BlogWithJWT.DataSeed {

    public class LoremIpsum
    {
        public int id { get; set; }
        public string uid { get; set; }
        public string word { get; set; }
        public List<string> words { get; set; }
        public string characters { get; set; }
        public string short_sentence { get; set; }
        public string long_sentence { get; set; }
        public string very_long_sentence { get; set; }
        public List<string> paragraphs { get; set; }
        public string question { get; set; }
        public List<string> questions { get; set; }
    }
}
