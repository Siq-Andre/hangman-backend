namespace HangmanBackend.Model
{
    public class GuessLetterResponse
    {
        public string UpdatedWord { get; set; }
        public bool GameWon { get; set; }
        public bool GameLost { get; set; }
        public bool CurrentGuess { get; set; }
    }
}
