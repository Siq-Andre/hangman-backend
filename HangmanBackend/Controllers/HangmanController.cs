using HangmanBackend.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HangmanBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangmanController : ControllerBase
    {
        private static Hangman hangmanGame = new Hangman();

        [HttpGet]
        public ActionResult<List<Words>> GetAllWords()
        {
            return Ok(hangmanGame.GetWords());
        }

        [HttpGet("NewGame")]
        public ActionResult<string> NewGame()
        {
            hangmanGame.InitGame();

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
