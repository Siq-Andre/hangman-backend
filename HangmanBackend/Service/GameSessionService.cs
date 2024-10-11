using HangmanBackend.Model;

namespace HangmanBackend.Service
{
    public class GameSessionService
    {
        private Dictionary<string, (Hangman GameState, DateTime Expiration)> sessionTokens = new Dictionary<string, (Hangman, DateTime)>();

        public string GenerateToken()
        {
            var token = Guid.NewGuid().ToString();
            while (sessionTokens.ContainsKey(token))
            {
                token = Guid.NewGuid().ToString();
            }

            return token;
        }

        public void StoreToken(string token, Hangman gameState)
        {
            sessionTokens[token] = (gameState, DateTime.Now.AddHours(1)); 
        }

        public bool ValidateToken(string token)
        {
            if (sessionTokens.ContainsKey(token))
            {
                var (_, expiration) = sessionTokens[token];
                if (DateTime.Now <= expiration)
                {
                    return true;
                }
                sessionTokens.Remove(token);
            }
            return false;
        }

        public Hangman GetGameState(string token)
        {
            if (ValidateToken(token))
            {
                return sessionTokens[token].GameState;
            }
            return null;
        }


        public void RemoveExpiredTokens()
        {
            var now = DateTime.Now;

            var expiredTokens = sessionTokens
                .Where(token => token.Value.Expiration < now)
                .Select(token => token.Key)
                .ToList();

            foreach (var token in expiredTokens)
            {
                sessionTokens.Remove(token);
            }
        }

        public void UpdateGameState(string token, Hangman gameState)
        {
            if (ValidateToken(token))
            {
                sessionTokens[token] = (gameState, DateTime.Now.AddHours(1));
            }
        }
    }
}
