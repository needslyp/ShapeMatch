using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "Game/Shape Data")]
public class ShapeData : ScriptableObject {
    public Sprite shapeSprite;
    public Sprite animalSprite;
    public SpecialType specialType;
}

public enum SpecialType { Normal, Heavy, Sticky, Frozen }