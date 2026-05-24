using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Infrastructure.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;



namespace SamaniCrm.Infrastructure.Connections; 
 

public class SignalRConnectionManager : IConnectionManager
{
    private readonly IHubContext<NotificationHub, INotificationHubService> _hubContext;
    private readonly ILogger<SignalRConnectionManager> _logger;

    // Thread-safe collections
    private readonly ConcurrentDictionary<string, ConnectionInfo> _connections = new();
    private readonly ConcurrentDictionary<string, ImmutableHashSet<string>> _userConnections = new();
    private readonly ConcurrentDictionary<string, DeviceSession> _sessions = new();
    private readonly ConcurrentDictionary<string, string> _connectionToSession = new();
    private readonly ConcurrentDictionary<string, ImmutableHashSet<string>> _groups = new();
    private readonly ConcurrentDictionary<string, string> _connectionToUser = new();

    public SignalRConnectionManager(
        IHubContext<NotificationHub, INotificationHubService> hubContext,
        ILogger<SignalRConnectionManager> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    #region Connection Tracking

    public Task<bool> AddConnectionAsync(string connectionId, IConnectionInfo connectionInfo)
    {
        try
        {
            var info = new ConnectionInfo
            {
                ConnectionId = connectionId,
                UserId = connectionInfo.UserId,
                DeviceId = connectionInfo.DeviceId,
                DeviceType = connectionInfo.DeviceType,
                DeviceName = connectionInfo.DeviceName,
                Browser = connectionInfo.Browser,
                OperatingSystem = connectionInfo.OperatingSystem,
                IpAddress = connectionInfo.IpAddress,
                UserAgent = connectionInfo.UserAgent,
                ConnectedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow,
                IsActive = true,
                Metadata = connectionInfo.Metadata
            };

            // Add to connections dictionary
            if (!_connections.TryAdd(connectionId, info))
            {
                _logger.LogWarning("Connection {ConnectionId} already exists", connectionId);
                return Task.FromResult(false);
            }

            // Add to user's connection list
            _userConnections.AddOrUpdate(
                connectionInfo.UserId,
                _ => ImmutableHashSet.Create(connectionId),
                (_, existingSet) => existingSet.Add(connectionId)
            );

            // Map connection to user
            _connectionToUser.TryAdd(connectionId, connectionInfo.UserId);

            _logger.LogInformation(
                "Connection {ConnectionId} added for user {UserId} on {DeviceType}",
                connectionId, connectionInfo.UserId, connectionInfo.DeviceType);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding connection {ConnectionId}", connectionId);
            return Task.FromResult(false);
        }
    }

    public Task<bool> RemoveConnectionAsync(string connectionId)
    {
        try
        {
            if (!_connections.TryRemove(connectionId, out var connection))
            {
                return Task.FromResult(false);
            }

            // Remove from user's connections
            if (_userConnections.TryGetValue(connection.UserId, out var userConns))
            {
                userConns.Remove(connectionId);
                if (userConns.Count == 0)
                {
                    _userConnections.TryRemove(connection.UserId, out _);
                }
            }

            // Remove session mapping
            if (_connectionToSession.TryRemove(connectionId, out var sessionId))
            {
                if (_sessions.TryGetValue(sessionId, out var session))
                {
                    session.ConnectionIds.Remove(connectionId);
                }
            }

            // Remove user mapping
            _connectionToUser.TryRemove(connectionId, out _);

            _logger.LogInformation(
                "Connection {ConnectionId} removed for user {UserId}",
                connectionId, connection.UserId);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing connection {ConnectionId}", connectionId);
            return Task.FromResult(false);
        }
    }

    public Task UpdateLastActivityAsync(string connectionId)
    {
        if (_connections.TryGetValue(connectionId, out var connection))
        {
            connection.LastActivity = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public IConnectionInfo? GetConnection(string connectionId)
    {
        return _connections.TryGetValue(connectionId, out var connection) ? connection : null;
    }

    public IReadOnlyList<IConnectionInfo> GetUserConnections(string userId)
    {
        if (!_userConnections.TryGetValue(userId, out ImmutableHashSet<string>? connectionIds))
        {
            return Array.Empty<IConnectionInfo>();
        }

        return connectionIds
            .Select(id => _connections.TryGetValue(id, out var conn) ? conn : null)
            .Where(c => c != null)
            .Cast<IConnectionInfo>()
            .ToList();
    }

    public int GetUserConnectionCount(string userId)
    {
        return _userConnections.TryGetValue(userId, out var connections) ? connections.Count : 0;
    }

    public bool IsUserOnline(string userId)
    {
        return _userConnections.TryGetValue(userId, out var connections) && connections.Count > 0;
    }

    public IReadOnlyList<IConnectionInfo> GetAllConnections()
    {
        return _connections.Values.ToList();
    }

    #endregion

    #region Device & Session Management

    public Task<IDeviceSession> CreateSessionAsync(string userId, string deviceId,
        string deviceType, string deviceName, string ipAddress)
    {
        var session = new DeviceSession
        {
            SessionId = Guid.NewGuid().ToString(),
            UserId = userId,
            DeviceId = deviceId,
            DeviceType = deviceType,
            DeviceName = deviceName,
            IpAddress = ipAddress,
            LoginAt = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            IsCurrent = true
        };

        _sessions[session.SessionId] = session;

        _logger.LogInformation(
            "Session {SessionId} created for user {UserId} on {DeviceType} ({DeviceName})",
            session.SessionId, userId, deviceType, deviceName);

        return Task.FromResult<IDeviceSession>(session);
    }

    public IReadOnlyList<IDeviceSession> GetUserSessions(string userId)
    {
        return _sessions.Values
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.LoginAt)
            .ToList();
    }

    public UserSessionSummary GetUserSessionSummary(string userId)
    {
        var sessions = _sessions.Values
            .Where(s => s.UserId == userId)
            .ToList();

        return new UserSessionSummary
        {
            UserId = userId,
            TotalConnections = GetUserConnectionCount(userId),
            ActiveSessions = sessions.Count(s => s.ConnectionIds.Count > 0),
            Sessions = sessions.Select(s => new DeviceSessionInfo
            {
                SessionId = s.SessionId,
                DeviceId = s.DeviceId,
                DeviceType = s.DeviceType,
                DeviceName = s.DeviceName,
                IpAddress = s.IpAddress,
                LoginAt = s.LoginAt,
                LastActivity = s.LastActivity,
                IsCurrent = s.IsCurrent,
                ConnectionCount = s.ConnectionIds.Count
            }).ToList()
        };
    }

    public async Task<bool> TerminateSessionAsync(string userId, string sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var session) || session.UserId != userId)
        {
            return false;
        }

        // Disconnect all connections in this session
        foreach (var connectionId in session.ConnectionIds.ToList())
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("SessionTerminated",
                new { Reason = "Session ended by user", SessionId = sessionId });

            await RemoveConnectionAsync(connectionId);
        }

        _sessions.TryRemove(sessionId, out _);

        _logger.LogInformation("Session {SessionId} terminated for user {UserId}", sessionId, userId);

        return true;
    }

    public async Task<int> TerminateOtherSessionsAsync(string userId, string currentSessionId)
    {
        var sessionsToTerminate = _sessions.Values
            .Where(s => s.UserId == userId && s.SessionId != currentSessionId)
            .ToList();

        foreach (var session in sessionsToTerminate)
        {
            await TerminateSessionAsync(userId, session.SessionId);
        }

        return sessionsToTerminate.Count;
    }

    public async Task<int> TerminateAllUserSessionsAsync(string userId)
    {
        var sessionsToTerminate = _sessions.Values
            .Where(s => s.UserId == userId)
            .ToList();

        foreach (var session in sessionsToTerminate)
        {
            await TerminateSessionAsync(userId, session.SessionId);
        }

        return sessionsToTerminate.Count;
    }

    #endregion

    #region Group Management

    public Task AddToGroupAsync(string connectionId, string groupName)
    {
         _groups.AddOrUpdate(
               groupName,
               _ => ImmutableHashSet.Create(connectionId),
               (_, existingSet) => existingSet.Add(connectionId)
           );


        _logger.LogDebug("Connection {ConnectionId} added to group {GroupName}", connectionId, groupName);

        return Task.CompletedTask;
    }

    public Task RemoveFromGroupAsync(string connectionId, string groupName)
    {
        if (_groups.TryGetValue(groupName, out var group))
        {
            group.Remove(connectionId);
        }

        return Task.CompletedTask;
    }

    public IReadOnlyList<string> GetGroupMembers(string groupName)
    {
        return _groups.TryGetValue(groupName, out var members)
            ? members.ToList()
            : Array.Empty<string>();
    }

    public IReadOnlyList<string> GetAllGroups()
    {
        return _groups.Keys.ToList();
    }

    #endregion

    #region Statistics

    public ConnectionStatistics GetStatistics()
    {
        var stats = new ConnectionStatistics
        {
            TotalConnections = _connections.Count,
            TotalUsers = _userConnections.Count,
            TotalSessions = _sessions.Count,
            TotalGroups = _groups.Count
        };

        // Connections per user
        foreach (var kvp in _userConnections)
        {
            stats.ConnectionsPerUser[kvp.Key] = kvp.Value.Count;
        }

        // Connections by device type
        foreach (var conn in _connections.Values)
        {
            if (stats.ConnectionsByDeviceType.ContainsKey(conn.DeviceType))
                stats.ConnectionsByDeviceType[conn.DeviceType]++;
            else
                stats.ConnectionsByDeviceType[conn.DeviceType] = 1;
        }

        return stats;
    }

    #endregion
}
 