using System.Linq;
using Match3.Engine.Descriptions.Spells;
using Match3.Engine.Levels;

namespace Match3.Engine.InputActions
{
  public class UseSpellInputAction : InputAction
  {
    /// <summary>
    /// идентификатор спела
    /// </summary>
    public int Id;

    /// <summary>
    /// уровень исполльзуемого спела
    /// </summary>
    public int Level;

    /// <summary>
    /// ячейки в которые по очереди тыкнул пользователь
    /// </summary>
    public Point[] Positions;

    /// <summary>
    /// тип использования спела
    /// </summary>
    public UseSpellType Type;

    /// <summary>
    /// тип спелла
    /// </summary>
    public SpellUseType SpellType;

    public override string ToString()
    {
      return string.Format("[{0}][{1}] Id:{2}, Level:{3}, UseType:{4}, Positions:{5}", Tick, GetType().Name, Id, Level, Type, string.Join(",", Positions.Select(p => p.ToString()).ToArray()));
    }
  }
}
