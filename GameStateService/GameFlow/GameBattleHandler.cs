using GameStateService.Models;
using GameStateService.Utils;
using GameStateService.Services;

using GameStateService.Dtos;

public class GameBattleHandler
{
    private readonly MemoryCacheService _memoryCacheService;
    private readonly GameFlowManager _gameFlowManager;

    public GameBattleHandler(MemoryCacheService memoryCacheService, GameFlowManager gameFlowManager)
    {
        _memoryCacheService = memoryCacheService;
        _gameFlowManager = gameFlowManager;
    }

    public async Task<bool> MonsterExistsAsync(string userId)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new UserErrorException($"Player: {userId} data not found.");
        }
        int CurrentRoomId = playerData.CurrentMapData.CurrentRoom;
        var currentRoom = playerData.CurrentMapData.Rooms[CurrentRoomId];
        var monster = currentRoom.Monster;
        return monster != null; // Return true if monster exists
    }

    public async Task<bool> BossRoomClearedAsync(string userId)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new UserErrorException($"Player: {userId} data not found.");
        }
        var bossRoom = playerData.CurrentMapData.Rooms[playerData.CurrentMapData.Rooms.Count - 1];
        var boss = bossRoom.Monster;
        return boss == null; // Monster still alive
    }

    public async Task<string> GetBattleSummaryAsync(string userId)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new UserErrorException($"Player: {userId} data not found.");
        }
        int CurrentRoomId = playerData.CurrentMapData.CurrentRoom;
        var currentRoom = playerData.CurrentMapData.Rooms[CurrentRoomId];
        var monster = currentRoom.Monster;
        if (monster == null)
        {
            return "No monster in the current room.";
        }
        string battleMessage = $@"
        **👤 You**
        ❤️HP: {playerData.Health} / {playerData.MaxHealth}
        🔵MP: {playerData.Mana} / {playerData.MaxMana}

        **🐉 {monster.Name}**
        ❤️HP: {monster.Health} / {monster.MaxHealth}
        ".Trim();

        // Assuming playerData has a method to get battle summary
        return battleMessage;
    }

    public async Task<string> AttackMonsterAsync(string userId, bool skillUsed = false)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new UserErrorException($"Player: {userId} data not found.");
        }
        int CurrentRoomId = playerData.CurrentMapData.CurrentRoom;
        var currentRoom = playerData.CurrentMapData.Rooms[CurrentRoomId];
        var monster = currentRoom.Monster;
        var reward = currentRoom.Reward;

        string attackMessage;

        if (monster == null)
        {
            return "😌 The room is quiet. No monsters here...";
        }

        // Perform attack logic here
        // For example, reduce monster's health by player's attack power
        if (skillUsed)
        {
            // Assuming playerData.Weapon has a special skill
            playerData.Mana -= playerData.Weapon.Skill.ManaCost;
            monster.Health = Math.Max(monster.Health - playerData.Weapon.Skill.Damage, 0);
            attackMessage = $"✨ You unleashed a powerful skill on 🐉 **{monster.Name}** and dealt **{playerData.Weapon.Skill.Damage}** damage!";
        }
        else
        {
            // Normal attack
            playerData.Mana -= playerData.Weapon.ManaCost;
            monster.Health = Math.Max(monster.Health - playerData.Weapon.AttackPower, 0);
            attackMessage = $"🗡️ You attacked the 🐉 **{monster.Name}** and dealt **{playerData.Weapon.AttackPower}** damage!";
        }

        if (monster.Health <= 0)
        {
            int playerLevel = playerData.Level;
            playerData.ApplyReward(reward);

            string message = $@"
            💥 You defeated the 🐉 **{monster.Name}**!

            🎉 You gained:
            ❤️ +{reward.Health} HP
            🔵 +{reward.Mana} MP
            🌟 +{reward.Experience} EXP
            ".Trim();

            string levelUpMessage = $@"
            🎉 **Congratulations, {playerData.UserName}!**
            You've leveled up from **Lv. {playerLevel}** to **Lv. {playerData.Level}**!";

            if (playerData.Level > playerLevel)
            {
                message += levelUpMessage;
            }

            currentRoom.Monster = null; // Monster defeated
            currentRoom.Reward = null; // Clear reward after collection
            await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));
            await _gameFlowManager.ChangeGameStateAsync(userId, GameStateType.ExplorationState);
            return message;
        }

        await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));

        return attackMessage;
    }

    public async Task<string> MonsterAttackAsync(string userId)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new UserErrorException($"Player: {userId} data not found.");
        }
        int CurrentRoomId = playerData.CurrentMapData.CurrentRoom;
        var currentRoom = playerData.CurrentMapData.Rooms[CurrentRoomId];
        var monster = currentRoom.Monster;
        if (monster == null)
        {
            return "😌 The room is quiet. No monsters here...";
        }

        // Perform monster attack logic here
        // For example, reduce player's health by monster's attack power
        playerData.Health -= monster.Attack;

        if (playerData.Health <= 0)
        {
            string message = $@"
            💢 The 🐉 **{monster.Name}** attacked you and dealt **{monster.Attack}** damage!
            ☠️ **You have been defeated...**

            Your HP has dropped to 0.  
            The adventure ends here... for now.
            ".Trim();
            await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));
            await _gameFlowManager.ChangeGameStateAsync(userId, GameStateType.MainMenuState);
            return message;
        }

        await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));

        return $"💢 The 🐉 **{monster.Name}** attacked you and dealt **{monster.Attack}** damage!";
    }

    public async Task<BattleEscapeResultDto> RunAwayAsync(string userId)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        var battleEscapeResult = new BattleEscapeResultDto();
        if (playerData == null)
        {
            throw new UserErrorException($"Player: {userId} data not found.");
        }
        int CurrentRoomId = playerData.CurrentMapData.CurrentRoom;
        var currentRoom = playerData.CurrentMapData.Rooms[CurrentRoomId];
        var monster = currentRoom.Monster;
        if (monster == null)
        {
            battleEscapeResult.IsEscaped = true;
            battleEscapeResult.Message = "😌 The room is quiet. No monsters here...";
            return battleEscapeResult;
        }

        bool escaped = RandomProvider.FiftyFiftyChance();
        if (currentRoom.RoomType == RoomType.Boss)
        {
            battleEscapeResult.IsEscaped = false;
            battleEscapeResult.Message = "❌ You cannot escape from the boss!";
            return battleEscapeResult;
        }

        if (escaped)
        {
            string message = $@"
            🏃 You successfully ran away from the 🐉 **{monster.Name}**!
            ".Trim();
            await _gameFlowManager.ChangeGameStateAsync(userId, GameStateType.ExplorationState);
            battleEscapeResult.IsEscaped = true;
            battleEscapeResult.Message = message;
            return battleEscapeResult;
        }

        battleEscapeResult.IsEscaped = false;
        battleEscapeResult.Message =  $"💢 You couldn't escape! The 🐉 **{monster.Name}** is still chasing you!";
        return battleEscapeResult;
    }
}