﻿using Microsoft.AspNetCore.SignalR;
using SpinBladeArena.Hubs;
using System.Diagnostics;
using System.Numerics;

namespace SpinBladeArena.LogicCenter;

public record Lobby(int Id, int CreateUserId, DateTime CreateTime, IHubContext<GameHub, IGameHubClient> Hub)
{
    public Vector2 MaxSize = new(1000, 1000);
    public Player[] Players = [];
    // Key: UserId, Value: User index in Players
    public Dictionary<int, int> PlayerIdMap = [];
    public PickableBonus[] PickableBonuses = [];
    public Player[] DeadPlayers = [];
    private CancellationTokenSource? _cancellationTokenSource = null;
    // Key: UserId
    private Dictionary<int, AddPlayerRequest> _addPlayerRequests = [];

    public void AddPlayerToRandomPosition(AddPlayerRequest req)
    {
        if (PlayerIdMap.TryGetValue(req.UserId, out int userIndex))
        {
            ref Player player = ref Players[userIndex];
            player.ConnectionId = req.ConnectionId;
        }
        else
        {
            _addPlayerRequests[req.UserId] = req;
        }
    }

    public Vector2 RandomPosition()
    {
        return new(Random.Shared.NextSingle() * MaxSize.X - MaxSize.Y / 2, Random.Shared.NextSingle() * MaxSize.Y - MaxSize.Y / 2);
    }

    public void AddPickableBonus(Vector2 position)
    {
        Array.Resize(ref PickableBonuses, PickableBonuses.Length + 1);
        PickableBonuses[^1] = PickableBonus.CreateRandom(position);
    }

    public void EnsureStart()
    {
        if (_cancellationTokenSource == null)
        {
            _cancellationTokenSource = new();
            new Thread(() => Run(_cancellationTokenSource.Token)).Start();
        }
    }

    public void Terminate() => _cancellationTokenSource?.Cancel();

    public void Run(CancellationToken cancellationToken)
    {
        const int DeadRespawnTimeInSeconds = 5;
        float bonusSpawnCooldown = 5;
        float maxBonusCount = 7;
        float bonusSpawnTimer = 0;

        Stopwatch sw = Stopwatch.StartNew();
        float oldTime = 0;
        float deltaTime = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            Thread.Sleep(Math.Max(1, (int)(30 - deltaTime)));
            deltaTime = MathF.Min((float)sw.Elapsed.TotalSeconds - oldTime, 0.25f);
            oldTime = (float)sw.Elapsed.TotalSeconds;

            // handle add player requests
            {
                int playerIndex = Players.Length;
                Array.Resize(ref Players, Players.Length + _addPlayerRequests.Count);
                foreach (AddPlayerRequest req in _addPlayerRequests.Values)
                {
                    Players[playerIndex] = new Player(req.UserId, req.UserName, req.ConnectionId, RandomPosition());
                    PlayerIdMap[req.UserId] = playerIndex;
                    playerIndex++;
                }
                _addPlayerRequests.Clear();
            }

            // handle move
            for (int i = 0; i < Players.Length; ++i)
            {
                ref Player player = ref Players[i];
                player.Move(deltaTime);
            }

            // handle bonus
            for (int i = 0; i < Players.Length; ++i)
            {
                ref Player player = ref Players[i];
                for (int j = 0; j < PickableBonuses.Length; ++j)
                {
                    ref PickableBonus bonus = ref PickableBonuses[j];
                    if (Vector2.Distance(player.Position, bonus.Position) < player.Size)
                    {
                        bonus.Apply(ref player);
                        Array.Copy(PickableBonuses, j + 1, PickableBonuses, j, PickableBonuses.Length - j - 1);
                        Array.Resize(ref PickableBonuses, PickableBonuses.Length - 1);
                        break;
                    }
                }
            }

            // handle attack
            for (int i = 0; i < Players.Length - 1; ++i)
            {
                ref Player p1 = ref Players[i];
                for (int j = i + 1; j < Players.Length; ++j)
                {
                    ref Player p2 = ref Players[j];
                    Player.Attack(ref p1, ref p2);
                }
            }

            // handle dead
            for (int i = 0; i < Players.Length; ++i)
            {
                ref Player player = ref Players[i];
                if (player.Dead)
                {
                    player.DeadTime = sw.Elapsed.TotalSeconds;
                    // insert into dead players
                    Array.Resize(ref DeadPlayers, DeadPlayers.Length + 1);
                    DeadPlayers[^1] = player;
                    // remove from players
                    Array.Copy(Players, i + 1, Players, i, Players.Length - i - 1);
                    Array.Resize(ref Players, Players.Length - 1);
                    --i;
                    PlayerIdMap.Remove(player.UserId);
                }
            }

            // handle bonus spawn cooldown
            bonusSpawnTimer += deltaTime;
            while (bonusSpawnTimer > bonusSpawnCooldown)
            {
                if (PickableBonuses.Length < maxBonusCount)
                {
                    AddPickableBonus(new(Random.Shared.NextSingle() * MaxSize.X, Random.Shared.NextSingle() * MaxSize.Y));
                }
                bonusSpawnTimer -= bonusSpawnCooldown;
            }

            // handle player respawn
            for (int i = 0; i < DeadPlayers.Length; ++i)
            {
                ref Player player = ref DeadPlayers[i];
                if (sw.Elapsed.TotalSeconds - player.DeadTime > DeadRespawnTimeInSeconds)
                {
                    // insert into players
                    AddPlayerToRandomPosition(new (player.UserId, player.UserName, player.ConnectionId));
                    // remove from dead players
                    Array.Copy(DeadPlayers, i + 1, DeadPlayers, i, DeadPlayers.Length - i - 1);
                    Array.Resize(ref DeadPlayers, DeadPlayers.Length - 1);
                    --i;
                }
            }

            DispatchMessage();
        }
    }

    private void DispatchMessage()
    {
        PlayerDto[] playerDtos = Players.Select(x => x.ToDto()).ToArray();
        PickableBonusDto[] pickableBonusDtos = PickableBonuses.Select(x => x.ToDto()).ToArray();
        PlayerDto[] deadPlayerDtos = DeadPlayers.Select(x => x.ToDto()).ToArray();
        Hub.Clients.Group(Id.ToString()).Update(playerDtos, pickableBonusDtos, deadPlayerDtos);
    }

    internal void SetPlayerDestination(int userId, float x, float y)
    {
        if (PlayerIdMap.TryGetValue(userId, out int index))
        {
            ref Player player = ref Players[index];
            if (player.UserId == userId)
            {
                player.Destination = new(x, y);
            }
        }
        // else ignore, because dead or not in this lobby
    }
}

public record AddPlayerRequest(int UserId, string UserName, string ConnectionId);