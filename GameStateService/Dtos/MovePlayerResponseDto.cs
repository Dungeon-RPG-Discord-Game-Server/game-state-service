namespace GameStateService.Dtos
{
    public class MovePlayerResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
    }
}