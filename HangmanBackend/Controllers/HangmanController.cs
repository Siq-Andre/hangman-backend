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

        // Iniciar um novo jogo
        [HttpGet("NewGame")]
        public ActionResult<NewGameResponse> NewGame()
        {
            // Cria uma nova instância do jogo da forca
            Hangman hangmanGame = new Hangman();
            hangmanGame.InitGame();

            // Gera um token para o novo jogo
            string token = _gameSessionService.GenerateToken();
            _gameSessionService.StoreToken(token, hangmanGame);

            // Retorna o token e o estado inicial do jogo
            var response = new
            {
                Token = token,
                MaskedWord = hangmanGame.GetUserAnswer(),
                Clue = hangmanGame.SecretWord.Clue
            };

            return Ok(response);
        }

        // Adivinhar uma letra
        [HttpPost("guessLetter")]
        public ActionResult<GuessLetterResponse> LetterGuess([FromBody] string guessedLetter, [FromHeader] string token)
        {
            if (string.IsNullOrEmpty(guessedLetter) || guessedLetter.Length != 1)
            {
                return BadRequest("Please provide a single valid letter.");
            }

            // Valida o token e recupera o estado do jogo
            if (!_gameSessionService.ValidateToken(token))
            {
                return BadRequest("Invalid or expired session token.");
            }

            var hangmanGame = _gameSessionService.GetGameState(token);
            if (hangmanGame == null || string.IsNullOrEmpty(hangmanGame.GetUserAnswer()))
            {
                return BadRequest("The secret word has not been initialized. Please start a new game.");
            }

            // Processa o palpite do jogador
            hangmanGame.GuessLetter(guessedLetter);

            // Atualiza o estado do jogo na sessão
            _gameSessionService.UpdateGameState(token, hangmanGame);

            // Retorna o estado atualizado do jogo
            var response = new
            {
                UpdatedWord = hangmanGame.GetUserAnswer(),
                GameWon = hangmanGame.GameWon,
                GameLost = hangmanGame.GameLost,
                CurrentGuess = hangmanGame.CorrectGuess
            };

            return Ok(response);
        }

        // Verificar o resultado do jogo
        [HttpGet("gameResult")]
        public ActionResult<string> CheckGameResults([FromHeader] string token)
        {
            // Valida o token e recupera o estado do jogo
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