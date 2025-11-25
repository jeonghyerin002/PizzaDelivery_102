using UnityEngine;

[CreateAssetMenu(fileName = "New Badge")]
public class BadgeData : ScriptableObject
{
    public int badgeID;
    public string badgeName;
    public Sprite badgeIcon;
    public bool privateBadge = false;
    [TextArea]
    public string badgeDescription;
}