using System.Collections.Generic;
using Match3.Engine.Descriptions.Spells;
using Match3.Engine.Providers;
using Match3.Engine.Spells;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedSpellTypeActionProvider : ISpellTypeActionsProvider
  {
    private readonly Dictionary<SpellType, SpellTypeAction> _map;

    public SharedSpellTypeActionProvider()
    {
      _map = new Dictionary<SpellType, SpellTypeAction>
      {
        { SpellType.ChangeItem,      new ChangeItemSpellTypeAction() },
        { SpellType.MakeBonusItem,   new MakeBonusItemSpellTypeAction() },
        { SpellType.DestroySomeItem, new DestroySomeItemSpellTypeAction() },
        { SpellType.RandomDestory,   new RandomDestorySpellTypeAction() },
        { SpellType.Splash,          new SplashSpellTypeAction() },
        { SpellType.ChainDestroy,    new ChainDestroySpellTypeAction() },
        { SpellType.SwapItem,        new SwapItemSpellTypeAction() },
        { SpellType.DiagonalSplash,  new DiagonalSplashSpellTypeAction() },

        { SpellType.DestroyCenterItemAsStart, new DestroyCenterItemAsStartSpellTypeAction() },
        { SpellType.DestoryVerticalLineThroughOne,   new DestoryVerticalLineThroughOneSpellTypeAction() },
        { SpellType.DestoryHorizontalLineThroughOne, new DestoryHorizontalLineThroughOneSpellTypeAction() },
        { SpellType.DestroyGridDiagonalLines,        new DestroyGridDiagonalLinesSpellTypeAction() },
        { SpellType.RandomChangeItemLevel,           new RandomChangeItemLevelSpellTypeAction() },
        { SpellType.SelectAndChangeItemsBySelected,  new SelectAndChangeItemsBySelectedSpellTypeAction() },
        { SpellType.DestoryAllByItemId,              new DestoryAllByItemIdSpellTypeAction() },
        { SpellType.SplashByValue,                   new SplashByValueSpellTypeAction() },
        { SpellType.RandomSplashByValue,             new RandomSplashByValueSpellTypeAction() },
        { SpellType.RandomMakeBonusItemAndActivate,  new RandomMakeBonusItemAndActivateSpellTypeAction() },
        { SpellType.DestroySelectedVerticalLine,     new DestroySelectedVerticalLineSpellTypeAction() },
        { SpellType.DestroySelectedHorizontalLine,   new DestroySelectedHorizontalLineSpellTypeAction() },

      };
    }

    public SpellTypeAction Get(SpellType spellType)
    {
      return _map[spellType];
    }
  }
}
