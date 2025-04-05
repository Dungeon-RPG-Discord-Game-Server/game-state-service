namespace GameStateService.Dtos
{
    public class RegisterPlayerRequestDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public int WeaponType { get; set; }
    }
}
