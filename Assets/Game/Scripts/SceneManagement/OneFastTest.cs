namespace SceneManagement
{
    public class OneFastTest
    {
        public OneFastTest(int id, string questionText, string answer1, string answer2, string answer3, string answer4,
            int correctAnswerIndex, int testIndex) // TODO rename args
        {
            Id = id;
            QuestionText = questionText;
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            CorrectAnswerIndex = correctAnswerIndex;
            TestIndex = testIndex;
        }

        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public int TestIndex { get; set; }
    }
}