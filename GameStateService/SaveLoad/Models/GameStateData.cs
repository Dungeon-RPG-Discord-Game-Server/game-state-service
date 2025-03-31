namespace GameStateService.Models
{
    public enum GameStateType{
        MainMenuState,
        ExplorationState,
        BattleState
    }

    public class GameStateData
    {
        public string PlayerId { get; set; }
        public GameStateType CurrentGameState { get; set; }
        public Room CurrentRoom { get; set; }
    }
}