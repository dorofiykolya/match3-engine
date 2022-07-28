using System;

namespace Match3.LevelConverter.MagicCrush
{
  [Serializable]
  public class MCMatrixInfo
  {
    public int[][] matrixCellIndexes;
    public int[][] matrixSymbolIndexes;
    public int[][] matrixSymbolLevels;
    public int[][] lockedSymbolsIndexes;
    public int[][] backbroundBodrersIndexes;
    public int[][] portalInIndexes;
    public int[][] portalOutIndexes;
    public int[][] backgroundLockedCellIndexes;
    public int[][] boxIndexes;
    public int[][] additionalMatrix;
  }
}
