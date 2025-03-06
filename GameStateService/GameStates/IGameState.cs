public interface IGameState
{
    void Render(); // 화면 출력
    IGameState HandleInput(string input); // 입력 처리 및 상태 전환
}