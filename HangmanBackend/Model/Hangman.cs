namespace HangmanBackend.Model
{
    public class Hangman
    {
        private static List<Words> words = new List<Words>();
        Random random = new Random();
        private Words SecretWord;
        private string userAnswer = "";
        private int Mistakes = 0;
        private const int MaxMistakes = 6;
        private List<string> GuessedLetters = new List<string>();
        public bool GameWon { get; private set; }
        public bool GameLost { get; private set; }

        public void SetWords(List<Words> possibleWords)
        {
            words = possibleWords;
        }

        public List<Words> GetWords()
        {
            return words;
        }

        public string GetUserAnswer()
        {
            return userAnswer;
        }

        public void SetSecretWord()
        {
            userAnswer = "";
            GameWon = false;
            GameLost = false;

            int randomID = random.Next(0, words.Count);
            SecretWord = words[randomID];

            for (var i = 0; i < SecretWord.WordName.Length; i++)
            {
                userAnswer += "*";
            }

            GuessedLetters.Clear();
            Mistakes = 0;
        }

        public bool CheckUsedLetters(string letter)
        {
            return GuessedLetters.Contains(letter.ToUpper());
        }

        public string AddLetterToPlayerGuess(string guessedLetter)
        {
            string userAnswerHelper = "";
            string upperGuessedLetter = guessedLetter.ToUpper();

            for (int i = 0; i < SecretWord.WordName.Length; i++)
            {
                if (SecretWord.WordName[i].ToString().ToUpper() == upperGuessedLetter)
                {
                    userAnswerHelper += upperGuessedLetter;
                }
                else
                {
                    userAnswerHelper += userAnswer[i];
                }
            }
            userAnswer = userAnswerHelper;
            return userAnswer;
        }

        public void GuessLetter(string letter)
        {
            letter = letter.ToUpper();

            if (CheckUsedLetters(letter))
            {
                return;
            }

            GuessedLetters.Add(letter);

            if (!SecretWord.WordName.Contains(letter))
            {
                Mistakes += 1;
                GameResult();
                return;
            }

            AddLetterToPlayerGuess(letter);
            GameResult();

        }

        public void GameResult()
        {
            if (SecretWord.WordName == userAnswer)
            {
                GameWon = true;
            }

            if (Mistakes == MaxMistakes)
            {
                GameLost = true;
            }
        }
    }
}
