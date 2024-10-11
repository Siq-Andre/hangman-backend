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
        public ActionResult<GuessLetterResponse> LetterGuess([FromBody] GuessLetterRequest guessedLetter, [FromHeader] string token)
        {
            if (string.IsNullOrEmpty(guessedLetter.Letter) || guessedLetter.Letter.Length != 1)
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

            hangmanGame.GuessLetter(guessedLetter.Letter);

            _gameSessionService.UpdateGameState(token, hangmanGame);

            var response = new GuessLetterResponse();


            response.UpdatedWord = hangmanGame.GetUserAnswer();
            response.GameWon = hangmanGame.GameWon;
            response.GameLost = hangmanGame.GameLost;
            response.CurrentGuess = hangmanGame.CorrectGuess;
            

            return Ok(response);
        }

    }
}