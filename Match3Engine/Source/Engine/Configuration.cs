using System.Collections.Generic;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.InputActions;
using Match3.Engine.Levels;
using Match3.Engine.Providers;

namespace Match3.Engine
{
  /// <summary>
  /// конфигурация
  /// </summary>
  public class Configuration
  {
    /// <summary>
    /// провайдеры данных и обработчиков (общие)
    /// </summary>
    public IEngineProviders Providers;

    /// <summary>
    /// к-во энергии пользоватиля на начало игры
    /// </summary>
    public int Energy;

    /// <summary>
    /// максимальное к-во тиков игры (перемещения и спелы) (для отладки)
    /// </summary>
    public int MaxTicks;

    /// <summary>
    /// действия, которые определены наперед
    /// </summary>
    public List<InputAction> Actions;

    /// <summary>
    /// тик на который нужно сразу перевести игру
    /// </summary>
    public int Tick;

    /// <summary>
    /// список доступных спелов на начало игры
    /// </summary>
    public Spell[] Spells;

    /// <summary>
    /// уровень
    /// </summary>
    public LevelDescription LevelDescription;

    /// <summary>
    /// конструктор конфигурации
    /// </summary>
    /// <param name="environment">окружение (Client|Server...)</param>
    public Configuration(EngineEnvironment environment)
    {
      Environment = environment;
    }

    /// <summary>
    /// окружение
    /// </summary>
    public EngineEnvironment Environment { get; private set; }
  }
}
