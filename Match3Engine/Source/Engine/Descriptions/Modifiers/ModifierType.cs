﻿namespace Match3.Engine.Descriptions.Modifiers
{
  /// <summary>
  /// тип модификатора
  /// </summary>
  [System.Serializable]
  public enum ModifierType
  {
    /// <summary>
    /// нет эффекта
    /// </summary>
    None,

    /// <summary>
    /// Броня – элемент, который находится на верхнем слое объектов. Имеет несколько уровней прочности, которые должны отображаться разным вижуалом (максимум 8). Игрок не может взаимодействовать с элементами (камни и бонусы), находящимися под бронёй. И эти элементы не могут быть разрушены или перемещены (в том числе не могут упасть ниже, если там есть свободное место). Однако если камень находящейся под бронёй становится частью цепочки – цепочка считается составленной и все элементы входящие в неё уничтожаются, кроме камней под бронёй. Вместо этого броня теряет одну единицу прочности. Кроме того броня теряет по единице прочности каждый раз, когда собранная цепочка прилегает к тайлу с бронёй. Либо тайл с бронёй получает удар от заклинания или сработавшего бонуса на поле.
    /// </summary>
    Armor,

    /// <summary>
    /// «Подложка» - элемент на нижнем слое. Никак не взаимодействует с другими окружающими элементами, не мешает перемещениям и т.д. Их уничтожение обычно является частью задания. Имеют хитпоинты до восьми. Каждый хитпоинт имеет свой вижуал. Когда хитпоинты достигают нулья – сущность удаляется. Хитпоинты снимаются когда по какой-либо причине уничтожается камень находящийся в том же поле.
    /// </summary>
    Substrate,

    /// <summary>
    /// "Коробка" - итем не падает внутырь
    /// </summary>
    Box
  }
}