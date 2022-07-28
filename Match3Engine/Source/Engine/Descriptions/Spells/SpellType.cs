﻿namespace Match3.Engine.Descriptions.Spells
{

  /// <summary>
  /// Набор состоит из шести заклинаний, каждое из которых делится на шесть уровней в зависимости от уровня одетых (обычный, необычный, легендарный и пр) на героя вещей. Каждый из предметов отвечает за свой тип заклинания.
  /// </summary>
  [System.Serializable]
  public enum SpellType
  {
    /// <summary>
    /// Шлем – перекрашивание камней. Некоторое количество камней меняет свой цвет на какой-то определённо выбранный.
    /// </summary>
    ChangeItem = 0,

    /// <summary>
    /// Кираса – создание бонусов. Несколько камней становятся бонусными.
    /// </summary>
    MakeBonusItem = 1,

    /// <summary>
    /// Сапоги – уничтожает камни выбранного цвета. Целью выбирается камень. Затем начиная от 12 часов по часовой стрелки начинают искать камни того же цвета и уничтожаться, пока не достигнут лимита заклинаний. С уровнем растёт количество камней, которые будут уничтожены.
    /// </summary>
    DestroySomeItem = 2,

    /// <summary>
    /// Оружие – точечно рандомно разбивает несколько камней на поле. От уровня растёт количество разбиваемых камней.
    /// </summary>
    RandomDestory = 3,

    /// <summary>
    /// Щит – разбивает камни по определённой площади. Площадь поражения растёт от уровня.
    /// </summary>
    Splash = 4,

    /// <summary>
    /// Амулет – «Цепная молния». Количество камней, который разбиваются по цепочке (игрок выбирает начальный, молния прыгает на соседние камни или через один)
    /// </summary>
    ChainDestroy = 5,

    /// <summary>
    /// меняет выбранные ячейки местами
    /// </summary>
    SwapItem = 6,

    /// <summary>
    /// рандомно преобразует камни первого уровня во второй и взрывает их
    /// </summary>
    RandomMakeBonusItemAndActivate = 7,

    /// <summary>
    /// выжигает выбранный вертикальный ряд
    /// </summary>
    DestroySelectedVerticalLine = 8,

    /// <summary>
    /// выжигает выбранный горизонтальный ряд
    /// </summary>
    DestroySelectedHorizontalLine = 9,

    /// <summary>
    /// уничтожает выбранный цвет
    /// </summary>
    DestoryAllByItemId = 10,

    /// <summary>
    /// выбирается камень и от него по диагонали на все 4 стороны выжигая
    /// </summary>
    DiagonalSplash = 11,

    /// <summary>
    /// превращает в камни любого уровня
    /// </summary>
    RandomChangeItemLevel = 12,

    /// <summary>
    /// выбирает любой камень на поле, затем он взрывается из него как бы вылетает жидкость и он рандомно перекрашивает от 5 до 7 камней на поле в цвет камня который взорвался
    /// </summary>
    SelectAndChangeItemsBySelected = 13,

    /// <summary>
    /// выбираем центральный камень поля и взрывает 8 камней (с центральным 9) в форме звезды
    /// </summary>
    DestroyCenterItemAsStart = 14,

    /// <summary>
    /// выжигает два ряда наискосок (от крайних угловых камней)
    /// </summary>
    DestroyGridDiagonalLines = 15,

    /// <summary>
    /// уничтожает по одному вертикальному ряду через один
    /// </summary>
    DestoryVerticalLineThroughOne = 16,

    /// <summary>
    /// уничтожает по одному горизонтальному ряду через один
    /// </summary>
    DestoryHorizontalLineThroughOne = 17,

    /// <summary>
    /// уничтожает любой выбранный камень на поле и по камню вокруг него
    /// </summary>
    SplashByValue = 18,
    
    /// <summary>
    /// рандомно уничтожает любой камень на поле и по камню вокруг него
    /// </summary>
    RandomSplashByValue = 19
  }
}
