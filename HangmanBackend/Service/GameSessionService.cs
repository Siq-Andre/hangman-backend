using HangmanBackend.Model;

namespace HangmanBackend.Service
{
    public class GameSessionService
    {
        // Dicionário para armazenar tokens e os estados de jogo (Hangman)
        private Dictionary<string, (Hangman GameState, DateTime Expiration)> sessionTokens = new Dictionary<string, (Hangman, DateTime)>();

        // Gera um novo token usando Guid
        public string GenerateToken()
        {
            return Guid.NewGuid().ToString(); // Cria um token único
        }

        // Armazena o token e o estado do jogo
        public void StoreToken(string token, Hangman gameState)
        {
            sessionTokens[token] = (gameState, DateTime.Now.AddHours(1)); // Sessão válida por 1 hora
        }

        // Valida o token para ver se ele ainda é válido
        public bool ValidateToken(string token)
        {
            if (sessionTokens.ContainsKey(token))
            {
                var (_, expiration) = sessionTokens[token];
                if (DateTime.Now <= expiration)
                {
                    return true;
                }
                sessionTokens.Remove(token); // Remove se expirou
            }
            return false;
        }

        // Retorna o estado do jogo associado ao token
        public Hangman GetGameState(string token)
        {
            if (ValidateToken(token))
            {
                return sessionTokens[token].GameState;
            }
            return null;
        }

        // Atualiza o estado do jogo e reseta a expiração
        public void UpdateGameState(string token, Hangman gameState)
        {
            if (ValidateToken(token))
            {
                sessionTokens[token] = (gameState, DateTime.Now.AddHours(1));
            }
        }
    }
}
