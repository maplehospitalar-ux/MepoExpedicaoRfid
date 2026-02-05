using MepoExpedicaoRfid.Models;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Gerencia o estado da sessão RFID ativa única
/// </summary>
public sealed class SessionStateManager
{
    private readonly object _lock = new();
    private SessionInfo? _currentSession;
    private readonly AppLogger _log;

    public SessionInfo? CurrentSession
    {
        get { lock (_lock) return _currentSession; }
    }

    public bool HasActiveSession => CurrentSession?.Status == SessionStatus.Ativa;

    // Eventos
    public event EventHandler<SessionInfo>? OnSessionStarted;
    public event EventHandler<SessionInfo>? OnSessionPaused;
    public event EventHandler<SessionInfo>? OnSessionResumed;
    public event EventHandler<SessionInfo>? OnSessionEnded;

    public SessionStateManager(AppLogger log)
    {
        _log = log;
    }

    /// <summary>
    /// Inicia uma nova sessão
    /// </summary>
    public bool StartSession(SessionInfo session)
    {
        lock (_lock)
        {
            if (_currentSession != null && _currentSession.Status == SessionStatus.Ativa)
            {
                _log.Warn($"Já existe uma sessão ativa: {_currentSession.SessionId}");
                return false;
            }

            session.Status = SessionStatus.Ativa;
            session.IniciadaEm = DateTime.UtcNow;
            _currentSession = session;

            _log.Info($"Sessão iniciada: {session.SessionId}");
            OnSessionStarted?.Invoke(this, session);
            return true;
        }
    }

    /// <summary>
    /// Pausa a sessão atual
    /// </summary>
    public bool PauseCurrentSession()
    {
        lock (_lock)
        {
            if (_currentSession == null || _currentSession.Status != SessionStatus.Ativa)
            {
                return false;
            }

            _currentSession.Status = SessionStatus.Pausada;
            _log.Info($"Sessão pausada: {_currentSession.SessionId}");
            OnSessionPaused?.Invoke(this, _currentSession);
            return true;
        }
    }

    /// <summary>
    /// Retoma a sessão pausada
    /// </summary>
    public bool ResumeCurrentSession()
    {
        lock (_lock)
        {
            if (_currentSession == null || _currentSession.Status != SessionStatus.Pausada)
            {
                return false;
            }

            _currentSession.Status = SessionStatus.Ativa;
            _log.Info($"Sessão retomada: {_currentSession.SessionId}");
            OnSessionResumed?.Invoke(this, _currentSession);
            return true;
        }
    }

    /// <summary>
    /// Finaliza a sessão atual
    /// </summary>
    public bool EndSession(SessionStatus finalStatus = SessionStatus.Finalizada)
    {
        lock (_lock)
        {
            if (_currentSession == null)
            {
                return false;
            }

            _currentSession.Status = finalStatus;
            _currentSession.FinalizadaEm = DateTime.UtcNow;

            var endedSession = _currentSession;
            _currentSession = null;

            _log.Info($"Sessão finalizada: {endedSession.SessionId} - Status: {finalStatus}");
            OnSessionEnded?.Invoke(this, endedSession);
            return true;
        }
    }

    /// <summary>
    /// Cancela a sessão atual
    /// </summary>
    public bool CancelSession(string? motivo = null)
    {
        _log.Info($"Sessão cancelada. Motivo: {motivo}");
        return EndSession(SessionStatus.Cancelada);
    }

    /// <summary>
    /// Atualiza contadores da sessão
    /// </summary>
    public void UpdateTagCounts(int total, int validas, int invalidas)
    {
        lock (_lock)
        {
            if (_currentSession != null)
            {
                _currentSession.TotalTagsLidas = total;
                _currentSession.TotalTagsValidas = validas;
                _currentSession.TotalTagsInvalidas = invalidas;
            }
        }
    }

    /// <summary>
    /// Incrementa contador de tags
    /// </summary>
    public void IncrementTagCount(bool isValida)
    {
        lock (_lock)
        {
            if (_currentSession != null)
            {
                _currentSession.TotalTagsLidas++;
                if (isValida)
                    _currentSession.TotalTagsValidas++;
                else
                    _currentSession.TotalTagsInvalidas++;
            }
        }
    }
}
