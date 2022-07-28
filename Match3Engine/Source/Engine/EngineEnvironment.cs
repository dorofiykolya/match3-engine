using System;

namespace Match3.Engine
{
  [Flags]
  public enum EngineEnvironment
  {
    /// <summary>
    /// включает отладочный код
    /// </summary>
    Debug = 1 << 0,

    /// <summary>
    /// включает релизный код
    /// </summary>
    Release = 1 << 1,

    /// <summary>
    /// включает код для клиента
    /// </summary>
    Client = 1 << 2,

    /// <summary>
    /// включает код для сервера
    /// </summary>
    Server = 1 << 3,

    /// <summary>
    /// пропустить проверки на правильность входных данных
    /// </summary>
    SkipDataCheck = 1 << 4,

    /// <summary>
    /// расширенный алгоритм доступных ходов
    /// </summary>
    [Obsolete]
    AdvanceAlgorithmCheckAvailableSwaps = 1 << 5,

    /// <summary>
    /// анализ требуемых шагов (требуется для тестирования и анализатора на специальные условия)
    /// </summary>
    AnalysisOfRequiredSwaps = 1 << 6,

    /// <summary>
    /// активировать мидификации вокруг ячейки
    /// </summary>
    [Obsolete]
    ActivateNearTileModifier = 1 << 7,

    /// <summary>
    /// доступны все спелы (пропустить проверку на наличие спела)
    /// </summary>
    AvailableAllSpells = 1 << 8,

    /// /// <summary>
    /// спелы не расходуют энергию
    /// </summary>
    ForceSkipUseSpellEnergy = 1 << 9,

    /// <summary>
    /// спел, который требуется для прохождения уровня не требует энергии
    /// </summary>
    SkipRequiredSpellEnergy = 1 << 10,

    /// <summary>
    /// Нужно ли генерировать выходные ивенты
    /// </summary>
    GenerateOutputEvents = 1 << 11,

    DefaultClient = Client | ActivateNearTileModifier | SkipRequiredSpellEnergy | GenerateOutputEvents,
    DefaultServer = Server | ActivateNearTileModifier | SkipRequiredSpellEnergy,
    DefaultDebugClient = Debug | Client | ActivateNearTileModifier | SkipRequiredSpellEnergy | GenerateOutputEvents,
    DefaultDebugServer = Debug | Server | ActivateNearTileModifier | SkipRequiredSpellEnergy,
  }

  public static class EngineEnvironmentExtension
  {
    public static bool IsDebug(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.Debug) == EngineEnvironment.Debug;
    }

    public static bool IsRelease(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.Release) == EngineEnvironment.Release;
    }

    public static bool IsAvailableAllSpells(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.AvailableAllSpells) == EngineEnvironment.AvailableAllSpells;
    }

    public static bool IsForceSkipUseSpellEnergy(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.ForceSkipUseSpellEnergy) == EngineEnvironment.ForceSkipUseSpellEnergy;
    }

    public static bool IsSkipRequiredSpellEnergy(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.SkipRequiredSpellEnergy) == EngineEnvironment.SkipRequiredSpellEnergy;
    }

    public static bool IsClient(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.Client) == EngineEnvironment.Client;
    }

    public static bool IsGenerateOutputEvents(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.Client) == EngineEnvironment.Client;
    }

    public static bool IsServer(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.Server) == EngineEnvironment.Server;
    }

    public static bool IsSkipDataCheck(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.SkipDataCheck) == EngineEnvironment.SkipDataCheck;
    }

    [Obsolete]
    public static bool IsActivateNearTileModifier(this EngineEnvironment environment)
    {
      return (environment & EngineEnvironment.ActivateNearTileModifier) == EngineEnvironment.ActivateNearTileModifier;
    }
  }
}
