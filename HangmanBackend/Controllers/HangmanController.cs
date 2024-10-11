using HangmanBackend.Model;
using HangmanBackend.Service;
using Microsoft.AspNetCore.Mvc;

namespace HangmanBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangmanController : ControllerBase
    {
        private readonly GameSessionService _gameSessionService;

        public HangmanController(GameSessionService gameSessionService)
        {
            _gameSessionService = gameSessionService;
        }

        [HttpGet("NewGame")]
        public ActionResult<NewGameResponse> NewGame()
        {
            _gameSessionService.RemoveExpiredTokens();

            var hangmanGame = new Hangman();
            hangmanGame.InitGame();

            var token = _gameSessionService.GenerateToken();
            _gameSessionService.StoreToken(token, hangmanGame);

            var response = new NewGameResponse();

            response.MaskedWord = hangmanGame.GetUserAnswer();
            response.Clue = hangmanGame.SecretWord.Clue;
            response.Token = token;

            return Ok(response);
        }

        [HttpPost("GuessLetter")]
        public ActionResult<GuessLetterResponse> LetterGuess([FromBody] string guessedLetter, [FromHeader] string token)
        {
            if (string.IsNullOrEmpty(guessedLetter) || guessedLetter.Length != 1)
            {
                return BadRequest("Please provide a single valid letter.");
            }

            if (!_gameSessionService.ValidateToken(token))
            {
                return BadRequest("Invalid or expired session token.");
            }

            var hangmanGame = _gameSessionService.GetGameState(token);
            if (hangmanGame == null || string.IsNullOrEmpty(hangmanGame.GetUserAnswer()))
            {
                return BadRequest("The secret word has not been initialized. Please start a new game.");
            }

            hangmanGame.GuessLetter(guessedLetter);

            _gameSessionService.UpdateGameState(token, hangmanGame);

            var response = new GuessLetterResponse();


            response.UpdatedWord = hangmanGame.GetUserAnswer();
            response.GameWon = hangmanGame.GameWon;
            response.GameLost = hangmanGame.GameLost;
            response.CurrentGuess = hangmanGame.CorrectGuess;
            

            return Ok(response);
        }

        [HttpGet("gameResult")]
        public ActionResult<string> CheckGameResults([FromHeader] string token)
        {
            if (!_gameSessionService.ValidateToken(token))
            {
                return BadRequest("Invalid or expired session token.");
            }

            var hangmanGame = _gameSessionService.GetGameState(token);

            if (hangmanGame == null)
            {
                return BadRequest("No active game session found.");
            }

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
                return Ok("The game is still going.");
            }
        }
    }
}