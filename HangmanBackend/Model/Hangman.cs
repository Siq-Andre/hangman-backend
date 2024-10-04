using System.Text.Json;

namespace HangmanBackend.Model
{
    public class Hangman
    {
        private static List<Words> words = new List<Words>();
        Random random = new Random();
        public Words SecretWord;
        private string userAnswer = "";
        private int Mistakes = 0;
        private const int MaxMistakes = 7;
        private List<string> GuessedLetters = new List<string>();
        public bool CorrectGuess { get; private set; }
        public bool GameWon { get; private set; }
        public bool GameLost { get; private set; }

        public List<Words> GetWords()
        {
            return words;
        }

        public string GetUserAnswer()
        {
            return userAnswer;
        }

        public void SetUserAnswer(Words SecretWord)
        {
            userAnswer = "";
            for (var i = 0; i < SecretWord.WordName.Length; i++)
            {
                userAnswer += "_";
            }

        }

        public string SetWords()
        {
            string wordsJsonPath = "Data/Words.json";

            if (!System.IO.File.Exists(wordsJsonPath))
            {
                return ("The JSON file was not found.");
            }


            var jsonData = System.IO.File.ReadAllText(wordsJsonPath);

            List<Words> listOfWords = JsonSerializer.Deserialize<List<Words>>(jsonData);

            if (listOfWords == null || listOfWords.Count == 0)
            {
                return ("The JSON file is invalid or contains no words.");
            }

            words = listOfWords;
            return ("The list of words was updated");

        }

        public string InitGame()
        {
            if (words.Count == 0)
            {
                SetWords();
            }

            GameWon = false;
            GameLost = false;

            Random random = new Random();

            int randomID = random.Next(0, words.Count);
            SecretWord = words[randomID];

            SetUserAnswer(SecretWord);

            GuessedLetters.Clear();
            Mistakes = 0;

            return userAnswer;
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
                CorrectGuess = false;
                GameResult();
                return;
            }

            AddLetterToPlayerGuess(letter);
            CorrectGuess = true;
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
