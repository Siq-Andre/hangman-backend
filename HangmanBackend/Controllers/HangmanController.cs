using HangmanBackend.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HangmanBackend.Controllers
{
    public class HangmanController : Controller
    {
        [Route("api/[controller]")]
        [ApiController]
        public class WordsController : ControllerBase
        {
            private static Hangman hangmanGame = new Hangman();

            [HttpPost("addWords")]
            public IActionResult PostAllWordsFromJson()
            {
                string wordsJsonPath = "Data/Words.json";

                if (!System.IO.File.Exists(wordsJsonPath))
                {
                    return NotFound("The JSON file was not found.");
                }

                try
                {
                    var jsonData = System.IO.File.ReadAllText(wordsJsonPath);

                    List<Words> listOfWords = JsonSerializer.Deserialize<List<Words>>(jsonData);

                    if (listOfWords == null || listOfWords.Count == 0)
                    {
                        return BadRequest("The JSON file is invalid or contains no words.");
                    }

                    hangmanGame.SetWords(listOfWords);
                    return Ok("Words list updated from JSON file.");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error processing the JSON file: {ex.Message}");
                }
            }

            [HttpGet]
            public ActionResult<List<Words>> GetAllWords()
            {
                return Ok(hangmanGame.GetWords());
            }

            [HttpGet("getRandom")]
            public ActionResult<string> GetRandomWord()
            {
                hangmanGame.SetSecretWord();
                string maskedWord = hangmanGame.GetUserAnswer();
                return Ok(maskedWord);
            }

            [HttpPost("guessLetter")]
            public ActionResult<string> LetterGuess([FromBody] string guessedLetter)
            {
                if (string.IsNullOrEmpty(guessedLetter) || guessedLetter.Length != 1)
                {
                    return BadRequest("Please provide a single valid letter.");
                }

                if (string.IsNullOrEmpty(hangmanGame.GetUserAnswer()))
                {
                    return BadRequest("The secret word has not been initialized. Please start a new game.");
                }

                hangmanGame.GuessLetter(guessedLetter);
                string currentStateOfWord = hangmanGame.GetUserAnswer();

                return Ok(currentStateOfWord);
            }

            [HttpGet("gameResult")]
            public ActionResult<string> CheckGameResults()
            {
                if (hangmanGame.GameWon)
                {
                    return Ok("You won the game!");
                }

                else if (hangmanGame.GameLost)
                {
                    return Ok("You lost the game!");
                }

                else
                {
                    return Ok("The game is still going");
                }
            }
        }
}
